using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;

namespace LonelyApi.Controllers;

/// <summary>
/// 统计控制器
/// 处理网站访问和功能使用的统计数据
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly StatsService _statsService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="statsService">统计服务</param>
    public StatsController(StatsService statsService)
    {
        _statsService = statsService;
    }
    
    /// <summary>
    /// 记录网站访问
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Stats/RecordVisit
    /// 需要认证
    /// </remarks>
    [HttpPost("RecordVisit")]
    public async Task<ActionResult<ApiResponse>> RecordVisit()
    {
        try
        {
            await _statsService.RecordVisit();
            return Ok(new ApiResponse(true, "记录成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "记录失败: " + ex.Message));
        }
    }
    
    /// <summary>
    /// 记录扔瓶子
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Stats/RecordBottle
    /// 需要认证
    /// </remarks>
    [HttpPost("RecordBottle")]
    public async Task<ActionResult<ApiResponse>> RecordBottle()
    {
        try
        {
            await _statsService.RecordBottle();
            return Ok(new ApiResponse(true, "记录成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "记录失败: " + ex.Message));
        }
    }
    
    /// <summary>
    /// 记录树洞
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Stats/RecordTreeHole
    /// 需要认证
    /// </remarks>
    [HttpPost("RecordTreeHole")]
    public async Task<ActionResult<ApiResponse>> RecordTreeHole()
    {
        try
        {
            await _statsService.RecordTreeHole();
            return Ok(new ApiResponse(true, "记录成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "记录失败: " + ex.Message));
        }
    }
    
    /// <summary>
    /// 记录动态
    /// </summary>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Stats/RecordPost
    /// 需要认证
    /// </remarks>
    [HttpPost("RecordPost")]
    public async Task<ActionResult<ApiResponse>> RecordPost()
    {
        try
        {
            await _statsService.RecordPost();
            return Ok(new ApiResponse(true, "记录成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "记录失败: " + ex.Message));
        }
    }
    
    /// <summary>
    /// 获取每日统计数据
    /// </summary>
    /// <param name="days">天数</param>
    /// <returns>统计数据响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Stats/Daily
    /// 需要认证
    /// </remarks>
    [HttpGet("Daily")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetDailyStats(int days = 30)
    {
        try
        {
            var response = await _statsService.GetDailyStats(days);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取每周统计数据
    /// </summary>
    /// <param name="weeks">周数</param>
    /// <returns>统计数据响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Stats/Weekly
    /// 需要认证
    /// </remarks>
    [HttpGet("Weekly")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetWeeklyStats(int weeks = 12)
    {
        try
        {
            var response = await _statsService.GetWeeklyStats(weeks);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取每月统计数据
    /// </summary>
    /// <param name="months">月数</param>
    /// <returns>统计数据响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Stats/Monthly
    /// 需要认证
    /// </remarks>
    [HttpGet("Monthly")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMonthlyStats(int months = 6)
    {
        try
        {
            var response = await _statsService.GetMonthlyStats(months);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取统计趋势数据
    /// </summary>
    /// <param name="days">天数</param>
    /// <returns>趋势数据响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Stats/Trend
    /// 需要认证
    /// </remarks>
    [HttpGet("Trend")]
    public async Task<ActionResult<ApiResponse<object>>> GetStatsTrend(int days = 30)
    {
        try
        {
            var response = await _statsService.GetStatsTrend(days);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "获取失败: " + ex.Message, null));
        }
    }
}