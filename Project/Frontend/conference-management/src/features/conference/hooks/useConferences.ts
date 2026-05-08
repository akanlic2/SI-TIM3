import { useEffect, useState, useCallback } from 'react'
import { fetchConferences } from '../api/conferenceApi'
import type { ConferenceState } from '../types'

const initialState: ConferenceState = {
  items: [],
  isLoading: true,
  error: null,
}

export function useConferences() {
  const [state, setState] = useState<ConferenceState>(initialState)
  const [counter, setCounter] = useState(0)

  useEffect(() => {
    setState(prev => ({ ...prev, isLoading: true }))
    fetchConferences()
      .then((items) => {
        setState({ items, isLoading: false, error: null })
      })
      .catch(() => {
        setState({ items: [], isLoading: false, error: 'Failed to load conferences.' })
      })
  }, [counter])

  const refresh = useCallback(() => {
    setCounter(c => c + 1)
  }, [])

  return { ...state, refresh }
}
