using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.DTO.Tests;
using LibraryServer.Model;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly TestService _testService;
        private readonly LibraryContext _context;

        public TestController(TestService testService, LibraryContext context)
        {
            _testService = testService;
            _context = context;
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests()
        {
            var tests = await _testService.GetAllTests();
            return Ok(tests);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTest(int id)
        {
            try
            {
                var test = await _testService.GetTestById(id);
                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitTest([FromBody] SubmitTestDTO submitTest)
        {
            try
            {
                var result = await _testService.SubmitTest(submitTest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Teacher,Librarian")]
        [HttpGet("results")]
        public async Task<IActionResult> GetAllResults()
        {
            try
            {
                var results = await _testService.GetAllResults();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("results/{id}")]
        public async Task<IActionResult> GetResultById(int id)
        {
            try
            {
                var result = await _testService.GetResultById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestDTO createTest)
        {
            try
            {
                var test = await _testService.CreateTest(createTest);
                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Teacher,Librarian")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTest([FromBody] AssignTestDTO dto)
        {
            try
            {
                var result = await _testService.AssignTest(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedTests(int userId)
        {
            var tests = await _testService.GetUserAssignedTests(userId);
            return Ok(tests);
        }

        [HttpGet("getTestByBookId/{bookId}")]
        public async Task<IActionResult> GetTestByBookId(int bookId)
        {
            var test = await _context.Tests
                .Where(t => t.BookId == bookId)
                .Select(t => new
                {
                    id = t.Id,
                    bookId = t.BookId,
                    testName = t.TestName,
                    testDesc = t.TestDescription
                })
                .FirstOrDefaultAsync();

            if (test == null)
                return NotFound();

            return Ok(test);
        }

        [Authorize(Roles = "Teacher,Librarian")]
        [HttpGet("assigned/teacher/{teacherId}")]
        public async Task<IActionResult> GetAssignedByTeacher(int teacherId)
        {
            var data = await _testService.GetAssignedTestsByTeacher(teacherId);
            return Ok(data);
        }

    }
}
