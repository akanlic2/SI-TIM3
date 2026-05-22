import { useEffect, useState } from 'react';
import { useAuth } from '../auth/AuthProvider';
import { useMaterials } from '../features/session/hooks/useMaterials';
import { UploadMaterialModal } from '../features/session/components/UploadMaterialModal';
import '../features/conference/ConferencesPage.css';

export default function SessionDetailsPage() {
  const { token, isLoading: isAuthLoading } = useAuth();
  const sessionId = window.location.pathname.split('/')[2] ?? '';
  const [showUploadModal, setShowUploadModal] = useState(false);
  const {
    items: materials,
    isLoading: isLoadingMaterials,
    error: materialsError,
    refresh: refreshMaterials,
  } = useMaterials(sessionId);

  interface SpeakerSessionDetails {
    sessionId: string;
    title: string;
    description?: string;
    startTime: string;
    endTime: string;
    sessionType: string;
    status?: string;
    conferenceTitle?: string;
    location?: string;
    roomName?: string;
    speakerName?: string;
    attendees?: {
      userId: string;
      firstName: string;
      lastName: string;
      email: string;
      registrationDate: string;
    }[];
  }

  const [session, setSession] = useState<SpeakerSessionDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const formatDateTime = (dateString: string) => {
    try {
      return new Intl.DateTimeFormat('bs-BA', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(new Date(dateString));
    } catch {
      return dateString;
    }
  };

  useEffect(() => {
    if (!token || !sessionId) return;

    setIsLoading(true);
    setError(null);

    fetch(`/api/speakers/sessions/${sessionId}`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((r) => {
        if (!r.ok) throw new Error('Greška pri dohvatanju sesije.');
        return r.json();
      })
      .then((data) => {
        setSession(data as SpeakerSessionDetails);
      })
      .catch((err) => {
        const message = err instanceof Error ? err.message : 'Greška pri dohvatanju sesije.';
        setError(message);
      })
      .finally(() => setIsLoading(false));
  }, [token, sessionId]);

  const goBack = () => {
    window.history.pushState({}, '', '/dashboard');
    window.dispatchEvent(new PopStateEvent('popstate'));
  };

  if (isAuthLoading) {
    return (
      <main className="conferences-page">
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p className="loading-text">Učitavanje autorizacije...</p>
        </div>
      </main>
    );
  }

  if (isLoading) {
    return (
      <main className="conferences-page">
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p className="loading-text">Učitavanje detalja sesije...</p>
        </div>
      </main>
    );
  }

  if (error) {
    return (
      <main className="conferences-page">
        <div className="conferences-content">
          <div className="error-message">Greška: {error}</div>
          <button onClick={goBack} className="btn-secondary">← Nazad</button>
        </div>
      </main>
    );
  }

  if (!session) {
    return (
      <main className="conferences-page">
        <div className="conferences-content">
          <div className="error-message">Sesija nije pronađena.</div>
          <button onClick={goBack} className="btn-secondary">← Nazad</button>
        </div>
      </main>
    );
  }

  return (
    <main className="conferences-page">
      <div className="conferences-header">
        <div className="conferences-header-content">
          <div className="conferences-title-section" style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
            <button
              type="button"
              onClick={goBack}
              className="back-button"
              style={{
                width: '38px',
                height: '38px',
                minWidth: '38px',
                borderRadius: '9999px',
                backgroundColor: '#0f172a',
                color: '#cbd5e1',
                border: '1px solid rgba(148,163,184,0.2)',
                display: 'inline-flex',
                alignItems: 'center',
                justifyContent: 'center',
                cursor: 'pointer',
              }}
            >
              ←
            </button>

            <div>
              <h1>Moje sesije</h1>
              <p>Pregled detalja sesije</p>
            </div>
          </div>
        </div>
      </div>

      <div className="conferences-content">
        <div className="session-card" style={{ maxWidth: '900px', margin: '0 auto' }}>
          <div className="session-card-header">
            <h3 className="session-card-title">{session.title}</h3>
            <span className={`session-status session-status-${session.status?.toLowerCase()}`}>
              {session.status}
            </span>
          </div>

          {session.description && (
            <p style={{ color: '#94a3b8', fontSize: '14px', margin: '8px 0 4px 0' }}>
              {session.description}
            </p>
          )}

          {/* Naziv konferencije se ispisuje odmah ispod opisa sesije */}
          {session.conferenceTitle && (
            <p style={{ color: '#64748b', fontSize: '14px', fontWeight: 500, margin: '0 0 16px 0' }}>
              Konferencija: {session.conferenceTitle}
            </p>
          )}

          <div className="session-card-info">
            <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="session-info-icon">🚨</span>
              <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="session-info-label">POČETAK</span>
                <p className="session-info-value">{formatDateTime(session.startTime)}</p>
              </div>
            </div>

            <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="session-info-icon">🏁</span>
              <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="session-info-label">ZAVRŠETAK</span>
                <p className="session-info-value">{formatDateTime(session.endTime)}</p>
              </div>
            </div>

            <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="session-info-icon">🏷️</span>
              <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="session-info-label">TIP SESIJE</span>
                <p className="session-info-value">{session.sessionType}</p>
              </div>
            </div>

            {/* Lokacija ispisuje samo čistu lokaciju iz baze */}
            {session.location && (
              <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
                <span className="session-info-icon">📍</span>
                <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                  <span className="session-info-label">LOKACIJA</span>
                  <p className="session-info-value">{session.location}</p>
                </div>
              </div>
            )}

            {/* Dvorana ispisuje roomName sa backenda */}
            <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="session-info-icon">🏢</span>
              <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                <span className="session-info-label">DVORANA</span>
                <p className="session-info-value">{session.roomName ?? 'Nije dostupno'}</p>
              </div>
            </div>

            <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
              <span className="session-info-icon">📎</span>
              <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <span className="session-info-label">MATERIJALI</span>
                {isLoadingMaterials ? (
                  <p className="session-info-value">Učitavanje materijala...</p>
                ) : materialsError ? (
                  <p className="session-info-value">{materialsError}</p>
                ) : materials.length === 0 ? (
                  <p className="session-info-value">Nema materijala</p>
                ) : (
                  materials.map((material, index) => (
                    <div key={material.materialId} style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
                      <div>
                        <strong>{material.title}</strong>
                        {material.description && (
                          <p className="session-info-value" style={{ margin: '4px 0 0 0' }}>
                            {material.description}
                          </p>
                        )}
                      </div>
                      <button
                        type="button"
                        className="btn-primary-sm"
                        style={{ width: 'fit-content', padding: '8px 16px', cursor: 'pointer', border: 'none' }}
                        onClick={() => {
                          const url = `${import.meta.env.VITE_API_URL ?? 'http://localhost:8082'}${material.fileUrl}`;
                          window.open(url, '_blank');
                        }}
                      >
                        Preuzmi
                      </button>
                      {index < materials.length - 1 && <hr style={{ borderColor: 'rgba(148,163,184,0.2)', margin: '8px 0' }} />}
                    </div>
                  ))
                )}
              </div>
            </div>

            {session.speakerName && (
              <div className="session-info-row" style={{ display: 'flex', alignItems: 'flex-start', gap: '0.75rem', flexWrap: 'wrap' }}>
                <span className="session-info-icon">🎤</span>
                <div className="session-info-content" style={{ display: 'flex', flexDirection: 'column', gap: '0.15rem' }}>
                  <span className="session-info-label">PREDAVAČ</span>
                  <p className="session-info-value">{session.speakerName}</p>
                </div>
              </div>
            )}
          </div>

          <div className="session-card-actions" style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px', paddingTop: '16px', borderTop: '1px solid rgba(148,163,184,0.2)', flexWrap: 'wrap' }}>
            <button
              className="btn-edit"
              style={{
                backgroundColor: '#EAB308',
                color: '#000',
                borderRadius: '9999px',
                padding: '8px 20px',
                border: 'none',
                cursor: 'pointer',
              }}
              onClick={() => setShowUploadModal(true)}
            >
              Upload Materijala
            </button>
          </div>

          {showUploadModal && (
            <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
              <UploadMaterialModal
                sessionId={session.sessionId}
                onCancel={() => setShowUploadModal(false)}
                onSuccess={() => {
                  setShowUploadModal(false);
                  refreshMaterials();
                }}
              />
            </div>
          )}
        </div>
      </div>
    </main>
  );
}