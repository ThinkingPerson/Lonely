using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("TreeHoleReplies")]
public class TreeHoleReply
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int TreeHoleId { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(TreeHoleId))]
    public TreeHole? TreeHole { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User? User { get; set; }
}