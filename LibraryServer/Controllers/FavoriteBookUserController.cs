using LibraryServer.DbContext;
using LibraryServer.Service;
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
        public async Task<IActionResult> GetAllUserBook()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFavoriteByUser()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddFavourite()
        {
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFavourite()
        {
            return Ok();
        }

    }
}
