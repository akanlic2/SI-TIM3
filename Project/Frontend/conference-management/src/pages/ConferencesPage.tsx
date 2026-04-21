import { ConferenceList, useConferences } from '../features/conference'

export default function ConferencesPage() {
  const { items, isLoading, error } = useConferences()

  return (
    <main>
      <h1>Conferences</h1>
      {isLoading && <p>Loading conferences...</p>}
      {error && <p>{error}</p>}
      {!isLoading && !error && <ConferenceList conferences={items} />}
    </main>
  )
}
