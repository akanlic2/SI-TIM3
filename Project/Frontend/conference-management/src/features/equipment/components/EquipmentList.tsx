import type { Equipment } from '../types';

interface EquipmentListProps {
  items: Equipment[];
  isAdminOrOrganizer: boolean;
  onAction?: (item: Equipment) => void;
  onReduceTotal?: (item: Equipment) => void;
  reducingEquipmentId?: string | null;
  actionLabel?: string;
  isSessionView?: boolean;
}

export function EquipmentList({
  items,
  isAdminOrOrganizer,
  onAction,
  onReduceTotal,
  reducingEquipmentId = null,
  actionLabel = 'Ukloni',
  isSessionView = false,
}: EquipmentListProps) {
  const showAction = Boolean(isAdminOrOrganizer && onAction);
  const gridTemplateColumns = isSessionView
    ? (showAction ? '2fr 1.4fr 1fr 0.8fr' : '2fr 1.4fr 1fr')
    : (showAction ? '2fr 1.4fr 0.9fr 0.9fr 1fr 0.8fr' : '2fr 1.4fr 0.9fr 0.9fr 1fr');

  if (items.length === 0) {
    return (
      <div className="section-block">
        <div className="empty-state">
          <div className="empty-icon">🧰</div>
          <p>Trenutno nema registrovane opreme.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="section-block">
      <div className="conference-table">
        <div className="table-header" style={{ gridTemplateColumns }}>
          <span className="btn-secondary" style={{ textAlign: 'center', display: 'inline-block', width: '100%' }}>
            Naziv opreme
          </span>
          <span className="btn-secondary" style={{ textAlign: 'center', display: 'inline-block', width: '100%' }}>
            Tip
          </span>
          {!isSessionView && (
            <span className="btn-secondary" style={{ textAlign: 'center', display: 'inline-block', width: '100%' }}>
              Ukupno
            </span>
          )}
          <span className="btn-secondary" style={{ textAlign: 'center', display: 'inline-block', width: '100%' }}>
            {isSessionView ? 'Količina na sesiji' : 'Dostupno'}
          </span>
          {!isSessionView && (
            <span className="btn-secondary" style={{ textAlign: 'center', display: 'inline-block', width: '100%' }}>
              Status
            </span>
          )}
          {showAction && (
            <span className="btn-secondary" style={{ textAlign: 'center', display: 'inline-block', width: '100%' }}>
              Akcije
            </span>
          )}
        </div>
        {items.map((item) => {
          const statusText = item.isAvailable ? 'Dostupno' : 'Nedostupno';
          const statusStyle = item.isAvailable
            ? undefined
            : {
                background: 'rgba(248, 113, 113, 0.12)',
                color: '#fca5a5',
                border: '1px solid rgba(248, 113, 113, 0.25)',
              };

          return (
            <div key={item.equipmentId} className="table-row" style={{ gridTemplateColumns, textAlign: 'center' }}>
              <span className="table-title">{item.name}</span>
              <span className="table-location">{item.type}</span>
              {!isSessionView && (
                <span className="table-date" style={{ textAlign: 'center' }}>
                  <span style={{ display: 'inline-flex', alignItems: 'center', gap: '8px' }}>
                    <span>{item.quantity}</span>
                    {onReduceTotal && (
                      <button
                        type="button"
                        onClick={() => onReduceTotal(item)}
                        className="btn-secondary"
                        style={{
                          padding: '3px 7px',
                          minWidth: '24px',
                          lineHeight: 1,
                        }}
                        disabled={item.availableQuantity <= 0 || reducingEquipmentId === item.equipmentId}
                        aria-label={`Smanji ukupnu kolicinu opreme ${item.name}`}
                        title={item.availableQuantity <= 0 ? 'Nema dostupne opreme' : 'Smanji ukupnu kolicinu'}
                      >
                        -
                      </button>
                    )}
                  </span>
                </span>
              )}
              <span className="table-date" style={{ textAlign: 'center', color: 'cyan' }}>
                {isSessionView ? item.quantity : item.availableQuantity}
              </span>
              {!isSessionView && (
                <span>
                  <span className="conf-badge" style={statusStyle}>{statusText}</span>
                </span>
              )}
              {showAction && (
                <span style={{ textAlign: 'center' }}>
                  <button
                    type="button"
                    onClick={() => onAction?.(item)}
                    onMouseEnter={(event) => {
                      event.currentTarget.style.backgroundColor = '#DC2626';
                    }}
                    onMouseLeave={(event) => {
                      event.currentTarget.style.backgroundColor = '#EF4444';
                    }}
                    className="btn-delete"
                    style={{
                      backgroundColor: '#EF4444',
                      color: '#fff',
                      borderRadius: 'var(--radius-md)',
                      padding: '6px 15px',
                      border: 'none',
                      cursor: 'pointer',
                      transition: 'background-color 0.2s ease',
                    }}
                  >
                    {actionLabel}
                  </button>
                </span>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
