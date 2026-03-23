<template>
  <div class="disclaimer-container">
    <!-- 背景图片 -->
    <div class="disclaimer-background">
      <!-- 半透明叠加层，降低亮度 -->
      <div class="background-overlay"></div>
    </div>
    
    <div class="content-wrapper">
      <div class="disclaimer-card">
        <!-- 头部 -->
        <div class="disclaimer-header">
          <button class="back-btn" @click="goBack">
            <LeftOutlined />
          </button>
          <h2 class="disclaimer-title">Lonely 匿名轻社交免责声明</h2>
        </div>
        
        <div class="disclaimer-content">
          <h3>内容责任</h3>
          <p>平台仅提供匿名社交通道，不审核每一条聊天或树洞的具体内容，用户须自行判断信息真伪与安全性。</p>
          
          <h3>风险自负</h3>
          <p>因匿名互动产生的任何情感、心理、财产或其他损失，平台不承担法律责任。</p>
          
          <h3>禁止行为</h3>
          <p>严禁发布违法、侵权、骚扰、暴力、色情等内容，违规将被封禁。</p>
          
          <h3>未成年人提示</h3>
          <p>本产品不面向未满 18 岁用户，如误用，请立即停止并卸载。</p>
          
          <h3>服务变更与终止</h3>
          <p>平台有权随时修改、暂停或终止服务，恕不另行通知。</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { LeftOutlined } from '@ant-design/icons-vue'

const router = useRouter()

const goBack = () => {
  router.push('/main')
}
</script>

<style scoped>
.disclaimer-container {
  width: 100vw;
  min-height: 100vh;
  position: relative;
  overflow: hidden;
}

/* 背景图片 */
.disclaimer-background {
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
  min-height: 100vh;
  padding: 20px;
  display: flex;
  flex-direction: column;
}

.disclaimer-card {
  background: rgba(255, 255, 255, 0.98);
  border-radius: 16px;
  padding: 24px;
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  backdrop-filter: blur(10px);
}

.disclaimer-header {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
  position: relative;
}

.back-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #333;
  padding: 8px;
  margin-right: 16px;
  font-size: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.disclaimer-title {
  font-size: 20px;
  font-weight: 700;
  color: #111;
  margin: 0;
  flex: 1;
  text-align: center;
}

.disclaimer-content {
  line-height: 1.6;
  color: #222;
}

.disclaimer-content h3 {
  font-size: 16px;
  font-weight: 600;
  margin-top: 20px;
  margin-bottom: 10px;
  color: #333;
}

.disclaimer-content p {
  margin-bottom: 12px;
  color: #222;
}

/* 适配移动端 */
@media (max-width: 768px) {
  .disclaimer-card {
    margin-top: 20px;
    margin-bottom: 20px;
  }
  
  .disclaimer-title {
    font-size: 18px;
  }
  
  .disclaimer-content h3 {
    font-size: 15px;
  }
  
  .disclaimer-content p {
    font-size: 14px;
  }
}
</style>