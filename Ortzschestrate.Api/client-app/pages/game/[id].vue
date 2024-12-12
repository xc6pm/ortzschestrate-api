<script setup lang="ts">
import { BoardApi, TheChessboard, type BoardConfig } from "vue3-chessboard"
import "vue3-chessboard/style.css"
import type { Game, GameResult, GameUpdate } from "~/types/Game"

const route = useRoute()
const gameId = route.params.id

const userStore = useUserStore()
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

const opponentTime = ref(formatMilliseconds(game.timeInMilliseconds))
const playerTime = ref(formatMilliseconds(game.timeInMilliseconds))
const isPlayersTurn = ref(playerColor === "white")

connection.on("PlayerMoved", (gameUpdate: GameUpdate) => {
  console.log("new move", gameUpdate)

  if (boardApi?.getLastMove()?.san === gameUpdate.san) {
    playerTime.value = formatMilliseconds(gameUpdate.remainingTimeInMilliseconds)
    isPlayersTurn.value = false
    return
  } else {
    opponentTime.value = formatMilliseconds(gameUpdate.remainingTimeInMilliseconds)
  }

  boardApi?.move(gameUpdate.san)
  isPlayersTurn.value = true
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
  <section class="h-[90svh] flex flex-col justify-between content-between">
    <UCard id="opponentCard" class="my-2 mx-auto w-full max-w-full landscape:max-w-[700px]">
      <div class="flex flex-row justify-between">
        <span>{{ game.opponent }}</span>
        <ChessTimer :run="!isPlayersTurn" :duration="game.timeInMilliseconds" />
      </div>
    </UCard>

    <TheChessboard
      :player-color="playerColor"
      :board-config="boardConfig"
      @move="onMove"
      @board-created="(api) => (boardApi = api)"
    />

    <UCard id="playerCard" class="my-2 mx-auto w-full max-w-full landscape:max-w-[700px]">
      <div class="flex justify-between">
        <span>{{ userStore.user?.userName }}</span>
        <ChessTimer :run="isPlayersTurn" :duration="game.timeInMilliseconds" />
      </div>
    </UCard>
  </section>

  <p v-if="gameResult">{{ gameResult }} <button @click="goBackClicked">go back</button></p>
</template>
