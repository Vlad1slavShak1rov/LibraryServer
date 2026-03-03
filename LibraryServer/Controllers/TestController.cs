using LibraryServer.DTO;
using LibraryServer.Service;
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


        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromBody]string topic)
        {
            try
            {
                var test = await _testService.CreateTest(topic);
                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveTest([FromBody] TestDTO testDTO)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("byUser")]
        public async Task<IActionResult> GetTestsByUserId([FromBody] int userId)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
