namespace Obuchalkin.Models
{
    public class StudentCourseViewModel
    {
        public Course Course { get; set; }
        public int CurrentPage { get; set; }
        public Progress Progress { get; set; }
        public int AttemptsUsed { get; set; } // Новое свойство
    }
}