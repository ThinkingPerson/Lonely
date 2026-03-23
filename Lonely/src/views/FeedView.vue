<template>
  <div class="feed-container" 
    @touchstart="handleTouchStart"
    @touchmove="handleTouchMove"
    @touchend="handleTouchEnd"
  >
    <PageHeader title="动态广场" />

    
    <div class="feed-content" :style="{ transform: `translateY(${pullDistance}px)` }">
      <!-- 下拉刷新 -->
      <div v-if="pullDistance > 0 || isRefreshing" class="pull-refresh">
        <div class="pull-refresh-content" :class="{ 'refreshing': isRefreshing }">
          <div class="pull-refresh-icon" :class="{ 'rotating': isRefreshing }">↓</div>
          <span class="pull-refresh-text">{{ refreshText }}</span>
        </div>
      </div>
      
      <!-- 加载状态 -->
      <div v-if="loading" class="loading-container">
        <div class="loading-spinner"></div>
        <p>加载中...</p>
      </div>
      
      <!-- 动态列表 -->
      <div v-else class="feed-list">
        <div 
          v-for="(post, index) in posts" 
          :key="post.id"
          class="feed-item"
        >
          <!-- 用户信息 -->
          <div class="post-user" @click="goToUserProfile(post.userId)">
            <div class="user-avatar" :style="{ backgroundColor: post.avatar }">{{ post.username.charAt(0) }}</div>
            <div class="user-info">
              <h4 class="username">{{ post.username }}</h4>
              <p class="post-time">{{ post.time }}</p>
            </div>
          </div>
          
          <!-- 动态内容 -->
          <div class="post-content">
            <p class="post-text">{{ post.content }}</p>
            <!-- 图片 -->
            <div class="post-images" v-if="post.images && post.images.length > 0">
              <img 
                v-for="(image, imgIndex) in post.images" 
                :key="imgIndex"
                :src="image"
                class="post-image"
                alt="Post image"
                @click="viewImage(image)"
              />
            </div>
            
            <!-- 图片预览模态框 -->
            <Modal
              v-model:visible="showImageModal"
              title="图片预览"
              width="90%"
              :footer="null"
              @ok="showImageModal = false"
              @cancel="showImageModal = false"
            >
              <div class="image-preview-container">
                <img :src="previewImage" alt="Preview" class="preview-image" />
              </div>
            </Modal>
            <!-- 语音 -->
            <div class="post-audio" v-if="post.audio">
              <audio controls class="audio-player">
                <source :src="post.audio" type="audio/mpeg">
                您的浏览器不支持音频播放
              </audio>
            </div>
          </div>
          
          <!-- 互动按钮 -->
          <div class="post-actions">
            <button class="action-btn like-btn" @click="toggleLike(post)">
              <span class="action-icon">{{ post.isLike ? '❤️' : '🤍' }}</span>
              <span class="action-text">{{ post.likes }}</span>
            </button>
            <button class="action-btn comment-btn" @click="toggleComments(post)">
              <span class="action-icon">💬</span>
              <span class="action-text">{{ post.comments ? post.comments.length : 0 }}</span>
            </button>
            <button class="action-btn share-btn" @click="sharePost(post)">
              <span class="action-icon">↗️</span>
              <span class="action-text">分享</span>
            </button>
            <button 
              v-if="isCurrentUser(post.userId)" 
              class="action-btn delete-btn" 
              @click="deletePost(post, index)"
            >
              <span class="action-icon">🗑️</span>
              <span class="action-text">删除</span>
            </button>
          </div>
          
          <!-- 评论区 -->
          <div class="comments-section" v-if="post.showComments">
            <div class="comments-list">
              <div 
                v-for="(comment, commentIndex) in post.comments" 
                :key="commentIndex"
                class="comment-item"
              >
                <div class="comment-user" @click="goToUserProfile(comment.userId)">
                  <div class="comment-avatar" :style="{ backgroundColor: comment.avatar }">{{ comment.username ? comment.username.charAt(0) : '?' }}</div>
                  <div class="comment-info">
                    <h5 class="comment-username">{{ comment.username }}</h5>
                    <p class="comment-text">{{ comment.content }}</p>
                  </div>
                  <button 
                    v-if="isCurrentUser(comment.userId)" 
                    class="comment-delete-btn" 
                    @click.stop="deleteComment(post, commentIndex)"
                  >
                    ×
                  </button>
                </div>
              </div>
            </div>
            <div class="comment-input-container">
              <input 
                type="text" 
                v-model="commentInput"
                placeholder="写下你的评论..."
                class="comment-input"
                @keyup.enter="addComment(post)"
              />
              <button class="comment-submit" @click="addComment(post)">
                发送
              </button>
            </div>
          </div>
        </div>
        
        <!-- 加载更多状态 -->
        <div v-if="loadingMore" class="loading-more">
          <div class="loading-spinner small"></div>
          <p>加载中...</p>
        </div>
        
        <!-- 没有更多数据 -->
        <div v-else-if="!hasMore && posts.length > 0" class="no-more">
          <p>没有更多动态了</p>
        </div>
      </div>
    </div>
    
    <!-- 右下角添加按钮 -->
    <button class="floating-post-btn" @click="goToPost">
      <span class="post-icon">+</span>
    </button>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'
