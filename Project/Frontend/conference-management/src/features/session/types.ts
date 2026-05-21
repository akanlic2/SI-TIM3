export interface Session {
  sessionId: string;
  sessionRegistrationId?: string;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  sessionType: string;
  status: string;
  speakerName?: string;
  roomName?: string;
  roomId?: string;
  assignedSpeakerId?: string;
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

export interface Question {
  questionId: string;
  sessionId: string;
  userId: string;
  authorName: string;
  content: string;
  askedAt: string;
  answer: string | null;
  status: string;
}
 
export interface CreateQuestionData {
  content: string;
}