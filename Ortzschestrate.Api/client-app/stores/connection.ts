import * as signalr from "@microsoft/signalr"

export const useConnectionStore = defineStore("connectionStore", () => {
    const connection = ref<signalr.HubConnection | null>()

    const resolveConnection = async () : Promise<signalr.HubConnection> => {
        if (connection.value && connection.value)
            return Promise.resolve(connection!.value!)

        const conn =  new signalr.HubConnectionBuilder()
            .withUrl("https://localhost:7132/hubs/game")
            .build()

        await conn.start()

        connection.value = conn

        return connection.value
    }

    return {resolveConnection}
})