import PageHeader from '../components/PageHeader.vue'
import request from '../utils/http'

const router = useRouter()

// 动态数据
const posts = ref([])
const commentInput = ref('')
const loading = ref(true)
const loadingMore = ref(false)
const previewImage = ref('')
const showImageModal = ref(false)

// 分页相关
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)
const hasMore = ref(true)
const userId = ref(localStorage.getItem('userId') || '1')

// 下拉刷新相关
const isPulling = ref(false)
const isRefreshing = ref(false)
const pullDistance = ref(0)
const startY = ref(0)
const refreshText = ref('')
const refreshThreshold = 80 // 下拉阈值

// 返回上一页
const goBack = () => {
  router.push('/main')
}

// 跳转到发动态页面
const goToPost = () => {
  router.push('/post')
}

// 跳转到用户信息页面
const goToUserProfile = (userId) => {
  router.push(`/user/${userId}`)
}

// 切换点赞状态
const toggleLike = async (post) => {
  // 保存当前状态，以便在失败时恢复
  const originalLiked = post.liked
  const originalLikes = post.likes
  
  try {
    // 先更新UI，提供即时反馈
    post.liked = !post.liked
    if (post.liked) {
      post.likes++
    } else {
      post.likes--
    }
    
    // 这里需要调用点赞API
    // await postApi.like(post.id)
  } catch (error) {
    console.error('点赞失败:', error)
    // 失败时恢复原状态
    post.liked = originalLiked
    post.likes = originalLikes
  }
}

// 切换评论区显示
const toggleComments = async (post) => {
  post.showComments = !post.showComments
  if (post.showComments && (!post.comments || post.comments.length === 0)) {
    try {
      const response = await request.get(`/Post/GetComments?postId=${post.id}&page=1&pageSize=20`)
      if (response.success) {
        post.comments = response.data.list || []
      }
    } catch (error) {
      console.error('获取评论失败:', error)
      // 失败时不改变页面状态
    }
  }
}

// 添加评论
const addComment = async (post) => {
  if (!commentInput.value.trim()) return
  
  const originalComment = commentInput.value
  
  try {
    // 这里需要调用评论API
    // const response = await postApi.comment({
    //   postId: post.id,
    //   content: commentInput.value
    // })
    
    // if (response.success) {
    //   const newComment = response.data
    //   post.comments.push(newComment)
    //   commentInput.value = ''
    // }
    
    // 暂时模拟添加评论
    const newComment = {
      id: Date.now(),
      userId: userId.value,
      username: '当前用户',
      avatar: '#9b59b6',
      content: commentInput.value,
      time: new Date().toLocaleString()
    }
    if (!post.comments) post.comments = []
    post.comments.push(newComment)
    commentInput.value = ''
  } catch (error) {
    console.error('评论失败:', error)
    // 失败时不改变页面状态，保持评论输入框内容
  }
}

// 分享动态
const sharePost = (post) => {
  // 这里可以实现分享功能
  message.info('分享功能开发中...')
}

// 查看图片
const viewImage = (image) => {
  previewImage.value = image
  showImageModal.value = true
}

// 检查是否是当前用户
const isCurrentUser = (userId) => {
  // 这里需要从本地存储或状态管理中获取当前用户ID
  // 暂时返回true，实际项目中需要根据真实的用户ID进行判断
  return true
}

// 删除动态
const deletePost = (post, index) => {
  Modal.confirm({
    title: '确认删除',
    content: '确定要删除这条动态吗？',
    okText: '确定',
    cancelText: '取消',
    onOk: async () => {
      try {
        // 这里需要调用删除动态的API
        // await postApi.delete(post.id)
        // 暂时模拟删除
        posts.value.splice(index, 1)
        message.success('动态删除成功')
      } catch (error) {
        console.error('删除动态失败:', error)
        message.error('删除失败，请稍后重试')
      }
    }
  })
}

