<script setup lang="ts">
const gameTimes = [
  { name: "Rapid", value: 10 },
  { name: "Blitz", value: 5 },
  { name: "Bullet", value: 3 },
]
const colors = [
  { name: "White", value: "w" },
  { name: "Black", value: "b" },
]

const newGame = reactive({ time: 10, color: "w", wagered: false, stake: 0 })

const connectionStore = useConnectionStore()

const createGame = async () => {
  await connectionStore.invoke("create", newGame.time, newGame.color)
  console.log("create invoked")
}
</script>

<template>
  <UCard :ui="{ body: { padding: 'p-3' }, strategy: 'override' }">
    <h2 class="text-lg text-center">Setup a board</h2>
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

      <UFormGroup class="my-3">
        <UCheckbox v-model="newGame.wagered" label="Wagered" />
      </UFormGroup>

      <UFormGroup label="Stake:" class="my-3">
        <UInput type="number" v-model="newGame.stake" />
      </UFormGroup>

      <UButton type="submit" class="mb-3" block>Create</UButton>
    </UForm>
  </UCard>
</template>
