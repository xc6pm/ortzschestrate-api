<script setup lang="ts">
const errorMessage = ref(null)

const showTransient = (message: string, secondsToLast: number = 5) => {
  errorMessage.value = message
  setTimeout(() => {
    errorMessage.value = ""
  }, secondsToLast * 1000)
}

defineExpose({showTransient})
</script>

<template v-if="errorMessage">
  <p style="color: red">{{ errorMessage }}</p>
</template>

<style scoped>

</style>