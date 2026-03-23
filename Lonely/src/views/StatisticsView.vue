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
        <!-- 时间范围选择 -->
        <div class="time-range-selector">
          <ButtonGroup>
            <Button :type="timeRange === 'day' ? 'primary' : 'default'" @click="timeRange = 'day'">日</Button>
            <Button :type="timeRange === 'week' ? 'primary' : 'default'" @click="timeRange = 'week'">周</Button>
            <Button :type="timeRange === 'month' ? 'primary' : 'default'" @click="timeRange = 'month'">月</Button>
          </ButtonGroup>
        </div>
        
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
        
        <!-- 详细数据列表 -->
        <div class="data-table">
          <h3>每日详细数据</h3>
          <Table :columns="columns" :data-source="tableData" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { Button, ButtonGroup, Table, message } from 'ant-design-vue'
import * as echarts from 'echarts'
import { statsApi } from '../services/api'
import PageHeader from '../components/PageHeader.vue'

const router = useRouter()

// 权限控制
const hasAccess = computed(() => {
  const phone = localStorage.getItem('phone')
  return phone === '18860423687'
})

// 时间范围
const timeRange = ref('day')

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

// 图表引用
const visitChartRef = ref(null)
const functionChartRef = ref(null)

// 表格数据
const columns = [
  {
    title: '日期',
    dataIndex: 'date',
    key: 'date'
  },
  {
    title: '访问次数',
    dataIndex: 'visits',
    key: 'visits'
  },
  {
    title: '扔瓶子次数',
    dataIndex: 'bottles',
    key: 'bottles'
  },
  {
    title: '树洞次数',
    dataIndex: 'treeHoles',
    key: 'treeHoles'
  },
  {
    title: '动态发布次数',
    dataIndex: 'posts',
    key: 'posts'
  }
]

const tableData = ref([
  { key: '1', date: '2026-03-23', visits: 120, bottles: 30, treeHoles: 25, posts: 15 },
  { key: '2', date: '2026-03-22', visits: 110, bottles: 28, treeHoles: 22, posts: 12 },
  { key: '3', date: '2026-03-21', visits: 130, bottles: 32, treeHoles: 28, posts: 18 },
  { key: '4', date: '2026-03-20', visits: 95, bottles: 25, treeHoles: 20, posts: 10 },
  { key: '5', date: '2026-03-19', visits: 105, bottles: 27, treeHoles: 23, posts: 14 }
])

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

// 加载数据
const loadData = async () => {
  try {
    // 根据时间范围获取数据
    if (timeRange.value === 'day') {
      // 获取每日统计数据
      const dailyResponse = await statsApi.getDailyStats(30)
      if (dailyResponse.success) {
        const dailyData = dailyResponse.data
        // 计算总计
        totalVisits.value = dailyData.reduce((sum, item) => sum + item.visitCount, 0)
        totalBottles.value = dailyData.reduce((sum, item) => sum + item.bottleCount, 0)
        totalTreeHoles.value = dailyData.reduce((sum, item) => sum + item.treeHoleCount, 0)
        totalPosts.value = dailyData.reduce((sum, item) => sum + item.postCount, 0)
        
        // 更新表格数据
        tableData.value = dailyData.map((item, index) => ({
          key: (index + 1).toString(),
          date: item.date,
          visits: item.visitCount,
          bottles: item.bottleCount,
          treeHoles: item.treeHoleCount,
          posts: item.postCount
        }))
      }
      
      // 获取趋势数据
      const trendResponse = await statsApi.getTrendStats(30)
      if (trendResponse.success) {
        trendData.value = trendResponse.data
        initCharts()
      }
    } else if (timeRange.value === 'week') {
      // 获取每周统计数据
      const weeklyResponse = await statsApi.getWeeklyStats(12)
      if (weeklyResponse.success) {
        const weeklyData = weeklyResponse.data
        // 计算总计
        totalVisits.value = weeklyData.reduce((sum, item) => sum + item.visitCount, 0)
        totalBottles.value = weeklyData.reduce((sum, item) => sum + item.bottleCount, 0)
        totalTreeHoles.value = weeklyData.reduce((sum, item) => sum + item.treeHoleCount, 0)
        totalPosts.value = weeklyData.reduce((sum, item) => sum + item.postCount, 0)
        
        // 更新表格数据
        tableData.value = weeklyData.map((item, index) => ({
          key: (index + 1).toString(),
          date: item.week,
          visits: item.visitCount,
          bottles: item.bottleCount,
          treeHoles: item.treeHoleCount,
          posts: item.postCount
        }))
        
        // 构建趋势数据
        trendData.value = {
          dates: weeklyData.map(item => item.week),
          visitData: weeklyData.map(item => item.visitCount),
          bottleData: weeklyData.map(item => item.bottleCount),
          treeHoleData: weeklyData.map(item => item.treeHoleCount),
          postData: weeklyData.map(item => item.postCount)
        }
        initCharts()
      }
    } else if (timeRange.value === 'month') {
      // 获取每月统计数据
      const monthlyResponse = await statsApi.getMonthlyStats(6)
      if (monthlyResponse.success) {
        const monthlyData = monthlyResponse.data
        // 计算总计
        totalVisits.value = monthlyData.reduce((sum, item) => sum + item.visitCount, 0)
        totalBottles.value = monthlyData.reduce((sum, item) => sum + item.bottleCount, 0)
        totalTreeHoles.value = monthlyData.reduce((sum, item) => sum + item.treeHoleCount, 0)
        totalPosts.value = monthlyData.reduce((sum, item) => sum + item.postCount, 0)
        
        // 更新表格数据
        tableData.value = monthlyData.map((item, index) => ({
          key: (index + 1).toString(),
          date: item.month,
          visits: item.visitCount,
          bottles: item.bottleCount,
          treeHoles: item.treeHoleCount,
          posts: item.postCount
        }))
        
        // 构建趋势数据
        trendData.value = {
          dates: monthlyData.map(item => item.month),
          visitData: monthlyData.map(item => item.visitCount),
          bottleData: monthlyData.map(item => item.bottleCount),
          treeHoleData: monthlyData.map(item => item.treeHoleCount),
          postData: monthlyData.map(item => item.postCount)
        }
        initCharts()
      }
    }
  } catch (error) {
    console.error('加载统计数据失败:', error)
    message.error('加载统计数据失败，请稍后重试')
  }
}

// 监听时间范围变化
watch(timeRange, (newRange) => {
  loadData()
})

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

.time-range-selector {
  margin-bottom: 30px;
  text-align: right;
}

.statistics-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
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
}
</style>