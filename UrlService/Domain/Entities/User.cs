using UrlService.Domain.Roles; 

namespace UrlService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public ICollection<Url> Urls { get; set; }
    }
}