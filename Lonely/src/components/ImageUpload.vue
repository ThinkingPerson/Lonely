<template>
  <div class="image-upload">
    <button class="media-btn" @click="triggerFileInput" :disabled="loading">
      <PictureOutlined v-if="!loading" style="font-size: 24px;" />
      <div v-else class="loading-spinner"></div>
      <span>{{ loading ? '上传中...' : (hasImage ? '已上传' : '图片') }}</span>
    </button>
    <input 
      type="file" 
      ref="fileInput" 
      accept="image/*" 
      capture="camera" 
      style="display: none;" 
      @change="handleFileChange"
    />
    <!-- 图片预览 -->
    <div v-if="hasImage && modelValue" class="image-preview">
      <img :src="modelValue" alt="Preview" class="preview-image" />
      <button class="remove-btn" @click="removeImage">×</button>
    </div>
  </div>
</template>

<script setup>
import { ref, defineEmits, defineProps } from 'vue'
import { message } from 'ant-design-vue'
import { PictureOutlined } from '@ant-design/icons-vue'
import { uploadImage } from '../services/upload'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:modelValue'])

const fileInput = ref(null)
const hasImage = ref(props.modelValue !== '')
const loading = ref(false)

const triggerFileInput = () => {
  fileInput.value.click()
}

const handleFileChange = async (event) => {
  const file = event.target.files[0]
  if (file) {
    // 检查文件大小（限制为5MB）
    if (file.size > 5 * 1024 * 1024) {
      message.error('图片大小不能超过5MB')
      return
    }
    
    // 检查文件类型
    if (!file.type.startsWith('image/')) {
      message.error('请选择图片文件')
      return
    }
    
    loading.value = true
    console.log("=========file======",file);
    try {
      // 上传图片到服务器
      const response = await uploadImage(file)
      if (response.success) {
        // 服务器返回的文件路径
        const imageUrl = response.data
        emit('update:modelValue', imageUrl)
        hasImage.value = true
        message.success('图片上传成功')
      } else {
        message.error('图片上传失败：' + response.message)
      }
    } catch (error) {
      console.error('上传失败:', error)
      message.error('图片上传失败，请稍后重试')
    } finally {
      loading.value = false
    }
  }
}

// 移除图片
const removeImage = () => {
  emit('update:modelValue', '')
  hasImage.value = false
  message.info('图片已移除')
}
</script>

<style scoped>
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

.icon {
  width: 24px;
  height: 24px;
}

.loading-spinner {
  width: 24px;
  height: 24px;
  border: 2px solid rgba(0, 0, 0, 0.1);
  border-top: 2px solid #333;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.media-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.image-preview {
  position: relative;
  margin-top: 12px;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  max-width: 100%;
}

.preview-image {
  width: 100%;
  height: auto;
  max-height: 200px;
  object-fit: cover;
}

.remove-btn {
  position: absolute;
  top: 8px;
  right: 8px;
  width: 28px;
  height: 28px;
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

.remove-btn:hover {
  background-color: rgba(0, 0, 0, 0.8);
  transform: scale(1.1);
}
</style>