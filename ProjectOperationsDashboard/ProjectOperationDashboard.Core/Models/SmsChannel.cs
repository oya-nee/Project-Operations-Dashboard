namespace ProjectOperationsDashboard.Core.Models
{
    public class SmsChannel : NotificationChannelBase
    {
        public override string ChannelName => "SMS";
        public override void Send(NotificationMessage<string> msg)
        {
            base.Send(msg);
            Console.WriteLine($"[SMS] Sending to On-call Leader:{msg.Title} {msg.Content}");
            //Console.WriteLine($"[SMS] To {msg.Recipient}: {msg.Content}");
        }
    }

}