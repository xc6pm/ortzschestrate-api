<script setup lang="ts">
import { Game } from "~/models/Game"
import CapturedPieces from "./CapturedPieces.vue"

const { game } = defineProps({
  game: { type: Game, required: true },
})
</script>

<template>
  <section style="display: flex; flex-direction: column">
    <div class="inline-block mx-auto">
      <CapturedPieces
        class="block my-1"
        :game="game"
        show-captured-by="white"
      />
      <ChessBoard :game="game" style="display: inline-block" />
      <MoveHistory
        :move-history="game.moveHistory"
        style="display: inline-block"
      />
      <CapturedPieces class="block" :game="game" show-captured-by="black" />
    </div>
  </section>
</template>
