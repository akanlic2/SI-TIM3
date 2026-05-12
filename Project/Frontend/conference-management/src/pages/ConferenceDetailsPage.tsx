import { useEffect, useState } from 'react'
import { fetchConferenceById } from '../features/conference/api/conferenceApi'
import type { Conference } from '../features/conference/types'
import { useAuth } from '../auth/AuthProvider'
import '../features/conference/ConferencesPage.css'

interface RegistrationUser {
  firstName?: string
  lastName?: string
  email?: string
}

interface ConferenceRegistrationUser {
  conferenceRegistrationId: string
  userId: string
  registrationDate: string
  registrationStatus: string
  user: RegistrationUser
}

export default function ConferenceDetailsPage() {
  const { user, token } = useAuth()
  const [conference, setConference] = useState<Conference | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [registrations, setRegistrations] = useState<ConferenceRegistrationUser[]>([])
  const [isLoadingRegistrations, setIsLoadingRegistrations] = useState(false)
  const [registrationsError, setRegistrationsError] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [sortKey, setSortKey] = useState<'firstName' | 'lastName' | 'email' | 'registrationDate' | 'registrationStatus'>(
    'registrationDate'
  )
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc')

  const id = window.location.pathname.split('/').pop() ?? ''
  const isAdmin = user?.role?.toLowerCase().includes('admin') ?? false

  useEffect(() => {
    async function loadConference() {
      setIsLoading(true)
      const data = await fetchConferenceById(id)
      setConference(data)
      setIsLoading(false)
    }

    loadConference()
  }, [id])

  useEffect(() => {
    if (!token || !isAdmin || !id) return

    setIsLoadingRegistrations(true)
    setRegistrationsError(null)

    fetch(`/api/conferences/${id}/registrations`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error('Greška pri dohvatanju prijava.')
        }
        return response.json()
      })
      .then((data) => {
        setRegistrations(Array.isArray(data) ? data : [])
      })
      .catch((error) => {
        const message = error instanceof Error ? error.message : 'Greška pri dohvatanju prijava.'
        setRegistrationsError(message)
      })
      .finally(() => setIsLoadingRegistrations(false))
  }, [id, isAdmin, token])

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

  const normalizedSearch = searchTerm.trim().toLowerCase()
  const statusOptions = Array.from(
    new Set(registrations.map((item) => item.registrationStatus).filter(Boolean))
  ).sort((a, b) => a.localeCompare(b))

  const filteredRegistrations = registrations.filter((registration) => {
    if (statusFilter && registration.registrationStatus !== statusFilter) return false
    if (!normalizedSearch) return true

    const firstName = registration.user?.firstName?.toLowerCase() ?? ''
    const lastName = registration.user?.lastName?.toLowerCase() ?? ''
    const email = registration.user?.email?.toLowerCase() ?? ''
    const fullName = `${firstName} ${lastName}`.trim()

    return (
      firstName.includes(normalizedSearch) ||
      lastName.includes(normalizedSearch) ||
      fullName.includes(normalizedSearch) ||
      email.includes(normalizedSearch)
    )
  })

  const sortedRegistrations = [...filteredRegistrations].sort((a, b) => {
    const getValue = (item: ConferenceRegistrationUser) => {
      switch (sortKey) {
        case 'firstName':
          return item.user?.firstName ?? ''
        case 'lastName':
          return item.user?.lastName ?? ''
        case 'email':
          return item.user?.email ?? ''
        case 'registrationStatus':
          return item.registrationStatus ?? ''
        case 'registrationDate':
        default:
          return item.registrationDate ?? ''
      }
    }

    const left = getValue(a)
    const right = getValue(b)

    if (sortKey === 'registrationDate') {
      const leftTime = new Date(left).getTime()
      const rightTime = new Date(right).getTime()
      return sortDirection === 'asc' ? leftTime - rightTime : rightTime - leftTime
    }

    const comparison = String(left).localeCompare(String(right))
    return sortDirection === 'asc' ? comparison : -comparison
  })

  const toggleSort = (
    key: 'firstName' | 'lastName' | 'email' | 'registrationDate' | 'registrationStatus'
  ) => {
    if (sortKey === key) {
      setSortDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'))
      return
    }

    setSortKey(key)
    setSortDirection('asc')
  }

  return (
    <main className="conferences-page">
      <div className="conferences-header">
        <div className="conferences-header-content">
          <div className="conferences-title-section">
            <h1>{conference.title}</h1>
            <p>Detalji konferencije</p>
          </div>

          <div style={{ display: 'flex', gap: '12px' }}>
            <button onClick={() => {
              window.history.pushState({}, '', `/conferences/${id}/sessions`);
              window.dispatchEvent(new PopStateEvent('popstate'));
            }} className="btn-primary">
              📅 Sesije
            </button>

            <button onClick={goBack} className="btn-secondary">
              ← Nazad
            </button>
          </div>
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

        {isAdmin && (
          <div className="section-block" style={{ marginTop: '24px' }}>
            <div className="section-header">
              <h2 className="section-title">Prijavljeni učesnici</h2>
            </div>

            {isLoadingRegistrations ? (
              <div className="loading-row">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="skeleton-card" />
                ))}
              </div>
            ) : registrationsError ? (
              <div className="error-message">{registrationsError}</div>
            ) : registrations.length === 0 ? (
              <div className="empty-state">
                <div className="empty-icon">👥</div>
                <p>Nema prijavljenih korisnika</p>
              </div>
            ) : (
              <>
                <div
                  style={{
                    display: 'flex',
                    gap: '12px',
                    padding: '16px 24px 0',
                    flexWrap: 'wrap',
                    alignItems: 'center',
                  }}
                >
                  <input
                    type="text"
                    placeholder="Pretraži po imenu ili emailu..."
                    value={searchTerm}
                    onChange={(event) => setSearchTerm(event.target.value)}
                    className="form-input"
                    style={{ maxWidth: '280px' }}
                  />
                  <select
                    value={statusFilter}
                    onChange={(event) => setStatusFilter(event.target.value)}
                    className="form-select"
                    style={{ maxWidth: '220px' }}
                  >
                    <option value="">Svi statusi</option>
                    {statusOptions.map((status) => (
                      <option key={status} value={status}>{status}</option>
                    ))}
                  </select>
                </div>
                <div className="conference-table">
                  <div className="table-header">
                    <button type="button" className="btn-secondary" onClick={() => toggleSort('firstName')}>
                      Ime {sortKey === 'firstName' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                    </button>
                    <button type="button" className="btn-secondary" onClick={() => toggleSort('lastName')}>
                      Prezime {sortKey === 'lastName' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                    </button>
                    <button type="button" className="btn-secondary" onClick={() => toggleSort('email')}>
                      Email {sortKey === 'email' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                    </button>
                    <button type="button" className="btn-secondary" onClick={() => toggleSort('registrationDate')}>
                      Datum prijave {sortKey === 'registrationDate' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                    </button>
                    <button type="button" className="btn-secondary" onClick={() => toggleSort('registrationStatus')}>
                      Status {sortKey === 'registrationStatus' ? (sortDirection === 'asc' ? '↑' : '↓') : ''}
                    </button>
                  </div>
                  {sortedRegistrations.map((registration) => (
                    <div
                      key={registration.conferenceRegistrationId}
                      className="table-row"
                    >
                      <span className="table-title">{registration.user?.firstName ?? '—'}</span>
                      <span className="table-location">{registration.user?.lastName ?? '—'}</span>
                      <span className="table-date">{registration.user?.email ?? '—'}</span>
                      <span className="table-date">
                        {new Date(registration.registrationDate).toLocaleString('bs-BA')}
                      </span>
                      <span className="conf-badge">{registration.registrationStatus}</span>
                    </div>
                  ))}
                </div>
              </>
            )}
          </div>
        )}
      </div>
    </main>
  )
}