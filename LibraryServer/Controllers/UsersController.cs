using LibraryServer.DTO;
using LibraryServer.Service;
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

            var user = await _userService.GetById(id!.Value);

            if (user == null)
            {
                return NotFound(new { msg = "User not has in database!"});
            }

            var userDto = new UserDTO()
            {
                Id = user.Id,
                Login = user.Login,
                Password = user.Password,
                Role = user.Role,
            };

            return Ok(userDto);
        }


        [HttpGet("login")]
        public async Task<IActionResult> Authorization(string login, string password)
        {
            try
            {
                var user = await _userService.Authorization(login, password);
                return Ok(user);
            } 
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDTO request)
        {
            return Ok();
        }


        /*
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        */
        [HttpPost("registration")]
        public async Task<IActionResult> Registration(string login, string password)
        {
            try
            {
                var user = await _userService.Registration(login, password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
