export interface Conference {
	id: string
	title: string
	location: string
	startsAt: string
}

export interface ConferenceState {
	items: Conference[]
	isLoading: boolean
	error: string | null
}
