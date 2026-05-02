import type { Conference } from '../types'

const MOCK_CONFERENCES: Conference[] = [
  {
    id: 'conf-1',
    title: 'Frontend Fundamentals',
    location: 'Banja Luka',
    startsAt: '2026-05-16T09:00:00Z',
  },
]

export async function fetchConferences(): Promise<Conference[]> {
  return Promise.resolve(MOCK_CONFERENCES)
}
