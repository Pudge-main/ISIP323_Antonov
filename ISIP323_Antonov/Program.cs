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

        public Student(string name, int age, string email, int studentId)
            : base(name, age, email)
        {
            if (studentId <= 0) throw new ArgumentException("StudentId должен быть положительным");
            StudentId = studentId;
        }

        public void Enroll(Course course) { }
        public List<Course> GetEnrolledCourses() { return null; }
        public override void ShowInfo() { }
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

        public void AssignCourse(Course course) { }
        public override void ShowInfo() { }
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

        public bool EnrollStudent(Student student) { return false; }
        public void RemoveStudent(Student student) { }
        public void ShowInfo() { }
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

        public void AddStudent(Student student) { }
        public Student GetStudentById(int id) { return null; }

        public void AddTeacher(Teacher teacher) { }
        public Teacher GetTeacherById(int id) { return null; }

        public void AddCourse(Course course) { }
        public Course GetCourseById(int id) { return null; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Программа: система управления университетом (версия: базовые конструкторы и валидация)");
        }
    }
}
