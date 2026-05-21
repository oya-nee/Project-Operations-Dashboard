using ProjectOperationsDashboard.Core.Models;

namespace ProjectOperationsDashboard.Core.Interfaces
{
    public interface INotificationChannel
    {
        string ChannelName { get; } //ต้องมีชื่อ
        int SentCount { get; } // ต้องนับได้ว่าส่งไปกี่ครั้ง

        //List<string> Logs { get; }
        IReadOnlyCollection<string> Logs { get; }

        void Send(NotificationMessage<string> msg); //รับข้อความละส่ง

    }
}
