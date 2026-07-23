using MyNote.Domain.Vaults;

namespace MyNote.Infrastructure.Vault;

public class FileVaultManager : IVaultManager
{
    public Task<VaultInfo> OpenAsync(string path, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        return Task.Run(() =>
        {
            token.ThrowIfCancellationRequested();

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Директория не найдена");
            }

            var d = new DirectoryInfo(path);

            return new VaultInfo(d.Name, d.FullName);
        }, token);
    }

    public Task<VaultInfo> CreateAsync(string path, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}