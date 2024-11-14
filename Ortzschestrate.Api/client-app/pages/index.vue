<script setup lang="ts">
const config = useRuntimeConfig()

let user = null
try {
  user = await $fetch(config.public.apiUrl + "/auth/user", {credentials: "include"})
  if (!user) {
    await navigateTo("/login")
  }
} catch (ex) {
  await navigateTo("/login")
}
</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  {{ user }}
</template>
