import { useEffect, useState, type CSSProperties } from 'react';
import { useAuth } from '../auth/AuthProvider';
import SettingsPage from './SettingsPage';
import { AdminUsersPanel } from '../features/user';
import { useSpeakerSessions } from '../features/session/hooks/useSpeakerSessions';
import { NotificationBell } from '../features/notification';
import './DashboardPage.css';

// ─── Tip za konferenciju ───────────────────────────────────────────────────────
interface Conference {
  conferenceId: string;
  title: string;
  location: string;
  startDate: string;
  status: string;
}

interface RegisteredConference extends Conference {
  conferenceRegistrationId?: string;
}

// ─── Stat kartica ─────────────────────────────────────────────────────────────
interface StatCardProps {
  icon: string;
  label: string;
  value: string | number;
  trend?: string;
  color: string;
}

function StatCard({ icon, label, value, trend, color }: StatCardProps) {
  return (
    <div className="stat-card" style={{ '--accent': color } as CSSProperties}>
      <div className="stat-icon">{icon}</div>
      <div className="stat-body">
        <span className="stat-label">{label}</span>
        <span className="stat-value">{value}</span>
        {trend && <span className="stat-trend">{trend}</span>}
      </div>
      <div className="stat-glow" />
    </div>
  );
}

// ─── Badge rola ───────────────────────────────────────────────────────────────
function RoleBadge({ role }: { role: string }) {
  const isAdmin = role.toLowerCase().includes('admin');
  return (
    <span className={`role-badge ${isAdmin ? 'admin' : 'user'}`}>
      {isAdmin ? '⚡ Admin' : '👤 ' + role}
    </span>
  );
}

