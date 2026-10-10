import { fileURLToPath } from 'node:url'
import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vitest/config'

// A dedicated config (rather than reusing vite.config.ts) so tests do not pull
// in Tailwind or the Vue DevTools plugin.
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  test: {
    environment: 'jsdom',
    root: fileURLToPath(new URL('./', import.meta.url)),
    include: ['src/**/__tests__/*.spec.ts'],
  },
})
