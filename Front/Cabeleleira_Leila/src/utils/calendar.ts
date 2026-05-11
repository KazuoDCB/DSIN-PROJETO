export interface CalendarDay {
  date: Date
  day: number
  isCurrentMonth: boolean
  isToday: boolean
}

export const sameDay = (left: Date, right: Date) =>
  left.getFullYear() === right.getFullYear() &&
  left.getMonth() === right.getMonth() &&
  left.getDate() === right.getDate()

export const buildMonthDays = (monthDate: Date): CalendarDay[] => {
  const year = monthDate.getFullYear()
  const month = monthDate.getMonth()
  const firstDay = new Date(year, month, 1)
  const daysInMonth = new Date(year, month + 1, 0).getDate()
  const startOffset = firstDay.getDay()
  const today = new Date()
  const days: CalendarDay[] = []

  for (let index = 0; index < startOffset; index += 1) {
    const date = new Date(year, month, index - startOffset + 1)
    days.push({
      date,
      day: date.getDate(),
      isCurrentMonth: false,
      isToday: sameDay(date, today),
    })
  }

  for (let day = 1; day <= daysInMonth; day += 1) {
    const date = new Date(year, month, day)
    days.push({
      date,
      day,
      isCurrentMonth: true,
      isToday: sameDay(date, today),
    })
  }

  const rest = days.length % 7 === 0 ? 0 : 7 - (days.length % 7)
  for (let index = 1; index <= rest; index += 1) {
    const date = new Date(year, month + 1, index)
    days.push({
      date,
      day: date.getDate(),
      isCurrentMonth: false,
      isToday: sameDay(date, today),
    })
  }

  return days
}

export const monthTitle = (date: Date) =>
  new Intl.DateTimeFormat('pt-BR', {
    month: 'long',
    year: 'numeric',
  }).format(date)
