<script setup lang="ts">
const { run, duration } = defineProps(["run", "duration"])

const msRemaining = ref(duration)
const formatted = computed(() => formatMilliseconds(msRemaining.value))

let intervalId: NodeJS.Timeout | null = null

const runCountdown = () => {
  msRemaining.value -= 1000
  if (msRemaining.value <= 0) {
    emit("timeout")
    if (intervalId) clearInterval(intervalId)
  }
}

if (run) {
  intervalId = setInterval(runCountdown, 1000)
}
watch(
  () => run,
  (newValue) => {
    console.log("inside watch")
    if (newValue) {
      console.log("value is true")
      intervalId = setInterval(runCountdown, 1000)
    } else {
      if (intervalId) clearInterval(intervalId)
    }
  }
)

defineExpose({
  syncWithServer: (remainingTimeFromServer: number) => {
    console.log("timer syncing", msRemaining.value, remainingTimeFromServer)
    msRemaining.value = remainingTimeFromServer
  },
})

const emit = defineEmits(["timeout"])
</script>

<template>
  <span>{{ formatted }}</span>
</template>
