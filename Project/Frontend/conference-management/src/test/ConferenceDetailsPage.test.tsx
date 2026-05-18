import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi, beforeEach } from 'vitest'
import ConferenceDetailsPage from '../pages/ConferenceDetailsPage'
import * as conferenceApi from '../features/conference/api/conferenceApi'

vi.mock('../features/conference/api/conferenceApi', () => ({
  fetchConferenceById: vi.fn(),
}))

let mockRole = 'admin-sistema'
let mockToken: string | null = 'test-token'

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: mockRole ? { role: mockRole } : null,
    token: mockToken,
  }),
}))

const conference = {
  conferenceId: 'conf-1',
  title: 'AI Summit Sarajevo',
  description: 'Konferencija o vjestackoj inteligenciji',
  startDate: '2026-06-10T09:00:00Z',
  endDate: '2026-06-10T17:00:00Z',
  location: 'Sarajevo',
  category: 'IT',
  maxParticipants: 100,
  status: 'Active',
}

const capacity = {
  registeredCount: 23,
  maxParticipants: 50,
  availableSpots: 27,
  isFull: false,
}

const fullCapacity = {
  registeredCount: 50,
  maxParticipants: 50,
  availableSpots: 0,
  isFull: true,
}

const registrations = [
  {
    conferenceRegistrationId: 'reg-1',
    userId: 'user-1',
    registrationDate: '2026-06-01T10:00:00Z',
    registrationStatus: 'Confirmed',
    firstName: 'Test',
    lastName: 'Ucesnik',
    email: 'ucesnik@test.com',
  },
  {
    conferenceRegistrationId: 'reg-2',
    userId: 'user-2',
    registrationDate: '2026-06-02T10:00:00Z',
    registrationStatus: 'Pending',
    firstName: 'Drugi',
    lastName: 'Korisnik',
    email: 'drugi@test.com',
  },
]

function mockConferenceFetch({
  capacityResponse = capacity,
  registrationsResponse = registrations,
  capacityOk = true,
  registrationsOk = true,
}: {
  capacityResponse?: typeof capacity
  registrationsResponse?: typeof registrations
  capacityOk?: boolean
  registrationsOk?: boolean
} = {}) {
  vi.stubGlobal(
    'fetch',
    vi.fn((input: RequestInfo | URL) => {
      const url = String(input)

      if (url.includes('/capacity')) {
        return Promise.resolve({
          ok: capacityOk,
          status: capacityOk ? 200 : 500,
          json: async () => capacityResponse,
        })
      }

      if (url.includes('/participants')) {
        return Promise.resolve({
          ok: registrationsOk,
          status: registrationsOk ? 200 : 500,
          json: async () => registrationsResponse,
        })
      }

      return Promise.resolve({
        ok: false,
        status: 404,
        json: async () => ({}),
      })
    })
  )
}

