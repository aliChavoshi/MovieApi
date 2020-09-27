namespace MoviesApi.DTOs
{
    public class FilterMoviesDto
    {
        public int Page { get; set; } = 1;
        public int RecorsPerPage { get; set; } = 10;

        public PaginationDto Pagination => new PaginationDto { Page = Page, RecordsPerPage = RecorsPerPage };


        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value.ToLower();
        }


        public int GenreId { get; set; }
        public bool InTheaters { get; set; }
        public bool UpcomingReleases { get; set; }

        public string OrderingField { get; set; }
        public bool AscendingOrder { get; set; } = true;

    }
}