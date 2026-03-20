using LibraryServer.DTO.BookingBook;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingBookController :ControllerBase
    {
        private BookigBookService _bookingBookService;
        public BookingBookController(BookigBookService bookigBookService)
        {
            _bookingBookService = bookigBookService;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll(string? seacrhText = null, string? sortedBy = null)
        {
            return Ok(await _bookingBookService.GetAll(seacrhText,sortedBy));
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
        {
            try
            {
                await _bookingBookService.CreateReservation(createBookingDto);
                return Ok(true);
            } 
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, access = false });
            }
           
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpGet("myActive/{userId}")]
        public async Task<IActionResult> GetActive([FromRoute] int userId)
        {
            try
            {
                var list = await _bookingBookService.GetMyActive(userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message});
            }
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpPut("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookDto returnBookDto)
        {
            try
            {
                bool res = await _bookingBookService.ReturnBook(returnBookDto);
                return Ok(res);
            } 
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
            
        }
    }
}
