import { useState } from 'react';
import { createEquipment } from '../api/equipmentApi';
import type { CreateEquipmentData } from '../types';

interface CreateEquipmentModalProps {
  onCancel: () => void;
  onSuccess: () => void;
}

export function CreateEquipmentModal({ onCancel, onSuccess }: CreateEquipmentModalProps) {
  const [formData, setFormData] = useState<CreateEquipmentData>({
    name: '',
    type: '',
    quantity: 1,
  });
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});
  const [backendError, setBackendError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.name.trim()) {
      errors.name = 'Naziv opreme je obavezan.';
    }

    if (!formData.type.trim()) {
      errors.type = 'Tip opreme je obavezan.';
    }

    if (!formData.quantity || formData.quantity <= 0) {
      errors.quantity = 'Količina mora biti veća od 0.';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) return;

    setIsSaving(true);
    setBackendError(null);

    try {
      await createEquipment(formData);
      onSuccess();
    } catch (error) {
      console.error('Greška pri kreiranju opreme:', error);
      if (error instanceof Error) {
        setBackendError(error.message);
      } else {
        setBackendError('Greška pri kreiranju opreme.');
      }
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
      <h2 className="modal-title">Nova oprema</h2>
      <form onSubmit={handleSubmit} className="conference-form">
        <div className="form-group">
          <label className="form-label">Naziv opreme</label>
          <input
            type="text"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            className="form-input"
            placeholder="npr. Projektor Epson X41"
            required
          />
          {validationErrors.name && (
            <p className="text-red-400 text-sm mt-1">{validationErrors.name}</p>
          )}
        </div>

        <div className="form-group">
          <label className="form-label">Tip opreme</label>
          <input
            type="text"
            value={formData.type}
            onChange={(e) => setFormData({ ...formData, type: e.target.value })}
            className="form-input"
            placeholder="npr. Audio-Vizuelna"
            required
          />
          {validationErrors.type && (
            <p className="text-red-400 text-sm mt-1">{validationErrors.type}</p>
          )}
        </div>

        <div className="form-group">
          <label className="form-label">Ukupna količina</label>
          <input
            type="number"
            value={formData.quantity}
            onChange={(e) => setFormData({ ...formData, quantity: Number(e.target.value) })}
            className="form-input"
            min={1}
            required
          />
          {validationErrors.quantity && (
            <p className="text-red-400 text-sm mt-1">{validationErrors.quantity}</p>
          )}
        </div>

        {backendError && <div className="error-message">{backendError}</div>}

        <div className="form-actions">
          <button type="button" onClick={onCancel} className="btn-secondary">
            Odustani
          </button>
          <button type="submit" className="btn-primary-sm" disabled={isSaving}>
            Sačuvaj opremu
          </button>
        </div>
      </form>
    </div>
  );
}
