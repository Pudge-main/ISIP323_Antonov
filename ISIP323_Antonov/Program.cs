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
            if (ok)
            {
                _enrolledCourses.Add(course);
            }
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
        static void Main(string[] args)
        {
            Console.WriteLine("Программа: система управления университетом (версия: регистрация на курс)");
        }
    }
}
