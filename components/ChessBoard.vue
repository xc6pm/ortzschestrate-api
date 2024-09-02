<script setup lang="ts">
import { Game } from "~/models/Game"
import Sqr from "./Sqr.vue"
import { initBoard } from "~/models/Board"
import type { Sqr as SqrType } from "~/models/Square"

const { game } = defineProps({
  game: { type: Game, required: true },
})

// Reverse to have a1 at the bottom for playing white
const sqrs = game.board

const moveTargets: globalThis.Ref<SqrType[]> = ref([])

const onSqrClicked = (clickedSqr: SqrType) => {
  if (clickedSqr.piece) {
    moveTargets.value = game.findMoves(clickedSqr)
  } else {
    moveTargets.value = []
  }
}
</script>

<template>
  <!--Must render rows in reverse since the array is initialized with a1 as the start-->
  <div v-for="n in [...Array(8).keys()].reverse()">
    <Sqr
      v-for="sqr in sqrs.slice(8 * n, 8 * (n + 1))"
      :sqr="sqr"
      :move-target="moveTargets.includes(sqr)"
      v-on:click="onSqrClicked"
    />
  </div>
</template>
