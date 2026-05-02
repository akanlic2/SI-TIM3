import type { Conference } from '../types'

interface ConferenceListProps {
  conferences: Conference[]
}

export function ConferenceList({ conferences }: ConferenceListProps) {
  if (conferences.length === 0) {
    return <p>No conferences available.</p>
  }

  return (
    <ul>
      {conferences.map((conference) => (
        <li key={conference.id}>
          <strong>{conference.title}</strong> - {conference.location}
        </li>
      ))}
    </ul>
  )
}
