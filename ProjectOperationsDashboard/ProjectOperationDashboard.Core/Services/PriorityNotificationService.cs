using ProjectOperationsDashboard.Core.Interfaces;
using ProjectOperationsDashboard.Core.Models;

namespace ProjectOperationsDashboard.Core.Services
{
    public class PriorityNotificationService : NotificationService
    {
        public Action<string>? OnDashboardUpdate;
        private readonly Dictionary<string, ChannelRateLimiter> _rateLimiters = new(); //ลิมิตแต่ละช่อง

        public PriorityNotificationService()
        {
            _rateLimiters["Email"] = new ChannelRateLimiter(5);
            _rateLimiters["SMS"] = new ChannelRateLimiter(5);
            _rateLimiters["Dashboard"] = new ChannelRateLimiter(100);
        }

        public override void Broadcast(NotificationMessage<string> msg)
        {
            Dictionary<string, INotificationChannel> channels;
            lock (_queueLock) //ดึงช่องทางทั้งหมดมาใส่ตัวแปรไว้ก่อน
            {
                channels = new Dictionary<string, INotificationChannel>(GetChannels());
            }

            foreach (var channel in channels.Values) //ลูปส่งแต่ละช่อง
            {
                if (ShouldSend(channel.ChannelName, msg.Priority))
                {
                    if (msg.Priority != AlertPriority.Critical)
                    {
                        if (_rateLimiters.TryGetValue(channel.ChannelName, out var limiter) && !limiter.AllowRequest())
                        {
                            Console.WriteLine($"[RateLimit] {channel.ChannelName} throttled for {msg.Priority} alert: {msg.Title}"); //Rate Limit บล็อก
                            continue; //ไปช่องอื่นแทน
                        }
                    }

                    channel.Send(msg);
                }
            }
        }

        private bool ShouldSend(string channelName, AlertPriority priority)
        {
            return priority switch
            {
                AlertPriority.Critical => true,
                AlertPriority.Warning => channelName != "Dashboard",
                AlertPriority.Info => channelName == "Dashboard",
                _ => false
            };
        }

        public void ProcessQueue()
        {
            while (true)
            {
                NotificationMessage<string> msg;
                lock (_queueLock)
                {
                    if (_priorityQueue.Count == 0) break;
                    msg = _priorityQueue.Dequeue();
                }

                Broadcast(msg);
                OnDashboardUpdate?.Invoke($"Processed {msg.Title} [{msg.Priority}]");
            }
        }

        public int GetTotalSentAllChannels()
        {
            lock (_queueLock) { return GetChannels().Values.Sum(c => c.SentCount); }
        }

        public Dictionary<string, int> GetChannelSummary() // ดึงสรุปยอดของแต่ละช่อง 
        {
            lock (_queueLock)
            {
                return GetChannels().Values.ToDictionary(c => c.ChannelName, c => c.SentCount);
            }
        }


        public List<string> GetAllFlatLogs() // ดึง log ทั้งหมดจากทุกช่องมารวมเป็น List 
        {
            lock (_queueLock) { return GetChannels().Values.SelectMany(c => c.Logs).ToList(); }
        }
    }
}