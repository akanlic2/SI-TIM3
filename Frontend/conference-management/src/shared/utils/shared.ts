export function formatShortDate(dateInput: string | Date): string {
  const date = dateInput instanceof Date ? dateInput : new Date(dateInput)

  return new Intl.DateTimeFormat('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}
