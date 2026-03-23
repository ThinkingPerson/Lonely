using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 漂流瓶控制器
/// 处理漂流瓶的投掷、拾取、查看和删除等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BottleController : ControllerBase
{
    private readonly BottleService _bottleService;
    private readonly StatsService _statsService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="bottleService">漂流瓶服务</param>
    /// <param name="statsService">统计服务</param>
    public BottleController(BottleService bottleService, StatsService statsService)
    {
        _bottleService = bottleService;
        _statsService = statsService;
    }
    
    /// <summary>
    /// 投掷漂流瓶接口
    /// </summary>
    /// <param name="request">漂流瓶请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Bottle/Throw
    /// 需要认证
    /// </remarks>
    [HttpPost("Throw")]
    public async Task<ActionResult<ApiResponse<object>>> ThrowBottle([FromBody] BottleRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (string.IsNullOrEmpty(request.Content) && string.IsNullOrEmpty(request.VoiceUrl) && string.IsNullOrEmpty(request.ImageUrl))
        {
            return BadRequest(new ApiResponse<object>(false, "漂流瓶内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _bottleService.ThrowBottle(userId, request);
            
            // 记录扔瓶子统计
            await _statsService.RecordBottle();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "投掷失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 拾取漂流瓶接口
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Bottle/Pick
    /// 需要认证
    /// </remarks>
    [HttpGet("Pick")]
    public async Task<ActionResult<ApiResponse<object>>> PickBottle()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _bottleService.PickBottle(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "拾取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取我的漂流瓶接口
    /// </summary>
    /// <returns>漂流瓶列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Bottle/My
    /// 需要认证
    /// </remarks>
    [HttpGet("My")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMyBottles()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _bottleService.GetMyBottles(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取收到的漂流瓶接口
    /// </summary>
    /// <returns>漂流瓶列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Bottle/Received
    /// 需要认证
    /// </remarks>
    [HttpGet("Received")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetReceivedBottles()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _bottleService.GetReceivedBottles(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 删除漂流瓶接口
    /// </summary>
    /// <param name="id">漂流瓶ID</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: DELETE
    /// 接口路径: api/Bottle/{id}
    /// 需要认证
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteBottle(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ApiResponse(false, "漂流瓶ID无效"));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _bottleService.DeleteBottle(id, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "删除失败: " + ex.Message));
        }
    }

    /// <summary>
    /// 回复漂流瓶接口
    /// </summary>
    /// <param name="request">回复请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Bottle/Reply
    /// 需要认证
    /// </remarks>
    [HttpPost("Reply")]
    public async Task<ActionResult<ApiResponse<object>>> ReplyBottle([FromBody] BottleReplyRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (request.BottleId <= 0)
        {
            return BadRequest(new ApiResponse<object>(false, "漂流瓶ID无效", null));
        }

        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest(new ApiResponse<object>(false, "回复内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _bottleService.AddBottleReply(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "回复失败: " + ex.Message, null));
        }
    }

    /// <summary>
    /// 获取漂流瓶回复列表接口
    /// </summary>
    /// <param name="bottleId">漂流瓶ID</param>
    /// <returns>回复列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Bottle/Replies/{bottleId}
    /// 需要认证
    /// </remarks>
    [HttpGet("Replies/{bottleId}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetBottleReplies(int bottleId)
    {
        if (bottleId <= 0)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "漂流瓶ID无效", null));
        }

        try
        {
            var response = await _bottleService.GetBottleReplies(bottleId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
}
