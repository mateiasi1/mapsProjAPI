using mapsProjAPI.DTOs;
using mapsProjAPI.DTOs.Response;

namespace mapsProjAPI.Interfaces
{
    public interface IAuthManager
    {
        bool SendVerificationMessage(LoginDTO loginDTO);
        LoginDTOResponse Login(LoginDTO loginDTO);
        UserDto GetUser(string token);
    }
}
