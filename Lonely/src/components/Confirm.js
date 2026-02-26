import { Modal } from 'ant-design-vue'

// 封装确认弹窗组件
const Confirm = {
  // 确认弹窗
  show: (title, content, onOk, onCancel, okText = '确定', cancelText = '取消') => {
    Modal.confirm({
      title: title,
      content: content,
      onOk: onOk,
      onCancel: onCancel,
      okText: okText,
      cancelText: cancelText,
      className: 'custom-confirm'
    })
  },
  
  // 成功弹窗
  success: (title, content, onOk, okText = '确定') => {
    Modal.success({
      title: title,
      content: content,
      onOk: onOk,
      okText: okText,
      className: 'custom-confirm'
    })
  },
  
  // 错误弹窗
  error: (title, content, onOk, okText = '确定') => {
    Modal.error({
      title: title,
      content: content,
      onOk: onOk,
      okText: okText,
      className: 'custom-confirm'
    })
  },
  
  // 警告弹窗
  warning: (title, content, onOk, onCancel, okText = '确定', cancelText = '取消') => {
    Modal.warning({
      title: title,
      content: content,
      onOk: onOk,
      onCancel: onCancel,
      okText: okText,
      cancelText: cancelText,
      className: 'custom-confirm'
    })
  }
}

export default Confirm