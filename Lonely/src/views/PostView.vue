<template>
  <div class="post-container">
    <PageHeader title="发布动态" />

    
    <div class="post-content">
      <!-- 用户信息 -->
      <div class="user-info">
        <div class="user-avatar" :style="{ backgroundColor: userAvatar }">{{ userNickname.charAt(0) }}</div>
        <span class="user-name">{{ userNickname }}</span>
      </div>
      
      <!-- 动态内容输入 -->
      <div class="content-input">
        <textarea 
          v-model="postContent"
          placeholder="分享你的心情..."
          class="content-textarea"
          rows="4"
        ></textarea>
      </div>
      
      <!-- 媒体选择 -->
      <div class="media-options">
        <div class="upload-component">
          <ImageUpload 
            v-model="tempImage" 
            @update:modelValue="handleImageUpload"
          />
        </div>
        <div class="upload-component">
          <VoiceUpload 
            v-model="tempAudio" 
            @update:modelValue="handleAudioUpload"
          />
        </div>
      </div>
      
      <!-- 已选择的媒体 -->
      <div class="selected-media" v-if="selectedImages.length > 0 || selectedAudio">
        <!-- 已选择的图片 -->
        <div class="selected-images" v-if="selectedImages.length > 0">
          <div 
            v-for="(image, index) in selectedImages" 
            :key="index"
            class="selected-image"
          >
            <img :src="image" alt="Selected image" />
            <button class="remove-image" @click="removeImage(index)">
              ×
            </button>
          </div>
        </div>
        
        <!-- 已选择的语音 -->
        <div class="selected-audio" v-if="selectedAudio">
          <audio controls class="audio-player">
            <source :src="selectedAudio" type="audio/mpeg">
            您的浏览器不支持音频播放
          </audio>
          <button class="remove-audio" @click="removeAudio">
            ×
          </button>
        </div>
      </div>
      
      <!-- 发布按钮 -->
      <div class="publish-container">
        <button class="publish-btn" @click="publishPost" :disabled="!isFormValid || loading">
          {{ loading ? '发布中...' : '发布' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
import ImageUpload from '../components/ImageUpload.vue'
import VoiceUpload from '../components/VoiceUpload.vue'
import { postApi } from '../services/api'

const router = useRouter()

// 用户信息
const userNickname = ref(localStorage.getItem('nickname') || '神秘用户')
const userAvatar = ref(localStorage.getItem('avatar') || '#FF6B6B')

// 动态内容
const postContent = ref('')
const selectedImages = ref([])
const selectedAudio = ref('')
const tempImage = ref('')
const tempAudio = ref('')
const loading = ref(false)

// 计算属性：表单是否有效
const isFormValid = computed(() => {
  return postContent.value.trim() !== '' || selectedImages.value.length > 0 || selectedAudio.value
})

// 返回上一页
const goBack = () => {
  router.push('/feed')
}

// 处理图片上传
const handleImageUpload = (imageUrl) => {
  if (imageUrl && !selectedImages.value.includes(imageUrl)) {
    selectedImages.value.push(imageUrl)
  }
  // 重置tempImage，以便可以再次上传
  tempImage.value = ''
}

// 处理语音上传
const handleAudioUpload = (audioUrl) => {
  selectedAudio.value = audioUrl
  // 重置tempAudio，以便可以再次上传
  tempAudio.value = ''
}

// 移除图片
const removeImage = (index) => {
  selectedImages.value.splice(index, 1)
}

// 移除音频
const removeAudio = () => {
  selectedAudio.value = ''
}

// 发布动态
const publishPost = async () => {
  if (!isFormValid.value) return
  
  try {
    loading.value = true
    const response = await postApi.create({
      content: postContent.value,
      images: selectedImages.value,
      audio: selectedAudio.value
    })
    
    if (response.success) {
      message.success('动态发布成功！')
      // 跳转到动态广场页面
      router.push('/feed')
    } else {
      message.error('发布失败：' + response.message)
    }
  } catch (error) {
    console.error('发布动态失败:', error)
    message.error('发布失败，请稍后重试')
  } finally {
    loading.value = false
  }
}

// 初始化
onMounted(() => {
  // 可以在这里初始化数据
})
</script>

<style scoped>
.post-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 20px;
}



.publish-btn {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 16px;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(155, 89, 182, 0.4);
}

.publish-btn:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(155, 89, 182, 0.6);
}

.publish-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
  box-shadow: none;
}

.publish-btn:disabled:hover {
  transform: none;
  box-shadow: none;
}

.post-content {
  max-width: 600px;
  margin: 0 auto;
  background-color: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 20px;
}

.user-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  font-weight: 600;
  color: white;
}

.user-name {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.content-input {
  margin-bottom: 20px;
}

.content-textarea {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  line-height: 1.5;
  resize: none;
  background-color: #f9f9f9;
  transition: all 0.3s ease;
}

.content-textarea:focus {
  outline: none;
  background-color: #f0f0f0;
}

.media-options {
  display: flex;
  gap: 16px;
  margin-bottom: 20px;
  padding-top: 15px;
  border-top: 1px solid #f0f0f0;
}

.upload-component {
  flex: 1;
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
  transform: translateY(-2px);
}

.media-icon {
  font-size: 24px;
}

.media-text {
  font-size: 12px;
  color: #666;
}

.selected-media {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #f0f0f0;
}

.selected-images {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-bottom: 15px;
}

.selected-image {
  position: relative;
  width: 100px;
  height: 100px;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.selected-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-image {
  position: absolute;
  top: 5px;
  right: 5px;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background-color: rgba(0, 0, 0, 0.6);
  color: white;
  border: none;
  font-size: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.remove-image:hover {
  background-color: rgba(0, 0, 0, 0.8);
  transform: scale(1.1);
}

.selected-audio {
  position: relative;
  margin-bottom: 15px;
}

.audio-player {
  width: 100%;
  height: 40px;
  border-radius: 20px;
  overflow: hidden;
}

.remove-audio {
  position: absolute;
  top: 50%;
  right: 10px;
  transform: translateY(-50%);
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background-color: rgba(0, 0, 0, 0.6);
  color: white;
  border: none;
  font-size: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.remove-audio:hover {
  background-color: rgba(0, 0, 0, 0.8);
  transform: translateY(-50%) scale(1.1);
}

.publish-container {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #f0f0f0;
}

.publish-btn {
  width: 100%;
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 14px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(155, 89, 182, 0.4);
}

.publish-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(155, 89, 182, 0.6);
}

.publish-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
  box-shadow: none;
}

.publish-btn:disabled:hover {
  transform: none;
  box-shadow: none;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .post-container {
    padding: 10px;
  }
  
  .post-content {
    padding: 15px;
  }
  
  .user-avatar {
    width: 40px;
    height: 40px;
    font-size: 18px;
  }
  
  .selected-image {
    width: 80px;
    height: 80px;
  }
}
</style>