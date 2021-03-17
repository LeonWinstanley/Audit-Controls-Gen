using System.Threading.Tasks;
using FinalYearProject.Enums;
using FinalYearProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject.Services
{
    public class UserService
    {
        private readonly DatabaseContext _context;

        public UserService(DatabaseContext Context)
        {
            _context = Context;
        }

        public async Task<User> FetchUserFromDb(string UserName)
        {
            return await _context.Users.SingleOrDefaultAsync(X => X.UserName == UserName);
        }

        public async Task AddNewUserToDb(string UserName, Role UserRole)
        {
            User user = new User
            {
                UserName = UserName,
                UserRole = UserRole
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}