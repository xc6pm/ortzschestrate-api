<script setup lang="ts">
import { BoardApi, TheChessboard, type BoardConfig } from "vue3-chessboard"
import "vue3-chessboard/style.css"
import type { Game, GameResult } from "~/types/Game"

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

let boardApi: BoardApi | undefined

connection.on("PlayerMoved", (move) => {
  console.log("new move", move)
  if (boardApi?.getLastMove() === move) {
    return
  }

  boardApi?.move(move)
})

const gameResult = ref<string | null>(null)

connection.on("GameEnded", (res: GameResult) => {
  console.log("game ended", res)
  if (res.wonSide) {
    gameResult.value =
      (res.wonSide === "w" ? "White" : "Black") + " won by " + (res.result === "Resigned" ? "resignation" : res.result)
  } else {
    gameResult.value = "Draw by " + res.result
  }
})

const onMove = async (move) => {
  console.log("onMove", move)

  // move came from server.
  if (move.color !== game.color) return

  await connection.invoke("move", gameId, move.san)
  console.log("move invoked")
}

const goBackClicked = () => {
  navigateTo("/")
}
</script>

<template>
  Game {{ gameId }}

  <TheChessboard
    :player-color="playerColor"
    :board-config="boardConfig"
    @move="onMove"
    @board-created="(api) => (boardApi = api)"
  />

  <p v-if="gameResult">{{ gameResult }} <button @click="goBackClicked">go back</button></p>
</template>
