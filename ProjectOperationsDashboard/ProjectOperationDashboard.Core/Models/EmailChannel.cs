using System.Net.Mail;

namespace ProjectOperationsDashboard.Core.Models
{
    public class EmailChannel : NotificationChannelBase, IDisposable
    {
        public override string ChannelName => "Email";

        private readonly Lazy<SmtpClient> _smtpClient = new Lazy<SmtpClient>(() =>
        {
            Console.WriteLine("[System] Initializing SMTP Connection.");
            return new SmtpClient("smtp.test.internal");
        });

        public override void Send(NotificationMessage<string> msg)
        {
            base.Send(msg);
            Console.WriteLine($"[Email] Sent via {_smtpClient.Value.Host}: {msg.Title} {msg.Content}");
        }

        public void Dispose()
        {
            if (_smtpClient.IsValueCreated)
                _smtpClient.Value.Dispose();
        }
    }
}
