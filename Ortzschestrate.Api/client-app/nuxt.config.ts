// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
    compatibilityDate: "2024-04-03",
    devtools: {enabled: true},
    devServer: {
        host: "localhost",
        https: {
            key: "../localhost-key.pem",
            cert: "../localhost.pem",
        },
    },
    runtimeConfig: {
        public: {
            apiUrl: process.env.NUXT_API_URL
        }
    },
    nitro: {
        devProxy: {
            "/api/**": {
                target: "https://localhost:7132/api/**",
                secure: true,
            }
        },
    },
    ssr: false
});
