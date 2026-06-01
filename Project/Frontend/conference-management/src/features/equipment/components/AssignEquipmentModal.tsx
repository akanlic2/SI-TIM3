import { useState } from 'react';
import { assignEquipmentToSession } from '../api/equipmentApi';
import { useEquipment } from '../hooks/useEquipment';

interface AssignEquipmentModalProps {
  sessionId: string;
  onCancel: () => void;
  onSuccess: () => void;
}

export function AssignEquipmentModal({ sessionId, onCancel, onSuccess }: AssignEquipmentModalProps) {
  const { items: globalEquipment, isLoading, error: fetchError } = useEquipment();
  const [selectedEquipmentId, setSelectedEquipmentId] = useState<string>('');
  const [quantity, setQuantity] = useState<number>(1);
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});
  const [backendError, setBackendError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const selectedItem = globalEquipment.find((e) => e.equipmentId === selectedEquipmentId);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!selectedEquipmentId) {
      errors.equipmentId = 'Izbor opreme je obavezan.';
    }

    if (!quantity || quantity <= 0) {
      errors.quantity = 'Količina mora biti veća od 0.';
    } else if (selectedItem && quantity > selectedItem.availableQuantity) {
      errors.quantity = `Količina ne može biti veća od dostupne (${selectedItem.availableQuantity}).`;
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
      await assignEquipmentToSession(sessionId, {
        equipmentId: selectedEquipmentId,
        quantity,
      });
      onSuccess();
    } catch (error) {
      console.error('Greška pri dodjeli opreme:', error);
      if (error instanceof Error) {
        setBackendError(error.message);
      } else {
        setBackendError('Greška pri dodjeli opreme.');
      }
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
      <h2 className="modal-title">Dodijeli opremu sesiji</h2>

      {isLoading ? (
        <p className="text-slate-300">Učitavanje inventara opreme...</p>
      ) : fetchError ? (
        <div className="error-message">Greška pri učitavanju opreme: {fetchError}</div>
      ) : (
        <form onSubmit={handleSubmit} className="conference-form">
          <div className="form-group">
            <label className="form-label">Izaberi opremu</label>
            <select
              value={selectedEquipmentId}
              onChange={(e) => {
                setSelectedEquipmentId(e.target.value);
                setQuantity(1);
                setValidationErrors({});
              }}
              className="form-select"
              required
            >
              <option value="">-- Izaberi opremu sa stanja --</option>
              {globalEquipment
                .filter((item) => item.availableQuantity > 0)
                .map((item) => (
                  <option key={item.equipmentId} value={item.equipmentId}>
                    {item.name} ({item.type}) — Dostupno: {item.availableQuantity}
                  </option>
                ))}
            </select>
            {validationErrors.equipmentId && (
              <p className="text-red-400 text-sm mt-1">{validationErrors.equipmentId}</p>
            )}
          </div>

          {selectedItem && (
            <div className="form-group">
              <label className="form-label">Količina (maksimalno {selectedItem.availableQuantity})</label>
              <input
                type="number"
                value={quantity}
                onChange={(e) => setQuantity(Number(e.target.value))}
                className="form-input"
                min={1}
                max={selectedItem.availableQuantity}
                required
              />
              {validationErrors.quantity && (
                <p className="text-red-400 text-sm mt-1">{validationErrors.quantity}</p>
              )}
            </div>
          )}

          {backendError && <div className="error-message">{backendError}</div>}

          <div className="form-actions">
            <button type="button" onClick={onCancel} className="btn-secondary">
              Odustani
            </button>
            <button type="submit" className="btn-primary-sm" disabled={isSaving || !selectedEquipmentId}>
              Dodijeli opremu
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
