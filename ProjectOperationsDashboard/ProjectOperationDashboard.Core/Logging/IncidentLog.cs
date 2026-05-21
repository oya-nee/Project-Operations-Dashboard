namespace ProjectOperationsDashboard.Core.Logging
{
    public record IncidentLog(
        DateTimeOffset Timestamp,
        string Severity,
        string Message,
        string? NodeId = "Unknow"
    );
}