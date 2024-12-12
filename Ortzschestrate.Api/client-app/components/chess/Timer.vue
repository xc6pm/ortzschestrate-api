<script setup lang="ts">
const { run, duration } = defineProps(["run", "duration"])

const msRemaining = ref(duration)
const formatted = computed(() => formatMilliseconds(msRemaining.value))

let intervalId: NodeJS.Timeout | null = null
if (run) {
  intervalId = setInterval(() => {
    msRemaining.value -= 1000
  }, 1000)
}
watch(
  () => run,
  (newValue) => {
    console.log("inside watch")
    if (newValue) {
      console.log("value is true")
      intervalId = setInterval(() => {
        msRemaining.value -= 1000
      }, 1000)
    } else {
      if (intervalId) clearInterval(intervalId)
    }
  }
)
</script>

<template>
  <span>{{ formatted }}</span>
</template>
