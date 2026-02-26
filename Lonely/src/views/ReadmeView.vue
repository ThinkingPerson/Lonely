<template>
  <div class="readme-container">
    <!-- 背景图片 -->
    <div class="readme-background">
      <!-- 半透明叠加层，降低亮度 -->
      <div class="background-overlay"></div>
    </div>
    
    <div class="content-wrapper">
      <div class="readme-card">
        <h2 class="readme-title">欢迎使用 Lonely — 匿名轻社交</h2>
        
        <div class="readme-content" ref="readmeContent">
          <p>本产品为匿名轻社交工具，您无需注册或填写任何个人资料，系统会在首次使用时自动生成唯一的匿名身份标识（基于设备特征与随机数），该标识仅用于后台风控与内容追溯，不会展示给任何用户。</p>
          
          <h3>隐私与追溯说明</h3>
          <p>虽然您的使用是匿名的，但平台会对违规行为（如发布违法、骚扰、暴力、色情等内容）进行技术追溯和封禁，并可能依法向有关部门上报。请遵守社区规范，文明互动。</p>
          
          <h3>快捷登录（可选）</h3>
          <p>您可以选择使用微信或手机号快捷登录，此操作仅用于增强账号安全与身份保护，例如在举报或纠纷中提供更完整的身份依据，防止被他人冒用。登录并非强制，您可以随时选择是否启用。</p>
          
          <h3>风险提示</h3>
          <p>匿名社交可能存在陌生人发送的不良内容或情绪波动，请谨慎回应，并可随时结束会话或举报。本产品不替代心理咨询或紧急救助，如遇严重情绪困扰，请及时寻求专业帮助。</p>
        </div>
        
        <div class="readme-footer">
          <div class="checkbox-container">
            <input 
              type="checkbox" 
              id="agree" 
              v-model="isAgreed"
              @change="checkCanAgree"
            >
            <label for="agree">我已阅读并同意</label>
          </div>
          
          <button 
            class="btn btn-primary agree-btn" 
            @click="agreeAndEnter"
            :disabled="!canAgree"
          >
            进入应用
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const readmeContent = ref(null)
const isAgreed = ref(false)
const canAgree = ref(false)

// 检查是否滚动到底部
const checkScrollPosition = () => {
  if (!readmeContent.value) return
  
  const { scrollTop, scrollHeight, clientHeight } = readmeContent.value
  const isAtBottom = scrollHeight - scrollTop <= clientHeight + 10 // 10px 误差
  
  if (isAtBottom && isAgreed.value) {
    canAgree.value = true
  } else {
    canAgree.value = false
  }
}

// 检查是否可以同意
const checkCanAgree = () => {
  checkScrollPosition()
}

// 同意并进入应用
const agreeAndEnter = () => {
  if (canAgree.value) {
    localStorage.setItem('has_read_instructions', 'true')
    router.push('/main')
  }
}

onMounted(() => {
  if (readmeContent.value) {
    readmeContent.value.addEventListener('scroll', checkScrollPosition)
  }
})

watch(isAgreed, () => {
  checkCanAgree()
})
</script>

<style scoped>
.readme-container {
  width: 100vw;
  height: 100vh;
  position: relative;
  overflow: hidden;
}

/* 背景图片 */
.readme-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: url('../assets/bed1.jpg');
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
  background-color: rgba(0, 0, 0, 0.4);
  z-index: 1;
}

.content-wrapper {
  position: relative;
  z-index: 2;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.readme-card {
  background: rgba(255, 255, 255, 0.98);
  border-radius: 16px;
  padding: 24px;
  width: 100%;
  max-width: 500px;
  max-height: 80vh;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(10px);
  display: flex;
  flex-direction: column;
}

.readme-title {
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 20px;
  color: #111;
  text-align: center;
}

.readme-content {
  flex: 1;
  overflow-y: auto;
  margin-bottom: 20px;
  padding-right: 10px;
  line-height: 1.6;
  color: #222;
  max-height: 60vh;
}

.readme-content h3 {
  font-size: 16px;
  font-weight: 600;
  margin-top: 20px;
  margin-bottom: 10px;
  color: #333;
}

.readme-content p {
  margin-bottom: 12px;
  color: #222;
}

.checkbox-container label {
  font-size: 14px;
  color: #111;
  cursor: pointer;
}

.readme-content::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 3px;
}

.readme-content::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
}

.readme-content::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

.readme-footer {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.checkbox-container {
  display: flex;
  align-items: center;
  gap: 8px;
}

.checkbox-container input[type="checkbox"] {
  width: 18px;
  height: 18px;
  cursor: pointer;
}

.checkbox-container label {
  font-size: 14px;
  color: #333;
  cursor: pointer;
}

.agree-btn {
  width: 100%;
  padding: 14px;
  font-size: 16px;
  border-radius: 12px;
  border: none;
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  cursor: pointer;
  transition: all 0.3s ease;
}

.agree-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(155, 89, 182, 0.6);
}

.agree-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}
</style>