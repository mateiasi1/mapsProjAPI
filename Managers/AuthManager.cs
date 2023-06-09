﻿using mapsProjAPI.Constants;
using mapsProjAPI.Data;
using mapsProjAPI.DTOs;
using mapsProjAPI.DTOs.Response;
using mapsProjAPI.Interfaces;
using mapsProjAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace mapsProjAPI.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context;
        public AuthManager(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }

        public bool SendVerificationMessage(LoginDTO loginDTO)
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
                loginAttempt.ValidUntill = DateTime.UtcNow.AddDays(90);
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
                return true;
            }
            loginAttempt.Description = "Error Code: " + message.ErrorCode + " Message Error: " + message.ErrorMessage;
            return false;
        }
        public LoginDTOResponse Login(LoginDTO loginDTO)
        {
            var lastAttempt = new LoginAttempt();
            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == loginDTO.PhoneNumber);
            if (user == null) return null;

            var loginResponse = new LoginDTOResponse
            {
                User = null,
                ActiveUntil = null,
                Token = null,
                StatusCode = null
            };

            lastAttempt = _context.LoginAttempts
                .Where(la => la.PhoneNumber == loginDTO.PhoneNumber)
                .OrderByDescending(la => la.DateCreated)
                .FirstOrDefault();
            
            if (lastAttempt == null) {
                lastAttempt.AttemptCounter = 0;
                lastAttempt.PhoneNumber = loginDTO.PhoneNumber;
                lastAttempt.DateCreated = DateTime.UtcNow;
                lastAttempt.ValidUntill = DateTime.UtcNow.AddDays(90);
                lastAttempt.VerificationCode = loginDTO.VerificationCode;
            }
            if (lastAttempt.LockedForLogin >= DateTime.UtcNow)
            {
                return loginResponse;
            }
            if (lastAttempt.LockedForLogin != null && lastAttempt.LockedForLogin < DateTime.UtcNow)
            {
                lastAttempt.AttemptCounter = 0;
                lastAttempt.LockedForLogin = null;
            }
            if (lastAttempt.VerificationCode != loginDTO.VerificationCode)
            {
                lastAttempt.AttemptCounter += 1;
                if (lastAttempt == null || lastAttempt.AttemptCounter >= 3 || lastAttempt.ValidUntill < DateTime.UtcNow)
                {
                    lastAttempt.LockedForLogin = DateTime.UtcNow.AddMinutes(5);
                    loginResponse.StatusCode = CustomErrorCodes.MAX_ATTEMPT_REACHED;
                    _context.SaveChanges();
                    return loginResponse;
                };
                lastAttempt.Description = "Codul nu este valid, te rugăm să încerci din nou! Atentie: codul poate fi greșit de maxim 3 ori și este valid doar 10 minute!";
                _context.SaveChanges();
                return null;
            }
           

            if (user.PhoneNumber != loginDTO.PhoneNumber) return null;

            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            var subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, user.PhoneNumber) });
            var expires = DateTime.UtcNow.AddDays(90);

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
            lastAttempt.Description = "Success";
            _context.SaveChanges();

            loginResponse.User = user;
            loginResponse.ActiveUntil = expires;
            loginResponse.Token = jwtToken;
            loginResponse.StatusCode = StatusCodes.Status200OK.ToString();
           
            return loginResponse;
        }
        public UserDto GetUser(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var phone = tokenS.Claims.First(claim => claim.Type == "sub").Value;
            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == phone);
            if (user == null) return null;
            var userDto = new UserDto()
            {
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                UserRole = user.UserRole
            };
            return userDto;
        }
    }
}
