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
        public async Task<IActionResult> GetAll(string? searchText, string? sortedBy)
        {
            var list = await _bookService.GetAll(searchText, sortedBy);
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
        public async Task<IActionResult> UpdateBook(BookDTO bookDTO)
        {
            try
            {
                var newBook = await _bookService.EditBook(bookDTO);
                return Ok(newBook);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpPatch]
        public async Task<IActionResult> ChangeStatus([FromBody] BookStatusChangedDTO bookStatusChanged)
        {
            try
            {
                var newBookStatus = await _bookService.ChangeStatusBook(bookStatusChanged);
                return Ok(newBookStatus);
            }
            catch(Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            try
            {
                await _bookService.RemoveBook(id);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

    }
}
