using System;
using System.Collections.Generic;
using System.Text;

namespace MyNote.Domain.Config
{
    public interface IConfigStorage
    {
        Task<Config> GetConfigAsync(CancellationToken token = default);
        Task<Config> SaveConfigAsync(Config newConfig, CancellationToken token = default);
        Task<Config> ReplaceFieldAsync(ConfigField field, string value, CancellationToken token = default);
        Task<string?> GetFieldAsync(ConfigField field, CancellationToken token = default);
    }
}
