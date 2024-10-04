export function isUpperCase(str: string): boolean {
  for (let i = 0; i < str.length; i++) {
    const charCode = str.charCodeAt(i)
    if (charCode < 65 || charCode > 90) {
      return false
    }
  }
  return true
}

export function isNumber(str: string): boolean {
  return /^\d+$/.test(str)
}
