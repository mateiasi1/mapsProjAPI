using mapsProjAPI.Models;

namespace mapsProjAPI.DTOs.Response
{
    public class LoginDTOResponse
    {
        public User User { get; set; }
        public DateTime ActiveUntil { get; set; }
        public string Token { get; set; }
    }
}
