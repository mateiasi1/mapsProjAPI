using mapsProjAPI.Data;
using mapsProjAPI.DTOs;
using mapsProjAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace mapsProjAPI.Controllers
{

    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IAuthManager _authManager;
        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/[controller]/SendVerificationMessage")]
        public IActionResult SendVerificationMessage([FromBody] LoginDTO loginDTO)
        {
            IActionResult response = Unauthorized();

            if (loginDTO == null)
            {
                return BadRequest();
            }
            var isEmailSent = _authManager.SendVerificationMessage(loginDTO);
            if (!isEmailSent)
            {
                return BadRequest();
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/[controller]/Login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO) {
            if (loginDTO == null)
            {
                return BadRequest();
            }
            var loginResponse = _authManager.Login(loginDTO);
            if (loginResponse == null)
            {
                return Unauthorized();
            }
            
            return Ok(loginResponse);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/getUser")]
        public IActionResult GetUser()
        {
            string authHeader = Request.Headers["Authorization"];
            
            string token = authHeader.Substring("Bearer ".Length);
            var loginResponse = _authManager.GetUser(token);
            if (loginResponse == null)
            {
                return Unauthorized();
            }

            return Ok(loginResponse);
        }
    }
}
