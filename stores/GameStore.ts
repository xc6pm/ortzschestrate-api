import { defineStore } from "pinia"
import { Game } from "~/models/Game"

export const useGamesStore = defineStore("games", () => {
  const games = ref<Game[]>([])

  const newGame = () => {
    const newItem = new Game()
    games.value = [...games.value, newItem]
    return newItem
  }

  return { games, newGame }
})
