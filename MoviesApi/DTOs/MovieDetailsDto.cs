using System.Collections.Generic;

namespace MoviesApi.DTOs
{
    public class MovieDetailsDto : MovieDto
    {
        public List<GenreDtOs> Genres { get; set; }
        public List<ActorDto> Actors { get; set; }

    }
}