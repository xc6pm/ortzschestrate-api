<script setup lang="ts">
import * as signalR from "@microsoft/signalr"

let userStore = useUserStore()
await userStore.fetch()

if (!userStore.user) {
  await navigateTo("/login")
}

const matchUp = async () => {
  const connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:7132/hubs/game").build()

  await connection.start()
  
  connection.on("NewGameCreated", (creator) => {
    console.log("new game created by ", creator)
  })

  connection.on("LobbyUpdated", (waitingPlayers) => {
    console.log("players in lobby ", waitingPlayers)
  })


  await connection.invoke("create")
  console.log("create invoked")
}
</script>

<template>
  <h1>Welcome to ortzschestrate!</h1>

  {{ userStore.user }}

  <button @click="matchUp">match me up!</button>
</template>
