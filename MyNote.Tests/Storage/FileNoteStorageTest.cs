using MyNote.Infrastructure.Storage;

namespace MyNote.Tests.Storage
{
    public sealed class FileNoteStorageTest
    {
        [Fact]
        public async Task ReadAsync_WhenMarkdownFileExists_ReturnsFilledNote()
        {
            var temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                $"GraphiteTests-{Guid.NewGuid():N}");

            Directory.CreateDirectory(temporaryDirectory);

            var notePath = Path.Combine(
                temporaryDirectory,
                "Первая заметка.md");

            const string expectedContent =
                "# Первая заметка\n\nТестовое содержимое на русском языке.";

            try
            {
                await File.WriteAllTextAsync(
                    notePath,
                    expectedContent);

                var storage = new FileNoteStorage();

                var result = await storage.ReadAsync(
                    notePath,
                    CancellationToken.None);

                Assert.NotNull(result);
                Assert.Equal(notePath, result.Path);
                Assert.Equal("Первая заметка", result.Title);
                Assert.Equal(expectedContent, result.Content);
                Assert.NotEqual(default, result.ModifiedAt);
            }
            finally
            {
                if (Directory.Exists(temporaryDirectory))
                    Directory.Delete(temporaryDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task ReadAsync_WhenFileDoesNotExist_ReturnsNull()
        {
            var missingFilePath = Path.Combine(
                Path.GetTempPath(),
                $"GraphiteMissing-{Guid.NewGuid():N}.md");

            var storage = new FileNoteStorage();

            var result = await storage.ReadAsync(
                missingFilePath,
                CancellationToken.None);

            Assert.Null(result);
        }
    }
}