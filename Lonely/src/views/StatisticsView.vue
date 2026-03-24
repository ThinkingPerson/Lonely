<template>
  <div class="statistics-container">
    <PageHeader title="数据统计" />
    
    <div class="statistics-content">
      <!-- 权限检查 -->
      <div v-if="!hasAccess" class="no-access">
        <h3>权限不足</h3>
        <p>您没有权限访问此页面</p>
        <button class="btn btn-primary" @click="goBack">返回</button>
      </div>
      
      <!-- 统计内容 -->
      <div v-else>
        <!-- 统计卡片 -->
        <div class="statistics-cards">
          <div class="card">
            <div class="card-title">总访问次数</div>
            <div class="card-value">{{ totalVisits }}</div>
          </div>
          <div class="card">
            <div class="card-title">扔瓶子次数</div>
            <div class="card-value">{{ totalBottles }}</div>
          </div>
          <div class="card">
            <div class="card-title">树洞次数</div>
            <div class="card-value">{{ totalTreeHoles }}</div>
          </div>
          <div class="card">
            <div class="card-title">动态发布次数</div>
            <div class="card-value">{{ totalPosts }}</div>
          </div>
        </div>
        
        <!-- 时间范围选择 -->
        <div class="time-range">
          <h3>时间范围</h3>
          <Radio.Group v-model="timeRange" @change="handleTimeRangeChange">
            <Radio.Button value="7">7天</Radio.Button>
            <Radio.Button value="30">30天</Radio.Button>
            <Radio.Button value="90">90天</Radio.Button>
          </Radio.Group>
        </div>
        
        <!-- 折线图 -->
        <div class="charts">
          <div class="chart-container">
            <h3>访问趋势</h3>
            <div class="chart" ref="visitChartRef"></div>
          </div>
          <div class="chart-container">
            <h3>功能使用趋势</h3>
            <div class="chart" ref="functionChartRef"></div>
          </div>
        </div>
        
        <!-- 所有用户统计数据 -->
        <div class="data-table">
          <h3>所有用户统计数据</h3>
          <Table :columns="userColumns" :data-source="userData" />
        </div>
        
        <!-- 每日详细数据 -->
        <div class="data-table">
          <h3>每日详细操作记录</h3>
          <Table :columns="dailyColumns" :data-source="dailyData" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import { useRouter } from 'vue-router'
import { Table, message, Radio } from 'ant-design-vue'
import * as echarts from 'echarts'
import { statsApi } from '../services/api'
import PageHeader from '../components/PageHeader.vue'

const router = useRouter()

// 注入全局用户状态
const userState = inject('userState')

// 检查是否为管理员账号
const hasAccess = computed(() => {
  return userState.value.userInfo && userState.value.userInfo.phone === '18860423687'
})

// 统计数据
const totalVisits = ref(0)
const totalBottles = ref(0)
const totalTreeHoles = ref(0)
const totalPosts = ref(0)

// 趋势数据
const trendData = ref({
  dates: [],
  visitData: [],
  bottleData: [],
  treeHoleData: [],
  postData: []
})

// 时间范围
const timeRange = ref('7')

// 图表引用
const visitChartRef = ref(null)
const functionChartRef = ref(null)

// 表格数据
const userColumns = [
  {
    title: '用户ID',
    dataIndex: 'userId',
    key: 'userId'
  },
  {
    title: '用户昵称',
    dataIndex: 'nickname',
    key: 'nickname'
  },
  {
    title: '日期',
    dataIndex: 'date',
    key: 'date'
  },
  {
    title: '操作类型',
    dataIndex: 'operationType',
    key: 'operationType'
  },
  {
    title: '操作次数',
    dataIndex: 'count',
    key: 'count'
  }
]

const userData = ref([])

const dailyColumns = [
  {
    title: 'ID',
    dataIndex: 'id',
    key: 'id'
  },
  {
    title: '用户ID',
    dataIndex: 'userId',
    key: 'userId'
  },
  {
    title: '用户昵称',
    dataIndex: 'nickname',
    key: 'nickname'
  },
  {
    title: '操作类型',
    dataIndex: 'operationType',
    key: 'operationType'
  },
  {
    title: '内容',
    dataIndex: 'content',
    key: 'content'
  },
  {
    title: '创建时间',
    dataIndex: 'createdAt',
    key: 'createdAt'
  }
]

const dailyData = ref([])

// 返回按钮
const goBack = () => {
  router.push('/main')
}

