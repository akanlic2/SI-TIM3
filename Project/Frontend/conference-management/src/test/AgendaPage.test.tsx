import { fireEvent, render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import AgendaPage from '../pages/AgendaPage'
import * as agendaApi from '../features/agenda/api/agendaApi'
import * as sessionApi from '../features/session/api/sessionApi'
import * as roomApi from '../features/room/api/roomApi'
import type { AgendaItem } from '../features/agenda/types'

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: 'organizator',
    },
    isLoading: false,
  }),
}))

vi.mock('../features/agenda/api/agendaApi', () => ({
  fetchAgenda: vi.fn(),
  createAgendaItem: vi.fn(),
  updateAgendaItem: vi.fn(),
  deleteAgendaItem: vi.fn(),
}))

vi.mock('../features/session/api/sessionApi', () => ({
  fetchSessions: vi.fn(),
}))

vi.mock('../features/room/api/roomApi', () => ({
  fetchRooms: vi.fn(),
}))

const agendaItems: AgendaItem[] = [
  {
    agendaItemId: 'agenda-1',
    conferenceId: 'conf-1',
    sessionId: 'session-1',
    roomId: 'room-1',
    title: 'Keynote',
    description: 'Opening keynote',
    startTime: '2026-06-10T10:00:00Z',
    endTime: '2026-06-10T11:00:00Z',
    type: 'Session',
    createdAt: '2026-06-01T10:00:00Z',
    sessionTitle: 'Keynote',
    sessionType: 'Lecture',
    roomName: 'Sala A',
  },
]

const sessions = [
  {
    sessionId: 'session-1',
    title: 'Keynote',
    description: 'Opening keynote',
    startTime: '2026-06-10T10:00:00Z',
    endTime: '2026-06-10T11:00:00Z',
    sessionType: 'Lecture',
    status: 'Planned',
    roomId: 'room-1',
    roomName: 'Sala A',
  },
]

const rooms = [
  {
    roomId: 'room-1',
    name: 'Sala A',
    location: 'Prvi sprat',
    capacity: 100,
    description: 'Glavna sala',
  },
]

function setAgendaRoute() {
  window.history.pushState({}, '', '/conferences/conf-1/agenda')
  window.location.pathname = '/conferences/conf-1/agenda'
  window.location.href = 'http://localhost:5173/conferences/conf-1/agenda'
}

async function openAgendaDetails(title = 'Keynote') {
  const timelineItem = await screen.findByText(title)
  fireEvent.mouseEnter(timelineItem.closest('li') as HTMLElement)
  return screen.findByRole('heading', { name: title })
}

function getAgendaFormModal(title: string) {
  const modal = screen.getByText(title).closest('.modal-content')
  expect(modal).not.toBeNull()
  return modal as HTMLElement
}

