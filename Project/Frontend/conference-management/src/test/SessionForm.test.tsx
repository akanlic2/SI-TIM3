import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi, beforeEach } from 'vitest'
import { SessionForm } from '../features/session/components/SessionForm'
import * as sessionApi from '../features/session/api/sessionApi'
import axios from 'axios'

vi.mock('../features/session/api/sessionApi', () => ({
  createSession: vi.fn(),
  updateSession: vi.fn(),
  assignSpeaker: vi.fn(),
  fetchUsers: vi.fn(),
}))

describe('SessionForm', () => {
  beforeEach(() => {
    vi.clearAllMocks()

    vi.spyOn(axios, 'get').mockResolvedValue({
      data: [{ id: '11111111-1111-1111-1111-111111111111', name: 'Sala A' }]
    })

    vi.mocked(sessionApi.fetchUsers).mockResolvedValue([
      {
        userId: 'speaker-1',
        username: 'predavac1',
        firstName: 'Test',
        lastName: 'Predavac',
        email: 'predavac@test.com',
        role: 'predavac',
      },
    ])
  })

  it('renders create session form', async () => {
    render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )
    expect(screen.getByText('Naziv sesije')).toBeInTheDocument()
    expect(screen.getByText('Opis')).toBeInTheDocument()
    expect(screen.getByText('Predavač')).toBeInTheDocument()
    expect(await screen.findByText(/Test\s+Predavac/)).toBeInTheDocument()
  })

  it('shows validation error for short title', async () => {
    const { container } = render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )
    await userEvent.type(screen.getByPlaceholderText('npr. Uvod u React'), 'ab')
    await userEvent.type(
      screen.getByPlaceholderText('O čemu se radi na ovoj sesiji...'),
      'Detaljan opis React radionice'
    )
    const dateInputs = container.querySelectorAll('input[type="datetime-local"]')
    fireEvent.change(dateInputs[0], { target: { value: '2026-06-10T10:00' } })
    fireEvent.change(dateInputs[1], { target: { value: '2026-06-10T12:00' } })
    const selects = screen.getAllByRole('combobox')
    await userEvent.selectOptions(selects[1], '11111111-1111-1111-1111-111111111111')
    await userEvent.click(screen.getByText('Sačuvaj sesiju'))
    expect(await screen.findByText('Naziv mora sadržati najmanje 3 karaktera')).toBeInTheDocument()
  })

  it('creates session with valid data', async () => {
    const onSuccess = vi.fn()
    vi.mocked(sessionApi.createSession).mockResolvedValue({
      sessionId: 'session-1',
    })
    const { container } = render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={onSuccess}
        onCancel={vi.fn()}
      />
    )
    await userEvent.type(screen.getByPlaceholderText('npr. Uvod u React'), 'React radionica')
    await userEvent.type(
      screen.getByPlaceholderText('O čemu se radi na ovoj sesiji...'),
      'Detaljan opis React radionice'
    )
    const dateInputs = container.querySelectorAll('input[type="datetime-local"]')
    fireEvent.change(dateInputs[0], { target: { value: '2026-06-10T10:00' } })
    fireEvent.change(dateInputs[1], { target: { value: '2026-06-10T12:00' } })
    const selects = screen.getAllByRole('combobox')
    await userEvent.selectOptions(selects[1], '11111111-1111-1111-1111-111111111111')
    await userEvent.selectOptions(selects[2], 'speaker-1')
    await userEvent.click(screen.getByText('Sačuvaj sesiju'))
    await waitFor(() => {
      expect(sessionApi.createSession).toHaveBeenCalled()
      expect(sessionApi.assignSpeaker).toHaveBeenCalledWith('session-1', { userId: 'speaker-1' })
      expect(onSuccess).toHaveBeenCalled()
    })
  })
})
