import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': { // Zmiana z '/diary' na '/api' (obejmuje wszystkie kontrolery)
        target: 'http://localhost:5128',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})