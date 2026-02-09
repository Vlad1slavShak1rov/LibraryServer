using BCrypt.Net;
using LibraryServer.DbContext;
using LibraryServer.DTO;
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

        public async Task<List<UserDTO>> GetAll(string? sortedBy = null, string? searchText = null)
        {
            IQueryable<UserDTO> query = _context.Users.Select(u=> new UserDTO
            {
                Id = u.Id,
                Login = u.Login,
                Role = u.Role
            });

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

        [AllowAnonymous]
        public async Task<string> Registration(RegistrationDTO registrationDTO)
        {
            string login = registrationDTO.Login;
            string password = registrationDTO.Password;
            var role = registrationDTO.Role;

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
                Password = "1",
                Role = role == null ? Enums.Role.Student : role.Value,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            if(newUser.Role == Enums.Role.Student)
            {
                var student = new Student()
                {
                    UserID = newUser.Id,
                    FirstName = "",
                    SecondName = "",
                    LastName = "",
                    ClassNum = "",
                };

                await AddEntity<Student>(student);
            }
            else if (newUser.Role == Enums.Role.Teacher)
            {
                var teacher = new Teacher()
                {
                    UserID = newUser.Id,
                    FirstName = "",
                    SecondName = "",
                    LastName = "",
                    Contact = "",
                };

                await AddEntity<Teacher>(teacher);
            }

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.Role, newUser.Role.ToString()),
            };

            string jwt = _jwtCreater.JWTCreate(claims);
            return jwt;
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

        public async Task AddEntity<T>(T entity) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
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
