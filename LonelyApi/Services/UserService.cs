
using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LonelyApi.Services;

public class UserService : IService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _configuration;


    public UserService(ISqlSugarClient db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<ApiResponse<User>> UpdateProfile(int userId, UserInfo request)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            return new ApiResponse<User>(false, "用户不存在", null);
        }

        // 更新用户信息
        if (!string.IsNullOrEmpty(request.Nickname))
        {
            user.Nickname = request.Nickname;
        }

        if (!string.IsNullOrEmpty(request.Avatar))
        {
            user.Avatar = request.Avatar;
        }

        if (!string.IsNullOrEmpty(request.Signature))
        {
            user.Signature = request.Signature;
        }

        await _db.Updateable(user).ExecuteCommandAsync();

        return new ApiResponse<User>(true, "更新成功", user);
    }

    public async Task<ApiResponse<string>> UploadAvatar([FromForm] int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return new ApiResponse<string>(false, "请选择文件", null);
        }

        // 检查文件类型
        if (!file.ContentType.StartsWith("image/"))
        {
            return new ApiResponse<string>(false, "请上传图片文件", null);
        }

        // 检查文件大小（限制为2MB）
        if (file.Length > 2 * 1024 * 1024)
        {
            return new ApiResponse<string>(false, "文件大小不能超过2MB", null);
        }

        // 生成唯一文件名
        var fileName = $"avatar_{userId}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_configuration["ApiUrl"], "avatars");

        // 创建目录
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        // 保存文件
        var fullPath = Path.Combine(filePath, fileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 更新用户头像
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user != null)
        {
            user.Avatar = $"/uploads/avatars/{fileName}";
            await _db.Updateable(user).ExecuteCommandAsync();
        }

        return new ApiResponse<string>(true, "上传成功", $"/uploads/avatars/{fileName}");
    }

    public async Task<ApiResponse<User>> GetProfile(int userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            return new ApiResponse<User>(false, "用户不存在", null);
        }

        return new ApiResponse<User>(true, "获取成功", user);
    }

    public async Task<ApiResponse<User>> BindAccount(int userId, string phone, string password)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user == null)
        {
            return new ApiResponse<User>(false, "用户不存在", null);
        }

        // 检查手机号是否已被绑定
        var existingUser = await _db.Queryable<User>()
            .Where(u => u.Phone == phone && u.Id != userId)
            .FirstAsync();

        if (existingUser != null)
        {
            return new ApiResponse<User>(false, "该手机号已被绑定", null);
        }

        // 更新用户信息
        user.Phone = phone;
        user.Password = password; // 实际应用中应该使用密码加密
        user.LoginType = "quick";

        await _db.Updateable(user).ExecuteCommandAsync();

        return new ApiResponse<User>(true, "绑定成功", user);
    }
}