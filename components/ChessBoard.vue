<script setup lang="ts">
import { Game } from "~/models/Game"
import Sqr from "./Sqr.vue"
import type { Sqr as SqrType } from "~/models/Square"

const { game } = defineProps({
  game: { type: Game, required: true },
})

const sqrInFocus = ref<SqrType | null>(null)
const moveTargets = ref<SqrType[]>([])

const onSqrClicked = (clickedSqr: SqrType) => {
  if (moveTargets.value.includes(clickedSqr) && sqrInFocus.value) {
    game.movePiece(sqrInFocus.value, clickedSqr)
    sqrInFocus.value = null
    moveTargets.value = []
    return
  }

  if (clickedSqr.piece) {
    sqrInFocus.value = clickedSqr
    moveTargets.value = game.findMoves(clickedSqr)
  } else {
    sqrInFocus.value = null
    moveTargets.value = []
  }
}
</script>

<template>
  <!--Must render rows in reverse since the array is initialized with a1 as the start-->
  <div v-for="n in [...Array(8).keys()].reverse()">
    <Sqr
      v-for="sqr in game.board.slice(8 * n, 8 * (n + 1))"
      :sqr="sqr"
      :move-target="moveTargets.includes(sqr)"
      v-on:click="onSqrClicked"
    />
  </div>
</template>
