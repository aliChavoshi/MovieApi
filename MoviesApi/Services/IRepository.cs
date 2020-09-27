using System.Collections.Generic;
using MoviesApi.Entities;

namespace MoviesApi.Services
{
    public interface IRepository
    {
        List<Genre> GetAllGenre();

        Genre GetById(int id);

        void AddGenre(Genre genre);
    }
}