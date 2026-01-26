using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Security.Claims;
using LibraryServer.Tools;
namespace LibraryServer.Service
{
    public class UserService
    {
        private readonly LibraryContext _context;
        private readonly JWTCreater _jwtCreater;
        public UserService(LibraryContext context, JWTCreater jwtCreater) 
        {
            _context = context;
            _jwtCreater = jwtCreater;
        }

        public async Task<List<User>> GetAll(string? sortedBy = null, string? searchText = null)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(u => u.Login.ToLower().StartsWith(searchText.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(sortedBy))
            {
                query = sortedBy.ToLower() switch
                {
                    "byname" => query.OrderBy(u => u.Login),
                    "bydescname" => query.OrderByDescending(u => u.Login),
                    _ => query.OrderBy(u => u.Id)
                };
            }
            else
            {
                query = query.OrderBy(u => u.Id);
            }

            return await query.ToListAsync();
        }

        public async Task<User?> GetById(int? id)
        {
            return await _context.Users.FirstOrDefaultAsync(u=>u.Id == id);
        }

        public async Task<string> Authorization(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new Exception("Login is empty!");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password is empty!");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Login == login);

            if(user == null)
            {
                throw new Exception("The user is not registered!");
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!isValid)
            {
                throw new Exception("Password is incorrect!");
            }

            if (!isValid)
            {
                throw new Exception("Password is incorrect!");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            string jwt = _jwtCreater.JWTCreate(claims);
            return jwt;
        }

        public async Task<string> Registration(string login, string password, Enums.Role? role = null)
        {
            if (string.IsNullOrEmpty(login))
            {
                throw new Exception("Login is empty!");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password is empty!");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user != null)
            {
                throw new Exception("User is already registered");
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Login = login,
                Password = hashPassword,
                Role = role == null ? Enums.Role.User : role!.Value,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.Role, newUser.Role.ToString()),
            };

            string jwt = _jwtCreater.JWTCreate(claims);

            return jwt;
        }
    }
}
