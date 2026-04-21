import { useEffect, useState } from 'react'
import { fetchConferences } from '../api/conferenceApi'
import type { ConferenceState } from '../types'

const initialState: ConferenceState = {
  items: [],
  isLoading: true,
  error: null,
}

export function useConferences() {
  const [state, setState] = useState<ConferenceState>(initialState)

  useEffect(() => {
    fetchConferences()
      .then((items) => {
        setState({ items, isLoading: false, error: null })
      })
      .catch(() => {
        setState({ items: [], isLoading: false, error: 'Failed to load conferences.' })
      })
  }, [])

  return state
}
