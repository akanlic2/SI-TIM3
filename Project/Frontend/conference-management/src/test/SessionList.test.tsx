import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi, beforeEach } from 'vitest'
import { SessionList } from '../features/session/components/SessionList'
import * as sessionApi from '../features/session/api/sessionApi'

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: 'ucesnik',
      firstName: 'Test',
      lastName: 'User',
    },
    token: 'test-token',
  }),
}))

vi.mock('../features/session/api/sessionApi', () => ({
  deleteSession: vi.fn(),
  fetchRegisteredSessions: vi.fn(),
  registerForSession: vi.fn(),
  cancelSessionRegistration: vi.fn(),
  fetchSessionMaterials: vi.fn(),
}))

const sessions = [
  {
    sessionId: 'session-1',
    title: 'React radionica',
    description: 'Uvod u React i TypeScript',
    startTime: '2026-06-10T10:00:00Z',
    endTime: '2026-06-10T12:00:00Z',
    sessionType: 'Workshop',
    status: 'Active',
    roomName: 'Amfiteatar 1',
    speakerName: 'Test Predavac',
  },
]

describe('SessionList', () => {
  beforeEach(() => {
    vi.clearAllMocks()

    vi.stubGlobal(
      'fetch',
      vi.fn(() =>
        Promise.resolve({
          ok: true,
          json: () => Promise.resolve([{ conferenceId: 'conf-1' }]),
        })
      )
    )

    vi.spyOn(window, 'alert').mockImplementation(() => {})

    vi.mocked(sessionApi.fetchRegisteredSessions).mockResolvedValue([])
    vi.mocked(sessionApi.fetchSessionMaterials).mockResolvedValue([])
  })

  it('renders session card', async () => {
    render(
      <SessionList
        sessions={sessions}
        conferenceId="conf-1"
        isAdminOrOrganizer={false}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    expect(screen.getByText('React radionica')).toBeInTheDocument()
    expect(screen.getByText(/Workshop/)).toBeInTheDocument()
    expect(screen.getByText(/Amfiteatar 1/)).toBeInTheDocument()
  })

  it('shows register button for participant', async () => {
    render(
      <SessionList
        sessions={sessions}
        conferenceId="conf-1"
        isAdminOrOrganizer={false}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    expect(await screen.findByText('Prijavi se')).toBeInTheDocument()
  })

  it('calls registerForSession when participant clicks register', async () => {
    vi.mocked(sessionApi.registerForSession).mockResolvedValue('Uspješna prijava na sesiju.')

    render(
      <SessionList
        sessions={sessions}
        conferenceId="conf-1"
        isAdminOrOrganizer={false}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    await userEvent.click(await screen.findByText('Prijavi se'))

    await waitFor(() => {
      expect(sessionApi.registerForSession).toHaveBeenCalledWith('session-1')
    })
  })

  it('shows edit and delete buttons for admin or organizer', () => {
    render(
      <SessionList
        sessions={sessions}
        conferenceId="conf-1"
        isAdminOrOrganizer={true}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    expect(screen.getAllByText('Uredi').length).toBeGreaterThan(0)
    expect(screen.getAllByText('Obriši').length).toBeGreaterThan(0)
  })

  it('opens delete confirmation modal', async () => {
    render(
      <SessionList
        sessions={sessions}
        conferenceId="conf-1"
        isAdminOrOrganizer={true}
        onDeleteSuccess={vi.fn()}
        onEditClick={vi.fn()}
      />
    )

    await userEvent.click(screen.getAllByText('Obriši')[0])

   expect(screen.getByText('Potvrda')).toBeInTheDocument()
expect(screen.getByText('Da')).toBeInTheDocument()
expect(screen.getByText('Ne')).toBeInTheDocument()
  })
})