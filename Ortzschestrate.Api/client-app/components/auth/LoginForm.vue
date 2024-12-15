<script setup lang="ts">
import type { FormSubmitEvent } from "#ui/types"

const config = useRuntimeConfig()

const loginUrl = apiUrl("/auth/login")
const googleLoginUrl =
  config.public.apiUrl +
  "/auth/google?" +
  new URLSearchParams({ redirect: window.location.protocol + "//" + window.location.host })

const state = reactive({
  emailOrUsername: "",
  password: "",
})
const toast = useToast()
const tryingLogin = ref(false)

const userStore = useUserStore()

const tryLogin = async (event: FormSubmitEvent<any>) => {
  event.preventDefault()

  tryingLogin.value = true
  const enteredValueIsEmail = isValidEmail(state.emailOrUsername)

  try {
    const res = await $fetch.raw(loginUrl, {
      method: "POST",
      body: JSON.stringify({
        password: state.password,
        email: enteredValueIsEmail ? state.emailOrUsername : "",
        username: !enteredValueIsEmail ? state.emailOrUsername : "",
      }),
      credentials: "include",
    })

    if (res.status !== 200) {
      console.log("Login result:", res)
      toast.add({
        description: "Login failed! Invalid credentials.",
        color: "red",
        timeout: 5000,
        icon: "i-heroicons-x-circle",
      })
      return
    }

    await userStore.fetch()
    await navigateTo("/")
  } catch (ex) {
    console.log("login exception:", ex)
    toast.add({
      description: "Login failed! Invalid credentials.",
      color: "red",
      timeout: 5000,
      icon: "i-heroicons-x-circle",
    })
  } finally {
    tryingLogin.value = false
  }
}
</script>

<template>
  <UForm @submit="tryLogin" class="px-3 pt-3" :state="state">
    <UFormGroup label="Email or username:" name="emailOrUsername" class="mb-3">
      <UInput v-model="state.emailOrUsername" type="text" required autofocus :disabled="tryingLogin"/>
    </UFormGroup>
    <UFormGroup label="Password:" name="password" class="mb-5">
      <UInput v-model="state.password" type="password" required :disabled="tryingLogin"/>
    </UFormGroup>

    <UButton type="submit" block class="mb-3" :loading="tryingLogin" size="lg">Login</UButton>

    <UButton :to="googleLoginUrl" color="white" block :disabled="tryingLogin" size="lg">Login with Google</UButton>
  </UForm>
</template>

<style scoped></style>
