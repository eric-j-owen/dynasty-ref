using DataPipeline.Helpers;
using DataPipeline.Loaders;
namespace Tests.Unit
{

    public class PlayerMatchingTests
    {
        [Theory]
        [InlineData("", "")]
        public void RemoveSuffix_PlayersSuffixIsRemoved(string input, string expected)
        {
            var actual = PlayerMatcher.RemoveSuffix(input);
            Assert.Equal(expected, actual);
        }
    }

    public class FileServiceTests : IDisposable
    {
        private readonly string _testDataDirectory = "./test-data";
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _fileService = new FileService(_testDataDirectory);
        }

        private class TestPlayer
        {
            public required string Name { get; set; }
        }

        [Fact]
        public void FileService_SavesJsonFileAndReadData()
        {
            var testData = new TestPlayer { Name = "test player" };
            var testFileName = "test";

            var testPath = Path.Combine(_testDataDirectory, $"{testFileName}.json");

            _fileService.WriteToFileJson(testFileName, testData);
            var returnedData = _fileService.ReadFromFileJson<TestPlayer>(testFileName);

            Assert.NotNull(returnedData);
            Assert.True(File.Exists(testPath));
            Assert.Equal("test player", returnedData.Name);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDataDirectory))
            {
                Directory.Delete(_testDataDirectory, true);
            }

            GC.SuppressFinalize(this);
        }
    }

    public class HelpersUnitTests
    {
        [Theory]
        [InlineData("Ja&#x27;Marr Chase", "jamarrchase")]
        [InlineData("A.J. Brown", "ajbrown")]
        [InlineData("De&#x27;Von Achane", "devonachane")]
        public void Name_IsNormalized(string input, string expected)
        {
            var actual = NormalizeField.Name(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("RB5", "RB")]
        [InlineData("TE13", "TE")]
        public void Position_IsNormalized(string input, string expected)
        {
            var actual = NormalizeField.Position(input);
            Assert.Equal(expected, actual);
        }
    }

    public class ScraperUnitTests
    {
        [Fact]
        public void ParsePlayer_ReturnsPlayerObject()
        {
            string name = "Brian Thomas Jr.";
            int value = 5000;
            string team = "JAC";
            string position = "WR13";

            var actual = KtcScraper.ParsePlayer(name, value, team, position);

            Assert.NotNull(actual);
            Assert.Equal("brianthomasjr", actual.SearchFullName);
            Assert.Equal(5000, actual.Value);
            Assert.Equal("JAX", actual.Team);
            Assert.Equal("WR", actual.Position);
        }
    }
}