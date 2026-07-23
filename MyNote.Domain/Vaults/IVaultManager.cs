namespace MyNote.Domain.Vaults;

public interface IVaultManager
{
    Task<VaultInfo> OpenAsync(string path, CancellationToken token = default);
    Task<VaultInfo> CreateAsync(string path, CancellationToken token = default);
}