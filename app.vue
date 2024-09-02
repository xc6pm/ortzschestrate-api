<script setup>
import ChessBoard from './components/ChessBoard.vue';
import { useGamesStore } from './stores/GameStore';

const games = useGamesStore()
const newGame = games.newGame()
</script>

<template>
  <div>
    <ChessBoard :game="newGame"/>
  </div>
</template>
