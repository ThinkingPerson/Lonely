import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    component: () => import('../views/SplashView.vue'),
    meta: {
      title: '启动页 - 展示Logo与宣传语，停留2秒后自动跳转'
    }
  },
  {
    path: '/entry',
    component: () => import('../views/EntryView.vue'),
    meta: {
      title: '进入页 - 提供直接进入和快捷登录选项'
    }
  },
  {
    path: '/main',
    component: () => import('../views/MainView.vue'),
    meta: {
      title: '主界面 - 显示用户信息和三个功能入口'
    }
  },
  {
    path: '/bottle',
    component: () => import('../views/BottleView.vue'),
    meta: {
      title: '扔瓶子页 - 海滩场景，支持文字、语音、图片输入'
    }
  },
  {
    path: '/match',
    component: () => import('../views/MatchView.vue'),
    meta: {
      title: '随机匹配聊天页 - 选择兴趣标签并开始匹配'
    }
  },
  {
    path: '/chat',
    component: () => import('../views/ChatView.vue'),
    meta: {
      title: '聊天会话页 - 支持文字、语音、表情发送，可开启阅后即焚'
    }
  },
  {
    path: '/treehole',
    component: () => import('../views/TreeHoleView.vue'),
    meta: {
      title: '发树洞回声页 - 树林场景，支持文字和语音输入'
    }
  },
  {
    path: '/treehole-receive',
    component: () => import('../views/TreeHoleReceiveView.vue'),
    meta: {
      title: '树洞回声接收页 - 查看树洞内容并匿名回复'
    }
  },
  {
    path: '/bottle-receive',
    component: () => import('../views/BottleReceiveView.vue'),
    meta: {
      title: '捡到瓶子页 - 查看捡到的瓶子内容并回复'
    }
  },
  {
    path: '/settings',
    component: () => import('../views/SettingsView.vue'),
    meta: {
      title: '设置页 - 显示用户信息、隐私说明、举报入口等'
    }
  },
  {
    path: '/readme',
    component: () => import('../views/ReadmeView.vue'),
    meta: {
      title: '阅读说明 - 首次使用必读'
    }
  },
  {
    path: '/disclaimer',
    component: () => import('../views/DisclaimerView.vue'),
    meta: {
      title: '免责声明 - 法律责任说明'
    }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router