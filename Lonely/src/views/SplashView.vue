<template>
  <div class="splash-container">
    <div class="logo">Lonely</div>
    <div class="slogan">遇见陌生人的温度</div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onMounted(() => {
  setTimeout(() => {
    router.push('/entry')
  }, 2000)
})
</script>

<style scoped>
.splash-container {
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: linear-gradient(rgba(155, 89, 182, 0.9), rgba(75, 0, 130, 0.95)), url('https://trae-api-cn.mchost.guru/api/ide/v1/text_to_image?prompt=night%20sky%20with%20purple%20and%20blue%20gradients%2C%20stars%2C%20romantic%20atmosphere%2C%20soft%20blurred%20background&image_size=landscape_16_9');
  background-size: cover;
  background-position: center;
  color: white;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
}

.logo {
  font-size: 48px;
  font-weight: 700;
  margin-bottom: 24px;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.slogan {
  font-size: 18px;
  opacity: 0.9;
}
</style>