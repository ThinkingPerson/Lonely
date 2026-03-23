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
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (request.ToUserId <= 0)
        {
            return BadRequest(new ApiResponse<object>(false, "接收者ID无效", null));
        }

        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest(new ApiResponse<object>(false, "消息内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _messageService.SendMessage(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "发送消息失败: " + ex.Message, null));
        }
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
        if (otherUserId <= 0)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "用户ID无效", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _messageService.GetMessages(userId, otherUserId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取消息失败: " + ex.Message, null));
        }
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
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _messageService.GetUnreadCount(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<int>(false, "获取未读消息计数失败: " + ex.Message, 0));
        }
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
        if (id <= 0)
        {
            return BadRequest(new ApiResponse(false, "消息ID无效"));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _messageService.DeleteMessage(id, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "删除消息失败: " + ex.Message));
        }
    }
}
