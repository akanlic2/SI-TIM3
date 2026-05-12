import axios from 'axios';
import type { Conference, CreateConferenceData, UpdateConferenceData } from '../types';

// API base URL - uses env variable, falls back to Docker port 8082
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';
const API_URL = `${BASE_URL}/api/Conference`;

export interface ConferenceQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  location?: string;
  category?: string;
  status?: string;
}

export interface PagedConferenceResponse {
  items: Conference[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiMessageResponse {
  Message?: string;
  message?: string;
}

export async function fetchConferences(
  query: ConferenceQuery = {}
): Promise<PagedConferenceResponse> {
  try {
    const response = await axios.get<PagedConferenceResponse>(API_URL, {
      params: query,
    });

    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju konferencija:', error);

    return {
      items: [],
      totalCount: 0,
      page: query.page ?? 1,
      pageSize: query.pageSize ?? 6,
      totalPages: 1,
    };
  }
}

export async function fetchConferenceById(id: string): Promise<Conference | null> {
  try {
    const response = await axios.get<Conference>(`${API_URL}/${id}`);

    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju konferencije:', error);
    return null;
  }
}

export async function createConference(
  conferenceData: CreateConferenceData
): Promise<Conference> {
  const response = await axios.post<Conference>(API_URL, conferenceData);

  return response.data;
}

export async function updateConference(
  id: string,
  conferenceData: UpdateConferenceData
): Promise<void> {
  await axios.put(`${API_URL}/${id}`, conferenceData);
}

export async function deleteConference(id: string): Promise<void> {
  await axios.delete(`${API_URL}/${id}`);
}

export async function registerForConference(id: string): Promise<string> {
  try {
    const response = await axios.post<ApiMessageResponse>(
      `${BASE_URL}/api/conference/${id}/register`
    );

    const data = response.data;
    const message =
      typeof data === 'string' ? data : data?.Message ?? data?.message;

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

    throw new Error('Greška prilikom prijave.');
  }
}