// 初始化图表
const initCharts = () => {
  // 访问趋势图表
  const visitChart = echarts.init(visitChartRef.value)
  const visitOption = {
    tooltip: {
      trigger: 'axis'
    },
    legend: {
      data: ['访问次数']
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: trendData.value.dates
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        name: '访问次数',
        type: 'line',
        data: trendData.value.visitData,
        smooth: true,
        lineStyle: {
          color: '#9b59b6'
        },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            {
              offset: 0,
              color: 'rgba(155, 89, 182, 0.3)'
            },
            {
              offset: 1,
              color: 'rgba(155, 89, 182, 0.1)'
            }
          ])
        }
      }
    ]
  }
  visitChart.setOption(visitOption)
  
  // 功能使用趋势图表
  const functionChart = echarts.init(functionChartRef.value)
  const functionOption = {
    tooltip: {
      trigger: 'axis'
    },
    legend: {
      data: ['扔瓶子', '树洞', '动态']
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: trendData.value.dates
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        name: '扔瓶子',
        type: 'line',
        data: trendData.value.bottleData,
        smooth: true,
        lineStyle: {
          color: '#3498db'
        }
      },
      {
        name: '树洞',
        type: 'line',
        data: trendData.value.treeHoleData,
        smooth: true,
        lineStyle: {
          color: '#2ecc71'
        }
      },
      {
        name: '动态',
        type: 'line',
        data: trendData.value.postData,
        smooth: true,
        lineStyle: {
          color: '#e74c3c'
        }
      }
    ]
  }
  functionChart.setOption(functionOption)
  
  // 响应式调整
  window.addEventListener('resize', () => {
    visitChart.resize()
    functionChart.resize()
  })
}

// 时间范围变化处理
const handleTimeRangeChange = () => {
  loadData()
}

// 加载数据
const loadData = async () => {
  try {
    const days = parseInt(timeRange.value)
    
    // 获取趋势数据
    const trendResponse = await statsApi.getTrendStats(days)
    if (trendResponse.success) {
      trendData.value = trendResponse.data
      // 计算总计
      totalVisits.value = trendData.value.visitData.reduce((sum, count) => sum + count, 0)
      totalBottles.value = trendData.value.bottleData.reduce((sum, count) => sum + count, 0)
      totalTreeHoles.value = trendData.value.treeHoleData.reduce((sum, count) => sum + count, 0)
      totalPosts.value = trendData.value.postData.reduce((sum, count) => sum + count, 0)
      initCharts()
    }
    
    // 获取所有用户统计数据
    const allUsersResponse = await statsApi.getAllUsersStats(days)
    if (allUsersResponse.success) {
      userData.value = allUsersResponse.data.map((item, index) => ({
        key: (index + 1).toString(),
        ...item
      }))
    }
    
    // 获取每日详细数据
    const dailyDetailsResponse = await statsApi.getDailyDetails()
    if (dailyDetailsResponse.success) {
      dailyData.value = dailyDetailsResponse.data.map((item, index) => ({
        key: (index + 1).toString(),
        ...item
      }))
    }
  } catch (error) {
    console.error('加载统计数据失败:', error)
    message.error('加载统计数据失败，请稍后重试')
  }
}

onMounted(() => {
  if (hasAccess.value) {
    loadData()
  }
})
</script>

<style scoped>
.statistics-container {
  width: 100%;
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 20px;
}

.statistics-content {
  max-width: 1200px;
  margin: 0 auto;
}

.no-access {
  background-color: white;
  border-radius: 12px;
  padding: 60px;
  text-align: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  margin-top: 100px;
}

.no-access h3 {
  font-size: 24px;
  color: #333;
  margin-bottom: 20px;
}

.no-access p {
  font-size: 16px;
  color: #666;
  margin-bottom: 30px;
}



.statistics-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.time-range {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;
}

.time-range h3 {
  font-size: 16px;
  color: #333;
  margin-bottom: 16px;
  font-weight: 600;
}

.card {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  text-align: center;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.card-title {
  font-size: 14px;
  color: #666;
  margin-bottom: 12px;
}

.card-value {
  font-size: 32px;
  font-weight: 600;
  color: #333;
}

.charts {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 30px;
  margin-bottom: 30px;
}

.chart-container {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.chart-container h3 {
  font-size: 16px;
  color: #333;
  margin-bottom: 20px;
  font-weight: 600;
}

.chart {
  height: 300px;
  background-color: #fafafa;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #999;
}

.data-table {
  background-color: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.data-table h3 {
  font-size: 16px;
  color: #333;
  margin-bottom: 20px;
  font-weight: 600;
}

/* 按钮样式 */
.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background: linear-gradient(135deg, #9b59b6 0%, #8e44ad 100%);
  color: white;
  box-shadow: 0 4px 15px rgba(155, 89, 182, 0.4);
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(155, 89, 182, 0.6);
}

/* 响应式设计 */
@media (max-width: 768px) {
  .statistics-container {
    padding: 10px;
  }
  
  .charts {
    grid-template-columns: 1fr;
  }
  
  .chart-container {
    padding: 16px;
  }
  
  .chart {
    height: 250px;
  }
  
  .statistics-cards {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .card {
    padding: 16px;
  }
  
  .card-value {
    font-size: 24px;
  }
  
  .time-range {
    padding: 16px;
  }
  
  .data-table {
    padding: 16px;
    overflow-x: auto;
  }
  
  .data-table h3 {
    font-size: 14px;
  }
  
  /* 表格响应式样式 */
  .ant-table {
    font-size: 12px;
  }
  
  .ant-table-thead > tr > th {
    padding: 8px;
    font-size: 12px;
  }
  
  .ant-table-tbody > tr > td {
    padding: 8px;
    font-size: 12px;
  }
  
  /* 隐藏部分列以适应手机屏幕 */
  .ant-table-tbody > tr > td:nth-child(3),
  .ant-table-thead > tr > th:nth-child(3) {
    display: none;
  }
  
  .ant-table-tbody > tr > td:nth-child(6),
  .ant-table-thead > tr > th:nth-child(6) {
    display: none;
  }
}
</style>