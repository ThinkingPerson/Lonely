using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LonelyApi.Services;
using LonelyApi.DTOs;
using System.Security.Claims;

namespace LonelyApi.Controllers;

/// <summary>
/// 认证控制器
/// 处理用户登录、匿名登录和获取用户信息等操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="authService">认证服务</param>
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// 快速登录接口
    /// </summary>
    /// <param name="request">登录请求参数</param>
    /// <returns>登录响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Auth/Login
    /// </remarks>
    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> QuickLogin([FromBody] LoginRequest request)
    {
        var response = await _authService.QuickLogin(request.Account, request.Password);
        return Ok(response);
    }
    
    /// <summary>
    /// 匿名登录接口
    /// </summary>
    /// <param name="request">匿名登录请求参数</param>
    /// <returns>登录响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Auth/AnonymousLogin
    /// </remarks>
    [HttpPost("AnonymousLogin")]
    public async Task<ActionResult<ApiResponse<UserInfo>>> AnonymousLogin([FromBody] UserInfo request)
    {
        var user = await _authService.AnonymousLogin(request.AnonUID, request.Nickname, request.Avatar);
        
        var userInfo = new UserInfo
        {
            AnonUID = user.AnonUID,
            Nickname = user.Nickname,
            Avatar = user.Avatar,
            LoginType = user.LoginType
        };
        
        return Ok(new ApiResponse<UserInfo>(true, "登录成功", userInfo));
    }
    
    /// <summary>
    /// 获取用户信息接口
    /// </summary>
    /// <returns>用户信息响应</returns>
    /// <remarks>
    /// 请求类型: GET
    /// 接口路径: api/Auth/UserInfo
    /// 需要认证
    /// </remarks>
    [HttpGet("UserInfo")]
    [Authorize]
    public ActionResult<ApiResponse<UserInfo>> GetUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var anonUID = User.FindFirstValue(ClaimTypes.Name);
        var nickname = User.FindFirstValue("nickname");
        var avatar = User.FindFirstValue("avatar");
        var loginType = User.FindFirstValue("loginType");
        
        var userInfo = new UserInfo
        {
            AnonUID = anonUID,
            Nickname = nickname,
            Avatar = avatar,
            LoginType = loginType
        };
        
        return Ok(new ApiResponse<UserInfo>(true, "获取成功", userInfo));
    }
}
