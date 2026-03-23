using LonelyApi.DTOs;
using LonelyApi.Models;
using LonelyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 动态控制器
/// 处理动态的发布、获取、点赞和评论等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly PostService _postService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="postService">动态服务</param>
    public PostController(PostService postService)
    {
        _postService = postService;
    }
    
    /// <summary>
    /// 发布动态接口
    /// </summary>
    /// <param name="request">动态请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Post/Create
    /// 需要认证
    /// </remarks>
    [HttpPost("Create")]
    public async Task<ActionResult<ApiResponse<object>>> CreatePost([FromBody] PostRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (string.IsNullOrEmpty(request.Content) && (request.Images == null || request.Images.Count == 0) && string.IsNullOrEmpty(request.Audio))
        {
            return BadRequest(new ApiResponse<object>(false, "动态内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _postService.CreatePost(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "发布失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取动态列表接口
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>动态列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Post/Get
    /// 需要认证
    /// </remarks>
    [HttpGet("Get")]
    public async Task<ActionResult<ApiResponse<object>>> GetPosts(int userId, int page = 1, int pageSize = 10)
    {
        if (userId <= 0)
        {
            return BadRequest(new ApiResponse(false, "当前用户参数丢失"));
        }

        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1 || pageSize > 50)
        {
            pageSize = 10;
        }

        try
        {
            var response = await _postService.GetPosts(userId, page, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 点赞/取消点赞接口
    /// </summary>
    /// <param name="postId">动态ID</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Post/Like/{postId}
    /// 需要认证
    /// </remarks>
    [HttpPost("Like/{postId}")]
    public async Task<ActionResult<ApiResponse>> LikePost(int postId)
    {
        if (postId <= 0)
        {
            return BadRequest(new ApiResponse(false, "动态ID无效"));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _postService.LikePost(postId, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(false, "操作失败: " + ex.Message));
        }
    }
    
    /// <summary>
    /// 添加评论接口
    /// </summary>
    /// <param name="request">评论请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Post/Comment
    /// 需要认证
    /// </remarks>
    [HttpPost("Comment")]
    public async Task<ActionResult<ApiResponse<object>>> AddComment([FromBody] CommentRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (request.PostId <= 0)
        {
            return BadRequest(new ApiResponse<object>(false, "动态ID无效", null));
        }

        if (string.IsNullOrEmpty(request.Content))
        {
            return BadRequest(new ApiResponse<object>(false, "评论内容不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _postService.AddComment(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "评论失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取评论列表接口
    /// </summary>
    /// <param name="postId">动态ID</param>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>评论列表响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Post/Comments/{postId}
    /// 需要认证
    /// </remarks>
    [HttpGet("Comments/{postId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetComments(int postId, int page = 1, int pageSize = 20)
    {
        if (postId <= 0)
        {
            return BadRequest(new ApiResponse<object>(false, "动态ID无效", null));
        }
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1 || pageSize > 50)
        {
            pageSize = 20;
        }

        try
        {
            var response = await _postService.GetComments(postId, page, pageSize);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "获取失败: " + ex.Message, null));
        }
    }
}