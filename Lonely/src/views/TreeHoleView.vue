<template>
  <div class="treehole-container">
    <!-- 树林背景 -->
    <div class="forest-background">
      <!-- 半透明叠加层，降低背景亮度 -->
      <div class="background-overlay"></div>
    </div>
    
    <div class="content-wrapper">
      <!-- 头部 -->
      <div class="header">
        <button class="back-btn" @click="goBack">
          <LeftOutlined />
        </button>
        <h1 class="page-title">发树洞回声</h1>
      </div>
      
      <!-- 内容区域 - 只在showForm为true时显示 -->
      <div class="content" v-if="showForm">
        <!-- 文本输入 -->
        <textarea 
          class="textarea" 
          v-model="content.text"
          placeholder="写下你的心声..."
          maxlength="200"
          rows="6"
        ></textarea>
        <div class="text-count text-sm text-gray">{{ content.text.length }}/200</div>
        
        <!-- 多媒体按钮 -->
        <div class="media-buttons">
          <VoiceUpload v-model="content.voice" />
          <ImageUpload v-model="content.image" />
        </div>
        <p class="text-sm text-gray">语音最长 60 秒</p>
        
        <!-- 发出树洞按钮 -->
        <div class="send-container">
          <button class="btn btn-primary send-btn" @click="sendTreeHole">
            发出树洞
          </button>
          <div class="treehole-animation" v-if="showTreeHoleAnimation">
            <svg class="treehole-svg" viewBox="0 0 24 24" fill="white" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M18 8h1a4 4 0 0 1 0 8h-1"></path>
              <path d="M2 8h16v9a4 4 0 0 1-4 4H6a4 4 0 0 1-4-4V8z"></path>
              <line x1="6" y1="1" x2="6" y2="4"></line>
              <line x1="10" y1="1" x2="10" y2="4"></line>
              <line x1="14" y1="1" x2="14" y2="4"></line>
            </svg>
          </div>
        </div>
      </div>
      
      <!-- 底部按钮 -->
      <div class="bottom-buttons">
        <button class="btn btn-primary bottom-btn" @click="toggleForm">
          {{ showForm ? '取消' : '发树洞' }}
        </button>
        <button class="btn btn-secondary bottom-btn" @click="goToMessages">
          消息
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { LeftOutlined } from '@ant-design/icons-vue'
import request from '../utils/http'
import ImageUpload from '../components/ImageUpload.vue'
import VoiceUpload from '../components/VoiceUpload.vue'

const router = useRouter()

const content = ref({
  text: '',
  voice: '',
  image: ''
})

const showTreeHoleAnimation = ref(false)
const showForm = ref(false)

const goBack = () => {
  router.push('/main')
}

const toggleForm = () => {
  showForm.value = !showForm.value
  if (showForm.value) {
    // 重置表单
    content.value = {
      text: '',
      voice: '',
      image: ''
    }
  }
}

const goToMessages = () => {
  router.push('/treehole-receive')
}

const sendTreeHole = async () => {
  // 校验至少有一种内容
  if (!content.value.text && !content.value.voice && !content.value.image) {
    message.error('请至少填写一种内容')
    return
  }
  
  try {
    // 显示树洞动画
    showTreeHoleAnimation.value = true
    
    // 调用后端API发送树洞
    const response = await request.post('/TreeHole/Post', {
      Content: content.value.text,
      VoiceUrl: content.value.voice,
      ImageUrl: content.value.image
    })
    
    if (response.success) {
      // 显示成功消息
      setTimeout(() => {
        message.success('树洞已发出，将推送给3~5个随机用户')
        // 重置表单
        content.value = {
          text: '',
          voice: '',
          image: ''
        }
        // 隐藏动画
        showTreeHoleAnimation.value = false
        // 关闭表单
        showForm.value = false
      }, 1000)
    } else {
      message.error('发送树洞失败，请重试')
      showTreeHoleAnimation.value = false
    }
  } catch (error) {
    console.error('发送树洞失败:', error)
    message.error('发送树洞失败，请检查网络连接')
    showTreeHoleAnimation.value = false
  }
}
</script>

<style scoped>
.treehole-container {
  width: 100vw;
  min-height: 100vh;
  position: relative;
  overflow: hidden;
}

/* 树林背景 */
.forest-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: url('../assets/tree.jpg');
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

.header {
  display: flex;
  align-items: center;
  margin-bottom: 32px;
  color: #fff;
}

.back-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: white;
  padding: 8px;
  margin-right: 16px;
  font-size: 24px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
}

.page-title {
  font-size: 24px;
  font-weight: 700;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.8);
  margin: 0;
}

.content {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.text-count {
  text-align: right;
  margin-bottom: 24px;
}

.media-buttons {
  display: flex;
  gap: 16px;
  margin-bottom: 16px;
}

.media-btn {
  flex: 1;
  background-color: #f0f0f0;
  border: none;
  border-radius: 8px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.media-btn:hover {
  background-color: #e0e0e0;
}

.media-buttons + p {
  margin-top: 8px;
  margin-bottom: 32px;
}

.send-container {
  position: relative;
}

.send-btn {
  width: 100%;
  padding: 16px;
  font-size: 18px;
}

.treehole-animation {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 10;
  animation: treeHoleDrop 1s ease-out forwards;
}

.treehole-svg {
  width: 48px;
  height: 48px;
}

@keyframes treeHoleDrop {
  0% {
    transform: translate(-50%, -150%) scale(1);
    opacity: 1;
  }
  50% {
    transform: translate(-50%, -50%) scale(1.2);
    opacity: 0.8;
  }
  100% {
    transform: translate(-50%, -50%) scale(0.8);
    opacity: 0;
  }
}

/* 底部按钮 */
.bottom-buttons {
  display: flex;
  gap: 16px;
  margin-top: auto;
  margin-bottom: 20px;
  padding: 0 20px;
}

.bottom-btn {
  flex: 1;
  padding: 16px;
  font-size: 16px;
  border-radius: 12px;
}

.bottom-btn:first-child {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  border: none;
  color: white;
}

.bottom-btn:last-child {
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  backdrop-filter: blur(10px);
}
</style>