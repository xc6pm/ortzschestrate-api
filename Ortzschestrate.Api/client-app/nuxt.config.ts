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
  nitro: {
    devProxy: {
      "/weatherforecast": {
        target: "https://localhost:7132/weatherforecast",
        changeOrigin: false,
        secure: false,
      },
    },
  },
});
