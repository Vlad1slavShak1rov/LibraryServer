using LibraryServer.DTO;
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

        public async Task<IActionResult> GetAll(string? sortedBy = null, int? userId = null)
        {
            return Ok(await _testService.GetAll(sortedBy, userId));
        }

        [Authorize(Roles = "Librian, Teacher, Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromBody]CreateTestDTO createTest)
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

        [Authorize(Roles = "Librian, Teacher, Student")]
        [HttpGet("get-test")]
        public async Task<IActionResult> GetTestById(int? testId)
        {
            try
            {
                var test = await _testService.GetById(testId);
                return Ok(test);

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Librian, Teacher, Student")]
        [HttpPost("send-test")]
        public async Task<IActionResult> CheckResult(SolvedTestDto solvedTest)
        {
            try
            {
                var result = await _testService.TestVerification(solvedTest);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
    }
}
