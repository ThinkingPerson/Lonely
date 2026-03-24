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
  
  // 获取统计趋势数据
  getTrendStats: (days = 30) => {
    return request.get('/Stats/Trend', { days })
  },
  
  // 获取用户统计趋势数据
  getUserTrendStats: (days = 30) => {
    return request.get('/Stats/UserTrend', { days })
  },
  
  // 获取所有用户统计数据
  getAllUsersStats: (days = 7) => {
    return request.get('/Stats/AllUsers', { days })
  },
  
  // 获取每日详细数据
  getDailyDetails: (date = null) => {
    return request.get('/Stats/DailyDetails', { date })
  }
}

// 用户相关API
export const userApi = {
  // 获取用户个人资料
  getProfile: () => {
    return request.get('/User/GetProfile')
  }
}

export default {
  postApi,
  checkInApi,
  statsApi,
  userApi
}