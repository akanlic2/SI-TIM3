import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import EquipmentPage from '../pages/EquipmentPage'
import SessionDetailsPage from '../pages/SessionDetailsPage'
import * as equipmentApi from '../features/equipment/api/equipmentApi'
import type { Equipment } from '../features/equipment/types'

const authState = vi.hoisted(() => ({
  role: 'organizator',
  isLoading: false,
  token: 'test-token',
}))

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: authState.role,
    },
    token: authState.token,
    isLoading: authState.isLoading,
  }),
}))

vi.mock('../features/equipment/api/equipmentApi', () => ({
  fetchEquipment: vi.fn(),
  createEquipment: vi.fn(),
  deleteEquipment: vi.fn(),
  decrementEquipmentQuantity: vi.fn(),
  fetchSessionEquipment: vi.fn(),
  assignEquipmentToSession: vi.fn(),
  unassignEquipmentFromSession: vi.fn(),
}))

vi.mock('../features/session/hooks/useMaterials', () => ({
  useMaterials: () => ({
    items: [],
    isLoading: false,
    error: null,
    refresh: vi.fn(),
  }),
}))

const inventory: Equipment[] = [
  {
    equipmentId: 'eq-1',
    sessionId: null,
    name: 'Projector Epson',
    type: 'Video',
    quantity: 5,
    availableQuantity: 3,
    isAvailable: true,
    availabilityStatus: 'Available',
    createdAt: '2026-06-01T10:00:00Z',
  },
  {
    equipmentId: 'eq-2',
    sessionId: null,
    name: 'Wireless Microphone',
    type: 'Audio',
    quantity: 2,
    availableQuantity: 0,
    isAvailable: false,
    availabilityStatus: 'Unavailable',
    createdAt: '2026-06-01T11:00:00Z',
  },
]

const assignedEquipment: Equipment[] = [
  {
    equipmentId: 'assigned-1',
    sessionId: 'session-1',
    name: 'Projector Epson',
    type: 'Video',
    quantity: 2,
    availableQuantity: 0,
    isAvailable: false,
    availabilityStatus: 'Assigned',
    createdAt: '2026-06-01T12:00:00Z',
  },
]

function setEquipmentRoute() {
  window.history.pushState({}, '', '/equipment')
  window.location.pathname = '/equipment'
  window.location.href = 'http://localhost:5173/equipment'
}

function setSessionRoute() {
  window.history.pushState({}, '', '/sessions/session-1')
  window.location.pathname = '/sessions/session-1'
  window.location.href = 'http://localhost:5173/sessions/session-1'
}

