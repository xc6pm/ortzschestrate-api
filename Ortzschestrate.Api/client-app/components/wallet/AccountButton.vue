<script setup lang="ts">
import { useAccount } from "@wagmi/vue"
import { readContract } from "@wagmi/core"
import { config } from "~/web3/wagmiConfig"
import { formatEther } from "viem"

const account = useAccount()
if (!account.isConnected.value) throw new Error("The account must be connected for this component.")

const stakesInContract = ref("0")

const readBalance = async () => {
  if (!account?.address?.value) return

  const deployment = await $fetch<any>("/deployment/ORTBet.json")

  const data = await readContract(config, {
    abi: deployment.abi,
    address: deployment.address,
    functionName: "getBalance",
    args: [account.address.value],
  })

  stakesInContract.value = formatEther(data as bigint)
}

const { wagmiAdapter } = useWagmi()

wagmiAdapter.on("accountChanged", async (arg) => {
  console.log("accountChanged", arg)
  await readBalance()
})

readBalance()

const deposit = () => {}

const withdraw = () => {}

const disconnect = () => {}

const dropdownItems = [
  [
    {
      label: "deposit",
      click: deposit,
    },
    { label: "withdraw", click: withdraw },
    { label: "disconnect", click: disconnect },
  ],
]
</script>

<template>
  <UDropdown :items="dropdownItems">
    <UButton color="oxford-blue">
      {{ account.address.value?.substring(0, 5) }}...{{
        account.address.value?.substring(account.address.value.length - 4, account.address.value.length)
      }}
      ({{ stakesInContract }} ETH)
    </UButton>
  </UDropdown>
</template>
