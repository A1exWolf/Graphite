using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyNote.Domain.Config;
using MyNote.Domain.Vaults;

namespace MyNote.Infrastructure.Config
{
    public class ConfigManager : IConfigStorage
    {
        private readonly string _configName;
        private readonly string _pathConfig;
        

        public ConfigManager(string configName = "config.json")
        {
            if (!string.IsNullOrEmpty(configName) && Path.GetExtension(configName) == ".json")
            {
                _configName = configName;
                _pathConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Graphite" , _configName);
            }
            else
            {
                throw new ArgumentNullException(nameof(configName));
            }
        }

        public async Task<Domain.Config.Config> GetConfigAsync(CancellationToken token = default)
        {
            try
            {
                if (!File.Exists(_pathConfig))
                {
                    await SaveConfigAsync(new Domain.Config.Config(), token);
                }

                var getFile = await File.ReadAllTextAsync(_pathConfig, token);

                if (string.IsNullOrEmpty(getFile))
                {
                    await SaveConfigAsync(new Domain.Config.Config(), token);

                    throw new ArgumentNullException($"Файл {nameof(_configName)} пуст");
                }

                var config = JsonSerializer.Deserialize<Domain.Config.Config>(getFile) ??
                             await SaveConfigAsync(new Domain.Config.Config(), token);

                return config;
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException("Ошибка во время сериализации");
            }
            catch (JsonException)
            {
                await SaveConfigAsync(new Domain.Config.Config(), token);

                throw new JsonException($"Ошибка во время попытки чтения конфига по пути {_pathConfig}. Откройте хранилище заново");
            }
            catch
            {
                throw new InvalidOperationException($"Ошибка во время записи файл {nameof(_configName)}");
            }
        }

        public async Task<Domain.Config.Config> SaveConfigAsync(Domain.Config.Config newConfig, CancellationToken token = default)
        {
            try
            {
                if (File.Exists(_pathConfig))
                {
                    File.Delete(_pathConfig);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(_pathConfig)!);

                var configJsonSerialaze = JsonSerializer.Serialize(newConfig);

                await using StreamWriter stream = new StreamWriter(_pathConfig, true, Encoding.UTF8);

                await stream.WriteAsync(configJsonSerialaze);

                return newConfig;
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException("Ошибка во время сериализации");
            }
            catch
            {
                throw new InvalidOperationException($"Ошибка во время записи файл {nameof(_configName)}");
            }
        }

        public async Task<Domain.Config.Config> ReplaceFieldAsync(ConfigField field, string value, CancellationToken token = default)
        {
            try
            {
                var config = await GetConfigAsync(token);

                switch (field)
                {
                    case ConfigField.VaultPath:
                        config.LastOpenVault = value;
                        break;

                    default:
                        throw new ArgumentException(
                            "Для данного аргумента еще не задана команда/или такое поле отсутсвует");
                }

                await SaveConfigAsync(config, token);

                return config;
            }
            catch (Exception e)
            {
                throw new Exception($"Во время выполнения {nameof(ReplaceFieldAsync)} произошла ошибка {e.Message}");
            }
        }

        public async Task<string?> GetFieldAsync(ConfigField field, CancellationToken token = default)
        {
            try
            {
                string value;
                var config = await GetConfigAsync(token);

                switch (field)
                {
                    case ConfigField.VaultPath:
                        value = config.LastOpenVault;
                        break;

                    default:
                        throw new ArgumentException(
                            "Для данного аргумента еще не задана команда/или такое поле отсутсвует");
                }

                return value;
            }
            catch (Exception e)
            {
                throw new Exception($"Во время выполнения {nameof(ReplaceFieldAsync)} произошла ошибка {e.Message}");
            }
        }
    }
}
