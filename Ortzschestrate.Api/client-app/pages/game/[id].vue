<script setup lang="ts">
import { BoardApi, TheChessboard, type BoardConfig } from "vue3-chessboard"
import "vue3-chessboard/style.css"
import type { Game } from "~/types/Game"

const route = useRoute()
const gameId = route.params.id

const connectionStore = useConnectionStore()
const connection = await connectionStore.resolveConnection()

const game: Game = await connection.invoke("getGame", gameId)
console.log("got game", game)
const playerColor = game.color === "w" ? "white" : "black"
console.log("playerColor", playerColor)
const boardConfig: BoardConfig = {
  orientation: playerColor,
  premovable: { enabled: false },
  predroppable: { enabled: false },
}

const move = ref("")

let boardApi: BoardApi | undefined

connection.on("PlayerMoved", (_move) => {
  console.log("new move", _move)
  if (boardApi?.getLastMove() === _move) {
    return
  }

  boardApi?.move(_move)
})

const sendMove = async () => {
  const updatedGame = await connection.invoke("move", gameId, move.value)
  console.log("move invoked ", updatedGame)
}

const onMove = async (move) => {
  console.log("onMove", move)

  // move came from server.
  if (move.color !== game.color) return

  await connection.invoke("move", gameId, move.san)
  console.log("move invoked")
}
</script>

<template>
  Game {{ gameId }}

  <p>Type in your move:</p>
  <form @submit.prevent="sendMove">
    <input type="text" v-model="move" />
    <button type="submit">send</button>
  </form>

  <TheChessboard
    :player-color="playerColor"
    :board-config="boardConfig"
    @move="onMove"
    @board-created="(api) => (boardApi = api)"
  />
</template>
