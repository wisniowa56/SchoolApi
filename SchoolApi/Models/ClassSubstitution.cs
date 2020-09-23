using SubstitutionService.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SchoolApi.Models
{
    public class ClassSubstitution : IComparable<ClassSubstitution>
    {
        public ClassSubstitution()
        {

        }

        public ClassSubstitution(Lesson lesson, string teacher)
        {
            Number = lesson.Number;
            Description = lesson.Description;
            Teacher = teacher;
            Substitute = lesson.Substitute;
            Comment = lesson.Comment;
        }

        public int Number { get; set; }
        public string Description { get; set; }
        public string Teacher { get; set; }
        public string Substitute { get; set; }
        public string Comment { get; set; }

        public int CompareTo(ClassSubstitution other)
        {
            return Number.CompareTo(other.Number);
        }
    }
}