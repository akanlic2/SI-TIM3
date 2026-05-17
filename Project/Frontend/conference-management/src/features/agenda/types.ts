export interface AgendaItem {
  agendaItemId: string;
  conferenceId: string;
  sessionId?: string;
  roomId?: string;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  type: string;
  createdAt: string;

  // Session podaci (samo ako je type === 'Session')
  sessionTitle?: string;
  sessionType?: string;
  speakerName?: string;

  // Room podaci
  roomName?: string;
}

export interface CreateAgendaItemData {
  type: string;
  startTime: string;
  endTime: string;
  sessionId?: string;
  title?: string;
  description?: string;
  roomId?: string;
}

export interface UpdateAgendaItemData {
  type: string;
  startTime: string;
  endTime: string;
  sessionId?: string;
  title?: string;
  description?: string;
  roomId?: string;
}

export interface AgendaState {
  items: AgendaItem[];
  isLoading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
}

export const AGENDA_ITEM_TYPES = [
  { value: 'Session', label: 'Sesija' },
  { value: 'Break', label: 'Pauza' },
  { value: 'Lunch', label: 'Ručak' },
  { value: 'Networking', label: 'Networking' },
  { value: 'Opening', label: 'Otvaranje' },
  { value: 'Closing', label: 'Zatvaranje' },
] as const;

export type AgendaItemType = typeof AGENDA_ITEM_TYPES[number]['value'];
