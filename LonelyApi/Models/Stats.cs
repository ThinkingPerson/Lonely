using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Stats")]
public class Stats
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(ColumnDataType = "date", IsNullable = false)]
    public DateTime Date { get; set; }
    
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int VisitCount { get; set; }
    
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int BottleCount { get; set; }
    
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int TreeHoleCount { get; set; }
    
    [SugarColumn(IsNullable = false, DefaultValue = "0")]
    public int PostCount { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; }
}