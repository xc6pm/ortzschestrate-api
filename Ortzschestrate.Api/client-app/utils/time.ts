export const formatMilliseconds = (milliseconds: number) => {
  const millis = milliseconds % 1000
  const seconds = Math.floor((milliseconds / 1000) % 60)
  const minutes = Math.floor((milliseconds / (60 * 1000)) % 60)

  return minutes + ":" + seconds + "." + millis
}
