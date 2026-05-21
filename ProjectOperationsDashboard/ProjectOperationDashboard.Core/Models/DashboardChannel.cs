namespace ProjectOperationsDashboard.Core.Models
{
    public class DashboardAlertChannel : NotificationChannelBase
    {
        public override string ChannelName => "Dashboard";

        public override void Send(NotificationMessage<string> msg)
        {
            base.Send(msg);
            Console.WriteLine($"[Dashboard] Alert Appears: {msg.Title} {msg.Content}");
            //Console.WriteLine($"[Dashboard] Displaying: {msg.Title} (from {msg.Sender})");
        }
    }
}