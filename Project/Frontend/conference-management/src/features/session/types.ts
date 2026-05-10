export interface Session {
  sessionId: string;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  sessionType: string;
  status: string;
  speakerName?: string;
}

export interface CreateSessionData {
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  conferenceId: string;
  roomId: string; // Need to handle rooms
  sessionType: string;
}

export interface UpdateSessionData {
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  roomId: string;
  sessionType: string;
}

export interface AssignSpeakerData {
  userId: string;
}

export interface User {
  userId: string;
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
}

export interface SessionState {
  items: Session[];
  isLoading: boolean;
  error: string | null;
  refresh?: () => Promise<void>;
}