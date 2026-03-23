using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Comments")]
public class Comment
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(ColumnDataType = "int", IsNullable = false)]
    public int PostId { get; set; }
    
    [SugarColumn(ColumnDataType = "int", IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(PostId))]
    public Post Post { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User User { get; set; }
}