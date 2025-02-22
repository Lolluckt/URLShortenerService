namespace UrlService.Application.DTOs
{
    public class UrlDto
    {
        public int Id { get; set; }

        public string OriginalUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByEmail { get; set; }
    }
}