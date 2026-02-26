namespace LonelyApi.DTOs;

public class ReportRequest
{
    public int ReportedUserId { get; set; }
    public string ReportType { get; set; }
    public string Content { get; set; }
}
