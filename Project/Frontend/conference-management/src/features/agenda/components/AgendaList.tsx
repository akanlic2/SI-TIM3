import { useMemo, useState } from 'react'
import type { AgendaItem } from '../types'
import { deleteAgendaItem } from '../api/agendaApi'

interface AgendaListProps {
  items: AgendaItem[]
  isAdminOrOrganizer: boolean
  onDeleteSuccess: () => void
  onEditClick: (item: AgendaItem) => void
}

export function AgendaList({
  items = [],
  isAdminOrOrganizer,
  onDeleteSuccess,
  onEditClick,
}: AgendaListProps) {
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [itemToDelete, setItemToDelete] = useState<AgendaItem | null>(null)
  const [activeItemId, setActiveItemId] = useState<string | null>(null)

  const TYPE_LABELS: Record<string, string> = {
    Session: 'Sesija',
    Break: 'Pauza',
    Lunch: 'Ručak',
    Networking: 'Umrežavanje',
    Opening: 'Otvaranje',
    Closing: 'Zatvaranje',
  }

  const getTypeLabel = (type: string) => TYPE_LABELS[type] ?? type

  const sortedItems = useMemo(() => {
    return [...items].sort((a, b) => {
      const left = new Date(a.startTime).getTime()
      const right = new Date(b.startTime).getTime()
      return left - right
    })
  }, [items])

  const formatDateOnly = (dateString: string) => {
    const date = new Date(dateString)
    if (Number.isNaN(date.getTime())) return dateString

    const day = String(date.getDate()).padStart(2, '0')
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const year = date.getFullYear()
    return `${day}.${month}.${year}`
  }

  const formatDateTime = (dateString: string) => {
    const dateOnly = formatDateOnly(dateString)
    const timeOnly = formatTime(dateString)
    return `${dateOnly} ${timeOnly}`
  }

  const formatTime = (dateString: string) => {
    try {
      return new Intl.DateTimeFormat('bs-BA', {
        hour: '2-digit',
        minute: '2-digit',
      }).format(new Date(dateString))
    } catch {
      return dateString
    }
  }

  const formatDateLabel = (dateString: string) => formatDateOnly(dateString)

  const getDayKey = (dateString: string) => {
    const date = new Date(dateString)
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  const activeItem = activeItemId
    ? sortedItems.find((item) => item.agendaItemId === activeItemId) ?? null
    : null

  const timelineRows = useMemo(() => {
    const rows: Array<
      | { type: 'day'; key: string; label: string }
      | { type: 'item'; item: AgendaItem }
    > = []

    let lastDayKey = ''
    for (const item of sortedItems) {
      const dayKey = getDayKey(item.startTime)
      if (dayKey !== lastDayKey) {
        rows.push({ type: 'day', key: `day-${dayKey}`, label: formatDateLabel(item.startTime) })
        lastDayKey = dayKey
      }

      rows.push({ type: 'item', item })
    }

    return rows
  }, [sortedItems])

  const handleDelete = async (agendaItemId: string) => {
    try {
      await deleteAgendaItem(agendaItemId)
      onDeleteSuccess()
    } catch (error) {
      console.error('Delete failed:', error)
      alert('Greška prilikom brisanja. Provjerite konzolu.')
    }
  }

  if (!sortedItems || sortedItems.length === 0) {
    return (
      <div className="session-empty-state">
        <div className="session-empty-icon">📋</div>
        <h3 className="session-empty-title">Nema stavki u agendi</h3>
        <p>Stavke će se pojaviti ovdje čim budu dodane</p>
      </div>
    )
  }

  return (
    <>
      <div
        className="agenda-timeline-wrap"
        onMouseLeave={() => setActiveItemId(null)}
      >
        <div className="agenda-timeline">
          <div className="agenda-timeline-axis" />
          <ul className="agenda-timeline-list">
            {timelineRows.map((row) => {
              if (row.type === 'day') {
                return (
                  <li key={row.key} className="agenda-timeline-day">
                    <span>{row.label}</span>
                  </li>
                )
              }

              const { item } = row
              const isActive = item.agendaItemId === activeItemId
              const timeRange = `${formatTime(item.startTime)} - ${formatTime(item.endTime)}`

              return (
                <li
                  key={item.agendaItemId}
                  className={`agenda-timeline-item${isActive ? ' is-active' : ''}`}
                  onMouseEnter={() => setActiveItemId(item.agendaItemId)}
                >
                  <span className="agenda-timeline-point" />
                  <div className="agenda-timeline-time">{timeRange}</div>
                  <div className="agenda-timeline-label">{item.title}</div>
                </li>
              )
            })}
          </ul>
        </div>

        <div className="agenda-timeline-card-area">
          {activeItem ? (
            (() => {
              const description = activeItem.description?.trim()
                ? activeItem.description
                : 'Nema opisa.'
              const roomName = activeItem.roomName?.trim()
                ? activeItem.roomName
                : 'Nije dodijeljena'

              return (
                <div
                  className="session-card agenda-card"
                  onMouseEnter={() => setActiveItemId(activeItem.agendaItemId)}
                >
                  <div className="session-card-header">
                    <h3 className="session-card-title">{activeItem.title}</h3>
                    <span className="session-status session-status-planned">{getTypeLabel(activeItem.type)}</span>
                  </div>

                  <div className="session-card-info">
                    <div className="session-info-row">
                      <span className="session-info-icon">⏰</span>
                      <div className="session-info-content">
                        <span className="session-info-label">Početak</span>
                        <p className="session-info-value">{formatDateTime(activeItem.startTime)}</p>
                      </div>
                    </div>

                    <div className="session-info-row">
                      <span className="session-info-icon">🏁</span>
                      <div className="session-info-content">
                        <span className="session-info-label">Završetak</span>
                        <p className="session-info-value">{formatDateTime(activeItem.endTime)}</p>
                      </div>
                    </div>

                    <div className="session-info-row">
                      <span className="session-info-icon">🏷️</span>
                      <div className="session-info-content">
                        <span className="session-info-label">Tip</span>
                        <p className="session-info-value">{getTypeLabel(activeItem.type)}</p>
                      </div>
                    </div>

                    <div className="session-info-row">
                      <span className="session-info-icon">📝</span>
                      <div className="session-info-content">
                        <span className="session-info-label">Opis</span>
                        <p className="session-info-value">{description}</p>
                      </div>
                    </div>

                    <div className="session-info-row">
                      <span className="session-info-icon">🏢</span>
                      <div className="session-info-content">
                        <span className="session-info-label">Sala</span>
                        <p className="session-info-value">{roomName}</p>
                      </div>
                    </div>
                  </div>

                  {isAdminOrOrganizer && (
                    <div className="session-card-actions">
                      <button
                        onClick={() => onEditClick(activeItem)}
                        className="btn-edit"
                        style={{
                          backgroundColor: '#EAB308',
                          color: '#000',
                          borderRadius: '9999px',
                          padding: '8px 20px',
                          border: 'none',
                          cursor: 'pointer',
                        }}
                      >
                        Uredi
                      </button>
                      <button
                        onClick={() => {
                          setItemToDelete(activeItem)
                          setShowDeleteModal(true)
                        }}
                        className="btn-delete"
                        style={{
                          backgroundColor: '#EF4444',
                          color: '#fff',
                          borderRadius: '9999px',
                          padding: '8px 20px',
                          border: 'none',
                          cursor: 'pointer',
                        }}
                      >
                        Obriši
                      </button>
                    </div>
                  )}
                </div>
              )
            })()
          ) : (
            <div className="agenda-timeline-placeholder">
              Pređite mišem preko tačke na vremenskoj osi da vidite detalje.
            </div>
          )}
        </div>
      </div>

      {showDeleteModal && itemToDelete && (
        <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
          <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
            <h2 className="modal-title">Potvrda brisanja</h2>
            <p>Jeste li sigurni da želite obrisati ovu stavku agende?</p>
            <div className="form-actions">
              <button
                type="button"
                onClick={() => {
                  setShowDeleteModal(false)
                  setItemToDelete(null)
                }}
                className="btn-secondary"
              >
                Ne
              </button>
              <button
                type="button"
                onClick={() => {
                  void handleDelete(itemToDelete.agendaItemId)
                  setShowDeleteModal(false)
                  setItemToDelete(null)
                }}
                className="btn-delete"
              >
                Da
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  )
}
