using mapsProjAPI.Enums;

namespace mapsProjAPI.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateLastLogin { get; set; }
        public UserRoles UserRole { get; set; }


    }
}
