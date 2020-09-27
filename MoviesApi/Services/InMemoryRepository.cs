using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using MoviesApi.Entities;

namespace MoviesApi.Services
{
    public class InMemoryRepository : IRepository
    {
        private readonly List<Genre> _genres;

        public InMemoryRepository()
        {
            _genres = new List<Genre>
            {
                new Genre{Id = 1,Name = "Ali"},
                new Genre{Id = 2,Name = "Mahshad"}
            };
        }


        public List<Genre> GetAllGenre()
        {
            return _genres;
        }

        public Genre GetById(int id)
        {
            return _genres.SingleOrDefault(x => x.Id == id);
        }

        public void AddGenre(Genre genre)
        {
            var id = _genres.Max(x => x.Id);
            genre.Id = id + 1;
            _genres.Add(genre);
        }
    }
}