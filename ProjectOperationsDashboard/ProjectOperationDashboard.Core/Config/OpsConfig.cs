namespace ProjectOperationsDashboard.Core.Config
{
    // record- Immutability (ค่าไม่เปลี่ยนแปลงหลังจากสร้าง)
    // Nullable Reference Types 
    public record OpsConfig(
        string? ServerEndpoint,
        int? Port,
        int? HeartbeatInterval
    );
}