using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace LonelyApi.Services;

public class AuthService
{
    private readonly ISqlSugarClient _db;
    private readonly IConfiguration _config;
    
    public AuthService(ISqlSugarClient db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }
    
    public async Task<LoginResponse> QuickLogin(string phone, string password)
    {
        // 模拟快捷登录验证
        if (phone == "18860423687" && password == "423687")
        {
            // 生成随机匿名UID
            var anonUID = Guid.NewGuid().ToString("N");
            
            // 检查用户是否存在
            var existingUser = await _db.Queryable<User>()
                .Where(u => u.Phone == phone)
                .FirstAsync();
            
            User user;
            if (existingUser != null)
            {
                // 更新用户信息
                existingUser.AnonUID = anonUID;
                existingUser.LastLoginTime = DateTime.Now;
                existingUser.LoginType = "quick";
                await _db.Updateable(existingUser).ExecuteCommandAsync();
                user = existingUser;
            }
            else
            {
                // 创建新用户
                user = new User
                {
                    AnonUID = anonUID,
                    Nickname = GenerateRandomNickname(),
                    Avatar = GenerateRandomAvatar(),
                    Phone = phone,
                    LoginType = "quick",
                    LastLoginTime = DateTime.Now
                };
                await _db.Insertable(user).ExecuteCommandAsync();
            }
            
            // 生成JWT令牌
            var token = GenerateJwtToken(user);
            
            return new LoginResponse
            {
                Success = true,
                Token = token,
                User = new UserInfo
                {
                    AnonUID = user.AnonUID,
                    Nickname = user.Nickname,
                    Avatar = user.Avatar,
                    LoginType = user.LoginType
                }
            };
        }
        
        return new LoginResponse
        {
            Success = false,
            Token = null,
            User = null
        };
    }
    
    public async Task<User> AnonymousLogin(string anonUID, string nickname, string avatar)
    {
        // 检查用户是否存在
        var existingUser = await _db.Queryable<User>()
            .Where(u => u.AnonUID == anonUID)
            .FirstAsync();
        
        if (existingUser != null)
        {
            // 更新用户信息
            existingUser.LastLoginTime = DateTime.Now;
            await _db.Updateable(existingUser).ExecuteCommandAsync();
            return existingUser;
        }
        else
        {
            // 创建新用户
            var user = new User
            {
                AnonUID = anonUID,
                Nickname = nickname,
                Avatar = avatar,
                LoginType = "anonymous",
                LastLoginTime = DateTime.Now
            };
            await _db.Insertable(user).ExecuteCommandAsync();
            return user;
        }
    }
    
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.AnonUID),
            new Claim("nickname", user.Nickname),
            new Claim("avatar", user.Avatar),
            new Claim("loginType", user.LoginType)
        };
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(jwtSettings["ExpiresInMinutes"]!)),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateRandomNickname()
    {
        var adjectives = new string[] { "快乐的", "神秘的", "自由的", "安静的", "活泼的", "孤独的", "温暖的", "勇敢的" };
        var nouns = new string[] { "旅行者", "探险家", "守望者", "梦想家", "倾听者", "创造者", "守护者", "思考者" };
        var adj = adjectives[new Random().Next(adjectives.Length)];
        var noun = nouns[new Random().Next(nouns.Length)];
        var num = new Random().Next(1000);
        return $"{adj}{noun}{num}";
    }
    
    private string GenerateRandomAvatar()
    {
        var colors = new string[] { "#FF6B6B", "#4ECDC4", "#45B7D1", "#FFA07A", "#98D8C8", "#F7DC6F", "#BB8FCE", "#85C1E9" };
        return colors[new Random().Next(colors.Length)];
    }
}
