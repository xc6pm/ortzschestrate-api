<script setup lang="ts">
import { GameProcessor } from "~/models/GameProcessor"
import type { Color } from "~/models/Piece"

const { game, showCapturedBy } = defineProps<{
  game: GameProcessor
  showCapturedBy: Color
}>()

const capturedPieces = computed(() =>
  showCapturedBy === "white"
    ? game.moveHistory[game.moveHistory.length - 1].whiteCaptures
    : game.moveHistory[game.moveHistory.length - 1].blackCaptures
)
</script>

<template>
  <div>
    <img
      v-for="capturedPiece in capturedPieces"
      :src="'/pieces/' + capturedPiece.color[0] + capturedPiece.type + '.png'"
      :alt="capturedPiece.color + ' ' + capturedPiece.type"
      style="width: 2rem;"
      class="inline-block"
    />
  </div>
</template>
