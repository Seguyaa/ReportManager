namespace ReportManager.Tests
{
    [TestClass]
    public static class AssemblyCleanup
    {
        [AssemblyCleanup]
        public static void Cleanup()
        {
            TestHelpers.CleanupTestDirectory();
        }
    }
}