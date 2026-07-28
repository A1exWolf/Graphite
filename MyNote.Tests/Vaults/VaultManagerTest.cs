using MyNote.Domain.Vaults;
using MyNote.Infrastructure.Vault;

namespace MyNote.Tests.Vaults;

public sealed class VaultManagerTest
{
    [Fact]
    public async Task OpenAsync_WhenDirectoryIsExists()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), $"Graphite_Test_{Guid.NewGuid():N}");
        
        try
        {
            Directory.CreateDirectory(path);

            var v = new FileVaultManager();

            var result = await v.OpenAsync(path);

            Assert.Equal(new DirectoryInfo(path).Name, result.Name);
            Assert.Equal(new DirectoryInfo(path).FullName, result.Path);
        }
        finally
        {
            Directory.Delete(path);
        }
    }
    
    [Fact]
    public async Task OpenAsync_WhenDirectoryIsNotExists()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), $"Graphite_Test_{Guid.NewGuid():N}");

        var v = new FileVaultManager();

        await Assert.ThrowsAsync<DirectoryNotFoundException>(async () =>
        {
            await v.OpenAsync(path, CancellationToken.None);
        });
    }
}