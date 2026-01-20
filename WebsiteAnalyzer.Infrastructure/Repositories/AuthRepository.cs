using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebsiteAnalyzer.Application.DTOs;
using WebsiteAnalyzer.Application.Interfaces;
using WebsiteAnalyzer.Domain.Entities;
using WebsiteAnalyzer.Infrastructure.Data;

namespace WebsiteAnalyzer.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            bool userExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && !u.Is_Deleted);

            if (userExists)
                return null;

            var user = new User
            {
                Full_Name = dto.Full_Name,
                Email = dto.Email,
                Password_Hash = HashPassword(dto.Password),
                Role_Id = dto.Role_Id,
                Created_On = DateTime.Now,
                Modified_On = DateTime.Now,
                Is_Active = true,
                Is_Deleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> LoginAsync(LoginDto dto)
        {
            string hashedPassword = HashPassword(dto.Password);

            var user = await (
                from u in _context.Users
                join r in _context.Roles
                    on u.Role_Id equals r.Id
                where u.Email == dto.Email
                      && u.Password_Hash == hashedPassword
                      && u.Is_Active
                      && !u.Is_Deleted
                select new User
                {
                    Id = u.Id,
                    Full_Name = u.Full_Name,
                    Email = u.Email,
                    Password_Hash = u.Password_Hash,
                    Role_Id = u.Role_Id,
                    Role = r,

                    Created_On = u.Created_On,
                    Modified_On = u.Modified_On,
                    Is_Active = u.Is_Active,
                    Is_Deleted = u.Is_Deleted
                }
            ).FirstOrDefaultAsync();

            return user;
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
