import type { Session } from '../types'
import { deleteSession } from '../api/sessionApi'
import { useAuth } from '../../../auth/AuthProvider'
import { useState } from 'react'

interface SessionListProps {
  sessions: Session[]
  conferenceId: string
  isAdminOrOrganizer: boolean
  onDeleteSuccess: () => void
  onEditClick: (session: Session) => void
}

export function SessionList({
  sessions = [],
  conferenceId,
  isAdminOrOrganizer,
  onDeleteSuccess,
  onEditClick,
}: SessionListProps) {
  const { user } = useAuth()

  const role = user?.role?.toLowerCase() ?? ''

  const isParticipant = role === 'ucesnik'
  const isSpeaker = role === 'predavac'

  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [sessionToDelete, setSessionToDelete] = useState<string | null>(null);

  // Filter sessions based on role
  const filteredSessions = sessions.filter(session => {
    if (isAdminOrOrganizer) return true;
    if (isParticipant) return true;
    if (isSpeaker) return session.speakerName === `${user?.firstName} ${user?.lastName}`;
    return true;
  });

  const handleDelete = async (id: string) => {
    try {
      await deleteSession(id)
      onDeleteSuccess()
    } catch (error) {
      console.error('Delete failed:', error)
      alert('Greška prilikom brisanja. Provjerite konzolu.')
    }
  }

  const formatDateTime = (dateString: string) => {
    try {
      return new Intl.DateTimeFormat('bs-BA', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(new Date(dateString))
    } catch {
      return dateString
    }
  }

  if (!filteredSessions || filteredSessions.length === 0) {
    return (
      <div className="session-empty-state">
        <div className="session-empty-icon">📅</div>
        <h3>Nema pronađenih sesija</h3>
        <p>Sesije će se pojaviti ovdje čim budu dodane</p>
      </div>
    )
  }

  return (
    <>
    <div className="session-grid">
      {filteredSessions.map((session) => (
        <div
          key={session.sessionId}
          className="session-card"
        >
          {/* Header */}
          <div className="session-card-header">
            <h3 className="session-card-title">{session.title}</h3>

            <span
              className={`session-status session-status-${session.status.toLowerCase()}`}
            >
              {session.status}
            </span>
          </div>

          {/* Speaker Badge */}
          {isSpeaker && (
            <div style={{ marginBottom: '12px' }}>
              <span
                style={{
                  backgroundColor: '#2563EB',
                  color: 'white',
                  padding: '6px 12px',
                  borderRadius: '9999px',
                  fontSize: '12px',
                  fontWeight: 600,
                }}
              >
                Predavač
              </span>
            </div>
          )}

          {/* Info */}
          <div className="session-card-info">
            <div
              className="session-info-row"
              style={{
                display: 'flex',
                alignItems: 'flex-start',
                gap: '0.75rem',
                flexWrap: 'wrap',
              }}
            >
              <span className="session-info-icon">⏰</span>

              <div
                className="session-info-content"
                style={{
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '0.15rem',
                }}
              >
                <span className="session-info-label">Početak</span>
                <p className="session-info-value">
                  {formatDateTime(session.startTime)}
                </p>
              </div>
            </div>

            <div
              className="session-info-row"
              style={{
                display: 'flex',
                alignItems: 'flex-start',
                gap: '0.75rem',
                flexWrap: 'wrap',
              }}
            >
              <span className="session-info-icon">🏁</span>

              <div
                className="session-info-content"
                style={{
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '0.15rem',
                }}
              >
                <span className="session-info-label">Završetak</span>
                <p className="session-info-value">
                  {formatDateTime(session.endTime)}
                </p>
              </div>
            </div>

            <div
              className="session-info-row"
              style={{
                display: 'flex',
                alignItems: 'flex-start',
                gap: '0.75rem',
                flexWrap: 'wrap',
              }}
            >
              <span className="session-info-icon">🏷️</span>

              <div
                className="session-info-content"
                style={{
                  display: 'flex',
                  flexDirection: 'column',
                  gap: '0.15rem',
                }}
              >
                <span className="session-info-label">Tip sesije</span>
                <p className="session-info-value">{session.sessionType}</p>
              </div>
            </div>

            {session.speakerName && (
              <div
                className="session-info-row"
                style={{
                  display: 'flex',
                  alignItems: 'flex-start',
                  gap: '0.75rem',
                  flexWrap: 'wrap',
                }}
              >
                <span className="session-info-icon">🎤</span>

                <div
                  className="session-info-content"
                  style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '0.15rem',
                  }}
                >
                  <span className="session-info-label">Predavač</span>
                  <p className="session-info-value">{session.speakerName}</p>
                </div>
              </div>
            )}
          </div>

          {/* Actions */}
          <div
            className="session-card-actions"
            style={{
              display: 'flex',
              justifyContent: 'flex-end',
              gap: '12px',
              paddingTop: '16px',
              borderTop: '1px solid rgba(148,163,184,0.2)',
              flexWrap: 'wrap',
            }}
          >
            {/* Admin / Organizer */}
            {isAdminOrOrganizer && (
              <>
                <button
                  onClick={() => onEditClick(session)}
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
                    setSessionToDelete(session.sessionId);
                    setShowDeleteModal(true);
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
              </>
            )}
          </div>
        </div>
      ))}
    </div>

    {showDeleteModal && (
      <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
        <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
          <h2 className="modal-title">Potvrda brisanja</h2>
          <p>Jeste li sigurni da želite obrisati ovu sesiju?</p>
          <div className="form-actions">
            <button type="button" onClick={() => { setShowDeleteModal(false); setSessionToDelete(null); }} className="btn-secondary">
              Ne
            </button>
            <button type="button" onClick={() => { if (sessionToDelete) handleDelete(sessionToDelete); setShowDeleteModal(false); setSessionToDelete(null); }} className="btn-delete">
              Da
            </button>
          </div>
        </div>
      </div>
    )}
    </>
  )
}