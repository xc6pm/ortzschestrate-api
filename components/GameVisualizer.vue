<script setup lang="ts">
import { GameProcessor } from "~/models/GameProcessor"
import CapturedPieces from "./CapturedPieces.vue"

const { game } = defineProps({
  game: { type: GameProcessor, required: true },
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

      <p>
        {{ game.moveHistory[game.moveHistory.length - 1].isWhiteChecked }} -
        {{ game.moveHistory[game.moveHistory.length - 1].isWhiteCheckmated }}
      </p>
      <p>
        {{ game.moveHistory[game.moveHistory.length - 1].isBlackChecked }} -
        {{ game.moveHistory[game.moveHistory.length - 1].isBlackCheckmated }}
      </p>
    </div>
  </section>
</template>
