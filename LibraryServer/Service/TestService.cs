using LibraryServer.DbContext;

namespace LibraryServer.Service
{
    public class TestService
    {
        private readonly LibraryContext _context;
        private readonly DeepSeekService _deepSeekService;
        public TestService(LibraryContext context, DeepSeekService deepSeekService)
        {
            _context = context;
            _deepSeekService = deepSeekService;
        }

        public async Task<JsonTest> CreateTest(string promt)
        {
            var test = await _deepSeekService.GenerateTestAsync(promt);
            return test;
        }
    }
}
