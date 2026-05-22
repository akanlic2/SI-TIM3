import type { Session } from '../types'
import { cancelSessionRegistration, deleteSession, fetchRegisteredSessions, registerForSession } from '../api/sessionApi'
import { useAuth } from '../../../auth/AuthProvider'
import { useCallback, useEffect, useState } from 'react'
import { useMaterials } from '../hooks/useMaterials'
import { UploadMaterialModal } from './UploadMaterialModal'

interface SessionMaterialsSectionProps {
  sessionId: string
  refreshKey: number
}

function SessionMaterialsSection({ sessionId, refreshKey }: SessionMaterialsSectionProps) {
  const { items, isLoading, error, refresh } = useMaterials(sessionId)

  useEffect(() => {
    if (refreshKey > 0) {
      void refresh()
    }
  }, [refreshKey, refresh])

  return (
    <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
      <span className="session-info-icon">📎</span>
      <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
        <span className="session-info-label">MATERIJALI</span>
        {isLoading ? (
          <p className="session-info-value">Učitavanje materijala...</p>
        ) : error ? (
          <p className="session-info-value">{error}</p>
        ) : items.length === 0 ? (
          <p className="session-info-value">Nema materijala</p>
        ) : (
          items.map((material, index) => (
            <div key={material.materialId} style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
              <div>
                <strong>{material.title}</strong>
                {material.description && (
                  <p className="session-info-value" style={{ margin: '4px 0 0 0' }}>
                    {material.description}
                  </p>
                )}
              </div>
              <a
                href={`${import.meta.env.VITE_API_URL ?? 'http://localhost:8082'}${material.fileUrl}`}
                target="_blank"
                rel="noreferrer"
                className="btn-primary-sm"
                style={{ width: 'fit-content', padding: '8px 16px' }}
              >
                Preuzmi
              </a>
              {index < items.length - 1 && <hr style={{ borderColor: 'rgba(148,163,184,0.2)', margin: '8px 0' }} />}
            </div>
          ))
        )}
      </div>
    </div>
  )
}

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
  const { user, token } = useAuth()

  const role = user?.role?.toLowerCase() ?? ''

  const isParticipant = role === 'ucesnik'
  const isSpeaker = role === 'predavac'

  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [sessionToDelete, setSessionToDelete] = useState<string | null>(null);
  const [registeredConferenceIds, setRegisteredConferenceIds] = useState<Set<string>>(new Set());
  const [isLoadingRegistered, setIsLoadingRegistered] = useState(true);
  const [registeredSessions, setRegisteredSessions] = useState<Record<string, string>>({});
  const [isLoadingRegisteredSessions, setIsLoadingRegisteredSessions] = useState(true);
  const [cancellingSessionId, setCancellingSessionId] = useState<string | null>(null);

  // Filter sessions based on role
  const filteredSessions = sessions;

  useEffect(() => {
    if (!token) return;
    setIsLoadingRegistered(true);
    fetch('/api/Conference/registered', {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((r) => (r.ok ? r.json() : []))
      .then((data) => {
        const items = Array.isArray(data) ? data : [];
        const ids = new Set<string>(
          items.map((item: { conferenceId?: string }) => item.conferenceId).filter(Boolean) as string[]
        );
        setRegisteredConferenceIds(ids);
      })
      .catch(() => setRegisteredConferenceIds(new Set()))
      .finally(() => setIsLoadingRegistered(false));
  }, [token]);

  const loadRegisteredSessions = useCallback(async () => {
    if (!token || !isParticipant) {
      setRegisteredSessions({});
      setIsLoadingRegisteredSessions(false);
      return;
    }

    setIsLoadingRegisteredSessions(true);
    try {
      const items = await fetchRegisteredSessions();
      const next: Record<string, string> = {};
      for (const item of items) {
        if (item.sessionId && item.sessionRegistrationId) {
          next[item.sessionId] = item.sessionRegistrationId;
        }
      }
      setRegisteredSessions(next);
    } catch {
      setRegisteredSessions({});
    } finally {
      setIsLoadingRegisteredSessions(false);
    }
  }, [token, isParticipant]);

  useEffect(() => {
    void loadRegisteredSessions();
  }, [loadRegisteredSessions]);

  const isRegisteredForConference = registeredConferenceIds.has(conferenceId);
  const isRegistrationBlocked = !isRegisteredForConference || isLoadingRegistered;
  const registrationTooltip = isRegisteredForConference
    ? 'Prijavi se na sesiju'
    : 'Morate se prvo prijaviti na konferenciju.'

  const isRegisteredForSession = (sessionId: string) => Boolean(registeredSessions[sessionId])
  const [activeUploadSessionId, setActiveUploadSessionId] = useState<string | null>(null)
  const [materialsRefreshKey, setMaterialsRefreshKey] = useState(0)

  const handleDelete = async (id: string) => {
    try {
      await deleteSession(id)
      onDeleteSuccess()
    } catch (error) {
      console.error('Delete failed:', error)
      alert('Greška prilikom brisanja. Provjerite konzolu.')
    }
  }

  const handleRegister = async (id: string) => {
    try {
      const message = await registerForSession(id)
      await loadRegisteredSessions()
      alert(message)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Greška prilikom prijave na sesiju.'
      alert(message)
    }
  }

  const handleCancel = async (id: string) => {
    if (cancellingSessionId) return

    const registrationId = registeredSessions[id]
    if (!registrationId) {
      alert('Nije pronađena prijava za ovu sesiju.')
      return
    }

    setCancellingSessionId(id)
    try {
      const message = await cancelSessionRegistration(registrationId)
      await loadRegisteredSessions()
      alert(message)
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Greška prilikom odjave sa sesije.'
      alert(message)
    } finally {
      setCancellingSessionId(null)
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

          {/* Description */}
          {session.description && (
            <p style={{ color: '#94a3b8', fontSize: '14px', margin: '8px 0 12px 0' }}>
              {session.description}
            </p>
          )}

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

            {session.roomName && (
              <div
                className="session-info-row"
                style={{
                  display: 'flex',
                  alignItems: 'flex-start',
                  gap: '0.75rem',
                  flexWrap: 'wrap',
                }}
              >
                <span className="session-info-icon">🏢</span>

                <div
                  className="session-info-content"
                  style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '0.15rem',
                  }}
                >
                  <span className="session-info-label">Sala</span>
                  <p className="session-info-value">{session.roomName}</p>
                </div>
              </div>
            )}

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

            {isAdminOrOrganizer && (
              <SessionMaterialsSection sessionId={session.sessionId} refreshKey={materialsRefreshKey} />
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
            {/* Participant */}
            {isParticipant && (
              <button
                className="btn-primary-sm"
                style={{
                  backgroundColor: '#10B981',
                  color: 'white',
                }}
                onClick={() => handleRegister(session.sessionId)}
                disabled={
                  isRegistrationBlocked ||
                  isLoadingRegisteredSessions ||
                  isRegisteredForSession(session.sessionId)
                }
                title={
                  isRegisteredForSession(session.sessionId)
                    ? 'Već ste prijavljeni na sesiju.'
                    : registrationTooltip
                }
              >
                Prijavi se
              </button>
            )}
            {isParticipant && !isLoadingRegisteredSessions && isRegisteredForSession(session.sessionId) && (
              <button
                className="btn-secondary"
                onClick={() => handleCancel(session.sessionId)}
                disabled={cancellingSessionId === session.sessionId}
                title="Odjavi se sa sesije"
              >
                Odjavi
              </button>
            )}
            {/* Admin / Organizer */}
            {isAdminOrOrganizer && (
              <>
                <button
                  onClick={() => setActiveUploadSessionId(session.sessionId)}
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
                  Upload Materijala
                </button>

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

    {activeUploadSessionId && (
      <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
        <UploadMaterialModal
          sessionId={activeUploadSessionId}
          onCancel={() => setActiveUploadSessionId(null)}
          onSuccess={() => {
            setActiveUploadSessionId(null);
            setMaterialsRefreshKey((prev) => prev + 1);
          }}
        />
      </div>
    )}
    </>
  )
}