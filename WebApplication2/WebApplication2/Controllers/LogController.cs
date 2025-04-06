using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public LogsController(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<Response> GetLogs()
        {
            var user = HttpContext.User; 
            var userId = _userService.GetUserIdFromToken(user);
            var isAdmin = _userService.IsUserAdmin(user);
            Console.WriteLine($"User ID: {userId}");
            List<Logs> logs = null;
            if (isAdmin)
            {
                logs = await _unitOfWork.Logs.GetAllAsync(); 
            }
            else
            {
                logs = await _unitOfWork.Logs.FindAsync(log => log.UserId == userId);
            }

            var _response = new Response();

            if (logs == null || logs.Count == 0) 
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Couldn't get logs.";
                return _response;
            }

            _response.Value = logs;
            _response.Result = true;
            _response.Message = "Successful";
            return _response;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<Response> GetLog(int id)
        {
            var log = await _unitOfWork.Logs.GetByIdAsync(id);
            var _response = new Response();

            if (log == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Couldn't get log.";
                return _response;
            }

            _response.Value = log;
            _response.Result = true;
            _response.Message = "Successful";
            return _response;
        }
        [Authorize]
        [HttpPost]
        public async Task<Response> CreateLog([FromBody] Logs log)
        {
            var _response = new Response();

            if (log == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Invalid log data.";
                return _response;
            }

            try
            {
                await _unitOfWork.Logs.AddAsync(log);
                await _unitOfWork.CompleteAsync();

                _response.Value = log;
                _response.Result = true;
                _response.Message = "Log successfully created.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while creating the log: {ex.Message}";
            }

            return _response;
        }



        [Authorize]
        [HttpPut("{id}")]
        public async Task<Response> UpdateLog(int id, [FromBody] Logs log)
        {
            var _response = new Response();

            if (log == null || id != log.Id)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Invalid log data or mismatched ID.";
                return _response;
            }

            try
            {
                _unitOfWork.Logs.Update(log);
                await _unitOfWork.CompleteAsync();

                _response.Value = log;
                _response.Result = true;
                _response.Message = "Log successfully updated.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while updating the log: {ex.Message}";
            }

            return _response;
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<Response> DeleteLog(int id)
        {
            var _response = new Response();

            var log = await _unitOfWork.Logs.GetByIdAsync(id);
            if (log == null)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = "Log not found.";
                return _response;
            }

            try
            {
                _unitOfWork.Logs.Delete(log);
                await _unitOfWork.CompleteAsync();

                _response.Value = null;
                _response.Result = true;
                _response.Message = "Log successfully deleted.";
            }
            catch (Exception ex)
            {
                _response.Value = null;
                _response.Result = false;
                _response.Message = $"An error occurred while deleting the log: {ex.Message}";
            }

            return _response;
        }

    }
}
