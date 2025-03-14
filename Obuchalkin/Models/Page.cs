// Models/Page.cs
using System.Collections.Generic;

namespace Obuchalkin.Models
{
    public class Page
    {
        public int ID { get; set; }
        public string Content { get; set; } // Текст страницы, включая HTML-теги <b> и <i>
        public string ImagePath { get; set; } // Путь к картинке, если она загружена
    }
}