<template>
  <div class="entry-container">
    <!-- 背景图片 -->
    <div class="entry-background" :style="{ backgroundImage: `url(${backgroundImage})` }">
      <!-- 半透明叠加层，降低亮度 -->
      <div class="background-overlay"></div>
    </div>
    
    <div class="content-wrapper">
      <div class="logo">Lonely</div>
      <div class="slogan">匿名·轻量·低压力</div>
      
      <div class="buttons">
        <button class="btn btn-primary" @click="directEntry">直接进入</button>
        <button class="btn btn-secondary" @click="showQuickLoginModal">快捷登录</button>
      </div>
      
      <div class="tips text-sm">
        <p>直接进入：系统随机生成昵称与虚拟头像</p>
        <p>快捷登录：使用手机号登录，仅用于风控</p>
      </div>
    </div>
    
    <!-- 快捷登录弹窗 -->
    <div v-if="showQuickLogin" class="modal-overlay" @click="closeQuickLoginModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>快捷登录</h3>
          <button class="close-btn" @click="closeQuickLoginModal">×</button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label for="phone">手机号</label>
            <input 
              type="tel" 
              id="phone" 
              v-model="phone" 
              placeholder="请输入手机号"
              class="form-input"
            />
            <div class="form-error" v-if="phoneError">{{ phoneError }}</div>
          </div>
          <div class="form-group">
            <label for="password">密码</label>
            <div class="password-input-container">
              <input 
                :type="showPassword ? 'text' : 'password'" 
                id="password" 
                v-model="password" 
                placeholder="请输入密码"
                class="form-input password-input"
              />
              <button 
                type="button" 
                class="password-toggle" 
                @click="togglePasswordVisibility"
              >
                <EyeOutlined v-if="showPassword" />
                <EyeInvisibleOutlined v-else />
              </button>
            </div>
          </div>
          <div class="form-group checkbox-group">
            <input type="checkbox" id="agree" v-model="agreeTerms">
            <label for="agree">我同意系统中没有的账号（手机号）将自动注册</label>
            <div class="form-error" v-if="agreeError">{{ agreeError }}</div>
          </div>
          <button class="btn btn-primary login-btn" @click="handleQuickLogin">登录</button>
          <button class="btn btn-secondary get-phone-btn" @click="oneClickLogin">手机号一键登录</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { EyeOutlined, EyeInvisibleOutlined } from '@ant-design/icons-vue'
import request from '../utils/http'

import bed1 from '../assets/bed1.jpg'
import bed2 from '../assets/bed2.jpg'

const router = useRouter()

// 根据时间判断使用哪个背景图片
const backgroundImage = ref(bed1) // 默认使用白天背景

// 快捷登录弹窗状态
const showQuickLogin = ref(false)
const phone = ref('')
const password = ref('')
const phoneError = ref('')
const agreeTerms = ref(false)
const agreeError = ref('')
const showPassword = ref(false)

const updateBackgroundImage = () => {
  const now = new Date()
  const hour = now.getHours()
  // 6点到18点为白天，使用 bed1.jpg
  // 其他时间为晚上，使用 bed2.jpg
  if (hour >= 6 && hour < 18) {
    backgroundImage.value = bed1
  } else {
    backgroundImage.value = bed2
  }
}

onMounted(() => {
  updateBackgroundImage()
})

// 显示快捷登录弹窗
const showQuickLoginModal = () => {
  showQuickLogin.value = true
  phone.value = ''
  password.value = ''
  phoneError.value = ''
  agreeTerms.value = false
  agreeError.value = ''
  showPassword.value = false
}

// 切换密码可见性
const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value
}

// 关闭快捷登录弹窗
const closeQuickLoginModal = () => {
  showQuickLogin.value = false
}

// 验证手机号
const validatePhone = (phoneNumber) => {
  const phoneRegex = /^1[3-9]\d{9}$/
  return phoneRegex.test(phoneNumber)
}

// 手机号一键登录
const oneClickLogin = async () => {
  // 验证是否同意条款
  if (!agreeTerms.value) {
    agreeError.value = '请同意自动注册条款'
    return
  }
  
  // 在实际应用中，这里可以使用浏览器API或第三方库获取手机号
  // 由于浏览器安全限制，通常无法直接获取手机号
  message.info('获取手机号功能需要在实际设备上运行')
  
  // 模拟获取手机号
  const phoneNumber = '18860423687'
  phone.value = phoneNumber
  
  try {
    const response = await request.post('/Auth/QuickLogin', {
      account: phoneNumber,
      password: phoneNumber.substring(phoneNumber.length - 6) // 使用手机号后6位作为默认密码
    })
    if (response.success) {
      // 存储登录信息
      localStorage.setItem('token', response.token)
      localStorage.setItem('anon_uid', response.user.anonUID)
      localStorage.setItem('nickname', response.user.nickname)
      localStorage.setItem('avatar', response.user.avatar)
      localStorage.setItem('loginType', response.user.loginType)
      
      // 关闭弹窗
      closeQuickLoginModal()
      
      // 检查是否首次进入
      const hasReadInstructions = localStorage.getItem('has_read_instructions')
      
      if (!hasReadInstructions) {
        // 首次进入，跳转到阅读说明
        router.push('/readme')
      } else {
        // 已阅读，直接进入主界面
        router.push('/main')
      }
    } else {
      message.error(response.message || '登录失败，请重试')
    }
  } catch (error) {
    console.error('登录失败:', error)
    message.error('登录失败，请检查网络连接')
  }
}

