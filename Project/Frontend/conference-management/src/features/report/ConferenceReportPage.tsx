import { useEffect, useState } from 'react'
import { useAuth } from '../../auth/AuthProvider'
import {
  fetchConferenceReport,
  downloadConferenceReport,
  type ConferenceReportDto,
} from './api/reportApi'
import '../../features/conference/ConferencesPage.css'

export default function ConferenceReportPage() {
  const { user, token } = useAuth()
  const [report, setReport] = useState<ConferenceReportDto | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [isDownloading, setIsDownloading] = useState(false)

  const id = window.location.pathname.split('/')[2] ?? ''
  const isAdmin = user?.role?.toLowerCase().includes('admin') ?? false
  const isOrganizer = user?.role?.toLowerCase().includes('organizator') ?? false
  const canSeeReport = isAdmin || isOrganizer

  useEffect(() => {
    if (!token || !canSeeReport || !id) return

    setIsLoading(true)
    setError(null)

    fetchConferenceReport(id, token)
      .then(setReport)
      .catch((err) => setError(err instanceof Error ? err.message : 'Greška.'))
      .finally(() => setIsLoading(false))
  }, [id, token, canSeeReport])

  const goBack = () => {
    window.history.pushState({}, '', `/conferences/${id}`)
    window.dispatchEvent(new PopStateEvent('popstate'))
  }

  const handleDownload = async () => {
    if (!token) return
    setIsDownloading(true)
    try {
      await downloadConferenceReport(id, token)
    } catch {
      setError('Greška pri preuzimanju PDF-a.')
    } finally {
      setIsDownloading(false)
    }
  }

  if (!canSeeReport) {
    return (
      <main className="conferences-page">
        <div className="conferences-content">
          <div className="error-message">Nemate pristup ovoj stranici.</div>
        </div>
      </main>
    )
  }

  if (isLoading) {
    return (
      <main className="conferences-page">
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p className="loading-text">Učitavanje izvještaja...</p>
        </div>
      </main>
    )
  }

  if (error || !report) {
    return (
      <main className="conferences-page">
        <div className="conferences-content">
          <div className="error-message">{error ?? 'Izvještaj nije pronađen.'}</div>
          <button onClick={goBack} className="btn-secondary">← Nazad</button>
        </div>
      </main>
    )
  }

  return (
    <main className="conferences-page">
      <div className="conferences-header">
        <div className="conferences-header-content">
          <div className="conferences-title-section">
            <h1>Izvještaj</h1>
            <p>{report.title}</p>
          </div>
          <div style={{ display: 'flex', gap: '12px' }}>
            <button
              onClick={handleDownload}
              className="btn-primary"
              disabled={isDownloading}
            >
              {isDownloading ? 'Preuzimanje...' : '⬇️ Preuzmi PDF'}
            </button>
            <button onClick={goBack} className="btn-secondary">← Nazad</button>
          </div>
        </div>
      </div>

      <div className="conferences-content">

        {/* Info o konferenciji */}
        <div className="conference-card">
          <div className="conference-card-header">
            <h2 className="conference-card-title">{report.title}</h2>
          </div>
          <div className="conference-card-info">
            <p><strong>📍 Lokacija:</strong> {report.location}</p>
            <p><strong>📅 Početak:</strong> {new Date(report.startDate).toLocaleString('bs-BA')}</p>
            <p><strong>🏁 Završetak:</strong> {new Date(report.endDate).toLocaleString('bs-BA')}</p>
          </div>
        </div>

        {/* Statistike prijava */}
        <div className="section-block" style={{ marginTop: '24px' }}>
          <div className="section-header">
            <h2 className="section-title">Statistike prijava</h2>
          </div>
          <div className="capacity-grid">
            <div className="capacity-card">
              <span className="capacity-label">Ukupno</span>
              <span className="capacity-value">{report.registrationStats.total}</span>
            </div>
            <div className="capacity-card">
              <span className="capacity-label">Potvrđeno</span>
              <span className="capacity-value" style={{ color: 'var(--success)' }}>
                {report.registrationStats.confirmed}
              </span>
            </div>
            <div className="capacity-card">
              <span className="capacity-label">Na čekanju</span>
              <span className="capacity-value" style={{ color: 'var(--warning, #f59e0b)' }}>
                {report.registrationStats.pending}
              </span>
            </div>
            <div className="capacity-card">
              <span className="capacity-label">Otkazano</span>
              <span className="capacity-value" style={{ color: 'var(--error)' }}>
                {report.registrationStats.cancelled}
              </span>
            </div>
          </div>
        </div>

        {/* Ukupno predavači i materijali */}
        <div className="section-block" style={{ marginTop: '24px' }}>
          <div className="section-header">
            <h2 className="section-title">Pregled</h2>
          </div>
          <div className="capacity-grid">
            <div className="capacity-card">
              <span className="capacity-label">Ukupno predavača</span>
              <span className="capacity-value">{report.totalSpeakers}</span>
            </div>
            <div className="capacity-card">
              <span className="capacity-label">Ukupno materijala</span>
              <span className="capacity-value">{report.totalMaterials}</span>
            </div>
          </div>
        </div>

        {/* Sesije */}
        <div className="section-block" style={{ marginTop: '24px' }}>
          <div className="section-header">
            <h2 className="section-title">Sesije</h2>
          </div>
          {report.sessions.length === 0 ? (
            <div className="empty-state">
              <div className="empty-icon">📅</div>
              <p>Nema sesija za ovu konferenciju.</p>
            </div>
          ) : (
            <div className="conference-table">
              <div className="table-header">
                <span>Sesija</span>
                <span>Prijavljeni</span>
                <span>Kapacitet</span>
                <span>Predavači</span>
                <span>Materijali</span>
              </div>
              {report.sessions.map((session) => (
                <div key={session.sessionId} className="table-row">
                  <span className="table-title">{session.title}</span>
                  <span className="table-date">{session.registeredCount}</span>
                  <span className="table-date">
                    {session.roomCapacity > 0 ? session.roomCapacity : '—'}
                  </span>
                  <span className="table-date">{session.speakerCount}</span>
                  <span className="table-date">{session.materialCount}</span>
                </div>
              ))}
            </div>
          )}
        </div>

      </div>
    </main>
  )
}