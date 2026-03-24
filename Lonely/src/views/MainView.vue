<template>
  <div class="main-container">
    <!-- 背景 -->
    <div class="main-background">
      <!-- 半透明叠加层，降低背景亮度 -->
      <div class="background-overlay"></div>
    </div>
    
    <div class="content-wrapper">
      <!-- 顶部用户信息 -->
      <div class="header">
        <div class="user-info" @click="goToProfile">
          <div class="avatar" :style="{ backgroundColor: avatar }">{{ nickname.charAt(0) }}</div>
          <span class="nickname">{{ nickname }}</span>
        </div>
        <div class="header-buttons">
          <button class="profile-btn" @click="goToProfile">
            <UserOutlined />
          </button>
          <button class="settings-btn" @click="goToSettings">
            <SettingOutlined />
          </button>
        </div>
      </div>
      
      <!-- 快捷登录提示 -->
    <div v-if="!hasLoggedIn" class="login-tip">
      <div class="tip-content">
        <p>快捷登录可增强身份保护，在举报或纠纷中提供更完整的身份依据，防止被冒用。</p>
        <button class="tip-btn" @click="showLoginPrompt">了解更多</button>
      </div>
    </div>
    
    <!-- 签到功能 -->
    <div class="check-in-section" v-if="!isCheckedIn">
      <div class="check-in-card">
        <div class="check-in-icon">
          <span class="icon">
            <SmileOutlined />
          </span>
        </div>
        <h3 class="check-in-title">今日签到</h3>
        <p class="check-in-desc">记录你的每日心情</p>
        <div class="status-options">
          <div 
            v-for="(status, index) in defaultStatuses" 
            :key="index"
            class="status-option"
            @click="selectStatus(status)"
            :class="{ active: selectedStatus === status }"
          >
            {{ status }}
          </div>
          <div class="status-option custom-status-option" @click="showCustomInput = !showCustomInput">
            自定义状态+
          </div>
          <div class="custom-status-input-container" v-if="showCustomInput">
            <input 
              type="text" 
              v-model="customStatus" 
              placeholder="输入自定义状态"
              class="custom-status-input"
              @blur="selectStatus(customStatus); showCustomInput = false"
              @keyup.enter="selectStatus(customStatus); showCustomInput = false"
              ref="customInput"
            />
          </div>
        </div>
        <button 
          class="check-in-btn" 
          @click="checkIn"
          :disabled="!selectedStatus || loading"
        >
          {{ loading ? '签到中...' : '立即签到' }}
        </button>
      </div>
    </div>
    <div class="check-in-section" v-else>
      <div class="checked-in-card">
        <div class="checked-in-icon">✓</div>
        <h3 class="checked-in-title">今日已签到</h3>
        <p class="checked-in-status">{{ todayStatus }}</p>
      </div>
    </div>
    
    <!-- 功能入口 -->
    <div class="functions">
      <button class="function-btn" @click="goToBottle">
        <div class="function-icon bottle-icon">
          <MessageOutlined />
        </div>
        <div class="function-content">
          <span class="function-title">扔一个瓶子</span>
          <span class="function-desc">将心情投入大海，等待有缘人回应</span>
        </div>
      </button>
      
      <button class="function-btn" @click="goToMatch">
        <div class="function-icon match-icon">
          <TeamOutlined />
        </div>
        <div class="function-content">
          <span class="function-title">随机匹配聊天</span>
          <span class="function-desc">基于兴趣标签，结识志同道合的朋友</span>
        </div>
      </button>
      
      <button class="function-btn" @click="goToTreeHole">
        <div class="function-icon treehole-icon">
          <DeleteOutlined />
        </div>
        <div class="function-content">
          <span class="function-title">发树洞回声</span>
          <span class="function-desc">匿名分享心情，释放内心压力</span>
        </div>
      </button>
      
      <button class="function-btn" @click="goToFeed">
        <div class="function-icon feed-icon">
          <FireOutlined />
        </div>
        <div class="function-content">
          <span class="function-title">动态广场</span>
          <span class="function-desc">查看和发布动态，与朋友互动</span>
        </div>
      </button>
    </div>
    
    <!-- 底部链接 -->
    <div class="bottom-links">
      <span class="link" @click="goToReadme">阅读说明</span>
      <span class="link-divider">·</span>
      <span class="link" @click="goToDisclaimer">免责声明</span>
      <span v-if="isAdmin" class="link-divider">·</span>
      <span v-if="isAdmin" class="link" @click="goToStatistics">数据统计</span>
    </div>
  </div>
