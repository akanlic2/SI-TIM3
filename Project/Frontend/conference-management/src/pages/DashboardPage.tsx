import React, { useEffect, useState, type CSSProperties } from 'react';
import { useAuth } from '../auth/AuthProvider';
import './DashboardPage.css';

// ─── Tip za konferenciju ───────────────────────────────────────────────────────
interface Conference {
  conferenceId: string;
  title: string;
  location: string;
  startDate: string;
  status: string;
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
  const [loggingOut, setLoggingOut] = useState(false);
  const [activeNav, setActiveNav] = useState('dashboard');
  const [sidebarOpen, setSidebarOpen] = useState(true);

  const displayName =
    user?.name ?? user?.preferred_username ?? user?.email ?? 'Korisnik';
  const initials = displayName
    .split(' ')
    .map((n) => n[0])
    .join('')
    .slice(0, 2)
    .toUpperCase();

  const roles = user?.realm_access?.roles?.filter(
    (r) => !['default-roles-conference-app', 'offline_access', 'uma_authorization'].includes(r)
  ) ?? [];

  // ─── Dohvat konferencija ───────────────────────────────────────────────────
  useEffect(() => {
    if (!token) return;
    fetch('/api/Conference', {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((r) => (r.ok ? r.json() : []))
      .then((data) => setConferences(Array.isArray(data) ? data : []))
      .catch(() => setConferences([]))
      .finally(() => setIsLoadingConferences(false));
  }, [token]);

  // ─── Logout ───────────────────────────────────────────────────────────────
  const handleLogout = async () => {
    setLoggingOut(true);
    await logout();
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
            { id: 'dashboard', icon: '⬡', label: 'Dashboard' },
            { id: 'conferences', icon: '🗓', label: 'Konferencije' },
            { id: 'speakers', icon: '🎙', label: 'Govornici' },
            { id: 'reports', icon: '📊', label: 'Izvještaji' },
            { id: 'settings', icon: '⚙', label: 'Postavke' },
          ].map((item) => (
            <button
              key={item.id}
              className={`nav-item ${activeNav === item.id ? 'active' : ''}`}
              onClick={() => setActiveNav(item.id)}
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
            <div className="header-roles">
              {roles.map((r) => <RoleBadge key={r} role={r} />)}
            </div>
            <div className="avatar-lg" title={displayName}>{initials}</div>
          </div>
        </header>

        {/* ── Dashboard sadržaj ────────────────────────────────────── */}
        {activeNav === 'dashboard' && (
          <div className="dash-content">
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

            {/* Nadolazeće konferencije */}
            <section className="section-block">
              <div className="section-header">
                <h2 className="section-title">Nadolazeće konferencije</h2>
                <button
                  className="btn-secondary"
                  onClick={() => setActiveNav('conferences')}
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
                  {conferences.slice(0, 5).map((conf) => (
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
                  <span className="info-value">{user?.preferred_username ?? '—'}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Email</span>
                  <span className="info-value">{user?.email ?? '—'}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Ime</span>
                  <span className="info-value">{user?.given_name ?? '—'}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Prezime</span>
                  <span className="info-value">{user?.family_name ?? '—'}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Keycloak ID</span>
                  <span className="info-value mono">{user?.sub?.slice(0, 16) ?? '—'}…</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Uloge</span>
                  <span className="info-value">
                    {roles.length > 0 ? roles.join(', ') : 'Nema posebnih uloga'}
                  </span>
                </div>
              </div>
            </section>
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
        {['speakers', 'reports', 'settings'].includes(activeNav) && (
          <div className="dash-content">
            <div className="coming-soon">
              <div className="coming-soon-icon">
                {activeNav === 'speakers' && '🎙'}
                {activeNav === 'reports' && '📊'}
                {activeNav === 'settings' && '⚙'}
              </div>
              <h2>Uskoro dostupno</h2>
              <p>Ova sekcija je u razvoju.</p>
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
