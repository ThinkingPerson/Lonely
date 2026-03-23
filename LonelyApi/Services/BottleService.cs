using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;
using Microsoft.Extensions.Configuration;

namespace LonelyApi.Services;

public class BottleService : IService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _config;

    public BottleService(ISqlSugarClient db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<ApiResponse<Bottle>> ThrowBottle(int userId, BottleRequest request)
    {
        var expiryHours = _config.GetValue<int>("AppSettings:BottleExpiryHours");

        var bottle = new Bottle
        {
            UserId = userId,
            Content = request.Content,
            VoiceUrl = request.VoiceUrl,
            ImageUrl = request.ImageUrl,
            Emotion = request.Emotion,
            Topics = request.Topics,
            Status = "floating",
            ExpiredAt = DateTime.Now.AddHours(expiryHours)
        };

        await _db.Insertable(bottle).ExecuteCommandAsync();

        return new ApiResponse<Bottle>(true, "瓶子已成功扔出", bottle);
    }

    public async Task<ApiResponse<Bottle>> PickBottle(int userId)
    {
        // 查找一个未被当前用户捡到的漂流瓶
        var bottle = await _db.Queryable<Bottle>()
            .Where(b => b.Status == "floating" && b.UserId != userId && b.ExpiredAt > DateTime.Now)
            .OrderBy("RAND()")
            .FirstAsync();

        if (bottle == null)
        {
            return new ApiResponse<Bottle>(false, "暂无漂流瓶", null);
        }

        // 更新瓶子状态
        bottle.Status = "received";
        await _db.Updateable(bottle).ExecuteCommandAsync();

        return new ApiResponse<Bottle>(true, "成功捡到漂流瓶", bottle);
    }

    public async Task<ApiResponse<List<Bottle>>> GetMyBottles(int userId)
    {
        var bottles = await _db.Queryable<Bottle>()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return new ApiResponse<List<Bottle>>(true, "获取成功", bottles);
    }

    public async Task<ApiResponse<List<Bottle>>> GetReceivedBottles(int userId)
    {
        // 这里需要关联查询，暂时返回空列表
        // 实际实现需要创建一个关联表来记录谁捡到了哪个瓶子
        return new ApiResponse<List<Bottle>>(true, "获取成功", new List<Bottle>());
    }

    public async Task<ApiResponse> DeleteBottle(int bottleId, int userId)
    {
        var bottle = await _db.Queryable<Bottle>()
            .Where(b => b.Id == bottleId && b.UserId == userId)
            .FirstAsync();

        if (bottle == null)
        {
            return new ApiResponse(false, "瓶子不存在或无权限");
        }

        await _db.Deleteable(bottle).ExecuteCommandAsync();

        return new ApiResponse(true, "瓶子已删除");
    }

    public async Task<ApiResponse<BottleReply>> AddBottleReply(int userId, BottleReplyRequest request)
    {
        // 检查瓶子是否存在
        var bottle = await _db.Queryable<Bottle>()
            .Where(b => b.Id == request.BottleId && b.Status == "received" && b.ExpiredAt > DateTime.Now)
            .FirstAsync();

        if (bottle == null)
        {
            return new ApiResponse<BottleReply>(false, "瓶子不存在或已过期", null);
        }

        var reply = new BottleReply
        {
            BottleId = request.BottleId,
            UserId = userId,
            Content = request.Content
        };

        await _db.Insertable(reply).ExecuteCommandAsync();

        return new ApiResponse<BottleReply>(true, "回复成功", reply);
    }

    public async Task<ApiResponse<List<object>>> GetBottleReplies(int bottleId)
    {
        var replies = await _db.Queryable<BottleReply>()
            .Includes(r => r.User)
            .Where(r => r.BottleId == bottleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var result = new List<object>();
        foreach (var reply in replies)
        {
            var replyData = new
            {
                id = reply.Id,
                userId = reply.UserId,
                content = reply.Content,
                time = reply.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                username = "匿名"
            };
            result.Add(replyData);
        }

        return new ApiResponse<List<object>>(true, "获取回复成功", result);
    }
}