function getEquipmentRow(name: string) {
  const row = screen.getByText(name).closest('.table-row')
  expect(row).not.toBeNull()
  return row as HTMLElement
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

async function openCreateEquipmentModal() {
  await screen.findByText('Projector Epson')
  await userEvent.click(screen.getByText('+ Dodaj opremu'))
  expect(screen.getByText('Nova oprema')).toBeInTheDocument()
}

async function fillCreateEquipmentForm(name: string, type: string, quantity: string) {
  const modal = screen.getByText('Nova oprema').closest('.modal-content') as HTMLElement
  const textboxes = within(modal).getAllByRole('textbox')
  const quantityInput = within(modal).getByRole('spinbutton')

  await userEvent.clear(textboxes[0])
  await userEvent.type(textboxes[0], name)
  await userEvent.clear(textboxes[1])
  await userEvent.type(textboxes[1], type)
  await userEvent.clear(quantityInput)
  await userEvent.type(quantityInput, quantity)
}

describe('EquipmentPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    authState.role = 'organizator'
    authState.isLoading = false
    authState.token = 'test-token'
    setEquipmentRoute()
    vi.mocked(equipmentApi.fetchEquipment).mockResolvedValue(inventory)
    vi.mocked(equipmentApi.createEquipment).mockResolvedValue(inventory[0])
    vi.mocked(equipmentApi.deleteEquipment).mockResolvedValue(undefined)
    vi.mocked(equipmentApi.decrementEquipmentQuantity).mockResolvedValue({
      ...inventory[0],
      quantity: 4,
      availableQuantity: 2,
    })
  })

  it('renderuje globalni inventar sa nazivom, tipom, kolicinama i statusom', async () => {
    render(<EquipmentPage />)

    expect(await screen.findByText('Projector Epson')).toBeInTheDocument()
    expect(screen.getByText('Wireless Microphone')).toBeInTheDocument()
    const projectorRow = getEquipmentRow('Projector Epson')
    const microphoneRow = getEquipmentRow('Wireless Microphone')

    expect(within(projectorRow).getByText('Video')).toBeInTheDocument()
    expect(within(projectorRow).getByText('5')).toBeInTheDocument()
    expect(within(projectorRow).getByText('3')).toBeInTheDocument()
    expect(within(projectorRow).getByText('Dostupno')).toBeInTheDocument()
    expect(within(microphoneRow).getByText('Audio')).toBeInTheDocument()
    expect(within(microphoneRow).getByText('Nedostupno')).toBeInTheDocument()
    expect(equipmentApi.fetchEquipment).toHaveBeenCalled()
  })

  it('prikazuje prazno stanje kada nema opreme', async () => {
    vi.mocked(equipmentApi.fetchEquipment).mockResolvedValue([])

    render(<EquipmentPage />)

    expect(await screen.findByText('Trenutno nema registrovane opreme.')).toBeInTheDocument()
  })

  it('prikazuje loading state dok se oprema ucitava', () => {
    vi.mocked(equipmentApi.fetchEquipment).mockImplementation(() => new Promise(() => {}))

    render(<EquipmentPage />)

    expect(screen.getByText(/opreme iz baze/i)).toBeInTheDocument()
  })

  it('prikazuje error state ako API za listu padne', async () => {
    vi.mocked(equipmentApi.fetchEquipment).mockRejectedValue(new Error('Network error'))

    render(<EquipmentPage />)

    expect(await screen.findByText('Greška pri učitavanju opreme.')).toBeInTheDocument()
    expect(screen.getByText('Trenutno nema registrovane opreme.')).toBeInTheDocument()
  })

  it('klik na kreiranje otvara modal sa poljima naziv tip i kolicina', async () => {
    render(<EquipmentPage />)

    await openCreateEquipmentModal()
    const modal = screen.getByText('Nova oprema').closest('.modal-content') as HTMLElement

    expect(within(modal).getByText('Naziv opreme')).toBeInTheDocument()
    expect(within(modal).getByText('Tip opreme')).toBeInTheDocument()
    expect(within(modal).getByText('Ukupna količina')).toBeInTheDocument()
    expect(within(modal).getByRole('spinbutton')).toHaveValue(1)
  })

  it('submit validne forme poziva createEquipment i osvjezava listu', async () => {
    const created: Equipment = {
      ...inventory[0],
      equipmentId: 'eq-3',
      name: 'Mixer Console',
      type: 'Audio',
      quantity: 1,
      availableQuantity: 1,
    }
    vi.mocked(equipmentApi.fetchEquipment)
      .mockResolvedValueOnce(inventory)
      .mockResolvedValueOnce([...inventory, created])
    vi.mocked(equipmentApi.createEquipment).mockResolvedValue(created)

    render(<EquipmentPage />)

    await openCreateEquipmentModal()
    await fillCreateEquipmentForm('Mixer Console', 'Audio', '1')
    await userEvent.click(screen.getByText('Sačuvaj opremu'))

    await waitFor(() => {
      expect(equipmentApi.createEquipment).toHaveBeenCalledWith({
        name: 'Mixer Console',
        type: 'Audio',
        quantity: 1,
      })
    })
    expect(await screen.findByText('Mixer Console')).toBeInTheDocument()
  })

  it('greska pri kreiranju prikazuje error poruku', async () => {
    vi.mocked(equipmentApi.createEquipment).mockRejectedValue(new Error('Create equipment failed'))

    render(<EquipmentPage />)

    await openCreateEquipmentModal()
    await fillCreateEquipmentForm('Mixer Console', 'Audio', '1')
    await userEvent.click(screen.getByText('Sačuvaj opremu'))

    expect(await screen.findByText('Create equipment failed')).toBeInTheDocument()
  })

  it('delete otvara potvrdu, poziva deleteEquipment i osvjezava listu', async () => {
    vi.mocked(equipmentApi.fetchEquipment)
      .mockResolvedValueOnce(inventory)
      .mockResolvedValueOnce([inventory[1]])

    render(<EquipmentPage />)

    await screen.findByText('Projector Epson')
    const row = getEquipmentRow('Projector Epson')
    await userEvent.click(within(row).getByText(/Obri/i))

    expect(screen.getByText('Potvrda brisanja')).toBeInTheDocument()
    await userEvent.click(screen.getByText('Da'))

    await waitFor(() => {
      expect(equipmentApi.deleteEquipment).toHaveBeenCalledWith('eq-1')
    })
    await waitFor(() => {
      expect(screen.queryByText('Projector Epson')).not.toBeInTheDocument()
    })
  })

  it('greska pri brisanju prikazuje error poruku u modalu', async () => {
    vi.mocked(equipmentApi.deleteEquipment).mockRejectedValue(new Error('Cannot delete assigned equipment'))

    render(<EquipmentPage />)

    await screen.findByText('Projector Epson')
    const row = getEquipmentRow('Projector Epson')
    await userEvent.click(within(row).getByText(/Obri/i))
    await userEvent.click(screen.getByText('Da'))

    expect(await screen.findByText('Cannot delete assigned equipment')).toBeInTheDocument()
  })

  it('decrement dugme poziva API za smanjenje ukupne kolicine i osvjezava listu', async () => {
    const confirmSpy = vi.spyOn(window, 'confirm').mockReturnValue(true)

    render(<EquipmentPage />)

    await screen.findByText('Projector Epson')
    const row = getEquipmentRow('Projector Epson')
    await userEvent.click(within(row).getByRole('button', { name: /Smanji ukupnu kolicinu opreme Projector Epson/i }))

    await waitFor(() => {
      expect(confirmSpy).toHaveBeenCalled()
      expect(equipmentApi.decrementEquipmentQuantity).toHaveBeenCalledWith('eq-1')
    })
  })

  it('admin ili organizator vidi create delete i decrement akcije', async () => {
    authState.role = 'admin-sistema'

    render(<EquipmentPage />)

    expect(await screen.findByText('+ Dodaj opremu')).toBeInTheDocument()
    const row = getEquipmentRow('Projector Epson')
    expect(within(row).getByText(/Obri/i)).toBeInTheDocument()
    expect(within(row).getByRole('button', { name: /Smanji ukupnu kolicinu/i })).toBeInTheDocument()
  })

  it('predavac nema pristup inventar stranici i ne vidi akcije', async () => {
    authState.role = 'predavac'
    const replaceSpy = vi.spyOn(window.history, 'replaceState')

    render(<EquipmentPage />)

    await waitFor(() => {
      expect(replaceSpy).toHaveBeenCalledWith({}, '', '/dashboard')
    })
    expect(screen.queryByText('+ Dodaj opremu')).not.toBeInTheDocument()
  })
})

