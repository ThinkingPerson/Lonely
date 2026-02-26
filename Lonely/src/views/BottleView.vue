<template>
  <div class="bottle-container">
    <!-- 海滩背景 -->
    <div class="beach-background">
      <!-- 半透明叠加层，降低背景亮度 -->
      <div class="background-overlay"></div>
    </div>
    
    <div class="content-wrapper">
      <!-- 头部 -->
      <div class="header">
        <button class="back-btn" @click="goBack">
          <LeftOutlined />
        </button>
        <h1 class="page-title">扔一个瓶子</h1>
      </div>
      
      <!-- 内容区域 - 只在showForm为true时显示 -->
      <div class="content" v-if="showForm">
        <!-- 文本输入 -->
        <textarea 
          class="textarea" 
          v-model="content.text"
          placeholder="写下你的心情..."
          maxlength="200"
          rows="4"
        ></textarea>
        <div class="text-count text-sm text-gray">{{ content.text.length }}/200</div>
        
        <!-- 多媒体按钮 -->
        <div class="media-buttons">
          <button class="media-btn" @click="recordVoice">
            <AudioOutlined style="font-size: 24px;" />
            <span>{{ content.voice ? '已录制' : '语音' }}</span>
          </button>
          
          <button class="media-btn" @click="uploadImage">
            <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
              <circle cx="8.5" cy="8.5" r="1.5"></circle>
              <polyline points="21 15 16 10 5 21"></polyline>
            </svg>
            <span>{{ content.image ? '已上传' : '图片' }}</span>
          </button>
        </div>
        
        <!-- 情绪标签 -->
        <div class="section">
          <h3>情绪</h3>
          <div class="tags">
            <span 
              v-for="emotion in emotions" 
              :key="emotion"
              class="tag" 
              :class="content.emotion === emotion ? 'tag-active' : 'tag-inactive'"
              @click="selectEmotion(emotion)"
            >
              {{ emotion }}
            </span>
          </div>
        </div>
        
        <!-- 主题标签 -->
        <div class="section">
          <h3>主题</h3>
          <div class="tags">
            <span 
              v-for="topic in topics" 
              :key="topic"
              class="tag" 
              :class="content.topics.includes(topic) ? 'tag-active' : 'tag-inactive'"
              @click="toggleTopic(topic)"
            >
              {{ topic }}
            </span>
          </div>
        </div>
        
        <!-- 扔出去按钮 -->
        <div class="throw-container">
          <button class="btn btn-primary throw-btn" @click="throwBottle">
            扔出去
          </button>
          <div class="bottle-animation" v-if="showBottleAnimation">
            <svg class="bottle-svg" viewBox="0 0 24 24" fill="white" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 2L2 7l10 5 10-5-10-5z"></path>
              <path d="M2 17l10 5 10-5"></path>
              <path d="M2 12l10 5 10-5"></path>
            </svg>
          </div>
        </div>
      </div>
      
      <!-- 底部按钮 -->
      <div class="bottom-buttons">
        <button class="btn btn-primary bottom-btn" @click="toggleForm">
          {{ showForm ? '取消' : '扔一个瓶子' }}
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
import { LeftOutlined, AudioOutlined } from '@ant-design/icons-vue'
import request from '../utils/http'

const router = useRouter()

const content = ref({
  text: '',
  voice: '',
  image: '',
  emotion: '',
  topics: []
})

const showBottleAnimation = ref(false)
const showForm = ref(false)

const emotions = ['开心', '疲惫', '无聊', '难过', '兴奋', '焦虑']
const topics = ['游戏', '电影', '情感', '音乐', '旅行', '美食', '读书', '运动']

const goBack = () => {
  router.back()
}

const toggleForm = () => {
  showForm.value = !showForm.value
  if (showForm.value) {
    // 重置表单
    content.value = {
      text: '',
      voice: '',
      image: '',
      emotion: '',
      topics: []
    }
  }
}

const goToMessages = () => {
  router.push('/bottle-receive')
}

const recordVoice = () => {
  // 模拟语音录制
  content.value.voice = 'voice_' + Date.now()
  alert('语音录制功能已模拟')
}

const uploadImage = () => {
  // 模拟图片上传
  content.value.image = 'image_' + Date.now()
  alert('图片上传功能已模拟')
}

const selectEmotion = (emotion) => {
  content.value.emotion = emotion
}

const toggleTopic = (topic) => {
  const index = content.value.topics.indexOf(topic)
  if (index > -1) {
    content.value.topics.splice(index, 1)
  } else {
    content.value.topics.push(topic)
  }
}

const throwBottle = async () => {
  // 校验至少有一种内容
  if (!content.value.text && !content.value.voice && !content.value.image) {
    alert('请至少填写一种内容')
    return
  }
  
  try {
    // 显示扔瓶子动画
    showBottleAnimation.value = true
    
    // 调用后端API扔瓶子
    const response = await request.post('/Bottle/Throw', {
      Content: content.value.text,
      VoiceUrl: content.value.voice,
      ImageUrl: content.value.image,
      Emotion: content.value.emotion,
      Topics: JSON.stringify(content.value.topics)
    })
    
    if (response.success) {
      // 显示成功消息
      setTimeout(() => {
        alert('瓶子已漂向远方…等待回应')
        // 重置表单
        content.value = {
          text: '',
          voice: '',
          image: '',
          emotion: '',
          topics: []
        }
        // 隐藏动画
        showBottleAnimation.value = false
        // 关闭表单
        showForm.value = false
      }, 1000)
    } else {
      alert('扔瓶子失败，请重试')
      showBottleAnimation.value = false
    }
  } catch (error) {
    console.error('扔瓶子失败:', error)
    alert('扔瓶子失败，请检查网络连接')
    showBottleAnimation.value = false
  }
}
</script>

<style scoped>
.bottle-container {
  width: 100vw;
  min-height: 100vh;
  position: relative;
  overflow: hidden;
}

/* 海滩背景 */
.beach-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: url('../assets/sea.jpg');
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
  margin-bottom: 32px;
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

.throw-container {
  position: relative;
}

.throw-btn {
  width: 100%;
  padding: 16px;
  font-size: 18px;
  margin-top: 16px;
}

.bottle-animation {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 10;
  animation: throwBottle 1s ease-out forwards;
}

.bottle-svg {
  width: 48px;
  height: 48px;
}

@keyframes throwBottle {
  0% {
    transform: translate(-50%, -50%) scale(1);
    opacity: 1;
  }
  50% {
    transform: translate(-200px, -300px) scale(0.8);
    opacity: 0.8;
  }
  100% {
    transform: translate(-400px, -500px) scale(0.5);
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