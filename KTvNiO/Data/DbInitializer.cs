using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KTvNiO.Data;
using KTvNiO.Models;

namespace KTvNiO.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Создаем базу данных если не существует
            if (context.Database.EnsureCreated())
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Создаем роли
                string[] roles = { "Student", "Teacher", "Developer", "Support" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // Создаем пользователей для демонстрации
                var students = new[]
                {
                    new ApplicationUser { UserName = "student1@test.com", Email = "student1@test.com", FullName = "Иванов Иван", Role = UserRole.Student },
                    new ApplicationUser { UserName = "student2@test.com", Email = "student2@test.com", FullName = "Петров Петр", Role = UserRole.Student }
                };

                foreach (var student in students)
                {
                    var existingUser = await userManager.FindByEmailAsync(student.Email);
                    if (existingUser == null)
                    {
                        await userManager.CreateAsync(student, "password123");
                        await userManager.AddToRoleAsync(student, "Student");
                    }
                }

                var teachers = new[]
                {
                    new ApplicationUser { UserName = "teacher1@test.com", Email = "teacher1@test.com", FullName = "Сидоров Сидор", Role = UserRole.Teacher }
                };

                foreach (var teacher in teachers)
                {
                    var existingUser = await userManager.FindByEmailAsync(teacher.Email);
                    if (existingUser == null)
                    {
                        await userManager.CreateAsync(teacher, "password123");
                        await userManager.AddToRoleAsync(teacher, "Teacher");
                    }
                }

                var developers = new[]
                {
                    new ApplicationUser { UserName = "developer1@test.com", Email = "developer1@test.com", FullName = "Разработчиков Разраб", Role = UserRole.Developer }
                };

                foreach (var developer in developers)
                {
                    var existingUser = await userManager.FindByEmailAsync(developer.Email);
                    if (existingUser == null)
                    {
                        await userManager.CreateAsync(developer, "password123");
                        await userManager.AddToRoleAsync(developer, "Developer");
                    }
                }

                var supports = new[]
                {
                    new ApplicationUser { UserName = "support1@test.com", Email = "support1@test.com", FullName = "Поддержкин Поддерж", Role = UserRole.Support }
                };

                foreach (var support in supports)
                {
                    var existingUser = await userManager.FindByEmailAsync(support.Email);
                    if (existingUser == null)
                    {
                        await userManager.CreateAsync(support, "password123");
                        await userManager.AddToRoleAsync(support, "Support");
                    }
                }

                // Создаем курс по решению квадратных уравнений
                if (!context.Courses.Any())
                {
                    var course = new Course
                    {
                        Title = "Решение квадратных уравнений",
                        Description = "Изучение методов решения квадратных уравнений в общем виде",
                        Content = @"
# Решение квадратных уравнений

## Общие сведения

Квадратное уравнение имеет вид: **ax² + bx + c = 0**, где a ≠ 0

## Дискриминант

Дискриминант квадратного уравнения вычисляется по формуле:
**D = b² - 4ac**

## Случаи решения

### Случай 1: D > 0
Уравнение имеет два различных корня:
- x₁ = (-b + √D) / (2a)
- x₂ = (-b - √D) / (2a)

### Случай 2: D = 0
Уравнение имеет один корень (два совпадающих корня):
- x = -b / (2a)

### Случай 3: D < 0
Уравнение не имеет действительных корней

## Пример решения

Решим уравнение: 2x² - 5x + 2 = 0

1. Находим дискриминант:
   D = (-5)² - 4·2·2 = 25 - 16 = 9

2. Так как D > 0, уравнение имеет два корня:
   - x₁ = (5 + 3) / 4 = 2
   - x₂ = (5 - 3) / 4 = 0.5

**Ответ:** x₁ = 2, x₂ = 0.5
",
                        IsPublished = true
                    };

                    context.Courses.Add(course);
                    context.SaveChanges();

                    // Добавляем тестовые вопросы
                    var questions = new[]
                    {
                        new TestQuestion
                        {
                            CourseId = course.Id,
                            QuestionText = "Что такое дискриминант квадратного уравнения?",
                            OptionA = "D = b² - 4ac",
                            OptionB = "D = b² + 4ac",
                            OptionC = "D = b - 4ac",
                            OptionD = "D = b² - ac",
                            CorrectAnswer = 'A'
                        },
                        new TestQuestion
                        {
                            CourseId = course.Id,
                            QuestionText = "Сколько корней имеет квадратное уравнение при D > 0?",
                            OptionA = "Один корень",
                            OptionB = "Два различных корня",
                            OptionC = "Не имеет корней",
                            OptionD = "Бесконечно много корней",
                            CorrectAnswer = 'B'
                        },
                        new TestQuestion
                        {
                            CourseId = course.Id,
                            QuestionText = "Какая формула используется для нахождения корней при D > 0?",
                            OptionA = "x = -b / (2a)",
                            OptionB = "x = (-b ± √D) / (2a)",
                            OptionC = "x = b / (2a)",
                            OptionD = "x = (b ± √D) / a",
                            CorrectAnswer = 'B'
                        },
                        new TestQuestion
                        {
                            CourseId = course.Id,
                            QuestionText = "Что происходит при D = 0?",
                            OptionA = "Два различных корня",
                            OptionB = "Один корень (два совпадающих)",
                            OptionC = "Нет корней",
                            OptionD = "Комплексные корни",
                            CorrectAnswer = 'B'
                        },
                        new TestQuestion
                        {
                            CourseId = course.Id,
                            QuestionText = "Решите уравнение: x² - 4x + 4 = 0",
                            OptionA = "x = 2",
                            OptionB = "x = -2",
                            OptionC = "x = 4",
                            OptionD = "x = 0",
                            CorrectAnswer = 'A'
                        }
                    };

                    context.TestQuestions.AddRange(questions);
                    context.SaveChanges();
                }
            }
        }
    }
}
