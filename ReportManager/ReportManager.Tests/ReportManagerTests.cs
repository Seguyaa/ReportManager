namespace ReportManager.Tests
{
    [TestClass]
    public class ReportManagerTests
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
        public void Constructor_WhenFileDoesNotExist_CreatesEmptyList()
        {
            // Act
            var manager = new ReportManager(_testFilePath);

            // Assert
            Assert.IsNotNull(manager.Reports);
            Assert.AreEqual(0, manager.Reports.Count);
        }

        [TestMethod]
        public void AddReport_ValidReport_AddsToCollection()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var report = TestHelpers.CreateTestReport();

            // Act
            manager.AddReport(report);

            // Assert
            Assert.AreEqual(1, manager.Reports.Count);
            Assert.AreSame(report, manager.Reports[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddReport_NullReport_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);

            // Act
            manager.AddReport(null);
        }

        [TestMethod]
        public void RemoveReport_ExistingReport_RemovesFromCollection()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var report = TestHelpers.CreateTestReport();
            manager.AddReport(report);

            // Act
            manager.RemoveReport(report);

            // Assert
            Assert.AreEqual(0, manager.Reports.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveReport_NullReport_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);

            // Act
            manager.RemoveReport(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveReport_NonExistentReport_ThrowsInvalidOperationException()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var report1 = TestHelpers.CreateTestReport("Report 1");
            var report2 = TestHelpers.CreateTestReport("Report 2");
            manager.AddReport(report1);

            // Act
            manager.RemoveReport(report2);
        }

        [TestMethod]
        public void UpdateReport_ExistingReport_UpdatesProperties()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var report = TestHelpers.CreateTestReport("Old Title", "Old Content");
            manager.AddReport(report);

            // Act
            manager.UpdateReport(report, "New Title", "New Content");

            // Assert
            Assert.AreEqual("New Title", report.Title);
            Assert.AreEqual("New Content", report.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateReport_NullReport_ThrowsArgumentNullException()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);

            // Act
            manager.UpdateReport(null, "Title", "Content");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateReport_EmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var report = TestHelpers.CreateTestReport();
            manager.AddReport(report);

            // Act
            manager.UpdateReport(report, "", "Content");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateReport_EmptyContent_ThrowsArgumentException()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var report = TestHelpers.CreateTestReport();
            manager.AddReport(report);

            // Act
            manager.UpdateReport(report, "Title", "");
        }

        [TestMethod]
        public void SearchReports_WithEmptyTerm_ReturnsAllReports()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            var reports = TestHelpers.CreateTestReports(5);
            foreach (var report in reports)
            {
                manager.AddReport(report);
            }

            // Act
            var results = manager.SearchReports("");

            // Assert
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void SearchReports_WithValidTerm_ReturnsMatchingReports()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(TestHelpers.CreateTestReport("Annual Report", "Content about year"));
            manager.AddReport(TestHelpers.CreateTestReport("Monthly Report", "Content about month"));
            manager.AddReport(TestHelpers.CreateTestReport("Weekly Summary", "Different content"));

            // Act
            var results = manager.SearchReports("Report");

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void SearchReports_CaseInsensitive_ReturnsMatchingReports()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(TestHelpers.CreateTestReport("ANNUAL REPORT", "CONTENT"));
            manager.AddReport(TestHelpers.CreateTestReport("monthly report", "content"));

            // Act
            var results = manager.SearchReports("report");

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void SearchReports_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(TestHelpers.CreateTestReport("Report 1", "Content 1"));
            manager.AddReport(TestHelpers.CreateTestReport("Report 2", "Content 2"));

            // Act
            var results = manager.SearchReports("NonExistent");

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void SearchReports_SearchByContent_ReturnsMatchingReports()
        {
            // Arrange
            var manager = new ReportManager(_testFilePath);
            manager.AddReport(TestHelpers.CreateTestReport("Report 1", "Important information"));
            manager.AddReport(TestHelpers.CreateTestReport("Report 2", "Regular content"));
            manager.AddReport(TestHelpers.CreateTestReport("Report 3", "Important data"));

            // Act
            var results = manager.SearchReports("Important");

            // Assert
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void SaveAndLoadReports_PersistsDataCorrectly()
        {
            // Arrange
            var manager1 = new ReportManager(_testFilePath);
            var report1 = TestHelpers.CreateTestReport("Saved Report 1", "Content 1");
            var report2 = TestHelpers.CreateTestReport("Saved Report 2", "Content 2");
            manager1.AddReport(report1);
            manager1.AddReport(report2);

            // Act
            var manager2 = new ReportManager(_testFilePath);

            // Assert
            Assert.AreEqual(2, manager2.Reports.Count);
            Assert.AreEqual("Saved Report 1", manager2.Reports[0].Title);
            Assert.AreEqual("Content 1", manager2.Reports[0].Content);
            Assert.AreEqual("Saved Report 2", manager2.Reports[1].Title);
            Assert.AreEqual("Content 2", manager2.Reports[1].Content);
        }
    }
}