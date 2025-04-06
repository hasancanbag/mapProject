/*using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<Response> GetUsers()
        {
            var _response = new Response();

            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();

                _response.Value = users;
                _response.Result = true;
                _response.Message = users.Any() ? "Users retrieved successfully." : "No users found.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while retrieving users: {ex.Message}";
            }

            return _response;
        }

        [HttpDelete("{id}")]
        public async Task<Response> DeleteUser(int id)
        {
            var _response = new Response();

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "User not found.";
                return _response;
            }

            try
            {
                _unitOfWork.Users.Delete(user);
                await _unitOfWork.CompleteAsync();

                _response.Value = null;
                _response.Result = true;
                _response.Message = "User successfully deleted.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while deleting the user: {ex.Message}";
            }

            return _response;
        }

        [HttpPut("{id}")]
        public async Task<Response> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var _response = new Response();

            if (updatedUser == null || id != updatedUser.Id)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Invalid user data.";
                return _response;
            }

            var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
            if (existingUser == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "User not found.";
                return _response;
            }

            try
            {
                existingUser.Username = updatedUser.Username;
                existingUser.Email = updatedUser.Email;
                existingUser.Role = updatedUser.Role;
                existingUser.PasswordHash = existingUser.PasswordHash;

                _unitOfWork.Users.Update(existingUser);
                await _unitOfWork.CompleteAsync();

                _response.Value = existingUser;
                _response.Result = true;
                _response.Message = "User successfully updated.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while updating the user: {ex.Message}";
            }

            return _response;
        }

    }
} */

using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<Response> GetUsers()
        {
            var _response = new Response();

            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();

                var userDtos = users.Select(u => new UserDto
                {
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    Id = u.Id
                }).ToList();

                _response.Value = userDtos;
                _response.Result = true;
                _response.Message = users.Any() ? "Users retrieved successfully." : "No users found.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while retrieving users: {ex.Message}";
            }

            return _response;
        }

        [HttpDelete("{id}")]
        public async Task<Response> DeleteUser(int id)
        {
            var _response = new Response();

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "User not found.";
                return _response;
            }

            try
            {
                _unitOfWork.Users.Delete(user);
                await _unitOfWork.CompleteAsync();

                _response.Value = null;
                _response.Result = true;
                _response.Message = "User successfully deleted.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while deleting the user: {ex.Message}";
            }

            return _response;
        }

        [HttpPut("{id}")]
        public async Task<Response> UpdateUser(int id, [FromBody] UserDto updatedUserDto)
        {
            var _response = new Response();

            if (updatedUserDto == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Invalid user data.";
                return _response;
            }

            var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
            if (existingUser == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "User not found.";
                return _response;
            }

            try
            {
                existingUser.Username = updatedUserDto.Username;
                existingUser.Email = updatedUserDto.Email;
                existingUser.Role = updatedUserDto.Role;

                _unitOfWork.Users.Update(existingUser);
                await _unitOfWork.CompleteAsync();

                _response.Value = updatedUserDto;
                _response.Result = true;
                _response.Message = "User successfully updated.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while updating the user: {ex.Message}";
            }

            return _response;
        }
    }
}

