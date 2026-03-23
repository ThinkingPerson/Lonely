using LonelyApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 添加日志服务
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 添加CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 添加JWT认证
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

// 添加SqlSugar
builder.Services.AddSingleton<ISqlSugarClient>(s =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    var db = new SqlSugarClient(new ConnectionConfig
    {
        ConnectionString = connectionString,
        DbType = DbType.MySql,
        IsAutoCloseConnection = true,
        InitKeyType = InitKeyType.Attribute
    });
    return db;
});

// 添加Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Lonely API", Version = "v1" });
    
    // 添加JWT认证支持
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "请输入JWT令牌"
    });
    
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var assembly = Assembly.GetExecutingAssembly(); // 或指定程序集

// 找到所有实现 IService 的非抽象类
var serviceTypes = assembly.GetTypes().Where(t => typeof(IService).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

foreach (var type in serviceTypes)
{
    // 如果有接口，优先注册为 接口 = 实现类
    var interfaces = type.GetInterfaces().Where(i => i != typeof(IService));
    if (interfaces.Any())
    {
        foreach (var i in interfaces)
        {
            builder.Services.AddScoped(i, type);
        }
    }
    else
    {
        // 没有接口，直接注册自身
        builder.Services.AddScoped(type);
    }
}


// 添加控制器
builder.Services.AddControllers();

var app = builder.Build();
app.UseForwardedHeaders();
// 日志中间件
app.Use(async (context, next) =>
{
    // 记录请求开始
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {context.Request.Method} {context.Request.Path}");
    
    // 记录请求头
    foreach (var header in context.Request.Headers)
    {
        // 不记录敏感信息
        if (!header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogDebug($"Header: {header.Key} = {header.Value}");
        }
    }
    
    // 继续处理请求
    await next();
    
    // 记录响应状态码
    logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Response Status: {context.Response.StatusCode}");
});

// 使用异常处理中间件
app.UseMiddleware<LonelyApi.Middlewares.ExceptionHandlerMiddleware>();

// 使用CORS
app.UseCors("AllowAll");

// 使用静态文件服务
app.UseStaticFiles();

// 使用Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Lonely API v1");
    options.RoutePrefix = "swagger";
});

// 使用认证
app.UseAuthentication();
app.UseAuthorization();

// 未登录拦截中间件
app.Use(async (context, next) =>
{
    var requestPath = context.Request.Path.Value?.ToLower() ?? "";

    // 跳过认证的路径
    var skipPaths = new List<string>
    {
        "/api/Auth/QuickLogin",
        "/api/Auth/AnonymousLogin",
        "/Auth/QuickLogin",
        "/Auth/AnonymousLogin",
        "/swagger",
        "/swagger/index.html",
        "/swagger/v1/swagger.json"
    };
    if (skipPaths.Any(path => requestPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)))
    {
        await next();
        return;
    }

    // 检查是否已认证
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"success\": false, \"message\": \"未登录或登录已过期\", \"data\": null}");
        return;
    }

    await next();
});

app.MapControllers();

app.Run();
