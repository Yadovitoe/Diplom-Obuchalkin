// Models/Test.cs
using System.Collections.Generic;

namespace Obuchalkin.Models
{
    public class Test
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; } = new List<string>(); // Для вопросов с выбором
        public string CorrectAnswer { get; set; } // Правильный ответ (индекс для выбора или текст)
        public string Type { get; set; } // "MultipleChoice" или "TextInput"
    }
}