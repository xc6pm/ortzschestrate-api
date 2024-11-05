<script setup lang="ts">
const config = useRuntimeConfig()

const loginUrl = config.public.apiUrl + "/api/auth/login"

const email = ref("")
const password = ref("")
const error = ref("")

function transientError(value: string, secondsToLast: number = 5) {
  error.value = value
  setTimeout(() => {
    error.value = ""
  }, secondsToLast * 1000)
}

async function tryLogin(evt) {
  evt.preventDefault()
  try {
    const res = await $fetch(loginUrl, {
      method: "POST",
      headers: {authorization: `Basic ${btoa(email.value + ":" + password.value)}`}
    })

    if (res.status !== 200) {
      transientError("Email or password invalid!")
    }

    navigateTo("/")
  } catch (ex) {
    transientError("Email or password invalid!")
  }
}
</script>

<template>
  <h2>Login</h2>

  <form method="post" enctype="application/x-www-form-urlencoded" @submit="tryLogin">
    <p>
      <input type="email" autocomplete="off" autofocus="autofocus" placeholder="Email address" name="email"
             v-model="email"/>
    </p>
    <p>
      <input type="password" autocomplete="off" autofocus="autofocus" placeholder="Password" name="password"
             v-model="password"/>
    </p>

    <button type="submit">Login</button>
  </form>

  <NuxtLink to="https://localhost:7132/api/auth/google">Login with Google</NuxtLink>
  
  <p v-if="error" style="color: red">{{ error }}</p>
</template>

<style scoped>

</style>