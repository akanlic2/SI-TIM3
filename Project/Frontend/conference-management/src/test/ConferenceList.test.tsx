import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { ConferenceList } from '../features/conference/components/ConferenceList'

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: 'ucesnik',
    },
  }),
}))

const conferences = [
  {
    conferenceId: '1',
    title: 'AI Summit Sarajevo',
    description: 'Konferencija o vjestackoj inteligenciji',
    startDate: '2026-06-10T09:00:00Z',
    endDate: '2026-06-10T17:00:00Z',
    location: 'Sarajevo',
    category: 'IT',
    maxParticipants: 100,
    status: 'Active',
  },
]

describe('ConferenceList', () => {
  it('renders conference card', () => {
    render(
      <ConferenceList
        conferences={conferences}
        isAdminOrOrganizer={false}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    expect(screen.getByText('AI Summit Sarajevo')).toBeInTheDocument()
    expect(screen.getByText('Sarajevo')).toBeInTheDocument()
    expect(screen.getByText('IT')).toBeInTheDocument()
  })

  it('shows participant apply button', () => {
    render(
      <ConferenceList
        conferences={conferences}
        isAdminOrOrganizer={false}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    expect(screen.getByText('Prijavi se')).toBeInTheDocument()
  })

  it('shows edit and delete buttons for admin or organizer', () => {
    render(
      <ConferenceList
        conferences={conferences}
        isAdminOrOrganizer={true}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    expect(screen.getByText('Uredi')).toBeInTheDocument()
    expect(screen.getByText('Obriši')).toBeInTheDocument()
  })
})