</div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { SettingOutlined, MessageOutlined, TeamOutlined, DeleteOutlined, UserOutlined, FireOutlined, SmileOutlined } from '@ant-design/icons-vue'
import { checkInApi, statsApi } from '../services/api'

// 注入全局用户状态
const userState = inject('userState')

// 检查是否为管理员账号
const isAdmin = computed(() => {
  return userState.value.userInfo && userState.value.userInfo.phone === '18860423687'
})

const router = useRouter()
const nickname = ref('')
const avatar = ref('#007aff')
const hasLoggedIn = ref(false)

// 签到相关状态
const isCheckedIn = ref(false)
const todayStatus = ref('')
const selectedStatus = ref('')
const customStatus = ref('')
const showCheckInAnimation = ref(false)
const showCustomInput = ref(false)
const customInput = ref(null)
const loading = ref(false)

// 默认状态词
const defaultStatuses = [
  '又活了一天',
  '我还活着',
  '又是美好的一天'
]

onMounted(() => {
  // 记录网站访问
  statsApi.recordVisit().catch(error => {
    console.error('记录访问失败:', error)
  })
  
  // 从全局用户状态获取用户信息
  if (userState.value.isLoggedIn && userState.value.userInfo) {
    const userInfo = userState.value.userInfo
    nickname.value = userInfo.nickname || '神秘用户'
    avatar.value = userInfo.avatar || '#007aff'
    hasLoggedIn.value = true
  } else {
    // 未登录用户
    nickname.value = '神秘用户'
    avatar.value = '#007aff'
    hasLoggedIn.value = false
  }
  
  // 检查今日是否已签到
  checkIfCheckedIn()
})

// 检查今日是否已签到
const checkIfCheckedIn = async () => {
  if (userState.value.isLoggedIn) {
    // 登录用户，调用后端API
    try {
      const response = await checkInApi.getStatus()
      if (response.success) {
        isCheckedIn.value = response.data.isCheckedIn
        todayStatus.value = response.data.status || ''
      }
    } catch (error) {
      console.error('获取签到状态失败:', error)
      // 失败时不更新页面状态，保持原样
    }
  } else {
    // 匿名用户，从本地存储获取
    try {
      const today = new Date().toDateString()
      const lastCheckInDate = localStorage.getItem('lastCheckInDate')
      const lastCheckInStatus = localStorage.getItem('lastCheckInStatus')
      
      if (lastCheckInDate === today) {
        isCheckedIn.value = true
        todayStatus.value = lastCheckInStatus || ''
      } else {
        isCheckedIn.value = false
        todayStatus.value = ''
      }
    } catch (error) {
      console.error('获取本地存储失败:', error)
      isCheckedIn.value = false
      todayStatus.value = ''
    }
  }
}

// 选择状态
const selectStatus = (status) => {
  selectedStatus.value = status
  customStatus.value = status
}

// 签到
const checkIn = async () => {
  if (!selectedStatus.value) return
  
  try {
    loading.value = true
    
    if (userState.value.isLoggedIn) {
      // 登录用户，调用后端API
      const response = await checkInApi.checkIn({
        status: selectedStatus.value
      })
      
      if (response.success) {
        // 更新状态
        isCheckedIn.value = true
        todayStatus.value = selectedStatus.value
        
        // 显示签到动画
        showCheckInAnimation.value = true
        setTimeout(() => {
          showCheckInAnimation.value = false
        }, 2000)
      } else {
        message.error('签到失败：' + response.message)
      }
    } else {
      // 匿名用户，存储在本地
      try {
        const today = new Date().toDateString()
        localStorage.setItem('lastCheckInDate', today)
        localStorage.setItem('lastCheckInStatus', selectedStatus.value)
        
        // 更新状态
        isCheckedIn.value = true
        todayStatus.value = selectedStatus.value
        
        // 显示签到动画
        showCheckInAnimation.value = true
        setTimeout(() => {
          showCheckInAnimation.value = false
        }, 2000)
        
        message.success('签到成功')
      } catch (localError) {
        console.error('存储本地签到信息失败:', localError)
        message.error('签到失败，请稍后重试')
      }
    }
  } catch (error) {
    console.error('签到失败:', error)
    // 失败时不更新页面状态，保持原样
  } finally {
    loading.value = false
  }
}

