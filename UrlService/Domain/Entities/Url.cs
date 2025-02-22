namespace UrlService.Domain.Entities
{
    public class Url : BaseEntity
    {
        public string OriginalUrl { get; set; }
        public string ShortUrl { get; set; }
        public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}