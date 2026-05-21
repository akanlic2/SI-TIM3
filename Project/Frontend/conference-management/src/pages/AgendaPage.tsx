import { useState } from 'react';
import { AgendaList, AgendaForm, useAgenda } from '../features/agenda';
import { useAuth } from '../auth/AuthProvider';
import type { AgendaItem } from '../features/agenda/types';
import '../features/conference/ConferencesPage.css'; // Reuse styles for consistency

export default function AgendaPage() {
  const conferenceId = window.location.pathname.split('/')[2]; // /conferences/{id}/agenda

  const {
    items,
    isLoading: isDataLoading,
    error,
    refresh,
  } = useAgenda(conferenceId);

  const { user, isLoading: isAuthLoading } = useAuth();

  const [showForm, setShowForm] = useState(false);
  const [editingItem, setEditingItem] = useState<AgendaItem | null>(null);

  const role = user?.role?.toLowerCase() ?? '';
  const isAdminOrOrganizer = role === 'admin-sistema' || role === 'organizator';

  const resetForm = () => {
    setEditingItem(null);
  };

  const handleCreateClick = () => {
    resetForm();
    setShowForm(true);
  };

  const handleEditClick = (item: AgendaItem) => {
    setEditingItem(item);
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
              <h1>Agenda konferencije</h1>
              <p>Pregledajte raspored, dodajte nove ili uredite postojeće stavke agende</p>
            </div>
          </div>

          {isAdminOrOrganizer && (
            <button onClick={handleCreateClick} className="btn-primary">
              + Dodaj stavku
            </button>
          )}
        </div>
      </div>

      <div className="conferences-content">
        {showForm && (
          <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
            <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
              <h2 className="modal-title">
                {editingItem ? 'Uredi stavku agende' : 'Nova stavka agende'}
              </h2>

              <AgendaForm
                conferenceId={conferenceId}
                editingItem={editingItem}
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
              <p className="loading-text">Učitavanje agende...</p>
            </div>
          ) : (
            <AgendaList
              items={items}
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
