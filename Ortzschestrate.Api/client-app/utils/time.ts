export const formatMilliseconds = (milliseconds: number) => {
  if (milliseconds <= 0) return "00:00.00"

  const millis = Math.floor((milliseconds % 1000) / 10)
  const seconds = Math.floor((milliseconds / 1000) % 60)
  const minutes = Math.floor((milliseconds / (60 * 1000)) % 60)

  if (milliseconds < 20000) {
    return `${formTwoDigits(minutes)}:${formTwoDigits(seconds)}.${formTwoDigits(millis)}`
  }

  return `${formTwoDigits(minutes)}:${formTwoDigits(seconds)}`
}

const formTwoDigits = (x: number): string => (x < 10 ? "0" + x : x.toString())
