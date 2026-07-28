using MyNote.Domain.Config;
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

    public async Task<VaultInfo> CreateAsync(string path, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        token.ThrowIfCancellationRequested();

        Directory.CreateDirectory(path);

        return await OpenAsync(path, token);
    }

    public async Task SaveVault(IConfigStorage config, string path, CancellationToken token = default)
    {
        try
        {
            await config.ReplaceFieldAsync(ConfigField.VaultPath, path, token);
        }
        catch (Exception e)
        {
            throw new Exception($"Во время сохранения хранилища прозошла ошибка!");
        }
    }

    public async Task<string?> GetVault(IConfigStorage config, CancellationToken token = default)
    {
        try
        {
            return await config.GetFieldAsync(ConfigField.VaultPath, token);
        }
        catch (Exception e)
        {
            throw new Exception($"Во время сохранения хранилища прозошла ошибка!");
        }
    }
}