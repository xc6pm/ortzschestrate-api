<script setup lang="ts">
let userStore = useUserStore()
await userStore.fetch()

if (!userStore.user) {
  await navigateTo("/login")
}
</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  {{ userStore.user }}
</template>
