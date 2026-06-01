import type { Session } from '../types'
import {
    cancelSessionRegistration,
    deleteSession,
    fetchRegisteredSessions,
    registerForSession
} from '../api/sessionApi'
import { useAuth } from '../../../auth/AuthProvider'
import { useCallback, useEffect, useState } from 'react'
import QAPanel from './QAPanel'
import { useMaterials } from '../hooks/useMaterials'
import { UploadMaterialModal } from './UploadMaterialModal'
import { AssignEquipmentModal } from '../../equipment/components/AssignEquipmentModal'
import { useSessionEquipment } from '../../equipment/hooks/useEquipment'
import { unassignEquipmentFromSession } from '../../equipment/api/equipmentApi'

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

                            {index < items.length - 1 && (
                                <hr style={{ borderColor: 'rgba(148,163,184,0.2)', margin: '8px 0' }} />
                            )}
                        </div>
                    ))
                )}
            </div>
        </div>
    )
}

interface SessionEquipmentSectionProps {
    sessionId: string
    refreshKey: number
    canManageEquipment: boolean
}

function SessionEquipmentSection({ sessionId, refreshKey, canManageEquipment }: SessionEquipmentSectionProps) {
    const { items, isLoading, error, refresh } = useSessionEquipment(sessionId)
    const [removingEquipmentId, setRemovingEquipmentId] = useState<string | null>(null)
    const [toastMessage, setToastMessage] = useState<string | null>(null)

    useEffect(() => {
        if (refreshKey > 0) {
            void refresh()
        }
    }, [refreshKey, refresh])

    const handleUnassign = async (equipmentId: string, equipmentName: string) => {
        if (!sessionId || removingEquipmentId) return

        const shouldRemove = window.confirm(`Ukloniti opremu "${equipmentName}" sa sesije?`)
        if (!shouldRemove) return

        setRemovingEquipmentId(equipmentId)
        try {
            await unassignEquipmentFromSession(sessionId, equipmentId)
            await refresh()
        } catch (unassignError) {
            console.error('Greška pri uklanjanju opreme sa sesije:', unassignError)
            setToastMessage('Uklanjanje opreme nije uspjelo. Pokušajte ponovo.')
            window.setTimeout(() => setToastMessage(null), 3000)
        } finally {
            setRemovingEquipmentId(null)
        }
    }

    return (
        <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
            <span className="session-info-icon">🧰</span>
            <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <span className="session-info-label">OPREMA</span>

                {isLoading ? (
                    <p className="session-info-value">Učitavanje opreme...</p>
                ) : error ? (
                    <p className="session-info-value">{error}</p>
                ) : items.length === 0 ? (
                    <p className="session-info-value">Nema dodijeljene opreme</p>
                ) : (
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '6px' }}>
                        {items.map((item) => (
                            <div key={item.equipmentId} style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
                                <span style={{ fontWeight: 600 }}>{item.name}</span>
                                <span className="session-info-value" style={{ margin: 0 }}>
                                    × {item.quantity}
                                </span>
                                {canManageEquipment && (
                                    <button
                                        type="button"
                                        onClick={() => handleUnassign(item.equipmentId, item.name)}
                                        className="btn-secondary"
                                        style={{
                                            padding: '4px 8px',
                                            minWidth: '26px',
                                            lineHeight: 1,
                                        }}
                                        disabled={removingEquipmentId === item.equipmentId}
                                        aria-label={`Ukloni ${item.name} sa sesije`}
                                        title="Ukloni opremu"
                                    >
                                        −
                                    </button>
                                )}
                            </div>
                        ))}
                        {toastMessage && (
                            <div
                                role="status"
                                style={{
                                    marginTop: '6px',
                                    padding: '8px 12px',
                                    borderRadius: '10px',
                                    border: '1px solid rgba(248, 113, 113, 0.35)',
                                    background: 'rgba(248, 113, 113, 0.12)',
                                    color: '#fca5a5',
                                    fontSize: '0.85rem',
                                    fontWeight: 600,
                                }}
                            >
                                {toastMessage}
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    )
}

interface SessionListProps {
    sessions: Session[]
    conferenceId: string
    isAdminOrOrganizer: boolean
    canManageEquipment?: boolean
    onDeleteSuccess: () => void
    onEditClick: (session: Session) => void
}

export function SessionList({
    sessions = [],
    conferenceId,
    isAdminOrOrganizer,
    canManageEquipment,
    onDeleteSuccess,
    onEditClick,
}: SessionListProps) {
    const { user, token } = useAuth()

    const role = user?.role?.toLowerCase() ?? ''

    const isParticipant = role === 'ucesnik'
    const isSpeaker = role === 'predavac'

    const [showDeleteModal, setShowDeleteModal] = useState(false)
    const [sessionToDelete, setSessionToDelete] = useState<string | null>(null)

    const [registeredConferenceIds, setRegisteredConferenceIds] = useState<Set<string>>(new Set())
    const [isLoadingRegistered, setIsLoadingRegistered] = useState(true)

    const [registeredSessions, setRegisteredSessions] = useState<Record<string, string>>({})

    const [cancellingSessionId, setCancellingSessionId] = useState<string | null>(null)
    const [openQASessionId, setOpenQASessionId] = useState<string | null>(null)

    const [activeUploadSessionId, setActiveUploadSessionId] = useState<string | null>(null)
    const [activeAssignEquipmentSessionId, setActiveAssignEquipmentSessionId] = useState<string | null>(null)
    const [materialsRefreshKey, setMaterialsRefreshKey] = useState(0)
    const [equipmentRefreshKey, setEquipmentRefreshKey] = useState(0)

    const canManageEquipmentResolved = canManageEquipment ?? isAdminOrOrganizer

    const filteredSessions = sessions.filter(() => {
        if (isAdminOrOrganizer) return true
        if (isParticipant) return true
        if (isSpeaker) return true
        return true
    })

    useEffect(() => {
        if (!token) return

        const loadConferenceRegistrations = async () => {
            setIsLoadingRegistered(true)
            try {
                const r = await fetch('/api/Conference/registered', {
                    headers: { Authorization: `Bearer ${token}` },
                })
                const data = r.ok ? await r.json() : []
                const items = Array.isArray(data) ? data : []
                const ids = new Set<string>(
                    items.map((item: { conferenceId?: string }) => item.conferenceId).filter(Boolean) as string[]
                )
                setRegisteredConferenceIds(ids)
            } catch {
                setRegisteredConferenceIds(new Set())
            } finally {
                setIsLoadingRegistered(false)
            }
        }

        void loadConferenceRegistrations()
    }, [token])

    const loadRegisteredSessions = useCallback(async () => {
        if (!token || !isParticipant) {
            setRegisteredSessions({})
            return
        }

        try {
            const items = await fetchRegisteredSessions()

            const next: Record<string, string> = {}

            for (const item of items) {
                if (item.sessionId && item.sessionRegistrationId) {
                    next[item.sessionId] = item.sessionRegistrationId
                }
            }

            setRegisteredSessions(next)
        } catch {
            setRegisteredSessions({})
        }
    }, [token, isParticipant])

    useEffect(() => {
        const run = async () => {
            await loadRegisteredSessions()
        }
        void run()
    }, [loadRegisteredSessions])

    const isRegisteredForConference = registeredConferenceIds.has(conferenceId)

    const isRegistrationBlocked = !isRegisteredForConference || isLoadingRegistered

    const isRegisteredForSession = (sessionId: string) =>
        Boolean(registeredSessions[sessionId])

    const handleDelete = async (id: string) => {
        try {
            await deleteSession(id)
            onDeleteSuccess()
        } catch (error) {
            console.error('Delete failed:', error)
            alert('Greška prilikom brisanja.')
        }
    }

    const handleRegister = async (id: string) => {
        try {
            const message = await registerForSession(id)
            await loadRegisteredSessions()
            alert(message)
        } catch (error) {
            alert(error instanceof Error ? error.message : 'Greška')
        }
    }

    const handleCancel = async (id: string) => {
        if (cancellingSessionId) return

        const registrationId = registeredSessions[id]

        if (!registrationId) {
            alert('Nije pronađena prijava.')
            return
        }

        setCancellingSessionId(id)

        try {
            const message = await cancelSessionRegistration(registrationId)
            await loadRegisteredSessions()
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
            </div>
        )
    }

    const activeQASession = openQASessionId
        ? sessions.find(session => session.sessionId === openQASessionId)
        : null

    return (
        <>
            <div className="session-grid">
                {filteredSessions.map(session => (
                    <div key={session.sessionId} className="session-card">

                        <div className="session-card-header">
                            <h3 className="session-card-title">{session.title}</h3>
                            <span className={`session-status session-status-${session.status.toLowerCase()}`}>
                                {session.status}
                            </span>
                        </div>

                        {session.description && (
                            <p>{session.description}</p>
                        )}

                        {isSpeaker && (
                            <span>Predavač</span>
                        )}

                        <div className="session-card-info">

                            <p>⏰ {formatDateTime(session.startTime)}</p>
                            <p>🏁 {formatDateTime(session.endTime)}</p>
                            <p>🏷️ {session.sessionType}</p>
                            {session.roomName && <p>🏢 {session.roomName}</p>}
                            {session.speakerName && <p>🎤 {session.speakerName}</p>}

                            {(isAdminOrOrganizer || (isParticipant && isRegisteredForSession(session.sessionId))) && (
                                <SessionMaterialsSection
                                    sessionId={session.sessionId}
                                    refreshKey={materialsRefreshKey}
                                />
                            )}

                            {canManageEquipmentResolved && (
                                <SessionEquipmentSection
                                    sessionId={session.sessionId}
                                    refreshKey={equipmentRefreshKey}
                                    canManageEquipment={canManageEquipmentResolved}
                                />
                            )}
                        </div>

                        <div className="session-card-actions">

                            {isParticipant && (
                                <button
                                    onClick={() => handleRegister(session.sessionId)}
                                    disabled={isRegistrationBlocked || isRegisteredForSession(session.sessionId)}
                                    className="btn-primary-sm"
                                    style={{
                                        backgroundColor: '#10B981',
                                        color: 'white',
                                        borderRadius: 'var(--radius-md)',
                                    }}
                                >
                                    Prijavi se
                                </button>
                            )}

                            {(isParticipant && isRegisteredForSession(session.sessionId)) && (
                                <button
                                    onClick={() => handleCancel(session.sessionId)}
                                    className="btn-secondary"
                                >
                                    Odjavi
                                </button>
                            )}

                            {(isParticipant || isSpeaker || isAdminOrOrganizer) && (
                                <button
                                    onClick={() =>
                                        setOpenQASessionId(
                                            openQASessionId === session.sessionId
                                                ? null
                                                : session.sessionId
                                        )
                                    }
                                    className="btn-qa"
                                >
                                    Q&A
                                </button>
                            )}

                            {isAdminOrOrganizer && (
                                <>
                                    <div className="session-admin-actions">
                                        <button
                                            onClick={() => setActiveUploadSessionId(session.sessionId)}
                                            className="btn-secondary"
                                        >
                                            Upload Materijala
                                        </button>

                                        {canManageEquipmentResolved && (
                                            <button
                                                onClick={() => setActiveAssignEquipmentSessionId(session.sessionId)}
                                                className="btn-secondary"
                                                style={{
                                                    backgroundColor: '#38bdf8',
                                                    color: '#000',
                                                    borderRadius: 'var(--radius-md)',
                                                    padding: '8px 20px',
                                                    border: 'none',
                                                    cursor: 'pointer',
                                                    fontWeight: 'bold',
                                                }}
                                            >
                                                Dodijeli Opremu
                                            </button>
                                        )}

                                        <button
                                            onClick={() => onEditClick(session)}
                                            className="btn-edit"
                                            style={{
                                                backgroundColor: '#EAB308',
                                                color: '#000',
                                                borderRadius: 'var(--radius-md)',
                                                padding: '8px 20px',
                                                border: 'none',
                                                cursor: 'pointer',
                                            }}
                                        >
                                            Uredi
                                        </button>

                                        <button
                                            onClick={() => {
                                                setSessionToDelete(session.sessionId)
                                                setShowDeleteModal(true)
                                            }}
                                            className="btn-delete"
                                            style={{
                                                backgroundColor: '#EF4444',
                                                color: '#fff',
                                                borderRadius: 'var(--radius-md)',
                                                padding: '8px 20px',
                                                border: 'none',
                                                cursor: 'pointer',
                                            }}
                                        >
                                            Obriši
                                        </button>
                                    </div>

                                    <details className="session-admin-menu">
                                        <summary className="btn-secondary session-admin-menu-trigger" aria-label="Akcije">
                                            ...
                                        </summary>
                                        <div className="session-admin-menu-list">
                                            <button
                                                onClick={() => setActiveUploadSessionId(session.sessionId)}
                                                className="session-admin-menu-item"
                                            >
                                                Upload Materijala
                                            </button>
                                            {canManageEquipmentResolved && (
                                                <button
                                                    onClick={() => setActiveAssignEquipmentSessionId(session.sessionId)}
                                                    className="session-admin-menu-item"
                                                >
                                                    Dodijeli Opremu
                                                </button>
                                            )}
                                            <button
                                                onClick={() => onEditClick(session)}
                                                className="session-admin-menu-item"
                                            >
                                                Uredi
                                            </button>
                                            <button
                                                onClick={() => {
                                                    setSessionToDelete(session.sessionId)
                                                    setShowDeleteModal(true)
                                                }}
                                                className="session-admin-menu-item session-admin-menu-item-danger"
                                            >
                                                Obriši
                                            </button>
                                        </div>
                                    </details>
                                </>
                            )}
                        </div>

                    </div>
                ))}
            </div>

            {activeQASession && (
                <div
                    className="modal-overlay qa-modal-overlay"
                    onClick={() => setOpenQASessionId(null)}
                >
                    <div
                        className="qa-modal-content"
                        onClick={(event) => event.stopPropagation()}
                    >
                        <div className="qa-modal-header">
                            <div>
                                <h2 className="qa-modal-title">Q&A</h2>
                                <p className="qa-modal-subtitle">{activeQASession.title}</p>
                            </div>
                            <button
                                className="btn-secondary"
                                onClick={() => setOpenQASessionId(null)}
                            >
                                Zatvori
                            </button>
                        </div>

                        <QAPanel
                            sessionId={activeQASession.sessionId}
                            sessionStartTime={activeQASession.startTime}
                            sessionEndTime={activeQASession.endTime}
                            role={role}
                            canAnswer={
                                isSpeaker &&
                                activeQASession.speakerName === `${user?.firstName ?? ''} ${user?.lastName ?? ''}`.trim()
                            }
                            canAsk={isParticipant && isRegisteredForSession(activeQASession.sessionId)}
                        />
                    </div>
                </div>
            )}

            {showDeleteModal && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h2>Potvrda</h2>
                        <button onClick={() => setShowDeleteModal(false)}>Ne</button>
                        <button
                            onClick={() => {
                                if (sessionToDelete) handleDelete(sessionToDelete)
                                setShowDeleteModal(false)
                            }}
                        >
                            Da
                        </button>
                    </div>
                </div>
            )}

            {activeUploadSessionId && (
                <UploadMaterialModal
                    sessionId={activeUploadSessionId}
                    onCancel={() => setActiveUploadSessionId(null)}
                    onSuccess={() => {
                        setActiveUploadSessionId(null)
                        setMaterialsRefreshKey(prev => prev + 1)
                    }}
                />
            )}

            {/* NOVO: Modal za dodjelu opreme */}
            {activeAssignEquipmentSessionId && (
                <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
                    <AssignEquipmentModal
                        sessionId={activeAssignEquipmentSessionId}
                        onCancel={() => setActiveAssignEquipmentSessionId(null)}
                        onSuccess={() => {
                            setActiveAssignEquipmentSessionId(null)
                            setEquipmentRefreshKey(prev => prev + 1)
                        }}
                    />
                </div>
            )}
        </>
    )
}