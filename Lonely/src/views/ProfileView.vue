<template>
  <div class="profile-container">
    <PageHeader title="个人中心" />

    
    <div class="profile-content">
      <!-- 头像和昵称 -->
      <div class="avatar-section">
        <div class="avatar-container" @click="openAvatarUpload">
          <div v-if="avatar.startsWith('/uploads')" class="avatar avatar-image">
            <img :src="avatar" alt="头像" />
          </div>
          <div v-else class="avatar" :style="{ backgroundColor: avatar }">{{ nickname.charAt(0) }}</div>
          <div class="avatar-edit">
            <span class="edit-icon">✏️</span>
          </div>
        </div>
        <div class="nickname-container">
          <input 
            type="text" 
            v-model="nickname" 
            class="nickname-input"
            @blur="updateUserProfile"
            placeholder="输入昵称"
          />
        </div>
        <p class="login-type">{{ loginType === 'anonymous' ? '匿名登录' : '账号登录' }}</p>
      </div>
      
      <!-- 绑定账号 -->
      <div class="bind-account-section" v-if="loginType === 'anonymous'">
        <h4>绑定账号</h4>
        <p class="bind-tip">绑定账号后可以保存您的个人信息</p>
        <button class="btn btn-primary" @click="bindAccount">绑定账号</button>
      </div>
      
      <!-- 个性签名 -->
      <div class="signature-section">
        <h4>个性签名</h4>
        <div class="signature-input-container">
          <input 
            type="text" 
            v-model="signature" 
            placeholder="输入个性签名"
            class="signature-input"
            @blur="updateUserProfile"
          />
        </div>
      </div>
      
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-overlay">
        <div class="loading-spinner"></div>
      </div>

    </div>
    
    <!-- 绑定账号模态框 -->
    <Modal
      v-model:open="showBindModal"
      title="绑定账号"
      width="90%"
      :maskClosable="false"
    >
      <Form layout="vertical">
        <Form.Item
          label="手机号"
          :required="true"
        >
          <Input
            v-model:value="phone"
            placeholder="请输入手机号"
            maxlength="11"
            @blur="validatePhone"
          />
        </Form.Item>
        <Form.Item
          label="密码"
          :required="true"
        >
          <Input.Password
            v-model:value="password"
            placeholder="请输入密码"
            :visibility-toggle="true"
          />
        </Form.Item>
        <div class="bind-tip-modal">
          <p>系统中没有的账号（手机号）将自动注册</p>
        </div>
      </Form>
      <template #footer>
        <Button @click="showBindModal = false">取消</Button>
        <Button type="primary" @click="submitBindAccount" :loading="bindLoading">
          绑定
        </Button>
      </template>
    </Modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message, Modal, Form, Input, Button } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
import request from '../utils/http'

const router = useRouter()

const goBack = () => {
  router.push('/main')
}

// 状态管理
const nickname = ref(localStorage.getItem('nickname') || '未知用户')
const avatar = ref(localStorage.getItem('avatar') || '#FF6B6B')
const loginType = ref(localStorage.getItem('loginType') || 'anonymous')
const signature = ref(localStorage.getItem('signature') || '')
const loading = ref(false)
const userId = ref(localStorage.getItem('userId') || '1')

// 绑定账号相关
const showBindModal = ref(false)
const phone = ref('')
const password = ref('')
const bindLoading = ref(false)

// 验证手机号
const validatePhone = (event) => {
  const phoneNumber = event.target.value
  const phoneRegex = /^1[3-9]\d{9}$/
  if (phoneNumber && !phoneRegex.test(phoneNumber)) {
    message.error('请输入正确的手机号')
  }
}

// 打开头像上传
const openAvatarUpload = () => {
  // 创建文件输入框
  const input = document.createElement('input')
  input.type = 'file'
  input.accept = 'image/*'
  input.onchange = handleFileUpload
  input.click()
}

// 处理文件上传
const handleFileUpload = async (event) => {
  const file = event.target.files[0]
  if (file) {
    // 检查文件大小（限制为2MB）
    if (file.size > 2 * 1024 * 1024) {
      message.error('文件大小不能超过2MB')
      return
    }
    
    // 检查文件类型
    if (!file.type.startsWith('image/')) {
      message.error('请上传图片文件')
      return
    }
    
    try {
      loading.value = true
      
      // 上传头像到服务器
      const formData = new FormData()
      formData.append('userId', userId.value)
      formData.append('file', file)
      
      const response = await request.post('/User/UploadAvatar', formData)
      
      if (response.success) {
        // 服务器返回的头像URL
        const avatarUrl = response.data
        avatar.value = avatarUrl
        localStorage.setItem('avatar', avatarUrl)
        message.success('头像上传成功')
        
        // 同时更新用户资料
        await updateUserProfile()
      } else {
        message.error('上传失败：' + response.message)
      }
    } catch (error) {
      console.error('上传头像失败:', error)
      message.error('上传失败，请稍后重试')
    } finally {
      loading.value = false
    }
  }
}

