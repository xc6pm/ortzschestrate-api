<script setup lang="ts">
const userResponse = await useFetch("/api/auth/user")
const user = userResponse.data.value
console.log("user", user)
if (!user) {
  console.log("no user found")
  await navigateTo("/login")
}


</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  <h4>Create an account to start playing.</h4>
  
  {{ user }}
</template>
