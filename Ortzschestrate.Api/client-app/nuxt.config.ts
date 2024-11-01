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
    apiUrl: "https://localhost:7132",
  },
  nitro: {
    devProxy: {
      "/api/weatherforecast": {
        target: "https://localhost:7132/weatherforecast",
        secure: true,
      },
    },
  },
});
