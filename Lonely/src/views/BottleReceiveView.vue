<template>
  <div class="bottle-receive-container">
    <div class="header">
      <button class="back-btn" @click="goBack">
        <LeftOutlined />
      </button>
      <h1>捡到一个瓶子</h1>
    </div>
    
    <div class="content">
      <!-- 瓶子内容 -->
      <div class="bottle-content">
        <div class="bottle-header">
          <div class="function-icon bottle-icon">
            <svg class="icon" viewBox="0 0 24 24" fill="white" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 2L2 7l10 5 10-5-10-5z"></path>
              <path d="M2 17l10 5 10-5"></path>
              <path d="M2 12l10 5 10-5"></path>
            </svg>
          </div>
          <span class="time">10分钟前</span>
        </div>
        <div class="bottle-body">
          <p>{{ bottleContent }}</p>
          <div class="bottle-tags">
            <span class="tag tag-active">{{ bottleEmotion }}</span>
            <span v-for="topic in bottleTopics" :key="topic" class="tag tag-inactive">{{ topic }}</span>
          </div>
        </div>
      </div>
      
      <!-- 回复输入 -->
      <div class="reply-input">
        <textarea 
          v-model="replyContent"
          placeholder="写下你的回复..."
          maxlength="100"
          rows="3"
        ></textarea>
        <button class="btn btn-primary reply-btn" @click="sendReply">
          回复
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { LeftOutlined } from '@ant-design/icons-vue'
import request from '../utils/http'

const router = useRouter()

const bottleContent = ref('')
const bottleEmotion = ref('')
const bottleTopics = ref([])
const replyContent = ref('')
const currentBottleId = ref(0)

const goBack = () => {
  router.back()
}

const pickBottle = async () => {
  try {
    const response = await request.get('/Bottle/Pick')
    if (response.success) {
      const bottle = response.data
      bottleContent.value = bottle.Content
      bottleEmotion.value = bottle.Emotion || ''
      bottleTopics.value = bottle.Topics ? JSON.parse(bottle.Topics) : []
      currentBottleId.value = bottle.Id
    } else {
      alert('暂无漂流瓶')
      router.push('/bottle')
    }
  } catch (error) {
    console.error('捡瓶子失败:', error)
    alert('捡瓶子失败，请检查网络连接')
  }
}

const sendReply = async () => {
  if (replyContent.value.trim()) {
    try {
      // 这里需要实现回复功能，暂时模拟
      setTimeout(() => {
        alert('回复已发送，对方将收到你的消息')
        // 重置表单
        replyContent.value = ''
        // 返回主界面
        router.push('/main')
      }, 1000)
    } catch (error) {
      console.error('回复失败:', error)
      alert('回复失败，请检查网络连接')
    }
  }
}

onMounted(() => {
  pickBottle()
})
</script>

<style scoped>
.bottle-receive-container {
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

.bottle-content {
  margin-bottom: 32px;
  padding-bottom: 24px;
  border-bottom: 1px solid #eee;
}

.bottle-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.time {
  font-size: 12px;
  color: #999;
}

.bottle-body {
  font-size: 16px;
  line-height: 1.5;
  color: #333;
}

.bottle-tags {
  margin-top: 16px;
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
</style>