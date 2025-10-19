using Shared.Services;

namespace Shared.Tests;

public class SharedUnitTests
{
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
}
