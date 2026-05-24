import { useState, useEffect } from 'react';
import { useSessions } from '../../session';
import { fetchRooms } from '../../room/api/roomApi';
import type { Room } from '../../room/types';
import { createAgendaItem, updateAgendaItem } from '../api/agendaApi';
import { AGENDA_ITEM_TYPES, type AgendaItem } from '../types';
import 'react-datepicker/dist/react-datepicker.css';
import DatePicker from 'react-datepicker';

interface AgendaFormProps {
  conferenceId: string;
  editingItem: AgendaItem | null;
  onSuccess: () => void;
  onCancel: () => void;
}

export function AgendaForm({ conferenceId, editingItem, onSuccess, onCancel }: AgendaFormProps) {
  const DatePickerComponent = DatePicker as any;

  const toDatetimeLocalValue = (dateString: string) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (Number.isNaN(date.getTime())) return '';

    const pad = (v: number) => String(v).padStart(2, '0');
    // Local getters — input shows user's local time
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  };

  const fromDate = (date: Date) => {
    const pad = (v: number) => String(v).padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  };

  const [type, setType] = useState(editingItem?.type ?? 'Session');
  const [startTime, setStartTime] = useState(editingItem ? toDatetimeLocalValue(editingItem.startTime) : '');
  const [endTime, setEndTime] = useState(editingItem ? toDatetimeLocalValue(editingItem.endTime) : '');

  const [sessionId, setSessionId] = useState(editingItem?.sessionId ?? '');
  const [title, setTitle] = useState(editingItem?.title ?? '');
  const [description, setDescription] = useState(editingItem?.description ?? '');
  const [roomId, setRoomId] = useState(editingItem?.roomId ?? '');

  const [rooms, setRooms] = useState<Room[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const isSessionType = type.toLowerCase() === 'session';

  // Dohvatimo sesije iz conference-a da bi ponudili u dropdown-u (Tim C/Tim A hibrid)
  const { items: sessions, isLoading: sessionsLoading } = useSessions(conferenceId);

  // In handleSubmit, before building `data`:
  const toUTCIso = (localDatetime: string) => new Date(localDatetime).toISOString();

  useEffect(() => {
    fetchRooms()
      .then(setRooms)
      .catch((err) => {
        console.error('Error fetching rooms:', err);
        // Fallback ako rooms endpoint ne radi (nije naš modul)
      });
  }, []);

  // Kad se promijeni tip stavke, resetujemo specifična polja
  useEffect(() => {
    if (!editingItem) {
      if (isSessionType) {
        setTitle('');
        setDescription('');
        setStartTime('');  // add this
        setEndTime('');    // add this
        setSessionId('');  // reset session too
        setRoomId('');     // reset room too
      } else {
        setSessionId('');
        setStartTime('');
        setEndTime('');
      }
    }
  }, [type, editingItem]);

  useEffect(() => {
    if (!isSessionType || !sessionId) return;

    const selected = sessions.find((s) => s.sessionId === sessionId);
    if (!selected) return;

    setStartTime(toDatetimeLocalValue(selected.startTime));
    setEndTime(toDatetimeLocalValue(selected.endTime));

    if (selected.roomId)
      setRoomId(selected.roomId);
  }, [isSessionType, sessionId, sessions, sessionsLoading]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validacija
    if (!startTime || !endTime) {
      setError('Vrijeme početka i završetka su obavezni.');
      return;
    }

    if (new Date(endTime) <= new Date(startTime)) {
      setError('Vrijeme završetka mora biti nakon vremena početka.');
      return;
    }

    if (isSessionType && !sessionId) {
      setError('Morate odabrati sesiju za ovaj tip stavke.');
      return;
    }

    if (!isSessionType && !title) {
      setError('Naziv stavke je obavezan.');
      return;
    }

    setIsSubmitting(true);

    try {
      const data = {
        type,
        startTime: toUTCIso(startTime),
        endTime: toUTCIso(endTime),
        sessionId: isSessionType ? sessionId : undefined,
        title: !isSessionType ? title : undefined,
        description: !isSessionType ? description : undefined,
        roomId: roomId || undefined,
      };

      if (editingItem) {
        await updateAgendaItem(editingItem.agendaItemId, data);
      } else {
        await createAgendaItem(conferenceId, data);
      }

      onSuccess();
    } catch (err: any) {
      console.error('Greška pri spašavanju:', err);
      setError(err.response?.data?.error || 'Došlo je do greške prilikom spašavanja.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <div className="error-message mb-4">{error}</div>}

      <div className="form-group">
        <label>Tip stavke <span className="text-red-500">*</span></label>
        <select
          value={type}
          onChange={(e) => setType(e.target.value)}
          className="form-input"
          required
        >
          {AGENDA_ITEM_TYPES.map((t) => (
            <option key={t.value} value={t.value}>
              {t.label}
            </option>
          ))}
        </select>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="form-group min-w-0">
          <label>Početak <span className="text-red-500">*</span></label>
          <DatePickerComponent
            selected={startTime ? new Date(startTime) : null}
            onChange={(date: Date | null) => setStartTime(date ? fromDate(date) : '')}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            dateFormat="dd.MM.yyyy HH:mm"
            placeholderText="Odaberite datum i vrijeme"
            className="form-input"
            disabled={isSessionType}
          />
          {isSessionType && sessionId && (
            <span className="text-sm text-gray-500">Vrijeme je preuzeto iz odabrane sesije.</span>
          )}
        </div>

        <div className="form-group min-w-0">
          <label>Završetak <span className="text-red-500">*</span></label>
          <DatePickerComponent
            selected={endTime ? new Date(endTime) : null}
            onChange={(date: Date | null) => setEndTime(date ? fromDate(date) : '')}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            dateFormat="dd.MM.yyyy HH:mm"
            placeholderText="Odaberite datum i vrijeme"
            className="form-input"
            disabled={isSessionType}
          />
        </div>
      </div>

      {isSessionType ? (
        <div className="form-group">
          <label>Sesija <span className="text-red-500">*</span></label>
          <select
            value={sessionId}
            onChange={(e) => setSessionId(e.target.value)}
            className="form-input"
            required
            disabled={sessionsLoading}
          >
            <option value="">Odaberite sesiju</option>
            {sessions.map((s) => (
              <option key={s.sessionId} value={s.sessionId}>
                {s.title} ({new Date(s.startTime).toLocaleTimeString('bs-BA', { hour: '2-digit', minute: '2-digit' })})
              </option>
            ))}
          </select>
          {sessionsLoading && <span className="text-sm text-gray-500">Učitavanje sesija...</span>}
        </div>
      ) : (
        <>
          <div className="form-group">
            <label>Naziv <span className="text-red-500">*</span></label>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className="form-input"
              placeholder="Npr. Pauza za kafu"
              required
            />
          </div>

          <div className="form-group">
            <label>Opis</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="form-input"
              rows={3}
              placeholder="Opcioni opis"
            />
          </div>
        </>
      )}

      <div className="form-group">
        <label>Soba (Opciono)</label>
        <select
          value={roomId}
          onChange={(e) => setRoomId(e.target.value)}
          className="form-input"
          disabled={isSessionType && !!sessions.find((s) => s.sessionId === sessionId)?.roomId}
        >
          <option value="">Nema sobe / Opciono</option>
          {rooms.map((r) => (
            <option key={r.roomId} value={r.roomId}>
              {r.name}
            </option>
          ))}
        </select>
      </div>

      <div className="flex gap-4 pt-4">
        <button
          type="button"
          onClick={onCancel}
          className="btn-secondary flex-1"
          disabled={isSubmitting}
        >
          Odustani
        </button>
        <button
          type="submit"
          className="btn-primary flex-1"
          disabled={isSubmitting}
        >
          {isSubmitting ? 'Spašavanje...' : 'Spasi'}
        </button>
      </div>
    </form>
  );
}
