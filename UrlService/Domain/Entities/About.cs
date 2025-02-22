namespace UrlService.Domain.Entities
{
    public class About : BaseEntity
    {
        public string Content { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
