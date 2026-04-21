import { useEffect, useState } from 'react'
import App from './App'
import ConferencesPage from '../pages/ConferencesPage'

function resolveRoute(pathname: string) {
	if (pathname === '/conferences') {
		return <ConferencesPage />
	}

	return <App />
}

export function Router() {
	const [pathname, setPathname] = useState(window.location.pathname)

	useEffect(() => {
		const onPopState = () => {
			setPathname(window.location.pathname)
		}

		window.addEventListener('popstate', onPopState)

		return () => {
			window.removeEventListener('popstate', onPopState)
		}
	}, [])

	return resolveRoute(pathname)
}

