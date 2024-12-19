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

const pendingGamesFromServer: PendingGame[] = await connectionStore.invoke("getPending")
const pendingGames = ref<PendingGame[]>(pendingGamesFromServer)

const gameTimes = [
  { name: "Rapid", value: 10 },
  { name: "Blitz", value: 5 },
  { name: "Bullet", value: 3 },
]
const colors = [
  { name: "White", value: "w" },
  { name: "Black", value: "b" },
]
const newGame = reactive({ time: 10, color: "w" })

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

const createGame = async () => {
  await connectionStore.invoke("create", newGame.time, newGame.color)
  console.log("create invoked")
}

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
  <section class="flex flex-col lg:flex-row">
    <UCard class="my-3 ml-0 lg:mr-3 lg:w-80 lg:h-fit" :ui="{ body: { padding: 'p-3' }, strategy: 'override' }">
      <h2 class="text-lg text-center">Setup a table</h2>
      <UForm :state="newGame" @submit.prevent="createGame">
        <UFormGroup label="Game Type:" class="my-3">
          <USelectMenu
            :options="gameTimes"
            v-model="newGame.time"
            option-attribute="name"
            value-attribute="value"
            placeholder="Game Type"
            required
          />
        </UFormGroup>
        <UFormGroup label="Color:" class="my-3">
          <USelectMenu
            :options="colors"
            v-model="newGame.color"
            option-attribute="name"
            value-attribute="value"
            placeholder="Color"
            required
          />
        </UFormGroup>
        <UButton type="submit" class="mb-3" block>Create</UButton>
      </UForm>
    </UCard>

    <UCard :ui="{ body: { padding: 'p-0' } }" class="mt-3 grow">
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
  </section>
</template>
