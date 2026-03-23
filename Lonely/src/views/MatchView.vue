<template>
  <div class="match-container">
    <div class="header">
      <button class="back-btn" @click="goBack">
        <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <path d="M19 12H5M12 19l-7-7 7-7"></path>
        </svg>
      </button>
      <h1>随机匹配聊天</h1>
    </div>
    
    <div class="content" v-if="!isMatching">
      <!-- 兴趣标签 -->
      <div class="section">
        <h3>选择兴趣标签</h3>
        <div class="tags">
          <span 
            v-for="interest in interests" 
            :key="interest"
            class="tag" 
            :class="selectedInterests.includes(interest) ? 'tag-active' : 'tag-inactive'"
            @click="toggleInterest(interest)"
          >
            {{ interest }}
          </span>
        </div>
      </div>
      
      <!-- 开始匹配按钮 -->
      <button class="btn btn-primary match-btn" @click="startMatching">
        开始匹配
      </button>
    </div>
    
    <!-- 匹配中状态 -->
    <div class="matching-content" v-else>
      <div class="matching-animation">
        <div class="circle"></div>
        <div class="circle"></div>
        <div class="circle"></div>
      </div>
      <p class="matching-text">系统正在为你寻找聊友…</p>
      <button class="btn btn-secondary cancel-btn" @click="cancelMatching">
        取消匹配
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const selectedInterests = ref([])
const isMatching = ref(false)

const interests = ['游戏', '电影', '音乐', '旅行', '美食', '读书', '运动', '科技', '艺术', '情感']

const goBack = () => {
  router.push('/main')
}

const toggleInterest = (interest) => {
  const index = selectedInterests.value.indexOf(interest)
  if (index > -1) {
    selectedInterests.value.splice(index, 1)
  } else {
    selectedInterests.value.push(interest)
  }
}

const startMatching = () => {
  isMatching.value = true
  
  // 模拟匹配过程
  setTimeout(() => {
    // 匹配成功，进入聊天页面
    router.push('/chat')
  }, 3000)
}

const cancelMatching = () => {
  isMatching.value = false
}
</script>

<style scoped>
.match-container {
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

.section {
  margin-bottom: 32px;
}

.section h3 {
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 16px;
}

.tags {
  flex-wrap: wrap;
}

.match-btn {
  width: 100%;
  padding: 16px;
  font-size: 18px;
}

.matching-content {
  background-color: white;
  border-radius: 12px;
  padding: 64px 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 32px;
}

.matching-animation {
  display: flex;
  gap: 12px;
}

.circle {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background-color: #007aff;
  animation: pulse 1.5s infinite ease-in-out;
}

.circle:nth-child(2) {
  animation-delay: 0.2s;
}

.circle:nth-child(3) {
  animation-delay: 0.4s;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(0.8);
    opacity: 0.5;
  }
  50% {
    transform: scale(1.2);
    opacity: 1;
  }
}

.matching-text {
  font-size: 18px;
  color: #666;
}

.cancel-btn {
  padding: 12px 32px;
  margin-top: 16px;
}
</style>