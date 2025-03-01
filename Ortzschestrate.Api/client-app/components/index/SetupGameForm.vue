<script setup lang="ts">
import type { FormError } from "@nuxt/ui/dist/runtime/types"
import { useContractStateStore } from "~/stores/contractState"

const gameTimes = [
  { name: "Rapid", value: 10 },
  { name: "Blitz", value: 5 },
  { name: "Bullet", value: 3 },
]
const colors = [
  { name: "White", value: "w" },
  { name: "Black", value: "b" },
]

const minStakeAmount = 0.0003
const newGame = reactive({ time: 10, color: "w", wagered: false, stake: 0 })
const contractState = useContractStateStore()
watch(
  () => newGame.wagered,
  (newValue) => {
    if (!newValue) newGame.stake = 0
  }
)

const connectionStore = useConnectionStore()

const createGame = async () => {
  await connectionStore.invoke("create", newGame.time, newGame.color, newGame.stake)
  console.log("create invoked")
}

const validate = (state: { wagered: boolean; stake: number }): FormError[] => {
  if (state.wagered) {
    if (state.stake <= 0)
      return [{ path: "stake", message: "Must specify a stake amount greater than " + minStakeAmount }]
    else if (state.stake > contractState.stakesEth)
      return [{ path: "stake", message: `Cannot specify that amount. You have ${contractState.stakesEth} ETH.` }]
  } else if (state.stake > 0) {
    return [{ path: "stake", message: "Can't specify stake when the game is non-wagered." }]
  }

  return []
}
</script>

<template>
  <UCard :ui="{ body: { padding: 'p-3' }, strategy: 'override' }">
    <h2 class="text-lg text-center">Setup a board</h2>
    <UForm :state="newGame" @submit.prevent="createGame" :validate="validate">
      <UFormGroup label="Game Type:" name="time" class="my-3">
        <USelectMenu
          :options="gameTimes"
          v-model="newGame.time"
          option-attribute="name"
          value-attribute="value"
          placeholder="Game Type"
          required
        />
      </UFormGroup>
      <UFormGroup label="Color:" name="color" class="my-3">
        <USelectMenu
          :options="colors"
          v-model="newGame.color"
          option-attribute="name"
          value-attribute="value"
          placeholder="Color"
          required
        />
      </UFormGroup>

      <UFormGroup class="my-3" name="wagered" v-if="contractState.account.isConnected">
        <UCheckbox v-model="newGame.wagered" label="Wagered" />
      </UFormGroup>

      <UFormGroup label="Stake:" name="stake" class="my-3" v-if="contractState.account.isConnected && newGame.wagered">
        <UInput type="number" v-model="newGame.stake" />
      </UFormGroup>

      <UButton type="submit" class="mb-3" block>Create</UButton>
    </UForm>
  </UCard>
</template>
