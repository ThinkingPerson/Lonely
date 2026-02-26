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
        <button class="btn btn-secondary" @click="quickLogin">快捷登录</button>
      </div>
      
      <div class="tips text-sm">
        <p>直接进入：系统随机生成昵称与虚拟头像</p>
        <p>快捷登录：仅用于风控，不展示身份信息</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import request from '../utils/http'

import bed1 from '../assets/bed1.jpg'
import bed2 from '../assets/bed2.jpg'

const router = useRouter()

// 根据时间判断使用哪个背景图片
const backgroundImage = ref(bed1) // 默认使用白天背景

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
  const encoder = new TextEncoder()
  const data = encoder.encode(input)
  const hashBuffer = await crypto.subtle.digest('SHA-256', data)
  const hashArray = Array.from(new Uint8Array(hashBuffer))
  return hashArray.map(b => b.toString(16).padStart(2, '0')).join('')
}

// 生成匿名 UID
const generateAnonUID = async () => {
  const deviceFingerprint = generateDeviceFingerprint()
  const randomString = generateRandomString()
  const anonUID = await generateSHA256(deviceFingerprint + randomString)
  return anonUID
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
      localStorage.setItem('nickname', response.data.Nickname)
      localStorage.setItem('avatar', response.data.Avatar)
      localStorage.setItem('loginType', response.data.LoginType)
      
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
      alert('登录失败，请重试')
    }
  } catch (error) {
    console.error('登录失败:', error)
    alert('登录失败，请检查网络连接')
  }
}

const quickLogin = async () => {
  try {
    const response = await request.post('/Auth/Login', {
      account: '18860423687',
      password: '423687'
    })
    
    if (response.success) {
      // 存储登录信息
      localStorage.setItem('token', response.token)
      localStorage.setItem('anon_uid', response.user.AnonUID)
      localStorage.setItem('nickname', response.user.Nickname)
      localStorage.setItem('avatar', response.user.Avatar)
      localStorage.setItem('loginType', response.user.LoginType)
      
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
      alert('登录失败，请重试')
    }
  } catch (error) {
    console.error('登录失败:', error)
    alert('登录失败，请检查网络连接')
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
</style>