// 删除评论
const deleteComment = (post, commentIndex) => {
  Modal.confirm({
    title: '确认删除',
    content: '确定要删除这条评论吗？',
    okText: '确定',
    cancelText: '取消',
    onOk: async () => {
      try {
        // 这里需要调用删除评论的API
        // await postApi.deleteComment(post.id, post.comments[commentIndex].id)
        // 暂时模拟删除
        post.comments.splice(commentIndex, 1)
        message.success('评论删除成功')
      } catch (error) {
        console.error('删除评论失败:', error)
        message.error('删除失败，请稍后重试')
      }
    }
  })
}

// 加载动态数据
const loadPosts = async (isLoadMore = false) => {
  if (loadingMore.value || !hasMore.value) return
  
  try {
    if (isLoadMore) {
      loadingMore.value = true
    } else {
      loading.value = true
      currentPage.value = 1
      posts.value = []
      hasMore.value = true
    }
    
    const response = await request.get(`/Post/Get?userId=${userId.value}&page=${currentPage.value}&pageSize=${pageSize.value}`)
    if (response.success) {
      const pagination = response.data
      const newPosts = pagination.list || []
      
      // 为每个动态添加showComments属性
      const postsWithShowComments = newPosts.map(post => ({
        ...post,
        showComments: false
      }))
      
      if (isLoadMore) {
        posts.value = [...posts.value, ...postsWithShowComments]
      } else {
        posts.value = postsWithShowComments
      }
      
      total.value = pagination.total || 0
      hasMore.value = currentPage.value < pagination.totalPages
      currentPage.value++
    }
  } catch (error) {
    console.error('获取动态失败:', error)
  } finally {
    loading.value = false
    loadingMore.value = false
  }
}

// 滚动事件处理
const handleScroll = () => {
  const scrollTop = document.documentElement.scrollTop || document.body.scrollTop
  const scrollHeight = document.documentElement.scrollHeight || document.body.scrollHeight
  const clientHeight = document.documentElement.clientHeight || window.innerHeight
  
  // 当滚动到距离底部100px时加载更多
  if (scrollTop + clientHeight >= scrollHeight - 100 && !loadingMore.value && hasMore.value) {
    loadPosts(true)
  }
}

// 初始化
onMounted(() => {
  loadPosts()
  window.addEventListener('scroll', handleScroll)
})

// 下拉刷新相关方法
const handleTouchStart = (e) => {
  // 只有在页面顶部才能下拉刷新
  const scrollTop = document.documentElement.scrollTop || document.body.scrollTop
  if (scrollTop === 0) {
    startY.value = e.touches[0].clientY
    isPulling.value = true
  }
}

const handleTouchMove = (e) => {
  if (!isPulling.value || isRefreshing.value) return
  
  const currentY = e.touches[0].clientY
  const distance = currentY - startY.value
  
  if (distance > 0) {
    // 阻止默认滚动
    e.preventDefault()
    // 计算下拉距离，添加阻尼效果
    pullDistance.value = Math.min(distance * 0.5, refreshThreshold * 1.5)
    
    // 更新提示文本
    if (pullDistance.value < refreshThreshold) {
      refreshText.value = '下拉刷新'
    } else {
      refreshText.value = '释放刷新'
    }
  }
}

const handleTouchEnd = () => {
  if (!isPulling.value) return
  
  if (pullDistance.value >= refreshThreshold) {
    // 触发刷新
    refresh()
  } else {
    // 重置状态
    resetPullState()
  }
  
  isPulling.value = false
}

const refresh = async () => {
  isRefreshing.value = true
  refreshText.value = '刷新中...'
  
  try {
    // 重新加载数据
    await loadPosts()
  } catch (error) {
    console.error('刷新失败:', error)
  } finally {
    // 延迟重置状态，让用户看到刷新动画
    setTimeout(() => {
      resetPullState()
      isRefreshing.value = false
    }, 500)
  }
}

const resetPullState = () => {
  pullDistance.value = 0
  refreshText.value = '下拉刷新'
}

// 清理
onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.feed-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 20px;
  overflow-x: hidden;
  touch-action: pan-y;
}

/* 下拉刷新 */
.pull-refresh {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  transition: all 0.3s ease;
}

