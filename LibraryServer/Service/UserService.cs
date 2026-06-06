using BCrypt.Net;
using LibraryServer.DbContext;
using LibraryServer.DTO.Authorization;
using LibraryServer.DTO.User;
using LibraryServer.Enums;
using LibraryServer.Model;
using LibraryServer.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

        public async Task<List<UserFullDTO>> GetAll(string? sortedBy = null, string? searchText = null, Role? role = null)
        {
            var query = _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .AsQueryable();

            // фильтр по роли
            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }

            // поиск
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(u => u.Login.ToLower().Contains(searchText.ToLower()));
            }

            // сортировка
            query = sortedBy?.ToLower() switch
            {
                "byname" => query.OrderBy(u => u.Login),
                "bydescname" => query.OrderByDescending(u => u.Login),
                _ => query.OrderBy(u => u.Id)
            };

            return await query.Select(u => new UserFullDTO
            {
                Id = u.Id,
                Login = u.Login,
                Role = u.Role,

                FullName =
                    u.Role == Enums.Role.Student && u.Student != null
                        ? $"{u.Student.SecondName} {u.Student.FirstName} {u.Student.LastName}"
                        : u.Teacher != null
                            ? $"{u.Teacher.SecondName} {u.Teacher.FirstName} {u.Teacher.LastName}"
                            : "",

                ClassNum = u.Role == Enums.Role.Student ? u.Student.ClassNum : null,

                Contact = u.Role == Enums.Role.Teacher ? u.Teacher.Contact : null
            }).ToListAsync();
        }

        public async Task<UserDTO?> GetById(int? id)
        {
            if (id is null)
            {
                throw new ArgumentNullException("ID was null!");
            }

            var user =  await _context.Users.FirstOrDefaultAsync(u=>u.Id == id);

            if (user == null)
            {
                throw new Exception("User has not found");
            }

            var userDto = new UserDTO()
            {
                Id = user.Id,
                Login = user.Login,
                Role = user.Role,
            };

            return userDto;
        }

        [AllowAnonymous]
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

        public async Task<int> Registration(RegistrationDTO registrationDTO)
        {
            string login = registrationDTO.Login;
            string password = registrationDTO.Password;
            Enums.Role? role = registrationDTO.Role;

            if (string.IsNullOrEmpty(login))
                throw new Exception("Login is required!");

            if (string.IsNullOrEmpty(password))
                throw new Exception("Password is required!");

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (existingUser != null)
                throw new Exception("This user already exist!");

            var newUser = new User
            {
                Login = login,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role ?? Enums.Role.Student,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Id;
        }

        public async Task<string> UpdateLogin(string? login, int? id)
        {
            if(id is null || id == 0)
            {
                throw new Exception("Id was empty or null");
            }

            if (string.IsNullOrEmpty(login))
            {
                throw new Exception("Login was empty or null");
            }

            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                throw new Exception("User is not found");
            }

            users.Login = login;
            _context.SaveChanges();

            return login;
        }

       
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id == id);

            if(user == null) return false;
        
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
