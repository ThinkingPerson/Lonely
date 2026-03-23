using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("CheckIns")]
public class CheckIn
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(ColumnDataType = "int", IsNullable = false)]
    public int UserId { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(100)", IsNullable = false)]
    public string Status { get; set; }
    
    [SugarColumn(ColumnDataType = "date", IsNullable = false, DefaultValue = "CURRENT_DATE")]
    public DateTime CheckInDate { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public User User { get; set; }
}