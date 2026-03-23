using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Posts")]
public class Post
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(ColumnDataType = "int", IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "text", IsNullable = false)]
    public string Content { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(500)", IsNullable = true)]
    public string Images { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(255)", IsNullable = true)]
    public string Audio { get; set; }
    
    [SugarColumn(ColumnDataType = "int", IsNullable = false, DefaultValue = "0")]
    public int LikeCount { get; set; }
    
    [SugarColumn(ColumnDataType = "int", IsNullable = false, DefaultValue = "0")]
    public int CommentCount { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User User { get; set; }
}