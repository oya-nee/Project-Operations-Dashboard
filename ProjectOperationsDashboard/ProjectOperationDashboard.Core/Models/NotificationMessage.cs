namespace ProjectOperationsDashboard.Core.Models
{
    //public class NotificationMessage<T>
    //{
    //    public string Title { get; set; } = string.Empty;
    //    public T? Content { get; set; }
    //    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    //    public AlertPriority Priority { get; set; } = AlertPriority.Info;
    //    //public string Sender { get; set; } = "System";
    //    //public string Recipient { get; set; } = "Administrator";
    //}
    public class NotificationMessage<T>
    {
        public string Title { get; set; } = string.Empty;
        public T? Content { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        public AlertPriority Priority
        {
            get
            {
                if (Title.Contains("CRITICAL", StringComparison.OrdinalIgnoreCase))
                    return AlertPriority.Critical;

                if (Title.Contains("WARNING", StringComparison.OrdinalIgnoreCase))
                    return AlertPriority.Warning;

                return AlertPriority.Info;
            }
        }
    }
}