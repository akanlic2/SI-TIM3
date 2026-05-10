import { useState } from 'react';
import { SessionList, SessionForm, useSessions } from '../features/session';
import { useAuth } from '../auth/AuthProvider';
import type { Session } from '../features/session/types';
import '../features/conference/ConferencesPage.css'; // Reuse styles

export default function SessionsPage() {
  const conferenceId = window.location.pathname.split('/')[2]; // /conferences/{id}/sessions

  const {
    items,
    isLoading: isDataLoading,
    error,
    refresh,
  } = useSessions(conferenceId);

  const { user, isLoading: isAuthLoading } = useAuth();

  const [showForm, setShowForm] = useState(false);
  const [editingSession, setEditingSession] = useState<Session | null>(null);

  const role = user?.role?.toLowerCase() ?? '';
  const isAdminOrOrganizer = role === 'admin-sistema' || role === 'organizator';

  const resetForm = () => {
    setEditingSession(null);
  };

  const handleCreateClick = () => {
    resetForm();
    setShowForm(true);
  };

  const handleEditClick = (session: Session) => {
    setEditingSession(session);
    setShowForm(true);
  };

  const handleFormSuccess = () => {
    setShowForm(false);
    resetForm();
    refresh();
  };

  const goBack = () => {
    window.history.pushState({}, '', `/conferences/${conferenceId}`);
    window.dispatchEvent(new PopStateEvent('popstate'));
  };

  if (isAuthLoading) {
    return (
      <div className="p-8 text-white bg-[#0b0e14] min-h-screen font-sans">
        Učitavanje autorizacije...
      </div>
    );
  }

  return (
    <main className="conferences-page">
      <div className="conferences-header">
        <div className="conferences-header-content">
          <div
            className="conferences-title-section"
            style={{ display: 'flex', alignItems: 'center', gap: '16px' }}
          >
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
              <h1>Sesije konferencije</h1>
              <p>Pregledajte sesije, dodajte nove ili uredite postojeće</p>
            </div>
          </div>

          {isAdminOrOrganizer && (
            <button onClick={handleCreateClick} className="btn-primary">
              + Kreiraj sesiju
            </button>
          )}
        </div>
      </div>

      <div className="conferences-content">
        {showForm && (
          <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
            <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
              <h2 className="modal-title">
                {editingSession ? 'Uredi sesiju' : 'Nova Sesija'}
              </h2>

              <SessionForm
                conferenceId={conferenceId}
                editingSession={editingSession}
                onSuccess={handleFormSuccess}
                onCancel={() => setShowForm(false)}
              />
            </div>
          </div>
        )}

        <div>
          {error && <div className="error-message">Greška: {error}</div>}

          {isDataLoading ? (
            <div className="loading-container">
              <div className="loading-spinner"></div>
              <p className="loading-text">Učitavanje sesija iz baze...</p>
            </div>
          ) : (
            <SessionList
              sessions={items}
              conferenceId={conferenceId}
              isAdminOrOrganizer={isAdminOrOrganizer}
              onDeleteSuccess={refresh}
              onEditClick={handleEditClick}
            />
          )}
        </div>
      </div>
    </main>
  );
}