using SqlSugar;

namespace LonelyApi.Models;

[SugarTable("Users")]
public class User
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(64)", IsNullable = false, ColumnDescription = "唯一标识符")]
    public string AnonUID { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(50)", IsNullable = false)]
    public string Nickname { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(20)", IsNullable = false)]
    public string Avatar { get; set; }
    
    [SugarColumn(ColumnDataType = "varchar(20)", IsNullable = true)]
    public string? Phone { get; set; }
    
    [SugarColumn(ColumnDataType = "enum('anonymous','quick')", IsNullable = false, DefaultValue = "'anonymous'")]
    public string LoginType { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false)]
    public DateTime LastLoginTime { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP")]
    public DateTime CreatedAt { get; set; }
    
    [SugarColumn(ColumnDataType = "datetime", IsNullable = false, DefaultValue = "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")]
    public DateTime UpdatedAt { get; set; }
}
