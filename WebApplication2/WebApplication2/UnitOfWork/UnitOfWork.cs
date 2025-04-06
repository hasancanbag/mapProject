using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Interfaces;
using WebApplication2.Models;
using WebApplication2.Repositories;

namespace WebApplication2.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;


        public IUserRepository Users { get; private set; }
        public IRepository<Logs> Logs { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Logs = new Repository<Logs>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
