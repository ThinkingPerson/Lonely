using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;
using Newtonsoft.Json;

namespace LonelyApi.Services;

public class PostService : IService
{
    private readonly ISqlSugarClient _db;

    public PostService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<ApiResponse<Post>> CreatePost(int userId, PostRequest request)
    {
        var post = new Post
        {
            UserId = userId,
            Content = request.Content,
            Images = JsonConvert.SerializeObject(request.Images),
            Audio = request.Audio,
            LikeCount = 0,
            CommentCount = 0
        };

        await _db.Insertable(post).ExecuteCommandAsync();

        return new ApiResponse<Post>(true, "动态发布成功", post);
    }

    public async Task<ApiResponse<object>> GetPosts(int userId, int page = 1, int pageSize = 10)
    {
        // 计算总记录数
        var totalCount = await _db.Queryable<Post>().CountAsync();

        // 分页查询
        var posts = await _db.Queryable<Post>()
            .Includes(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var likePostIds = await _db.Queryable<Like>().Where(i => i.UserId == userId).Select(i => i.PostId).ToListAsync();

        var result = new List<object>();
        foreach (var post in posts)
        {
            var postData = new
            {
                id = post.Id,
                userId = post.UserId,
                username = post.User?.Nickname,
                avatar = post.User?.Avatar,
                content = post.Content,
                images = string.IsNullOrWhiteSpace(post.Images) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(post.Images) ?? new List<string>(),
                audio = post.Audio,
                time = post.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                likes = post.LikeCount,
                isLike = likePostIds.Count(i => i == post.Id) > 0,
                comments = post.CommentCount
            };
            result.Add(postData);
        }

        // 构造分页响应
        var pagination = new
        {
            total = totalCount,
            page = page,
            pageSize = pageSize,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            list = result
        };

        return new ApiResponse<object>(true, "获取动态成功", pagination);
    }

    public async Task<ApiResponse> LikePost(int postId, int userId)
    {
        // 检查是否已经点赞
        var existingLike = await _db.Queryable<Like>()
            .Where(l => l.PostId == postId && l.UserId == userId)
            .FirstAsync();

        if (existingLike != null)
        {
            // 取消点赞
            await _db.Deleteable(existingLike).ExecuteCommandAsync();
            await _db.Updateable<Post>()
                .SetColumns(p => p.LikeCount == p.LikeCount - 1)
                .Where(p => p.Id == postId)
                .ExecuteCommandAsync();
            return new ApiResponse(true, "取消点赞成功");
        }
        else
        {
            // 添加点赞
            var like = new Like
            {
                PostId = postId,
                UserId = userId
            };
            await _db.Insertable(like).ExecuteCommandAsync();
            await _db.Updateable<Post>()
                .SetColumns(p => p.LikeCount == p.LikeCount + 1)
                .Where(p => p.Id == postId)
                .ExecuteCommandAsync();
            return new ApiResponse(true, "点赞成功");
        }
    }

    public async Task<ApiResponse<Comment>> AddComment(int userId, CommentRequest request)
    {
        var comment = new Comment
        {
            PostId = request.PostId,
            UserId = userId,
            Content = request.Content
        };

        await _db.Insertable(comment).ExecuteCommandAsync();

        // 更新评论数
        await _db.Updateable<Post>()
            .SetColumns(p => p.CommentCount == p.CommentCount + 1)
            .Where(p => p.Id == request.PostId)
            .ExecuteCommandAsync();

        return new ApiResponse<Comment>(true, "评论成功", comment);
    }

    public async Task<ApiResponse<object>> GetComments(int postId, int page = 1, int pageSize = 20)
    {
        // 计算总记录数
        var totalCount = await _db.Queryable<Comment>().Where(c => c.PostId == postId).CountAsync();

        // 分页查询
        var comments = await _db.Queryable<Comment>()
            .Includes(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 构造分页响应
        var pagination = new
        {
            total = totalCount,
            page = page,
            pageSize = pageSize,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            list = comments
        };

        return new ApiResponse<object>(true, "获取评论成功", pagination);
    }

    public async Task<ApiResponse> DeletePost(int postId, int userId)
    {
        // 检查动态是否存在且属于当前用户
        var post = await _db.Queryable<Post>()
            .Where(p => p.Id == postId && p.UserId == userId)
            .FirstAsync();

        if (post == null)
        {
            return new ApiResponse(false, "动态不存在或无权限删除");
        }

        // 开始事务
        _db.Ado.BeginTran();
        try
        {
            // 删除相关的点赞
            await _db.Deleteable<Like>().Where(l => l.PostId == postId).ExecuteCommandAsync();

            // 删除相关的评论
            await _db.Deleteable<Comment>().Where(c => c.PostId == postId).ExecuteCommandAsync();

            // 删除动态
            await _db.Deleteable<Post>().Where(p => p.Id == postId).ExecuteCommandAsync();

            // 提交事务
            _db.Ado.CommitTran();

            return new ApiResponse(true, "删除成功");
        }
        catch (Exception ex)
        {
            // 回滚事务
            _db.Ado.RollbackTran();
            return new ApiResponse(false, "删除失败: " + ex.Message);
        }
    }

    public async Task<ApiResponse> DeleteComment(int commentId, int userId)
    {
        // 检查评论是否存在且属于当前用户
        var comment = await _db.Queryable<Comment>()
            .Where(c => c.Id == commentId && c.UserId == userId)
            .FirstAsync();

        if (comment == null)
        {
            return new ApiResponse(false, "评论不存在或无权限删除");
        }

        // 开始事务
        _db.Ado.BeginTran();
        try
        {
            // 记录帖子ID，用于后续更新评论数
            var postId = comment.PostId;

            // 删除评论
            await _db.Deleteable<Comment>().Where(c => c.Id == commentId).ExecuteCommandAsync();

            // 更新帖子评论数
            await _db.Updateable<Post>()
                .SetColumns(p => p.CommentCount == p.CommentCount - 1)
                .Where(p => p.Id == postId)
                .ExecuteCommandAsync();

            // 提交事务
            _db.Ado.CommitTran();

            return new ApiResponse(true, "删除成功");
        }
        catch (Exception ex)
        {
            // 回滚事务
            _db.Ado.RollbackTran();
            return new ApiResponse(false, "删除失败: " + ex.Message);
        }
    }
}