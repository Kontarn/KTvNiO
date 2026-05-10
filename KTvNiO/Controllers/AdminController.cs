using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTvNiO.Models;
using KTvNiO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace KTvNiO.Controllers
{
    [Authorize(Roles = "Developer, Support")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var courses = await _context.Courses.ToListAsync();
            var enrollments = await _context.Enrollments.Include(e => e.User).Include(e => e.Course).ToListAsync();
            
            ViewBag.Users = users;
            ViewBag.Courses = courses;
            ViewBag.Enrollments = enrollments;
            
            return View();
        }

        public async Task<IActionResult> Logs()
        {
            var logs = await _context.ActivityLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToListAsync();
            
            return View(logs);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
            }
            
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Results()
        {
            var results = await _context.TestResults
                .Include(r => r.User)
                .Include(r => r.Course)
                .OrderByDescending(r => r.TakenAt)
                .ToListAsync();
            
            return View(results);
        }
    }
}
