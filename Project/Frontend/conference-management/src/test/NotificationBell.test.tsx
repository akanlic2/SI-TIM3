import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi, beforeEach } from 'vitest'
import { NotificationBell } from '../features/notification/components/NotificationBell'

const markAsReadMock = vi.fn()
const markAllAsReadMock = vi.fn()

vi.mock('../features/notification/hooks/useNotifications', () => ({
  useNotifications: () => ({
    notifications: [
      {
        notificationId: 'notif-1',
        title: 'Novo pitanje u sesiji',
        content: 'Postavljeno je novo pitanje [conferenceId:abc-123]',
        notificationType: 'QuestionAsked',
        sentDate: new Date().toISOString(),
        isRead: false,
      },
      {
        notificationId: 'notif-2',
        title: 'Sesija otkazana',
        content: 'Sesija je otkazana.',
        notificationType: 'SessionCancelled',
        sentDate: new Date().toISOString(),
        isRead: true,
      },
    ],
    unreadCount: 1,
    markAsRead: markAsReadMock,
    markAllAsRead: markAllAsReadMock,
    refresh: vi.fn(),
  }),
}))

describe('NotificationBell', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('shows unread notifications counter', () => {
    render(<NotificationBell />)

    expect(screen.getByText('1')).toBeInTheDocument()
  })

  it('opens notification dropdown on click', async () => {
    render(<NotificationBell />)

    await userEvent.click(screen.getByRole('button'))

    expect(screen.getByText('Notifikacije')).toBeInTheDocument()
    expect(screen.getByText('Novo pitanje u sesiji')).toBeInTheDocument()
    expect(screen.getByText('Sesija otkazana')).toBeInTheDocument()
  })

  it('shows mark all as read button when unread notifications exist', async () => {
    render(<NotificationBell />)

    await userEvent.click(screen.getByRole('button'))

    expect(screen.getByText('Označi sve kao pročitano')).toBeInTheDocument()
  })

  it('calls markAllAsRead when clicking mark all button', async () => {
    render(<NotificationBell />)

    await userEvent.click(screen.getByRole('button'))
    await userEvent.click(screen.getByText('Označi sve kao pročitano'))

    expect(markAllAsReadMock).toHaveBeenCalledTimes(1)
  })

  it('marks unread notification as read on click', async () => {
    render(<NotificationBell />)

    await userEvent.click(screen.getByRole('button'))
    await userEvent.click(screen.getByText('Novo pitanje u sesiji'))

    expect(markAsReadMock).toHaveBeenCalledWith('notif-1')
  })
})