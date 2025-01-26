export type User = { id: string; userName: string; email: string, verifiedWallet: string }

export const useUserStore = defineStore("userStore", () => {
  const user = ref<User | null>(null)

  const config = useRuntimeConfig()

  const fetch = async () => {
    try {
      const fetchResult = await $fetch<User>(config.public.apiUrl + "/auth/user", {
        credentials: "include",
      })
      
      if (fetchResult) {
        user.value = fetchResult
      }
    } catch (error) {
      if (error.message.includes("401")) {
        user.value = null
      }
    }
  }

  return { user, fetch }
})
