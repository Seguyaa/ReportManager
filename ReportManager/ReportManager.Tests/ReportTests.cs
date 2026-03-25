namespace ReportManager.Tests
{
    [TestClass]
    public class ReportTests
    {
        [TestMethod]
        public void Constructor_WithValidData_CreatesReport()
        {
            // Arrange
            string title = "Test Report";
            string content = "Test Content";
            DateTime date = DateTime.Now;

            // Act
            var report = new Report(title, content, date);

            // Assert
            Assert.AreEqual(title, report.Title);
            Assert.AreEqual(content, report.Content);
            Assert.AreEqual(date, report.CreationDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithEmptyTitle_ThrowsArgumentException()
        {
            // Act
            new Report("", "Content", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithWhitespaceTitle_ThrowsArgumentException()
        {
            // Act
            new Report("   ", "Content", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithNullTitle_ThrowsArgumentException()
        {
            // Act
            new Report(null, "Content", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithEmptyContent_ThrowsArgumentException()
        {
            // Act
            new Report("Title", "", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithWhitespaceContent_ThrowsArgumentException()
        {
            // Act
            new Report("Title", "   ", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithNullContent_ThrowsArgumentException()
        {
            // Act
            new Report("Title", null, DateTime.Now);
        }

        [TestMethod]
        public void ToString_ReturnsCorrectFormat()
        {
            // Arrange
            var report = new Report("Test Title", "Test Content", new DateTime(2024, 1, 15));

            // Act
            string result = report.ToString();

            // Assert
            StringAssert.Contains(result, "Test Title");
            StringAssert.Contains(result, "2024-01-15");
        }

        [TestMethod]
        public void GetFullInfo_ReturnsCompleteInformation()
        {
            // Arrange
            var report = new Report("Test Title", "Test Content", new DateTime(2024, 1, 15, 10, 30, 0));

            // Act
            string result = report.GetFullInfo();

            // Assert
            StringAssert.Contains(result, "Test Title");
            StringAssert.Contains(result, "Test Content");
            StringAssert.Contains(result, "2024-01-15");
            StringAssert.Contains(result, "10:30:00");
        }
    }
}