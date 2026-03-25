namespace ReportManager.Tests
{
    [TestClass]
    public class BoundaryTests
    {
        private string _testFilePath;

        [TestInitialize]
        public void SetUp()
        {
            _testFilePath = TestHelpers.GetUniqueTestFilePath();
        }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if (File.Exists(_testFilePath))
                {
                    File.Delete(_testFilePath);
                }
            }
            catch
            {
                // Игнорируем ошибки при очистке
            }
        }

        [TestMethod]
        public void ReportTitle_MinimumLength_OneCharacter()
        {
            // Act
            var report = new Report("A", "Content", DateTime.Now);

            // Assert
            Assert.AreEqual("A", report.Title);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReportTitle_MinimumLength_ZeroCharacters_ThrowsException()
        {
            // Act
            new Report("", "Content", DateTime.Now);
        }

        [TestMethod]
        public void ReportContent_MinimumLength_OneCharacter()
        {
            // Act
            var report = new Report("Title", "C", DateTime.Now);

            // Assert
            Assert.AreEqual("C", report.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReportContent_MinimumLength_ZeroCharacters_ThrowsException()
        {
            // Act
            new Report("Title", "", DateTime.Now);
        }

        [TestMethod]
        public void ReportTitle_MaximumLength_1000Characters()
        {
            // Arrange
            string longTitle = new string('A', 1000);

            // Act
            var report = new Report(longTitle, "Content", DateTime.Now);

            // Assert
            Assert.AreEqual(1000, report.Title.Length);
        }

        [TestMethod]
        public void ReportContent_MaximumLength_10000Characters()
        {
            // Arrange
            string longContent = new string('A', 10000);

            // Act
            var report = new Report("Title", longContent, DateTime.Now);

            // Assert
            Assert.AreEqual(10000, report.Content.Length);
        }

        [TestMethod]
        public void SearchReports_SearchTerm_SpacesOnly_ReturnsAllReports()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(TestHelpers.CreateTestReport("Report 1", "Content 1"));
            manager.AddReport(TestHelpers.CreateTestReport("Report 2", "Content 2"));

            // Act
            var results = manager.SearchReports("   ");

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void ReportTitle_WithNewlines_IsPreserved()
        {
            // Arrange
            string titleWithNewline = "Title\nWith\nNewlines";

            // Act
            var report = new Report(titleWithNewline, "Content", DateTime.Now);
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(report);
            var loadedReport = manager.Reports[0];

            // Assert
            Assert.AreEqual(titleWithNewline, loadedReport.Title);
        }

        [TestMethod]
        public void ReportContent_WithSpecialCharacters_IsPreserved()
        {
            // Arrange
            string contentWithSpecials = "Content with | and \\ and \n newlines";

            // Act
            var report = new Report("Title", contentWithSpecials, DateTime.Now);
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(report);
            var loadedReport = manager.Reports[0];

            // Assert
            Assert.AreEqual(contentWithSpecials, loadedReport.Content);
        }

        [TestMethod]
        public void ReportContent_WithWindowsLineEndings_IsPreserved()
        {
            // Arrange
            string contentWithWindowsLines = "First line\r\nSecond line\r\nThird line";

            // Act
            var report = new Report("Title", contentWithWindowsLines, DateTime.Now);
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(report);
            var loadedReport = manager.Reports[0];

            // Assert
            Assert.AreEqual(contentWithWindowsLines, loadedReport.Content);
        }

        [TestMethod]
        public void ReportTitle_WithPipeCharacter_IsPreserved()
        {
            // Arrange
            string titleWithPipe = "Title|With|Pipe";

            // Act
            var report = new Report(titleWithPipe, "Content", DateTime.Now);
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(report);
            var loadedReport = manager.Reports[0];

            // Assert
            Assert.AreEqual(titleWithPipe, loadedReport.Title);
        }

        [TestMethod]
        public void ReportContent_WithBackslash_IsPreserved()
        {
            // Arrange
            string contentWithBackslash = "Path: C:\\Users\\Documents\\report.txt";

            // Act
            var report = new Report("Title", contentWithBackslash, DateTime.Now);
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(report);
            var loadedReport = manager.Reports[0];

            // Assert
            Assert.AreEqual(contentWithBackslash, loadedReport.Content);
        }

        [TestMethod]
        public void MultipleReports_WithSpecialCharacters_ArePreserved()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);

            var report1 = new Report("Report|1", "Content\nwith|newline", DateTime.Now);
            var report2 = new Report("Report\\2", "Content\\with\\backslash", DateTime.Now);
            var report3 = new Report("Report\n3", "Content\r\nwith\r\nwindows", DateTime.Now);

            // Act
            manager.AddReport(report1);
            manager.AddReport(report2);
            manager.AddReport(report3);

            var loadedReports = manager.Reports;

            // Assert
            Assert.AreEqual(3, loadedReports.Count);
            Assert.AreEqual(report1.Title, loadedReports[0].Title);
            Assert.AreEqual(report1.Content, loadedReports[0].Content);
            Assert.AreEqual(report2.Title, loadedReports[1].Title);
            Assert.AreEqual(report2.Content, loadedReports[1].Content);
            Assert.AreEqual(report3.Title, loadedReports[2].Title);
            Assert.AreEqual(report3.Content, loadedReports[2].Content);
        }

        [TestMethod]
        public void SearchReports_SearchWithSpecialCharacters_ReturnsMatches()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(TestHelpers.CreateTestReport("Test@#$%", "Content with @#$%"));
            manager.AddReport(TestHelpers.CreateTestReport("Normal Report", "Normal Content"));

            // Act
            var results = manager.SearchReports("@#$%");

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Test@#$%", results[0].Title);
        }
    }
}