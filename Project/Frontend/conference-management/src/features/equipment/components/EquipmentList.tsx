import type { Equipment } from '../types';

interface EquipmentListProps {
  items: Equipment[];
  isAdminOrOrganizer: boolean;
  onAction?: (item: Equipment) => void;
  actionLabel?: string;
  isSessionView?: boolean;
}

export function EquipmentList({
  items,
  isAdminOrOrganizer,
  onAction,
  actionLabel = 'Ukloni',
  isSessionView = false,
}: EquipmentListProps) {
  if (items.length === 0) {
    return (
      <div className="p-8 text-center text-slate-400 bg-slate-900/30 rounded-lg border border-slate-800">
        Trenutno nema registrovane opreme.
      </div>
    );
  }

  return (
    <div className="overflow-x-auto rounded-lg border border-slate-800 bg-[#0f172a]/80 backdrop-blur-md">
      <table className="w-full text-left border-collapse">
        <thead>
          <tr className="border-b border-slate-800 bg-[#0f172a]">
            <th className="p-4 text-slate-300 font-semibold text-sm">Naziv opreme</th>
            <th className="p-4 text-slate-300 font-semibold text-sm">Tip</th>
            {!isSessionView && <th className="p-4 text-slate-300 font-semibold text-sm text-center">Ukupno</th>}
            <th className="p-4 text-slate-300 font-semibold text-sm text-center">
              {isSessionView ? 'Količina na sesiji' : 'Dostupno'}
            </th>
            {!isSessionView && <th className="p-4 text-slate-300 font-semibold text-sm">Status</th>}
            {isAdminOrOrganizer && onAction && (
              <th className="p-4 text-slate-300 font-semibold text-sm text-right">Akcije</th>
            )}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-800/50">
          {items.map((item) => {
            const statusClass = item.isAvailable
              ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20'
              : 'bg-rose-500/10 text-rose-400 border-rose-500/20';

            const statusText = item.isAvailable ? 'Dostupno' : 'Nedostupno';

            return (
              <tr
                key={item.equipmentId}
                className="hover:bg-slate-900/40 transition-colors"
                style={{ color: '#e2e8f0' }}
              >
                <td className="p-4 font-medium text-white">{item.name}</td>
                <td className="p-4 text-slate-300">{item.type}</td>
                {!isSessionView && <td className="p-4 text-center font-mono">{item.quantity}</td>}
                <td className="p-4 text-center font-mono text-cyan-400 font-semibold">
                  {isSessionView ? item.quantity : item.availableQuantity}
                </td>
                {!isSessionView && (
                  <td className="p-4">
                    <span
                      className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${statusClass}`}
                    >
                      {statusText}
                    </span>
                  </td>
                )}
                {isAdminOrOrganizer && onAction && (
                  <td className="p-4 text-right">
                    <button
                      onClick={() => onAction(item)}
                      className={`px-3 py-1.5 rounded-md text-xs font-semibold transition-colors ${
                        isSessionView
                          ? 'bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/20'
                          : 'bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 border border-rose-500/20'
                      }`}
                    >
                      {actionLabel}
                    </button>
                  </td>
                )}
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
