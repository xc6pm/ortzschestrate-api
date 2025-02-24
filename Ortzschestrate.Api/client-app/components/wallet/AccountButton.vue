<script setup lang="ts">
import { useAccount, useWriteContract } from "@wagmi/vue"
import { readContract } from "@wagmi/core"
import { config } from "~/web3/wagmiConfig"
import { formatEther, parseEther, type Abi } from "viem"
import type { FormError } from "@nuxt/ui/dist/runtime/types"

const account = useAccount()
if (!account.isConnected.value) throw new Error("The account must be connected for this component.")

const stakesInContract = ref("0")
const { getDeployment } = useDeploymentStore()

const readBalance = async () => {
  if (!account?.address?.value) return

  const deployment = await getDeployment()

  const data = await readContract(config, {
    abi: deployment.abi as Abi,
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

const { writeContract } = useWriteContract({})
const isModalOpen = ref(false)
const modalState = reactive({
  amount: 0,
})
let isDepositModal = true

const deposit = async () => {
  modalState.amount = 0
  isDepositModal = true
  isModalOpen.value = true
}

const withdraw = () => {}

const amountEntered = async () => {
  if (errors.value.length) {
    return
  }

  const deployment = await getDeployment()

  if (isDepositModal) {
    await writeContract({
      address: deployment.address,
      value: parseEther(modalState.amount.toString()),
      abi: deployment.abi,
      functionName: "depositStakes",
      args: [],
    })
  } else {
  }

  isModalOpen.value = false
}

const dropdownItems = [
  [
    {
      label: "deposit",
      click: deposit,
    },
    { label: "withdraw", click: withdraw },
    {
      label: "disconnect",
      click: () => {
        wagmiAdapter.disconnect()
      },
    },
  ],
]

const errors = ref<FormError[]>([])
const validate = (state: { amount: number }): FormError[] => {
  const minimumAmount = 0.0001
  if (state.amount < minimumAmount) errors.value = [{ path: "amount", message: `The minimum is ${minimumAmount} ETH.` }]
  else errors.value = []
  return errors.value
}

validate(modalState)
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

  <UModal v-model="isModalOpen">
    <UCard>
      <UForm :state="modalState" :validate="validate">
        <UFormGroup label="Amount" name="amount">
          <UInput v-model="modalState.amount" type="number" />
        </UFormGroup>
        <UButton type="submit" @click="amountEntered" block class="mt-3"> Submit </UButton>
        <UButton type="cancel" @click="() => (isModalOpen = false)" block class="mt-3" color="white"> Cancel </UButton>
      </UForm>
    </UCard>
  </UModal>
</template>
