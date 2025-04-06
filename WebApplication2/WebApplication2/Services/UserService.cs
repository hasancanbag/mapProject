using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication2.Interfaces;

namespace WebApplication2.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsUserAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        public int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        public async Task<bool> ConfirmUserEmail(string userId)
        {
            // String id'yi int'e dönüştür
            if (!int.TryParse(userId, out int userIdInt))
            {
                // Eğer dönüşüm başarısız olursa false döndürüyoruz
                return false;
            }

            // Kullanıcıyı UserRepository ile asenkron şekilde buluyoruz
            var user = await _unitOfWork.Users.GetByIdAsync(userIdInt);
            if (user == null)
            {
                return false; // Kullanıcı bulunamadı
            }

            user.Emailconfirmed = true; // E-posta onaylı olarak işaretle

            // Değişiklikleri asenkron olarak kaydediyoruz
            await _unitOfWork.CompleteAsync();

            return true;
        }

    }

}
