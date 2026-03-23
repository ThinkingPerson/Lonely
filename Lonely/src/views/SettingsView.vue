<template>
  <div class="settings-container">
    <div class="header">
      <button class="back-btn" @click="goBack">
        <LeftOutlined style="font-size: 20px;" />
      </button>
      <h1>设置</h1>
    </div>
    
    <div class="content">
      <!-- 用户信息 -->
      <div class="user-info">
        <div class="avatar" :style="{ backgroundColor: avatar }">{{ nickname.charAt(0) }}</div>
        <div class="user-details">
          <h2>{{ nickname }}</h2>
          <p class="text-sm text-gray">随机生成，不可编辑</p>
        </div>
      </div>
      
      <!-- 设置项 -->
      <div class="settings-list">
        <div class="settings-item" @click="showPrivacy">
          <span>隐私说明</span>
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M9 5l7 7-7 7"></path>
          </svg>
        </div>
        
        <div class="settings-item" @click="showReport">
          <span>举报入口</span>
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M9 5l7 7-7 7"></path>
          </svg>
        </div>
        
        <div class="settings-item" @click="showAbout">
          <span>关于我们</span>
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M9 5l7 7-7 7"></path>
          </svg>
        </div>
        
        <div class="settings-item version">
          <span>版本号</span>
          <span class="text-gray">v1.0.0</span>
        </div>
      </div>
      
      <!-- 退出登录按钮 -->
      <button class="btn btn-secondary logout-btn" @click="logout">
        退出登录
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { LeftOutlined } from '@ant-design/icons-vue'

const router = useRouter()

const nickname = ref('')
const avatar = ref('#007aff')

onMounted(() => {
  // 从本地存储获取用户信息
  const storedNickname = localStorage.getItem('nickname')
  const storedAvatar = localStorage.getItem('avatar')
  
  if (storedNickname) {
    nickname.value = storedNickname
  } else {
    nickname.value = '神秘用户'
  }
  
  if (storedAvatar) {
    avatar.value = storedAvatar
  }
})

const goBack = () => {
  router.push('/main')
}

const showPrivacy = () => {
  message.info('隐私说明：\n1. 匿名机制：所有内容均匿名发布\n2. 数据不保存：聊天记录不存储到服务器\n3. 最小化存储：仅存储必要信息用于风控\n4. 隐私优先：不采集真实身份信息')
}

const showReport = () => {
  message.info('举报功能已模拟，您可以举报违规内容')
}

const showAbout = () => {
  message.info('关于我们：\nLonely - 匿名·轻量·低压力的社交App\n致力于为用户提供一个安全、自由的社交空间')
}

const logout = () => {
  if (confirm('确定要退出登录吗？')) {
    // 清除本地存储
    localStorage.clear()
    // 跳转到进入页
    router.push('/entry')
  }
}
</script>

<style scoped>
.settings-container {
  width: 100vw;
  min-height: 100vh;
  padding: 20px;
  background-color: #f5f5f5;
}

.header {
  display: flex;
  align-items: center;
  margin-bottom: 32px;
}

.back-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #333;
  padding: 8px;
  margin-right: 16px;
}

.header h1 {
  font-size: 24px;
  font-weight: 600;
}

.content {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid #eee;
}

.user-details h2 {
  font-size: 20px;
  font-weight: 600;
  margin-bottom: 4px;
}

.settings-list {
  margin-bottom: 40px;
}

.settings-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 0;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  transition: all 0.3s ease;
}

.settings-item:hover {
  color: #007aff;
}

.settings-item:last-child {
  border-bottom: none;
}

.version {
  cursor: default;
}

.version:hover {
  color: #333;
}

.logout-btn {
  width: 100%;
  padding: 16px;
  font-size: 18px;
}
</style>