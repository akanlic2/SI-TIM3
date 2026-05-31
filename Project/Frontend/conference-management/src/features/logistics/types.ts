export interface LogisticsTask {
  logisticsTaskId: string;
  conferenceId: string;
  title: string;
  description: string;
  taskType: string;
  dueDate: string;
  status: string;
}

export interface CreateLogisticsTaskData {
  title: string;
  description: string;
  taskType: string;
  dueDate: string;
  status: string;
}

export interface UpdateLogisticsTaskData {
  title: string;
  description: string;
  taskType: string;
  dueDate: string;
  status: string;
}

export interface LogisticsState {
  items: LogisticsTask[];
  isLoading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
}

export const LOGISTICS_TASK_TYPES = [
  { value: 'Catering', label: 'Catering' },
  { value: 'Ručak', label: 'Ručak' },
  { value: 'Video sadržaj', label: 'Video sadržaj' },
  { value: 'Registracija učesnika', label: 'Registracija učesnika' },
  { value: 'Čišćenje prostora', label: 'Čišćenje prostora' },
  { value: 'Transport', label: 'Transport' },
  { value: 'Ostalo', label: 'Ostalo' },
];

export const LOGISTICS_STATUS_OPTIONS = [
  { value: 'Pending', label: 'Na čekanju' },
  { value: 'InProgress', label: 'U toku' },
  { value: 'Completed', label: 'Završeno' },
  { value: 'Cancelled', label: 'Otkazano' },
];
