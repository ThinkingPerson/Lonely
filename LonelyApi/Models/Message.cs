using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Messages")]
public class Message
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int FromUserId { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int ToUserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('text','voice','image')", IsNullable = false, DefaultValue = "'text'")]
    public string Type { get; set; }
    
    [SugarColumn(IsNullable = false, DefaultValue = "false")]
    public bool IsRead { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(FromUserId))]
    public User? FromUser { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(ToUserId))]
    public User? ToUser { get; set; }
}
