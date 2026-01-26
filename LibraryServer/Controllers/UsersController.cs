using LibraryServer.DTO;
using LibraryServer.Service;
using LibraryServer.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserService _userService;
       
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? sortedBy = null, string? searchText = null)
        {
            var list = await _userService.GetAll(sortedBy, searchText);
            return Ok(list);
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int? id)
        {
            if(id is null)
            {
                return BadRequest(new {msg = "ID was null!"});
            }

            if(!int.TryParse(id.ToString(), out var idValue))
            {
                return BadRequest(new {msg = "ID is not integer!" });
            }

            var userDto = await _userService.GetById(id!.Value);

            if (userDto == null)
            {
                return NotFound(new { msg = "User not has in database!"});
            }

            return Ok(userDto);
        }


        [HttpPost("registration")]
        public async Task<IActionResult> Registration(string login, string password)
        {
            try
            {
                var token = await _userService.Registration(login, password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpGet("login")]
        public async Task<IActionResult> Authorization(string login, string password)
        {
            try
            {
                var token = await _userService.Authorization(login, password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }


        [HttpPatch("swaplogin/{id}")]
        public async Task<IActionResult> UpdateLogin(string? login, int? id)
        {
            try
            {
                var newLogin = await UpdateLogin(login, id);
                return Ok(new {login = newLogin});
            } 
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteUser(id);
            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}
