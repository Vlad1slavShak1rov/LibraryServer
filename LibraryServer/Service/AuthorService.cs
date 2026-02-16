using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;
namespace LibraryServer.Service
{
    public class AuthorService
    {
        private readonly LibraryContext _context;
        public AuthorService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<AuthorDTO>> GetAll(string? searchText = null, string? sortedBy = null)
        {
            IQueryable<AuthorDTO> authors = _context.Authors.
                Select(a => new AuthorDTO
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    SecondName = a.SecondName,
                    DateOfBirth = a.DateOfBirth,
                    DateOfDeath = a.DateOfDeath,
                    Description = a.Description,
                    ImagePath = a.ImagePath,
                });

            if(!string.IsNullOrEmpty(searchText))
            {
                authors = authors.Where(a=>a.FirstName.ToLower().StartsWith(searchText.ToLower()) ||
                a.SecondName.ToLower().StartsWith(searchText.ToLower()) ||
                a.LastName.ToLower().StartsWith(searchText.ToLower()));
            }

            if (!string.IsNullOrEmpty(sortedBy))
            {
                authors = sortedBy.ToLower() switch
                {
                    "byDate" => authors.OrderBy(a => a.DateOfBirth),
                    "bySecondName" => authors.OrderBy(a=>a.SecondName),
                    _ => authors = authors.OrderBy(a=>a.Id)
                };
            }


            return await authors.ToListAsync();
        }

        public async Task<AuthorDTO> GetById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                throw new NullReferenceException(nameof(author));
            }

            return new AuthorDTO()
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                SecondName = author.LastName,
                DateOfBirth = author.DateOfBirth,
                DateOfDeath = author.DateOfDeath,
                Description = author.Description,
                ImagePath = author.ImagePath,
            };
        }
        public async Task<int> AddAuthor(AuthorDTO author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            if (string.IsNullOrEmpty(author.FirstName))
            {
                throw new Exception("First Name was empty!");
            }

            if (string.IsNullOrEmpty(author.SecondName))
            {
                throw new Exception("Second Name was empty!");
            }

            if (string.IsNullOrEmpty(author.LastName))
            {
                throw new Exception("Last Name was empty!");
            }

            if (author.DateOfBirth == null)
            {
                throw new Exception("Date Of Birth was empty!");
            }

            var existAuthor = await _context.Authors.FirstOrDefaultAsync(a => $"{a.SecondName} {a.FirstName} {a.LastName}" == $"{author.SecondName} {author.FirstName} {author.LastName}");
            if (existAuthor != null)
            {
                throw new Exception("This author has already been added!");
            }

            var newAuthor = new Author()
            {
                FirstName = author.FirstName,
                LastName = author.LastName,
                SecondName= author.LastName,
                Description = author.Description,
                ImagePath = author.ImagePath,
                DateOfBirth = author.DateOfBirth!.Value,
                DateOfDeath = author.DateOfDeath,
            };

            await _context.Authors.AddAsync(newAuthor);
            await _context.SaveChangesAsync();

            return newAuthor.Id;
        }

        public async Task<int> UpdateAuthor(AuthorDTO author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            if (string.IsNullOrEmpty(author.FirstName))
            {
                throw new Exception("First Name was empty!");
            }

            if (string.IsNullOrEmpty(author.SecondName))
            {
                throw new Exception("Second Name was empty!");
            }

            if (string.IsNullOrEmpty(author.LastName))
            {
                throw new Exception("Last Name was empty!");
            }

            if (author.DateOfBirth == null)
            {
                throw new Exception("Date Of Birth was empty!");
            }

            var existAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.FullName == $"{author.SecondName} {author.FirstName} {author.LastName}");
            if (existAuthor == null)
            {
                throw new Exception("This author has not been added!");
            }

            existAuthor.FirstName = author.FirstName;
            existAuthor.LastName = author.LastName;
            existAuthor.SecondName = author.LastName;
            existAuthor.Description = author.Description;
            existAuthor.ImagePath = author.ImagePath;
            existAuthor.DateOfBirth = author.DateOfBirth!.Value;
            existAuthor.DateOfDeath = author.DateOfDeath;

            await _context.SaveChangesAsync();

            return existAuthor.Id;
        }

        public async Task<bool> RemoveAuthor(int? id)
        {
            if(id == null || id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null) 
            {
                throw new Exception("Author has not been found!");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
