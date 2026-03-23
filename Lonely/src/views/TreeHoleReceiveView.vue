<template>
  <div class="treehole-receive-container">
    <div class="header">
      <button class="back-btn" @click="goBack">
        <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <path d="M19 12H5M12 19l-7-7 7-7"></path>
        </svg>
      </button>
      <h1>树洞回声</h1>
    </div>
    
    <div class="content">
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>加载中...</p>
      </div>
      
      <!-- 树洞内容 -->
      <div v-else class="treehole-content">
        <div class="treehole-header">
          <div class="avatar">匿名</div>
          <span class="time">{{ formatTime(treeholeData.createdAt) }}</span>
        </div>
        <div class="treehole-body">
          <p>{{ treeholeData.content }}</p>
          <!-- 图片显示 -->
          <div v-if="treeholeData.imageUrl" class="treehole-media">
            <img :src="treeholeData.imageUrl" alt="Treehole image" class="treehole-image" />
          </div>
          <!-- 语音显示 -->
          <div v-if="treeholeData.voiceUrl" class="treehole-media">
            <audio :src="treeholeData.voiceUrl" controls class="treehole-audio"></audio>
          </div>
        </div>
      </div>
      
      <!-- 回复列表 -->
      <div v-if="!loading" class="replies">
        <h3>回声</h3>
        <div v-if="replies.length === 0" class="no-replies">
          <p>还没有回声，快来成为第一个回应的人吧</p>
        </div>
        <div v-else class="reply-item" v-for="(reply, index) in replies" :key="index">
          <div class="avatar">匿名</div>
          <div class="reply-content">
            <p>{{ reply.content }}</p>
            <span class="time">{{ reply.time }}</span>
          </div>
        </div>
      </div>
      
      <!-- 回复输入 -->
      <div v-if="!loading" class="reply-input">
        <textarea 
          v-model="replyContent"
          placeholder="写下你的回声..."
          maxlength="100"
          rows="3"
        ></textarea>
        <button class="btn btn-primary reply-btn" @click="sendReply" :disabled="sending">
          {{ sending ? '发送中...' : '发送回声' }}
        </button>
      </div>
    </div>
    
    <!-- 提示信息 -->
    <div class="tips text-sm text-gray">
      所有树洞及回复在48小时后自动删除
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import request from '../utils/http'

const router = useRouter()

const treeholeData = ref({
  id: 0,
  content: '',
  imageUrl: '',
  voiceUrl: '',
  createdAt: ''
})
const replies = ref([])
const replyContent = ref('')
const loading = ref(true)
const sending = ref(false)

const goBack = () => {
  router.push('/main')
}

const formatTime = (timeString) => {
  if (!timeString) return ''
  const date = new Date(timeString)
  const now = new Date()
  const diff = now - date
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)
  
  if (minutes < 1) return '刚刚'
  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  if (days < 7) return `${days}天前`
  return date.toLocaleDateString()
}

const getRandomTreeHole = async () => {
  try {
    loading.value = true
    const response = await request.get('/TreeHole/Random')
    if (response.success) {
      treeholeData.value = response.data
      // 获取回复列表
      await getTreeHoleReplies(treeholeData.value.id)
    } else {
      message.info('暂无树洞')
      router.push('/treehole')
    }
  } catch (error) {
    console.error('获取树洞失败:', error)
    message.error('获取树洞失败，请检查网络连接')
  } finally {
    loading.value = false
  }
}

const getTreeHoleReplies = async (treeHoleId) => {
  try {
    const response = await request.get(`/TreeHole/Replies/${treeHoleId}`)
    if (response.success) {
      replies.value = response.data
    }
  } catch (error) {
    console.error('获取回复失败:', error)
  }
}

const sendReply = async () => {
  if (!replyContent.value.trim()) return
  
  try {
    sending.value = true
    const response = await request.post('/TreeHole/Reply', {
      TreeHoleId: treeholeData.value.id,
      Content: replyContent.value
    })
    
    if (response.success) {
      const newReply = {
        id: response.data.id,
        userId: response.data.userId,
        content: response.data.content,
        time: '刚刚',
        username: '匿名'
      }
      replies.value.push(newReply)
      replyContent.value = ''
      message.success('回复已发送，将匿名展示')
    } else {
      message.error('回复失败: ' + (response.message || '未知错误'))
    }
  } catch (error) {
    console.error('回复失败:', error)
    message.error('回复失败，请检查网络连接')
  } finally {
    sending.value = false
  }
}

onMounted(() => {
  getRandomTreeHole()
})
</script>

<style scoped>
.treehole-receive-container {
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

.treehole-content {
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid #eee;
}

.treehole-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.time {
  font-size: 12px;
  color: #999;
}

.treehole-body {
  font-size: 16px;
  line-height: 1.5;
  color: #333;
}

.treehole-media {
  margin: 16px 0;
}

.treehole-image {
  width: 100%;
  max-height: 300px;
  object-fit: cover;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.treehole-audio {
  width: 100%;
  height: 40px;
  border-radius: 4px;
  overflow: hidden;
}

.replies {
  margin-bottom: 32px;
}

.replies h3 {
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 16px;
}

.reply-item {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
}

.reply-content {
  flex: 1;
}

.reply-content p {
  font-size: 14px;
  line-height: 1.4;
  margin-bottom: 4px;
}

.reply-input textarea {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  resize: none;
  margin-bottom: 16px;
}

.reply-btn {
  width: 100%;
  padding: 12px;
  font-size: 16px;
}

.tips {
  text-align: center;
  margin-top: 20px;
}

/* 加载状态 */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(155, 89, 182, 0.2);
  border-left-color: #9b59b6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.loading-container p {
  font-size: 14px;
  color: #666;
  margin: 0;
}

.no-replies {
  text-align: center;
  padding: 40px 20px;
  color: #999;
  font-size: 14px;
}
</style>