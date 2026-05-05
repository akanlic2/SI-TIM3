import type { Conference } from '../types'
import { deleteConference } from '../api/conferenceApi'

interface ConferenceListProps {
  conferences: Conference[]
  isAdminOrOrganizer: boolean
  onDeleteSuccess: () => void
  onEditClick: (conference: Conference) => void
}

export function ConferenceList({ conferences, isAdminOrOrganizer, onDeleteSuccess, onEditClick }: ConferenceListProps) {

  const handleDelete = async (id: string) => {
    if (window.confirm("Da li ste sigurni da želite obrisati ovu konferenciju?")) {
      try {
        await deleteConference(id);
        onDeleteSuccess();
      } catch (error) {
        console.error("Delete failed:", error)
        alert("Greška prilikom brisanja. Provjerite konzolu.");
      }
    }
  }

  const formatDate = (dateString: string) => {
    try {
      return new Intl.DateTimeFormat('bs-BA', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      }).format(new Date(dateString));
    } catch {
      return dateString;
    }
  }

  if (conferences.length === 0) {
    return (
      <div className="conference-empty-state">
        <div className="conference-empty-icon">🗓</div>
        <h3>Nema pronađenih konferencija</h3>
        <p>Konferencije će se pojaviti ovdje čim budu dodane</p>
      </div>
    )
  }

  return (
    <div className="conference-grid">
      {conferences.map((conference) => (
        <div
          key={conference.conferenceId}
          className="conference-card"
        >
          {/* Header with Title and Status */}
          <div className="conference-card-header">
            <h3 className="conference-card-title">{conference.title}</h3>
            <span className={`conference-status conference-status-${conference.status.toLowerCase()}`}>
              {conference.status}
            </span>
          </div>

          {/* Description */}
          {conference.description && (
            <p className="conference-card-description">{conference.description}</p>
          )}

          {/* Info Grid */}
          <div className="conference-card-info">
            <div className="conference-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="conference-info-icon">📍</span>
              <div className="conference-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="conference-info-label">Lokacija</span>
                <p className="conference-info-value">{conference.location}</p>
              </div>
            </div>

            <div className="conference-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="conference-info-icon">📅</span>
              <div className="conference-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="conference-info-label">Početak</span>
                <p className="conference-info-value">{formatDate(conference.startDate)}</p>
              </div>
            </div>

            <div className="conference-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="conference-info-icon">🏁</span>
              <div className="conference-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="conference-info-label">Završetak</span>
                <p className="conference-info-value">{formatDate(conference.endDate)}</p>
              </div>
            </div>

            <div className="conference-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="conference-info-icon">🏷️</span>
              <div className="conference-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="conference-info-label">Kategorija</span>
                <p className="conference-info-value">{conference.category}</p>
              </div>
            </div>

            <div className="conference-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="conference-info-icon">👥</span>
              <div className="conference-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="conference-info-label">Maksimalno učesnika</span>
                <p className="conference-info-value">{conference.maxParticipants}</p>
              </div>
            </div>
          </div>

          {/* Buttons */}
          {isAdminOrOrganizer && (
            <div className="conference-card-actions" style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', paddingTop: '16px', borderTop: '1px solid rgba(148,163,184,0.2)' }}>
              <button
                onClick={() => onEditClick(conference)}
                className="btn-edit"
                style={{ backgroundColor: '#EAB308', color: '#000', borderRadius: '9999px', padding: '8px 20px', border: 'none' }}
              >
                Uredi
              </button>
              <button
                onClick={() => handleDelete(conference.conferenceId)}
                className="btn-delete"
                style={{ backgroundColor: '#EF4444', color: '#fff', borderRadius: '9999px', padding: '8px 20px', border: 'none' }}
              >
                Obriši
              </button>
            </div>
          )}
        </div>
      ))}
    </div>
  )
}