import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
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
  }
})