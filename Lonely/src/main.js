import { createApp, ref, provide } from 'vue'
import App from './App.vue'
import router from './router'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import { userApi } from './services/api'

const app = createApp(App)

// 全局用户状态
const userState = ref({
  isLoggedIn: false,
  userInfo: null
})

// 提供全局用户状态
app.provide('userState', userState)

// 初始化用户信息
const initUserInfo = async () => {
  try {
    const response = await userApi.getProfile()
    if (response.success) {
      userState.value = {
        isLoggedIn: true,
        userInfo: response.data
      }
    } else {
      userState.value = {
        isLoggedIn: false,
        userInfo: null
      }
    }
  } catch (error) {
    console.error('获取用户信息失败:', error)
    userState.value = {
      isLoggedIn: false,
      userInfo: null
    }
  }
}

// 路由守卫，检查登录状态
router.beforeEach(async (to, from, next) => {
  // 初始化用户信息
  if (!userState.value.userInfo) {
    await initUserInfo()
  }
  
  // 这里可以添加需要登录才能访问的路由检查
  // 例如：if (需要登录 && !userState.value.isLoggedIn) { next('/entry'); return; }
  
  next()
})

app.use(router)
app.use(Antd)

app.mount('#app')