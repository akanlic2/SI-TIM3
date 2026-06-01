import { useEffect, useState } from 'react';
import { useAuth } from '../auth/AuthProvider';
import { useEquipment } from '../features/equipment/hooks/useEquipment';
import { EquipmentList } from '../features/equipment/components/EquipmentList';
import { CreateEquipmentModal } from '../features/equipment/components/CreateEquipmentModal';
import { decrementEquipmentQuantity, deleteEquipment } from '../features/equipment/api/equipmentApi';
import type { Equipment } from '../features/equipment/types';
import '../features/conference/ConferencesPage.css';
import './RoomsPage.css'; // Ponovno koristimo prelijepe stilove za raspored

export default function EquipmentPage() {
  const { user, isLoading: isAuthLoading } = useAuth();
  const role = user?.role?.toLowerCase() ?? '';
  const isAdminOrOrganizer = role === 'admin-sistema' || role === 'organizator';
  const [redirecting, setRedirecting] = useState(false);

  const [showForm, setShowForm] = useState(false);
  const [equipmentToDelete, setEquipmentToDelete] = useState<Equipment | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteModalStep, setDeleteModalStep] = useState<'confirm' | 'error'>('confirm');
  const [reducingEquipmentId, setReducingEquipmentId] = useState<string | null>(null);
  const [decrementToast, setDecrementToast] = useState<string | null>(null);

  // Filtriranje i pretraga
  const [searchQuery, setSearchQuery] = useState('');
  const [typeFilter, setTypeFilter] = useState('');
  const [availabilityFilter, setAvailabilityFilter] = useState('');

  const { items: equipmentItems, isLoading: isDataLoading, error, refresh } = useEquipment();

  const handleDeleteConfirm = async () => {
    if (!equipmentToDelete) return;

    setIsDeleting(true);
    setDeleteError(null);

    try {
      await deleteEquipment(equipmentToDelete.equipmentId);
      setEquipmentToDelete(null);
      refresh();
    } catch (err) {
      console.error('Greška pri brisanju opreme:', err);
      setDeleteError(err instanceof Error ? err.message : 'Greška pri brisanju opreme.');
      setDeleteModalStep('error');
    } finally {
      setIsDeleting(false);
    }
  };

  // Provjera prava pristupa
  useEffect(() => {
    if (isAuthLoading) return;
    if (!isAdminOrOrganizer) {
      window.history.replaceState({}, '', '/dashboard');
      window.dispatchEvent(new PopStateEvent('popstate'));
      setRedirecting(true);
    }
  }, [isAdminOrOrganizer, isAuthLoading]);

  if (isAuthLoading) {
    return (
      <div className="rooms-loading">
        <div className="global-spinner" />
        <p>Učitavanje autorizacije...</p>
      </div>
    );
  }

  if (redirecting) {
    return null;
  }

  // Filtriranje na klijentskoj strani
  const filteredEquipment = equipmentItems.filter((item) => {
    const matchesSearch =
      item.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      item.type.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesType = typeFilter === '' || item.type.toLowerCase() === typeFilter.toLowerCase();

    const matchesAvailability =
      availabilityFilter === '' ||
      (availabilityFilter === 'available' && item.isAvailable) ||
      (availabilityFilter === 'unavailable' && !item.isAvailable);

    return matchesSearch && matchesType && matchesAvailability;
  });

  // Izvlacenje jedinstvenih tipova opreme za filter dropdown
  const uniqueTypes = Array.from(new Set(equipmentItems.map((e) => e.type)));

  return (
    <>
      <main className="rooms-page">
        <div className="rooms-header">
          <div className="rooms-header-content">
            <div
              className="rooms-title-section"
              style={{ display: 'flex', alignItems: 'center', gap: '16px' }}
            >
              <button
                type="button"
                onClick={() => {
                  window.history.pushState({}, '', '/dashboard');
                  window.dispatchEvent(new PopStateEvent('popstate'));
                }}
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
                <h1>Tehnička oprema</h1>
                <p>Upravljajte globalnim inventarom opreme za konferencije</p>
              </div>
            </div>

            {isAdminOrOrganizer && (
              <button
                type="button"
                onClick={() => setShowForm(true)}
                className="btn-primary"
              >
                + Dodaj opremu
              </button>
            )}
          </div>
        </div>

        <div className="rooms-content">
          {/* Filteri */}
          <div
            style={{
              display: 'flex',
              gap: '12px',
              marginBottom: '24px',
              flexWrap: 'wrap',
              alignItems: 'center',
            }}
          >
            <input
              type="text"
              placeholder="Pretraži po nazivu ili tipu..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="form-input"
              style={{ maxWidth: '280px' }}
            />

            <select
              value={typeFilter}
              onChange={(e) => setTypeFilter(e.target.value)}
              className="form-select"
              style={{ maxWidth: '190px' }}
            >
              <option value="">Svi tipovi</option>
              {uniqueTypes.map((type) => (
                <option key={type} value={type}>
                  {type}
                </option>
              ))}
            </select>

            <select
              value={availabilityFilter}
              onChange={(e) => setAvailabilityFilter(e.target.value)}
              className="form-select"
              style={{ maxWidth: '190px' }}
            >
              <option value="">Svi statusi</option>
              <option value="available">Dostupno</option>
              <option value="unavailable">Nedostupno</option>
            </select>
          </div>

          {error && <div className="error-message">{error}</div>}
          {decrementToast && (
            <div
              role="status"
              style={{
                marginBottom: '16px',
                padding: '10px 14px',
                borderRadius: '10px',
                border: '1px solid rgba(248, 113, 113, 0.35)',
                background: 'rgba(248, 113, 113, 0.12)',
                color: '#fca5a5',
                fontSize: '0.85rem',
                fontWeight: 600,
              }}
            >
              {decrementToast}
            </div>
          )}

          {isDataLoading ? (
            <div className="loading-container">
              <div className="loading-spinner" />
              <p className="loading-text">Učitavanje opreme iz baze...</p>
            </div>
          ) : (
            <EquipmentList
              items={filteredEquipment}
              isAdminOrOrganizer={isAdminOrOrganizer}
              onReduceTotal={async (item) => {
                if (item.availableQuantity <= 0 || reducingEquipmentId) return;
                const shouldReduce = window.confirm(
                  `Smanjiti ukupnu kolicinu opreme "${item.name}" za 1?`
                );
                if (!shouldReduce) return;
                setReducingEquipmentId(item.equipmentId);
                try {
                  await decrementEquipmentQuantity(item.equipmentId);
                  refresh();
                } catch (err) {
                  console.error('Greška pri smanjenju opreme:', err);
                  setDecrementToast(
                    err instanceof Error ? err.message : 'Greška pri smanjenju opreme.'
                  );
                  window.setTimeout(() => setDecrementToast(null), 3000);
                } finally {
                  setReducingEquipmentId(null);
                }
              }}
              reducingEquipmentId={reducingEquipmentId}
              onAction={(item) => {
                setEquipmentToDelete(item);
                setDeleteError(null);
                setDeleteModalStep('confirm');
              }}
              actionLabel="Obriši"
              isSessionView={false}
            />
          )}
        </div>
      </main>

      {showForm && (
        <div
          style={{
            position: 'fixed',
            inset: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.6)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 2000,
            overflowY: 'auto',
            WebkitOverflowScrolling: 'touch',
          }}
        >
          <CreateEquipmentModal
            onCancel={() => setShowForm(false)}
            onSuccess={() => {
              setShowForm(false);
              refresh();
            }}
          />
        </div>
      )}

      {equipmentToDelete && (
        <div
          style={{
            position: 'fixed',
            inset: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.6)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 2000,
            overflowY: 'auto',
            WebkitOverflowScrolling: 'touch',
          }}
        >
          <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
            {deleteModalStep === 'confirm' ? (
              <>
                <h2 className="modal-title">Potvrda brisanja</h2>
                <p style={{ margin: '16px 0 24px 0', color: '#cbd5e1' }}>
                  Želite li zaista obrisati opremu <strong>{equipmentToDelete.name}</strong>?
                </p>
                <div className="form-actions">
                  <button
                    type="button"
                    onClick={() => {
                      setEquipmentToDelete(null);
                      setDeleteError(null);
                      setDeleteModalStep('confirm');
                    }}
                    className="btn-secondary"
                  >
                    Ne
                  </button>
                  <button
                    type="button"
                    onClick={handleDeleteConfirm}
                    className="btn-delete"
                    disabled={isDeleting}
                    style={{
                      backgroundColor: '#EF4444',
                      color: '#fff',
                      borderRadius: '9999px',
                      padding: '8px 24px',
                      border: 'none',
                      cursor: 'pointer',
                    }}
                  >
                    {isDeleting ? 'Brisanje...' : 'Da'}
                  </button>
                </div>
              </>
            ) : (
              <>
                <h2 className="modal-title" style={{ color: '#EF4444' }}>Nije moguće obrisati</h2>
                <div style={{ margin: '20px 0' }}>
                  <div
                    className="error-message"
                    style={{
                      backgroundColor: 'rgba(239, 68, 68, 0.1)',
                      border: '1px solid rgba(239, 68, 68, 0.2)',
                      color: '#F87171',
                      padding: '16px',
                      borderRadius: '8px',
                      fontSize: '0.95rem',
                      lineHeight: '1.5',
                      margin: 0,
                    }}
                  >
                    {deleteError}
                  </div>
                </div>
                <div className="form-actions">
                  <button
                    type="button"
                    onClick={() => {
                      setEquipmentToDelete(null);
                      setDeleteError(null);
                      setDeleteModalStep('confirm');
                    }}
                    className="btn-secondary"
                    style={{ width: '100%', justifyContent: 'center' }}
                  >
                    Nazad
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </>
  );
}
