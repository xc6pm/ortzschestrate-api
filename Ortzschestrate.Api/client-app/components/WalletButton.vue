<script lang="ts" setup>
import { BrowserWalletConnector, useVueDapp, type ConnWallet } from "@vue-dapp/core"
import { VueDappModal, useVueDappModal } from "@vue-dapp/modal"
import "@vue-dapp/modal/dist/style.css"

const userStore = useUserStore()

const { addConnectors, isConnected, wallet, disconnect, watchConnected, watchAddressChanged } = useVueDapp()

addConnectors([new BrowserWalletConnector()])

const colorMode = useColorMode()

const toast = useToast()
const walletConnectedOrAddressChanged = (wlt: ConnWallet) => {
  if (wlt.address === userStore.user!.verifiedWallet) return

  toast.add({
    title: "Unverified wallet",
    description: "You should verify your wallet to play staked games.",
    color: "yellow",
  })
}

watchConnected(walletConnectedOrAddressChanged)
watchAddressChanged(walletConnectedOrAddressChanged)

const isWalletVerified = computed(() => wallet.address === userStore.user!.verifiedWallet)

const verifyingWallet = ref(false)
const verifyWallet = async () => {
  verifyingWallet.value = true
  const response = await $fetch.raw(apiUrl("/wallet/verify"), {
    params: { walletAddress: wallet.address },
    method: "POST",
    credentials: "include",
  })

  if (!response.ok) {
    const bodyText = await response.text()
    toast.add({ title: "Verification failed", description: bodyText, color: "red" })
    return
  }

  toast.add({ title: "Check your email", description: "A link containing the verification link has been sent to your email", color: "yellow" })
}

const dropDownItems = computed(() => {
  if (isConnected) {
    const walletButtons = []
    if (wallet.address !== userStore.user!.verifiedWallet) walletButtons.push({ label: "verify", click: verifyWallet })

    walletButtons.push({ label: "disconnect", click: disconnect })

    return [walletButtons]
  }
})

function connectToggleClicked() {
  if (isConnected.value) disconnect()
  else useVueDappModal().open()
}
</script>

<template>
  <UButtonGroup size="sm" orientation="horizontal">
    <UButton @click="connectToggleClicked" color="oxford-blue">
      <UTooltip :text="isConnected ? `${isWalletVerified ? 'Verified &#9679 ' : ''} Click to disconnect` : ''">
        <span v-if="isConnected" class="align-middle justify-center"
          >{{ wallet.address!.slice(0, 7) }}...{{
            wallet.address!.slice(wallet.address!.length - 5, wallet.address!.length)
          }}
        </span>
        <span v-else>Connect Wallet</span>
      </UTooltip>
    </UButton>
    <UButton v-if="isConnected && !isWalletVerified" @click="verifyWallet" color="fiord" label="verify" />
  </UButtonGroup>

  <VueDappModal auto-connect :dark="colorMode.value === 'dark'"/>
</template>
