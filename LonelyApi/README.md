# Lonely API 后端服务

## 项目概述

Lonely 是一个匿名·轻量·低压力的社交应用，后端基于 .NET 8 和 SqlSugar 实现，提供完整的 RESTful API 接口。

## 技术栈

- **框架**: .NET 8
- **ORM**: SqlSugarCore
- **数据库**: MySQL
- **认证**: JWT
- **Web框架**: ASP.NET Core Web API
- **跨域**: CORS

## 项目结构

```
api/
├── Controllers/         # API控制器
│   ├── AuthController.cs
│   ├── BottleController.cs
│   ├── TreeHoleController.cs
│   ├── MatchController.cs
│   ├── MessageController.cs
│   └── ReportController.cs
├── DTOs/                # 数据传输对象
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── BottleRequest.cs
│   ├── TreeHoleRequest.cs
│   ├── MessageRequest.cs
│   ├── ReportRequest.cs
│   └── ApiResponse.cs
├── Models/              # 数据库模型
│   ├── User.cs
│   ├── Bottle.cs
│   ├── TreeHole.cs
│   ├── Match.cs
│   ├── Message.cs
│   └── Report.cs
├── Services/            # 业务逻辑服务
│   ├── AuthService.cs
│   ├── BottleService.cs
│   ├── TreeHoleService.cs
│   ├── MatchService.cs
│   ├── MessageService.cs
│   └── ReportService.cs
├── database.sql         # 数据库脚本
├── appsettings.json     # 应用配置
├── LonelyApi.csproj     # 项目文件
└── Program.cs           # 应用入口
```

## 核心功能

### 1. 用户认证
- 快捷登录（手机+密码）
- 匿名登录（基于设备指纹）
- JWT令牌生成与验证

### 2. 漂流瓶功能
- 扔瓶子（支持文本、语音、图片）
- 捡瓶子（随机匹配）
- 查看我的瓶子
- 查看收到的瓶子

### 3. 树洞功能
- 发树洞（支持文本、语音）
- 随机获取树洞
- 查看我的树洞

### 4. 匹配功能
- 开始匹配
- 获取匹配状态
- 取消匹配
- 结束匹配

### 5. 消息功能
- 发送消息（支持文本、语音、图片）
- 获取聊天记录
- 获取未读消息数
- 删除消息

### 6. 举报功能
- 提交举报
- 查看我的举报

## API接口

### 认证相关
- `POST /api/Auth/Login` - 快捷登录
- `POST /api/Auth/AnonymousLogin` - 匿名登录
- `GET /api/Auth/UserInfo` - 获取用户信息

### 漂流瓶相关
- `POST /api/Bottle/Throw` - 扔瓶子
- `GET /api/Bottle/Pick` - 捡瓶子
- `GET /api/Bottle/My` - 查看我的瓶子
- `GET /api/Bottle/Received` - 查看收到的瓶子
- `DELETE /api/Bottle/{id}` - 删除瓶子

### 树洞相关
- `POST /api/TreeHole/Post` - 发树洞
- `GET /api/TreeHole/Random` - 随机获取树洞
- `GET /api/TreeHole/My` - 查看我的树洞
- `DELETE /api/TreeHole/{id}` - 删除树洞

### 匹配相关
- `POST /api/Match/Start` - 开始匹配
- `GET /api/Match/Status` - 获取匹配状态
- `POST /api/Match/Cancel` - 取消匹配
- `POST /api/Match/End/{id}` - 结束匹配

### 消息相关
- `POST /api/Message/Send` - 发送消息
- `GET /api/Message/Get/{otherUserId}` - 获取聊天记录
- `GET /api/Message/UnreadCount` - 获取未读消息数
- `DELETE /api/Message/{id}` - 删除消息

### 举报相关
- `POST /api/Report/Submit` - 提交举报
- `GET /api/Report/My` - 查看我的举报

## 数据库设计

### 核心表结构

#### Users (用户表)
- Id (主键)
- AnonUID (匿名UID)
- Nickname (昵称)
- Avatar (头像颜色)
- Phone (手机号，快捷登录时使用)
- LoginType (登录类型：anonymous/quick)
- LastLoginTime (最后登录时间)
- CreatedAt (创建时间)
- UpdatedAt (更新时间)

#### Bottles (漂流瓶表)
- Id (主键)
- UserId (用户ID)
- Content (内容)
- VoiceUrl (语音URL)
- ImageUrl (图片URL)
- Emotion (情绪)
- Topics (主题，JSON格式)
- Status (状态：floating/received/expired)
- CreatedAt (创建时间)
- ExpiredAt (过期时间)

#### TreeHoles (树洞表)
- Id (主键)
- UserId (用户ID)
- Content (内容)
- VoiceUrl (语音URL)
- Status (状态：active/expired)
- CreatedAt (创建时间)
- ExpiredAt (过期时间)

#### Matches (匹配表)
- Id (主键)
- UserId1 (用户1 ID)
- UserId2 (用户2 ID)
- Status (状态：pending/matched/expired)
- CreatedAt (创建时间)
- ExpiredAt (过期时间)

#### Messages (消息表)
- Id (主键)
- FromUserId (发送者ID)
- ToUserId (接收者ID)
- Content (内容)
- Type (类型：text/voice/image)
- IsRead (是否已读)
- CreatedAt (创建时间)

#### Reports (举报表)
- Id (主键)
- ReporterId (举报者ID)
- ReportedUserId (被举报者ID)
- ReportType (举报类型：spam/inappropriate/harassment)
- Content (举报内容)
- Status (状态：pending/processed/dismissed)
- CreatedAt (创建时间)

## 部署说明

1. **创建数据库**
   - 执行 `database.sql` 脚本创建数据库和表结构

2. **配置连接字符串**
   - 修改 `appsettings.json` 中的 `ConnectionStrings:MySqlConnection` 配置

3. **构建项目**
   - 运行 `dotnet build` 构建项目

4. **启动服务**
   - 运行 `dotnet run` 启动服务

5. **环境变量**
   - 可通过环境变量覆盖配置，如 `VITE_API_BASE_URL` 用于配置API基础URL

## 安全说明

- JWT密钥需要在生产环境中更换为安全的密钥
- 密码存储建议使用加密算法
- 敏感信息应使用环境变量配置
- 生产环境应启用HTTPS

## 注意事项

- 语音和图片上传功能需要配置文件存储路径
- 匹配功能的实现为简化版，实际生产环境需要更复杂的匹配算法
- 漂流瓶和树洞的过期时间可通过 `appsettings.json` 中的 `AppSettings` 配置

## 前端对接

前端可通过 `http://localhost:5000/api` 访问API接口，需要在请求头中携带 `Authorization: Bearer {token}` 进行认证。
