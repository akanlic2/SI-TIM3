import { useEffect, useState } from 'react';
import { useAuth } from '../auth/AuthProvider';
import { AddRoomModal } from '../features/room/components/AddRoomModal';
import { EditRoomModal } from '../features/room/components/EditRoomModal';
import { useRooms } from '../features/room/hooks/useRooms';
import type { Room } from '../features/room/types';
import '../features/conference/ConferencesPage.css';
import './RoomsPage.css';

export default function RoomsPage() {
  const { user, token, isLoading: isAuthLoading } = useAuth();
  const role = user?.role?.toLowerCase() ?? '';
  const isAdminOrOrganizer = role === 'admin-sistema' || role === 'organizator';
  const [redirecting, setRedirecting] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editingRoom, setEditingRoom] = useState<Room | null>(null);
  const [roomToDelete, setRoomToDelete] = useState<Room | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteModalStep, setDeleteModalStep] = useState<'confirm' | 'error'>('confirm');

  const { items: rooms, isLoading: isDataLoading, error, refresh } = useRooms();

  const handleDeleteConfirm = async () => {
    if (!roomToDelete) return;
    if (!token) {
      setDeleteError('Niste autorizirani za brisanje dvorane.');
      setDeleteModalStep('error');
      return;
    }

    setIsDeleting(true);
    setDeleteError(null);

    try {
      const response = await fetch(`/api/rooms/${roomToDelete.roomId}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        const data = await response.json().catch(() => null);
        const backendMessage = data?.error || data?.message || `Status ${response.status}`;
        throw new Error(backendMessage);
      }

      setRoomToDelete(null);
      refresh();
    } catch (error) {
      console.error('Greška pri brisanju dvorane:', error);
      setDeleteError(error instanceof Error ? error.message : 'Greška pri brisanju dvorane.');
      setDeleteModalStep('error');
    } finally {
      setIsDeleting(false);
    }
  };

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
                <h1>Dvorane</h1>
                <p>Dodaj, uredi ili obriši dvoranu</p>
              </div>
            </div>

            {isAdminOrOrganizer && (
              <button
                type="button"
                onClick={() => setShowForm(true)}
                className="btn-primary"
              >
                + Dodaj dvoranu
              </button>
            )}
          </div>
        </div>

        <div className="rooms-content">
          {error && <div className="error-message">{error}</div>}

          {isDataLoading ? (
            <div className="loading-container">
              <div className="loading-spinner" />
              <p className="loading-text">Učitavanje dvorana iz baze...</p>
            </div>
          ) : rooms.length === 0 ? (
            <div className="rooms-empty-state">
              <p>Nema dvorana</p>
            </div>
          ) : (
            <div className="session-grid">
              {rooms.map((room) => (
                <div key={room.roomId} className="session-card">
                  <div className="session-card-header">
                    <h2 className="session-card-title">{room.name}</h2>
                  </div>

                  <div className="session-card-info">
                    <div className="session-info-row">
                      <span className="session-info-icon">📍</span>
                      <div className="session-info-content">
                        <span className="session-info-label">LOKACIJA</span>
                        <span className="session-info-value">{room.location}</span>
                      </div>
                    </div>
                    <div className="session-info-row">
                      <span className="session-info-icon">👥</span>
                      <div className="session-info-content">
                        <span className="session-info-label">KAPACITET</span>
                        <span className="session-info-value">{room.capacity}</span>
                      </div>
                    </div>
                    <div className="session-info-row">
                      <span className="session-info-icon">📝</span>
                      <div className="session-info-content">
                        <span className="session-info-label">OPIS</span>
                        <span className="session-info-value">
                          {room.description || 'Nema opisa'}
                        </span>
                      </div>
                    </div>
                  </div>

                  <div
                    className="session-card-actions"
                    style={{
                      display: 'flex',
                      justifyContent: 'flex-end',
                      gap: '12px',
                      flexWrap: 'wrap',
                    }}
                  >
                    <button
                      type="button"
                      style={{
                        backgroundColor: '#EAB308',
                        color: '#000',
                        borderRadius: '9999px',
                        padding: '8px 20px',
                        border: 'none',
                        cursor: 'pointer',
                      }}
                      onClick={() => setEditingRoom(room)}
                    >
                      Uredi
                    </button>
                    <button
                      type="button"
                      style={{
                        backgroundColor: '#EF4444',
                        color: '#fff',
                        borderRadius: '9999px',
                        padding: '8px 20px',
                        border: 'none',
                        cursor: 'pointer',
                      }}
                      onClick={() => {
                        setRoomToDelete(room);
                        setDeleteError(null);
                        setDeleteModalStep('confirm');
                      }}
                    >
                      Obriši
                    </button>
                  </div>
                </div>
              ))}
            </div>
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
          <AddRoomModal
            onCancel={() => setShowForm(false)}
            onSuccess={() => {
              setShowForm(false);
              refresh();
            }}
          />
        </div>
      )}

      {editingRoom && (
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
          <EditRoomModal
            key={editingRoom.roomId} /* DODANO: jedinstveni ključ */
            room={editingRoom}
            onCancel={() => setEditingRoom(null)}
            onSuccess={() => {
              setEditingRoom(null);
              refresh();
            }}
          />
        </div>
      )}

      {roomToDelete && (
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
                  Želite li zaista obrisati dvoranu <strong>{roomToDelete.name}</strong>?
                </p>
                <div className="form-actions">
                  <button
                    type="button"
                    onClick={() => {
                      setRoomToDelete(null);
                      setDeleteError(null);
                      setDeleteModalStep('confirm');
                    }}
                    className="btn-secondary"
                  >
                    Ne
                  </button>
                  <button
                    type="button"
                    onClick={async () => {
                      await handleDeleteConfirm();
                    }}
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
                  {/* POBOLJŠANO: ljepši boks za grešku sa unutrašnjim paddingom */}
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
                      margin: 0
                    }}
                  >
                    {deleteError}
                  </div>
                </div>
                <div className="form-actions">
                  <button
                    type="button"
                    onClick={() => {
                      setRoomToDelete(null);
                      setDeleteError(null);
                      setDeleteModalStep('confirm');
                    }}
                    className="btn-secondary"
                    style={{ width: '100%', justifyContent: 'center' }} /* Široko dugme za bolji izgled ekrana greške */
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