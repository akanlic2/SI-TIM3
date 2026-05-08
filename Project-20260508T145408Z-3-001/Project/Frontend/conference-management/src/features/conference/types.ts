export interface Conference {
  conferenceId: string;
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category: string;
  maxParticipants: number;
  status: string;
}

export interface CreateConferenceData {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category: string;
  maxParticipants: number;
}

export interface UpdateConferenceData {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  category: string;
  maxParticipants: number;
}

export interface ConferenceState {
  items: Conference[];
  isLoading: boolean;
  error: string | null;
  refresh?: () => Promise<void>;
}
