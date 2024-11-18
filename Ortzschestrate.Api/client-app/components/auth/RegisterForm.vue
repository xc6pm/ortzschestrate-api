<script setup lang="ts">
const config = useRuntimeConfig()

const registerUrl = config.public.apiUrl + "/auth/register"

const email = ref("")
const username = ref("")
const password = ref("")
const confirmedPassword = ref("")
const errorBox = useTemplateRef("errorBox")

const userStore = useUserStore()

const tryRegister = async () => {
  if (password.value !== confirmedPassword.value) {
    errorBox.value!.showTransient("Retype your passwords")
    return
  }

  try {
    const res = await $fetch.raw(registerUrl, {
      method: "POST",
      body: JSON.stringify({
        email: email.value,
        password: password.value,
        username: username.value,
      }),
      credentials: "include",
    })

    if (res.status !== 200) {
      if (res.status === 409) {
        errorBox.value!.showTransient("User already exists!")
      } else {
        errorBox.value!.showTransient("Registration failed!")
      }
      return
    }

    await userStore.fetch()
    await navigateTo("/")
  } catch (ex) {
    console.log("register exception:", ex)
    errorBox.value!.showTransient("Registration failed!")
  }
}
</script>

<template>
  <form method="post" @submit.prevent="tryRegister">
    <h3>Register</h3>

    <p>
      <input
        type="email"
        autocomplete="off"
        autofocus
        placeholder="Email address"
        name="email"
        v-model="email"
      />
    </p>
    <p>
      <input
        type="text"
        autocomplete="off"
        placeholder="Username"
        name="email"
        v-model="username"
      />
    </p>
    <p>
      <input
        type="password"
        autocomplete="off"
        placeholder="Password"
        name="password"
        v-model="password"
      />
    </p>
    <p>
      <input
        type="password"
        autocomplete="off"
        placeholder="Confirm password"
        v-model="confirmedPassword"
      />
    </p>

    <button type="submit">Register</button>

    <ErrorBox ref="errorBox" />
  </form>
</template>

<style scoped></style>
