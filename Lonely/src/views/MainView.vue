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
        <div class="user-info">
          <div class="avatar" :style="{ backgroundColor: avatar }">{{ nickname.charAt(0) }}</div>
          <span class="nickname">{{ nickname }}</span>
        </div>
        <button class="settings-btn" @click="goToSettings">
          <SettingOutlined />
        </button>
      </div>
      
      <!-- 快捷登录提示 -->
    <div v-if="!hasLoggedIn" class="login-tip">
      <div class="tip-content">
        <p>快捷登录可增强身份保护，在举报或纠纷中提供更完整的身份依据，防止被冒用。</p>
        <button class="tip-btn" @click="showLoginPrompt">了解更多</button>
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
    </div>
    
    <!-- 底部链接 -->
    <div class="bottom-links">
      <span class="link" @click="goToReadme">阅读说明</span>
      <span class="link-divider">·</span>
      <span class="link" @click="goToDisclaimer">免责声明</span>
    </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { SettingOutlined, MessageOutlined, TeamOutlined, DeleteOutlined } from '@ant-design/icons-vue'

const router = useRouter()
const nickname = ref('')
const avatar = ref('#007aff')
const hasLoggedIn = ref(false)

onMounted(() => {
  // 从本地存储获取用户信息
  const storedNickname = localStorage.getItem('nickname')
  const storedAvatar = localStorage.getItem('avatar')
  const loginType = localStorage.getItem('loginType')
  
  if (storedNickname) {
    nickname.value = storedNickname
  } else {
    // 生成默认昵称
    nickname.value = '神秘用户'
  }
  
  if (storedAvatar) {
    avatar.value = storedAvatar
  }
  
  // 检查是否已登录
  hasLoggedIn.value = !!loginType
})

const showLoginPrompt = () => {
  // 这里可以实现登录提示逻辑
  // 暂时使用 alert 模拟
  alert('快捷登录功能正在开发中，敬请期待！')
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
}

.nickname {
  font-size: 18px;
  font-weight: 600;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
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
  margin-top: auto;
  margin-bottom: 32px;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 16px;
  padding: 0 20px;
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
</style>