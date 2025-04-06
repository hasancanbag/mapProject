using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRepository<Logs> Logs { get; }
        Task<int> CompleteAsync();
    }
}
