using LibraryServer.DbContext;
using LibraryServer.DTO.Book;
using LibraryServer.DTO.FavouriteBook;
using LibraryServer.Model;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteBookUserController : ControllerBase
    {
        private readonly FavoriteBookUserService _favoriteBookUserService;
        public FavoriteBookUserController(FavoriteBookUserService favoriteBookUserService)
        {
            _favoriteBookUserService = favoriteBookUserService;
        }

        [HttpGet]
        [Authorize(Roles = "Student, Librarian, Teacher")]
        public async Task<IActionResult> GetAllUserBook()
        {
            try
            {
                var list = await _favoriteBookUserService.GetUserBooks();
                return Ok(list);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Student, Librarian, Teacher")]
        public async Task<IActionResult> GetAllFavoriteByUser([FromRoute] int id)
        {
            try
            {
                var list = await _favoriteBookUserService.GetBookByUser(id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Student, Librarian, Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddFavourite([FromBody] BookUserDTO bookUserDTO)
        {
            try
            {
                var bookUser = await _favoriteBookUserService.AddFavoriteBook(bookUserDTO);
                return Ok(bookUser);
            } catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles = "Student, Librarian, Teacher")]
        [HttpDelete]
        public async Task<IActionResult> RemoveFavourite([FromBody] RemoveFavouriteRequestDTO? removeFavouriteRequestDTO)
        {
            try
            {
                await _favoriteBookUserService.RemoveFavoriteBook(removeFavouriteRequestDTO);
                return Ok();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

    }
}
