import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { onUnauthorized } from '../config/axios'
import { sessionApi } from '../config/sessionApi'
import type { SessionResponse, SessionUser } from '../shared/types/contracts'

export type AuthStatus = 'checking' | 'authenticated' | 'anonymous'

interface AuthState {
  status: AuthStatus
  session: SessionResponse | null
}

interface AuthContextValue {
  status: AuthStatus
  user: SessionUser | null
  isAuthenticated: boolean
  login: (session: SessionResponse) => void
  logout: () => Promise<void>
}

const ANONYMOUS: AuthState = { status: 'anonymous', session: null }

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({ status: 'checking', session: null })

  const login = useCallback((session: SessionResponse) => {
    setState({ status: 'authenticated', session })
  }, [])

  const clearSession = useCallback(() => setState(ANONYMOUS), [])

  const logout = useCallback(async () => {
    try {
      await sessionApi.end()
    } finally {
      clearSession()
    }
  }, [clearSession])

  useEffect(() => {
    let isActive = true

    sessionApi
      .get()
      .then((session) => {
        if (isActive) {
          setState({ status: 'authenticated', session })
        }
      })
      .catch(() => {
        if (isActive) {
          setState(ANONYMOUS)
        }
      })

    return () => {
      isActive = false
    }
  }, [])

  useEffect(() => onUnauthorized(clearSession), [clearSession])

  useEffect(() => {
    if (!state.session) {
      return undefined
    }

    const remainingMs = new Date(state.session.expiresAt).getTime() - Date.now()
    const timer = window.setTimeout(clearSession, Math.max(remainingMs, 0))

    return () => window.clearTimeout(timer)
  }, [state.session, clearSession])

  const value = useMemo<AuthContextValue>(
    () => ({
      status: state.status,
      user: state.session?.user ?? null,
      isAuthenticated: state.status === 'authenticated',
      login,
      logout,
    }),
    [state, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }

  return context
}
