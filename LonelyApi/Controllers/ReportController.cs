using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 举报控制器
/// 处理举报的提交和查看等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="reportService">举报服务</param>
    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }
    
    /// <summary>
    /// 提交举报接口
    /// </summary>
    /// <param name="request">举报请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Report/Submit
    /// 需要认证
    /// </remarks>
    [HttpPost("Submit")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitReport([FromBody] ReportRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _reportService.SubmitReport(userId, request);
        return Ok(response);
    }
    
    /// <summary>
    /// 获取我的举报接口
    /// </summary>
    /// <returns>举报列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Report/My
    /// 需要认证
    /// </remarks>
    [HttpGet("My")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMyReports()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _reportService.GetMyReports(userId);
        return Ok(response);
    }
}
