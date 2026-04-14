using DotNetEnv;
using LibraryServer.DbContext;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Tools
{
    public class FileTools
    {
        private readonly IWebHostEnvironment _env;

        public FileTools(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadBookImage(int bookId, IFormFile file, LibraryContext context)
        {
            var path = await SaveFile(bookId, file, "book");

            var book = await context.Books.FindAsync(bookId);
            if (book != null)
            {
                DeleteOldFile(book.ImagePath);
                book.ImagePath = path;
                await context.SaveChangesAsync();
            }

            return path;
        }

        public async Task<string> UploadAuthorPhoto(int authorId, IFormFile file, LibraryContext context)
        {
            var path = await SaveFile(authorId, file, "author");

            var author = await context.Authors.FindAsync(authorId);
            if (author != null)
            {
                DeleteOldFile(author.ImagePath);
                author.ImagePath = path;
                await context.SaveChangesAsync();
            }

            return path;
        }

        private async Task<string> SaveFile(int id, IFormFile file, string folderName)
        {
            var folder = Path.Combine(_env.WebRootPath, "resources", folderName, id.ToString());
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N[..6]}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/resources/{folderName}/{id}/{fileName}";
        }

        private void DeleteOldFile(string oldPath)
        {
            if (string.IsNullOrEmpty(oldPath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, oldPath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
