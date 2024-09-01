<script setup lang="ts">
import { Sqr as SqrType } from "~/models/Square"

const { sqr } = defineProps({
  sqr: { type: SqrType, required: true },
})
</script>

<template>
  <button v-if="sqr" :class="sqr.color">
    <img
      v-if="sqr.piece"
      :src="'/pieces/' + sqr.piece.color[0] + sqr.piece.type + '.png'"
      :alt="sqr.piece.color + ' ' + sqr.piece.type"
    />
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

img {
  width: 80px;
  height: 80px;
}
</style>
