import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { Router } from './app/router.tsx'
import { setupAuthInterceptor } from './auth/httpInterceptor.ts'

setupAuthInterceptor()

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Router />
  </StrictMode>,
)
