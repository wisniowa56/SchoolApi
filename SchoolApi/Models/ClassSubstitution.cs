using SubstitutionService.Models;

namespace SchoolApi.Models
{
    public class ClassSubstitution
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
    }
}