<template>
  <div class="page-header">
    <button class="back-btn" @click="goBack">
      <LeftOutlined />
    </button>
    <h2 class="page-title">{{ title }}</h2>
    <slot name="right"></slot>
  </div>
</template>

<script setup>
import { defineProps } from 'vue'
import { useRouter } from 'vue-router'
import { LeftOutlined } from '@ant-design/icons-vue'

const router = useRouter()

const props = defineProps({
  title: {
    type: String,
    required: true
  }
})

const goBack = () => {
  router.push('/main')
}
</script>

<style scoped>
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20px;
  position: relative;
  padding: 0 20px;
}

.page-header :deep(.right-slot) {
  position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
}

.back-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 24px;
  color: #333;
  padding: 8px;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
}

.back-btn:hover {
  transform: translateY(-50%) scale(1.1);
  color: #9b59b6;
}

.page-title {
  font-size: 20px;
  font-weight: 700;
  color: #333;
  text-align: center;
  flex: 1;
  margin: 0;
}
</style>