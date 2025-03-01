import { useAccount } from "@wagmi/vue"
import { readContract } from "@wagmi/core"
import type { Deployment } from "~/types/Deployment"
import { config } from "~/web3/wagmiConfig"
import { formatEther, type Abi } from "viem"

export const useContractStateStore = defineStore("contractStateStore", () => {
  const deployment = ref<Deployment | null>(null)
  const account = useAccount()
  const stakesWei = ref(0n)
  const stakesEth = ref(0)

  const reload = async () => {
    deployment.value = await $fetch<any>("/deployment/ORTBet.json")
    return deployment.value
  }

  const getDeployment = async () => {
    if (deployment.value) return Promise.resolve(deployment.value)

    await reload()
    if (!deployment.value) throw new Error("Deployment not found!!!")

    return deployment.value
  }

  const updateBalance = async () => {
    if (!account?.address?.value) {
      stakesWei.value = 0n
      stakesEth.value = 0
      return
    }

    const deployment = await getDeployment()

    const data = await readContract(config, {
      abi: deployment.abi as Abi,
      address: deployment.address,
      functionName: "getBalance",
      args: [account.address.value],
    })
    stakesWei.value = data as bigint
    stakesEth.value = parseFloat(formatEther(stakesWei.value))
    return { stakesWei: stakesWei.value, stakesEth: stakesEth.value }
  }

  return { getDeployment, reload, updateBalance, stakesWei, stakesEth, account }
})
