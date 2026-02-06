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
            try
            {
                var userDto = await _userService.GetById(id);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
        }


        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationDTO registrationDTO)
        {
            try
            {
                var token = await _userService.Registration(registrationDTO);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authorization([FromBody] LoginRequestDTO loginRequestDTO )
        {
            try
            {
                var token = await _userService.Authorization(loginRequestDTO.Login, loginRequestDTO.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }


        [HttpPatch("swaplogin/{id}")]
        public async Task<IActionResult> UpdateLogin([FromBody] UpdateLoginDTO updateLogin, [FromRoute]int? id)
        {
            try
            {
                var newLogin = await _userService.UpdateLogin(updateLogin.Login, id);
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
