<script setup>
import { useGamesStore } from "./stores/GameStore"

const games = useGamesStore()
const newGame = games.newGame()
</script>

<template>
  <div>
    <Game :game="newGame" />
  </div>
</template>
