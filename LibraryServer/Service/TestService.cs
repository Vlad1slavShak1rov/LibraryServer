using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class TestService
    {
        private readonly LibraryContext _context;
        private readonly OpenRouteService _deepSeekService;
        public TestService(LibraryContext context, OpenRouteService deepSeekService)
        {
            _context = context;
            _deepSeekService = deepSeekService;
        }

        public async Task<List<ResultTest>> GetAll(string? sortedBy = null, int? userId = null)
        {
            var tests = _context.ResultTests.AsQueryable();

            if(userId is not null)
            {
                tests = tests.Where(t=>t.UserId == userId);
            }

            if (!string.IsNullOrEmpty(sortedBy))
            {
                tests = sortedBy.ToLower() switch
                {
                    "toSuccess" => tests.OrderBy(t => t.IsSuccess),
                    "toDateUp" => tests.OrderBy(t=>t.CreatedAt),
                    "toDateDown" => tests.OrderByDescending(t=>t.CreatedAt),
                    _ => tests.OrderBy(t=>t.Id)
                };
            }

            return await tests.ToListAsync();
        }

        public async Task<ResultTestDTO> CreateTest(CreateTestDTO createTest)
        {
            try
            {
                var test = await _deepSeekService.GenerateTestAsync(createTest.Topic);

                var entityTest = new Test
                {
                    Subject = test.Subject,
                };

                await _context.Tests.AddAsync(entityTest);
                await _context.SaveChangesAsync();

                var questions = test.Questions
                    .Select(q => new QuestionTest
                    {
                        TestId = entityTest.Id,
                        Number = q.Number,
                        Text = q.Text,
                        Options = q.Options,
                        CorrectAnswer = q.CorrectAnswer,
                        Explanation = q.Explanation,
                    });

                await _context.AddRangeAsync(questions);
                await _context.SaveChangesAsync();

                var resultTest = new ResultTest()
                {
                    UserId = createTest.UserId,
                    TestId = entityTest.Id,
                    Description = createTest.Description,
                    CreatedAt = DateTime.Now.Date
                };

                await _context.ResultTests.AddAsync(resultTest);
                await _context.SaveChangesAsync();

                var resultTestDto = new ResultTestDTO
                {
                    UserId = createTest.UserId,
                    TestId = entityTest.Id,
                    Description = createTest.Description,
                    Score = null
                };

                return resultTestDto;

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        public async Task<TestDto> GetById(int? testId)
        {
            if(testId == null) throw new ArgumentNullException(nameof(testId));

            var test = await _context.Tests
                .Include(t=>t.Questions)
                .FirstOrDefaultAsync(t=>t.Id == testId);

            if (test == null) throw new Exception("Test not found!");

            var testDto = new TestDto 
            {
                Subject = test.Subject,
                Questions = test.Questions,
            };

            return testDto;
        }

        public async Task<bool> TestVerification(SolvedTestDto? solvedTest)
        {
            var test = await GetById(solvedTest.UserId);

            int correctAnswer = 0;
            int i = 0;
            foreach(var t in test.Questions)
            {
                if(t.CorrectAnswer == solvedTest.Answers[i])
                {
                    correctAnswer ++;
                }

                i++;
            }

            int quantityQuestion = test.Questions.Count;
            double percent = correctAnswer / quantityQuestion * 100;

            int score = (percent) switch
            {
                >= 90 => 5,
                >= 75 => 4,
                >= 60 => 3,
                >= 0 => 2,
                _ => -1,
            };

            if (score == -1) throw new Exception("There was an error checking your score");

            var testResult = await _context.ResultTests
                .FirstOrDefaultAsync(t=>t.UserId == solvedTest.UserId && t.TestId == solvedTest.TestId);

            if (testResult == null) throw new Exception("The test is not found!");

            testResult.IsSuccess = true;
            testResult.Score = score;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
