using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using LonelyApi.DTOs;

namespace LonelyApi.Controllers;

/// <summary>
/// 上传控制器
/// 处理文件上传操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly string _uploadPath;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UploadController(IConfiguration configuration)
    {
        _uploadPath = configuration["ApiUrl"];

        // 确保目录存在
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    /// <summary>
    /// 上传图片接口
    /// </summary>
    /// <param name="file">图片文件</param>
    /// <param name="type">文件类型</param>
    /// <returns>上传响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Upload/Image
    /// 需要认证
    /// </remarks>
    [HttpPost("Image")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(IFormFile file, [FromForm] string type)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse<string>(false, "请选择文件", null));
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest(new ApiResponse<string>(false, "请选择图片文件", null));
        }

        if (file.Length > 5 * 1024 * 1024) // 5MB限制
        {
            return BadRequest(new ApiResponse<string>(false, "图片大小不能超过5MB", null));
        }

        try
        {
            // 创建图片存储目录
            var imagePath = Path.Combine(_uploadPath, "images");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            // 生成唯一文件名
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(imagePath, fileName);

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 生成文件访问URL
            var fileUrl = $"/uploads/images/{fileName}";
            return Ok(new ApiResponse<string>(true, "上传成功", fileUrl));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, "上传失败: " + ex.Message, null));
        }
    }

    /// <summary>
    /// 上传语音接口
    /// </summary>
    /// <param name="file">语音文件</param>
    /// <param name="type">文件类型</param>
    /// <returns>上传响应</returns>
    /// <remarks>
    /// 请求类型: POST
    /// 接口路径: api/Upload/Voice
    /// 需要认证
    /// </remarks>
    [HttpPost("Voice")]
    public async Task<ActionResult<ApiResponse<string>>> UploadVoice(IFormFile file, [FromForm] string type)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse<string>(false, "请选择文件", null));
        }

        if (!file.ContentType.StartsWith("audio/"))
        {
            return BadRequest(new ApiResponse<string>(false, "请选择语音文件", null));
        }

        if (file.Length > 10 * 1024 * 1024) // 10MB限制
        {
            return BadRequest(new ApiResponse<string>(false, "语音大小不能超过10MB", null));
        }

        try
        {
            // 创建语音存储目录
            var voicePath = Path.Combine(_uploadPath, "voices");
            if (!Directory.Exists(voicePath))
            {
                Directory.CreateDirectory(voicePath);
            }

            // 生成唯一文件名
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(voicePath, fileName);

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 生成文件访问URL
            var fileUrl = $"/uploads/voices/{fileName}";
            return Ok(new ApiResponse<string>(true, "上传成功", fileUrl));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(false, "上传失败: " + ex.Message, null));
        }
    }
}