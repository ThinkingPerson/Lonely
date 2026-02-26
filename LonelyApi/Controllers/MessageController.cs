using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 消息控制器
/// 处理消息的发送、获取、未读计数和删除等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="messageService">消息服务</param>
    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }
    
    /// <summary>
    /// 发送消息接口
    /// </summary>
    /// <param name="request">消息请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Message/Send
    /// 需要认证
    /// </remarks>
    [HttpPost("Send")]
    public async Task<ActionResult<ApiResponse<object>>> SendMessage([FromBody] MessageRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _messageService.SendMessage(userId, request);
        return Ok(response);
    }
    
    /// <summary>
    /// 获取消息列表接口
    /// </summary>
    /// <param name="otherUserId">对方用户ID</param>
    /// <returns>消息列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Message/Get/{otherUserId}
    /// 需要认证
    /// </remarks>
    [HttpGet("Get/{otherUserId}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMessages(int otherUserId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _messageService.GetMessages(userId, otherUserId);
        return Ok(response);
    }
    
    /// <summary>
    /// 获取未读消息计数接口
    /// </summary>
    /// <returns>未读消息计数响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Message/UnreadCount
    /// 需要认证
    /// </remarks>
    [HttpGet("UnreadCount")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _messageService.GetUnreadCount(userId);
        return Ok(response);
    }
    
    /// <summary>
    /// 删除消息接口
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: DELETE
    /// 接口路径: api/Message/{id}
    /// 需要认证
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteMessage(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _messageService.DeleteMessage(id, userId);
        return Ok(response);
    }
}
