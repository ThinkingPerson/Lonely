import { message } from 'ant-design-vue'

// 封装消息提示组件
const Toast = {
  // 成功提示
  success: (content, duration = 2) => {
    message.success({
      content: content,
      duration: duration,
      className: 'custom-toast'
    })
  },
  
  // 错误提示
  error: (content, duration = 2) => {
    message.error({
      content: content,
      duration: duration,
      className: 'custom-toast'
    })
  },
  
  // 警告提示
  warning: (content, duration = 2) => {
    message.warning({
      content: content,
      duration: duration,
      className: 'custom-toast'
    })
  },
  
  // 信息提示
  info: (content, duration = 2) => {
    message.info({
      content: content,
      duration: duration,
      className: 'custom-toast'
    })
  }
}

export default Toast