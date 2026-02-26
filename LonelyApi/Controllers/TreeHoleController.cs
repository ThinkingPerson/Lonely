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
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="treeHoleService">树洞服务</param>
    public TreeHoleController(TreeHoleService treeHoleService)
    {
        _treeHoleService = treeHoleService;
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _treeHoleService.PostTreeHole(userId, request);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _treeHoleService.GetRandomTreeHole(userId);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _treeHoleService.GetMyTreeHoles(userId);
        return Ok(response);
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _treeHoleService.DeleteTreeHole(id, userId);
        return Ok(response);
    }
}
