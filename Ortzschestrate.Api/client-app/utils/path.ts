export const apiUrl = (actionPath: string) => {
    const config = useRuntimeConfig()
    if (!actionPath.startsWith("/")) {
        actionPath = "/" + actionPath
    }
    return config.public.apiUrl + actionPath
}