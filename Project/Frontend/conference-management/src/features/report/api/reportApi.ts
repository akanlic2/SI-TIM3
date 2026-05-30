const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8082';

export interface RegistrationStatsDto {
  total: number
  confirmed: number
  pending: number
  cancelled: number
}

export interface SessionReportDto {
  sessionId: string
  title: string
  registeredCount: number
  roomCapacity: number
  speakerCount: number
  materialCount: number
}

export interface ConferenceReportDto {
  conferenceId: string
  title: string
  location: string
  startDate: string
  endDate: string
  registrationStats: RegistrationStatsDto
  sessions: SessionReportDto[]
  totalMaterials: number
  totalSpeakers: number
}

export async function fetchConferenceReport(
  conferenceId: string,
  token: string
): Promise<ConferenceReportDto> {
  const response = await fetch(
    `${BASE_URL}/api/conferences/${conferenceId}/report`,
    { headers: { Authorization: `Bearer ${token}` } }
  )
  if (!response.ok) throw new Error('Greška pri dohvatanju izvještaja.')
  return response.json()
}

export async function downloadConferenceReport(
  conferenceId: string,
  token: string
): Promise<void> {
  const response = await fetch(
    `${BASE_URL}/api/conferences/${conferenceId}/report/download`,
    { headers: { Authorization: `Bearer ${token}` } }
  )
  if (!response.ok) throw new Error('Greška pri preuzimanju PDF-a.')

  const blob = await response.blob()
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `izvjestaj-${conferenceId}.pdf`
  a.click()
  URL.revokeObjectURL(url)
}