<script setup lang="ts">
import { Sqr as SqrType } from "~/models/Square"

const props = defineProps({
  sqr: { type: SqrType, required: true },
})
</script>

<template>
  <button v-if="props.sqr" :class="props.sqr.color">
    <slot></slot>
  </button>
</template>

<style scoped>
button {
  width: 100px;
  height: 100px;
  border-style: none;
  margin-bottom: -4px;
  margin-top: -4px;
}

.white {
  background-color: gainsboro;
}

.black {
  background-color: indigo;
}
</style>
