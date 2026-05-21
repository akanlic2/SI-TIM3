import axios from 'axios';
import type { Question, CreateQuestionData } from '../types';
 
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';
 
// S47-BE-02: Dohvati sva pitanja za sesiju, sortirana po vremenu
export async function fetchQuestions(sessionId: string): Promise<Question[]> {
  try {
    const response = await axios.get<Question[]>(
      `${BASE_URL}/api/sessions/${sessionId}/questions`
    );
    return response.data;
  } catch (error) {
    console.error('Greška pri dohvatanju pitanja:', error);
    return [];
  }
}
 
// S47-BE-01: Postavi novo pitanje za sesiju
export async function createQuestion(
  sessionId: string,
  data: CreateQuestionData
): Promise<Question> {
  try {
    const response = await axios.post<Question>(
      `${BASE_URL}/api/sessions/${sessionId}/questions`,
      data
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.data) {
      throw error;
    }
    throw new Error('Greška pri slanju pitanja.');
  }
}