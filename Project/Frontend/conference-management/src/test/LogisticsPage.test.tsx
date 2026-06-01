import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import LogisticsPage from '../pages/LogisticsPage'
import * as logisticsApi from '../features/logistics/api/logisticsApi'
import type { LogisticsTask } from '../features/logistics/types'

const authState = vi.hoisted(() => ({
  role: 'organizator',
  isLoading: false,
}))

vi.mock('../auth/AuthProvider', () => ({
  useAuth: () => ({
    user: {
      role: authState.role,
    },
    isLoading: authState.isLoading,
  }),
}))

vi.mock('../features/logistics/api/logisticsApi', () => ({
  fetchLogistics: vi.fn(),
  createLogisticsTask: vi.fn(),
  updateLogisticsTask: vi.fn(),
  deleteLogisticsTask: vi.fn(),
}))

const logisticsItems: LogisticsTask[] = [
  {
    logisticsTaskId: 'log-1',
    conferenceId: 'conf-1',
    title: 'Catering setup',
    description: 'Prepare coffee, snacks and lunch tables',
    taskType: 'Catering',
    dueDate: '2026-06-10T00:00:00.000Z',
    status: 'Pending',
  },
  {
    logisticsTaskId: 'log-2',
    conferenceId: 'conf-1',
    title: 'Transport plan',
    description: 'Coordinate shuttle schedule',
    taskType: 'Transport',
    dueDate: '2026-06-11T00:00:00.000Z',
    status: 'Completed',
  },
]

function setLogisticsRoute() {
  window.history.pushState({}, '', '/conferences/conf-1/logistics')
  window.location.pathname = '/conferences/conf-1/logistics'
  window.location.href = 'http://localhost:5173/conferences/conf-1/logistics'
}

function getLogisticsRow(title: string) {
  const row = screen.getByText(title).closest('.table-row')
  expect(row).not.toBeNull()
  return row as HTMLElement
}

async function openCreateForm() {
  await screen.findByText('Catering setup')
  await userEvent.click(screen.getByText('+ Kreiraj aktivnost'))
  expect(screen.getByText(/Nova logisti/i)).toBeInTheDocument()
}

async function fillLogisticsForm(title: string, description: string, type = 'Transport', status = 'InProgress') {
  const modal = screen.getByPlaceholderText('Unesite naslov aktivnosti').closest('.modal-content')
  expect(modal).not.toBeNull()
  const root = modal as HTMLElement
  const titleInput = within(root).getByPlaceholderText('Unesite naslov aktivnosti')
  const descriptionInput = within(root).getByPlaceholderText('Unesite opis aktivnosti')
  const selects = within(root).getAllByRole('combobox')
  const dateInput = root.querySelector('input[type="date"]') as HTMLInputElement | null

  expect(dateInput).not.toBeNull()
  await userEvent.clear(titleInput)
  await userEvent.type(titleInput, title)
  await userEvent.clear(descriptionInput)
  await userEvent.type(descriptionInput, description)
  await userEvent.selectOptions(selects[0], type)
  await userEvent.selectOptions(selects[1], status)
  await userEvent.type(dateInput as HTMLInputElement, '2026-06-20')
}

