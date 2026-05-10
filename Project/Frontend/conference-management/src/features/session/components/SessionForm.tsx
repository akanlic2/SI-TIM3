import { useState, useEffect } from 'react';
import { createSession, updateSession, assignSpeaker } from '../api/sessionApi';
import { useUsers } from '../hooks/useUsers';
import type { Session, CreateSessionData, UpdateSessionData, AssignSpeakerData } from '../types';

interface SessionFormProps {
  conferenceId: string;
  editingSession: Session | null;
  onSuccess: () => void;
  onCancel: () => void;
}
function toDatetimeLocal(isoString: string): string {
  if (!isoString) return '';
  return new Date(isoString).toISOString().slice(0, 16);
}


export function SessionForm({ conferenceId, editingSession, onSuccess, onCancel }: SessionFormProps) {
  const { items: users } = useUsers();

  const speakers = users.filter(user => user.role.toLowerCase() === 'predavac');

  const [formData, setFormData] = useState<CreateSessionData>({
    title: '',
    description: '',
    startTime: '',
    endTime: '',
    conferenceId,
    roomId: '', // TODO: handle rooms
    sessionType: 'Lecture',
  });

  const [assignedSpeakerId, setAssignedSpeakerId] = useState<string>('');
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

 useEffect(() => {
  if (editingSession) {
    setFormData({
      title: editingSession.title,
      description: editingSession.description || '',
      startTime: toDatetimeLocal(editingSession.startTime),  // ← promjena
      endTime: toDatetimeLocal(editingSession.endTime),      // ← promjena
      conferenceId,
      roomId: '',
      sessionType: editingSession.sessionType,
    });
    } else {
      setFormData({
        title: '',
        description: '',
        startTime: '',
        endTime: '',
        conferenceId,
        roomId: '',
        sessionType: 'Lecture',
      });
      setAssignedSpeakerId('');
    }
    setValidationErrors({});
  }, [editingSession, conferenceId]);

  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

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

    if (!formData.startTime) {
      errors.startTime = 'Vrijeme početka je obavezno';
    }

    if (!formData.endTime) {
      errors.endTime = 'Vrijeme završetka je obavezno';
    } else if (formData.startTime && new Date(formData.endTime) <= new Date(formData.startTime)) {
      errors.endTime = 'Vrijeme završetka mora biti nakon vremena početka';
    }

    if (!formData.roomId) {
      errors.roomId = 'Sala je obavezna';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setSubmitError(null);
    setIsSubmitting(true);

    try {
      let savedSessionId: string | null = null;

      if (editingSession) {
        const updateData = {
          title: formData.title,
          description: formData.description,
          startTime: formData.startTime,
          endTime: formData.endTime,
          roomId: formData.roomId,
          sessionType: formData.sessionType,
        };

        await updateSession(editingSession.sessionId, updateData);
        savedSessionId = editingSession.sessionId;
      } else {
        const createdSession = await createSession(formData);
        if (!createdSession) {
          throw new Error('Neuspjelo kreiranje sesije');
        }
        savedSessionId = createdSession.sessionId;
      }

      if (assignedSpeakerId && savedSessionId) {
        await assignSpeaker(savedSessionId, { userId: assignedSpeakerId });
      }

      onSuccess();
    } catch (error) {
      console.error('Greška pri spremanju sesije:', error);
      setSubmitError('Greška pri spremanju sesije. Pokušajte ponovno.');
    } finally {
      setIsSubmitting(false);
    }
  };

  const selectedSpeaker = speakers.find(s => s.userId === assignedSpeakerId);

  return (
    <form onSubmit={handleSubmit} className="session-form">
      <div className="form-group">
        <label className="form-label">Naziv sesije</label>
        <input
          type="text"
          placeholder="npr. Uvod u React"
          className={`form-input ${validationErrors.title ? 'border-red-500 bg-red-500/10' : ''}`}
          value={formData.title}
          onChange={(e) => setFormData({ ...formData, title: e.target.value })}
          required
        />
        {validationErrors.title && (
          <p className="text-red-400 text-sm mt-1">{validationErrors.title}</p>
        )}
      </div>

      <div className="form-group">
        <label className="form-label">Opis</label>
        <textarea
          placeholder="O čemu se radi na ovoj sesiji..."
          className={`form-textarea ${validationErrors.description ? 'border-red-500 bg-red-500/10' : ''}`}
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
        />
        {validationErrors.description && (
          <p className="text-red-400 text-sm mt-1">{validationErrors.description}</p>
        )}
      </div>

      <div className="form-grid" style={{ width: '100%', display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
        <div className="form-group" style={{ minWidth: 0 }}>
          <label className="form-label">Vrijeme početka</label>
          <input
            type="datetime-local"
            className={`form-input ${validationErrors.startTime ? 'border-red-500 bg-red-500/10' : ''}`}
            value={formData.startTime}
            onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
            onKeyDown={(e) => e.preventDefault()}
            required
          />
          {validationErrors.startTime && (
            <p className="text-red-400 text-sm mt-1">{validationErrors.startTime}</p>
          )}
        </div>

        <div className="form-group" style={{ minWidth: 0 }}>
          <label className="form-label">Vrijeme završetka</label>
          <input
            type="datetime-local"
            className={`form-input ${validationErrors.endTime ? 'border-red-500 bg-red-500/10' : ''}`}
            value={formData.endTime}
            onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
            onKeyDown={(e) => e.preventDefault()}
            required
          />
          {validationErrors.endTime && (
            <p className="text-red-400 text-sm mt-1">{validationErrors.endTime}</p>
          )}
        </div>
      </div>

      <div className="form-group">
        <label className="form-label">Tip sesije</label>
        <select
          className="form-select"
          value={formData.sessionType}
          onChange={(e) => setFormData({ ...formData, sessionType: e.target.value })}
          required
        >
          <option value="Lecture">Predavanje</option>
          <option value="Workshop">Radionica</option>
          <option value="Panel">Panel</option>
          <option value="Other">Ostalo</option>
        </select>
      </div>

      <div className="form-group">
        <label className="form-label">Sala</label>
        <select
          className={`form-select ${validationErrors.roomId ? 'border-red-500 bg-red-500/10' : ''}`}
          value={formData.roomId}
          onChange={(e) => setFormData({ ...formData, roomId: e.target.value })}
          required
        >
          <option value="">Odaberite salu</option>
          {/* Seeded room GUIDs from backend */}
          <option value="11111111-1111-1111-1111-111111111111">Amfiteatar 1</option>
          <option value="22222222-2222-2222-2222-222222222222">Sala 203 (Lab)</option>
          <option value="33333333-3333-3333-3333-333333333333">Konferencijska Sala A</option>
        </select>
        {validationErrors.roomId && (
          <p className="text-red-400 text-sm mt-1">{validationErrors.roomId}</p>
        )}
      </div>

      <div className="form-group">
        <label className="form-label">Predavač</label>
        <select
          className="form-select"
          value={assignedSpeakerId}
          onChange={(e) => setAssignedSpeakerId(e.target.value)}
        >
          <option value="">Odaberite predavača (opciono)</option>
          {speakers.map((speaker) => (
            <option key={speaker.userId} value={speaker.userId}>
              {speaker.firstName} {speaker.lastName}
            </option>
          ))}
        </select>
        {selectedSpeaker && (
          <div style={{ marginTop: '8px', padding: '8px', backgroundColor: 'rgba(37, 99, 235, 0.1)', borderRadius: '4px' }}>
            <strong>Dodijeljen predavač:</strong> {selectedSpeaker.firstName} {selectedSpeaker.lastName}
          </div>
        )}
      </div>

      {submitError && (
        <div className="error-message" style={{ marginBottom: '16px' }}>
          {submitError}
        </div>
      )}

      <div className="form-actions">
        <button type="button" onClick={onCancel} className="btn-secondary">
          Odustani
        </button>
        <button type="submit" className="btn-primary-sm" disabled={isSubmitting}>
          {isSubmitting ? 'Spremanje...' : editingSession ? 'Sačuvaj promjene' : 'Sačuvaj sesiju'}
        </button>
      </div>
    </form>
  );
}