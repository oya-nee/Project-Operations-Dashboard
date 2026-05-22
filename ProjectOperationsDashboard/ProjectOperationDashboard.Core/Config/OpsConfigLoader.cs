using ProjectOperationsDashboard.Core.Exceptions;
using System.Text.Json;

namespace ProjectOperationsDashboard.Core.Config
{
    public class OpsConfigLoader
    {
        private OpsConfig GetDefaultConfig() => new OpsConfig("localhost", 8080, 30);

        public async Task<OpsConfig> LoadAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[Config] File not found: {filePath}. Generating default");
                    await SaveDefaultAsync(filePath);
                    return GetDefaultConfig();
                }

                string json = await File.ReadAllTextAsync(filePath);
                var config = JsonSerializer.Deserialize<OpsConfig>(json);

                return new OpsConfig(
                    config?.ServerEndpoint ?? GetDefaultConfig().ServerEndpoint,
                    config?.Port ?? GetDefaultConfig().Port,
                    config?.HeartbeatInterval ?? GetDefaultConfig().HeartbeatInterval
                );
            }
            catch (OpsConfigException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                throw new OpsConfigException("Invalid JSON format in config file", filePath, ex);
            }
            catch (Exception ex)
            {
                throw new OpsConfigException("Critical error loading config", filePath, ex);
            }
        }

        private async Task SaveDefaultAsync(string filePath)
        {
            try
            {
                var defaultConfig = GetDefaultConfig();
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(defaultConfig, options);

                string? folder = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);

                //await File.WriteAllTextAsync(filePath, json);
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    await sw.WriteAsync(json);
                }
                Console.WriteLine($"[Config] Default config saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] Could not save default config to disk: {ex.Message}\n");
            }
        }
    }
}