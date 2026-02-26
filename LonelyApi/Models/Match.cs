using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Matches")]
public class Match
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int UserId1 { get; set; }
    
    [SugarColumn(IsNullable = false)]
    public int UserId2 { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('pending','matched','expired')", IsNullable = false, DefaultValue = "'pending'")]
    public string Status { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false)]
    public DateTime ExpiredAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId1))]
    public User? User1 { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId2))]
    public User? User2 { get; set; }
}
