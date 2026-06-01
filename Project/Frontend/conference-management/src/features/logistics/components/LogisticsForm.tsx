import { useState } from 'react';
import { createLogisticsTask, updateLogisticsTask } from '../api/logisticsApi';
import { LOGISTICS_TASK_TYPES, LOGISTICS_STATUS_OPTIONS } from '../types';
import type { LogisticsTask } from '../types';

interface LogisticsFormProps {
  conferenceId: string;
  editingItem: LogisticsTask | null;
  onSuccess: () => void;
  onCancel: () => void;
}

export function LogisticsForm({ conferenceId, editingItem, onSuccess, onCancel }: LogisticsFormProps) {
  const pad = (v: number) => String(v).padStart(2, '0');

  const toDateInputValue = (dateString: string) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (Number.isNaN(date.getTime())) return '';
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
  };

  const todayString = (() => {
    const now = new Date();
    return `${now.getFullYear()}-${pad(now.getMonth() + 1)}-${pad(now.getDate())}`;
  })();

  const [title, setTitle] = useState(editingItem?.title ?? '');
  const [description, setDescription] = useState(editingItem?.description ?? '');
  const [taskType, setTaskType] = useState(editingItem?.taskType ?? LOGISTICS_TASK_TYPES[0].value);
  const [dueDate, setDueDate] = useState(editingItem ? toDateInputValue(editingItem.dueDate) : '');
  const [status, setStatus] = useState(editingItem?.status ?? LOGISTICS_STATUS_OPTIONS[0].value);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!title.trim()) {
      setError('Naslov je obavezan.');
      return;
    }

    if (!description.trim()) {
      setError('Opis je obavezan.');
      return;
    }

    if (!taskType) {
      setError('Tip aktivnosti je obavezan.');
      return;
    }

    if (!dueDate) {
      setError('Rok izvršenja je obavezan.');
      return;
    }

    if (dueDate < todayString) {
      setError('Rok izvršenja ne može biti u prošlosti.');
      return;
    }

    if (!status) {
      setError('Status je obavezan.');
      return;
    }

    setIsSubmitting(true);

    try {
      const data = {
        title: title.trim(),
        description: description.trim(),
        taskType,
        dueDate: new Date(dueDate).toISOString(),
        status,
      };

      if (editingItem) {
        await updateLogisticsTask(editingItem.logisticsTaskId, data);
      } else {
        await createLogisticsTask(conferenceId, data);
      }

      onSuccess();
    } catch (err: any) {
      console.error('Greška pri spašavanju:', err);
      const message = err.response?.data?.error || err.response?.data?.message || 'Došlo je do greške prilikom spašavanja.';
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <div className="error-message" style={{ marginBottom: '16px' }}>{error}</div>}

      <div className="form-group">
        <label>Naslov <span style={{ color: '#ef4444' }}>*</span></label>
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="form-input"
          placeholder="Unesite naslov aktivnosti"
          maxLength={150}
          required
        />
      </div>

      <div className="form-group">
        <label>Opis <span style={{ color: '#ef4444' }}>*</span></label>
        <textarea
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="form-input"
          placeholder="Unesite opis aktivnosti"
          rows={3}
          required
          style={{ resize: 'vertical', minHeight: '80px' }}
        />
      </div>

      <div className="form-group">
        <label>Tip aktivnosti <span style={{ color: '#ef4444' }}>*</span></label>
        <select
          value={taskType}
          onChange={(e) => setTaskType(e.target.value)}
          className="form-input"
          required
        >
          {LOGISTICS_TASK_TYPES.map((t) => (
            <option key={t.value} value={t.value}>
              {t.label}
            </option>
          ))}
        </select>
      </div>

      <div className="form-group">
        <label>Rok izvršenja <span style={{ color: '#ef4444' }}>*</span></label>
        <input
          type="date"
          value={dueDate}
          onChange={(e) => setDueDate(e.target.value)}
          className="form-input"
          min={todayString}
          required
        />
      </div>

      <div className="form-group">
        <label>Status <span style={{ color: '#ef4444' }}>*</span></label>
        <select
          value={status}
          onChange={(e) => setStatus(e.target.value)}
          className="form-input"
          required
        >
          {LOGISTICS_STATUS_OPTIONS.map((s) => (
            <option key={s.value} value={s.value}>
              {s.label}
            </option>
          ))}
        </select>
      </div>

      <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end', paddingTop: '16px' }}>
        <button
          type="button"
          onClick={onCancel}
          className="btn-secondary"
          disabled={isSubmitting}
        >
          Otkaži
        </button>
        <button
          type="submit"
          className="btn-primary"
          disabled={isSubmitting}
        >
          {isSubmitting ? 'Spašavanje...' : editingItem ? 'Sačuvaj izmjene' : 'Kreiraj aktivnost'}
        </button>
      </div>
    </form>
  );
}
