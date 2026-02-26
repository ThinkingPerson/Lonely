using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 匹配控制器
/// 处理用户匹配的开始、状态查询、取消和结束等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchController : ControllerBase
{
    private readonly MatchService _matchService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="matchService">匹配服务</param>
    public MatchController(MatchService matchService)
    {
        _matchService = matchService;
    }
    
    /// <summary>
    /// 开始匹配接口
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Match/Start
    /// 需要认证
    /// </remarks>
    [HttpPost("Start")]
    public async Task<ActionResult<ApiResponse<object>>> StartMatching()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _matchService.StartMatching(userId);
        return Ok(response);
    }
    
    /// <summary>
    /// 获取匹配状态接口
    /// </summary>
    /// <returns>匹配状态响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Match/Status
    /// 需要认证
    /// </remarks>
    [HttpGet("Status")]
    public async Task<ActionResult<ApiResponse<object>>> GetMatchStatus()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _matchService.GetMatchStatus(userId);
        return Ok(response);
    }
    
    /// <summary>
    /// 取消匹配接口
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Match/Cancel
    /// 需要认证
    /// </remarks>
    [HttpPost("Cancel")]
    public async Task<ActionResult<ApiResponse>> CancelMatching()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _matchService.CancelMatching(userId);
        return Ok(response);
    }
    
    /// <summary>
    /// 结束匹配接口
    /// </summary>
    /// <param name="id">匹配ID</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Match/End/{id}
    /// 需要认证
    /// </remarks>
    [HttpPost("End/{id}")]
    public async Task<ActionResult<ApiResponse>> EndMatch(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _matchService.EndMatch(id, userId);
        return Ok(response);
    }
}