describe('LogisticsPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    authState.role = 'organizator'
    authState.isLoading = false
    setLogisticsRoute()
    vi.mocked(logisticsApi.fetchLogistics).mockResolvedValue(logisticsItems)
    vi.mocked(logisticsApi.createLogisticsTask).mockResolvedValue(logisticsItems[0])
    vi.mocked(logisticsApi.updateLogisticsTask).mockResolvedValue(logisticsItems[0])
    vi.mocked(logisticsApi.deleteLogisticsTask).mockResolvedValue(undefined)
  })

  it('renderuje listu logistickih aktivnosti iz API-ja sa nazivom, tipom, statusom i rokom', async () => {
    render(<LogisticsPage />)

    expect(await screen.findByText('Catering setup')).toBeInTheDocument()
    expect(screen.getByText('Transport plan')).toBeInTheDocument()
    const cateringRow = getLogisticsRow('Catering setup')
    const transportRow = getLogisticsRow('Transport plan')
    expect(within(cateringRow).getByText('Catering')).toBeInTheDocument()
    expect(within(transportRow).getByText('Transport')).toBeInTheDocument()
    expect(within(cateringRow).getByText('Na čekanju')).toBeInTheDocument()
    expect(within(transportRow).getByText('Završeno')).toBeInTheDocument()
    expect(within(cateringRow).getByText(/2026|6\/10\/2026|10\. 6\. 2026|10\/06\/2026/)).toBeInTheDocument()
    expect(logisticsApi.fetchLogistics).toHaveBeenCalledWith('conf-1', undefined)
  })

  it('prikazuje prazno stanje kada nema aktivnosti', async () => {
    vi.mocked(logisticsApi.fetchLogistics).mockResolvedValue([])

    render(<LogisticsPage />)

    expect(await screen.findByText(/Nema logisti/i)).toBeInTheDocument()
  })

  it('prikazuje loading state dok se aktivnosti ucitavaju', () => {
    vi.mocked(logisticsApi.fetchLogistics).mockImplementation(() => new Promise(() => {}))

    render(<LogisticsPage />)

    expect(screen.getByText('Učitavanje logističkih aktivnosti...')).toBeInTheDocument()
  })

  it('prikazuje error state ako API za listu padne', async () => {
    vi.mocked(logisticsApi.fetchLogistics).mockRejectedValue(new Error('load failed'))

    render(<LogisticsPage />)

    expect(await screen.findByText(/Gre/i)).toBeInTheDocument()
    expect(screen.getByText(/Nema logisti/i)).toBeInTheDocument()
  })

  it('filter po tipu aktivnosti poziva API sa taskType parametrom i azurira listu', async () => {
    vi.mocked(logisticsApi.fetchLogistics)
      .mockResolvedValueOnce(logisticsItems)
      .mockResolvedValueOnce([logisticsItems[1]])

    render(<LogisticsPage />)

    expect(await screen.findByText('Catering setup')).toBeInTheDocument()
    const filter = screen.getAllByRole('combobox')[0]
    await userEvent.selectOptions(filter, 'Transport')

    await waitFor(() => {
      expect(logisticsApi.fetchLogistics).toHaveBeenLastCalledWith('conf-1', 'Transport')
    })
    expect(await screen.findByText('Transport plan')).toBeInTheDocument()
    await waitFor(() => {
      expect(screen.queryByText('Catering setup')).not.toBeInTheDocument()
    })
  })

  it('dugme za detalje otvara modal sa nazivom, opisom, tipom, statusom i rokom', async () => {
    render(<LogisticsPage />)

    await screen.findByText('Catering setup')
    const row = getLogisticsRow('Catering setup')
    await userEvent.click(within(row).getByTitle('Pogledaj detalje'))

    const modal = screen.getByText('Detalji aktivnosti').closest('.modal-content')
    expect(modal).not.toBeNull()
    expect(within(modal as HTMLElement).getByText('Catering setup')).toBeInTheDocument()
    expect(within(modal as HTMLElement).getByText('Prepare coffee, snacks and lunch tables')).toBeInTheDocument()
    expect(within(modal as HTMLElement).getByText('Catering')).toBeInTheDocument()
    expect(within(modal as HTMLElement).getByText('Na čekanju')).toBeInTheDocument()
    expect(within(modal as HTMLElement).getByText(/2026|6\/10\/2026|10\. 6\. 2026|10\/06\/2026/)).toBeInTheDocument()
  })

  it('klik na kreiranje otvara formu sa obaveznim poljima i dropdown tipovima', async () => {
    render(<LogisticsPage />)

    await openCreateForm()
    const modal = screen.getByText(/Nova logisti/i).closest('.modal-content') as HTMLElement

    expect(within(modal).getByPlaceholderText('Unesite naslov aktivnosti')).toBeRequired()
    expect(within(modal).getByPlaceholderText('Unesite opis aktivnosti')).toBeRequired()
    expect(modal.querySelector('input[type="date"]')).toBeRequired()
    expect(within(modal).getByRole('option', { name: 'Catering' })).toBeInTheDocument()
    expect(within(modal).getByRole('option', { name: 'Transport' })).toBeInTheDocument()
    expect(within(modal).getByRole('option', { name: /Ostalo/i })).toBeInTheDocument()
  })

  it('submit validne forme poziva create API i osvjezava listu', async () => {
    const created: LogisticsTask = {
      ...logisticsItems[0],
      logisticsTaskId: 'log-3',
      title: 'Transport check',
      description: 'Confirm vehicles',
      taskType: 'Transport',
      status: 'InProgress',
      dueDate: '2026-06-20T00:00:00.000Z',
    }
    vi.mocked(logisticsApi.fetchLogistics)
      .mockResolvedValueOnce(logisticsItems)
      .mockResolvedValueOnce([...logisticsItems, created])
    vi.mocked(logisticsApi.createLogisticsTask).mockResolvedValue(created)

    render(<LogisticsPage />)

    await openCreateForm()
    await fillLogisticsForm('Transport check', 'Confirm vehicles')
    const modal = screen.getByPlaceholderText('Unesite naslov aktivnosti').closest('.modal-content') as HTMLElement
    await userEvent.click(within(modal).getByRole('button', { name: /^Kreiraj aktivnost$/i }))

    await waitFor(() => {
      expect(logisticsApi.createLogisticsTask).toHaveBeenCalledWith(
        'conf-1',
        expect.objectContaining({
          title: 'Transport check',
          description: 'Confirm vehicles',
          taskType: 'Transport',
          status: 'InProgress',
        })
      )
    })
    expect(await screen.findByText('Transport check')).toBeInTheDocument()
  })

  it('greska pri kreiranju prikazuje error poruku u formi', async () => {
    vi.mocked(logisticsApi.createLogisticsTask).mockRejectedValue({
      response: { data: { error: 'Create failed' } },
    })

    render(<LogisticsPage />)

    await openCreateForm()
    await fillLogisticsForm('Transport check', 'Confirm vehicles')
    const modal = screen.getByPlaceholderText('Unesite naslov aktivnosti').closest('.modal-content') as HTMLElement
    await userEvent.click(within(modal).getByRole('button', { name: /^Kreiraj aktivnost$/i }))

    expect(await screen.findByText('Create failed')).toBeInTheDocument()
  })

  it('edit otvara formu sa postojecim podacima i update osvjezava listu', async () => {
    const updated: LogisticsTask = {
      ...logisticsItems[0],
      title: 'Catering updated',
      description: 'Updated description',
      status: 'Completed',
    }
    vi.mocked(logisticsApi.fetchLogistics)
      .mockResolvedValueOnce(logisticsItems)
      .mockResolvedValueOnce([updated, logisticsItems[1]])
    vi.mocked(logisticsApi.updateLogisticsTask).mockResolvedValue(updated)

    render(<LogisticsPage />)

    await screen.findByText('Catering setup')
    const row = getLogisticsRow('Catering setup')
    await userEvent.click(within(row).getByText(/Uredi/i))

    expect(screen.getByText(/Uredi aktivnost/i)).toBeInTheDocument()
    const titleInput = screen.getByDisplayValue('Catering setup')
    const descriptionInput = screen.getByDisplayValue('Prepare coffee, snacks and lunch tables')
    await userEvent.clear(titleInput)
    await userEvent.type(titleInput, 'Catering updated')
    await userEvent.clear(descriptionInput)
    await userEvent.type(descriptionInput, 'Updated description')
    await userEvent.click(screen.getByRole('button', { name: /Sa.*uvaj izmjene/i }))

    await waitFor(() => {
      expect(logisticsApi.updateLogisticsTask).toHaveBeenCalledWith(
        'log-1',
        expect.objectContaining({
          title: 'Catering updated',
          description: 'Updated description',
        })
      )
    })
    expect(await screen.findByText('Catering updated')).toBeInTheDocument()
  })

  it('greska pri update-u prikazuje error poruku u formi', async () => {
    vi.mocked(logisticsApi.updateLogisticsTask).mockRejectedValue({
      response: { data: { error: 'Update failed' } },
    })

    render(<LogisticsPage />)

    await screen.findByText('Catering setup')
    const row = getLogisticsRow('Catering setup')
    await userEvent.click(within(row).getByText(/Uredi/i))
    await userEvent.click(screen.getByRole('button', { name: /Sa.*uvaj izmjene/i }))

    expect(await screen.findByText('Update failed')).toBeInTheDocument()
  })

  it('delete otvara modal potvrde, poziva delete API i uklanja aktivnost nakon refresh-a', async () => {
    vi.mocked(logisticsApi.fetchLogistics)
      .mockResolvedValueOnce(logisticsItems)
      .mockResolvedValueOnce([logisticsItems[1]])

    render(<LogisticsPage />)

    await screen.findByText('Catering setup')
    const row = getLogisticsRow('Catering setup')
    await userEvent.click(within(row).getByText(/Obri/i))

    expect(screen.getByText('Potvrda brisanja')).toBeInTheDocument()
    await userEvent.click(screen.getByRole('button', { name: /^Obri/i }))

    await waitFor(() => {
      expect(logisticsApi.deleteLogisticsTask).toHaveBeenCalledWith('log-1')
    })
    await waitFor(() => {
      expect(screen.queryByText('Catering setup')).not.toBeInTheDocument()
    })
    expect(screen.getByText('Transport plan')).toBeInTheDocument()
  })

  it('greska pri brisanju prikazuje alert ako delete API padne', async () => {
    const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => undefined)
    vi.mocked(logisticsApi.deleteLogisticsTask).mockRejectedValue(new Error('Delete failed'))

    render(<LogisticsPage />)

    await screen.findByText('Catering setup')
    const row = getLogisticsRow('Catering setup')
    await userEvent.click(within(row).getByText(/Obri/i))
    await userEvent.click(screen.getByRole('button', { name: /^Obri/i }))

    await waitFor(() => {
      expect(alertSpy).toHaveBeenCalledWith(expect.stringMatching(/Gre/i))
    })
  })

  it('admin ili organizator vidi create edit i delete akcije', async () => {
    authState.role = 'admin-sistema'

    render(<LogisticsPage />)

    expect(await screen.findByText('+ Kreiraj aktivnost')).toBeInTheDocument()
    const row = getLogisticsRow('Catering setup')
    expect(within(row).getByText(/Uredi/i)).toBeInTheDocument()
    expect(within(row).getByText(/Obri/i)).toBeInTheDocument()
  })

  it('ucesnik ne vidi create edit delete niti detalje u trenutnoj implementaciji', async () => {
    authState.role = 'ucesnik'

    render(<LogisticsPage />)

    expect(await screen.findByText('Catering setup')).toBeInTheDocument()
    expect(screen.queryByText('+ Kreiraj aktivnost')).not.toBeInTheDocument()
    const row = getLogisticsRow('Catering setup')
    expect(within(row).queryByText(/Uredi/i)).not.toBeInTheDocument()
    expect(within(row).queryByText(/Obri/i)).not.toBeInTheDocument()
    expect(within(row).queryByTitle('Pogledaj detalje')).not.toBeInTheDocument()
  })

  it('predavac ne vidi create edit delete akcije', async () => {
    authState.role = 'predavac'

    render(<LogisticsPage />)

    expect(await screen.findByText('Catering setup')).toBeInTheDocument()
    expect(screen.queryByText('+ Kreiraj aktivnost')).not.toBeInTheDocument()
    const row = getLogisticsRow('Catering setup')
    expect(within(row).queryByText(/Uredi/i)).not.toBeInTheDocument()
    expect(within(row).queryByText(/Obri/i)).not.toBeInTheDocument()
  })
})
