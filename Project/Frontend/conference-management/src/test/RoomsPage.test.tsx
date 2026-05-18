import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import RoomsPage from '../pages/RoomsPage'
import * as roomApi from '../features/room/api/roomApi'
import type { Room } from '../features/room/types'

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: 'organizator',
    },
    token: 'test-token',
    isLoading: false,
  }),
}))

vi.mock('../features/room/api/roomApi', () => ({
  fetchRooms: vi.fn(),
}))

const initialRooms: Room[] = [
  {
    roomId: 'room-1',
    name: 'Sala A',
    location: 'Prvi sprat',
    capacity: 80,
    description: 'Velika konferencijska sala',
  },
  {
    roomId: 'room-2',
    name: 'Sala B',
    location: 'Drugi sprat',
    capacity: 40,
    description: 'Radionica',
  },
]

function mockFetchResponse(ok: boolean, data: unknown = {}) {
  return vi.fn().mockResolvedValue({
    ok,
    status: ok ? 200 : 400,
    json: vi.fn().mockResolvedValue(data),
  })
}

async function fillRoomForm(name: string, location: string, capacity: string, description: string) {
  const modal = screen.getByText('Nova dvorana').closest('.modal-content')
  expect(modal).not.toBeNull()
  const textboxes = within(modal as HTMLElement).getAllByRole('textbox')
  const capacityInput = within(modal as HTMLElement).getByRole('spinbutton')

  await userEvent.clear(textboxes[0])
  await userEvent.type(textboxes[0], name)
  await userEvent.clear(textboxes[1])
  await userEvent.type(textboxes[1], location)
  await userEvent.clear(capacityInput)
  await userEvent.type(capacityInput, capacity)
  await userEvent.clear(textboxes[2])
  await userEvent.type(textboxes[2], description)
}

function getAddRoomSubmitButton() {
  const modal = screen.getByText('Nova dvorana').closest('.modal-content')
  expect(modal).not.toBeNull()
  return within(modal as HTMLElement).getByRole('button', { name: /dvoranu$/i })
}

