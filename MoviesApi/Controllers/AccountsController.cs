using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }



        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="model">Model For Create User</param>
        /// <returns></returns>
        [HttpPost("Create",Name = "CreateUser")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
            //Create new User by identityUser
            var user = new IdentityUser { UserName = model.EmailAddress, Email = model.EmailAddress };
            //for create user use _usermanager In Create 
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //retuen user by token
                return await BuildToken(model);
            }
            //user not login
            return BadRequest(result.Errors);
        }



        [HttpPost("Login",Name = "Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            //for login user use _signInManager
            //isPersistent : پایدار
            //lockoutOnFailure : شکست قفل
            var result =
                await _signInManager.PasswordSignInAsync(userName: userInfo.EmailAddress, password: userInfo.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(userInfo);
            }
            else
            {
                return BadRequest("Invalid login attempt");
            }
        }


        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> Get([FromQuery] PaginationDto paginationDto)
        {
            var queryable = _context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDto.RecordsPerPage);

            var users = await queryable.Paginate(paginationDto).ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        [HttpGet("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _context.Roles.Select(x => x.Name).ToListAsync());
        }



        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] EditRoleDto editRoleDto)
        {
            IdentityUser user = await _userManager.FindByIdAsync(editRoleDto.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.AddClaimAsync(user: user, claim: new Claim(type: ClaimTypes.Role, value: editRoleDto.RoleName));
            return Ok(user);
        }


        [HttpPost("RemoveRole")]
        public async Task<IActionResult> RemoveRole(EditRoleDto editRoleDto)
        {
            IdentityUser user = await _userManager.FindByIdAsync(editRoleDto.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveClaimAsync(user: user, claim: new Claim(type: ClaimTypes.Role, value: editRoleDto.RoleName));
            return NoContent();
        }



        [HttpPost("RenewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renew()
        {
            var userInfo = new UserInfo
            {
                EmailAddress = HttpContext.User.Identity.Name
            };

            return await BuildToken(userInfo);
        }



        //BuildToken
        private async Task<UserToken> BuildToken(UserInfo userInfo)
        {
            //save in claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userInfo.EmailAddress),
                new Claim(ClaimTypes.Email,userInfo.EmailAddress),
                new Claim("myKey","whatever value I want")
            };

            //get user
            var identityUser = await _userManager.FindByEmailAsync(userInfo.EmailAddress);
            //get claims for user
            var claimsDb = await _userManager.GetClaimsAsync(identityUser);
            //add to claims
            claims.AddRange(claimsDb);

            //In Appsetting.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"]));
            //Credentials
            var creds = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);
            //Create Token
            JwtSecurityToken token = new JwtSecurityToken(
                //صادر کننده
                issuer: null,
                //حضار
                audience: null,
                //ادعاها
                claims: claims,
                //انقضا
                expires: expiration,
                //امضای اعتبار نامه
                signingCredentials: creds
                );

            return new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
