import { create } from 'zustand'
import type { Customer } from '../types'

interface AuthState {
  token: string | null
  refreshToken: string | null
  user: Customer | null
  role: string | null
  setAuth: (token: string, refreshToken: string, user: Customer, role?: string) => void
  logout: () => void
  isAuthenticated: () => boolean
  isAdmin: () => boolean
}

const savedToken = localStorage.getItem('token')
const savedRefresh = localStorage.getItem('refreshToken')
const savedUser = localStorage.getItem('user')
const savedRole = localStorage.getItem('role')

export const useAuthStore = create<AuthState>((set, get) => ({
  token: savedToken,
  refreshToken: savedRefresh,
  user: savedUser ? JSON.parse(savedUser) : null,
  role: savedRole,
  setAuth: (token, refreshToken, user, role) => {
    const r = role ?? 'user'
    localStorage.setItem('token', token)
    localStorage.setItem('refreshToken', refreshToken)
    localStorage.setItem('user', JSON.stringify(user))
    localStorage.setItem('role', r)
    set({ token, refreshToken, user, role: r })
  },
  logout: () => {
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')
    localStorage.removeItem('role')
    set({ token: null, refreshToken: null, user: null, role: null })
  },
  isAuthenticated: () => !!get().token,
  isAdmin: () => get().role === 'Admin',
}))
