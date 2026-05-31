import axios from 'axios';
import type { LogisticsTask, CreateLogisticsTaskData, UpdateLogisticsTaskData } from '../types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export async function fetchLogistics(conferenceId: string, taskType?: string): Promise<LogisticsTask[]> {
  try {
    const params: Record<string, string> = {};
    if (taskType) params.taskType = taskType;

    const response = await axios.get<LogisticsTask[]>(
      `${BASE_URL}/api/conferences/${conferenceId}/logistics`,
      { params }
    );
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju logističkih aktivnosti:', error);
    return [];
  }
}

export async function createLogisticsTask(
  conferenceId: string,
  data: CreateLogisticsTaskData
): Promise<LogisticsTask> {
  const response = await axios.post<LogisticsTask>(
    `${BASE_URL}/api/conferences/${conferenceId}/logistics`,
    data
  );
  return response.data;
}

export async function updateLogisticsTask(
  id: string,
  data: UpdateLogisticsTaskData
): Promise<LogisticsTask> {
  const response = await axios.put<LogisticsTask>(
    `${BASE_URL}/api/logistics/${id}`,
    data
  );
  return response.data;
}

export async function deleteLogisticsTask(id: string): Promise<void> {
  await axios.delete(`${BASE_URL}/api/logistics/${id}`);
}
