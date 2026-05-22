using ProjectOperationsDashboard.Core.Config;
using ProjectOperationsDashboard.Core.Exceptions;

namespace ProjectOperationsDashboard.Core.Logging
{
    public class IncidentLogger //บันทึกlog 
    {
        private readonly string _logFilePath;
        private readonly OpsConfig _config;

        public IncidentLogger(string logFilePath, OpsConfig config) //รับค่าตอนสร้างob 
        {
            _logFilePath = logFilePath;
            _config = config;

            try //สร้าง f
            {
                string? folder = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(folder)) Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] Could not create log directory: {ex.Message}");
            }
        }

        //bonus rolatelog
        private void RotateIfNeeded(long maxBytes = 2048)  // <- 2KB 
        {
            try
            {
                FileInfo fileInfo = new FileInfo(_logFilePath);
                if (fileInfo.Exists && fileInfo.Length > maxBytes)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string rotatedPath = _logFilePath.Replace(".log", $"_{timestamp}.log");
                    File.Move(_logFilePath, rotatedPath);
                    Console.WriteLine($"[Log] File rotated to: {rotatedPath}");
                }
            }
            catch (Exception ex)
            {
                // ถ้า rotate ไม่ได้ ให้เขียนต่อในไฟล์เดิมได้ตามโจทย์
                Console.WriteLine($"[Warning] Could not rotate log: {ex.Message}");
            }
        }

        public async Task WriteAsync(IncidentLog entry)
        {
            try
            {
                RotateIfNeeded();// เรียกใช้ rotate
                using (StreamWriter sw = new StreamWriter(_logFilePath, append: true))
                {
                    await sw.WriteLineAsync($"{entry.Timestamp:yyyy-MM-dd HH:mm:sszzz}  [{entry.Severity}] | {entry.Message} | Node: {entry.NodeId}");
                }
            }
            catch (Exception ex)
            {
                var logEx = new LogWriteException("Failed to write incident log", _logFilePath, entry);
                Console.WriteLine($"[Log Error] {logEx.Message} | Reason: {ex.Message} | Entry lost: {entry.Message}");
            }
        }

        public async Task<List<string>> ReadAllAsync()
        {
            var results = new List<string>();
            if (!File.Exists(_logFilePath)) return results; //ส่ง List เปล่าๆ

            using (StreamReader sr = new StreamReader(_logFilePath))
            {
                string? line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    results.Add(line);
                }
            }
            return results;
        }

        public async Task<List<string>> ReadBySeverityAsync(string severity)
        {
            var results = new List<string>();
            if (!File.Exists(_logFilePath)) return results;

            using (StreamReader sr = new StreamReader(_logFilePath))
            {
                string? line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (line.Contains(severity, StringComparison.OrdinalIgnoreCase)) //ไม่สนใจตัวพิมพ์เล็กพิมพ์ใหญ่
                    {
                        results.Add(line);
                    }
                }
            }
            return results;
        }
    }
}
