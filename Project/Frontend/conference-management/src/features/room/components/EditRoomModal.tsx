import { useEffect, useState } from 'react';
import { useAuth } from '../../../auth/AuthProvider';
import type { CreateRoomData, Room } from '../types';

interface EditRoomModalProps {
  room: Room;
  onCancel: () => void;
  onSuccess: () => void;
}

export function EditRoomModal({ room, onCancel, onSuccess }: EditRoomModalProps) {
  const { token } = useAuth();
  const [formData, setFormData] = useState<CreateRoomData>({
    name: room.name,
    location: room.location,
    capacity: room.capacity,
    description: room.description,
  });
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});
  const [backendError, setBackendError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    setFormData({
      name: room.name,
      location: room.location,
      capacity: room.capacity,
      description: room.description,
    });
    setValidationErrors({});
    setBackendError(null);
  }, [room]);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.name.trim()) {
      errors.name = 'Naziv je obavezan';
    }

    if (!formData.location.trim()) {
      errors.location = 'Lokacija je obavezna';
    }

    if (!formData.capacity || formData.capacity <= 0) {
      errors.capacity = 'Kapacitet mora biti veći od 0';
    }

    if (!formData.description.trim()) {
      errors.description = 'Opis je obavezan';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    if (!token) {
      setBackendError('Niste autorizirani za uređivanje dvorane.');
      return;
    }

    setIsSaving(true);
    setBackendError(null);

    try {
      const response = await fetch(`/api/rooms/${room.roomId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        const data = await response.json().catch(() => null);
        const backendMessage = data?.error || data?.message || `Status ${response.status}`;
        throw new Error(backendMessage);
      }

      onSuccess();
    } catch (error) {
      console.error('Greška pri uređivanju dvorane:', error);
      if (error instanceof Error) {
        setBackendError(error.message);
      } else {
        setBackendError('Greška pri uređivanju dvorane.');
      }
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
      <h2 className="modal-title">Uredi dvoranu</h2>
      <form onSubmit={handleSubmit} className="conference-form">
        <div className="form-group">
          <label className="form-label">Naziv dvorane</label>
          <input
            type="text"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            className="form-input"
            required
          />
          {validationErrors.name && <p className="text-red-400 text-sm mt-1">{validationErrors.name}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Lokacija</label>
          <input
            type="text"
            value={formData.location}
            onChange={(e) => setFormData({ ...formData, location: e.target.value })}
            className="form-input"
            required
          />
          {validationErrors.location && <p className="text-red-400 text-sm mt-1">{validationErrors.location}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Kapacitet</label>
          <input
            type="number"
            value={formData.capacity}
            onChange={(e) => setFormData({ ...formData, capacity: Number(e.target.value) })}
            className="form-input"
            min={1}
            required
          />
          {validationErrors.capacity && <p className="text-red-400 text-sm mt-1">{validationErrors.capacity}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Opis</label>
          <textarea
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            className="form-textarea"
            required
          />
          {validationErrors.description && <p className="text-red-400 text-sm mt-1">{validationErrors.description}</p>}
        </div>

        {backendError && <div className="error-message">{backendError}</div>}

        <div className="form-actions">
          <button type="button" onClick={onCancel} className="btn-secondary">
            Odustani
          </button>
          <button type="submit" className="btn-primary-sm" disabled={isSaving}>
            Sačuvaj promjene
          </button>
        </div>
      </form>
    </div>
  );
}
