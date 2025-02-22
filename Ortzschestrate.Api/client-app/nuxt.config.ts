// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: "2024-04-03",
  devtools: { enabled: true },

  devServer: {
    host: "localhost",
    https: {
      key: "../localhost-key.pem",
      cert: "../localhost.pem",
    },
  },

  runtimeConfig: {
    public: {
      apiUrl: process.env.NUXT_API_URL,
      reownProjectId: process.env.REOWN_PROJECT_ID
    },
  },

  nitro: {
    devProxy: {
      "/api/**": {
        target: "https://localhost:7132/api/**",
        secure: true,
      },
    },
  },

  vite: {
    optimizeDeps: {
      // https://github.com/wevm/wagmi/issues/3977
      include: ["eventemitter3"],
    },
  },

  ui: {
    safelistColors: ["oxford-blue", "fiord"],
  },

  vue: {
    compilerOptions: {
      isCustomElement: (tag) => ["appkit-connect-button"].includes(tag),
    },
  },

  ssr: false,
  plugins: ["~/plugins/connectWallet.client.ts"],
  modules: ["@pinia/nuxt", "@nuxt/ui", "@nuxt/icon"],
})
