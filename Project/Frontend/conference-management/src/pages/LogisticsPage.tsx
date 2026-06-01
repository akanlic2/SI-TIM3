import { useState } from 'react';
import { LogisticsList, LogisticsForm, useLogistics } from '../features/logistics';
import { useAuth } from '../auth/AuthProvider';
import { LOGISTICS_TASK_TYPES } from '../features/logistics/types';
import type { LogisticsTask } from '../features/logistics/types';
import '../features/conference/ConferencesPage.css';

export default function LogisticsPage() {
  const conferenceId = window.location.pathname.split('/')[2]; // /conferences/{id}/logistics

  const { user, isLoading: isAuthLoading } = useAuth();

  const [showForm, setShowForm] = useState(false);
  const [editingItem, setEditingItem] = useState<LogisticsTask | null>(null);
  const [typeFilter, setTypeFilter] = useState('');

  const {
    items,
    isLoading: isDataLoading,
    error,
    refresh,
  } = useLogistics(conferenceId, typeFilter || undefined);

  const role = user?.role?.toLowerCase() ?? '';
  const isAdminOrOrganizer = role === 'admin-sistema' || role === 'organizator';

  const resetForm = () => {
    setEditingItem(null);
  };

  const handleCreateClick = () => {
    resetForm();
    setShowForm(true);
  };

  const handleEditClick = (item: LogisticsTask) => {
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
              <h1>Logističke aktivnosti</h1>
              <p>Pregledajte, dodajte ili uredite logističke aktivnosti konferencije</p>
            </div>
          </div>

          {isAdminOrOrganizer && (
            <button onClick={handleCreateClick} className="btn-primary">
              + Kreiraj aktivnost
            </button>
          )}
        </div>
      </div>

      <div className="conferences-content">
        {showForm && (
          <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
            <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
              <h2 className="modal-title">
                {editingItem ? 'Uredi aktivnost' : 'Nova logistička aktivnost'}
              </h2>

              <LogisticsForm
                conferenceId={conferenceId}
                editingItem={editingItem}
                onSuccess={handleFormSuccess}
                onCancel={() => setShowForm(false)}
              />
            </div>
          </div>
        )}

        {/* Filter po tipu */}
        <div style={{ padding: '0 0 16px', display: 'flex', alignItems: 'center', gap: '12px' }}>
          <label style={{ fontSize: '0.875rem', color: '#94a3b8' }}>Filtriraj po tipu:</label>
          <select
            value={typeFilter}
            onChange={(e) => setTypeFilter(e.target.value)}
            className="form-select"
            style={{ maxWidth: '220px' }}
          >
            <option value="">Svi tipovi</option>
            {LOGISTICS_TASK_TYPES.map((t) => (
              <option key={t.value} value={t.value}>
                {t.label}
              </option>
            ))}
          </select>
        </div>

        <div>
          {error && <div className="error-message">Greška: {error}</div>}

          {isDataLoading ? (
            <div className="loading-container">
              <div className="loading-spinner"></div>
              <p className="loading-text">Učitavanje logističkih aktivnosti...</p>
            </div>
          ) : (
            <LogisticsList
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