// 处理快捷登录
const handleQuickLogin = async () => {
  // 验证手机号
  if (!phone.value) {
    phoneError.value = '请输入手机号'
    return
  }
  
  if (!validatePhone(phone.value)) {
    phoneError.value = '请输入正确的手机号'
    return
  }
  
  if (!password.value) {
    message.error('请输入密码')
    return
  }
  
  if (!agreeTerms.value) {
    agreeError.value = '请同意自动注册条款'
    return
  }
  
  try {
    const response = await request.post('/Auth/QuickLogin', {
      account: phone.value,
      password: password.value
    })
    
    if (response.success) {
      // 存储登录信息
      localStorage.setItem('token', response.token)
      localStorage.setItem('anon_uid', response.user.anonUID)
      localStorage.setItem('nickname', response.user.nickname)
      localStorage.setItem('avatar', response.user.avatar)
      localStorage.setItem('loginType', response.user.loginType)
      
      // 关闭弹窗
      closeQuickLoginModal()
      
      // 检查是否首次进入
      const hasReadInstructions = localStorage.getItem('has_read_instructions')
      
      if (!hasReadInstructions) {
        // 首次进入，跳转到阅读说明
        router.push('/readme')
      } else {
        // 已阅读，直接进入主界面
        router.push('/main')
      }
    } else {
      message.error(response.message || '登录失败，请重试')
    }
  } catch (error) {
    console.error('登录失败:', error)
    message.error('登录失败，请检查网络连接')
  }
}

// 生成设备指纹
const generateDeviceFingerprint = () => {
  const deviceInfo = {
    userAgent: navigator.userAgent,
    screenWidth: window.screen.width,
    screenHeight: window.screen.height,
    language: navigator.language,
    timezone: Intl.DateTimeFormat().resolvedOptions().timeZone
  }
  return JSON.stringify(deviceInfo)
}

// 生成随机字符串
const generateRandomString = (length = 32) => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789'
  let result = ''
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length))
  }
  return result
}

// 生成 SHA-256 哈希
const generateSHA256 = async (input) => {
  // 检查 crypto.subtle 是否可用
  if (typeof crypto !== 'undefined' && crypto.subtle && typeof crypto.subtle.digest === 'function') {
    try {
      const encoder = new TextEncoder()
      const data = encoder.encode(input)
      const hashBuffer = await crypto.subtle.digest('SHA-256', data)
      const hashArray = Array.from(new Uint8Array(hashBuffer))
      return hashArray.map(b => b.toString(16).padStart(2, '0')).join('')
    } catch (error) {
      console.error('SHA-256 生成失败，使用降级方案:', error)
    }
  } else {
    console.error('crypto.subtle 不可用，使用降级方案')
  }
  
  // 降级方案：使用简单的哈希函数
  let hash = 0
  for (let i = 0; i < input.length; i++) {
    const char = input.charCodeAt(i)
    hash = ((hash << 5) - hash) + char
    hash = hash & hash
  }
  return Math.abs(hash).toString(16) + generateRandomString(16)
}

// 生成匿名 UID
const generateAnonUID = async () => {
  try {
    const deviceFingerprint = generateDeviceFingerprint()
    const randomString = generateRandomString()
    const anonUID = await generateSHA256(deviceFingerprint + randomString)
    return anonUID
  } catch (error) {
    console.error('生成匿名 UID 失败，使用降级方案:', error)
    // 降级方案：直接使用随机字符串
    return generateRandomString(32)
  }
}

