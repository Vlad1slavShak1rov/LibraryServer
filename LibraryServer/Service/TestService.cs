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

        public async Task<List<TestShortDTO>> GetAllTests()
        {
            return await _context.Tests
                .Select(t => new TestShortDTO
                {
                    Id = t.Id,
                    TestName = t.TestName,
                    TestDescription = t.TestDescription,

                    QuestionCount = t.Questions.Count,
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
            // Получаем вопросы теста вместе с вариантами ответов
            var questions = await _context.QuestionTests
                .Include(q => q.Options)
                .Where(q => q.TestId == submitTest.TestId)
                .ToListAsync();

            if (!questions.Any())
                throw new Exception("Test not found");

            // Ищем назначенный тест
            // В AssignedTest.StudentId хранится User.Id
            var assigned = await _context.AssignedTest
                .FirstOrDefaultAsync(x =>
                    x.StudentId == submitTest.UserId &&
                    x.TestId == submitTest.TestId);

            // Подсчёт правильных ответов
            int correct = 0;

            foreach (var answer in submitTest.Answers)
            {
                // Находим вопрос
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);

                if (question == null)
                    continue;

                /*
                 * В submitTest.Answers.SelectedOption приходит ID записи QuestionOption,
                 * например 121, 127, 130 и т.д.
                 *
                 * В QuestionTest.CorrectAnswer хранится значение поля Order
                 * правильного варианта (0, 1, 2, 3).
                 *
                 * Поэтому:
                 * 1. Находим QuestionOption по Id == SelectedOption
                 * 2. Берём его Order
                 * 3. Сравниваем с CorrectAnswer
                 */

                var selectedOption = question.Options
                    .FirstOrDefault(o => o.Id == answer.SelectedOption);

                if (selectedOption == null)
                    continue;

                if (selectedOption.Order == question.CorrectAnswer)
                {
                    correct++;
                }
            }

            int total = questions.Count;

            double percent = total == 0
                ? 0
                : Math.Round((double)correct / total * 100, 2);

            // Сохраняем результат
            var result = new TestResult
            {
                TestId = submitTest.TestId,
                UserId = submitTest.UserId,
                PercentSuccess = percent,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Results.AddAsync(result);

            // Отмечаем назначенный тест как выполненный
            if (assigned != null)
            {
                assigned.IsCompleted = true;
            }

            // Сохраняем изменения
            await _context.SaveChangesAsync();

            // Возвращаем результат
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
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
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
                foreach (var q in generatedTest.Questions)
                {
                    questions.Add(new QuestionTest
                    {
                        TestId = test.Id,
                        Number = q.Number,
                        Text = q.Text,
                        CorrectAnswer = q.CorrectAnswer,
                        Explanation = q.Explanation
                    });
                }

                await _context.QuestionTests.AddRangeAsync(questions);
                await _context.SaveChangesAsync();

                var options = new List<QuestionOption>();
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

                await transaction.CommitAsync();

                return test;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AssignedTest> AssignTest(AssignTestDTO dto)
        {
            // Проверяем, не выдан ли уже этот тест этому ученику
            var existing = await _context.AssignedTest
                .FirstOrDefaultAsync(x =>
                    x.StudentId == dto.StudentId &&
                    x.TestId == dto.TestId);

            if (existing != null)
                throw new Exception("Этот тест уже назначен данному ученику.");

            var assignedTest = new AssignedTest
            {
                StudentId = dto.StudentId,   // сюда передается User.Id ученика
                TeacherId = dto.TeacherId,   // сюда передается User.Id учителя
                TestId = dto.TestId,
                DueDate = dto.DueDate,
                AssignedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            _context.AssignedTest.Add(assignedTest);
            await _context.SaveChangesAsync();

            return assignedTest;
        }

        public async Task<List<AssignedTestDTO>> GetUserAssignedTests(int userId)
        {
            return await _context.AssignedTest
                .Where(x => x.StudentId == userId)
                .Include(x => x.Test)
                .Select(x => new AssignedTestDTO
                {
                    Id = x.Id,
                    TestId = x.TestId,
                    TestName = x.Test.TestName,

                    StudentId = x.StudentId,
                    StudentName = x.Student.Login,

                    TeacherId = x.TeacherId ?? 0,
                    TeacherName = x.Teacher != null ? x.Teacher.Login : "",

                    AssignedAt = x.AssignedAt,
                    DueDate = x.DueDate,
                    IsCompleted = x.IsCompleted,

                    Percent = _context.Results
                        .Where(r =>
                            r.TestId == x.TestId &&
                            r.UserId == x.StudentId)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => (double?)r.PercentSuccess)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();
        }

        public async Task<List<AssignedTestDTO>> GetAssignedTestsByTeacher(int teacherId)
        {
            return await _context.AssignedTest
                .Where(x => x.TeacherId == teacherId)
                .Include(x => x.Test)
                .Include(x => x.Student)
                .Include(x => x.Teacher)
                .Select(x => new AssignedTestDTO
                {
                    Id = x.Id,

                    TestId = x.TestId,
                    TestName = x.Test.TestName,

                    StudentId = x.StudentId,
                    StudentName = x.Student.Login.ToString(), // или через Users

                    TeacherId = x.TeacherId.Value,
                    TeacherName = x.Teacher.Login.ToString(),

                    AssignedAt = x.AssignedAt,
                    DueDate = x.DueDate,
                    IsCompleted = x.IsCompleted,

                    Percent = _context.Results
                        .Where(r => r.TestId == x.TestId && r.UserId == x.Student.Id)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => (double?)r.PercentSuccess)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();
        }
    }
}
