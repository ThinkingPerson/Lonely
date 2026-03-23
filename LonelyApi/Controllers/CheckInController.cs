using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 签到控制器
/// 处理用户签到、获取签到状态等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInController : ControllerBase
{
    private readonly CheckInService _checkInService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="checkInService">签到服务</param>
    public CheckInController(CheckInService checkInService)
    {
        _checkInService = checkInService;
    }
    
    /// <summary>
    /// 签到接口
    /// </summary>
    /// <param name="request">签到请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/CheckIn/CheckIn
    /// 需要认证
    /// </remarks>
    [HttpPost("CheckIn")]
    public async Task<ActionResult<ApiResponse<object>>> CheckIn([FromBody] CheckInRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (string.IsNullOrEmpty(request.Status))
        {
            return BadRequest(new ApiResponse<object>(false, "签到状态不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _checkInService.CheckIn(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "签到失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取签到状态接口
    /// </summary>
    /// <returns>签到状态响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/CheckIn/Status
    /// 需要认证
    /// </remarks>
    [HttpGet("Status")]
    public async Task<ActionResult<ApiResponse<object>>> GetCheckInStatus()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _checkInService.GetCheckInStatus(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取签到历史接口
    /// </summary>
    /// <param name="days">查询天数</param>
    /// <returns>签到历史响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/CheckIn/History
    /// 需要认证
    /// </remarks>
    [HttpGet("History")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetCheckInHistory(int days = 30)
    {
        if (days <= 0)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "天数必须大于0", null));
        }

        if (days > 365)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "天数不能超过365", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _checkInService.GetCheckInHistory(userId, days);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
}