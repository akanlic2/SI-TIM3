import axios from 'axios';
import type { Session, CreateSessionData, UpdateSessionData, AssignSpeakerData, User, SessionMaterial } from '../types';

// API base URL - uses env variable, falls back to Docker port 8082
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export async function fetchSessions(conferenceId: string, token?: string): Promise<Session[]> {
  try {
    const response = await axios.get<Session[]>(`${BASE_URL}/api/conferences/${conferenceId}/sessions`, {
      headers: token
        ? {
            Authorization: `Bearer ${token}`,
          }
        : undefined,
    });
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju sesija:', error);
    return [];
  }
}

export async function fetchRegisteredSessions(): Promise<Session[]> {
  try {
    // ISPRAVLJENO: 'sessions' umjesto 'Sessions' radi Docker case-sensitivity-ja
    const response = await axios.get<Session[]>(`${BASE_URL}/api/sessions/registered`);
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju registrovanih sesija:', error);
    return [];
  }
}

export async function fetchSpeakerSessions(token: string): Promise<Array<Session & { conferenceTitle?: string }>> {
  try {
    const response = await axios.get<Array<Session & { conferenceTitle?: string }>>(
      `${BASE_URL}/api/speakers/sessions`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju predavačevih sesija:', error);
    if (axios.isAxiosError(error)) {
      const message = error.response?.data?.message || error.response?.data?.Message || error.message;
      throw new Error(message ?? 'Greška pri dohvatanju predavačevih sesija');
    }
    throw new Error('Greška pri dohvatanju predavačevih sesija');
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

export async function createSession(sessionData: CreateSessionData): Promise<any> {
  try {
    const payload = {
      ...sessionData,
      startTime: new Date(sessionData.startTime).toISOString(),
      endTime: new Date(sessionData.endTime).toISOString(),
    };
    
    // ISPRAVLJENO: Očekujemo objekat sesije nazad, a ne čisti string
    const response = await axios.post(`${BASE_URL}/api/sessions`, payload);
    return response.data;
  } catch (error) {
    // ISPRAVLJENO: Pametno prosljeđivanje { error: "poruka" } objekta prema formi
    if (axios.isAxiosError(error) && error.response?.data) {
      throw error; // Prosljeđujemo cijeli error objekt da ga forma može parsirati kroz .response.data.error
    }
    throw new Error('Greška pri kreiranju sesije');
  }
}

export async function updateSession(id: string, sessionData: UpdateSessionData): Promise<void> {
  try {
    const payload = {
      ...sessionData,
      startTime: new Date(sessionData.startTime).toISOString(),
      endTime: new Date(sessionData.endTime).toISOString(),
    };
    await axios.put(`${BASE_URL}/api/sessions/${id}`, payload);
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      throw error;
    }
    throw new Error('Greška pri ažuriranju sesije');
  }
}

// --- NOVO: DODANA FUNKCIJA ZA DODJELU DVORANE SESIJI ---
export async function assignRoomToSession(sessionId: string, roomId: string): Promise<void> {
  try {
    await axios.put(`${BASE_URL}/api/sessions/${sessionId}/room`, { roomId });
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      throw error;
    }
    throw new Error('Greška pri dodjeli dvorane sesiji.');
  }
}

export async function deleteSession(id: string): Promise<void> {
  await axios.delete(`${BASE_URL}/api/sessions/${id}`);
}

export async function assignSpeaker(sessionId: string, speakerData: AssignSpeakerData): Promise<void> {
  try {
    await axios.put(`${BASE_URL}/api/sessions/${sessionId}/assign-speaker`, speakerData);
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      throw error;
    }
    throw new Error('Greška pri dodjeli predavača.');
  }
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

export async function fetchSessionMaterials(sessionId: string, token?: string): Promise<SessionMaterial[]> {
  try {
    const response = await axios.get<SessionMaterial[]>(`${BASE_URL}/api/sessions/${sessionId}/materials`, {
      headers: token
        ? {
            Authorization: `Bearer ${token}`,
          }
        : undefined,
    });
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju materijala:', error);
    return [];
  }
}

export async function uploadSessionMaterial(
  sessionId: string,
  title: string,
  description: string,
  file: File,
  token: string
): Promise<void> {
  try {
    const formData = new FormData();
    formData.append('title', title);
    formData.append('description', description);
    formData.append('file', file);

    await axios.post(`${BASE_URL}/api/sessions/${sessionId}/materials`, formData, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const data = error.response?.data as { message?: string; Message?: string } | string | undefined;
      const message =
        typeof data === 'string'
          ? data
          : data?.message || data?.Message || error.message;
      throw new Error(message || 'Greška pri uploadu materijala.');
    }
    throw new Error('Greška pri uploadu materijala.');
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