<script setup lang="ts">
import { BoardApi, TheChessboard, type BoardConfig } from "vue3-chessboard"
import "vue3-chessboard/style.css"
import type { Game, GameResult, GameUpdate } from "~/types/Game"

const route = useRoute()
const gameId = route.params.id

const userStore = useUserStore()
const connectionStore = useConnectionStore()

const game: Game = await connectionStore.invoke("getGame", gameId)
console.log("got game", game)
if (!game) {
  const toast = useToast()
  toast.add({ description: "Couldn't find that game!", color: "red" })
  navigateTo("/")
}

const playerColor = game.color === "w" ? "white" : "black"
console.log("playerColor", playerColor)
const boardConfig: BoardConfig = {
  orientation: playerColor,
  premovable: { enabled: false },
  predroppable: { enabled: false },
  viewOnly: false,
}

let boardApi: BoardApi | undefined

const isPlayersTurn = ref(playerColor === "white")
const opponentTimer = useTemplateRef("opponentTimer")
const playerTimer = useTemplateRef("playerTimer")

useConnectionEvent("PlayerMoved", (gameUpdate: GameUpdate) => {
  console.log("new move", gameUpdate)

  if (boardApi?.getLastMove()?.san === gameUpdate.san) {
    playerTimer.value?.syncWithServer(gameUpdate.remainingTimeInMilliseconds)
    isPlayersTurn.value = false
    return
  } else {
    opponentTimer.value?.syncWithServer(gameUpdate.remainingTimeInMilliseconds)
  }

  boardApi?.move(gameUpdate.san)
  isPlayersTurn.value = true
})

const resultModal = reactive({ isOpen: false, playerPOVResult: "", reason: "" })

useConnectionEvent("GameEnded", (res: GameResult) => {
  console.log("game ended", res)
  boardConfig.viewOnly = true
  if (res.wonSide) {
    const playerWon = res.wonSide === game.color
    resultModal.playerPOVResult = `You ${playerWon ? "won" : "lost"}`
    resultModal.reason = `by ${res.result === "Resigned" ? "resignation" : res.result}`
  } else {
    resultModal.playerPOVResult = "Draw"
    resultModal.reason = `by ${res.result}`
  }
  resultModal.isOpen = true
})

const onMove = async (move: any) => {
  console.log("onMove", move)

  // move came from server.
  if (move.color !== game.color) return

  await connectionStore.invoke("move", gameId, move.san)
  console.log("move invoked")
}

const playerTimedOut = () => {
  console.log("timed out ")
  // This actually stops as soon as the game times out.
  // Invoking timeout only once is not effective because we may do it a
  // fraction of a second earlier than the server sees the timeout.
  const intervalId = setInterval(async () => {
    try {
      if (await connectionStore.invoke("timeout", gameId)) {
        clearInterval(intervalId)
      }
    } catch {
      clearInterval(intervalId)
    }
  }, 1000)
}
</script>

<template>
  <section class="h-[90svh] flex flex-col justify-between content-between">
    <UCard id="opponentCard" class="my-2 mx-auto w-full max-w-full landscape:max-w-[700px]">
      <div class="flex flex-row justify-between">
        <span>{{ game.opponent }}</span>
        <ChessTimer
          :run="!isPlayersTurn"
          :duration="game.timeInMilliseconds"
          ref="opponentTimer"
          @timeout="playerTimedOut"
        />
      </div>
    </UCard>

    <TheChessboard
      :player-color="playerColor"
      :board-config="boardConfig"
      @move="onMove"
      @board-created="(api) => (boardApi = api)"
      :reactive-config="true"
    />

    <UCard id="playerCard" class="my-2 mx-auto w-full max-w-full landscape:max-w-[700px]">
      <div class="flex justify-between">
        <span>{{ userStore.user?.userName }}</span>
        <ChessTimer
          :run="isPlayersTurn"
          :duration="game.timeInMilliseconds"
          ref="playerTimer"
          @timeout="playerTimedOut"
        />
      </div>
    </UCard>
  </section>

  <UModal v-model="resultModal.isOpen">
    <UCard>
      <h1 class="text-4xl text-center">{{ resultModal.playerPOVResult }}</h1>
      <h4 class="text-sm text-slate-200 text-center">{{ resultModal.reason }}</h4>

      <UButton to="/" block size="lg" class="m-3 ml-0">Back to lobby</UButton>
      <UButton @click="() => (resultModal.isOpen = false)" block size="lg" class="m-3 mb-0 ml-0"
        >See what happened</UButton
      >
    </UCard>
  </UModal>
</template>
