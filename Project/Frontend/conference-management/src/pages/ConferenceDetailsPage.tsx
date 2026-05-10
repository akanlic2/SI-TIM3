import { useEffect, useState } from 'react'
import { fetchConferenceById } from '../features/conference/api/conferenceApi'
import type { Conference } from '../features/conference/types'
import '../features/conference/ConferencesPage.css'

export default function ConferenceDetailsPage() {
  const [conference, setConference] = useState<Conference | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const id = window.location.pathname.split('/').pop() ?? ''

  useEffect(() => {
    async function loadConference() {
      setIsLoading(true)
      const data = await fetchConferenceById(id)
      setConference(data)
      setIsLoading(false)
    }

    loadConference()
  }, [id])

  const goBack = () => {
    window.history.pushState({}, '', '/conferences')
    window.dispatchEvent(new PopStateEvent('popstate'))
  }

  if (isLoading) {
    return (
      <main className="conferences-page">
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p className="loading-text">Učitavanje detalja konferencije...</p>
        </div>
      </main>
    )
  }

  if (!conference) {
    return (
      <main className="conferences-page">
        <div className="conferences-content">
          <div className="error-message">
            Konferencija nije pronađena ili nemate pristup ovoj konferenciji.
          </div>

          <button onClick={goBack} className="btn-secondary">
            Nazad na konferencije
          </button>
        </div>
      </main>
    )
  }

  return (
    <main className="conferences-page">
      <div className="conferences-header">
        <div className="conferences-header-content">
          <div className="conferences-title-section">
            <h1>{conference.title}</h1>
            <p>Detalji konferencije</p>
          </div>

          <button onClick={goBack} className="btn-secondary">
            ← Nazad
          </button>
        </div>
      </div>

      <div className="conferences-content">
        <div className="conference-card">
          <div className="conference-card-header">
            <h2 className="conference-card-title">{conference.title}</h2>

            <span className={`conference-status conference-status-${conference.status.toLowerCase()}`}>
              {conference.status}
            </span>
          </div>

          <p className="conference-card-description">
            {conference.description}
          </p>

          <div className="conference-card-info">
            <p><strong>📍 Lokacija:</strong> {conference.location}</p>
            <p><strong>🏷️ Kategorija:</strong> {conference.category}</p>
            <p><strong>📅 Početak:</strong> {new Date(conference.startDate).toLocaleString('bs-BA')}</p>
            <p><strong>🏁 Završetak:</strong> {new Date(conference.endDate).toLocaleString('bs-BA')}</p>
            <p><strong>👥 Maksimalno učesnika:</strong> {conference.maxParticipants}</p>
            <p><strong>Status:</strong> {conference.status}</p>
          </div>
        </div>
      </div>
    </main>
  )
}