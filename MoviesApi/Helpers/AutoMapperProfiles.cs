using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesApi.DTOs;
using MoviesApi.Entities;

namespace MoviesApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Genre

            CreateMap<Genre, GenreDtOs>().ReverseMap();

            CreateMap<GenreCreationDto, Genre>();

            #endregion

            #region Person

            CreateMap<Person, PersonDtos>().ReverseMap();

            CreateMap<PersonCreationDTOs, Person>()
                //بهش میگم نیازی نیست مقدار را خودم در کنترلر بهش اضافه میکنم
                .ForMember(x => x.Picture, c => c.Ignore());

            CreateMap<Person, PersonPatchDtos>().ReverseMap();

            #endregion

            #region Movie

            CreateMap<Movie, MovieDto>();
            //پوستر را خودم میخواهم به صورت دستی درست کنم لذا ایگنورش کردم
            //از سمت چپ به سمت راست میریزد
            CreateMap<MovieCreationDtos, Movie>()
                .ForMember(x => x.Poster, c => c.Ignore())
                .ForMember(x => x.MoviesGenres, c => c.MapFrom(MapMovieGenres))
                .ForMember(x => x.MoviesActors, c => c.MapFrom(MapMovieActors));

            //چیز های لازمه را باید از سمت چپ بریزم داخل سمت راستی
            CreateMap<Movie, MovieDetailsDto>()
                .ForMember(x => x.Actors, c => c.MapFrom(MapMovieActors))
                .ForMember(x => x.Genres, c => c.MapFrom(MapMovieGenres));


            CreateMap<Movie, MoviePatchDtos>().ReverseMap();

            #endregion

            #region Identity

            CreateMap<IdentityUser, UserDto>()
                .ForMember(x => x.EmailAddress, c => c.MapFrom(d => d.Email))
                .ForMember(x => x.UserId, c => c.MapFrom(d => d.Id));


            #endregion
        }


        private static List<ActorDto> MapMovieActors(Movie movie, MovieDetailsDto movieDetailsDto)
        {
            return movie.MoviesActors
                .Select(x => new ActorDto
                {
                    PersonName = x.Person.Name,
                    Character = x.Character,
                    PersonId = x.PersonId
                }).ToList();
        }
        private static List<MoviesActors> MapMovieActors(MovieCreationDtos movieCreationDtos, Movie movie)
        {
            return movieCreationDtos.Actors
                .Select(x => new MoviesActors
                {
                    Character = x.Character,
                    PersonId = x.PersonId
                }).ToList();
        }

        //جنس ورودی با مپ بالا باید یکسان باشد
        private static List<MoviesGenres> MapMovieGenres(MovieCreationDtos movieCreationDtos, Movie movie)
        {
            return movieCreationDtos.GenresIds
                .Select(id => new MoviesGenres
                {
                    GenreId = id
                }).ToList();
        }
        private static List<GenreDtOs> MapMovieGenres(Movie movie, MovieDetailsDto movieCreationDtos)
        {
            return movie.MoviesGenres
                .Select(x => new GenreDtOs
                {
                    Id = x.GenreId,
                    Name = x.Genre.Name
                }).ToList();
        }
        //جنس ورودی باید با مپ بالا یکسان باشد

    }
}