<script setup lang="ts">
import UserButton from "~/components/auth/UserButton.vue"
</script>

<template>
  <header class="bg-gray-800 text-white py-4 shadow-md">
    <div class="max-w-7xl mx-auto px-4 flex justify-between items-center">
      <NuxtLink to="/"><span class="text-3xl font-bold text-green-400">ortzschestrate</span></NuxtLink>

      <div class="relative">
        <UserButton />
      </div>
    </div>
  </header>

  <UContainer :ui="{ padding: 'px-4', strategy: 'override' }">
    <slot />
  </UContainer>

  <UNotifications />
</template>
