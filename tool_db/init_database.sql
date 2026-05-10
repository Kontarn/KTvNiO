-- =============================================
-- Скрипт создания и наполнения БД LmsDb
-- Система дистанционного обучения (LMS)
-- =============================================

USE master;
GO

-- Удаляем базу данных, если она существует
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'LmsDb')
BEGIN
    ALTER DATABASE [LmsDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [LmsDb];
END
GO

-- Создаем базу данных
CREATE DATABASE [LmsDb];
GO

USE [LmsDb];
GO

-- =============================================
-- Создание таблиц
-- =============================================

-- Таблица пользователей
CREATE TABLE [Users] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(256) NOT NULL,
    [FullName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [UserType] INT NOT NULL, -- 1: Слушатель, 2: Преподаватель, 3: Разработчик, 4: Техподдержка
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
);
GO

-- Таблица курсов
CREATE TABLE [Courses] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [AuthorId] INT FOREIGN KEY REFERENCES [Users]([Id]),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
);
GO

-- Таблица записей на курсы
CREATE TABLE [Enrollments] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [CourseId] INT NOT NULL FOREIGN KEY REFERENCES [Courses]([Id]),
    [EnrolledAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CompletedAt] DATETIME2 NULL,
    [Status] INT NOT NULL DEFAULT 1, -- 1: В процессе, 2: Завершен, 3: Отменен
    CONSTRAINT [UK_Enrollment] UNIQUE ([UserId], [CourseId])
);
GO

-- Таблица материалов курса
CREATE TABLE [CourseMaterials] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [CourseId] INT NOT NULL FOREIGN KEY REFERENCES [Courses]([Id]),
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [OrderIndex] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Таблица тестов
CREATE TABLE [Tests] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [CourseId] INT NOT NULL FOREIGN KEY REFERENCES [Courses]([Id]),
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [PassingScore] INT NOT NULL DEFAULT 70, -- Процент для прохождения
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Таблица вопросов теста
CREATE TABLE [Questions] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TestId] INT NOT NULL FOREIGN KEY REFERENCES [Tests]([Id]),
    [QuestionText] NVARCHAR(MAX) NOT NULL,
    [QuestionType] INT NOT NULL DEFAULT 1, -- 1: Один вариант, 2: Несколько вариантов
    [OrderIndex] INT NOT NULL DEFAULT 0
);
GO

-- Таблица вариантов ответов
CREATE TABLE [Answers] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [QuestionId] INT NOT NULL FOREIGN KEY REFERENCES [Questions]([Id]),
    [AnswerText] NVARCHAR(MAX) NOT NULL,
    [IsCorrect] BIT NOT NULL DEFAULT 0,
    [OrderIndex] INT NOT NULL DEFAULT 0
);
GO

-- Таблица результатов тестирования
CREATE TABLE [TestResults] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TestId] INT NOT NULL FOREIGN KEY REFERENCES [Tests]([Id]),
    [UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [StartedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CompletedAt] DATETIME2 NULL,
    [Score] INT NULL, -- Количество правильных ответов
    [TotalQuestions] INT NULL,
    [Percentage] DECIMAL(5,2) NULL, -- Процент результата
    [Passed] BIT NULL,
    [Status] INT NOT NULL DEFAULT 1 -- 1: В процессе, 2: Завершен
);
GO

