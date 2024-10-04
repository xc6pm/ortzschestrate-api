import { defineStore } from "pinia"
import { GameProcessor } from "~/models/GameProcessor"

export const useGamesStore = defineStore("games", () => {
  const games = ref<GameProcessor[]>([])

  const newGame = () => {
    const newItem = reactive(new GameProcessor())
    games.value = [...games.value, newItem]
    return newItem
  }

  return { games, newGame }
})
