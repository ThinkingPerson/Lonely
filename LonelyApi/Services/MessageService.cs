using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;

namespace LonelyApi.Services;

public class MessageService
{
    private readonly ISqlSugarClient _db;
    
    public MessageService(ISqlSugarClient db)
    {
        _db = db;
    }
    
    public async Task<ApiResponse<Message>> SendMessage(int fromUserId, MessageRequest request)
    {
        var message = new Message
        {
            FromUserId = fromUserId,
            ToUserId = request.ToUserId,
            Content = request.Content,
            Type = request.Type,
            IsRead = false
        };
        
        await _db.Insertable(message).ExecuteCommandAsync();
        
        return new ApiResponse<Message>(true, "消息已发送", message);
    }
    
    public async Task<ApiResponse<List<Message>>> GetMessages(int userId, int otherUserId)
    {
        var messages = await _db.Queryable<Message>()
            .Where(m => (m.FromUserId == userId && m.ToUserId == otherUserId) || 
                       (m.FromUserId == otherUserId && m.ToUserId == userId))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
        
        // 标记对方发送的消息为已读
        await _db.Updateable<Message>()
            .SetColumns(m => m.IsRead == true)
            .Where(m => m.FromUserId == otherUserId && m.ToUserId == userId && m.IsRead == false)
            .ExecuteCommandAsync();
        
        return new ApiResponse<List<Message>>(true, "获取成功", messages);
    }
    
    public async Task<ApiResponse<int>> GetUnreadCount(int userId)
    {
        var count = await _db.Queryable<Message>()
            .Where(m => m.ToUserId == userId && m.IsRead == false)
            .CountAsync();
        
        return new ApiResponse<int>(true, "获取成功", count);
    }
    
    public async Task<ApiResponse> DeleteMessage(int messageId, int userId)
    {
        var message = await _db.Queryable<Message>()
            .Where(m => m.Id == messageId && (m.FromUserId == userId || m.ToUserId == userId))
            .FirstAsync();
        
        if (message == null)
        {
            return new ApiResponse(false, "消息不存在或无权限");
        }
        
        await _db.Deleteable(message).ExecuteCommandAsync();
        
        return new ApiResponse(true, "消息已删除");
    }
}
