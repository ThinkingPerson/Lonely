import request from '../utils/http'

// 上传图片
export const uploadImage = async (file) => {
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('type', 'image')
    
    // 移除手动设置的Content-Type，让axios自动处理
    const response = await request.post('/Upload/Image', formData)
    
    return response
  } catch (error) {
    console.error('上传图片失败:', error)
    throw error
  }
}

// 上传语音
export const uploadVoice = async (file) => {
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('type', 'voice')
    
    // 移除手动设置的Content-Type，让axios自动处理
    const response = await request.post('/Upload/Voice', formData)
    
    return response
  } catch (error) {
    console.error('上传语音失败:', error)
    throw error
  }
}