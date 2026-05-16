import { useState } from 'react';
import type { AgendaItem } from '../types';
import { deleteAgendaItem } from '../api/agendaApi';

interface AgendaListProps {
  items: AgendaItem[];
  conferenceId: string;
  isAdminOrOrganizer: boolean;
  onDeleteSuccess: () => void;
  onEditClick: (item: AgendaItem) => void;
}

export function AgendaList({
  items,
  isAdminOrOrganizer,
  onDeleteSuccess,
  onEditClick,
}: AgendaListProps) {
  const [deletingId, setDeletingId] = useState<string | null>(null);

  if (items.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">📅</div>
        <p>Agenda još uvijek nije definisana</p>
      </div>
    );
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm('Da li ste sigurni da želite obrisati ovu stavku agende?')) {
      return;
    }

    setDeletingId(id);
    try {
      await deleteAgendaItem(id);
      onDeleteSuccess();
    } catch (err) {
      console.error('Greška pri brisanju stavke agende:', err);
      alert('Došlo je do greške pri brisanju.');
    } finally {
      setDeletingId(null);
    }
  };

  const getFormatTime = (isoString: string) => {
    return new Date(isoString).toLocaleTimeString('bs-BA', {
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getFormatDate = (isoString: string) => {
    return new Date(isoString).toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  // Group items by date
  const groupedItems = items.reduce((acc, item) => {
    const dateStr = getFormatDate(item.startTime);
    if (!acc[dateStr]) {
      acc[dateStr] = [];
    }
    acc[dateStr].push(item);
    return acc;
  }, {} as Record<string, AgendaItem[]>);

  const getTypeLabel = (type: string) => {
    switch (type) {
      case 'Session': return 'Sesija';
      case 'Break': return 'Pauza';
      case 'Lunch': return 'Ručak';
      case 'Networking': return 'Networking';
      case 'Opening': return 'Otvaranje';
      case 'Closing': return 'Zatvaranje';
      default: return type;
    }
  };

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'Session': return 'bg-blue-900/40 text-blue-300 border-blue-800/50';
      case 'Break': return 'bg-orange-900/40 text-orange-300 border-orange-800/50';
      case 'Lunch': return 'bg-green-900/40 text-green-300 border-green-800/50';
      case 'Networking': return 'bg-purple-900/40 text-purple-300 border-purple-800/50';
      case 'Opening':
      case 'Closing': return 'bg-indigo-900/40 text-indigo-300 border-indigo-800/50';
      default: return 'bg-slate-800 text-slate-300 border-slate-700';
    }
  };

  return (
    <div className="space-y-8">
      {Object.entries(groupedItems).map(([date, dayItems]) => (
        <div key={date} className="section-block">
          <div className="section-header border-b border-slate-800/50 pb-4 mb-4">
            <h2 className="text-xl font-semibold text-white flex items-center gap-2">
              <span>📅</span> {date}
            </h2>
          </div>

          <div className="space-y-4">
            {dayItems.map((item) => (
              <div
                key={item.agendaItemId}
                className="bg-slate-900/40 border border-slate-800/50 rounded-xl p-5 hover:border-slate-700/50 transition-colors"
              >
                <div className="flex justify-between items-start">
                  <div className="flex gap-4">
                    {/* Time Column */}
                    <div className="flex flex-col text-slate-400 font-mono text-sm min-w-[80px]">
                      <span className="text-white font-medium">{getFormatTime(item.startTime)}</span>
                      <span className="text-slate-500">{getFormatTime(item.endTime)}</span>
                    </div>

                    {/* Content Column */}
                    <div>
                      <div className="flex items-center gap-3 mb-1">
                        <h3 className="text-lg font-medium text-white">{item.title}</h3>
                        <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium border ${getTypeColor(item.type)}`}>
                          {getTypeLabel(item.type)}
                        </span>
                      </div>
                      
                      {item.description && (
                        <p className="text-slate-400 text-sm mt-2">{item.description}</p>
                      )}

                      <div className="flex gap-4 mt-3 text-sm text-slate-500">
                        {item.roomName && (
                          <span className="flex items-center gap-1">
                            📍 {item.roomName}
                          </span>
                        )}
                        {item.type === 'Session' && item.speakerName && (
                          <span className="flex items-center gap-1">
                            👤 Predavač: {item.speakerName}
                          </span>
                        )}
                      </div>
                    </div>
                  </div>

                  {/* Actions Column */}
                  {isAdminOrOrganizer && (
                    <div className="flex gap-2">
                      <button
                        onClick={() => onEditClick(item)}
                        className="btn-secondary px-3 py-1 text-sm"
                        disabled={deletingId === item.agendaItemId}
                      >
                        Uredi
                      </button>
                      <button
                        onClick={() => handleDelete(item.agendaItemId)}
                        className="btn-secondary px-3 py-1 text-sm border-red-900/30 text-red-400 hover:bg-red-900/20"
                        disabled={deletingId === item.agendaItemId}
                      >
                        {deletingId === item.agendaItemId ? '...' : 'Obriši'}
                      </button>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}
