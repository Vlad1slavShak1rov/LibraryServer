using LibraryServer.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LibraryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly LibraryContext _context;

        public SeedController(LibraryContext context)
        {
            _context = context;
        }

        [HttpPost("fill-db")]
        public async Task<IActionResult> FillDatabase()
        {
            var sqlPath = Path.Combine(Directory.GetCurrentDirectory(), "seed.sql");

            if (!System.IO.File.Exists(sqlPath))
            {
                return BadRequest("Файл seed.sql не найден.");
            }

            var sqlScript = await System.IO.File.ReadAllTextAsync(sqlPath);

            try
            {
                await _context.Database.ExecuteSqlRawAsync(sqlScript);
                return Ok("База данных успешно заполнена!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при заполнении БД: {ex.Message}");
            }
        }
    }
}