<script setup lang="ts">
import { Game } from "~/models/Game"

const { game } = defineProps({
  game: { type: Game, required: true },
})
</script>

<template>
  <section style="display: flex">
    <div style="display: inline-block; margin: 0 auto">
      <ChessBoard :game="game" style="display: inline-block" />
      <MoveHistory
        :move-history="game.moveHistory"
        style="display: inline-block;"
      />
    </div>
  </section>
</template>