const showLoginPrompt = () => {
  // 这里可以实现登录提示逻辑
  // 暂时使用 message 模拟
  message.info('快捷登录功能正在开发中，敬请期待！')
}

const goToBottle = () => {
  router.push('/bottle')
}

const goToMatch = () => {
  router.push('/match')
}

const goToTreeHole = () => {
  router.push('/treehole')
}

const goToSettings = () => {
  router.push('/settings')
}

const goToProfile = () => {
  router.push('/profile')
}

const goToFeed = () => {
  router.push('/feed')
}

const goToMessages = () => {
  // 这里可以根据实际情况跳转到消息列表页，或者直接跳转到某个具体的消息页面
  // 暂时先跳转到捡到瓶子的页面作为示例
  router.push('/bottle-receive')
}

const goToReadme = () => {
  router.push('/readme')
}

const goToDisclaimer = () => {
  router.push('/disclaimer')
}

const goToStatistics = () => {
  router.push('/statistics')
}
</script>

<style scoped>
.main-container {
  width: 100vw;
  min-height: 100vh;
  position: relative;
  overflow: hidden;
}

/* 主背景 */
.main-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #1a101c 0%, #0d0613 100%);
  z-index: 0;
}

/* 背景叠加层，降低亮度 */
.background-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.3);
  z-index: 1;
}

