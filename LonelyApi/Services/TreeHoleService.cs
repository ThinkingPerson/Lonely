using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;
using Microsoft.Extensions.Configuration;

namespace LonelyApi.Services;

public class TreeHoleService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _config;
    
    public TreeHoleService(ISqlSugarClient db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }
    
    public async Task<ApiResponse<TreeHole>> PostTreeHole(int userId, TreeHoleRequest request)
    {
        var expiryHours = _config.GetValue<int>("AppSettings:TreeHoleExpiryHours");
        
        var treeHole = new TreeHole
        {
            UserId = userId,
            Content = request.Content,
            VoiceUrl = request.VoiceUrl,
            Status = "active",
            ExpiredAt = DateTime.Now.AddHours(expiryHours)
        };
        
        await _db.Insertable(treeHole).ExecuteCommandAsync();
        
        return new ApiResponse<TreeHole>(true, "树洞已成功发出", treeHole);
    }
    
    public async Task<ApiResponse<TreeHole>> GetRandomTreeHole(int userId)
    {
        // 查找一个当前用户未看到过的树洞
        // 实际实现需要创建一个关联表来记录用户已查看的树洞
        var treeHole = await _db.Queryable<TreeHole>()
            .Where(t => t.Status == "active" && t.UserId != userId && t.ExpiredAt > DateTime.Now)
            .OrderBy(random => true)
            .FirstAsync();
        
        if (treeHole == null)
        {
            return new ApiResponse<TreeHole>(false, "暂无树洞", null);
        }
        
        return new ApiResponse<TreeHole>(true, "获取成功", treeHole);
    }
    
    public async Task<ApiResponse<List<TreeHole>>> GetMyTreeHoles(int userId)
    {
        var treeHoles = await _db.Queryable<TreeHole>()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        
        return new ApiResponse<List<TreeHole>>(true, "获取成功", treeHoles);
    }
    
    public async Task<ApiResponse> DeleteTreeHole(int treeHoleId, int userId)
    {
        var treeHole = await _db.Queryable<TreeHole>()
            .Where(t => t.Id == treeHoleId && t.UserId == userId)
            .FirstAsync();
        
        if (treeHole == null)
        {
            return new ApiResponse(false, "树洞不存在或无权限");
        }
        
        await _db.Deleteable(treeHole).ExecuteCommandAsync();
        
        return new ApiResponse(true, "树洞已删除");
    }
}
