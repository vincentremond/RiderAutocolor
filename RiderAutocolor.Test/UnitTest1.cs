using NUnit.Framework;

namespace RiderAutocolor.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(@"C:\Dir1\Solution1", @"C:\Dir1\Solution1\Project1", "Project1")]
        [TestCase(@"C:\Dir1\Solution1\", @"C:\Dir1\Solution1\Project1", "Project1")]
        [TestCase(@"C:\Dir1\Solution1", @"C:\Dir1\Solution1\Project1\", "Project1")]
        [TestCase(@"C:\Dir1\Solution1", @"C:\Dir1\Solution1\Project1\Project.2\", @"Project1\Project.2")]
        public void Test1(string sln, string csproj, string result)
        {
            var projectPath = Program.GetProjectPath(sln, csproj);

            Assert.AreEqual(result, projectPath);
        }
    }
}