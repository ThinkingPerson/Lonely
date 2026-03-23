import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'
import { copyFileSync, mkdirSync, existsSync, readdirSync, statSync } from 'fs'

// 复制目录函数
function copyDir(src, dest) {
  if (!existsSync(dest)) {
    mkdirSync(dest, { recursive: true })
  }
  
  const files = readdirSync(src)
  files.forEach(file => {
    const srcPath = resolve(src, file)
    const destPath = resolve(dest, file)
    const stat = statSync(srcPath)
    
    if (stat.isDirectory()) {
      copyDir(srcPath, destPath)
    } else {
      copyFileSync(srcPath, destPath)
    }
  })
}

// https://vite.dev/config/
export default defineConfig({
  publicDir: false, // 禁用默认的public目录处理
  plugins: [
    vue(),
    {
      name: 'copy-public-folder',
      closeBundle() {
        const publicDir = resolve(__dirname, 'public')
        const distPublicDir = resolve(__dirname, 'dist', 'public')
        
        if (existsSync(publicDir)) {
          copyDir(publicDir, distPublicDir)
          console.log('✅ Public directory copied to dist/public')
        }
      }
    }
  ],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src')
    }
  },
  server: {
    host: true, // 允许局域网访问
    proxy: {
      '/api': {
        target: 'http://localhost:5264',
        changeOrigin: true,
        secure: false
      }
    }
  },
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['vue', 'vue-router', 'ant-design-vue'],
          axios: ['axios']
        }
      }
    }
  }
})