<script setup lang="ts">

const route = useRoute()
const gameId = route.params.id

const move = ref("")

const connectionStore = useConnectionStore()
const connection = await connectionStore.resolveConnection()

const sendMove = async () => {
    const updatedGame = await connection.invoke("move", gameId, move.value)
    console.log("move invoked ", updatedGame)
}
</script>

<template>
Game {{ gameId }}

<p>Type in your move: </p>
<form @submit.prevent="sendMove">
    <input type="text" v-model="move">
    <button type="submit">send</button>
</form>
</template>