// ─── Dashboard ────────────────────────────────────────────────────────────────
export default function DashboardPage() {
  const { user, logout, token } = useAuth();
  const [conferences, setConferences] = useState<Conference[]>([]);
  const [isLoadingConferences, setIsLoadingConferences] = useState(true);
  const [registeredConferences, setRegisteredConferences] = useState<RegisteredConference[]>([]);
  const [isLoadingRegistered, setIsLoadingRegistered] = useState(true);
  const [cancellingRegistrationId, setCancellingRegistrationId] = useState<string | null>(null);
  const [loggingOut, setLoggingOut] = useState(false);
  const [activeNav, setActiveNav] = useState('dashboard');
  const [sidebarOpen, setSidebarOpen] = useState(true);

  const displayName =
    [user?.firstName, user?.lastName].filter(Boolean).join(' ') || user?.username || user?.email || 'Korisnik';
  const initials = displayName
    .split(' ')
    .map((n) => n[0])
    .join('')
    .slice(0, 2)
    .toUpperCase();

  const roles = user?.role ? [user.role] : [];
  const isAdmin = roles.some((role) => role.toLowerCase().includes('admin'));
  const isAdminOrOrganizer = roles.some(
    (role) => role.toLowerCase() === 'organizator' || role.toLowerCase().includes('admin')
  );
  const isParticipant = roles.some((role) => role.toLowerCase().includes('ucesnik'));
  const isSpeaker = user?.role?.toLowerCase() === 'predavac';

  const {
    items: speakerSessions,
    isLoading: isLoadingSpeakerSessions,
    error: speakerSessionsError,
  } = useSpeakerSessions(token ?? undefined, isSpeaker);

  // ─── Dohvat konferencija ───────────────────────────────────────────────────
  useEffect(() => {
    if (!token) return;
    fetch('/api/Conference?pageSize=1000', {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((r) => (r.ok ? r.json() : []))
      .then((data) => {
        const confArray = Array.isArray(data) ? data : Array.isArray(data?.items) ? data.items : [];
        setConferences(confArray);
      })
      .catch(() => setConferences([]))
      .finally(() => setIsLoadingConferences(false));
  }, [token]);

  useEffect(() => {
    if (!token) return;
    setIsLoadingRegistered(true);
    fetch('/api/Conference/registered', {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((r) => (r.ok ? r.json() : []))
      .then((data) => {
        const confArray = Array.isArray(data) ? data : [];
        setRegisteredConferences(confArray as RegisteredConference[]);
      })
      .catch(() => setRegisteredConferences([]))
      .finally(() => setIsLoadingRegistered(false));
  }, [token]);

  // ─── Logout ───────────────────────────────────────────────────────────────
  const handleLogout = async () => {
    setLoggingOut(true);
    logout();
    window.history.replaceState({}, '', '/login');
    window.dispatchEvent(new PopStateEvent('popstate'));
  };

  // ─── Formatiranje datuma ───────────────────────────────────────────────────
  const formatDate = (iso: string) => {
    if (!iso) return 'N/A';
    try {
      return new Intl.DateTimeFormat('bs-BA', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
      }).format(new Date(iso));
    } catch {
      return iso;
    }
  };

  const formatDateTime = (iso: string) => {
    if (!iso) return 'N/A';
    try {
      const date = new Date(iso);
      const formattedDate = new Intl.DateTimeFormat('bs-BA', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
      }).format(date);
      const formattedTime = new Intl.DateTimeFormat('bs-BA', {
        hour: '2-digit',
        minute: '2-digit',
      }).format(date);
      return `${formattedDate} u ${formattedTime}`;
    } catch {
      return iso;
    }
  };

  const readApiMessage = async (response: Response): Promise<string | null> => {
    try {
      const data = await response.json();
      if (typeof data === 'string') return data;
      if (typeof data?.Message === 'string') return data.Message;
      if (typeof data?.message === 'string') return data.message;
      return null;
    } catch {
      return null;
    }
  };

  const handleCancelRegistration = async (conference: RegisteredConference) => {
    const registrationId = conference.conferenceRegistrationId ?? conference.conferenceId;
    if (!registrationId || cancellingRegistrationId) return;

    setCancellingRegistrationId(registrationId);
    try {
      const response = await fetch(`/api/registration/${registrationId}/cancel`, {
        method: 'PUT',
        headers: { Authorization: `Bearer ${token}` },
      });

      const message = await readApiMessage(response);
      if (!response.ok) {
        alert(message ?? 'Greška prilikom odjave.');
        return;
      }

      setRegisteredConferences((prev) =>
        prev.filter((item) => item.conferenceId !== conference.conferenceId)
      );
      alert(message ?? 'Odjava je uspješna.');
    } catch {
      alert('Greška prilikom odjave.');
    } finally {
      setCancellingRegistrationId(null);
    }
  };

  return (
    <div className={`dashboard-root ${sidebarOpen ? 'sidebar-open' : 'sidebar-closed'}`}>
      {/* ── Sidebar ─────────────────────────────────────────────── */}
      <aside className="sidebar">
        <div className="sidebar-logo">
          <div className="logo-mark">CM</div>
          <span className="logo-text">ConferenceHub</span>
          <button className="sidebar-toggle" onClick={() => setSidebarOpen(!sidebarOpen)} aria-label="Toggle sidebar">
            {sidebarOpen ? '‹' : '›'}
          </button>
        </div>

        <nav className="sidebar-nav">
          {[
            { id: 'dashboard', icon: '⬡', label: 'Dashboard', path: '/dashboard' },
            { id: 'conferences', icon: '🗓', label: 'Konferencije', path: '/conferences' },
            ...(isAdminOrOrganizer ? [
              { id: 'rooms', icon: '🏟', label: 'Dvorane', path: '/rooms' },
              { id: 'equipment', icon: '🛠', label: 'Oprema', path: '/equipment' }
            ] : []),
            { id: 'speakers', icon: '🎙', label: 'Govornici' },
            { id: 'reports', icon: '📊', label: 'Izvještaji' },
            { id: 'settings', icon: '⚙', label: 'Postavke' },
          ].map((item) => (
            <button
              key={item.id}
              className={`nav-item ${activeNav === item.id ? 'active' : ''}`}
              onClick={() => {
                if (item.path) {
                  window.history.pushState({}, '', item.path);
                }
                setActiveNav(item.id);
              }}
              id={`nav-${item.id}`}
            >
              <span className="nav-icon">{item.icon}</span>
              <span className="nav-label">{item.label}</span>
              {activeNav === item.id && <span className="nav-indicator" />}
            </button>
          ))}
        </nav>

        <div className="sidebar-footer">
          <div className="user-mini">
            <div className="avatar-sm">{initials}</div>
            <div className="user-mini-info">
              <span className="user-mini-name">{displayName}</span>
              <span className="user-mini-email">{user?.email ?? ''}</span>
            </div>
          </div>
          <button
            id="logout-btn"
            className={`logout-btn ${loggingOut ? 'logging-out' : ''}`}
            onClick={handleLogout}
            disabled={loggingOut}
            title="Odjava"
          >
            {loggingOut ? (
              <span className="logout-spinner" />
            ) : (
              <span>⏻</span>
            )}
            <span className="nav-label">{loggingOut ? 'Odjava...' : 'Odjavi se'}</span>
          </button>
        </div>
      </aside>

      {/* ── Glavni sadržaj ───────────────────────────────────────── */}
      <main className="dashboard-main">
        {/* Header */}
        <header className="dash-header">
          <div className="dash-header-left">
            <h1 className="dash-title">
              {activeNav === 'dashboard' && 'Dashboard'}
              {activeNav === 'conferences' && 'Konferencije'}
              {activeNav === 'speakers' && 'Govornici'}
              {activeNav === 'reports' && 'Izvještaji'}
              {activeNav === 'settings' && 'Postavke'}
            </h1>
            <span className="dash-subtitle">Dobrodošli nazad, {displayName.split(' ')[0]}! 👋</span>
          </div>
          <div className="dash-header-right">
            <NotificationBell />
            <div className="header-roles">
              {roles.map((r) => <RoleBadge key={r} role={r} />)}
            </div>
            <div className="avatar-lg" title={displayName}>{initials}</div>
          </div>
        </header>

        {/* ── Dashboard sadržaj ────────────────────────────────────── */}
        {activeNav === 'dashboard' && (
          <div className="dash-content">
            {isSpeaker ? (
              <>
                <section className="stats-grid">
                  <StatCard
                    icon="📽"
                    label="Moje Sesije (Ukupno)"
                    value={isLoadingSpeakerSessions ? '—' : speakerSessions.length}
                    trend="Dodijeljena predavanja"
                    color="63, 131, 248"
                  />
                  <StatCard
                    icon="🗓"
                    label="Aktivne Konferencije"
                    value={isLoadingSpeakerSessions ? '—' : new Set(
                      speakerSessions.map((session) => session.conferenceName ?? session.conferenceId ?? session.sessionId)
                    ).size}
                    trend="Predstojeća učešća"
                    color="139, 92, 246"
                  />
                </section>

                <section className="section-block">
                  <div className="section-header">
                    <h2 className="section-title">Moja predavanja i sesije</h2>
                  </div>

                  {isLoadingSpeakerSessions ? (
                    <div className="loading-row">
                      {[1, 2, 3].map((i) => (
                        <div key={i} className="skeleton-card" />
                      ))}
                    </div>
                  ) : speakerSessionsError ? (
                    <div className="error-message">Greška: {speakerSessionsError}</div>
                  ) : speakerSessions.length === 0 ? (
                    <div className="empty-state">
                      <div className="empty-icon">🗓</div>
                      <p>Nemate dodijeljenih sesija</p>
                    </div>
                  ) : (
                    <div className="conference-list speaker-sessions-list">
                      {speakerSessions.map((session) => (
                        <div key={session.sessionId} className="conference-row speaker-session-card">
                          <div className="speaker-card-left">
                            <div className="speaker-card-row">
                              <span className="session-card-title">{session.title}</span>
                            </div>
                            <div className="speaker-card-row">
                              <span className="session-card-subtitle">
                                Konferencija: {session.conferenceName ?? 'N/A'}
                              </span>
                            </div>
                            <div className="speaker-card-row">
                              <span className="session-card-meta">
                                🕐 {formatDateTime(session.startTime)}{session.roomName ? ` | 📍 ${session.roomName}` : ''}
                              </span>
                            </div>
                          </div>
                          <button
                            type="button"
                            className="btn-secondary"
                            onClick={() => {
                              window.history.pushState({}, '', `/sessions/${session.sessionId}`);
                              window.dispatchEvent(new PopStateEvent('popstate'));
                            }}
                          >
                            Vidi Detalje
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </section>

                <section className="section-block user-info-section">
                  <div className="section-header">
                    <h2 className="section-title">Informacije o nalogu</h2>
                  </div>
                  <div className="user-info-grid">
                    <div className="info-item">
                      <span className="info-label">Korisničko ime</span>
                      <span className="info-value">{user?.username ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Email</span>
                      <span className="info-value">{user?.email ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Ime</span>
                      <span className="info-value">{user?.firstName ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Prezime</span>
                      <span className="info-value">{user?.lastName ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">User ID</span>
                      <span className="info-value mono">{user?.userId?.slice(0, 16) ?? '—'}…</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Uloge</span>
                      <span className="info-value">
                        {roles.length > 0 ? roles.join(', ') : 'Nema posebnih uloga'}
                      </span>
                    </div>
                  </div>
                </section>
              </>
            ) : (
              <>
                {/* Stat kartice */}
                <section className="stats-grid">
                  <StatCard
                    icon="🗓"
                    label="Ukupno konferencija"
                    value={isLoadingConferences ? '—' : conferences.length}
                    trend={conferences.length > 0 ? `+${conferences.length} aktivnih` : 'Nema podataka'}
                    color="63, 131, 248"
                  />
                  <StatCard
                    icon="🎙"
                    label="Govornici"
                    value="—"
                    trend="Uskoro dostupno"
                    color="139, 92, 246"
                  />
                  <StatCard
                    icon="👥"
                    label="Učesnici"
                    value="—"
                    trend="Uskoro dostupno"
                    color="16, 185, 129"
                  />
                  <StatCard
                    icon="📍"
                    label="Lokacije"
                    value={
                      isLoadingConferences
                        ? '—'
                        : new Set(conferences.map((c) => c.location)).size
                    }
                    trend="Globalno"
                    color="245, 158, 11"
                  />
                </section>

                {isAdmin && <AdminUsersPanel />}

                {/* Registrovane konferencije */}
                {isParticipant && (
                  <section className="section-block">
                    <div className="section-header">
                      <h2 className="section-title">Moje prijave</h2>
                    </div>

                    {isLoadingRegistered ? (
                      <div className="loading-row">
                        {[1, 2, 3].map((i) => (
                          <div key={i} className="skeleton-card" />
                        ))}
                      </div>
                    ) : registeredConferences.length === 0 ? (
                      <div className="empty-state">
                        <div className="empty-icon">✅</div>
                        <p>Nema aktivnih prijava</p>
                        <span>Prijavljene konferencije će se pojaviti ovdje</span>
                      </div>
                    ) : (
                      <div className="conference-list">
                        {registeredConferences.map((conf) => (
                          <div
                            key={conf.conferenceId}
                            className="conference-row"
                            onClick={() => {
                              window.history.pushState({}, '', `/conferences/${conf.conferenceId}`);
                              window.dispatchEvent(new PopStateEvent('popstate'));
                            }}
                            style={{ cursor: 'pointer' }}
                          >
                            <div className="conf-left">
                              <div className="conf-dot" />
                              <div>
                                <span className="conf-title">{conf.title}</span>
                                <span className="conf-location">📍 {conf.location}</span>
                              </div>
                            </div>
                            <div
                              className="conf-right"
                              style={{ display: 'flex', alignItems: 'center', gap: '12px' }}
                            >
                              <span className="conf-date">{formatDate(conf.startDate)}</span>
                              <span className="conf-badge">{conf.status || 'Aktivan'}</span>
                              <button
                                className="logout-btn logout-btn-inline"
                                onClick={(event) => {
                                  event.stopPropagation();
                                  handleCancelRegistration(conf);
                                }}
                                disabled={cancellingRegistrationId === (conf.conferenceRegistrationId ?? conf.conferenceId)}
                                title="Odjavi"
                                style={{ marginLeft: 'auto' }}
                              >
                                Odjavi
                              </button>
                            </div>
                          </div>
                        ))}
                      </div>
                    )}
                  </section>
                )}

                {/* Nadolazeće konferencije */}
                <section className="section-block">
                  <div className="section-header">
                    <h2 className="section-title">Nadolazeće konferencije</h2>
                    <button
                      className="btn-secondary"
                      onClick={() => window.history.pushState({}, '', '/conferences')}
                      id="view-all-conferences"
                    >
                      Vidi sve →
                    </button>
                  </div>

                  {isLoadingConferences ? (
                    <div className="loading-row">
                      {[1, 2, 3].map((i) => (
                        <div key={i} className="skeleton-card" />
                      ))}
                    </div>
                  ) : conferences.length === 0 ? (
                    <div className="empty-state">
                      <div className="empty-icon">🗓</div>
                      <p>Nema pronađenih konferencija</p>
                      <span>Konferencije će se pojaviti ovdje čim budu dodane</span>
                    </div>
                  ) : (
                    <div className="conference-list">
                      {conferences
                        .filter(c => new Date(c.startDate) >= new Date())
                        .sort((a, b) => new Date(a.startDate).getTime() - new Date(b.startDate).getTime())
                        .slice(0, 5)
                        .map((conf) => (
                        <div key={conf.conferenceId} className="conference-row" id={`conf-${conf.conferenceId}`}>
                          <div className="conf-left">
                            <div className="conf-dot" />
                            <div>
                              <span className="conf-title">{conf.title}</span>
                              <span className="conf-location">📍 {conf.location}</span>
                            </div>
                          </div>
                          <div className="conf-right">
                            <span className="conf-date">{formatDate(conf.startDate)}</span>
                            <span className="conf-badge">{conf.status || 'Aktivan'}</span>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </section>

                {/* User info kartica */}
                <section className="section-block user-info-section">
                  <div className="section-header">
                    <h2 className="section-title">Informacije o nalogu</h2>
                  </div>
                  <div className="user-info-grid">
                    <div className="info-item">
                      <span className="info-label">Korisničko ime</span>
                      <span className="info-value">{user?.username ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Email</span>
                      <span className="info-value">{user?.email ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Ime</span>
                      <span className="info-value">{user?.firstName ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Prezime</span>
                      <span className="info-value">{user?.lastName ?? '—'}</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">User ID</span>
                      <span className="info-value mono">{user?.userId?.slice(0, 16) ?? '—'}…</span>
                    </div>
                    <div className="info-item">
                      <span className="info-label">Uloge</span>
                      <span className="info-value">
                        {roles.length > 0 ? roles.join(', ') : 'Nema posebnih uloga'}
                      </span>
                    </div>
                  </div>
                </section>
              </>
            )}
          </div>
        )}

        {/* ── Konferencije tab ─────────────────────────────────────── */}
        {activeNav === 'conferences' && (
          <div className="dash-content">
            <section className="section-block">
              <div className="section-header">
                <h2 className="section-title">Sve konferencije</h2>
                <span className="badge-count">{conferences.length} ukupno</span>
              </div>
              {isLoadingConferences ? (
                <div className="loading-row">
                  {[1, 2, 3, 4].map((i) => <div key={i} className="skeleton-card" />)}
                </div>
              ) : conferences.length === 0 ? (
                <div className="empty-state">
                  <div className="empty-icon">🗓</div>
                  <p>Nema pronađenih konferencija</p>
                </div>
              ) : (
                <div className="conference-table">
                  <div className="table-header">
                    <span>Naziv</span>
                    <span>Lokacija</span>
                    <span>Datum</span>
                    <span>Status</span>
                  </div>
                  {conferences.map((conf) => (
                    <div key={conf.conferenceId} className="table-row" id={`table-conf-${conf.conferenceId}`}>
                      <span className="table-title">{conf.title}</span>
                      <span className="table-location">📍 {conf.location}</span>
                      <span className="table-date">{formatDate(conf.startDate)}</span>
                      <span className="conf-badge">{conf.status || 'Aktivan'}</span>
                    </div>
                  ))}
                </div>
              )}
            </section>
          </div>
        )}

        {/* ── Ostali tabovi (placeholder) ──────────────────────────── */}
        {['speakers', 'reports'].includes(activeNav) && (
          <div className="dash-content">
            <div className="coming-soon">
              <div className="coming-soon-icon">
                {activeNav === 'speakers' && '🎙'}
                {activeNav === 'reports' && '📊'}
              </div>
              <h2>Uskoro dostupno</h2>
              <p>Ova sekcija je u razvoju.</p>
            </div>
          </div>
        )}

        {activeNav === 'settings' && (
          <div className="dash-content">
            <section className="section-block user-info-section">
              <SettingsPage />
            </section>
          </div>
        )}
      </main>
    </div>
  );
}