.pull-refresh-content {
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.pull-refresh-icon {
  font-size: 20px;
  color: #9b59b6;
  margin-right: 8px;
  transition: all 0.3s ease;
}

.pull-refresh-icon.rotating {
  animation: rotate 1s linear infinite;
}

@keyframes rotate {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.pull-refresh-text {
  font-size: 14px;
  color: #666;
}

.feed-content {
  transition: transform 0.3s ease;
}



.floating-post-btn {
  position: fixed;
  bottom: 30px;
  right: 30px;
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  border: none;
  border-radius: 50%;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 32px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(155, 89, 182, 0.4);
  z-index: 1000;
  line-height: 1;
}

.floating-post-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 20px rgba(155, 89, 182, 0.6);
}

.post-icon {
  font-weight: bold;
  display: inline-block;
  line-height: 1;
  margin-top: -2px;
}

.feed-content {
  max-width: 600px;
  margin: 0 auto;
}

.feed-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.feed-item {
  background-color: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.feed-item:hover {
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

.post-user {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 15px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.post-user:hover {
  transform: translateX(5px);
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
  transition: all 0.3s ease;
}

.post-user:hover .user-avatar {
  transform: scale(1.1);
}

.user-info {
  flex: 1;
}

.username {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin: 0 0 4px 0;
}

.post-time {
  font-size: 12px;
  color: #999;
  margin: 0;
}

.post-content {
  margin-bottom: 15px;
}

.post-text {
  font-size: 14px;
  color: #333;
  line-height: 1.5;
  margin: 0 0 12px 0;
}

.post-images {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  gap: 8px;
  margin-bottom: 12px;
}

.post-image {
  width: 100%;
  height: 120px;
  object-fit: cover;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.post-image:hover {
  transform: scale(1.05);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  cursor: pointer;
}

.image-preview-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px 0px;
}

.preview-image {
  max-width: 100%;
  max-height: 80vh;
  object-fit: contain;
  border-radius: 8px;
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

.post-actions {
  display: flex;
  gap: 20px;
  padding-top: 15px;
  border-top: 1px solid #f0f0f0;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  background: none;
  border: none;
  font-size: 14px;
  color: #666;
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 6px 0;
}

.action-btn:hover {
  color: #9b59b6;
  transform: translateY(-2px);
}

.delete-btn:hover {
  color: #ff4d4f;
}

.action-icon {
  font-size: 16px;
}

.comments-section {
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #f0f0f0;
}

.comments-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 15px;
}

.comment-item {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  position: relative;
}

.comment-user {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  flex: 1;
  cursor: pointer;
  transition: all 0.3s ease;
}

.comment-user:hover {
  transform: translateX(5px);
}

.comment-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 600;
  color: white;
  flex-shrink: 0;
}

.comment-info {
  flex: 1;
}

.comment-username {
  font-size: 13px;
  font-weight: 600;
  color: #333;
  margin: 0 0 4px 0;
}

.comment-delete-btn {
  position: absolute;
  right: 10px;
  top: 10px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background-color: rgba(0, 0, 0, 0.6);
  color: white;
  border: none;
  font-size: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  opacity: 0;
}

.comment-item:hover .comment-delete-btn {
  opacity: 1;
}

.comment-delete-btn:hover {
  background-color: rgba(255, 0, 0, 0.8);
  transform: scale(1.1);
}

.comment-text {
  font-size: 13px;
  color: #666;
  line-height: 1.4;
  margin: 0;
}

.comment-input-container {
  display: flex;
  gap: 10px;
  align-items: center;
}

.comment-input {
  flex: 1;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 20px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.comment-input:focus {
  outline: none;
  border-color: #9b59b6;
}

.comment-submit {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  border: none;
  border-radius: 20px;
  padding: 8px 16px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(155, 89, 182, 0.4);
}

.comment-submit:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(155, 89, 182, 0.6);
}

/* 加载状态 */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(155, 89, 182, 0.2);
  border-left-color: #9b59b6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

.loading-spinner.small {
  width: 24px;
  height: 24px;
  border-width: 3px;
  margin-bottom: 8px;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.loading-container p {
  font-size: 14px;
  color: #666;
  margin: 0;
}

/* 加载更多状态 */
.loading-more {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px;
  text-align: center;
  margin-top: 10px;
}

.loading-more p {
  font-size: 14px;
  color: #666;
  margin: 0;
}

/* 没有更多数据 */
.no-more {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
  text-align: center;
  margin-top: 10px;
}

.no-more p {
  font-size: 14px;
  color: #999;
  margin: 0;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .feed-container {
    padding: 10px;
  }
  
  .feed-item {
    padding: 15px;
  }
  
  .user-avatar {
    width: 40px;
    height: 40px;
    font-size: 18px;
  }
  
  .post-images {
    grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  }
  
  .post-image {
    height: 100px;
  }
}
</style>