using ProjectOperationsDashboard.Core.Config;
using ProjectOperationsDashboard.Core.Exceptions;
using ProjectOperationsDashboard.Core.Logging;
using ProjectOperationsDashboard.Core.Models;
using ProjectOperationsDashboard.Core.Services;

var loader = new OpsConfigLoader();

//Normal Config
var normalConfig = await loader.LoadAsync("data/appsettings.json");
Console.WriteLine($"Config: Endpoint={normalConfig.ServerEndpoint}, Port={normalConfig.Port}, Heartbeat={normalConfig.HeartbeatInterval}\n");

//Missing Config
var missingConfig = await loader.LoadAsync("data/missing.json");
Console.WriteLine($"Missing Config: Endpoint={missingConfig.ServerEndpoint}, Port={missingConfig.Port}, Heartbeat={missingConfig.HeartbeatInterval}\n");

//Broken Config
try
{
    await loader.LoadAsync("data/appsettings-broken.json");
}
catch (OpsConfigException ex)
{
    Console.WriteLine($"Caught OpsConfigException: {ex.Message}");
    Console.WriteLine($"File Path: {ex.FilePath}");
    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}\n");
}

//Invalid Path
try
{
    await loader.LoadAsync("D:\\Test\\TestPathEx.json");
}
catch (Exception)
{

}


//Incident Log
var logger = new IncidentLogger("data/incidents.log", normalConfig);

await logger.WriteAsync(new IncidentLog(DateTimeOffset.UtcNow, "CRITICAL", "Database dead", "Node-01"));
await logger.WriteAsync(new IncidentLog(DateTimeOffset.UtcNow, "WARNING", "CPU Temp High", "Node-02"));
await logger.WriteAsync(new IncidentLog(DateTimeOffset.UtcNow, "INFO", "System Healthy", "Node-03"));

//ReadBySeverity critical
var criticalLogs = await logger.ReadBySeverityAsync("CRITICAL");
foreach (var log in criticalLogs)
{
    Console.WriteLine(log);
}
Console.WriteLine();

////ReadBySeverity warning
//var warningLogs = await logger.ReadBySeverityAsync("WARNING");
//foreach (var log in warningLogs)
//{
//    Console.WriteLine(log);
//}
//Console.WriteLine();

//Notification
var priorityService = new PriorityNotificationService();
priorityService.RegisterChannel(new EmailChannel());
priorityService.RegisterChannel(new DashboardAlertChannel());
priorityService.RegisterChannel(new SmsChannel());

priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[INFO] Server check OK", Content = "Server check OK" });
priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[WARNING] Memory High", Content = "Memory High" });
priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[CRITICAL] Srver Down", Content = "Srver Down" });

//เอาไว้เช็คแจ้งเตือนไม่เหมือนกันก็จะส่งอยู่ดี
//priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[INFO] Server check OK", Content = "Server check OK" }); 
//priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[CRITICAL] CPU Overheat", Content = "CPU Overheat" });

//critical > all channel
//warning > email + sms
//info > dashboard only
//เอาไว้เช็คส่งซ้ำแล้วบ้อก
//priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[CRITICAL] CPU Overheat", Content = "CPU Overheat" }); 
//priorityService.EnqueueMessage(new NotificationMessage<string> { Title = "[CRITICAL] CPU Overheat", Content = "CPU Overheat" });

priorityService.ProcessQueue();

//TT
Console.WriteLine($"\nGlobal Total Sent (Static): {NotificationChannelBase.TotalSentAllChannels}");
Console.WriteLine("\nChannel Summary");
foreach (var stat in priorityService.GetChannelSummary())
{
    Console.WriteLine($"Channel: {stat.Key}  Total Sent: {stat.Value}");
}