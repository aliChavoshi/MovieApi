using System.Collections.Generic;

namespace MoviesApi.DTOs
{
    public class ResourceCollection<T>
    {
        public List<T> Value { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public ResourceCollection(List<T> value)
        {
            Value = value;
        }
    }
}