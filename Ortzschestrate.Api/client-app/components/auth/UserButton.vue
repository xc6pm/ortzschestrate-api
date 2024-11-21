<script setup lang="ts">
import { useUserStore } from "~/stores/user"

let userStore = useUserStore()

await userStore.fetch()

const showDropdown = ref(false)

watch(
  showDropdown,
  (newValue) => {
    if (newValue) {
      window.onclick = (evt) => {
        if (
          evt.target?.id === "profileButton" ||
          evt.target?.parentElement?.id === "profileButton"
        ) {
          return
        }

        showDropdown.value = false
      }
    } else {
      window.onclick = null
    }
  },
  {}
)

const tryLogout = async () => {
  await $fetch(apiUrl("/auth/logout"), {
    method: "POST",
    credentials: "include",
  })

  await userStore.fetch()

  await navigateTo("login")
}
</script>

<template>
  <button
    v-if="userStore.user"
    class="bg-gray-700 hover:bg-gray-600 text-white py-2 px-4 rounded-lg shadow-md focus:outline-none inline-flex"
    @click="() => (showDropdown = !showDropdown)"
    id="profileButton"
  >
    {{ userStore.user?.userName }}
    <svg
      class="-mr-1 size-5 text-gray-400"
      viewBox="0 0 20 20"
      fill="currentColor"
      aria-hidden="true"
      data-slot="icon"
    >
      <path
        fill-rule="evenodd"
        d="M5.22 8.22a.75.75 0 0 1 1.06 0L10 11.94l3.72-3.72a.75.75 0 1 1 1.06 1.06l-4.25 4.25a.75.75 0 0 1-1.06 0L5.22 9.28a.75.75 0 0 1 0-1.06Z"
        clip-rule="evenodd"
      />
    </svg>
  </button>

  <div
    :class="[
      'absolute',
      'right-0',
      'z-10',
      'w-56',
      'origin-top-right',
      'rounded-md',
      'bg-white',
      'shadow-lg',
      'ring-1',
      'ring-black/5',
      'focus:outline-none',
      !showDropdown ? 'hidden' : '',
    ]"
    role="menu"
    aria-orientation="vertical"
    aria-labelledby="menu-button"
    tabindex="-1"
  >
    <div class="py-1" role="none">
      <form @submit.prevent="tryLogout">
        <button
          type="submit"
          class="block w-full px-4 py-2 text-left text-sm text-gray-700"
          role="menuitem"
          tabindex="-1"
          id="menu-item-3"
        >
          logout
        </button>
      </form>
    </div>
  </div>
</template>