const directEntry = async () => {
  try {
    // 检查是否已有匿名 UID
    let anonUID = localStorage.getItem('anon_uid')
    
    // 如果没有，生成新的
    if (!anonUID) {
      anonUID = await generateAnonUID()
      localStorage.setItem('anon_uid', anonUID)
    }
    
    // 生成随机昵称和头像
    const randomNickname = generateRandomNickname()
    const randomAvatar = generateRandomAvatar()
    
    // 调用后端匿名登录接口
    const response = await request.post('/Auth/AnonymousLogin', {
      AnonUID: anonUID,
      Nickname: randomNickname,
      Avatar: randomAvatar
    })
    
    if (response.success) {
      // 存储到本地存储
      localStorage.setItem('token', response.token)
      localStorage.setItem('anon_uid', response.user.anonUID)
      localStorage.setItem('nickname', response.user.nickname)
      localStorage.setItem('avatar', response.user.avatar)
      localStorage.setItem('loginType', response.user.loginType)
      
      // 检查是否首次进入
      const hasReadInstructions = localStorage.getItem('has_read_instructions')
      
      if (!hasReadInstructions) {
        // 首次进入，跳转到阅读说明
        router.push('/readme')
      } else {
        // 已阅读，直接进入主界面
        router.push('/main')
      }
    } else {
      message.error('登录失败，请重试')
    }
  } catch (error) {
    console.error('登录失败:', error)
    message.error('登录失败，请检查网络连接')
  }
}



const generateRandomNickname = () => {
  const adjectives = ['快乐的', '神秘的', '自由的', '安静的', '活泼的', '孤独的', '温暖的', '勇敢的']
  const nouns = ['旅行者', '探险家', '守望者', '梦想家', '倾听者', '创造者', '守护者', '思考者']
  const adj = adjectives[Math.floor(Math.random() * adjectives.length)]
  const noun = nouns[Math.floor(Math.random() * nouns.length)]
  const num = Math.floor(Math.random() * 1000)
  return `${adj}${noun}${num}`
}

const generateRandomAvatar = () => {
  // 生成随机头像颜色
  const colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#FFA07A', '#98D8C8', '#F7DC6F', '#BB8FCE', '#85C1E9']
  return colors[Math.floor(Math.random() * colors.length)]
}
</script>

<style scoped>
.entry-container {
  width: 100vw;
  height: 100vh;
  position: relative;
  overflow: hidden;
}

/* 背景图片 */
.entry-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: cover;
  background-position: center;
  z-index: 0;
}

/* 背景叠加层，降低亮度 */
.background-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.4);
  z-index: 1;
}

.content-wrapper {
  position: relative;
  z-index: 2;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px;
}

.logo {
  font-size: 48px;
  font-weight: 700;
  margin-bottom: 16px;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.slogan {
  font-size: 18px;
  color: white;
  margin-bottom: 64px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.buttons {
  width: 100%;
  max-width: 300px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 48px;
}

.buttons .btn {
  width: 100%;
  padding: 16px;
  font-size: 18px;
  border-radius: 12px;
  border: none;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(155, 89, 182, 0.4);
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(155, 89, 182, 0.6);
}

.btn-secondary:hover {
  transform: translateY(-2px);
  background: rgba(255, 255, 255, 0.3);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.4);
}

.tips {
  text-align: center;
  line-height: 1.5;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

/* 快捷登录弹窗样式 */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(5px);
  -webkit-overflow-scrolling: touch;
}

.modal-content {
  background-color: white;
  border-radius: 16px;
  padding: 30px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  animation: modalFadeIn 0.3s ease;
  -webkit-tap-highlight-color: transparent;
}

/* 防止输入框自动放大 */
input[type="tel"], input[type="password"] {
  font-size: 16px;
  -webkit-text-size-adjust: 100%;
  -moz-text-size-adjust: 100%;
  -ms-text-size-adjust: 100%;
  text-size-adjust: 100%;
}

@keyframes modalFadeIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.modal-header h3 {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #999;
  transition: color 0.3s ease;
}

.close-btn:hover {
  color: #333;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-size: 14px;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
}

.form-input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.password-input-container {
  position: relative;
  width: 100%;
}

.password-input {
  padding-right: 40px;
}

.password-toggle {
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  font-size: 16px;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: background-color 0.3s ease;
}

.password-toggle:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

.form-input:focus {
  outline: none;
  border-color: #9b59b6;
}

.form-error {
  font-size: 12px;
  color: #ff4d4f;
  margin-top: 4px;
}

.checkbox-group {
  position: relative;
  display: flex;
  align-items: center;
  gap: 8px;
  margin: 12px 0;
  flex-wrap: wrap;
}

.checkbox-group input[type="checkbox"] {
  margin: 0;
  transform: scale(1.2);
  flex-shrink: 0;
}

.checkbox-group label {
  font-size: 12px;
  color: #666;
  margin: 0;
  cursor: pointer;
  flex: 1;
  min-width: 0;
}

.checkbox-group .form-error {
  width: 100%;
  margin-top: 4px;
  text-align: left;
  padding-left: 24px;
}

.login-btn {
  width: 100%;
  padding: 14px;
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 12px;
}

.get-phone-btn {
  width: 100%;
  padding: 14px;
  font-size: 16px;
  background-color: #f0f0f0;
  color: #333;
  border: 1px solid #ddd;
}

.get-phone-btn:hover {
  background-color: #e0e0e0;
  transform: translateY(-2px);
}
</style>