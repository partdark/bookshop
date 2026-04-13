import api from './client'
import type { AuthResponse, Customer } from '../types'

export const login = (email: string, password: string) =>
  api.post<AuthResponse>('/auth/login', { email, password }).then((r) => r.data)

export const register = (data: {
  name: string
  email: string
  password: string
  phone: string
  dateOfBirth: string
}) => api.post<AuthResponse>('/auth/register', data).then((r) => r.data)

export const getMe = () =>
  api.get<Customer>('/auth/me').then((r) => r.data)

export const changePassword = (currentPassword: string, newPassword: string) =>
  api.post('/auth/change-password', { currentPassword, newPassword })

export const refreshToken = (refreshToken: string) =>
  api.post<AuthResponse>('/auth/refresh', { refreshToken }).then((r) => r.data)

export const logout = () =>
  api.post('/auth/logout').catch(() => {})
