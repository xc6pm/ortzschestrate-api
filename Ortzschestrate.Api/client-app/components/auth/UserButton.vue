<script setup lang="ts">
import {useUserStore} from "~/stores/user"

let userStore = useUserStore()

await userStore.fetch()

const tryLogout = async () => {
  await $fetch(apiUrl("/auth/logout"), {
    method: "POST",
    credentials: "include",
  })

  await userStore.fetch()

  await navigateTo("login")
}

const dropDownItems = [
  [
    {
      label: "log out",
      click: tryLogout,
    },
  ],
]
</script>

<template>
  <UDropdown v-if="userStore.user" :items="dropDownItems" :popper="{placement: 'bottom-end'}">
    <UButton :label="userStore.user.userName" trailing-icon="i-heroicons-chevron-down-solid" color="oxford-blue"/>
  </UDropdown>
</template>
