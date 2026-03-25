namespace ReportManager.Tests
{
    /// <summary>
    /// Вспомогательные методы для тестирования
    /// </summary>
    public static class TestHelpers
    {
        private static string _testDirectory;
        private static Random _random = new Random();

        static TestHelpers()
        {
            // Создаем уникальную директорию для каждого запуска тестов
            _testDirectory = Path.Combine(Path.GetTempPath(), $"ReportManagerTests_{DateTime.Now:yyyyMMdd_HHmmss}_{_random.Next(10000)}");
            Directory.CreateDirectory(_testDirectory);
        }

        /// <summary>
        /// Создает уникальный путь для тестового файла
        /// </summary>
        public static string GetUniqueTestFilePath()
        {
            return Path.Combine(_testDirectory, $"reports_{Guid.NewGuid()}.txt");
        }

        /// <summary>
        /// Создает тестовый отчет
        /// </summary>
        public static Report CreateTestReport(string title = "Test Report",
            string content = "Test Content",
            DateTime? date = null)
        {
            return new Report(
                title,
                content,
                date ?? DateTime.Now
            );
        }

        /// <summary>
        /// Создает список тестовых отчетов
        /// </summary>
        public static List<Report> CreateTestReports(int count = 3)
        {
            var reports = new List<Report>();
            for (int i = 1; i <= count; i++)
            {
                reports.Add(CreateTestReport(
                    $"Report {i}",
                    $"Content {i}",
                    DateTime.Now.AddDays(-i)
                ));
            }
            return reports;
        }

        /// <summary>
        /// Очищает тестовую директорию
        /// </summary>
        public static void CleanupTestDirectory()
        {
            try
            {
                if (Directory.Exists(_testDirectory))
                {
                    Directory.Delete(_testDirectory, true);
                }
            }
            catch
            {
                // Игнорируем ошибки при очистке
            }
        }

        /// <summary>
        /// Создает тестовый менеджер с уникальным файлом
        /// </summary>
        public static ReportManager CreateTestManager()
        {
            var tempFile = GetUniqueTestFilePath();
            return new ReportManager(tempFile);
        }
    }
}