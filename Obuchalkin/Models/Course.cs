// Models/Course.cs
using System.Collections.Generic;

namespace Obuchalkin.Models
{
    public class Course
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Page> Pages { get; set; } = new List<Page>();
        public List<Test> Tests { get; set; } = new List<Test>();
        public int? TestTimeLimit { get; set; } // Время на тест в минутах, null если не задано
    }
}