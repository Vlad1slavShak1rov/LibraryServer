using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.DTO.Tests;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;

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

        public async Task<List<TestShortDTO>> GetAllTests(int userId)
        {
            return await _context.Tests
                .Select(t => new TestShortDTO
                {
                    Id = t.Id,
                    TestName = t.TestName,
                    TestDescription = t.TestDescription,

                    QuestionCount = t.Questions.Count,

                    UserPercent = _context.Results
                        .Where(r => r.TestId == t.Id && r.UserId == userId)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => (double?)r.PercentSuccess)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<TestPassDTO> GetTestById(int testId)
        {
            var test = await _context.Tests
                .Where(t => t.Id == testId)
                .Select(t => new TestPassDTO
                {
                    Id = t.Id,
                    TestName = t.TestName,
                    TestDescription = t.TestDescription,

                    Questions = t.Questions
                        .OrderBy(q => q.Number)
                        .Select(q => new QuestionPassDTO
                        {
                            Id = q.Id,
                            Number = q.Number,
                            Text = q.Text,

                            Options = q.Options
                                .OrderBy(o => o.Order)
                                .Select(o => new OptionDTO
                                {
                                    Id = o.Id,
                                    Text = o.Text,
                                    Order = o.Order
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (test == null)
                throw new Exception("Test not found");

            return test;
        }

        public async Task<TestResultDTO> SubmitTest(SubmitTestDTO submitTest)
        {
            var questions = await _context.QuestionTests
                .Where(q => q.TestId == submitTest.TestId)
                .ToListAsync();

            if (!questions.Any())
                throw new Exception("Test not found");

            int correct = 0;

            foreach (var answer in submitTest.Answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);

                if (question == null)
                    continue;

                if (question.CorrectAnswer == answer.SelectedOption)
                    correct++;
            }

            int total = questions.Count;

            double percent = Math.Round((double)correct / total * 100, 2);

            var result = new TestResult
            {
                TestId = submitTest.TestId,
                UserId = submitTest.UserId,
                PercentSuccess = percent,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();

            return new TestResultDTO
            {
                TestId = submitTest.TestId,
                UserId = submitTest.UserId,
                PercentSuccess = percent,
                CorrectAnswers = correct,
                TotalQuestions = total
            };
        }

        public async Task<List<TestResultShortDTO>> GetAllResults()
        {
            return await _context.Results
                .Select(r => new TestResultShortDTO
                {
                    Id = r.Id,
                    TestId = r.TestId,
                    TestName = r.Test.TestName,
                    UserId = r.UserId,
                    UserName = r.User.Login,
                    PercentSuccess = r.PercentSuccess,
                    CreatedAt = r.CreatedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserTestResultDTO> GetResultById(int resultId)
        {
            var result = await _context.Results
                .Where(r => r.Id == resultId)
                .Select(r => new UserTestResultDTO
                {
                    Id = r.Id,
                    TestId = r.TestId,
                    TestName = r.Test.TestName,
                    UserId = r.UserId,
                    UserName = r.User.Login,
                    PercentSuccess = r.PercentSuccess,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (result == null)
                throw new Exception("Result not found");

            return result;
        }

        public async Task<Test> CreateTest(CreateTestDTO createTest)
        {
            var book = await _context.Books.FindAsync(createTest.BookId);

            if (book == null)
                throw new Exception("Book not found");

            var generatedTest = await _deepSeekService.GenerateTestAsync(
                book.Id,
                createTest.QuestionQuantity,
                book.Title
            );

            var test = new Test
            {
                BookId = book.Id,
                TestName = $"Тест по произведению {book.Title}",
                TestDescription = createTest.Description
            };

            await _context.Tests.AddAsync(test);
            await _context.SaveChangesAsync();

            var questions = new List<QuestionTest>();
            var options = new List<QuestionOption>();

            foreach (var q in generatedTest.Questions)
            {
                var question = new QuestionTest
                {
                    TestId = test.Id,
                    Number = q.Number,
                    Text = q.Text,
                    CorrectAnswer = q.CorrectAnswer,
                    Explanation = q.Explanation
                };

                questions.Add(question);
            }

            await _context.QuestionTests.AddRangeAsync(questions);
            await _context.SaveChangesAsync();

            foreach (var q in generatedTest.Questions)
            {
                var questionEntity = questions.First(x => x.Number == q.Number);

                for (int i = 0; i < q.Options.Count; i++)
                {
                    options.Add(new QuestionOption
                    {
                        QuestionTestId = questionEntity.Id,
                        Text = q.Options[i],
                        Order = i
                    });
                }
            }

            await _context.QuestionOptions.AddRangeAsync(options);
            await _context.SaveChangesAsync();

            return test;
        }
    }
}
