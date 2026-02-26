using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;
using Microsoft.Extensions.Configuration;

namespace LonelyApi.Services;

public class MatchService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _config;
    
    public MatchService(ISqlSugarClient db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }
    
    public async Task<ApiResponse<Match>> StartMatching(int userId)
    {
        var expiryMinutes = _config.GetValue<int>("AppSettings:MatchExpiryMinutes");
        
        // 查找一个等待匹配的用户
        var waitingUser = await _db.Queryable<User>()
            .Where(u => u.Id != userId)
            .OrderBy(random => true)
            .FirstAsync();
        
        if (waitingUser == null)
        {
            // 没有等待的用户，创建一个等待匹配的记录
            var match = new Match
            {
                UserId1 = userId,
                UserId2 = 0, // 占位符
                Status = "pending",
                ExpiredAt = DateTime.Now.AddMinutes(expiryMinutes)
            };
            await _db.Insertable(match).ExecuteCommandAsync();
            
            return new ApiResponse<Match>(true, "正在等待匹配...", match);
        }
        
        // 创建匹配记录
        var newMatch = new Match
        {
            UserId1 = userId,
            UserId2 = waitingUser.Id,
            Status = "matched",
            ExpiredAt = DateTime.Now.AddMinutes(expiryMinutes)
        };
        await _db.Insertable(newMatch).ExecuteCommandAsync();
        
        return new ApiResponse<Match>(true, "匹配成功", newMatch);
    }
    
    public async Task<ApiResponse<Match>> GetMatchStatus(int userId)
    {
        var match = await _db.Queryable<Match>()
            .Where(m => (m.UserId1 == userId || m.UserId2 == userId) && m.Status == "matched" && m.ExpiredAt > DateTime.Now)
            .FirstAsync();
        
        if (match == null)
        {
            return new ApiResponse<Match>(false, "暂无匹配", null);
        }
        
        return new ApiResponse<Match>(true, "获取成功", match);
    }
    
    public async Task<ApiResponse> CancelMatching(int userId)
    {
        var match = await _db.Queryable<Match>()
            .Where(m => m.UserId1 == userId && m.Status == "pending")
            .FirstAsync();
        
        if (match == null)
        {
            return new ApiResponse(false, "没有正在进行的匹配");
        }
        
        await _db.Deleteable(match).ExecuteCommandAsync();
        
        return new ApiResponse(true, "匹配已取消");
    }
    
    public async Task<ApiResponse> EndMatch(int matchId, int userId)
    {
        var match = await _db.Queryable<Match>()
            .Where(m => m.Id == matchId && (m.UserId1 == userId || m.UserId2 == userId))
            .FirstAsync();
        
        if (match == null)
        {
            return new ApiResponse(false, "匹配不存在或无权限");
        }
        
        match.Status = "expired";
        await _db.Updateable(match).ExecuteCommandAsync();
        
        return new ApiResponse(true, "匹配已结束");
    }
}
