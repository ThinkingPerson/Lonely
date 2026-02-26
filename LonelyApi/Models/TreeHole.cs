using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("TreeHoles")]
public class TreeHole
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(255)", IsNullable = true)]
    public string? VoiceUrl { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('active','expired')", IsNullable = false, DefaultValue = "'active'")]
    public string Status { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false)]
    public DateTime ExpiredAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User? User { get; set; }
}
