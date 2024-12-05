<script setup lang="ts">
import type { PendingGame } from "~/types/Game"

let userStore = useUserStore()
await userStore.fetch()

if (!userStore.user) {
  await navigateTo("/login")
}

const connectionStore = useConnectionStore()

const connection = await connectionStore.resolveConnection()

const pendingGames = ref<PendingGame[]>([])

connection.on("NewGameCreated", (creator) => {
  console.log("new game created by ", creator)
})

connection.on("LobbyUpdated", (updatedPendingGames: PendingGame[]) => {
  console.log("Pending games ", updatedPendingGames)
  pendingGames.value = updatedPendingGames
})

connection.on("GameStarted", (gameId) => {
  console.log("Game started", gameId)
  navigateTo("/game/" + gameId)
})

const matchUp = async () => {
  await connection.invoke("create", "Untimed", "White")
  console.log("create invoked")
}

const joinGame = async (opponentUserId: string, opponentConnectionId: string) => {
  const gameId = await connection.invoke("join", opponentUserId, opponentConnectionId)
  console.log("join invoked", gameId)
  navigateTo("/game/" + gameId)
}

const cancelGame = async (creatorConnectionId: string) => {
  await connection.invoke("cancel", creatorConnectionId)
  console.log("cancel invoked")
}
</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  <button @click="matchUp">create game!</button>

  <br />

  <h3>Open Games</h3>

  <ul>
    <li v-for="pendingGame of pendingGames" :key="pendingGame.creatorConnectionId">
      {{ pendingGame.creator.name }} ({{ pendingGame.creatorConnectionId }})
      {{ pendingGame.gameType }}
      <button class="mr-2" @click="() => joinGame(pendingGame.creator.userId, pendingGame.creatorConnectionId)">
        join
      </button>
      <button
        v-if="userStore.user?.id === pendingGame.creator.userId"
        @click="() => cancelGame(pendingGame.creatorConnectionId)"
      >
        cancel
      </button>
    </li>
  </ul>
</template>
