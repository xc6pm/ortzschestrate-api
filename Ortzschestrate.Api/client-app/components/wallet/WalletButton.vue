<script setup lang="ts">
import { createAppKit, type ThemeMode } from "@reown/appkit/vue"
import { useAccount } from "@wagmi/vue"
import { networks } from "~/web3/wagmiConfig"
import AccountButton from "./AccountButton.vue"

const account = useAccount()
console.log("connected", account.isConnected.value)

const { wagmiAdapter, projectId } = useWagmi()

wagmiAdapter.on("disconnect", (arg) => {
  console.log("disconnect", arg)
})
wagmiAdapter.on("pendingTransactions", (arg) => {
  console.log("pendingTransactions", arg)
})
wagmiAdapter.on("switchNetwork", (arg) => {
  console.log("switchNetwork", arg)
})

const metadata = {
  name: "AppKit",
  description: "AppKit Example",
  url: "https://localhost:3000", // origin must match your domain & subdomain
  icons: ["https://avatars.githubusercontent.com/u/179229932"],
}

const colorMode = useColorMode()
// 5. Create the modal
const modal = createAppKit({
  adapters: [wagmiAdapter],
  networks,
  projectId,
  metadata,
  features: {
    analytics: true, // Optional - defaults to your Cloud configuration
    send: false,
    swaps: false,
    receive: false,
    onramp: false,
  },
  themeMode: colorMode.value as ThemeMode,

  themeVariables: {
    "--w3m-accent": "#435063",
    "--w3m-border-radius-master": ".85px",
  },
})
</script>

<template>
  <appkit-connect-button v-if="!account.isConnected.value" size="sm" color="oxford-blue" />
  <AccountButton v-else />
</template>
