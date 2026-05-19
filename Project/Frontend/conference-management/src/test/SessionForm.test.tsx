import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { SessionForm } from '../features/session/components/SessionForm'
import * as sessionApi from '../features/session/api/sessionApi'
import axios from 'axios'

vi.mock('../features/session/api/sessionApi', () => ({
  createSession: vi.fn(),
  updateSession: vi.fn(),
  assignSpeaker: vi.fn(),
  fetchUsers: vi.fn(),
  assignRoomToSession: vi.fn(),
}))

async function fillRequiredSessionFields(container: HTMLElement, title = 'React radionica') {
  const textboxes = screen.getAllByRole('textbox')
  await userEvent.type(screen.getByPlaceholderText('npr. Uvod u React'), title)
  await userEvent.type(textboxes[1], 'Detaljan opis React radionice')
  const dateInputs = container.querySelectorAll('input[type="datetime-local"]')
  fireEvent.change(dateInputs[0], { target: { value: '2026-06-10T10:00' } })
  fireEvent.change(dateInputs[1], { target: { value: '2026-06-10T12:00' } })
}

describe('SessionForm', () => {
  beforeEach(() => {
    vi.clearAllMocks()

    vi.spyOn(axios, 'get').mockResolvedValue({
      data: [{ roomId: '11111111-1111-1111-1111-111111111111', name: 'Sala A' }],
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

  it('smoke: renderuje session form sa dropdownom za dvorane i submit dugmetom', async () => {
    render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )

    expect(screen.getByText('Naziv sesije')).toBeInTheDocument()
    expect(await screen.findByText('Odaberite salu')).toBeInTheDocument()
    expect(screen.getByText(/sesiju$/i)).toBeInTheDocument()
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
    expect(screen.getAllByText(/Predava/).length).toBeGreaterThan(0)
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
    await fillRequiredSessionFields(container, 'ab')
    const selects = screen.getAllByRole('combobox')
    await userEvent.selectOptions(selects[1], '11111111-1111-1111-1111-111111111111')
    await userEvent.click(screen.getByText(/sesiju$/i))
    expect(await screen.findByText(/Naziv mora/)).toBeInTheDocument()
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
    await fillRequiredSessionFields(container)
    const selects = screen.getAllByRole('combobox')
    await userEvent.selectOptions(selects[1], '11111111-1111-1111-1111-111111111111')
    await userEvent.selectOptions(selects[2], 'speaker-1')
    await userEvent.click(screen.getByText(/sesiju$/i))
    await waitFor(() => {
      expect(sessionApi.createSession).toHaveBeenCalled()
      expect(sessionApi.assignSpeaker).toHaveBeenCalledWith('session-1', { userId: 'speaker-1' })
      expect(onSuccess).toHaveBeenCalled()
    })
  })

  it('loads rooms and shows room dropdown options', async () => {
    render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )

    expect(await screen.findByText('Sala A')).toBeInTheDocument()
    expect(axios.get).toHaveBeenCalledWith('/api/rooms')
    expect(screen.getByText('Odaberite salu')).toBeInTheDocument()
  })

  it('allows user to select a room', async () => {
    render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )

    await screen.findByText('Sala A')
    const selects = screen.getAllByRole('combobox')
    await userEvent.selectOptions(selects[1], '11111111-1111-1111-1111-111111111111')

    expect(selects[1]).toHaveValue('11111111-1111-1111-1111-111111111111')
  })

  it('calls assignRoomToSession on submit when room is selected', async () => {
    vi.mocked(sessionApi.createSession).mockResolvedValue({
      sessionId: 'session-1',
    })
    const { container } = render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )

    await fillRequiredSessionFields(container)
    const selects = screen.getAllByRole('combobox')
    await userEvent.selectOptions(selects[1], '11111111-1111-1111-1111-111111111111')
    await userEvent.click(screen.getByText(/sesiju$/i))

    await waitFor(() => {
      expect(sessionApi.assignRoomToSession).toHaveBeenCalledWith(
        'session-1',
        '11111111-1111-1111-1111-111111111111'
      )
    })
  })

  it('does not call assignRoomToSession when room is not selected', async () => {
    const { container } = render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )

    await fillRequiredSessionFields(container)
    await userEvent.click(screen.getByText(/sesiju$/i))

    expect(sessionApi.createSession).not.toHaveBeenCalled()
    expect(sessionApi.assignRoomToSession).not.toHaveBeenCalled()
  })

  it('shows error when rooms fail to load', async () => {
    vi.mocked(axios.get).mockRejectedValue(new Error('rooms failed'))

    render(
      <SessionForm
        conferenceId="conf-1"
        editingSession={null}
        onSuccess={vi.fn()}
        onCancel={vi.fn()}
      />
    )

    expect(await screen.findByText(/Gre.*ka pri u.*itavanju dvorana/)).toBeInTheDocument()
  })
})
