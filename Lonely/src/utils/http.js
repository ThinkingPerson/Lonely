import axios from 'axios'

// 配置后端接口域名
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://acems.septnet.cn/api'

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
          // 未授权，可跳转到登录页
          console.error('未授权，请重新登录')
          break
        case 404:
          console.error('请求的资源不存在')
          break
        case 500:
          console.error('服务器内部错误')
          break
        default:
          console.error('请求失败:', error.response.data.message || '未知错误')
      }
    } else if (error.request) {
      // 请求已发出但没有收到响应
      console.error('网络错误，请检查网络连接')
    } else {
      // 请求配置出错
      console.error('请求配置错误:', error.message)
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