describe('SessionDetailsPage equipment', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    authState.role = 'organizator'
    authState.isLoading = false
    authState.token = 'test-token'
    setSessionRoute()
    mockSessionDetailsFetch()
    vi.mocked(equipmentApi.fetchSessionEquipment).mockResolvedValue(assignedEquipment)
    vi.mocked(equipmentApi.fetchEquipment).mockResolvedValue(inventory)
    vi.mocked(equipmentApi.assignEquipmentToSession).mockResolvedValue(undefined)
    vi.mocked(equipmentApi.unassignEquipmentFromSession).mockResolvedValue(undefined)
  })

  it('prikazuje opremu dodijeljenu sesiji sa nazivom, tipom i kolicinom', async () => {
    render(<SessionDetailsPage />)

    expect(await screen.findByText('Keynote session')).toBeInTheDocument()
    expect(await screen.findByText('Projector Epson')).toBeInTheDocument()
    expect(screen.getByText('(Video)')).toBeInTheDocument()
    expect(screen.getByText((_, element) => element?.textContent === 'Količina: 2')).toBeInTheDocument()
    expect(equipmentApi.fetchSessionEquipment).toHaveBeenCalledWith('session-1')
  })

  it('dodjela opreme prikazuje dropdown sa dostupnom kolicinom i poziva assign API', async () => {
    vi.mocked(equipmentApi.fetchSessionEquipment)
      .mockResolvedValueOnce(assignedEquipment)
      .mockResolvedValueOnce([
        ...assignedEquipment,
        {
          ...inventory[0],
          equipmentId: 'assigned-2',
          sessionId: 'session-1',
          quantity: 1,
          availableQuantity: 0,
          availabilityStatus: 'Assigned',
          isAvailable: false,
        },
      ])

    render(<SessionDetailsPage />)

    await screen.findByText('Keynote session')
    await userEvent.click(screen.getByText('Dodijeli Opremu'))

    expect(await screen.findByText('Dodijeli opremu sesiji')).toBeInTheDocument()
    const modal = screen.getByText('Dodijeli opremu sesiji').closest('.modal-content') as HTMLElement
    const equipmentSelect = within(modal).getByRole('combobox')
    expect(within(equipmentSelect).getByText(/Projector Epson.*Dostupno: 3/)).toBeInTheDocument()
    await userEvent.selectOptions(equipmentSelect, 'eq-1')
    const quantityInput = within(modal).getByRole('spinbutton')
    expect(within(modal).getByText(/maksimalno 3/)).toBeInTheDocument()
    await userEvent.clear(quantityInput)
    await userEvent.type(quantityInput, '2')
    await userEvent.click(within(modal).getByText('Dodijeli opremu'))

    await waitFor(() => {
      expect(equipmentApi.assignEquipmentToSession).toHaveBeenCalledWith('session-1', {
        equipmentId: 'eq-1',
        quantity: 2,
      })
    })
  })

  it('validira kolicinu vecu od dostupne pri dodjeli', async () => {
    render(<SessionDetailsPage />)

    await screen.findByText('Keynote session')
    await userEvent.click(screen.getByText('Dodijeli Opremu'))
    const modal = await screen.findByText('Dodijeli opremu sesiji').then((node) => node.closest('.modal-content') as HTMLElement)
    await userEvent.selectOptions(within(modal).getByRole('combobox'), 'eq-1')
    const quantityInput = within(modal).getByRole('spinbutton')
    await userEvent.clear(quantityInput)
    await userEvent.type(quantityInput, '4')
    await userEvent.click(within(modal).getByText('Dodijeli opremu'))

    expect(quantityInput).toBeInvalid()
    expect(equipmentApi.assignEquipmentToSession).not.toHaveBeenCalled()
  })

  it('greska pri dodjeli prikazuje error poruku', async () => {
    vi.mocked(equipmentApi.assignEquipmentToSession).mockRejectedValue(new Error('Assign failed'))

    render(<SessionDetailsPage />)

    await screen.findByText('Keynote session')
    await userEvent.click(screen.getByText('Dodijeli Opremu'))
    const modal = await screen.findByText('Dodijeli opremu sesiji').then((node) => node.closest('.modal-content') as HTMLElement)
    await userEvent.selectOptions(within(modal).getByRole('combobox'), 'eq-1')
    await userEvent.click(within(modal).getByText('Dodijeli opremu'))

    expect(await screen.findByText('Assign failed')).toBeInTheDocument()
  })

  it('predavac vidi dodijeljenu opremu ali ne vidi assign akciju', async () => {
    authState.role = 'predavac'

    render(<SessionDetailsPage />)

    expect(await screen.findByText('Projector Epson')).toBeInTheDocument()
    expect(screen.queryByText('Dodijeli Opremu')).not.toBeInTheDocument()
    expect(screen.queryByText('Ukloni')).not.toBeInTheDocument()
  })
})
