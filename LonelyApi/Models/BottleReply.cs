using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("BottleReplies")]
public class BottleReply
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int BottleId { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(BottleId))]
    public Bottle? Bottle { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User? User { get; set; }
}