using LibraryServer.DTO.User;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentController(StudentService studentService)
        {
            _studentService = studentService; 
        }

        [Authorize(Roles = "Librarian")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var teacher = await _studentService.GetById(id);
                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentDTO studentDTO)
        {
            try
            {
                var id = await _studentService.AddStudent(studentDTO);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }

        }

        [HttpPatch]
        public async Task<IActionResult> UpdateStudent([FromBody] StudentDTO studentDTO)
        {
            try
            {
                var id = await _studentService.UpdateStudent(studentDTO);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveStudent(int id)
        {
            try
            {
                var result = await _studentService.RemoveStudent(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
        [Authorize(Roles = "Teacher,Librarian")]
        [HttpGet("classes")]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                var classes = await _studentService.GetAllClasses();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [Authorize(Roles = "Teacher,Librarian")]
        [HttpGet("by-class/{classNum}")]
        public async Task<IActionResult> GetByClass(string classNum)
        {
            try
            {
                var students = await _studentService.GetByClass(classNum);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}

    
