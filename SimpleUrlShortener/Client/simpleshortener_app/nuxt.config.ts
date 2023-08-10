// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  modules: ['@nuxtjs/tailwindcss', '@pinia/nuxt'],
  typescript: {
    typeCheck: true,
  },
  imports: {
    dirs: ['./stores'],
  },
  pinia: {
    autoImports: [
      // automatically imports `defineStore` ==> https://pinia.vuejs.org/ssr/nuxt.html#Auto-imports
      'defineStore', // import { defineStore } from 'pinia'
      ['defineStore', 'definePiniaStore'], // import { defineStore as definePiniaStore } from 'pinia',
    ],
  },
  runtimeConfig: {
    public: {
      api: '/api',
    },
  },
  css: ['vue-toast-notification/dist/theme-default.css'],
});
