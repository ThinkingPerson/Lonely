using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;

namespace LonelyApi.Services;

public class CheckInService : IService
{
    private readonly ISqlSugarClient _db;
    
    public CheckInService(ISqlSugarClient db)
    {
        _db = db;
    }
    
    public async Task<ApiResponse<CheckIn>> CheckIn(int userId, CheckInRequest request)
    {
        // 检查今日是否已签到
        var today = DateTime.Today;
        var existingCheckIn = await _db.Queryable<CheckIn>()
            .Where(c => c.UserId == userId && c.CheckInDate == today)
            .FirstAsync();
        
        if (existingCheckIn != null)
        {
            return new ApiResponse<CheckIn>(false, "今日已签到", existingCheckIn);
        }
        
        var checkIn = new CheckIn
        {
            UserId = userId,
            Status = request.Status,
            CheckInDate = today
        };
        
        await _db.Insertable(checkIn).ExecuteCommandAsync();
        
        return new ApiResponse<CheckIn>(true, "签到成功", checkIn);
    }
    
    public async Task<ApiResponse<object>> GetCheckInStatus(int userId)
    {
        var today = DateTime.Today;
        var checkIn = await _db.Queryable<CheckIn>()
            .Where(c => c.UserId == userId && c.CheckInDate == today)
            .FirstAsync();
        
        if (checkIn != null)
        {
            return new ApiResponse<object>(true, "今日已签到", new
            {
                isCheckedIn = true,
                status = checkIn.Status
            });
        }
        else
        {
            return new ApiResponse<object>(true, "今日未签到", new
            {
                isCheckedIn = false,
                status = ""
            });
        }
    }
    
    public async Task<ApiResponse<List<CheckIn>>> GetCheckInHistory(int userId, int days = 30)
    {
        var startDate = DateTime.Today.AddDays(-days);
        var checkIns = await _db.Queryable<CheckIn>()
            .Where(c => c.UserId == userId && c.CheckInDate >= startDate)
            .OrderByDescending(c => c.CheckInDate)
            .ToListAsync();
        
        return new ApiResponse<List<CheckIn>>(true, "获取签到历史成功", checkIns);
    }
}