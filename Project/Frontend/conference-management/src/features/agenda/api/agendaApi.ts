import axios from 'axios';
import type { AgendaItem, CreateAgendaItemData, UpdateAgendaItemData } from '../types';

// API base URL - uses env variable, falls back to Docker port 8082
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export async function fetchAgenda(conferenceId: string): Promise<AgendaItem[]> {
  try {
    const response = await axios.get<AgendaItem[]>(
      `${BASE_URL}/api/conferences/${conferenceId}/agenda`
    );
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju agende:', error);
    return [];
  }
}

export async function createAgendaItem(
  conferenceId: string,
  data: CreateAgendaItemData
): Promise<AgendaItem> {
  const payload = {
    ...data,
    startTime: new Date(data.startTime).toISOString(),
    endTime: new Date(data.endTime).toISOString(),
  };
  const response = await axios.post<AgendaItem>(
    `${BASE_URL}/api/conferences/${conferenceId}/agenda`,
    payload
  );
  return response.data;
}

export async function updateAgendaItem(
  id: string,
  data: UpdateAgendaItemData
): Promise<void> {
  const payload = {
    ...data,
    startTime: new Date(data.startTime).toISOString(),
    endTime: new Date(data.endTime).toISOString(),
  };
  await axios.put(`${BASE_URL}/api/agenda/${id}`, payload);
}

export async function deleteAgendaItem(id: string): Promise<void> {
  await axios.delete(`${BASE_URL}/api/agenda/${id}`);
}
