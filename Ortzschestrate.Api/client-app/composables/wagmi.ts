import { WagmiAdapter } from "@reown/appkit-adapter-wagmi"
import { networks } from "~/web3/wagmiConfig"

export const useWagmi = () => {
  const projectId = useRuntimeConfig().public.reownProjectId
  console.log("reown project id ", projectId)
  if (!projectId) throw new Error("The wallet button requires a reown project id to work.")

  const wagmiAdapter = new WagmiAdapter({
    networks,
    projectId,
  })

  return { wagmiAdapter, projectId }
}
