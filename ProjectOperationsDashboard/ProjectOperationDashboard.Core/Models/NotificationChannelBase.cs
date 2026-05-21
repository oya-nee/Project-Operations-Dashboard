using ProjectOperationsDashboard.Core.Interfaces;

namespace ProjectOperationsDashboard.Core.Models
{
    public abstract class NotificationChannelBase : INotificationChannel
    {
        //นับรวมทุก Channel
        private static int _totalSentAllChannels;
        public static int TotalSentAllChannels => _totalSentAllChannels;

        public abstract string ChannelName { get; } //อันที่สือบทอดไปตั้งชื่อเอง
        public int SentCount { get; protected set; }

        private readonly int _maxLogEntries = 100;
        private readonly List<string> _logs = new();
        private readonly object _logLock = new(); //กันแย่งกันเวลาเขียน log

        public IReadOnlyCollection<string> Logs => _logs.AsReadOnly(); //list > IReadOnlyCollection

        public virtual void Send(NotificationMessage<string> msg) //สิ่งที่ให้ลูกคลาส override ได้
        {
            SentCount++;
            //TotalSentAllChannels++;
            Interlocked.Increment(ref _totalSentAllChannels);

            //Logs.Add($"[{DateTime.Now:HH:mm:ss}] {ChannelName} sent: {msg.Title}");

            string logEntry = $"[{DateTime.UtcNow:HH:mm:ss}] {ChannelName}  {msg.Priority} | {msg.Title}"; //ให้ Thread ทำงานทีละตัว
            ////Logs.Add(logEntry);

            ////if (Logs.Count > _maxLogEntries) Logs.RemoveAt(0);
            lock (_logLock)
            {
                _logs.Add(logEntry);
                if (_logs.Count > _maxLogEntries) _logs.RemoveAt(0);
            }
        }
    }
}