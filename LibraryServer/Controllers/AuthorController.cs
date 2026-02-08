using LibraryServer.DTO;
using LibraryServer.Model;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly AuthorService _authorService;
        public AuthorController(AuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchText, string? sortedBy)
        {
            return Ok(await _authorService.GetAll(searchText, sortedBy));
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromBody] int id)
        {
            try
            {
                var author = await _authorService.GetById(id);
                return Ok(author);
            }
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
           
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDTO authorDTO)
        {
            try
            {
                var id = _authorService.AddAuthor(authorDTO);
                return Ok(new {AuthorId = id});
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
            
        }

        [HttpPatch]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> UpdateAuthor([FromBody] AuthorDTO authorDTO)
        {
            try
            {
                var id = _authorService.UpdateAuthor(authorDTO);
                return Ok(new { AuthorId = id });
            }
            catch(Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpDelete]
        public async Task<IActionResult> RemoveAuthor([FromBody] int id)
        {
            try
            {
                var res = await _authorService.RemoveAuthor(id);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
