<script setup lang="ts">
import Sqr from "./Sqr.vue"
import { initBoard } from "~/models/Board";

// Reverse to have a1 at the bottom for playing white
const sqrs = initBoard().reverse()
</script>

<template>

  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 0, 8 * 1)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 1, 8 * 2)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 2, 8 * 3)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 3, 8 * 4)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 4, 8 * 5)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 5, 8 * 6)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 6, 8 * 7)" :sqr="sqr"/></div>
  <div style="display: block;"><Sqr v-for="sqr in sqrs.slice(8 * 7, 8 * 8)" :sqr="sqr"/></div>
</template>
