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
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="bottleService">漂流瓶服务</param>
    public BottleController(BottleService bottleService)
    {
        _bottleService = bottleService;
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _bottleService.ThrowBottle(userId, request);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _bottleService.PickBottle(userId);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _bottleService.GetMyBottles(userId);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _bottleService.GetReceivedBottles(userId);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _bottleService.DeleteBottle(id, userId);
        return Ok(response);
    }
}
