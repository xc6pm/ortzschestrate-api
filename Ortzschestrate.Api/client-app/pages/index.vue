<script setup lang="ts">
import * as signalR from "@microsoft/signalr"

let userStore = useUserStore()
await userStore.fetch()

if (!userStore.user) {
  await navigateTo("/login")
}

const connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7132/hubs/game").build()

await connection.start()

type Player = {connectionId: string, userId: string, name: string}

const pendingGames = ref<Player[]>([])

connection.on("NewGameCreated", (creator) => {
  console.log("new game created by ", creator)
})

connection.on("LobbyUpdated", (waitingPlayers: Player[]) => {
  console.log("players in lobby ", waitingPlayers)
  pendingGames.value = waitingPlayers
})

const matchUp = async () => {
  await connection.invoke("create")
  console.log("create invoked")
}

const joinGame = async (opponentConnectionId: string) => {
  await connection.invoke("join", opponentConnectionId)
  console.log("join invoked")
}

const cancelGame = async () => {
  await connection.invoke("cancel")
  console.log("cancel invoked")
}
</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  <button @click="matchUp">create game!</button>

  <br>

  <h3>Open Games</h3>
  
  <ul>
    <li v-for="player of pendingGames" :key="player.connectionId">
      {{ player.name }} 
      <button class="mr-2" @click="() => joinGame(player.connectionId)">join</button>
      <button v-if="connection.connectionId === player.connectionId" @click="cancelGame">cancel</button>
    </li>
  </ul>
</template>
