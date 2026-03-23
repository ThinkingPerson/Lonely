using SqlSugar;
using LonelyApi.Models;
using LonelyApi.DTOs;

namespace LonelyApi.Services;

public class StatsService : IService
{
    private readonly ISqlSugarClient _db;

    public StatsService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 记录网站访问
    /// </summary>
    public async Task RecordVisit()
    {
        await RecordStat("VisitCount");
    }

    /// <summary>
    /// 记录扔瓶子
    /// </summary>
    public async Task RecordBottle()
    {
        await RecordStat("BottleCount");
    }

    /// <summary>
    /// 记录树洞
    /// </summary>
    public async Task RecordTreeHole()
    {
        await RecordStat("TreeHoleCount");
    }

    /// <summary>
    /// 记录动态
    /// </summary>
    public async Task RecordPost()
    {
        await RecordStat("PostCount");
    }

    /// <summary>
    /// 记录统计数据
    /// </summary>
    private async Task RecordStat(string fieldName)
    {
        var today = DateTime.Today;
        
        // 查找今天的统计记录
        var stat = await _db.Queryable<Stats>()
            .Where(s => s.Date == today)
            .FirstAsync();

        if (stat == null)
        {
            // 不存在则创建新记录
            stat = new Stats
            {
                Date = today,
                VisitCount = 0,
                BottleCount = 0,
                TreeHoleCount = 0,
                PostCount = 0
            };
            await _db.Insertable(stat).ExecuteCommandAsync();
        }

        // 更新对应字段
        switch (fieldName)
        {
            case "VisitCount":
                await _db.Updateable<Stats>()
                    .SetColumns(it => it.VisitCount == it.VisitCount + 1)
                    .Where(s => s.Id == stat.Id)
                    .ExecuteCommandAsync();
                break;
            case "BottleCount":
                await _db.Updateable<Stats>()
                    .SetColumns(it => it.BottleCount == it.BottleCount + 1)
                    .Where(s => s.Id == stat.Id)
                    .ExecuteCommandAsync();
                break;
            case "TreeHoleCount":
                await _db.Updateable<Stats>()
                    .SetColumns(it => it.TreeHoleCount == it.TreeHoleCount + 1)
                    .Where(s => s.Id == stat.Id)
                    .ExecuteCommandAsync();
                break;
            case "PostCount":
                await _db.Updateable<Stats>()
                    .SetColumns(it => it.PostCount == it.PostCount + 1)
                    .Where(s => s.Id == stat.Id)
                    .ExecuteCommandAsync();
                break;
        }
    }

    /// <summary>
    /// 获取每日统计数据
    /// </summary>
    public async Task<ApiResponse<List<Stats>>> GetDailyStats(int days = 30)
    {
        var startDate = DateTime.Today.AddDays(-days + 1);
        
        var stats = await _db.Queryable<Stats>()
            .Where(s => s.Date >= startDate)
            .OrderBy(s => s.Date)
            .ToListAsync();

        // 填充缺失的日期
        var result = new List<Stats>();
        for (var date = startDate; date <= DateTime.Today; date = date.AddDays(1))
        {
            var existingStat = stats.FirstOrDefault(s => s.Date == date);
            if (existingStat != null)
            {
                result.Add(existingStat);
            }
            else
            {
                result.Add(new Stats
                {
                    Date = date,
                    VisitCount = 0,
                    BottleCount = 0,
                    TreeHoleCount = 0,
                    PostCount = 0
                });
            }
        }

        return new ApiResponse<List<Stats>>(true, "获取成功", result);
    }

    /// <summary>
    /// 获取每周统计数据
    /// </summary>
    public async Task<ApiResponse<List<object>>> GetWeeklyStats(int weeks = 12)
    {
        var startDate = DateTime.Today.AddDays(-weeks * 7);
        
        var stats = await _db.Queryable<Stats>()
            .Where(s => s.Date >= startDate)
            .ToListAsync();

        // 按周分组
        var weeklyData = stats
            .GroupBy(s => new { Year = s.Date.Year, Week = GetWeekNumber(s.Date) })
            .Select(g => new
            {
                week = $"{g.Key.Year}-W{g.Key.Week}",
                visitCount = g.Sum(s => s.VisitCount),
                bottleCount = g.Sum(s => s.BottleCount),
                treeHoleCount = g.Sum(s => s.TreeHoleCount),
                postCount = g.Sum(s => s.PostCount)
            })
            .OrderBy(d => d.week)
            .Cast<object>()
            .ToList();

        return new ApiResponse<List<object>>(true, "获取成功", weeklyData);
    }

    /// <summary>
    /// 获取每月统计数据
    /// </summary>
    public async Task<ApiResponse<List<object>>> GetMonthlyStats(int months = 6)
    {
        var startDate = DateTime.Today.AddMonths(-months + 1);
        startDate = new DateTime(startDate.Year, startDate.Month, 1);
        
        var stats = await _db.Queryable<Stats>()
            .Where(s => s.Date >= startDate)
            .ToListAsync();

        // 按月分组
        var monthlyData = stats
            .GroupBy(s => new { s.Date.Year, s.Date.Month })
            .Select(g => new
            {
                month = $"{g.Key.Year}-{g.Key.Month:00}",
                visitCount = g.Sum(s => s.VisitCount),
                bottleCount = g.Sum(s => s.BottleCount),
                treeHoleCount = g.Sum(s => s.TreeHoleCount),
                postCount = g.Sum(s => s.PostCount)
            })
            .OrderBy(d => d.month)
            .Cast<object>()
            .ToList();

        return new ApiResponse<List<object>>(true, "获取成功", monthlyData);
    }

    /// <summary>
    /// 获取统计趋势数据
    /// </summary>
    public async Task<ApiResponse<object>> GetStatsTrend(int days = 30)
    {
        var dailyStats = await GetDailyStats(days);
        
        var trendData = new
        {
            dates = dailyStats.Data.Select(s => s.Date.ToString("yyyy-MM-dd")).ToList(),
            visitData = dailyStats.Data.Select(s => s.VisitCount).ToList(),
            bottleData = dailyStats.Data.Select(s => s.BottleCount).ToList(),
            treeHoleData = dailyStats.Data.Select(s => s.TreeHoleCount).ToList(),
            postData = dailyStats.Data.Select(s => s.PostCount).ToList()
        };

        return new ApiResponse<object>(true, "获取成功", trendData);
    }

    /// <summary>
    /// 获取周数
    /// </summary>
    private int GetWeekNumber(DateTime date)
    {
        var firstDayOfYear = new DateTime(date.Year, 1, 1);
        var daysSinceFirstDay = (date - firstDayOfYear).Days;
        return (daysSinceFirstDay / 7) + 1;
    }
}