describe('ConferenceDetailsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockRole = 'admin-sistema'
    mockToken = 'test-token'
    Object.defineProperty(window, 'location', {
      value: {
        pathname: '/conferences/conf-1',
      },
      writable: true,
    })
    vi.mocked(conferenceApi.fetchConferenceById).mockResolvedValue(conference)
    mockConferenceFetch()
  })

  it('smoke: renderuje detalje konferencije sa capacity i participants sekcijama', async () => {
    render(<ConferenceDetailsPage />)

    expect(await screen.findAllByText('AI Summit Sarajevo')).toHaveLength(2)
    expect(await screen.findByText('Kapacitet konferencije')).toBeInTheDocument()
    expect(await screen.findByText('ucesnik@test.com')).toBeInTheDocument()
  })

  it('renders conference details', async () => {
    render(<ConferenceDetailsPage />)
    expect(await screen.findAllByText('AI Summit Sarajevo')).toHaveLength(2)
    expect(screen.getAllByText(/Sarajevo/).length).toBeGreaterThan(0)
    expect(screen.getAllByText(/IT/).length).toBeGreaterThan(0)
  })

  it('shows sessions button', async () => {
    render(<ConferenceDetailsPage />)
    expect(await screen.findByText(/Sesije/)).toBeInTheDocument()
  })

  it('renders admin registrations section', async () => {
    render(<ConferenceDetailsPage />)

    expect(await screen.findByText('ucesnik@test.com')).toBeInTheDocument()
    expect(screen.getByText('Test')).toBeInTheDocument()
    expect(screen.getByText('Ucesnik')).toBeInTheDocument()
    expect(screen.getAllByText('Confirmed').length).toBeGreaterThan(0)
  })

  it('filters registrations by search input', async () => {
    render(<ConferenceDetailsPage />)

    const input = await screen.findByPlaceholderText(/Pretra/)
    await userEvent.type(input, 'ucesnik@test.com')

    expect(screen.getByText('ucesnik@test.com')).toBeInTheDocument()
    expect(screen.queryByText('drugi@test.com')).not.toBeInTheDocument()
  })

  it('renders conference capacity widget with registered count, maximum, available spots and fill status', async () => {
    const { container } = render(<ConferenceDetailsPage />)

    expect(await screen.findByText('Kapacitet konferencije')).toBeInTheDocument()
    expect(screen.getByText('Prijavljenih')).toBeInTheDocument()
    expect(screen.getByText('Maksimum')).toBeInTheDocument()
    expect(screen.getByText('Slobodnih mjesta')).toBeInTheDocument()
    expect(screen.getByText('Popunjenost')).toBeInTheDocument()
    expect(screen.getByText('23')).toBeInTheDocument()
    expect(screen.getByText('50')).toBeInTheDocument()
    expect(screen.getByText('27')).toBeInTheDocument()
    expect(screen.getByText('46%')).toBeInTheDocument()
    expect(container.querySelector('div[style*="width: 46%"]')).not.toBeNull()
  })

  it('calls conference capacity API for the current conference', async () => {
    render(<ConferenceDetailsPage />)

    await screen.findByText('Kapacitet konferencije')

    expect(fetch).toHaveBeenCalledWith('/api/conferences/conf-1/capacity', {
      headers: { Authorization: 'Bearer test-token' },
    })
  })

  it('shows full capacity status when conference is full', async () => {
    mockConferenceFetch({ capacityResponse: fullCapacity })

    render(<ConferenceDetailsPage />)

    expect(await screen.findByText('Kapacitet konferencije')).toBeInTheDocument()
    expect(screen.getByText('0')).toBeInTheDocument()
    expect(screen.getByText('100%')).toBeInTheDocument()
    expect(screen.getByText('Konferencija je popunjena')).toBeInTheDocument()
  })

  it('does not render capacity widget when capacity API fails', async () => {
    mockConferenceFetch({ capacityOk: false })

    render(<ConferenceDetailsPage />)

    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith('/api/conferences/conf-1/capacity', {
        headers: { Authorization: 'Bearer test-token' },
      })
    })
    expect(screen.queryByText('Kapacitet konferencije')).not.toBeInTheDocument()
  })

  it('renders empty participants state when there are no registrations', async () => {
    mockConferenceFetch({ registrationsResponse: [] })

    render(<ConferenceDetailsPage />)

    expect(await screen.findByText('Nema prijavljenih korisnika')).toBeInTheDocument()
  })

  it('filters registrations by participant first name', async () => {
    render(<ConferenceDetailsPage />)

    const input = await screen.findByPlaceholderText(/Pretra/)
    await userEvent.type(input, 'Drugi')

    expect(screen.getByText('Drugi')).toBeInTheDocument()
    expect(screen.getByText('drugi@test.com')).toBeInTheDocument()
    expect(screen.queryByText('Test')).not.toBeInTheDocument()
  })

  it('filters registrations by participant email', async () => {
    render(<ConferenceDetailsPage />)

    const input = await screen.findByPlaceholderText(/Pretra/)
    await userEvent.type(input, 'drugi@test.com')

    expect(screen.getByText('drugi@test.com')).toBeInTheDocument()
    expect(screen.queryByText('ucesnik@test.com')).not.toBeInTheDocument()
  })

  it('filters registrations by registration status', async () => {
    render(<ConferenceDetailsPage />)

    const statusSelect = await screen.findByDisplayValue('Svi statusi')
    await userEvent.selectOptions(statusSelect, 'Pending')

    await waitFor(() => {
      expect(screen.getByText('Drugi')).toBeInTheDocument()
      expect(screen.getAllByText('Pending').length).toBeGreaterThan(0)
      expect(screen.queryByText('Test')).not.toBeInTheDocument()
    })
  })

  it('shows participants fetch error message when participants API fails', async () => {
    mockConferenceFetch({ registrationsOk: false })

    render(<ConferenceDetailsPage />)

    expect(await screen.findByText(/dohvatanju prijava/)).toBeInTheDocument()
  })

  it('allows organizer role to see capacity and participants after successful load', async () => {
    mockRole = 'organizator'

    render(<ConferenceDetailsPage />)

    expect(await screen.findByText('Kapacitet konferencije')).toBeInTheDocument()
    expect(await screen.findByText('ucesnik@test.com')).toBeInTheDocument()
  })

  it('does not show capacity or participants to participant role', async () => {
    mockRole = 'ucesnik'

    render(<ConferenceDetailsPage />)

    expect(await screen.findAllByText('AI Summit Sarajevo')).toHaveLength(2)
    expect(screen.queryByText('Kapacitet konferencije')).not.toBeInTheDocument()
    expect(screen.queryByText(/Prijavljeni/)).not.toBeInTheDocument()
    expect(fetch).not.toHaveBeenCalled()
  })
})
