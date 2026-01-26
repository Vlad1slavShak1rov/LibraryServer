using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly UserService _userService;
        public BookController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchText)
        {
            var list = await _userService.GetAll(searchText);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int? id)
        {
            return Ok();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> AddBook()
        {
            return Ok();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPatch]
        public async Task<IActionResult> UpdateNameBook()
        {
            return Ok();
        }

        [Authorize(Roles = "Librarian")]
        [HttpPut]
        public async Task<IActionResult> UpdateBook()
        {
            return Ok();
        }

        [Authorize(Roles = "Librarian")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBook()
        {
            return Ok();
        }

    }
}
