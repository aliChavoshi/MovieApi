using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MoviesApi.DTOs
{
    public class FilterMovieTheathersDto
    {
        [BindRequired]
        [Range(-90, 90)]
        public double Lat { get; set; }

        [BindRequired]
        [Range(-180, 180)]
        public double Long { get; set; }

        private int _distanceInKms = 10;
        private readonly int _maxDistanceInKms = 50;

        public int DistanceInKms
        {
            get => _distanceInKms;
            set => _distanceInKms = (value > _maxDistanceInKms) ? _maxDistanceInKms : value;
        }
    }
}