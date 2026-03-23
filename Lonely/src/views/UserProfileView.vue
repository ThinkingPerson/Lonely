<template>
  <div class="user-profile-container">
    <PageHeader title="用户信息" />

    
    <div class="user-profile-content">
      <!-- 用户基本信息 -->
      <div class="user-basic-info">
        <div class="user-avatar" :style="{ backgroundColor: user.avatar }">{{ user.username.charAt(0) }}</div>
        <h3 class="user-name">{{ user.username }}</h3>
        <p class="user-bio">{{ user.bio || '这个人很懒，什么都没有留下' }}</p>
        <button class="follow-btn" @click="toggleFollow">
          {{ isFollowing ? '已关注' : '关注' }}
        </button>
      </div>
      
      <!-- 用户统计信息 -->
      <div class="user-stats">
        <div class="stat-item">
          <span class="stat-value">{{ user.posts }}</span>
          <span class="stat-label">动态</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">{{ user.followers }}</span>
          <span class="stat-label">关注者</span>
        </div>
        <div class="stat-item">
          <span class="stat-value">{{ user.following }}</span>
          <span class="stat-label">关注</span>
        </div>
      </div>
      
      <!-- 用户动态 -->
      <div class="user-posts">
        <h4 class="posts-title">动态</h4>
        <div class="posts-list">
          <div 
            v-for="(post, index) in user.postsList" 
            :key="index"
            class="post-item"
          >
            <p class="post-content">{{ post.content }}</p>
            <div class="post-images" v-if="post.images && post.images.length > 0">
              <img 
                v-for="(image, imgIndex) in post.images" 
                :key="imgIndex"
                :src="image"
                class="post-image"
                alt="Post image"
              />
            </div>
            <div class="post-audio" v-if="post.audio">
              <audio controls class="audio-player">
                <source :src="post.audio" type="audio/mpeg">
                您的浏览器不支持音频播放
              </audio>
            </div>
            <div class="post-meta">
              <span class="post-time">{{ post.time }}</span>
              <div class="post-actions">
                <span class="action-item">{{ post.likes }} 赞</span>
                <span class="action-item">{{ post.comments }} 评论</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import PageHeader from '../components/PageHeader.vue'

const router = useRouter()
const route = useRoute()

// 模拟用户数据
const user = ref({
  id: 1,
  username: '快乐的旅行者123',
  avatar: '#FF6B6B',
  bio: '喜欢旅行，热爱生活',
  posts: 12,
  followers: 56,
  following: 34,
  postsList: [
    {
      id: 1,
      content: '今天天气真好，心情也跟着变好了！',
      images: ['https://trae-api-cn.mchost.guru/api/ide/v1/text_to_image?prompt=sunny%20day%20with%20blue%20sky%20and%20white%20clouds&image_size=landscape_16_9'],
      audio: null,
      time: '2024-03-03 10:30',
      likes: 12,
      comments: 2
    },
    {
      id: 2,
      content: '分享一段我最喜欢的音乐，希望大家喜欢！',
      images: [],
      audio: 'https://example.com/audio.mp3',
      time: '2024-03-02 15:45',
      likes: 8,
      comments: 1
    },
    {
      id: 3,
      content: '又是美好的一天，加油！',
      images: [
        'https://trae-api-cn.mchost.guru/api/ide/v1/text_to_image?prompt=beautiful%20sunrise%20over%20mountains&image_size=landscape_16_9',
        'https://trae-api-cn.mchost.guru/api/ide/v1/text_to_image?prompt=green%20forest%20path&image_size=landscape_16_9'
      ],
      audio: null,
      time: '2024-03-01 08:00',
      likes: 15,
      comments: 0
    }
  ]
})

const isFollowing = ref(false)

// 返回上一页
const goBack = () => {
  router.push('/main')
}

// 切换关注状态
const toggleFollow = () => {
  isFollowing.value = !isFollowing.value
  if (isFollowing.value) {
    user.value.followers++
  } else {
    user.value.followers--
  }
}

// 初始化
onMounted(() => {
  // 可以在这里根据路由参数获取用户 ID，然后加载用户数据
  const userId = route.params.id
  console.log('User ID:', userId)
  // 这里可以根据 userId 加载用户数据
})
</script>

<style scoped>
.user-profile-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 20px;
}



.user-profile-content {
  max-width: 600px;
  margin: 0 auto;
  background-color: white;
  border-radius: 12px;
  padding: 30px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.user-basic-info {
  text-align: center;
  margin-bottom: 30px;
}

.user-avatar {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 40px;
  font-weight: 600;
  color: white;
  margin: 0 auto 20px;
  transition: all 0.3s ease;
}

.user-name {
  font-size: 24px;
  font-weight: 600;
  color: #333;
  margin: 0 0 10px 0;
}

.user-bio {
  font-size: 14px;
  color: #666;
  margin: 0 0 20px 0;
  line-height: 1.5;
}

.follow-btn {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  border: none;
  border-radius: 20px;
  padding: 10px 24px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(155, 89, 182, 0.4);
}

.follow-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(155, 89, 182, 0.6);
}

.user-stats {
  display: flex;
  justify-content: space-around;
  margin-bottom: 30px;
  padding: 20px 0;
  border-top: 1px solid #f0f0f0;
  border-bottom: 1px solid #f0f0f0;
}

.stat-item {
  text-align: center;
}

.stat-value {
  display: block;
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 14px;
  color: #666;
}

.user-posts {
  margin-top: 30px;
}

.posts-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0 0 20px 0;
}

.posts-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.post-item {
  padding: 15px;
  border-radius: 8px;
  background-color: #f9f9f9;
  transition: all 0.3s ease;
}

.post-item:hover {
  background-color: #f0f0f0;
  transform: translateY(-2px);
}

.post-content {
  font-size: 14px;
  color: #333;
  line-height: 1.5;
  margin: 0 0 12px 0;
}

.post-images {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 8px;
  margin-bottom: 12px;
}

.post-image {
  width: 100%;
  height: 100px;
  object-fit: cover;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.post-image:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
}

.post-audio {
  margin-bottom: 12px;
}

.audio-player {
  width: 100%;
  height: 40px;
  border-radius: 20px;
  overflow: hidden;
}

.post-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 12px;
  color: #999;
}

.post-actions {
  display: flex;
  gap: 15px;
}

.action-item {
  cursor: pointer;
  transition: color 0.3s ease;
}

.action-item:hover {
  color: #9b59b6;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .user-profile-container {
    padding: 10px;
  }
  
  .user-profile-content {
    padding: 20px;
  }
  
  .user-avatar {
    width: 80px;
    height: 80px;
    font-size: 32px;
  }
  
  .user-name {
    font-size: 20px;
  }
  
  .post-images {
    grid-template-columns: repeat(auto-fill, minmax(80px, 1fr));
  }
  
  .post-image {
    height: 80px;
  }
}
</style>