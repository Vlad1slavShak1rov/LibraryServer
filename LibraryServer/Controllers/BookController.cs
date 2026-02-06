using LibraryServer.DTO;
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
        private readonly BookService _bookService;
        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? searchText)
        {
            var list = await _bookService.GetAll(searchText);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int? id)
        {
            try
            {
                var book = await _bookService.GetById(id);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookDTO addBookDTO)
        {
            try
            {
                var book = await _bookService.AddBook(addBookDTO);
                return Ok(book.Id);
            } 
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
            
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
