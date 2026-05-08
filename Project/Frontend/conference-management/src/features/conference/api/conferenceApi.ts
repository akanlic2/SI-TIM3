import axios from 'axios';
import type { Conference, CreateConferenceData, UpdateConferenceData } from '../types';

// API base URL - uses env variable, falls back to Docker port 8082
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';
const API_URL = `${BASE_URL}/api/Conference`;

export async function fetchConferences(): Promise<Conference[]> {
  try {
    const response = await axios.get<Conference[]>(API_URL);

    return response.data;
  } catch (error) {
    console.error("Greška pri dohvatanju konferencija:", error);
    return [];
  }
}

export async function fetchConferenceById(id: string): Promise<Conference | null> {
  try {
    const response = await axios.get<Conference>(`${API_URL}/${id}`);

    return response.data;
  } catch (error) {
    console.error("Greška pri dohvatanju konferencije:", error);
    return null;
  }
}

export async function createConference(conferenceData: CreateConferenceData): Promise<Conference> {
  const response = await axios.post<Conference>(API_URL, conferenceData);

  return response.data;
}

export async function updateConference(id: string, conferenceData: UpdateConferenceData): Promise<void> {
  await axios.put(`${API_URL}/${id}`, conferenceData);
}

export async function deleteConference(id: string): Promise<void> {
  await axios.delete(`${API_URL}/${id}`);
}