namespace SubstitutionService.Models
{
    public class Lesson
    {
        public int Number { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public string Substitute { get; set; }
        public string Comment { get; set; }
    }
}