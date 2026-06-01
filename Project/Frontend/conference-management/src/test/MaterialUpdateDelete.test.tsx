import { render, screen, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import SessionDetailsPage from '../pages/SessionDetailsPage'
import * as sessionApi from '../features/session/api/sessionApi'
import type { SessionMaterial } from '../features/session/types'

const authState = vi.hoisted(() => ({
  role: 'organizator',
  isLoading: false,
  token: 'test-token',
}))

const equipmentRefreshMock = vi.hoisted(() => vi.fn())

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: authState.role,
    },
    token: authState.token,
    isLoading: authState.isLoading,
  }),
}))

vi.mock('../features/session/api/sessionApi', async () => {
  const actual = await vi.importActual<typeof import('../features/session/api/sessionApi')>('../features/session/api/sessionApi')

  return {
    ...actual,
    fetchSessionMaterials: vi.fn(),
    uploadSessionMaterial: vi.fn(),
  }
})

vi.mock('../features/equipment/hooks/useEquipment', () => ({
  useSessionEquipment: () => ({
    items: [],
    isLoading: false,
    error: null,
    refresh: equipmentRefreshMock,
  }),
}))

vi.mock('../features/equipment/api/equipmentApi', () => ({
  unassignEquipmentFromSession: vi.fn(),
}))

const materials: SessionMaterial[] = [
  {
    materialId: 'mat-1',
    title: 'Architecture Slides',
    description: 'Deck for the keynote session',
    fileUrl: '/uploads/materials/architecture.pdf',
    materialType: 'application/pdf',
    uploadDate: '2026-06-01T10:00:00Z',
  },
  {
    materialId: 'mat-2',
    title: 'Workshop Notes',
    description: '',
    fileUrl: '/uploads/materials/workshop.pdf',
    materialType: 'application/pdf',
    uploadDate: '2026-06-01T11:00:00Z',
  },
]

function setSessionRoute() {
  window.history.pushState({}, '', '/sessions/session-1')
  window.location.pathname = '/sessions/session-1'
  window.location.href = 'http://localhost:5173/sessions/session-1'
}

function mockSessionDetailsFetch() {
  global.fetch = vi.fn().mockResolvedValue({
    ok: true,
    json: vi.fn().mockResolvedValue({
      sessionId: 'session-1',
      title: 'Keynote session',
      description: 'Main lecture',
      startTime: '2026-06-10T10:00:00Z',
      endTime: '2026-06-10T11:00:00Z',
      sessionType: 'Lecture',
      status: 'Planned',
      conferenceTitle: 'Conference 2026',
      roomName: 'Main Hall',
    }),
  })
}

async function renderSessionDetails() {
  render(<SessionDetailsPage />)
  expect(await screen.findByText('Keynote session')).toBeInTheDocument()
}

describe('Material update/delete frontend state', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    authState.role = 'organizator'
    authState.isLoading = false
    authState.token = 'test-token'
    setSessionRoute()
    mockSessionDetailsFetch()
    vi.mocked(sessionApi.fetchSessionMaterials).mockResolvedValue(materials)
    vi.spyOn(window, 'open').mockImplementation(() => null)
  })

  it('SessionDetailsPage prikazuje listu materijala sa nazivom opisom i download dugmetom', async () => {
    await renderSessionDetails()

    expect(await screen.findByText('Architecture Slides')).toBeInTheDocument()
    expect(screen.getByText('Deck for the keynote session')).toBeInTheDocument()
    expect(screen.getByText('Workshop Notes')).toBeInTheDocument()
    expect(screen.getAllByText('Preuzmi')).toHaveLength(2)
    expect(sessionApi.fetchSessionMaterials).toHaveBeenCalledWith('session-1', 'test-token')
  })

  it('download dugme otvara fileUrl materijala u novom tabu', async () => {
    await renderSessionDetails()
    await screen.findByText('Architecture Slides')

    await userEvent.click(screen.getAllByText('Preuzmi')[0])

    expect(window.open).toHaveBeenCalledWith(
      'http://localhost:8082/uploads/materials/architecture.pdf',
      '_blank'
    )
  })

  it('prikazuje prazno stanje kada sesija nema materijala', async () => {
    vi.mocked(sessionApi.fetchSessionMaterials).mockResolvedValue([])

    await renderSessionDetails()

    expect(await screen.findByText('Nema materijala')).toBeInTheDocument()
  })

  it('prikazuje upload modal koji vec postoji u trenutnom UI-ju', async () => {
    await renderSessionDetails()

    await userEvent.click(screen.getByRole('button', { name: 'Upload Materijala' }))
    const modal = screen.getByRole('heading', { name: 'Upload Materijala' }).closest('.modal-content') as HTMLElement

    expect(modal).toBeInTheDocument()
    expect(within(modal).getByText('Naziv')).toBeInTheDocument()
    expect(within(modal).getByText('Opis')).toBeInTheDocument()
    expect(within(modal).getByText('Odabir fajla')).toBeInTheDocument()
    expect(within(modal).getByText('Upload')).toBeInTheDocument()
  })

  it('edit akcija za materijal trenutno nije prisutna u UI-ju', async () => {
    await renderSessionDetails()
    await screen.findByText('Architecture Slides')

    expect(screen.queryByRole('button', { name: /edit|uredi|izmijeni/i })).not.toBeInTheDocument()
    expect(screen.queryByText(/sačuvaj izmjene|uredi materijal/i)).not.toBeInTheDocument()
  })

  it('delete akcija i modal potvrde za materijal trenutno nisu prisutni u UI-ju', async () => {
    await renderSessionDetails()
    await screen.findByText('Architecture Slides')

    expect(screen.queryByRole('button', { name: /delete|obriši|obrisi|ukloni/i })).not.toBeInTheDocument()
    expect(screen.queryByText(/potvrda brisanja|želite li zaista/i)).not.toBeInTheDocument()
  })

  it('role-based edit/delete UI za materijale nije implementiran ni za admina ni za predavaca', async () => {
    authState.role = 'admin-sistema'
    await renderSessionDetails()
    await screen.findByText('Architecture Slides')

    expect(screen.queryByRole('button', { name: /edit|uredi|izmijeni/i })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /delete|obriši|obrisi|ukloni/i })).not.toBeInTheDocument()
  })

  it('sessionApi trenutno nema exportovane update/delete funkcije za materijale', () => {
    expect('updateSessionMaterial' in sessionApi).toBe(false)
    expect('deleteSessionMaterial' in sessionApi).toBe(false)
    expect('updateMaterial' in sessionApi).toBe(false)
    expect('deleteMaterial' in sessionApi).toBe(false)
  })
})
