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
        private readonly string filePath = "reports.txt";

        public IReadOnlyList<Report> Reports => reports.AsReadOnly();

        public ReportManager()
        {
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

            return reports.Where(r =>
                r.Title.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                r.Content.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        private void SaveReports()
        {
            try
            {
                var lines = reports.Select(r =>
                    $"{EscapeString(r.Title)}|{EscapeString(r.Content)}|{r.CreationDate:yyyy-MM-dd HH:mm:ss}");
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new IOException("Ошибка при сохранении отчетов", ex);
            }
        }

        private void LoadReports()
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                var lines = File.ReadAllLines(filePath);
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

        private string EscapeString(string input)
        {
            return input.Replace("|", "\\|").Replace("\n", "\\n");
        }

        private string UnescapeString(string input)
        {
            return input.Replace("\\|", "|").Replace("\\n", "\n");
        }

        private string[] ParseLine(string line)
        {
            var parts = new List<string>();
            var current = new System.Text.StringBuilder();
            bool escape = false;

            foreach (char c in line)
            {
                if (escape)
                {
                    current.Append(c);
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else if (c == '|')
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            parts.Add(current.ToString());

            return parts.ToArray();
        }
    }
}