describe('AgendaPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    setAgendaRoute()
    vi.mocked(agendaApi.fetchAgenda).mockResolvedValue(agendaItems)
    vi.mocked(sessionApi.fetchSessions).mockResolvedValue(sessions)
    vi.mocked(roomApi.fetchRooms).mockResolvedValue(rooms)
    vi.mocked(agendaApi.createAgendaItem).mockResolvedValue(agendaItems[0])
    vi.mocked(agendaApi.updateAgendaItem).mockResolvedValue(undefined)
    vi.mocked(agendaApi.deleteAgendaItem).mockResolvedValue(undefined)
  })

  it('smoke: renderuje agendu sa dugmetom za kreiranje i listom stavki', async () => {
    render(<AgendaPage />)

    expect(screen.getByText('+ Dodaj stavku')).toBeInTheDocument()
    expect(await screen.findByText('Keynote')).toBeInTheDocument()
  })

  it('ucitava i prikazuje agenda stavke', async () => {
    render(<AgendaPage />)

    expect(await screen.findByText('Keynote')).toBeInTheDocument()
    expect(agendaApi.fetchAgenda).toHaveBeenCalledWith('conf-1')
  })

  it('prikazuje prazno stanje ako agenda nije definisana', async () => {
    vi.mocked(agendaApi.fetchAgenda).mockResolvedValue([])

    render(<AgendaPage />)

    expect(await screen.findByText('Nema stavki u agendi')).toBeInTheDocument()
  })

  it('agenda stavka prikazuje naziv, termin, tip i dvoranu', async () => {
    render(<AgendaPage />)

    await openAgendaDetails()

    expect(screen.getByRole('heading', { name: 'Keynote' })).toBeInTheDocument()
    expect(screen.getAllByText('Sesija').length).toBeGreaterThan(0)
    expect(screen.getByText('Sala A')).toBeInTheDocument()
    expect(screen.getByText('10:00 - 11:00')).toBeInTheDocument() //novo
  })

  it('forma za kreiranje AgendaItema se prikazuje', async () => {
    render(<AgendaPage />)

    await screen.findByText('Keynote')
    await userEvent.click(screen.getByText('+ Dodaj stavku'))

    expect(screen.getByText('Nova stavka agende')).toBeInTheDocument()
    expect(screen.getByText(/Tip stavke/)).toBeInTheDocument()
  })

  it('ako je tip Session, prikazuje dropdown za postojecu sesiju', async () => {
    render(<AgendaPage />)

    await screen.findByText('Keynote')
    await userEvent.click(screen.getByText('+ Dodaj stavku'))

    expect(await screen.findByText(/Keynote \(/)).toBeInTheDocument()
    expect(screen.getByText('Odaberite sesiju')).toBeInTheDocument()
  })

  it('ako tip nije Session, omogucava unos naziva i opisa', async () => {
    render(<AgendaPage />)

    await screen.findByText('Keynote')
    await userEvent.click(screen.getByText('+ Dodaj stavku'))
    const modal = getAgendaFormModal('Nova stavka agende')
    const selects = within(modal).getAllByRole('combobox')
    await userEvent.selectOptions(selects[0], 'Break')

    expect(within(modal).getByPlaceholderText('Npr. Pauza za kafu')).toBeInTheDocument()
    expect(within(modal).getByPlaceholderText('Opcioni opis')).toBeInTheDocument()
  })

  it('submit poziva create agenda API', async () => {
    render(<AgendaPage />)

    await screen.findByText('Keynote')
    await userEvent.click(screen.getByText('+ Dodaj stavku'))
    const modal = getAgendaFormModal('Nova stavka agende')
    const selects = within(modal).getAllByRole('combobox')
    await userEvent.selectOptions(selects[0], 'Break')
    await userEvent.type(within(modal).getByPlaceholderText('Npr. Pauza za kafu'), 'Pauza za kafu')
    await userEvent.type(within(modal).getByPlaceholderText('Opcioni opis'), 'Kratka pauza')
    const dateInputs = modal.querySelectorAll('input[type="datetime-local"]')
    fireEvent.change(dateInputs[0], { target: { value: '2026-06-10T12:00' } })
    fireEvent.change(dateInputs[1], { target: { value: '2026-06-10T12:30' } })
    await userEvent.selectOptions(within(modal).getAllByRole('combobox')[1], 'room-1')
    await userEvent.click(within(modal).getByText('Spasi'))

    await waitFor(() => {
      expect(agendaApi.createAgendaItem).toHaveBeenCalledWith(
        'conf-1',
        expect.objectContaining({
          type: 'Break',
          title: 'Pauza za kafu',
          description: 'Kratka pauza',
          roomId: 'room-1',
        })
      )
    })
  })

  it('edit otvara formu sa postojecim podacima', async () => {
    render(<AgendaPage />)

    await openAgendaDetails()
    await userEvent.click(screen.getByText('Uredi'))

    const modal = getAgendaFormModal('Uredi stavku agende')
    expect(within(modal).getByText(/Tip stavke/)).toBeInTheDocument()
    expect(within(modal).getAllByRole('combobox')[0]).toHaveValue('Session')
  })

  it('update poziva API', async () => {
    render(<AgendaPage />)

    await openAgendaDetails()
    await userEvent.click(screen.getByText('Uredi'))
    const modal = getAgendaFormModal('Uredi stavku agende')
    await userEvent.click(within(modal).getByText('Spasi'))

    await waitFor(() => {
      expect(agendaApi.updateAgendaItem).toHaveBeenCalledWith(
        'agenda-1',
        expect.objectContaining({
          type: 'Session',
          sessionId: 'session-1',
        })
      )
    })
  })

  it('delete otvara potvrdu i poziva API delete', async () => {
    render(<AgendaPage />)

    await openAgendaDetails()
    await userEvent.click(screen.getByText(/Obri/i))

    expect(screen.getByText('Potvrda brisanja')).toBeInTheDocument()
    await userEvent.click(screen.getByText('Da'))

    await waitFor(() => {
      expect(agendaApi.deleteAgendaItem).toHaveBeenCalledWith('agenda-1')
    })
  })

  it('prikazuje error poruku za load ako agenda API odbije zahtjev', async () => {
    vi.mocked(agendaApi.fetchAgenda).mockRejectedValue(new Error('load failed'))

    render(<AgendaPage />)

    expect(await screen.findByText(/Gre.*ka pri u.*itavanju agende/)).toBeInTheDocument()
  })

  it('prikazuje error poruku za create ako API odbije zahtjev', async () => {
    vi.mocked(agendaApi.createAgendaItem).mockRejectedValue({
      response: { data: { error: 'Create failed' } },
    })

    render(<AgendaPage />)

    await screen.findByText('Keynote')
    await userEvent.click(screen.getByText('+ Dodaj stavku'))
    const modal = getAgendaFormModal('Nova stavka agende')
    const selects = within(modal).getAllByRole('combobox')
    await userEvent.selectOptions(selects[0], 'Break')
    await userEvent.type(within(modal).getByPlaceholderText('Npr. Pauza za kafu'), 'Pauza za kafu')
    const dateInputs = modal.querySelectorAll('input[type="datetime-local"]')
    fireEvent.change(dateInputs[0], { target: { value: '2026-06-10T12:00' } })
    fireEvent.change(dateInputs[1], { target: { value: '2026-06-10T12:30' } })
    await userEvent.click(within(modal).getByText('Spasi'))

    expect(await screen.findByText('Create failed')).toBeInTheDocument()
  })

  it('prikazuje error poruku za update ako API odbije zahtjev', async () => {
    vi.mocked(agendaApi.updateAgendaItem).mockRejectedValue({
      response: { data: { error: 'Update failed' } },
    })

    render(<AgendaPage />)

    await openAgendaDetails()
    await userEvent.click(screen.getByText('Uredi'))
    const modal = getAgendaFormModal('Uredi stavku agende')
    await userEvent.click(within(modal).getByText('Spasi'))

    expect(await screen.findByText('Update failed')).toBeInTheDocument()
  })

  it('prikazuje error za delete preko alerta ako API odbije zahtjev', async () => {
    const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => undefined)
    vi.mocked(agendaApi.deleteAgendaItem).mockRejectedValue(new Error('Delete failed'))

    render(<AgendaPage />)

    await openAgendaDetails()
    await userEvent.click(screen.getByText(/Obri/i))
    await userEvent.click(screen.getByText('Da'))

    await waitFor(() => {
      expect(alertSpy).toHaveBeenCalledWith(expect.stringMatching(/Gre.*ka prilikom brisanja/))
    })
  })
})
