import { useState } from 'react';
import { useAuth } from '../../../auth/AuthProvider';
import { uploadSessionMaterial } from '../api/sessionApi';

interface UploadMaterialModalProps {
  sessionId: string;
  onCancel: () => void;
  onSuccess: () => void;
}

export function UploadMaterialModal({ sessionId, onCancel, onSuccess }: UploadMaterialModalProps) {
  const { token } = useAuth();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [file, setFile] = useState<File | null>(null);
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});
  const [backendError, setBackendError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!title.trim()) {
      errors.title = 'Naziv je obavezan';
    }

    if (!file) {
      errors.file = 'Fajl je obavezan';
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
      setBackendError('Niste autorizirani za upload materijala.');
      return;
    }

    if (!file) {
      setValidationErrors({ ...validationErrors, file: 'Fajl je obavezan' });
      return;
    }

    setIsSaving(true);
    setBackendError(null);

    try {
      await uploadSessionMaterial(sessionId, title.trim(), description.trim(), file, token);
      setTitle('');
      setDescription('');
      setFile(null);
      setValidationErrors({});
      onSuccess();
    } catch (error) {
      console.error('Greška pri uploadu materijala:', error);
      if (error instanceof Error) {
        setBackendError(error.message);
      } else {
        setBackendError('Greška pri uploadu materijala.');
      }
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="modal-content" style={{ maxHeight: '90vh', overflowY: 'auto' }}>
      <h2 className="modal-title">Upload Materijala</h2>
      <form onSubmit={handleSubmit} className="conference-form">
        <div className="form-group">
          <label className="form-label">Naziv</label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="form-input"
            required
          />
          {validationErrors.title && <p className="text-red-400 text-sm mt-1">{validationErrors.title}</p>}
        </div>

        <div className="form-group">
          <label className="form-label">Opis</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="form-textarea"
          />
        </div>

        <div className="form-group">
          <label className="form-label">Odabir fajla</label>
          <input
            type="file"
            accept=".pdf,.ppt,.pptx"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
            className="form-input"
            required
          />
          {validationErrors.file && <p className="text-red-400 text-sm mt-1">{validationErrors.file}</p>}
        </div>

        {backendError && <div className="error-message">{backendError}</div>}

        <div className="form-actions">
          <button type="button" onClick={onCancel} className="btn-secondary">
            Odustani
          </button>
          <button type="submit" className="btn-primary-sm" disabled={isSaving}>
            Upload
          </button>
        </div>
      </form>
    </div>
  );
}
