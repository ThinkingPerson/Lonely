import axios from 'axios'
import router from '../router'
import { message } from 'ant-design-vue'

// 配置后端接口域名
//const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://127.0.0.1:8081'
const API_BASE_URL = ''

// 创建 axios 实例
const http = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000, // 请求超时时间
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截器
http.interceptors.request.use(
  config => {
    if (config.url) {
      // 1. 不是以 http 开头（避免外部接口）
      const isExternal = /^https?:\/\//i.test(config.url)

      // 2. 没有 /api 前缀
      const hasApiPrefix = config.url.startsWith('/api')

      if (!isExternal && !hasApiPrefix) {
        config.url = '/api' + (config.url.startsWith('/') ? '' : '/') + config.url
      }
    }
    
    // 检测是否为FormData类型，如果是则移除默认的Content-Type
    if (config.data instanceof FormData) {
      delete config.headers['Content-Type']
    }
    
    // 从本地存储获取 token（如果有）
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => {
    return Promise.reject(error)
  }
)

// 响应拦截器
http.interceptors.response.use(
  response => {
    // 只返回响应数据
    return response.data
  },
  error => {
    // 统一处理错误
    if (error.response) {
      switch (error.response.status) {
        case 401:
          // 未授权，显示警告提示并跳转到登录页
          console.error('未授权，请重新登录')
          message.error('未授权，请重新登录')
          // 清除本地存储的登录信息
          localStorage.removeItem('token')
          localStorage.removeItem('nickname')
          localStorage.removeItem('avatar')
          localStorage.removeItem('loginType')
          localStorage.removeItem('anon_uid')
          // 跳转到登录页
          router.push('/')
          break
        case 404:
          console.error('请求的资源不存在')
          message.error('请求的资源不存在')
          break
        case 500:
          console.error('服务器内部错误')
          message.error('服务器内部错误，请稍后重试')
          break
        default:
          const errorMessage = error.response.data.message || '未知错误'
          console.error('请求失败:', errorMessage)
          message.error(`请求失败: ${errorMessage}`)
      }
    } else if (error.request) {
      // 请求已发出但没有收到响应
      console.error('网络错误，请检查网络连接')
      message.error('网络错误，请检查网络连接')
    } else {
      // 请求配置出错
      console.error('请求配置错误:', error.message)
      message.error(`请求配置错误: ${error.message}`)
    }
    return Promise.reject(error)
  }
)

// 封装常用请求方法
const request = {
  // GET 请求
  get(url, params = {}) {
    return http.get(url, { params })
  },
  
  // POST 请求
  post(url, data = {}) {
    return http.post(url, data)
  },
  
  // PUT 请求
  put(url, data = {}) {
    return http.put(url, data)
  },
  
  // DELETE 请求
  delete(url, params = {}) {
    return http.delete(url, { params })
  },
  
  // PATCH 请求
  patch(url, data = {}) {
    return http.patch(url, data)
  }
}

export default request
export { API_BASE_URL }