import { useState } from 'react';
import type { LogisticsTask } from '../types';
import { LOGISTICS_TASK_TYPES, LOGISTICS_STATUS_OPTIONS } from '../types';
import { deleteLogisticsTask } from '../api/logisticsApi';

interface LogisticsListProps {
  items: LogisticsTask[];
  isAdminOrOrganizer: boolean;
  onDeleteSuccess: () => void;
  onEditClick: (item: LogisticsTask) => void;
}

export function LogisticsList({
  items = [],
  isAdminOrOrganizer,
  onDeleteSuccess,
  onEditClick,
}: LogisticsListProps) {
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [itemToDelete, setItemToDelete] = useState<LogisticsTask | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const [detailItem, setDetailItem] = useState<LogisticsTask | null>(null);

  const getTypeLabel = (type: string) => {
    const found = LOGISTICS_TASK_TYPES.find((t) => t.value === type);
    return found ? found.label : type;
  };

  const getStatusLabel = (status: string) => {
    const found = LOGISTICS_STATUS_OPTIONS.find((s) => s.value === status);
    return found ? found.label : status;
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return '#10b981';
      case 'InProgress': return '#f59e0b';
      case 'Cancelled': return '#ef4444';
      case 'Pending':
      default: return '#6b7280';
    }
  };

  const formatDate = (dateString: string) => {
    try {
      const date = new Date(dateString);
      if (Number.isNaN(date.getTime())) return dateString;
      return date.toLocaleDateString();
    } catch {
      return dateString;
    }
  };

  const handleDeleteClick = (item: LogisticsTask) => {
    setItemToDelete(item);
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    if (!itemToDelete) return;

    setIsDeleting(true);
    try {
      await deleteLogisticsTask(itemToDelete.logisticsTaskId);
      onDeleteSuccess();
    } catch (error) {
      console.error('Delete failed:', error);
      alert('Greška prilikom brisanja. Pokušajte ponovo.');
    } finally {
      setIsDeleting(false);
      setShowDeleteModal(false);
      setItemToDelete(null);
    }
  };

  if (!items || items.length === 0) {
    return (
      <div className="session-empty-state">
        <div className="session-empty-icon">📦</div>
        <h3 className="session-empty-title">Nema logističkih aktivnosti</h3>
        <p>Aktivnosti će se pojaviti ovdje čim budu dodane</p>
      </div>
    );
  }

  return (
    <>
      <div className="conference-table">
        <div className="table-header">
          <span>Naziv</span>
          <span>Tip</span>
          <span>Rok</span>
          <span>Status</span>
          {isAdminOrOrganizer && <span>Akcije</span>}
        </div>
        {items.map((item) => (
          <div key={item.logisticsTaskId} className="table-row">
            <span className="table-title">{item.title}</span>
            <span className="table-location">{getTypeLabel(item.taskType)}</span>
            <span className="table-date">{formatDate(item.dueDate)}</span>
            <span>
              <span
                className="conf-badge"
                style={{ backgroundColor: getStatusColor(item.status), color: '#fff' }}
              >
                {getStatusLabel(item.status)}
              </span>
            </span>
            {isAdminOrOrganizer && (
              <span style={{ display: 'flex', gap: '8px' }}>
                <button
                  type="button"
                  onClick={() => setDetailItem(item)}
                  className="btn-secondary"
                  style={{ padding: '4px 12px', fontSize: '0.8rem' }}
                  title="Pogledaj detalje"
                >
                  👁️
                </button>
                <button
                  type="button"
                  onClick={() => onEditClick(item)}
                  className="btn-secondary"
                  style={{ padding: '4px 12px', fontSize: '0.8rem' }}
                >
                  ✏️ Uredi
                </button>
                <button
                  type="button"
                  onClick={() => handleDeleteClick(item)}
                  className="btn-secondary"
                  style={{ padding: '4px 12px', fontSize: '0.8rem', color: '#ef4444' }}
                >
                  🗑️ Obriši
                </button>
              </span>
            )}
          </div>
        ))}
      </div>

      {/* Detail modal */}
      {detailItem && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h2 className="modal-title">Detalji aktivnosti</h2>
            <div style={{ margin: '16px 0', display: 'flex', flexDirection: 'column', gap: '12px' }}>
              <div>
                <span style={{ fontSize: '0.75rem', color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Naziv</span>
                <p style={{ color: '#e2e8f0', marginTop: '2px' }}>{detailItem.title}</p>
              </div>
              <div>
                <span style={{ fontSize: '0.75rem', color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Opis</span>
                <p style={{ color: '#e2e8f0', marginTop: '2px', whiteSpace: 'pre-wrap' }}>{detailItem.description || '—'}</p>
              </div>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '12px' }}>
                <div>
                  <span style={{ fontSize: '0.75rem', color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Tip</span>
                  <p style={{ color: '#e2e8f0', marginTop: '2px' }}>{getTypeLabel(detailItem.taskType)}</p>
                </div>
                <div>
                  <span style={{ fontSize: '0.75rem', color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Rok</span>
                  <p style={{ color: '#e2e8f0', marginTop: '2px' }}>{formatDate(detailItem.dueDate)}</p>
                </div>
                <div>
                  <span style={{ fontSize: '0.75rem', color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Status</span>
                  <p style={{ marginTop: '2px' }}>
                    <span className="conf-badge" style={{ backgroundColor: getStatusColor(detailItem.status), color: '#fff' }}>
                      {getStatusLabel(detailItem.status)}
                    </span>
                  </p>
                </div>
              </div>
            </div>
            <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
              <button
                type="button"
                onClick={() => setDetailItem(null)}
                className="btn-secondary"
              >
                Zatvori
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Delete confirmation modal */}
      {showDeleteModal && itemToDelete && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h2 className="modal-title">Potvrda brisanja</h2>
            <p style={{ margin: '16px 0', color: '#94a3b8' }}>
              Da li ste sigurni da želite obrisati aktivnost <strong>"{itemToDelete.title}"</strong>?
            </p>
            <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
              <button
                type="button"
                onClick={() => {
                  setShowDeleteModal(false);
                  setItemToDelete(null);
                }}
                className="btn-secondary"
                disabled={isDeleting}
              >
                Otkaži
              </button>
              <button
                type="button"
                onClick={handleDeleteConfirm}
                className="btn-primary"
                style={{ backgroundColor: '#ef4444' }}
                disabled={isDeleting}
              >
                {isDeleting ? 'Brisanje...' : 'Obriši'}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
