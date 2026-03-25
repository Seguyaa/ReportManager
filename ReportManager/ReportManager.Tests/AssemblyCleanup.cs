namespace ReportManager.Tests
{
    [TestClass]
    public static class AssemblyCleanup
    {
        [AssemblyCleanup]
        public static void Cleanup()
        {
            // Очищаем тестовую директорию после всех тестов
            TestHelpers.CleanupTestDirectory();
        }
    }
}