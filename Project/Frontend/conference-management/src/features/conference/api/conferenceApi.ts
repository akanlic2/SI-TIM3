import axios from 'axios';
import type { Conference, CreateConferenceData, UpdateConferenceData } from '../types';

// API base URL - matches backend route
const API_URL = 'http://localhost:5268/api/Conference';

export async function fetchConferences(): Promise<Conference[]> {
  const token = localStorage.getItem('kc_access_token');

  try {
    const response = await axios.get<Conference[]>(API_URL, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    return response.data;
  } catch (error) {
    console.error("Greška pri dohvatanju konferencija:", error);
    return [];
  }
}

export async function fetchConferenceById(id: string): Promise<Conference | null> {
  const token = localStorage.getItem('kc_access_token');

  try {
    const response = await axios.get<Conference>(`${API_URL}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    return response.data;
  } catch (error) {
    console.error("Greška pri dohvatanju konferencije:", error);
    return null;
  }
}

export async function createConference(conferenceData: CreateConferenceData): Promise<Conference> {
  const token = localStorage.getItem('kc_access_token');

  const response = await axios.post<Conference>(API_URL, conferenceData, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  return response.data;
}

export async function updateConference(id: string, conferenceData: UpdateConferenceData): Promise<void> {
  const token = localStorage.getItem('kc_access_token');

  await axios.put(`${API_URL}/${id}`, conferenceData, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
}

export async function deleteConference(id: string): Promise<void> {
  const token = localStorage.getItem('kc_access_token');

  await axios.delete(`${API_URL}/${id}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
}