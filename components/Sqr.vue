<script setup lang="ts">
import { Sqr as SqrType } from "~/models/Square"

const { sqr, moveTarget } = defineProps({
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
  border-width: 0;
  margin-bottom: -5px;
  margin-top: -5px;
  padding: 0;
  position: relative;
}

.white {
  background-color: gainsboro;
}

.black {
  background-color: indigo;
}

img {
  width: 80%;
  height: 80%;
  position: absolute;
  margin: auto;
  left: 10%;
  right: 50%;
  top: 50%;
  bottom: 50%;
  text-align: center;
}

.moveTg {
  border-radius: 50%;
  background-color: black;
  opacity: 0.5;
  width: 35%;
  height: 35%;
  margin-left: auto;
  margin-right: auto;
}
</style>
