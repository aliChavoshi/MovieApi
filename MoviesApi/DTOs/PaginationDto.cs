namespace MoviesApi.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;

        private int _recordsPerPage = 10;
        private readonly int _maxRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get => _recordsPerPage;
            //اگر کاربر از 50 تا بیشتر زد همان 50 را دوباره بهش بده در غیر این صورت همان عدد خوودش را بهش بده
            set => _recordsPerPage = (value > _maxRecordsPerPage) ? _maxRecordsPerPage : value;
        }
    }
}