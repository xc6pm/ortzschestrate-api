<script setup lang="ts">
import Sqr from "./Sqr.vue"
import { initBoard } from "~/models/Board"

// Reverse to have a1 at the bottom for playing white
const sqrs = initBoard()
</script>

<template>
  <!--Must render rows in reverse since the array is initialized with a1 as the start-->
  <div v-for="n in [...Array(8).keys()].reverse()" style="display: block">
    <Sqr v-for="sqr in sqrs.slice(8 * n, 8 * (n + 1))" :sqr="sqr">
      <p>{{ sqr.loc }}</p>
      <span v-if="sqr.piece">{{ sqr.piece.type }}</span>
    </Sqr>
  </div>
</template>
