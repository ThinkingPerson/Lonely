<template>
  <div class="voice-upload">
    <button class="media-btn" @click="toggleRecording" :disabled="loading">
      <AudioOutlined v-if="!loading" style="font-size: 24px;" />
      <div v-else class="loading-spinner"></div>
      <span>{{ loading ? '上传中...' : (isRecording ? '停止录制' : hasVoice ? '已录制' : '语音') }}</span>
    </button>
    <!-- 语音预览 -->
    <div v-if="hasVoice && modelValue" class="voice-preview">
      <audio :src="modelValue" controls class="audio-player"></audio>
      <button class="remove-btn" @click="removeVoice">×</button>
    </div>
  </div>
</template>

<script setup>
import { ref, defineEmits, defineProps } from 'vue'
import { message } from 'ant-design-vue'
import { AudioOutlined } from '@ant-design/icons-vue'
import { uploadVoice } from '../services/upload'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:modelValue'])

const isRecording = ref(false)
const hasVoice = ref(props.modelValue !== '')
const mediaRecorder = ref(null)
const audioChunks = ref([])
const loading = ref(false)

const toggleRecording = () => {
  if (isRecording.value) {
    stopRecording()
  } else {
    startRecording()
  }
}

const startRecording = async () => {
  try {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
    mediaRecorder.value = new MediaRecorder(stream)
    audioChunks.value = []
    
    mediaRecorder.value.ondataavailable = (event) => {
      if (event.data.size > 0) {
        audioChunks.value.push(event.data)
      }
    }
    
    mediaRecorder.value.onstop = async () => {
      loading.value = true
      try {
        const audioBlob = new Blob(audioChunks.value, { type: 'audio/wav' })
        // 创建一个临时文件对象
        const audioFile = new File([audioBlob], 'recording.wav', { type: 'audio/wav' })
        
        // 上传语音到服务器
        const response = await uploadVoice(audioFile)
        if (response.success) {
          // 服务器返回的文件路径
          const audioUrl = response.data
          emit('update:modelValue', audioUrl)
          hasVoice.value = true
          message.success('语音录制成功')
        } else {
          message.error('语音上传失败：' + response.message)
        }
      } catch (error) {
        console.error('上传失败:', error)
        message.error('语音上传失败，请稍后重试')
      } finally {
        loading.value = false
      }
    }
    
    mediaRecorder.value.start()
    isRecording.value = true
    message.info('开始录制语音...')
  } catch (error) {
    console.error('录制失败:', error)
    message.error('无法访问麦克风，请检查权限')
  }
}

const stopRecording = () => {
  if (mediaRecorder.value && isRecording.value) {
    mediaRecorder.value.stop()
    // 停止所有音频轨道
    mediaRecorder.value.stream.getTracks().forEach(track => track.stop())
    isRecording.value = false
  }
}

// 移除语音
const removeVoice = () => {
  emit('update:modelValue', '')
  hasVoice.value = false
  message.info('语音已移除')
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

.voice-preview {
  position: relative;
  margin-top: 12px;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 12px;
  background-color: #f9f9f9;
}

.audio-player {
  width: 100%;
  height: 40px;
  border-radius: 4px;
  overflow: hidden;
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