using ReportManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportManager
{
    /// <summary>
    /// Класс для управления отчетами
    /// </summary>
    public class ReportManager
    {
        private List<Report> reports;
        private readonly string _filePath;
        private readonly object _fileLock = new object();

        public IReadOnlyList<Report> Reports => reports.AsReadOnly();

        // Конструктор по умолчанию
        public ReportManager() : this("reports.txt")
        {
        }

        // Конструктор с возможностью указать путь к файлу (для тестов)
        public ReportManager(string filePath)
        {
            _filePath = filePath;
            reports = new List<Report>();
            LoadReports();
        }

        public void AddReport(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            reports.Add(report);
            SaveReports();
        }

        public void RemoveReport(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (reports.Contains(report))
            {
                reports.Remove(report);
                SaveReports();
            }
            else
            {
                throw new InvalidOperationException("Отчет не найден в коллекции");
            }
        }

        public void UpdateReport(Report report, string newTitle, string newContent)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Название отчета не может быть пустым", nameof(newTitle));

            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("Содержание отчета не может быть пустым", nameof(newContent));

            report.Title = newTitle;
            report.Content = newContent;
            SaveReports();
        }

        public List<Report> SearchReports(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Report>(reports);

            // Для .NET Framework 4.7.2 используем ToLowerInvariant() для case-insensitive поиска
            string searchLower = searchTerm.ToLowerInvariant();

            return reports.Where(r =>
                r.Title.ToLowerInvariant().Contains(searchLower) ||
                r.Content.ToLowerInvariant().Contains(searchLower))
                .ToList();
        }

        private void SaveReports()
        {
            lock (_fileLock)
            {
                try
                {
                    var lines = reports.Select(r =>
                        $"{EscapeString(r.Title)}|{EscapeString(r.Content)}|{r.CreationDate:yyyy-MM-dd HH:mm:ss}");
                    File.WriteAllLines(_filePath, lines);
                }
                catch (Exception ex)
                {
                    throw new IOException("Ошибка при сохранении отчетов", ex);
                }
            }
        }

        private void LoadReports()
        {
            lock (_fileLock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                        return;

                    var lines = File.ReadAllLines(_filePath);
                    reports.Clear();

                    foreach (var line in lines)
                    {
                        var parts = ParseLine(line);
                        if (parts.Length == 3)
                        {
                            if (DateTime.TryParse(parts[2], out DateTime creationDate))
                            {
                                reports.Add(new Report(
                                    UnescapeString(parts[0]),
                                    UnescapeString(parts[1]),
                                    creationDate));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException("Ошибка при загрузке отчетов", ex);
                }
            }
        }

        private string EscapeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Экранируем специальные символы в правильном порядке
            // Сначала экранируем обратный слеш, затем разделитель и переводы строк
            return input
                .Replace("\\", "\\\\")    // Экранируем обратный слеш
                .Replace("|", "\\|")       // Экранируем разделитель
                .Replace("\n", "\\n")      // Экранируем перевод строки
                .Replace("\r", "\\r");     // Экранируем возврат каретки
        }

        private string UnescapeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Восстанавливаем символы в обратном порядке
            return input
                .Replace("\\n", "\n")      // Восстанавливаем перевод строки
                .Replace("\\r", "\r")      // Восстанавливаем возврат каретки
                .Replace("\\|", "|")       // Восстанавливаем разделитель
                .Replace("\\\\", "\\");    // Восстанавливаем обратный слеш
        }

        private string[] ParseLine(string line)
        {
            var parts = new List<string>();
            var current = new System.Text.StringBuilder();
            int i = 0;

            while (i < line.Length)
            {
                if (line[i] == '\\' && i + 1 < line.Length)
                {
                    // Пропускаем экранирующий символ и добавляем следующий символ как есть
                    current.Append(line[i + 1]);
                    i += 2;
                }
                else if (line[i] == '|')
                {
                    // Найден разделитель, добавляем текущую часть
                    parts.Add(current.ToString());
                    current.Clear();
                    i++;
                }
                else
                {
                    current.Append(line[i]);
                    i++;
                }
            }

            // Добавляем последнюю часть
            parts.Add(current.ToString());

            return parts.ToArray();
        }
    }
}