using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KTvNiO.Models;
using KTvNiO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace KTvNiO.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.Where(c => c.IsPublished).ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            
            // Проверяем, не записан ли уже пользователь на курс
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (existingEnrollment == null)
            {
                var enrollment = new Enrollment
                {
                    CourseId = courseId,
                    UserId = userId,
                    EnrolledAt = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                
                // Логируем запись на курс
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = userId,
                    Action = "Enroll",
                    Details = $"Запись на курс {courseId}",
                    Timestamp = DateTime.Now
                });
                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Learn", new { id = courseId });
        }

        public async Task<IActionResult> Learn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            
            // Проверяем, записан ли пользователь на курс
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == id && e.UserId == userId);

            if (enrollment == null)
            {
                // Автоматически записываем на курс
                enrollment = new Enrollment
                {
                    CourseId = id.Value,
                    UserId = userId,
                    EnrolledAt = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = userId,
                    Action = "StartCourse",
                    Details = $"Начало изучения курса {course.Title}",
                    Timestamp = DateTime.Now
                });
                
                await _context.SaveChangesAsync();
            }
            else if (!enrollment.IsCompleted)
            {
                // Логируем продолжение изучения
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = userId,
                    Action = "ContinueCourse",
                    Details = $"Продолжение изучения курса {course.Title}",
                    Timestamp = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            ViewBag.Enrollment = enrollment;
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteCourse(int courseId)
        {
            var userId = _userManager.GetUserId(User);
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

            if (enrollment != null)
            {
                enrollment.IsCompleted = true;
                enrollment.CompletedAt = DateTime.Now;
                
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = userId,
                    Action = "CompleteCourse",
                    Details = $"Завершение курса {courseId}",
                    Timestamp = DateTime.Now
                });
                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Test", new { id = courseId });
        }
    }
}
