<script setup lang="ts">
import { Sqr as SqrType } from "~/models/Square"

const { sqr } = defineProps({
  sqr: { type: SqrType, required: true },
  moveTarget: { type: Boolean, required: true },
})
const emit = defineEmits<{ click: [s: SqrType] }>()
</script>

<template>
  <button v-if="sqr" :class="sqr.color" @click="() => emit('click', sqr)">
    <img
      v-if="sqr.piece"
      :src="'/pieces/' + sqr.piece.color[0] + sqr.piece.type + '.png'"
      :alt="sqr.piece.color + ' ' + sqr.piece.type"
    />
    <div class="moveTg" v-if="moveTarget"></div>
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

.moveTg {
  border-radius: 50%;
  background-color: black;
  opacity: 0.5;
  width: 35px;
  height: 35px;
  margin-left: auto;
  margin-right: auto;
}
</style>
