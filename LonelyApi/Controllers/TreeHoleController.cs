using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 树洞控制器
/// 处理树洞的发布、随机获取、查看和删除等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TreeHoleController : ControllerBase
{
    private readonly TreeHoleService _treeHoleService;
    private readonly StatsService _statsService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="treeHoleService">树洞服务</param>
    /// <param name="statsService">统计服务</param>
    public TreeHoleController(TreeHoleService treeHoleService, StatsService statsService)
    {
        _treeHoleService = treeHoleService;
        _statsService = statsService;
    }
    
    /// <summary>
    /// 发布树洞接口
    /// </summary>
    /// <param name="request">树洞请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/TreeHole/Post
    /// 需要认证
    /// </remarks>
    [HttpPost("Post")]
    public async Task<ActionResult<ApiResponse<object>>> PostTreeHole([FromBody] TreeHoleRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (string.IsNullOrEmpty(request.Content) && string.IsNullOrEmpty(request.VoiceUrl))
        {
            return BadRequest(new ApiResponse<object>(false, "树洞内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _treeHoleService.PostTreeHole(userId, request);
            
            // 记录树洞统计
            await _statsService.RecordTreeHole();
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "发布失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 随机获取树洞接口
    /// </summary>
    /// <returns>树洞响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/TreeHole/Random
    /// 需要认证
    /// </remarks>
    [HttpGet("Random")]
    public async Task<ActionResult<ApiResponse<object>>> GetRandomTreeHole()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _treeHoleService.GetRandomTreeHole(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取我的树洞接口
    /// </summary>
    /// <returns>树洞列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/TreeHole/My
    /// 需要认证
    /// </remarks>
    [HttpGet("My")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetMyTreeHoles()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _treeHoleService.GetMyTreeHoles(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 删除树洞接口
    /// </summary>
    /// <param name="id">树洞ID</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: DELETE
    /// 接口路径: api/TreeHole/{id}
    /// 需要认证
    /// </remarks>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteTreeHole(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ApiResponse(false, "树洞ID无效"));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _treeHoleService.DeleteTreeHole(id, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "删除失败: " + ex.Message));
        }
    }

    /// <summary>
    /// 回复树洞接口
    /// </summary>
    /// <param name="request">回复请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/TreeHole/Reply
    /// 需要认证
    /// </remarks>
    [HttpPost("Reply")]
    public async Task<ActionResult<ApiResponse<object>>> ReplyTreeHole([FromBody] TreeHoleReplyRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (request.TreeHoleId <= 0)
        {
            return BadRequest(new ApiResponse<object>(false, "树洞ID无效", null));
        }

        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest(new ApiResponse<object>(false, "回复内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _treeHoleService.AddTreeHoleReply(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "回复失败: " + ex.Message, null));
        }
    }

    /// <summary>
    /// 获取树洞回复列表接口
    /// </summary>
    /// <param name="treeHoleId">树洞ID</param>
    /// <returns>回复列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/TreeHole/Replies/{treeHoleId}
    /// 需要认证
    /// </remarks>
    [HttpGet("Replies/{treeHoleId}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetTreeHoleReplies(int treeHoleId)
    {
        if (treeHoleId <= 0)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "树洞ID无效", null));
        }

        try
        {
            var response = await _treeHoleService.GetTreeHoleReplies(treeHoleId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<object>>(false, "获取失败: " + ex.Message, null));
        }
    }
}
