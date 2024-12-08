<script setup lang="ts">
import type { PendingGame } from "~/types/Game"

let userStore = useUserStore()
await userStore.fetch()

if (!userStore.user) {
  await navigateTo("/login")
}

const pendingGameFields = [
  {
    key: "creator",
    label: "Opponent",
  },
  {
    key: "gameType",
    label: "Time",
  },
  {
    key: "color",
    label: "Your Color",
  },
  {
    key: "actions",
  },
]

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

const createGame = async () => {
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
  <h1 class="text-center text-4xl mt-3">Welcome to ortzschestrate!</h1>

  <div class="flex flex-row justify-center mt-3">
    <UButton class="justify-center" size="lg" @click="createGame">create game!</UButton>
  </div>

  <h3 class="text-2xl text-center mt-3">or</h3>
  <h3 class="text-2xl text-center mt-3">Join a game:</h3>

  <UCard :ui="{ body: { padding: 'p-0' } }" class="mt-3">
    <UTable
      :columns="pendingGameFields"
      :rows="pendingGames"
      :empty-state="{ icon: 'i-heroicons-circle-stack-20-solid', label: 'No pending games.' }"
    >
      <template #creator-data="{ row }">
        {{ row.creator.name }}
      </template>
      <template #gameType-data="{ row }">
        {{ row.gameType.name }}
      </template>
      <template #color-data="{ row }">
        {{
          // Check if it's an own game. If it's not show the opposite color
          row.creator.userId === userStore.user?.id
            ? row.creatorColor.asChar === "w"
              ? "white"
              : "black"
            : row.creatorColor.asChar === "w"
            ? "black"
            : "white"
        }}
      </template>
      <template #actions-data="{ row }">
        <UButton
          v-if="row.creator.userId !== userStore.user?.id"
          label="Join"
          @click="() => joinGame(row.creator.userId, row.creatorConnectionId)"
        />
        <UButton
          v-if="row.creator.userId === userStore.user?.id"
          label="Cancel"
          @click="() => cancelGame(row.creatorConnectionId)"
        />
      </template>
    </UTable>
  </UCard>
</template>
