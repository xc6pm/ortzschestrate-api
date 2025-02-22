import { defineChain, type AppKitNetwork } from "@reown/appkit/networks"
import { http, createConfig } from "@wagmi/vue"
import { sepolia } from "@wagmi/vue/chains"

const hardhatLocalhost = defineChain({
  id: 31_337,
  chainNamespace: "eip155",
  caipNetworkId: "eip155:31337",
  name: "Localhost",
  nativeCurrency: {
    decimals: 18,
    name: "Ether",
    symbol: "ETH",
  },
  rpcUrls: {
    default: { http: ["http://127.0.0.1:8545"] },
  },
  testnet: true,
})

const config = createConfig({
  chains: [sepolia, hardhatLocalhost],
  transports: {
    [sepolia.id]: http(),
    [hardhatLocalhost.id]: http(),
  },
})

const networks: [AppKitNetwork, ...AppKitNetwork[]] = [hardhatLocalhost, sepolia]

export { hardhatLocalhost, config, networks }

declare module "@wagmi/vue" {
  interface Register {
    config: typeof config
  }
}
