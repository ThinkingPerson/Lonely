<template>
  <div class="chat-container">
    <!-- 顶部栏 -->
    <div class="header">
      <div class="user-info">
        <button class="back-btn" @click="goBack">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M19 12H5M12 19l-7-7 7-7"></path>
          </svg>
        </button>
        <div class="avatar">A</div>
        <span class="nickname">陌生人A</span>
      </div>
      <button class="end-btn" @click="endChat">结束聊天</button>
    </div>
    
    <!-- 消息区域 -->
    <div class="message-area">
      <div class="message" v-for="(msg, index) in messages" :key="index" :class="msg.isMine ? 'my-message' : 'other-message'">
        <div class="message-bubble" :class="msg.isMine ? 'my-bubble' : 'other-bubble'">
          {{ msg.content }}
        </div>
        <div class="message-time">{{ msg.time }}</div>
      </div>
    </div>
    
    <!-- 输入区域 -->
    <div class="input-area">
      <div class="input-tools">
        <button class="tool-btn" @click="sendVoice">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M12 15a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"></path>
            <path d="M19.071 4.929a10 10 0 0 1 0 14.142M15.545 8.455a5 5 0 0 1 0 7.071"></path>
          </svg>
        </button>
        <button class="tool-btn" @click="sendEmoji">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <circle cx="12" cy="12" r="10"></circle>
            <path d="M8 14s1.5 2 4 2 4-2 4-2"></path>
            <line x1="9" y1="9" x2="9.01" y2="9"></line>
            <line x1="15" y1="9" x2="15.01" y2="9"></line>
          </svg>
        </button>
        <label class="tool-btn file-input">
          <input type="file" accept="image/*" style="display: none;" @change="sendImage">
          <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
            <circle cx="8.5" cy="8.5" r="1.5"></circle>
            <polyline points="21 15 16 10 5 21"></polyline>
          </svg>
        </label>
      </div>
      <div class="input-wrapper">
        <input 
          type="text" 
          class="input" 
          v-model="inputMessage"
          placeholder="输入消息..."
          @keyup.enter="sendMessage"
        >
        <button class="send-btn" @click="sendMessage">发送</button>
      </div>
    </div>
    
    <!-- 阅后即焚开关 -->
    <div class="burn-after-reading">
      <label class="switch">
        <input type="checkbox" v-model="burnAfterReading">
        <span class="slider"></span>
      </label>
      <span>阅后即焚</span>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const messages = ref([
  { content: '你好，很高兴认识你！', isMine: false, time: '12:30' },
  { content: '你好，我也很高兴认识你！', isMine: true, time: '12:31' },
  { content: '今天过得怎么样？', isMine: false, time: '12:32' }
])

const inputMessage = ref('')
const burnAfterReading = ref(false)

const goBack = () => {
  router.back()
}

const sendMessage = () => {
  if (inputMessage.value.trim()) {
    const now = new Date()
    const time = now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0')
    
    messages.value.push({
      content: inputMessage.value,
      isMine: true,
      time: time
    })
    
    inputMessage.value = ''
    
    // 模拟对方回复
    setTimeout(() => {
      const replyTime = new Date()
      const replyTimeStr = replyTime.getHours().toString().padStart(2, '0') + ':' + replyTime.getMinutes().toString().padStart(2, '0')
      
      messages.value.push({
        content: '好的，我明白了！',
        isMine: false,
        time: replyTimeStr
      })
    }, 1000)
  }
}

const sendVoice = () => {
  alert('语音发送功能已模拟')
}

const sendEmoji = () => {
  alert('表情发送功能已模拟')
}

const sendImage = (event) => {
  alert('图片发送功能已模拟')
}

const endChat = () => {
  if (confirm('确定要结束聊天吗？聊天记录将不会保存。')) {
    router.push('/main')
  }
}

onMounted(() => {
  // 模拟聊天开始
  if (burnAfterReading.value) {
    setTimeout(() => {
      alert('阅后即焚已开启，消息将在60秒后自动删除')
    }, 1000)
  }
})
</script>

<style scoped>
.chat-container {
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #f5f5f5;
}

.header {
  background-color: white;
  padding: 16px 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.back-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #333;
  padding: 8px;
}

.end-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #ff3b30;
  font-size: 14px;
  font-weight: 600;
}

.message-area {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.message {
  display: flex;
  flex-direction: column;
  max-width: 70%;
}

.my-message {
  align-self: flex-end;
  align-items: flex-end;
}

.other-message {
  align-self: flex-start;
  align-items: flex-start;
}

.message-bubble {
  padding: 12px 16px;
  border-radius: 16px;
  font-size: 16px;
  line-height: 1.4;
}

.my-bubble {
  background-color: #007aff;
  color: white;
  border-bottom-right-radius: 4px;
}

.other-bubble {
  background-color: white;
  color: #333;
  border-bottom-left-radius: 4px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.message-time {
  font-size: 12px;
  color: #999;
  margin-top: 4px;
}

.input-area {
  background-color: white;
  padding: 16px 20px;
  box-shadow: 0 -2px 5px rgba(0, 0, 0, 0.1);
}

.input-tools {
  display: flex;
  gap: 16px;
  margin-bottom: 12px;
}

.tool-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #666;
  padding: 8px;
}

.input-wrapper {
  display: flex;
  gap: 12px;
}

.input-wrapper .input {
  flex: 1;
  margin-bottom: 0;
}

.send-btn {
  background-color: #007aff;
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0 20px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
}

.burn-after-reading {
  background-color: white;
  padding: 12px 20px;
  display: flex;
  align-items: center;
  gap: 8px;
  border-top: 1px solid #eee;
}

.switch {
  position: relative;
  display: inline-block;
  width: 48px;
  height: 24px;
}

.switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  transition: .4s;
  border-radius: 24px;
}

.slider:before {
  position: absolute;
  content: "";
  height: 18px;
  width: 18px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  transition: .4s;
  border-radius: 50%;
}

input:checked + .slider {
  background-color: #007aff;
}

input:checked + .slider:before {
  transform: translateX(24px);
}
</style>