import { QueryClient, VueQueryPlugin } from "@tanstack/vue-query"
import { WagmiPlugin } from "@wagmi/vue"
import { config } from "~/web3/wagmiConfig"

export default defineNuxtPlugin((nuxtApp) => {
  const queryClient = new QueryClient()
  nuxtApp.vueApp.use(WagmiPlugin, { config }).use(VueQueryPlugin, { queryClient })
})
