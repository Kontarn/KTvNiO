using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTvNiO.Models;
using KTvNiO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace KTvNiO.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var questions = await _context.TestQuestions.Where(q => q.CourseId == id).ToListAsync();
            
            ViewBag.Course = course;
            return View(questions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTest(int courseId, Dictionary<int, string> answers)
        {
            var userId = _userManager.GetUserId(User);
            var questions = await _context.TestQuestions.Where(q => q.CourseId == courseId).ToListAsync();
            
            int correctAnswers = 0;
            foreach (var question in questions)
            {
                if (answers.ContainsKey(question.Id) && answers[question.Id][0] == question.CorrectAnswer)
                {
                    correctAnswers++;
                }
            }

            var testResult = new TestResult
            {
                UserId = userId,
                CourseId = courseId,
                TotalQuestions = questions.Count,
                CorrectAnswers = correctAnswers,
                TakenAt = DateTime.Now,
                Duration = TimeSpan.FromMinutes(5) // Можно добавить отслеживание времени
            };

            _context.TestResults.Add(testResult);

            // Обновляем статус курса
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
            
            if (enrollment != null)
            {
                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.Now;
            }

            // Логируем результат теста
            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = userId,
                Action = "TakeTest",
                Details = $"Тест пройден. Результат: {correctAnswers}/{questions.Count} ({testResult.Score:F1}%)",
                Timestamp = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Result", new { id = testResult.Id });
        }

        public async Task<IActionResult> Result(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _context.TestResults
                .Include(r => r.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        public async Task<IActionResult> History()
        {
            var userId = _userManager.GetUserId(User);
            var results = await _context.TestResults
                .Include(r => r.Course)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.TakenAt)
                .ToListAsync();

            return View(results);
        }
    }
}
