using ProjectOperationsDashboard.Core.Logging;

namespace ProjectOperationsDashboard.Core.Exceptions
{
    //มาจากคลาส Exception หลัก
    public class LogWriteException : Exception
    {
        public string? FilePath { get; } //ไฟล์ไหนไม่เข้า
        public IncidentLog? FailedEntry { get; } //ข้อความไหนหาย

        public LogWriteException(string message) : base(message) { }

        public LogWriteException(string message, Exception inner) : base(message, inner) { } // // Constructor ที่รับ Ex ต้นทามาด้วย

        // Constructor รับ Path และข้อมูล Log มี่ยังไม่เข้า
        public LogWriteException(string message, string filePath, IncidentLog? entry) : base(message)
        {
            FilePath = filePath;
            FailedEntry = entry;
        }
    }
}