using mapsProjAPI.Data;
using mapsProjAPI.DTOs;
using mapsProjAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace mapsProjAPI.Controllers
{
    
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context;
        public AuthController(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/[controller]/SendVerificationMessage")]
        public IActionResult SendVerificationMessage([FromBody] LoginDTO loginDTO)
        {
            IActionResult response = Unauthorized();

            if (loginDTO != null)
            {
                var accountSid = _configuration["Twilio:AccountSID"];
                var authToken = _configuration["Twilio:AuthToken"];
                TwilioClient.Init(accountSid, authToken);

                Random random = new Random();
                int newlyCreatedOTP = random.Next(100000, 1000000);

                var messageOptions = new CreateMessageOptions(
                    //new PhoneNumber("+40761559101"));
                    new PhoneNumber(loginDTO.PhoneNumber));
                messageOptions.From = new PhoneNumber(_configuration["Twilio:PhoneNumber"]);
                messageOptions.MessagingServiceSid = _configuration["Twilio:MessagingServiceSid"];
                messageOptions.Body = "This is your account verification code: " + newlyCreatedOTP;
                messageOptions.ProvideFeedback = true;

                LoginAttempt loginAttempt = new LoginAttempt();

                var message = MessageResource.Create(messageOptions);
                if (message.ErrorMessage == null || message.ErrorCode == null)
                {
                    loginAttempt.PhoneNumber = loginDTO.PhoneNumber;
                    loginAttempt.VerificationCode = newlyCreatedOTP;
                    loginAttempt.Description = messageOptions.Body;
                    loginAttempt.DateCreated = DateTime.UtcNow;
                    loginAttempt.ValidUntill = DateTime.UtcNow.AddMinutes(10);
                    _context.LoginAttempts.Add(loginAttempt);
                    var user = _context.Users.Where(u => u.PhoneNumber == loginDTO.PhoneNumber).FirstOrDefault();
                    if (user == null)
                    {
                        var newUser = new User
                        {
                            PhoneNumber = loginDTO.PhoneNumber,
                            UserRole = Enums.UserRoles.User,
                        };
                        _context.Users.Add(newUser);
                    }
                    _context.SaveChanges();
                    return Ok();
                }
                loginAttempt.Description = "Error Code: " + message.ErrorCode + " Message Error: " + message.ErrorMessage ;
                return response;
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/[controller]/login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO) {
            IActionResult response = Unauthorized();
            var user = _context.Users.Where(u => u.PhoneNumber == loginDTO.PhoneNumber).FirstOrDefault();
            if (user==null)
            {
                return response;
            }
            var attempt = _context.LoginAttempts.Where(la => la.PhoneNumber == loginDTO.PhoneNumber).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (attempt==null || attempt.AttemptCounter >=3 || attempt.ValidUntill < DateTime.UtcNow)
            {
                return response;
            }
            if (loginDTO != null )
            {
                if (attempt.VerificationCode != loginDTO.VerificationCode)
                { var errorMessage = "Codul nu este valid te rugam sa reincerci! Atentie codul poate fi grsesit de maxim 3 ori si este valid doar 10 minute!";
                    attempt.AttemptCounter += 1;
                    attempt.Description = errorMessage;
                    _context.SaveChanges();
                    return Unauthorized(errorMessage);
                }
                //get the user from DB based on the phoneNumber
                //var user = new User
                //{
                //    Id = new Guid(),
                //    DateLastLogin = DateTime.UtcNow,
                //    Name = "Gicu",
                //    PhoneNumber = "+40761559101",
                //    UserRole = Enums.UserRoles.PowerUser
                //};

                if (user.PhoneNumber == loginDTO.PhoneNumber)
                {


                    var issuer = _configuration["JWT:Issuer"];
                    var audience = _configuration["JWT:Audience"];
                    var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                    var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

                    var subject = new ClaimsIdentity(new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, user.PhoneNumber),
                });

                    var expires = DateTime.UtcNow.AddMinutes(10);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = subject,
                        Expires = expires,
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = signingCredentials
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);
                    user.DateLastLogin = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Ok(jwtToken);
                }
            }
            return response;
        }
    }
}
