using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Bottles")]
public class Bottle
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(255)", IsNullable = true)]
    public string? VoiceUrl { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(255)", IsNullable = true)]
    public string? ImageUrl { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(20)", IsNullable = true)]
    public string? Emotion { get; set; }
    
    [SugarColumn(ColumnDataType = "json", IsNullable = true)]
    public string? Topics { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('floating','received','expired')", IsNullable = false, DefaultValue = "'floating'")]
    public string Status { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false)]
    public DateTime ExpiredAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User? User { get; set; }
}
