using Microsoft.AspNetCore.Mvc;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.Services;
using BCrypt.Net;
using System.Web;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public AuthController(IUnitOfWork unitOfWork, AuthService authService, IEmailService emailService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _emailService = emailService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<Response> Register([FromBody] RegisterDto dto)
        {
            var response = new Response();

            try
            {
                if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Role))
                {
                    response.Value = null;
                    response.Result = false;
                    response.Message = "All fields are required.";
                    return response;
                }

                var existingUser = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
                if (existingUser != null)
                {
                    response.Value = null;
                    response.Result = false;
                    response.Message = "Username already exists.";
                    return response;
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Role = dto.Role,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Emailconfirmed = false,
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                var token = _authService.GenerateEmailConfirmationToken(user);
                if (string.IsNullOrEmpty(token))
                {
                    response.Value = null;
                    response.Result = false;
                    response.Message = "Failed to generate confirmation token.";
                    return response;
                }
                var confirmationLink = $"http://localhost:7014/api/auth/confirm-email?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

                await _emailService.SendEmailAsync(user.Email, "Email Verification",
                    $"Please verify your email by clicking <a href='{confirmationLink}'>here</a>.");

                response.Value = null;
                response.Result = true;
                response.Message = "Please verify via mail.";


            }
            catch (Exception ex)
            {
                response.Value = null;
                response.Result = false;
                response.Message = "An error occurred during registration: " + ex.Message;
            }

            return response;
        }

        [HttpPost("login")]
        public async Task<Response> Login([FromBody] LoginDto dto)
        {
            var response = new Response();

            try
            {
                var user = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    response.Value = null;
                    response.Result = false;
                    response.Message = "Invalid credentials";
                    return response;
                }

                if (user.Emailconfirmed == false)
                {
                    response.Value = null;
                    response.Result = false;
                    response.Message = "User not verified";
                    return response;
                }

                var token = _authService.GenerateToken(user);

                response.Value = token;
                response.Result = true;
                response.Message = "Login successful.";
            }
            catch (Exception ex)
            {
                response.Value = null;
                response.Result = false;
                response.Message = "An error occurred during login: " + ex.Message;
            }

            return response;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid user ID or token.");
            }

            if (!_authService.ValidateEmailConfirmationToken(token, out string validatedUserId))
            {
                return BadRequest("Invalid or expired token.");
            }

            if (validatedUserId != userId)
            {
                return Unauthorized("Token does not match the user.");
            }

            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid user ID format.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userIdInt);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Emailconfirmed = true;
            await _unitOfWork.CompleteAsync(); 

            return Ok("Email successfully confirmed.");
        }


    }
}
