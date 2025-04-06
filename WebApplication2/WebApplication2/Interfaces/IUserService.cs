using System.Security.Claims;

namespace WebApplication2.Interfaces
{
    public interface IUserService
    {
        bool IsUserAdmin(ClaimsPrincipal user);
        int GetUserIdFromToken(ClaimsPrincipal user);

        Task<bool> ConfirmUserEmail(string userId);
    }
}
