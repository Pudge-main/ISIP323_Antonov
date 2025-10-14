using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversityApp
{
    public abstract class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

        protected Person(string name, int age, string email) { }

        public abstract void ShowInfo();
    }

    public class Student : Person
    {
        public int StudentId { get; set; }
        public List<Grade> Grades { get; set; }

        public Student(string name, int age, string email, int studentId)
            : base(name, age, email) { }

        public void Enroll(Course course) { }
        public List<Course> GetEnrolledCourses() { return null; }
        public override void ShowInfo() { }
    }
    public class Teacher : Person
    {
        public int TeacherId { get; set; }

        public Teacher(string name, int age, string email, int teacherId)
            : base(name, age, email) { }

        public void AssignCourse(Course course) { }
        public override void ShowInfo() { }
    }
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public Teacher AssignedTeacher { get; set; }
        public List<Student> EnrolledStudents { get; set; }
        public int MaxStudents { get; set; }

        public Course(int courseId, string title, int maxStudents) { }

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
        public List<Student> Students { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Course> Courses { get; set; }

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
            Console.WriteLine("Программа: система управления университетом (версия: декомпозиция)");
        }
    }
}
