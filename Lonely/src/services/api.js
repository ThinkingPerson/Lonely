import request from '../utils/http'

// 动态相关API
export const postApi = {
  // 创建动态
  create: (data) => {
    return request.post('/Post/Create', data)
  },
  
  // 获取动态列表
  get: () => {
    return request.get('/Post/Get')
  },
  
  // 点赞动态
  like: (postId) => {
    return request.post(`/Post/Like/${postId}`)
  },
  
  // 评论动态
  comment: (data) => {
    return request.post('/Post/Comment', data)
  },
  
  // 获取动态评论
  getComments: (postId) => {
    return request.get(`/Post/Comments/${postId}`)
  }
}

// 签到相关API
export const checkInApi = {
  // 签到
  checkIn: (data) => {
    return request.post('/CheckIn/CheckIn', data)
  },
  
  // 获取签到状态
  getStatus: () => {
    return request.get('/CheckIn/Status')
  },
  
  // 获取签到历史
  getHistory: (days = 30) => {
    return request.get('/CheckIn/History', { days })
  }
}

// 统计相关API
export const statsApi = {
  // 记录网站访问
  recordVisit: () => {
    return request.post('/Stats/RecordVisit')
  },
  
  // 记录扔瓶子
  recordBottle: () => {
    return request.post('/Stats/RecordBottle')
  },
  
  // 记录树洞
  recordTreeHole: () => {
    return request.post('/Stats/RecordTreeHole')
  },
  
  // 记录动态
  recordPost: () => {
    return request.post('/Stats/RecordPost')
  },
  
  // 获取每日统计数据
  getDailyStats: (days = 30) => {
    return request.get('/Stats/Daily', { days })
  },
  
  // 获取每周统计数据
  getWeeklyStats: (weeks = 12) => {
    return request.get('/Stats/Weekly', { weeks })
  },
  
  // 获取每月统计数据
  getMonthlyStats: (months = 6) => {
    return request.get('/Stats/Monthly', { months })
  },
  
  // 获取统计趋势数据
  getTrendStats: (days = 30) => {
    return request.get('/Stats/Trend', { days })
  }
}

export default {
  postApi,
  checkInApi,
  statsApi
}