<script setup lang="ts">
const { data } = await useFetch("/api/weatherforecast", { server: true });
console.log("data: ", data);
</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  <h4>Create an account to start playing.</h4>

  {{ data }}
</template>
