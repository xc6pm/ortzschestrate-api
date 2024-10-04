/// Checks whether the first argument is between the second and third,
/// with the last two parameter not in order.
/// The lower bound is inclusive, the upper is exclusive.
export function between(...numbers: number[]) {
  if (numbers[1] === numbers[2]) return numbers[0] === numbers[1]

  const min = numbers[2] > numbers[1] ? numbers[1] : numbers[2]
  const max = numbers[2] > numbers[1] ? numbers[2] : numbers[1]
  return numbers[0] >= min && numbers[0] < max
}
