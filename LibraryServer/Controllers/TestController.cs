using LibraryServer.DTO;
using LibraryServer.DTO.Tests;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly TestService _testService;

        public TestController(TestService testService)
        {
            _testService = testService;
        }

        [Authorize(Roles = "Librarian, Teacher, Student")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTests(int userId)
        {
            var tests = await _testService.GetAllTests(userId);
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
    }
}
