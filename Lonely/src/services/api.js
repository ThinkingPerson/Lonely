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

export default {
  postApi,
  checkInApi
}