describe('RoomsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(roomApi.fetchRooms).mockResolvedValue(initialRooms)
    global.fetch = mockFetchResponse(true)
  })

  it('smoke: renderuje upravljanje dvoranama sa dugmetom za dodavanje i listom dvorana', async () => {
    render(<RoomsPage />)

    expect(screen.getByText('+ Dodaj dvoranu')).toBeInTheDocument()
    expect(await screen.findByText('Sala A')).toBeInTheDocument()
    expect(screen.getByText('Sala B')).toBeInTheDocument()
  })

  it('prikazuje listu dvorana dobijenu iz API-ja', async () => {
    render(<RoomsPage />)

    expect(await screen.findByText('Sala A')).toBeInTheDocument()
    expect(screen.getByText('Sala B')).toBeInTheDocument()
    expect(screen.getByText('Prvi sprat')).toBeInTheDocument()
    expect(screen.getByText('80')).toBeInTheDocument()
  })

  it('prikazuje poruku kada nema dvorana', async () => {
    vi.mocked(roomApi.fetchRooms).mockResolvedValue([])

    render(<RoomsPage />)

    expect(await screen.findByText('Nema dvorana')).toBeInTheDocument()
  })

  it('prikazuje loading state dok se dvorane ucitavaju', () => {
    vi.mocked(roomApi.fetchRooms).mockImplementation(() => new Promise(() => {}))

    render(<RoomsPage />)

    expect(screen.getByText(/dvorana iz baze/i)).toBeInTheDocument()
  })

  it('prikazuje gresku pri ucitavanju dvorana', async () => {
    vi.mocked(roomApi.fetchRooms).mockRejectedValue(new Error('Network error'))

    render(<RoomsPage />)

    expect(await screen.findByText('Greška pri učitavanju dvorana')).toBeInTheDocument()
    expect(screen.getByText('Nema dvorana')).toBeInTheDocument()
  })

  it('klik na dodavanje otvara AddRoomModal sa ocekivanim poljima', async () => {
    render(<RoomsPage />)

    await screen.findByText('Sala A')
    await userEvent.click(screen.getByText('+ Dodaj dvoranu'))

    expect(screen.getByText('Nova dvorana')).toBeInTheDocument()
    expect(screen.getByText('Naziv dvorane')).toBeInTheDocument()
    expect(screen.getByText('Lokacija')).toBeInTheDocument()
    expect(screen.getByText('Kapacitet')).toBeInTheDocument()
  })

  it('submit dodavanja poziva API i osvjezena lista prikazuje novu dvoranu', async () => {
    const refreshedRooms = [
      ...initialRooms,
      {
        roomId: 'room-3',
        name: 'Sala C',
        location: 'Treci sprat',
        capacity: 25,
        description: 'Mala sala',
      },
    ]
    vi.mocked(roomApi.fetchRooms)
      .mockResolvedValueOnce(initialRooms)
      .mockResolvedValueOnce(refreshedRooms)

    render(<RoomsPage />)

    await screen.findByText('Sala A')
    await userEvent.click(screen.getByText('+ Dodaj dvoranu'))
    await fillRoomForm('Sala C', 'Treci sprat', '25', 'Mala sala')
    await userEvent.click(getAddRoomSubmitButton())

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith('/api/rooms', expect.objectContaining({
        method: 'POST',
        body: JSON.stringify({
          name: 'Sala C',
          location: 'Treci sprat',
          capacity: 25,
          description: 'Mala sala',
        }),
      }))
    })
    expect(await screen.findByText('Sala C')).toBeInTheDocument()
  })

  it('prikazuje gresku pri dodavanju', async () => {
    global.fetch = mockFetchResponse(false, { error: 'Dvorana vec postoji' })

    render(<RoomsPage />)

    await screen.findByText('Sala A')
    await userEvent.click(screen.getByText('+ Dodaj dvoranu'))
    await fillRoomForm('Sala A', 'Prvi sprat', '80', 'Duplikat')
    await userEvent.click(getAddRoomSubmitButton())

    expect(await screen.findByText('Dvorana vec postoji')).toBeInTheDocument()
  })

  it('klik na edit otvara EditRoomModal sa postojecim podacima', async () => {
    render(<RoomsPage />)

    await screen.findByText('Sala A')
    const salaACard = screen.getByText('Sala A').closest('.session-card')
    expect(salaACard).not.toBeNull()
    await userEvent.click(within(salaACard as HTMLElement).getByText('Uredi'))

    expect(screen.getByText('Uredi dvoranu')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Sala A')).toBeInTheDocument()
    expect(screen.getByDisplayValue('Prvi sprat')).toBeInTheDocument()
    expect(screen.getByDisplayValue('80')).toBeInTheDocument()
  })

  it('izmjena podataka poziva API update i prikazuje promjenu nakon refresh-a', async () => {
    vi.mocked(roomApi.fetchRooms)
      .mockResolvedValueOnce(initialRooms)
      .mockResolvedValueOnce([
        {
          ...initialRooms[0],
          name: 'Sala A1',
          capacity: 90,
        },
        initialRooms[1],
      ])

    render(<RoomsPage />)

    await screen.findByText('Sala A')
    const salaACard = screen.getByText('Sala A').closest('.session-card')
    await userEvent.click(within(salaACard as HTMLElement).getByText('Uredi'))
    const nameInput = screen.getByDisplayValue('Sala A')
    await userEvent.clear(nameInput)
    await userEvent.type(nameInput, 'Sala A1')
    const capacityInput = screen.getByDisplayValue('80')
    await userEvent.clear(capacityInput)
    await userEvent.type(capacityInput, '90')
    await userEvent.click(screen.getByText(/promjene$/i))

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith('/api/rooms/room-1', expect.objectContaining({
        method: 'PUT',
      }))
    })
    expect(await screen.findByText('Sala A1')).toBeInTheDocument()
    expect(screen.getByText('90')).toBeInTheDocument()
  })

  it('prikazuje gresku pri izmjeni', async () => {
    global.fetch = mockFetchResponse(false, { error: 'Update nije dozvoljen' })

    render(<RoomsPage />)

    await screen.findByText('Sala A')
    const salaACard = screen.getByText('Sala A').closest('.session-card')
    await userEvent.click(within(salaACard as HTMLElement).getByText('Uredi'))
    await userEvent.click(screen.getByText(/promjene$/i))

    expect(await screen.findByText('Update nije dozvoljen')).toBeInTheDocument()
  })

  it('klik na delete otvara potvrdu i potvrda brisanja uklanja dvoranu iz liste', async () => {
    vi.mocked(roomApi.fetchRooms)
      .mockResolvedValueOnce(initialRooms)
      .mockResolvedValueOnce([initialRooms[1]])

    render(<RoomsPage />)

    await screen.findByText('Sala A')
    const salaACard = screen.getByText('Sala A').closest('.session-card')
    await userEvent.click(within(salaACard as HTMLElement).getByText(/Obri/i))

    expect(screen.getByText('Potvrda brisanja')).toBeInTheDocument()
    await userEvent.click(screen.getByText('Da'))

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith('/api/rooms/room-1', expect.objectContaining({
        method: 'DELETE',
      }))
    })
    await waitFor(() => {
      expect(screen.queryByText('Sala A')).not.toBeInTheDocument()
    })
    expect(screen.getByText('Sala B')).toBeInTheDocument()
  })

  it('ako delete API vrati gresku, prikazuje error poruku', async () => {
    global.fetch = mockFetchResponse(false, { error: 'Dvorana ima aktivne sesije' })

    render(<RoomsPage />)

    await screen.findByText('Sala A')
    const salaACard = screen.getByText('Sala A').closest('.session-card')
    await userEvent.click(within(salaACard as HTMLElement).getByText(/Obri/i))
    await userEvent.click(screen.getByText('Da'))

    expect(await screen.findByText('Dvorana ima aktivne sesije')).toBeInTheDocument()
  })
})
