using System;

namespace ReportManager
{
    /// <summary>
    /// Класс, представляющий отчет
    /// </summary>
    public class Report
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }

        public Report(string title, string content, DateTime creationDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название отчета не может быть пустым", nameof(title));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Содержание отчета не может быть пустым", nameof(content));

            Title = title;
            Content = content;
            CreationDate = creationDate;
        }

        public override string ToString()
        {
            return $"{Title} - {CreationDate:yyyy-MM-dd}";
        }

        public string GetFullInfo()
        {
            return $"Отчет: {Title}\n" +
                   $"Дата создания: {CreationDate:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Содержание: {Content}";
        }
    }
}