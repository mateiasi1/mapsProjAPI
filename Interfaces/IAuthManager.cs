using mapsProjAPI.DTOs;

namespace mapsProjAPI.Interfaces
{
    public interface IAuthManager
    {
        bool SendVerificationMessage(LoginDTO loginDTO);
        string Login(LoginDTO loginDTO);
    }
}
