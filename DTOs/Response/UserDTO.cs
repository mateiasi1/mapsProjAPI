using mapsProjAPI.Enums;

namespace mapsProjAPI.DTOs.Response
{
    public class UserDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public UserRoles UserRole { get; set; }
    }
}
