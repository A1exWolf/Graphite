using MyNote.Domain.Config;

namespace MyNote.Domain.Vaults;

public interface IVaultManager
{
    Task<VaultInfo> OpenAsync(string path, CancellationToken token = default);
    Task<VaultInfo> CreateAsync(string path, CancellationToken token = default);
    Task SaveVault(IConfigStorage config, string path, CancellationToken token = default);
    Task<string?> GetVault(IConfigStorage config, CancellationToken token = default);
}