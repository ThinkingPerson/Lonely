using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;

namespace LonelyApi.Services;

public class ReportService : IService
{
    private readonly ISqlSugarClient _db;
    
    public ReportService(ISqlSugarClient db)
    {
        _db = db;
    }
    
    public async Task<ApiResponse<Report>> SubmitReport(int reporterId, ReportRequest request)
    {
        var report = new Report
        {
            ReporterId = reporterId,
            ReportedUserId = request.ReportedUserId,
            ReportType = request.ReportType,
            Content = request.Content,
            Status = "pending"
        };
        
        await _db.Insertable(report).ExecuteCommandAsync();
        
        return new ApiResponse<Report>(true, "举报已提交", report);
    }
    
    public async Task<ApiResponse<List<Report>>> GetMyReports(int userId)
    {
        var reports = await _db.Queryable<Report>()
            .Where(r => r.ReporterId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        
        return new ApiResponse<List<Report>>(true, "获取成功", reports);
    }
}
