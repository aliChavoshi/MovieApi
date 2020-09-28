using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CustomBaseController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CustomBaseController(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        //Genre
        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            //From Db
            var entity = await _context.Set<TEntity>().ToListAsync();
            //Dtos
            var dtos = _mapper.Map<List<TDTO>>(entity);
            return dtos;
        }

        //People Pagination
        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDto pagination) where TEntity : class
        {
            IQueryable<TEntity> queryble = _context.Set<TEntity>();

            //Insert Count Page To Header
            await HttpContext.InsertPaginationParametersInResponse(queryable: queryble, pagination.RecordsPerPage);

            //Insert Take Skip
            var entity = await queryble.Paginate(pagination).ToListAsync();

            //Create New Map List<PersonDtos>
            var dtos = _mapper.Map<List<TDTO>>(entity);

            return dtos;
        }

        //Get By Id
        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var queryable = _context.Set<TEntity>().AsQueryable();
            return await Get<TEntity, TDTO>(id, queryable);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id, IQueryable<TEntity> queryable) where TEntity : class, IId
        {
            var entity = await queryable.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<TDTO>(entity);

            return dto;
        }


        protected async Task<ActionResult> Post<TEntity, TCreation, TRead>(TCreation creation, string routeName) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(creation);
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            //reverse map
            //در نهایت مقدار اضافه شده را باید به مدل خودم مپ کنم 
            var readDto = _mapper.Map<TRead>(entity);

            //back to action
            //api/Genre/id
            return new CreatedAtRouteResult(routeName, new { id = entity.Id }, readDto);
        }

        protected async Task<IActionResult> Put<TEntity, TDTO>(int id, TDTO dto) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(dto);
            entity.Id = id;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            //retuen 204
            return Ok(entity);
        }

        protected async Task<IActionResult> Delete<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                //return 404
                return NotFound();
            }

            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<TDTO>(entity);
            return Ok(dto);
        }

        protected async Task<ActionResult> Patch<TEntity, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument) where TDTO : class
            where TEntity : class, IId
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            //Get Data From DB
            var entityFromDb = await _context.Set<TEntity>().FindAsync(id);
            if (entityFromDb == null)
            {
                return NotFound();
            }

            //ابتدا مدل دیتا بیس را با مدل پچ مپ میکنم و مقادیر دیتا بیس را داخل یک کتغیر میریزم
            //به این دلیل این کار را کردم که بتوانم مدل دیتابیس را به مدل ورودی نزدیک کنم
            var dto = _mapper.Map<TDTO>(source: entityFromDb);

            //حالا مدل ورودی را به مدل دیتا بیس باید اپلای کنم
            patchDocument.ApplyTo(dto, modelState: ModelState);
            //الان همه چیز داخل entityDTO  میباشد 
            //شاید کاربر مقادیر نال ارسال کرده باشه 
            var isValid = TryValidateModel(dto);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            //حالا باید مدلی که ویرایش کردیم را به مدل دیتابیس ارسال کنیم تا آپدیت بشه
            _mapper.Map(source: dto, destination: entityFromDb);

            //Save
            _context.Update(entityFromDb);
            await _context.SaveChangesAsync();
            return Ok(entityFromDb);
        }
    }
}
