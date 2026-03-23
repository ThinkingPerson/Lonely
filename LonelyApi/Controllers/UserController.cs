using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LonelyApi.Controllers;

/// <summary>
/// 用户控制器
/// 处理用户信息的更新、头像上传、获取用户信息和账号绑定等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    /// 更新用户信息接口
    /// </summary>
    /// <param name="request">用户信息请求参数</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/User/UpdateProfile
    /// 需要认证
    /// </remarks>
    [HttpPost("UpdateProfile")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UserInfo request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _userService.UpdateProfile(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "更新失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 上传头像接口
    /// </summary>
    /// <param name="file">头像文件</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/User/UploadAvatar
    /// 需要认证
    /// </remarks>
    [HttpPost("UploadAvatar")]
    public async Task<ActionResult<ApiResponse<object>>> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse<object>(false, "请选择文件", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _userService.UploadAvatar(userId, file);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "上传失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 获取用户信息接口
    /// </summary>
    /// <returns>用户信息响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/User/GetProfile
    /// 需要认证
    /// </remarks>
    [HttpGet("GetProfile")]
    public async Task<ActionResult<ApiResponse<object>>> GetProfile()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _userService.GetProfile(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "获取失败: " + ex.Message, null));
        }
    }
    
    /// <summary>
    /// 绑定账号接口
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <param name="password">密码</param>
    /// <returns>操作响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/User/BindAccount
    /// 需要认证
    /// </remarks>
    [HttpPost("BindAccount")]
    public async Task<ActionResult<ApiResponse<object>>> BindAccount([FromBody] LoginRequest request)
    {
        if (request == null)
        {
            return BadRequest(new ApiResponse<object>(false, "请求参数为空", null));
        }

        if (string.IsNullOrEmpty(request.Account))
        {
            return BadRequest(new ApiResponse<object>(false, "手机号不能为空", null));
        }

        if (string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new ApiResponse<object>(false, "密码不能为空", null));
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _userService.BindAccount(userId, request.Account, request.Password);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, "绑定失败: " + ex.Message, null));
        }
    }
}