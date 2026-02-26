using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Reports")]
public class Report
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int ReporterId { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int ReportedUserId { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('spam','inappropriate','harassment')", IsNullable = false)]
    public string ReportType { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('pending','processed','dismissed')", IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(ReporterId))]
    public User? Reporter { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(ReportedUserId))]
    public User? ReportedUser { get; set; }
}