-- Таблица ответов пользователя в тесте
CREATE TABLE [UserAnswers] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [TestResultId] INT NOT NULL FOREIGN KEY REFERENCES [TestResults]([Id]),
    [QuestionId] INT NOT NULL FOREIGN KEY REFERENCES [Questions]([Id]),
    [AnswerId] INT NOT NULL FOREIGN KEY REFERENCES [Answers]([Id]),
    [IsCorrect] BIT NOT NULL DEFAULT 0,
    [AnsweredAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- Таблица логов действий
CREATE TABLE [ActivityLogs] (
    [Id] BIGINT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [ActionType] NVARCHAR(50) NOT NULL, -- Login, Logout, ViewMaterial, StartTest, CompleteTest, Enroll, etc.
    [EntityType] NVARCHAR(50) NULL, -- User, Course, Test, Material, etc.
    [EntityId] INT NULL,
    [Details] NVARCHAR(MAX) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL
);
GO

-- =============================================
-- Заполнение данными
-- =============================================

-- Создаем пользователей
-- Пароли захешированы (для примера используем простой хэш, в реальности нужен bcrypt/Argon2)
-- Пароль для всех: "password123"
INSERT INTO [Users] ([Username], [PasswordHash], [FullName], [Email], [UserType]) VALUES
('student1', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Иванов Иван', 'ivanov@student.com', 1),
('student2', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Петров Петр', 'petrov@student.com', 1),
('student3', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Сидорова Анна', 'sidorova@student.com', 1),
('teacher1', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Смирнов Алексей', 'smirnov@teacher.com', 2),
('teacher2', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Кузнецова Мария', 'kuznetsova@teacher.com', 2),
('developer1', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Разработчиков Дмитрий', 'dev@system.com', 3),
('support1', 'AQAAAAEAACcQAAAAEOKqGhH8VxPzJZLvN9K5R8vF3qF5J5mZ8Y5nJ5mZ8Y5nJ5mZ8Y5n==', 'Поддержкин Сергей', 'support@system.com', 4);
GO

-- Создаем курсы
DECLARE @Teacher1Id INT = (SELECT Id FROM Users WHERE Username = 'teacher1');
DECLARE @Teacher2Id INT = (SELECT Id FROM Users WHERE Username = 'teacher2');

INSERT INTO [Courses] ([Title], [Description], [AuthorId]) VALUES
('Решение квадратных уравнений', 
 'Курс посвящен изучению методов решения квадратных уравнений в общем виде. Вы научитесь находить дискриминант, определять количество корней и применять формулы для решения.', 
 @Teacher1Id),
('Основы алгебры', 
 'Вводный курс по алгебре для начинающих. Охватывает основные понятия, линейные уравнения и неравенства.', 
 @Teacher2Id),
('Высшая математика', 
 'Продвинутый курс по высшей математике: пределы, производные, интегралы.', 
 @Teacher1Id);
GO

-- Записываем студентов на курсы
DECLARE @Student1Id INT = (SELECT Id FROM Users WHERE Username = 'student1');
DECLARE @Student2Id INT = (SELECT Id FROM Users WHERE Username = 'student2');
DECLARE @Student3Id INT = (SELECT Id FROM Users WHERE Username = 'student3');
DECLARE @Course1Id INT = (SELECT Id FROM Courses WHERE Title = 'Решение квадратных уравнений');
DECLARE @Course2Id INT = (SELECT Id FROM Courses WHERE Title = 'Основы алгебры');
DECLARE @Course3Id INT = (SELECT Id FROM Courses WHERE Title = 'Высшая математика');

INSERT INTO [Enrollments] ([UserId], [CourseId], [Status]) VALUES
(@Student1Id, @Course1Id, 1),
(@Student1Id, @Course2Id, 2),
(@Student2Id, @Course1Id, 1),
(@Student2Id, @Course3Id, 1),
(@Student3Id, @Course1Id, 1),
(@Student3Id, @Course2Id, 1);
GO

-- Добавляем материалы для курса "Решение квадратных уравнений"
INSERT INTO [CourseMaterials] ([CourseId], [Title], [Content], [OrderIndex]) VALUES
(@Course1Id, 'Введение в квадратные уравнения', 
 '<h2>Что такое квадратное уравнение?</h2>
  <p>Квадратное уравнение — это уравнение вида <strong>ax² + bx + c = 0</strong>, где:</p>
  <ul>
    <li><strong>a</strong> — старший коэффициент (a ≠ 0)</li>
    <li><strong>b</strong> — второй коэффициент</li>
    <li><strong>c</strong> — свободный член</li>
  </ul>
  <h3>Примеры квадратных уравнений:</h3>
  <ul>
    <li>2x² + 5x - 3 = 0</li>
    <li>x² - 4 = 0</li>
    <li>3x² + 7x = 0</li>
  </ul>', 1),
(@Course1Id, 'Дискриминант и количество корней', 
 '<h2>Дискриминант квадратного уравнения</h2>
  <p>Дискриминант вычисляется по формуле: <strong>D = b² - 4ac</strong></p>
  <h3>Возможные случаи:</h3>
  <ol>
    <li><strong>D > 0</strong> — уравнение имеет два различных корня</li>
    <li><strong>D = 0</strong> — уравнение имеет один корень (два совпадающих)</li>
    <li><strong>D < 0</strong> — уравнение не имеет действительных корней</li>
  </ol>
  <h3>Пример:</h3>
  <p>Для уравнения 2x² + 5x - 3 = 0:</p>
  <p>D = 5² - 4·2·(-3) = 25 + 24 = 49 > 0 → два корня</p>', 2),
(@Course1Id, 'Формула корней квадратного уравнения', 
 '<h2>Нахождение корней</h2>
  <p>Корни квадратного уравнения находятся по формуле:</p>
  <p><strong>x₁,₂ = (-b ± √D) / 2a</strong></p>
  <h3>Пример решения:</h3>
  <p>Решим уравнение 2x² + 5x - 3 = 0</p>
  <ol>
    <li>Находим D = 49</li>
    <li>√D = 7</li>
    <li>x₁ = (-5 + 7) / 4 = 2/4 = 0.5</li>
    <li>x₂ = (-5 - 7) / 4 = -12/4 = -3</li>
  </ol>
  <p><strong>Ответ:</strong> x₁ = 0.5, x₂ = -3</p>', 3),
(@Course1Id, 'Неполные квадратные уравнения', 
 '<h2>Неполные квадратные уравнения</h2>
  <p>Это уравнения, у которых один или два коэффициента равны нулю.</p>
  <h3>Типы неполных уравнений:</h3>
  <ol>
    <li><strong>ax² = 0</strong> → x = 0</li>
    <li><strong>ax² + bx = 0</strong> → x(ax + b) = 0 → x₁ = 0, x₂ = -b/a</li>
    <li><strong>ax² + c = 0</strong> → x² = -c/a → x = ±√(-c/a)</li>
  </ol>', 4);
GO

-- Создаем тест для курса "Решение квадратных уравнений"
DECLARE @Test1Id INT;
INSERT INTO [Tests] ([CourseId], [Title], [Description], [PassingScore]) VALUES
(@Course1Id, 'Проверка знаний по теме: Квадратные уравнения', 
 'Тест содержит 5 вопросов. Для успешного прохождения необходимо набрать минимум 70% правильных ответов.', 
 70);
SET @Test1Id = SCOPE_IDENTITY();
GO

-- Добавляем вопросы к тесту
DECLARE @Q1Id INT, @Q2Id INT, @Q3Id INT, @Q4Id INT, @Q5Id INT;

INSERT INTO [Questions] ([TestId], [QuestionText], [QuestionType], [OrderIndex]) VALUES
(@Test1Id, 'Какая формула используется для вычисления дискриминанта?', 1, 1),
(@Test1Id, 'Сколько корней имеет квадратное уравнение, если D > 0?', 1, 2),
(@Test1Id, 'Найдите корни уравнения: x² - 5x + 6 = 0', 1, 3),
(@Test1Id, 'Выберите все верные утверждения о неполных квадратных уравнениях:', 2, 4),
(@Test1Id, 'Чему равен дискриминант уравнения 3x² + 6x + 3 = 0?', 1, 5);

SET @Q1Id = SCOPE_IDENTITY();
SET @Q2Id = @Q1Id + 1;
SET @Q3Id = @Q1Id + 2;
SET @Q4Id = @Q1Id + 3;
SET @Q5Id = @Q1Id + 4;
GO

-- Добавляем варианты ответов
-- Вопрос 1
INSERT INTO [Answers] ([QuestionId], [AnswerText], [IsCorrect], [OrderIndex]) VALUES
(@Q1Id, 'D = b² - 4ac', 1, 1),
(@Q1Id, 'D = b² + 4ac', 0, 2),
(@Q1Id, 'D = 2b - 4ac', 0, 3),
(@Q1Id, 'D = b - 4ac', 0, 4);

-- Вопрос 2
INSERT INTO [Answers] ([QuestionId], [AnswerText], [IsCorrect], [OrderIndex]) VALUES
(@Q2Id, 'Два различных корня', 1, 1),
(@Q2Id, 'Один корень', 0, 2),
(@Q2Id, 'Нет корней', 0, 3),
(@Q2Id, 'Бесконечно много корней', 0, 4);

-- Вопрос 3
INSERT INTO [Answers] ([QuestionId], [AnswerText], [IsCorrect], [OrderIndex]) VALUES
(@Q3Id, 'x₁ = 2, x₂ = 3', 1, 1),
(@Q3Id, 'x₁ = -2, x₂ = -3', 0, 2),
(@Q3Id, 'x₁ = 1, x₂ = 6', 0, 3),
(@Q3Id, 'Уравнение не имеет корней', 0, 4);

-- Вопрос 4 (несколько правильных ответов)
INSERT INTO [Answers] ([QuestionId], [AnswerText], [IsCorrect], [OrderIndex]) VALUES
(@Q4Id, 'Уравнение ax² = 0 имеет один корень x = 0', 1, 1),
(@Q4Id, 'Уравнение ax² + c = 0 всегда имеет два корня', 0, 2),
(@Q4Id, 'Уравнение ax² + bx = 0 можно решить разложением на множители', 1, 3),
(@Q4Id, 'Неполное уравнение не может иметь дискриминант', 0, 4);

-- Вопрос 5
INSERT INTO [Answers] ([QuestionId], [AnswerText], [IsCorrect], [OrderIndex]) VALUES
(@Q5Id, '0', 1, 1),
(@Q5Id, '12', 0, 2),
(@Q5Id, '-12', 0, 3),
(@Q5Id, '36', 0, 4);
GO

-- Логируем действия при создании данных
DECLARE @DevUserId INT = (SELECT Id FROM Users WHERE Username = 'developer1');

INSERT INTO [ActivityLogs] ([UserId], [ActionType], [EntityType], [EntityId], [Details]) VALUES
(@DevUserId, 'DatabaseInitialized', 'Database', 1, 'База данных создана и заполнена начальными данными'),
(@DevUserId, 'UsersCreated', 'Users', 7, 'Создано 7 пользователей разных типов'),
(@DevUserId, 'CoursesCreated', 'Courses', 3, 'Создано 3 курса'),
(@DevUserId, 'TestsCreated', 'Tests', 1, 'Создан 1 тест с 5 вопросами');
GO

-- =============================================
-- Проверка созданных данных
-- =============================================

PRINT 'База данных LmsDb успешно создана и заполнена!';
PRINT '';
PRINT 'Пользователи:';
SELECT UserType, COUNT(*) as Count FROM Users GROUP BY UserType;
PRINT '';
PRINT 'Курсы:';
SELECT Title FROM Courses;
PRINT '';
PRINT 'Тесты:';
SELECT t.Title, c.Title as CourseName FROM Tests t JOIN Courses c ON t.CourseId = c.Id;
PRINT '';
PRINT 'Вопросы в тестах:';
SELECT q.QuestionText, t.Title as TestName FROM Questions q JOIN Tests t ON q.TestId = t.Id;
GO
