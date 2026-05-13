import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi, beforeEach } from 'vitest'
import ConferenceDetailsPage from '../pages/ConferenceDetailsPage'
import * as conferenceApi from '../features/conference/api/conferenceApi'

vi.mock('../features/conference/api/conferenceApi', () => ({
  fetchConferenceById: vi.fn(),
}))

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: 'admin-sistema',
    },
    token: 'test-token',
  }),
}))

describe('ConferenceDetailsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()

    Object.defineProperty(window, 'location', {
      value: {
        pathname: '/conferences/conf-1',
      },
      writable: true,
    })

    vi.mocked(conferenceApi.fetchConferenceById).mockResolvedValue({
      conferenceId: 'conf-1',
      title: 'AI Summit Sarajevo',
      description: 'Konferencija o vjestackoj inteligenciji',
      startDate: '2026-06-10T09:00:00Z',
      endDate: '2026-06-10T17:00:00Z',
      location: 'Sarajevo',
      category: 'IT',
      maxParticipants: 100,
      status: 'Active',
    })

    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => [
          {
            conferenceRegistrationId: 'reg-1',
            userId: 'user-1',
            registrationDate: '2026-06-01T10:00:00Z',
            registrationStatus: 'Confirmed',
            user: {
              firstName: 'Test',
              lastName: 'Ucesnik',
              email: 'ucesnik@test.com',
            },
          },
        ],
      })
    )
  })

  it('renders conference details', async () => {
    render(<ConferenceDetailsPage />)

    expect(await screen.findAllByText('AI Summit Sarajevo')).toHaveLength(2)
    expect(screen.getAllByText(/Sarajevo/).length).toBeGreaterThan(0)
    expect(screen.getAllByText(/IT/).length).toBeGreaterThan(0)
  })

  it('shows sessions button', async () => {
    render(<ConferenceDetailsPage />)

    expect(await screen.findByText('📅 Sesije')).toBeInTheDocument()
  })

  it('renders admin registrations section', async () => {
    render(<ConferenceDetailsPage />)

    expect(await screen.findByText('Prijavljeni učesnici')).toBeInTheDocument()

    await waitFor(() => {
      expect(screen.getByText('Test')).toBeInTheDocument()
      expect(screen.getByText('Ucesnik')).toBeInTheDocument()
      expect(screen.getByText('ucesnik@test.com')).toBeInTheDocument()
      expect(screen.getAllByText('Confirmed').length).toBeGreaterThan(0)
    })
  })

  it('filters registrations by search input', async () => {
    render(<ConferenceDetailsPage />)

    const input = await screen.findByPlaceholderText('Pretraži po imenu ili emailu...')

    await userEvent.type(input, 'ucesnik@test.com')

    expect(screen.getByText('ucesnik@test.com')).toBeInTheDocument()
  })
})