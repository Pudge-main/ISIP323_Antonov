using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityApp
{
    public abstract class Person
    {
        public string Name { get; protected set; }
        public int Age { get; protected set; }
        public string Email { get; protected set; }

        protected Person(string name, int age, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name не может быть пустым");
            if (age <= 0)
                throw new ArgumentException("Age должен быть положительным");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email должен быть корректным");

            Name = name;
            Age = age;
            Email = email;
        }

        public abstract void ShowInfo();
    }

    public class Student : Person
    {
        public int StudentId { get; private set; }
        public List<Grade> Grades { get; private set; } = new List<Grade>();
        private List<Course> _enrolledCourses = new List<Course>();

        public Student(string name, int age, string email, int studentId)
            : base(name, age, email)
        {
            if (studentId <= 0) throw new ArgumentException("StudentId должен быть положительным");
            StudentId = studentId;
        }

        public void Enroll(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            if (_enrolledCourses.Contains(course)) return;
            bool ok = course.EnrollStudent(this);
            if (ok) _enrolledCourses.Add(course);
            else Console.WriteLine("Не удалось записать на курс — возможно он полон.");
        }

        public List<Course> GetEnrolledCourses() => _enrolledCourses.ToList();

        public override void ShowInfo()
        {
            Console.WriteLine($"Студент #{StudentId}: {Name}, {Age} лет, {Email}");
        }
    }

    public class Teacher : Person
    {
        public int TeacherId { get; private set; }

        public Teacher(string name, int age, string email, int teacherId)
            : base(name, age, email)
        {
            if (teacherId <= 0) throw new ArgumentException("TeacherId должен быть положительным");
            TeacherId = teacherId;
        }

        public void AssignCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            course.AssignedTeacher = this;
        }

        public override void ShowInfo()
        {
            Console.WriteLine($"Преподаватель #{TeacherId}: {Name}, {Age} лет, {Email}");
        }
    }

    public class Course
    {
        public int CourseId { get; private set; }
        public string Title { get; private set; }
        public Teacher AssignedTeacher { get; set; }
        public List<Student> EnrolledStudents { get; private set; } = new List<Student>();
        public int MaxStudents { get; private set; }

        public Course(int courseId, string title, int maxStudents)
        {
            if (courseId <= 0) throw new ArgumentException("CourseId должен быть положительным");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title не может быть пустым");
            if (maxStudents <= 0) throw new ArgumentException("MaxStudents должен быть положительным");

            CourseId = courseId;
            Title = title;
            MaxStudents = maxStudents;
        }

        public bool EnrollStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (EnrolledStudents.Contains(student)) return true;
            if (EnrolledStudents.Count >= MaxStudents) return false;
            EnrolledStudents.Add(student);
            return true;
        }

        public void RemoveStudent(Student student)
        {
            if (student == null) return;
            EnrolledStudents.Remove(student);
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Курс #{CourseId}: {Title} (макс {MaxStudents}), Преподаватель: {(AssignedTeacher != null ? AssignedTeacher.Name : "не назначен")}");
            Console.WriteLine($"Записано студентов: {EnrolledStudents.Count}");
        }
    }

    public class Grade
    {
        public int CourseId { get; set; }
        public double Value { get; set; }
    }

    public class University
    {
        public List<Student> Students { get; private set; } = new List<Student>();
        public List<Teacher> Teachers { get; private set; } = new List<Teacher>();
        public List<Course> Courses { get; private set; } = new List<Course>();

        public University() { }

        public void AddStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            if (Students.Any(s => s.StudentId == student.StudentId))
                throw new ArgumentException("Студент с таким ID уже есть");
            Students.Add(student);
        }

        public Student GetStudentById(int id) => Students.FirstOrDefault(s => s.StudentId == id);

        public void AddTeacher(Teacher teacher)
        {
            if (teacher == null) throw new ArgumentNullException(nameof(teacher));
            if (Teachers.Any(t => t.TeacherId == teacher.TeacherId))
                throw new ArgumentException("Преподаватель с таким ID уже есть");
            Teachers.Add(teacher);
        }

        public Teacher GetTeacherById(int id) => Teachers.FirstOrDefault(t => t.TeacherId == id);

        public void AddCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            if (Courses.Any(c => c.CourseId == course.CourseId))
                throw new ArgumentException("Курс с таким ID уже есть");
            Courses.Add(course);
        }

        public Course GetCourseById(int id) => Courses.FirstOrDefault(c => c.CourseId == id);
    }

    class Program
    {
        static University uni = new University();

        static void Main(string[] args)
        {
            Console.WriteLine("Система управления университетом — простой CLI (версия: меню)");
            SeedSampleData();

            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                if (choice == "0") break;
                HandleChoice(choice);
            }

            Console.WriteLine("Выход. До свидания.");
        }

        static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Меню:");
            Console.WriteLine("1 - Добавить студента");
            Console.WriteLine("2 - Просмотреть всех студентов");
            Console.WriteLine("3 - Добавить преподавателя");
            Console.WriteLine("4 - Просмотреть всех преподавателей");
            Console.WriteLine("5 - Создать курс");
            Console.WriteLine("6 - Просмотреть все курсы");
            Console.WriteLine("7 - Записать студента на курс");
            Console.WriteLine("8 - Показать курсы студента");
            Console.WriteLine("9 - Показать студентов курса");
            Console.WriteLine("0 - Выход");
            Console.Write("Выберите пункт: ");
        }

        static void HandleChoice(string choice)
        {
            try
            {
                switch (choice)
                {
                    case "1": AddStudentCli(); break;
                    case "2": ListStudents(); break;
                    case "3": AddTeacherCli(); break;
                    case "4": ListTeachers(); break;
                    case "5": CreateCourseCli(); break;
                    case "6": ListCourses(); break;
                    case "7": EnrollStudentCli(); break;
                    case "8": ShowCoursesOfStudentCli(); break;
                    case "9": ShowStudentsOfCourseCli(); break;
                    default: Console.WriteLine("Неизвестный пункт"); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void SeedSampleData()
        {
            try
            {
                var t1 = new Teacher("Иван Иванов", 40, "ivan@example.com", 1);
                var t2 = new Teacher("Елена Петрова", 35, "elena@example.com", 2);
                uni.AddTeacher(t1);
                uni.AddTeacher(t2);

                var s1 = new Student("Алексей", 20, "alex@example.com", 101);
                var s2 = new Student("Мария", 19, "maria@example.com", 102);
                uni.AddStudent(s1);
                uni.AddStudent(s2);

                var c1 = new Course(201, "Математика", 30);
                var c2 = new Course(202, "Программирование", 2);
                uni.AddCourse(c1);
                uni.AddCourse(c2);

                t1.AssignCourse(c1);
                t2.AssignCourse(c2);
            }
            catch {  }
        }

        static void AddStudentCli()
        {
            Console.Write("Имя: "); string name = Console.ReadLine();
            Console.Write("Возраст: "); if (!int.TryParse(Console.ReadLine(), out int age)) { Console.WriteLine("Неправильный возраст"); return; }
            Console.Write("Email: "); string email = Console.ReadLine();
            Console.Write("ID студента (число): "); if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Неправильный ID"); return; }

            var student = new Student(name, age, email, id);
            uni.AddStudent(student);
            Console.WriteLine("Студент добавлен.");
        }

        static void ListStudents()
        {
            if (!uni.Students.Any()) { Console.WriteLine("Студентов нет."); return; }
            foreach (var s in uni.Students) s.ShowInfo();
        }

        static void AddTeacherCli()
        {
            Console.Write("Имя: "); string name = Console.ReadLine();
            Console.Write("Возраст: "); if (!int.TryParse(Console.ReadLine(), out int age)) { Console.WriteLine("Неправильный возраст"); return; }
            Console.Write("Email: "); string email = Console.ReadLine();
            Console.Write("ID преподавателя (число): "); if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Неправильный ID"); return; }

            var teacher = new Teacher(name, age, email, id);
            uni.AddTeacher(teacher);
            Console.WriteLine("Преподаватель добавлен.");
        }

        static void ListTeachers()
        {
            if (!uni.Teachers.Any()) { Console.WriteLine("Преподавателей нет."); return; }
            foreach (var t in uni.Teachers) t.ShowInfo();
        }

        static void CreateCourseCli()
        {
            Console.Write("ID курса: "); if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Неправильный ID"); return; }
            Console.Write("Название курса: "); string title = Console.ReadLine();
            Console.Write("Максимум студентов: "); if (!int.TryParse(Console.ReadLine(), out int max)) { Console.WriteLine("Неправильное число"); return; }

            var course = new Course(id, title, max);
            uni.AddCourse(course);
            Console.WriteLine("Курс создан.");
        }

        static void ListCourses()
        {
            if (!uni.Courses.Any()) { Console.WriteLine("Курсов нет."); return; }
            foreach (var c in uni.Courses)
            {
                c.ShowInfo();
            }
        }

        static void EnrollStudentCli()
        {
            Console.Write("ID студента: "); if (!int.TryParse(Console.ReadLine(), out int sid)) { Console.WriteLine("Неправильный ID"); return; }
            Console.Write("ID курса: "); if (!int.TryParse(Console.ReadLine(), out int cid)) { Console.WriteLine("Неправильный ID"); return; }

            var student = uni.GetStudentById(sid);
            var course = uni.GetCourseById(cid);
            if (student == null) { Console.WriteLine("Студент не найден"); return; }
            if (course == null) { Console.WriteLine("Курс не найден"); return; }

            student.Enroll(course);
            Console.WriteLine("Операция записи: выполнена (если курс не был полон).");
        }

        static void ShowCoursesOfStudentCli()
        {
            Console.Write("ID студента: "); if (!int.TryParse(Console.ReadLine(), out int sid)) { Console.WriteLine("Неправильный ID"); return; }
            var student = uni.GetStudentById(sid);
            if (student == null) { Console.WriteLine("Студент не найден"); return; }

            var courses = student.GetEnrolledCourses();
            if (!courses.Any()) { Console.WriteLine("Студент не записан ни на один курс."); return; }
            foreach (var c in courses) Console.WriteLine($"#{c.CourseId} {c.Title}");
        }

        static void ShowStudentsOfCourseCli()
        {
            Console.Write("ID курса: "); if (!int.TryParse(Console.ReadLine(), out int cid)) { Console.WriteLine("Неправильный ID"); return; }
            var course = uni.GetCourseById(cid);
            if (course == null) { Console.WriteLine("Курс не найден"); return; }

            if (!course.EnrolledStudents.Any()) { Console.WriteLine("На курсе нет студентов."); return; }
            foreach (var s in course.EnrolledStudents) Console.WriteLine($"#{s.StudentId} {s.Name}");
        }
    }
}
