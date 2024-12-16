export const useConnectionEvent = (eventName: string, handler: (...args: any[]) => any) => {
  const connectionStore = useConnectionStore()
  onMounted(() => connectionStore.on(eventName, handler))
  onUnmounted(() => connectionStore.off(eventName, handler))
}
