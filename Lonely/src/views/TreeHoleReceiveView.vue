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
      <!-- 树洞内容 -->
      <div class="treehole-content">
        <div class="treehole-header">
          <div class="avatar">匿名</div>
          <span class="time">10分钟前</span>
        </div>
        <div class="treehole-body">
          <p>{{ treeholeContent }}</p>
        </div>
      </div>
      
      <!-- 回复列表 -->
      <div class="replies">
        <h3>回声</h3>
        <div class="reply-item" v-for="(reply, index) in replies" :key="index">
          <div class="avatar">匿名</div>
          <div class="reply-content">
            <p>{{ reply.content }}</p>
            <span class="time">{{ reply.time }}</span>
          </div>
        </div>
      </div>
      
      <!-- 回复输入 -->
      <div class="reply-input">
        <textarea 
          v-model="replyContent"
          placeholder="写下你的回声..."
          maxlength="100"
          rows="3"
        ></textarea>
        <button class="btn btn-primary reply-btn" @click="sendReply">
          发送回声
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
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const treeholeContent = ref('今天天气真好，但是我有点孤独。想找个人说说话，但是又不想被认识的人知道。这就是我使用这个匿名App的原因吧。')

const replies = ref([
  { content: '我也有同感，有时候就是想和陌生人聊聊', time: '5分钟前' },
  { content: '天气确实不错，出去走走可能会好一点', time: '3分钟前' }
])

const replyContent = ref('')

const goBack = () => {
  router.back()
}

const sendReply = () => {
  if (replyContent.value.trim()) {
    // 模拟发送回复
    replies.value.push({
      content: replyContent.value,
      time: '刚刚'
    })
    
    replyContent.value = ''
    alert('回复已发送，将匿名展示')
  }
}
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
</style>