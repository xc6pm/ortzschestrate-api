<script setup lang="ts">
import LoginForm from "~/components/auth/LoginForm.vue"
import RegisterForm from "~/components/auth/RegisterForm.vue"

const tabs = [
  {
    key: "login",
    label: "Login",
    description: "Login to pre-existing account",
  },
  {
    key: "register",
    label: "Register",
    description: "Create a new account",
  },
]
</script>

<template>
  <UCard class="w-1/2 mx-auto mt-5" :ui="{body: {padding: 'px-0 pt-0 pb-5'}, strategy: 'override'}">
    <UTabs :items="tabs" class="w-full" :ui="{list: {rounded: 'rounded-t-lg'}, strategy: 'override'}" >
      <template #item="{ item }">
        <LoginForm v-if="item.key === 'login'" />

        <RegisterForm v-else />
      </template>
    </UTabs>
  </UCard>
</template>

<style scoped></style>
