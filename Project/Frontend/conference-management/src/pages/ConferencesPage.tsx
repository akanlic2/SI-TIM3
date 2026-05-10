import { useState } from 'react';
import { ConferenceList, useConferences } from '../features/conference';
import { useAuth } from '../auth/AuthProvider';
import { createConference, updateConference } from '../features/conference/api/conferenceApi';
import type { Conference, CreateConferenceData } from '../features/conference/types';
import '../features/conference/ConferencesPage.css';

const toDatetimeLocal = (dateStr: string): string => {
  if (!dateStr) return '';
  try {
    if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}/.test(dateStr)) {
      return dateStr.slice(0, 16);
    }

    const d = new Date(dateStr);
    if (isNaN(d.getTime())) return '';

    const pad = (n: number) => String(n).padStart(2, '0');

    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(
      d.getHours()
    )}:${pad(d.getMinutes())}`;
  } catch {
    return '';
  }
};

export default function ConferencesPage() {
  const {
    items,
    isLoading: isDataLoading,
    error,
    refresh,
    page,
    setPage,
    totalPages,
    search,
    setSearch,
    location,
    setLocation,
    category,
    setCategory,
  } = useConferences();

  const { user, isLoading: isAuthLoading } = useAuth();

  const [showForm, setShowForm] = useState(false);
  const [editingConference, setEditingConference] = useState<Conference | null>(null);

  const [formData, setFormData] = useState<CreateConferenceData>({
    title: '',
    description: '',
    location: '',
    startDate: '',
    endDate: '',
    maxParticipants: 50,
    category: 'IT',
  });

  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

const role = user?.role?.toLowerCase() ?? '';
const isAdminOrOrganizer = role === 'admin-sistema' || role === 'organizator';

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.title || formData.title.trim() === '') {
      errors.title = 'Naziv je obavezan';
    } else if (formData.title.trim().length < 3) {
      errors.title = 'Naziv mora sadržati najmanje 3 karaktera';
    } else if (formData.title.length > 100) {
      errors.title = 'Naziv ne može biti duži od 100 karaktera';
    }

    if (!formData.description || formData.description.trim() === '') {
      errors.description = 'Opis je obavezan';
    } else if (formData.description.trim().length < 10) {
      errors.description = 'Opis mora sadržati najmanje 10 karaktera';
    } else if (formData.description.length > 500) {
      errors.description = 'Opis ne može biti duži od 500 karaktera';
    }

    if (!formData.location || formData.location.trim() === '') {
      errors.location = 'Lokacija je obavezna';
    }

    if (!formData.startDate) {
      errors.startDate = 'Datum početka je obavezan';
    } else {
      const startDateTime = new Date(formData.startDate);
      const now = new Date();

      if (startDateTime <= now) {
        errors.startDate = 'Datum početka mora biti u budućnosti';
      }
    }

    if (!formData.endDate) {
      errors.endDate = 'Datum završetka je obavezan';
    } else if (formData.startDate) {
      const startDateTime = new Date(formData.startDate);
      const endDateTime = new Date(formData.endDate);

      if (endDateTime <= startDateTime) {
        errors.endDate = 'Datum završetka mora biti nakon datuma početka';
      }
    }

    if (!formData.maxParticipants || formData.maxParticipants <= 0) {
      errors.maxParticipants = 'Broj učesnika mora biti veći od 0';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const resetForm = () => {
    setFormData({
      title: '',
      description: '',
      location: '',
      startDate: '',
      endDate: '',
      maxParticipants: 50,
      category: 'IT',
    });

    setEditingConference(null);
    setValidationErrors({});
  };

  const handleCreateClick = () => {
    resetForm();
    setShowForm(true);
  };

  const handleEditClick = (conference: Conference) => {
    setFormData({
      title: conference.title,
      description: conference.description,
      location: conference.location,
      startDate: toDatetimeLocal(conference.startDate),
      endDate: toDatetimeLocal(conference.endDate),
      maxParticipants: conference.maxParticipants,
      category: conference.category,
    });

    setEditingConference(conference);
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    try {
      if (editingConference) {
        await updateConference(editingConference.conferenceId, formData);
        alert('Konferencija uspješno ažurirana! 🎉');
      } else {
        await createConference(formData);
        alert('Konferencija uspješno kreirana! 🎉');
      }

      setShowForm(false);
      resetForm();
      refresh();
    } catch (err) {
      console.error('Greška pri spašavanju konferencije:', err);
      alert('Došlo je do greške prilikom spašavanja konferencije.');
    }
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
              onClick={() => window.history.back()}
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
              <h1>Konferencije</h1>
              <p>Pregledajte konferencije, pretražite ih i otvorite detalje</p>
            </div>
          </div>

          {isAdminOrOrganizer && (
            <button onClick={handleCreateClick} className="btn-primary">
              + Kreiraj konferenciju
            </button>
          )}
        </div>
      </div>

      <div className="conferences-content">
        {showForm && (
          <div className="modal-overlay" style={{ overflowY: 'auto', WebkitOverflowScrolling: 'touch' }}>
            <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
              <h2 className="modal-title">
                {editingConference ? 'Uredi konferenciju' : 'Nova Konferencija'}
              </h2>

              <form onSubmit={handleSubmit} className="conference-form">
                <div className="form-group">
                  <label className="form-label">Naziv konferencije</label>
                  <input
                    type="text"
                    placeholder="npr. Tech Spark 2026"
                    className={`form-input ${validationErrors.title ? 'border-red-500 bg-red-500/10' : ''}`}
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    required
                  />
                  {validationErrors.title && (
                    <p className="text-red-400 text-sm mt-1">{validationErrors.title}</p>
                  )}
                </div>

                <div
                  className="form-grid"
                  style={{ width: '100%', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}
                >
                  <div className="form-group" style={{ minWidth: 0 }}>
                    <label className="form-label">Datum početka</label>
                    <input
                      type="datetime-local"
                      className={`form-input ${validationErrors.startDate ? 'border-red-500 bg-red-500/10' : ''}`}
                      min={toDatetimeLocal(new Date().toISOString())}
                      value={formData.startDate}
                      onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                      onKeyDown={(e) => e.preventDefault()}
                      required
                    />
                    {validationErrors.startDate && (
                      <p className="text-red-400 text-sm mt-1">{validationErrors.startDate}</p>
                    )}
                  </div>

                  <div className="form-group" style={{ minWidth: 0 }}>
                    <label className="form-label">Datum završetka</label>
                    <input
                      type="datetime-local"
                      className={`form-input ${validationErrors.endDate ? 'border-red-500 bg-red-500/10' : ''}`}
                      min={toDatetimeLocal(new Date().toISOString())}
                      value={formData.endDate}
                      onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                      onKeyDown={(e) => e.preventDefault()}
                      required
                    />
                    {validationErrors.endDate && (
                      <p className="text-red-400 text-sm mt-1">{validationErrors.endDate}</p>
                    )}
                  </div>
                </div>

                <div className="form-group">
                  <label className="form-label">Lokacija</label>
                  <input
                    type="text"
                    placeholder="npr. Sarajevo, Hotel Europe"
                    className={`form-input ${validationErrors.location ? 'border-red-500 bg-red-500/10' : ''}`}
                    value={formData.location}
                    onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                    required
                  />
                  {validationErrors.location && (
                    <p className="text-red-400 text-sm mt-1">{validationErrors.location}</p>
                  )}
                </div>

                <div className="form-group">
                  <label className="form-label">Kategorija</label>
                  <select
                    className="form-select"
                    value={formData.category}
                    onChange={(e) => setFormData({ ...formData, category: e.target.value })}
                    required
                  >
                    <option value="IT">IT</option>
                    <option value="Business">Business</option>
                    <option value="Science">Science</option>
                    <option value="Health">Health</option>
                    <option value="Education">Education</option>
                    <option value="Other">Other</option>
                  </select>
                </div>

                <div className="form-group">
                  <label className="form-label">Maksimalan broj učesnika</label>
                  <input
                    type="number"
                    min="1"
                    max="10000"
                    className={`form-input ${validationErrors.maxParticipants ? 'border-red-500 bg-red-500/10' : ''}`}
                    value={formData.maxParticipants || ''}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        maxParticipants: e.target.value ? parseInt(e.target.value) : 0,
                      })
                    }
                    required
                  />
                  {validationErrors.maxParticipants && (
                    <p className="text-red-400 text-sm mt-1">{validationErrors.maxParticipants}</p>
                  )}
                </div>

                <div className="form-group">
                  <label className="form-label">Kratki opis</label>
                  <textarea
                    placeholder="O čemu se radi na ovoj konferenciji..."
                    className={`form-textarea ${validationErrors.description ? 'border-red-500 bg-red-500/10' : ''}`}
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  />
                  {validationErrors.description && (
                    <p className="text-red-400 text-sm mt-1">{validationErrors.description}</p>
                  )}
                </div>

                <div className="form-actions">
                  <button type="button" onClick={() => setShowForm(false)} className="btn-secondary">
                    Odustani
                  </button>
                  <button type="submit" className="btn-primary-sm">
                    {editingConference ? 'Sačuvaj promjene' : 'Sačuvaj konferenciju'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

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
            placeholder="Pretraži po nazivu ili opisu..."
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            className="form-input"
            style={{ maxWidth: '280px' }}
          />

          <input
            type="text"
            placeholder="Filtriraj po lokaciji..."
            value={location}
            onChange={(e) => {
              setLocation(e.target.value);
              setPage(1);
            }}
            className="form-input"
            style={{ maxWidth: '230px' }}
          />

          <select
            value={category}
            onChange={(e) => {
              setCategory(e.target.value);
              setPage(1);
            }}
            className="form-select"
            style={{ maxWidth: '190px' }}
          >
            <option value="">Sve kategorije</option>
            <option value="IT">IT</option>
            <option value="Business">Business</option>
            <option value="Science">Science</option>
            <option value="Health">Health</option>
            <option value="Education">Education</option>
            <option value="Other">Other</option>
          </select>
        </div>

        <div>
          {error && <div className="error-message">Greška: {error}</div>}

          {isDataLoading ? (
            <div className="loading-container">
              <div className="loading-spinner"></div>
              <p className="loading-text">Učitavanje konferencija iz baze...</p>
            </div>
          ) : (
            <>
              <ConferenceList
                conferences={items}
                isAdminOrOrganizer={isAdminOrOrganizer}
                onDeleteSuccess={refresh}
                onEditClick={handleEditClick}
              />

              <div
                style={{
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                  gap: '16px',
                  marginTop: '32px',
                }}
              >
                <button
                  disabled={page <= 1}
                  onClick={() => setPage(page - 1)}
                  className="btn-secondary"
                >
                  Prethodna
                </button>

                <span style={{ color: 'white' }}>
                  Stranica {page} od {totalPages}
                </span>

                <button
                  disabled={page >= totalPages}
                  onClick={() => setPage(page + 1)}
                  className="btn-secondary"
                >
                  Sljedeća
                </button>
              </div>
            </>
          )}
        </div>
      </div>
    </main>
  );
}
 