// 更新用户资料
const updateUserProfile = async () => {
  try {
    loading.value = true
    const response = await request.post('/User/UpdateProfile', {
      userId: parseInt(userId.value),
      AnonUID: localStorage.getItem('anon_uid') || '',
      Nickname: nickname.value,
      Avatar: avatar.value,
      Signature: signature.value,
      LoginType: loginType.value
    })
    
    if (response.success) {
      localStorage.setItem('nickname', nickname.value)
      localStorage.setItem('avatar', avatar.value)
      localStorage.setItem('signature', signature.value)
      localStorage.setItem('loginType', loginType.value)
      message.success('资料更新成功')
    } else {
      message.error('更新失败：' + response.message)
    }
  } catch (error) {
    console.error('更新用户资料失败:', error)
    message.error('更新失败，请稍后重试')
  } finally {
    loading.value = false
  }
}

// 获取用户资料
const getUserProfile = async () => {
  try {
    loading.value = true
    const response = await request.get(`/User/GetProfile?userId=${userId.value}`)
    
    if (response.success) {
      const userData = response.data
      nickname.value = userData.nickname || nickname.value
      avatar.value = userData.avatar || avatar.value
      signature.value = userData.signature || signature.value
      loginType.value = userData.loginType || loginType.value
      
      // 更新本地存储
      localStorage.setItem('nickname', nickname.value)
      localStorage.setItem('avatar', avatar.value)
      localStorage.setItem('signature', signature.value)
      localStorage.setItem('loginType', loginType.value)
    } else {
      message.error('获取资料失败：' + response.message)
    }
  } catch (error) {
    console.error('获取用户资料失败:', error)
    message.error('获取资料失败，请稍后重试')
  } finally {
    loading.value = false
  }
}

// 绑定账号
const bindAccount = () => {
  showBindModal.value = true
}

// 提交绑定账号
const submitBindAccount = async () => {
  if (!phone.value || !password.value) {
    message.error('请输入手机号和密码')
    return
  }
  
  // 验证手机号
  const phoneRegex = /^1[3-9]\d{9}$/
  if (!phoneRegex.test(phone.value)) {
    message.error('请输入正确的手机号')
    return
  }
  
  try {
    bindLoading.value = true
    const response = await request.post('/User/BindAccount', {
      userId: parseInt(userId.value),
      phone: phone.value,
      password: password.value
    })
    
    if (response.success) {
      message.success('账号绑定成功')
      loginType.value = 'account'
      localStorage.setItem('loginType', 'account')
      showBindModal.value = false
      phone.value = ''
      password.value = ''
    } else {
      message.error('绑定失败：' + response.message)
    }
  } catch (error) {
    console.error('绑定账号失败:', error)
    message.error('绑定失败，请稍后重试')
  } finally {
    bindLoading.value = false
  }
}

// 初始化
onMounted(() => {
  // 获取用户资料
  getUserProfile()
})
</script>

<style scoped>
.profile-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 20px;
}

.profile-content {
  max-width: 600px;
  margin: 0 auto;
  background-color: white;
  border-radius: 12px;
  padding: 30px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  position: relative;
}

/* 头像和昵称 */
.avatar-section {
  text-align: center;
  margin-bottom: 30px;
}

.avatar-container {
  position: relative;
  display: inline-block;
  cursor: pointer;
  margin-bottom: 15px;
}

.avatar {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 40px;
  font-weight: 700;
  color: white;
  transition: all 0.3s ease;
}

.avatar-image {
  background-color: transparent;
  overflow: hidden;
}

.avatar-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 50%;
}

.avatar-container:hover .avatar {
  transform: scale(1.05);
}

.avatar-edit {
  position: absolute;
  bottom: 0;
  right: 0;
  background-color: rgba(0, 0, 0, 0.6);
  border-radius: 50%;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 14px;
}

.nickname-container {
  margin-bottom: 10px;
}

.nickname-input {
  width: 200px;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 20px;
  font-size: 16px;
  font-weight: 600;
  text-align: center;
  transition: all 0.3s ease;
}

.nickname-input:focus {
  outline: none;
  border-color: #9b59b6;
  box-shadow: 0 0 0 2px rgba(155, 89, 182, 0.2);
}

.login-type {
  font-size: 14px;
  color: #666;
  margin: 0;
}

/* 绑定账号 */
.bind-account-section {
  margin-bottom: 30px;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 8px;
}

.bind-account-section h4 {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 10px 0;
}

.bind-tip {
  font-size: 14px;
  color: #666;
  margin: 0 0 15px 0;
}

/* 个性签名 */
.signature-section {
  margin-bottom: 30px;
}

.signature-section h4 {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 10px 0;
}

.signature-input-container {
  position: relative;
}

.signature-input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.signature-input:focus {
  outline: none;
  border-color: #9b59b6;
  box-shadow: 0 0 0 2px rgba(155, 89, 182, 0.2);
}

/* 加载状态 */
.loading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.8);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #9b59b6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* 绑定账号模态框 */
.bind-tip-modal {
  margin-top: 10px;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 4px;
  font-size: 14px;
  color: #666;
}

/* 按钮样式 */
.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(155, 89, 182, 0.4);
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(155, 89, 182, 0.6);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .profile-container {
    padding: 10px;
  }
  
  .profile-content {
    padding: 20px;
  }
  
  .avatar {
    width: 80px;
    height: 80px;
    font-size: 32px;
  }
  
  .nickname-input {
    width: 160px;
    font-size: 14px;
  }
  
  .check-in-btn {
    padding: 12px;
    font-size: 14px;
  }
}
</style>