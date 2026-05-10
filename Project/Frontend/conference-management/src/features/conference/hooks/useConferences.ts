import { useEffect, useState, useCallback } from 'react'
import { fetchConferences } from '../api/conferenceApi'
import type { Conference } from '../types'

export function useConferences() {
  const [items, setItems] = useState<Conference[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [page, setPage] = useState(1)
  const [pageSize] = useState(6)
  const [totalPages, setTotalPages] = useState(1)

  const [search, setSearch] = useState('')
  const [location, setLocation] = useState('')
  const [category, setCategory] = useState('')

  const load = useCallback(async () => {
    try {
      setIsLoading(true)
      setError(null)

      const result = await fetchConferences({
        page,
        pageSize,
        search: search || undefined,
        location: location || undefined,
        category: category || undefined,
      })

      const conferences = Array.isArray(result) ? result : result.items ?? []

      setItems(conferences)
      setTotalPages(Array.isArray(result) ? 1 : result.totalPages || 1)
    } catch (error) {
      console.error(error)
      setItems([])
      setError('Failed to load conferences.')
    } finally {
      setIsLoading(false)
    }
  }, [page, pageSize, search, location, category])

  useEffect(() => {
    load()
  }, [load])

  const refresh = useCallback(() => {
    load()
  }, [load])

  return {
    items,
    isLoading,
    error,
    refresh,
    page,
    setPage,
    totalPages,
    search,
    setSearch,
    location,
    setLocation,
    category,
    setCategory,
  }
}