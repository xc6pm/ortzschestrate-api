<script setup lang="ts">
import type { PendingGame } from "~/types/Game"

let userStore = useUserStore()

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

const pendingGamesFromServer: PendingGame[] = await connectionStore.invoke("getPending")
const pendingGames = ref<PendingGame[]>(pendingGamesFromServer)

useConnectionEvent("NewGameCreated", (creator) => {
  console.log("new game created by ", creator)
})

useConnectionEvent("LobbyUpdated", (updatedPendingGames: PendingGame[]) => {
  console.log("Pending games ", updatedPendingGames)
  pendingGames.value = updatedPendingGames
})

useConnectionEvent("GameStarted", (gameId) => {
  console.log("Game started", gameId)
  navigateTo("/game/" + gameId)
})

const joinGame = async (opponentUserId: string) => {
  const gameId = await connectionStore.invoke("join", opponentUserId)
  console.log("join invoked", gameId)
  navigateTo("/game/" + gameId)
}

const cancelGame = async () => {
  await connectionStore.invoke("cancel")
  console.log("cancel invoked")
}
</script>

<template>
  <UCard :ui="{ body: { padding: 'p-0' } }" >
    <h2 class="text-lg m-3 text-center">Join someone</h2>
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
          @click="() => joinGame(row.creator.userId)"
        />
        <UButton v-if="row.creator.userId === userStore.user?.id" label="Cancel" @click="cancelGame" />
      </template>
    </UTable>
  </UCard>
</template>
