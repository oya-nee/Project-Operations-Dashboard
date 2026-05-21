using ProjectOperationsDashboard.Core.Interfaces;
using ProjectOperationsDashboard.Core.Models;

namespace ProjectOperationsDashboard.Core.Services
{
    public class NotificationService
    {
        private readonly Dictionary<string, INotificationChannel> _channels = new();
        protected readonly PriorityQueue<NotificationMessage<string>, int> _priorityQueue = new();
        protected readonly HashSet<string> _processedAlerts = new();
        protected readonly object _queueLock = new(); //ล็อก ThreadSafe
        private string _lastActiveMinute = string.Empty;//นาทีล่าสุดคือนาทีไหน

        public void RegisterChannel(INotificationChannel channel)
        {
            lock (_queueLock) { _channels[channel.ChannelName] = channel; }
        }

        public void EnqueueMessage(NotificationMessage<string> msg)
        {
            if (msg == null) return;

            string currentMinute = msg.TimestampUtc.ToString("yyyyMMddHHmm");
            string alertKey = $"{msg.Title}_{currentMinute}";

            lock (_queueLock)
            {
                if (_lastActiveMinute != currentMinute)
                {
                    _processedAlerts.Clear();
                    _lastActiveMinute = currentMinute;
                }

                if (_processedAlerts.Add(alertKey)) //ไม่มีก็เพิ่ม มีก็ไม่เพิ่มละส่งกลับ false ละแจ้งเตือน
                {
                    _priorityQueue.Enqueue(msg, (int)msg.Priority);
                    Console.WriteLine($"Enqueued: {msg.Title} with Priority: {msg.Priority}");
                }
                else
                {
                    Console.WriteLine($"Duplicate Alert blocked: {msg.Title}");
                }
            }
        }

        public virtual void Broadcast(NotificationMessage<string> msg) //ส่งข้อความไปยังทุกช่อง
        {
            lock (_queueLock)
            {
                foreach (var channel in _channels.Values)
                {
                    channel.Send(msg);
                }
            }
        }

        //แค่คลาสนี้และ Inherit เท่านั้นที่สามารถเข้าถึงได้
        //ส่งมอบกล่องเก็บข้อมูลที่บรรจุช่องทางการแจ้งเตือนต่างๆ //Channels ออกไปให้คลาสลูก
        protected Dictionary<string, INotificationChannel> GetChannels() => _channels;
    }
}