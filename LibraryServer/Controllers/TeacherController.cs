using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly TeacherService _teacherService;

        public TeacherController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var teacher = await _teacherService.GetById(id);
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
            
        }

        [Authorize(Roles = "Librarian")]
        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody]TeacherDTO teacherDto)
        {
            try
            {
                var id = await _teacherService.AddTeacher(teacherDto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
           
        }

        [Authorize(Roles = "Librarian")]
        [HttpPatch]
        public async Task<IActionResult> UpdateTeacher([FromBody] TeacherDTO teacherDto)
        {
            try
            {
                var id = await _teacherService.UpdateTeacher(teacherDto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [Authorize(Roles = "Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTeacher(int id)
        {
            try
            {
                var result = await _teacherService.RemoveTeacher(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new {msg = ex.Message});
            }
        }

    }
}
