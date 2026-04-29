using LibraryServer.DbContext;
using LibraryServer.Enums;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class MaterialService
    {
        private readonly LibraryContext _context;
        private readonly IWebHostEnvironment _env;

        public MaterialService(LibraryContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<Materials>> GetAll(string? search = null)
        {
            var query = _context.Materials.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Name.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<Materials> GetById(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
                throw new Exception("Материал не найден");
            return material;
        }

        public async Task<string> UploadMaterial(UploadMaterialDto uploadMaterialDto)
        {
            if (!uploadMaterialDto.File.FileName.EndsWith(".pdf"))
                throw new Exception("Только PDF файлы разрешены");

            var folder = Path.Combine(_env.WebRootPath, "materials", uploadMaterialDto.Subject.ToString());
            Directory.CreateDirectory(folder);

            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{uploadMaterialDto.File.FileName}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadMaterialDto.File.CopyToAsync(stream);
            }

            var relativePath = $"/materials/{uploadMaterialDto.Subject}/{fileName}";

            var material = new Materials
            {
                Name = uploadMaterialDto.Name,
                SenderID = uploadMaterialDto.SenderId,
                Subject = uploadMaterialDto.Subject,
                Path = relativePath
            };

            _context.Materials.Add(material);
            await _context.SaveChangesAsync();

            return relativePath;
        }

        public async Task<Materials> UpdateMaterial(int materialId, string newName, IFormFile? newFile)
        {
            var material = await _context.Materials.FindAsync(materialId);
            if (material == null)
                throw new Exception("Материал не найден");

            if (!string.IsNullOrEmpty(newName))
                material.Name = newName;

            if (newFile != null)
            {
                if (!newFile.FileName.EndsWith(".pdf"))
                    throw new Exception("Только PDF файлы разрешены");

                var oldFullPath = Path.Combine(_env.WebRootPath, material.Path.TrimStart('/'));
                if (File.Exists(oldFullPath))
                    File.Delete(oldFullPath);

                var folder = Path.Combine(_env.WebRootPath, "materials", material.Subject.ToString());
                Directory.CreateDirectory(folder);

                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{newFile.FileName}";
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newFile.CopyToAsync(stream);
                }

                material.Path = $"/materials/{material.Subject}/{fileName}";
            }

            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<bool> Delete(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
                throw new Exception("Материал не найден");

            var fullPath = Path.Combine(_env.WebRootPath, material.Path.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}