import axios from 'axios';
import type { Session, CreateSessionData, UpdateSessionData, AssignSpeakerData, User } from '../types';

// API base URL - uses env variable, falls back to Docker port 8082
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export async function fetchSessions(conferenceId: string): Promise<Session[]> {
  try {
    const response = await axios.get<Session[]>(`${BASE_URL}/api/conferences/${conferenceId}/sessions`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju sesija:', error);
    return [];
  }
}

export async function fetchRegisteredSessions(): Promise<Session[]> {
  try {
    const response = await axios.get<Session[]>(`${BASE_URL}/api/Sessions/registered`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju registrovanih sesija:', error);
    return [];
  }
}

interface ApiMessageResponse {
  Message?: string;
  message?: string;
}

export async function registerForSession(sessionId: string): Promise<string> {
  try {
    const response = await axios.post<ApiMessageResponse>(`${BASE_URL}/api/session/${sessionId}/register`);
    const data = response.data;
    const message = typeof data === 'string' ? data : data?.Message ?? data?.message;
    return message ?? 'Prijava je uspješno evidentirana.';
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const responseData = error.response?.data as ApiMessageResponse | string | undefined;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData?.Message ?? responseData?.message ?? error.message;
      throw new Error(message || `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška prilikom prijave na sesiju.');
  }
}

export async function cancelSessionRegistration(registrationId: string): Promise<string> {
  try {
    const response = await axios.put<ApiMessageResponse>(`${BASE_URL}/api/session/${registrationId}/cancel`);
    const data = response.data;
    const message = typeof data === 'string' ? data : data?.Message ?? data?.message;
    return message ?? 'Prijava je otkazana.';
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const responseData = error.response?.data as ApiMessageResponse | string | undefined;
      const message =
        typeof responseData === 'string'
          ? responseData
          : responseData?.Message ?? responseData?.message ?? error.message;
      throw new Error(message || `Status ${error.response?.status ?? 'unknown'}`);
    }
    throw new Error('Greška prilikom odjave sa sesije.');
  }
}

export async function createSession(sessionData: CreateSessionData): Promise<{ sessionId: string } | null> {
  try {
    const payload = {
      ...sessionData,
      startTime: new Date(sessionData.startTime).toISOString(),
      endTime: new Date(sessionData.endTime).toISOString(),
    };
    const response = await axios.post<string>(`${BASE_URL}/api/sessions`, payload);
    return { sessionId: response.data };
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      throw new Error(error.response.data);
    }
    throw new Error('Greška pri kreiranju sesije');
  }
}

export async function updateSession(id: string, sessionData: UpdateSessionData): Promise<void> {
  const payload = {
    ...sessionData,
    startTime: new Date(sessionData.startTime).toISOString(),
    endTime: new Date(sessionData.endTime).toISOString(),
  };
  await axios.put(`${BASE_URL}/api/sessions/${id}`, payload);
}

export async function deleteSession(id: string): Promise<void> {
  await axios.delete(`${BASE_URL}/api/sessions/${id}`);
}

export async function assignSpeaker(sessionId: string, speakerData: AssignSpeakerData): Promise<void> {
  await axios.put(`${BASE_URL}/api/sessions/${sessionId}/assign-speaker`, speakerData);
}

export async function fetchSessionById(id: string): Promise<Session | null> {
  try {
    const response = await axios.get<Session>(`${BASE_URL}/api/sessions/${id}`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju sesije:', error);
    return null;
  }
}
export async function fetchUsers(): Promise<User[]> {
  try {
    const token = localStorage.getItem('auth_token');
    const response = await axios.get<{ users: User[] }>(`${BASE_URL}/api/user/by-role`, {
      params: { role: 'predavac' },
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    console.log('Fetched lecturers:', response.data.users);
    return response.data.users;
  } catch (error) {
    console.error('Greška pri dohvatanju korisnika:', error);
    return [];
  }
}