using System.Collections.Generic;

namespace Obuchalkin.Models
{
    public class StudentIndexViewModel
    {
        public List<Course> Courses { get; set; }
        public List<Progress> Progresses { get; set; }
        public List<Result> Results { get; set; }
    }
}