.content-wrapper {
  position: relative;
  z-index: 2;
  padding: 20px;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* 快捷登录提示 */
.login-tip {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 24px;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.tip-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.tip-content p {
  color: white;
  font-size: 14px;
  line-height: 1.4;
  margin: 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.tip-btn {
  align-self: flex-start;
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 8px 16px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(155, 89, 182, 0.4);
}

.tip-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(155, 89, 182, 0.6);
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 40px;
  color: #fff;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.user-info:hover {
  transform: translateY(-2px);
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 18px;
  transition: all 0.3s ease;
}

.user-info:hover .avatar {
  transform: scale(1.1);
}

.nickname {
  font-size: 18px;
  font-weight: 600;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.header-buttons {
  display: flex;
  align-items: center;
  gap: 12px;
}

.profile-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: white;
  padding: 8px;
  font-size: 20px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.profile-btn:hover {
  transform: translateY(-2px);
  color: #e0e0e0;
}

.settings-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: white;
  padding: 8px;
  font-size: 20px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.settings-btn:hover {
  transform: translateY(-2px);
  color: #e0e0e0;
}

.functions {
  display: flex;
  flex-direction: column;
  gap: 20px;
  margin-top: 20px;
}

.function-btn {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 16px;
  padding: 32px 24px;
  display: flex;
  align-items: center;
  gap: 24px;
  cursor: pointer;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.3);
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.function-btn:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.5);
  background: rgba(255, 255, 255, 0.15);
}

.function-icon {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
}

.bottle-icon {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  box-shadow: 0 4px 15px rgba(155, 89, 182, 0.4);
}

.match-icon {
  background: linear-gradient(135deg, #3498db 0%, #2980b9 100%);
  box-shadow: 0 4px 15px rgba(52, 152, 219, 0.4);
}

.treehole-icon {
  background: linear-gradient(135deg, #2ecc71 0%, #27ae60 100%);
  box-shadow: 0 4px 15px rgba(46, 204, 113, 0.4);
}

.function-content {
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex: 1;
  text-align: left;
}

.function-title {
  font-size: 18px;
  font-weight: 600;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.function-desc {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.8);
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
  line-height: 1.4;
}

/* 底部链接 */
.bottom-links {
  padding-top: 15px;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  padding: 30px 20px 0;
}

.link {
  color: white;
  font-size: 14px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
  cursor: pointer;
  transition: all 0.3s ease;
}

.link:hover {
  color: #e0e0e0;
  text-decoration: underline;
}

.link-divider {
  color: rgba(255, 255, 255, 0.5);
  font-size: 14px;
}

/* 签到功能 */
.check-in-section {
  margin-bottom: 30px;
}

.check-in-card {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 16px;
  padding: 30px;
  text-align: center;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
  animation: fadeIn 0.5s ease;
}

.check-in-icon {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  background: linear-gradient(135deg, #e91e63 0%, #9c27b0 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 20px;
  box-shadow: 0 6px 20px rgba(233, 30, 99, 0.4);
  animation: pulse 1s ease-in-out;
}

.check-in-icon .icon {
  font-size: 55px;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
}

.check-in-title {
  font-size: 20px;
  font-weight: 600;
  color: white;
  margin: 0 0 10px 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.check-in-desc {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.8);
  margin: 0 0 20px 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.status-options {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 15px;
  justify-content: center;
}

.status-option {
  padding: 8px 16px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 20px;
  font-size: 14px;
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
  background: rgba(255, 255, 255, 0.1);
}

.status-option:hover {
  border-color: #9b59b6;
  color: #9b59b6;
  background: rgba(155, 89, 182, 0.2);
}

.status-option.active {
  background-color: #9b59b6;
  border-color: #9b59b6;
  color: white;
}

.custom-status-option {
  font-weight: 600;
}

.custom-status-input-container {
  margin-top: 10px;
  width: 100%;
}

.custom-status-input {
  width: 100%;
  padding: 12px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 8px;
  font-size: 14px;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  transition: border-color 0.3s ease;
}

.custom-status-input::placeholder {
  color: rgba(255, 255, 255, 0.5);
}

.custom-status-input:focus {
  outline: none;
  border-color: #9b59b6;
}

.check-in-btn {
  width: 100%;
  padding: 18px;
  font-size: 20px;
  font-weight: 800;
  background: linear-gradient(135deg, #ff4081 0%, #7b1fa2 100%);
  color: white;
  border: none;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px rgba(255, 64, 129, 0.5);
  text-transform: uppercase;
  letter-spacing: 2px;
  position: relative;
  overflow: hidden;
}

.check-in-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.6s ease;
}

.check-in-btn:hover:not(:disabled) {
  transform: translateY(-4px);
  box-shadow: 0 12px 30px rgba(255, 64, 129, 0.7);
  background: linear-gradient(135deg, #ff80ab 0%, #9c27b0 100%);
}

.check-in-btn:hover:not(:disabled)::before {
  left: 100%;
}

.check-in-btn:disabled {
  background: rgba(255, 255, 255, 0.3);
  cursor: not-allowed;
  box-shadow: none;
}

.checked-in-card {
  background: rgba(76, 175, 80, 0.2);
  border: 1px solid rgba(76, 175, 80, 0.4);
  border-radius: 16px;
  padding: 30px;
  text-align: center;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
  animation: fadeIn 0.5s ease;
}

.checked-in-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background-color: #4caf50;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 30px;
  font-weight: 700;
  margin: 0 auto 15px;
  animation: pulse 1s ease-in-out;
}

.checked-in-title {
  font-size: 18px;
  font-weight: 600;
  color: white;
  margin: 0 0 10px 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

.checked-in-status {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.8);
  margin: 0;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
}

/* 动态广场图标样式 */
.feed-icon {
  background: linear-gradient(135deg, #f39c12 0%, #e67e22 100%);
  box-shadow: 0 4px 15px rgba(243, 156, 18, 0.4);
}

/* 签到动画 */
.check-in-animation {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: fadeIn 0.3s ease;
}

.animation-content {
  background-color: white;
  border-radius: 12px;
  padding: 40px;
  text-align: center;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  animation: scaleIn 0.3s ease;
}

.check-mark {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background-color: #4caf50;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 40px;
  font-weight: 700;
  margin: 0 auto 20px;
  animation: pulse 1s ease-in-out;
}

.animation-content h3 {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin: 0;
}

/* 动画 */
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes scaleIn {
  from {
    transform: scale(0.8);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes pulse {
  0% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
  100% {
    transform: scale(1);
  }
}
</style>