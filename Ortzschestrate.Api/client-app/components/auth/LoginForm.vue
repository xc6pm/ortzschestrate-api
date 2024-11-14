<script setup lang="ts">
const config = useRuntimeConfig()

const loginUrl = config.public.apiUrl + "/auth/login"

const emailOrUsername = ref("")
const password = ref("")
const errorBox = useTemplateRef("errorBox")

const tryLogin = async () => {
  const enteredValueIsEmail = isValidEmail(emailOrUsername.value)

  try {
    const res = await $fetch.raw(loginUrl, {
      method: "POST",
      body: JSON.stringify({
        password: password.value,
        email: enteredValueIsEmail ? emailOrUsername.value : "",
        username: !enteredValueIsEmail ? emailOrUsername.value : "",
      }),
      credentials: "include",
    })

    if (res.status !== 200) {
      console.log("Login result:", res)
      errorBox.value!.showTransient("Email or password invalid!")
      return
    }

    navigateTo("/")
  } catch (ex) {
    console.log("login exception:", ex)
    errorBox.value!.showTransient("Email or password invalid!")
  }
}
</script>

<template>
  <form method="post" @submit.prevent="tryLogin">
    <h3>Login</h3>

    <p>
      <input
        type="text"
        autocomplete="off"
        placeholder="Email or username"
        v-model="emailOrUsername"
        autofocus
        required
      />
    </p>
    <p>
      <input
        type="password"
        autocomplete="off"
        placeholder="Password"
        v-model="password"
        required
      />
    </p>

    <button type="submit">Login</button>
  </form>

  <NuxtLink to="https://localhost:7132/api/auth/google"
    >Login with Google</NuxtLink
  >

  <ErrorBox ref="errorBox" />
</template>

<style scoped></style>
