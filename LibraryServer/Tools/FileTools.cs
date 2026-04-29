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

        public async Task<EventPhoto> UploadEventPhoto(int eventId, IFormFile file, LibraryContext context)
        {
            var path = await SaveFile(eventId, file, "events");

            var eventPhoto = new EventPhoto
            {
                EventId = eventId,
                Path = path
            };

            context.EventPhoto.Add(eventPhoto);
            await context.SaveChangesAsync();

            return eventPhoto;
        }

        public async Task<List<EventPhoto>> UploadEventPhotos(int eventId, List<IFormFile> files, LibraryContext context)
        {
            var photos = new List<EventPhoto>();

            foreach (var file in files)
            {
                var photo = await UploadEventPhoto(eventId, file, context);
                photos.Add(photo);
            }

            return photos;
        }

        public async Task<bool> DeleteEventPhoto(int photoId, LibraryContext context)
        {
            var photo = await context.EventPhoto.FindAsync(photoId);
            if (photo == null)
                throw new Exception("Фото не найдено");

            DeleteOldFile(photo.Path);
            context.EventPhoto.Remove(photo);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAllEventPhotos(int eventId, LibraryContext context)
        {
            var photos = await context.EventPhoto
                .Where(p => p.EventId == eventId)
                .ToListAsync();

            foreach (var photo in photos)
            {
                DeleteOldFile(photo.Path);
                context.EventPhoto.Remove(photo);
            }

            await context.SaveChangesAsync();
            return true;
        }

        private async Task<string> SaveFile(int id, IFormFile file, string folderName)
        {
            var folder = Path.Combine(_env.WebRootPath, "resources", folderName, id.ToString());
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{id}{ext}";
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
        public async Task<EventPhoto> UpdateEventPhoto(int photoId, IFormFile newFile, LibraryContext context)
        {
            var oldPhoto = await context.EventPhoto.FindAsync(photoId);
            if (oldPhoto == null)
                throw new Exception("Фото не найдено");

            DeleteOldFile(oldPhoto.Path);

            var newPath = await SaveFile(oldPhoto.EventId, newFile, "events");
            oldPhoto.Path = newPath;
            await context.SaveChangesAsync();

            return oldPhoto;
        }
    }
}
