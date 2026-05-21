namespace ProjectOperationsDashboard.Core.Exceptions
{
    public class OpsConfigException : Exception //โหลด Config พัง 
    {
        public string? FilePath { get; }

        public OpsConfigException(string message) : base(message) { }

        public OpsConfigException(string message, Exception inner) : base(message, inner) { }

        // Constructor เฉพาะสำหรับเก็บ Path ไฟล์ที่คอนฟิกพัง
        public OpsConfigException(string message, string filePath, Exception inner) : base(message, inner)
        {
            FilePath = filePath;
        }
    }
}