import type { Deployment } from "~/types/Deployment"

export const useDeploymentStore = defineStore("deploymentStore", () => {
  const deployment = ref<Deployment | null>(null)

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

  return { getDeployment, reload }
})
