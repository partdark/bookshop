import api from './client'
import type { Order, AuthResponse } from '../types'

// Orders
export const getAllOrders = () =>
  api.get<Order[]>('/orders').then((r) => r.data)

export const updateOrderStatus = (id: number, status: string) =>
  api.patch(`/order/${id}/status`, { status })

// Books
export const addBook = (data: {
  title: string
  description: string
  price: number
  urlImage: string
  count: number
  publicationYear: number
  authorsIds: string[]
  genresIds: string[]
}) =>
  api.post<string>('/book/createbookwithfullinfo', {
    bookDto: {
      title: data.title,
      description: data.description,
      rating: 0,
      price: data.price,
      urlImage: data.urlImage,
      count: data.count,
      publicationYear: data.publicationYear,
    },
    authorsIds: data.authorsIds,
    genresIds: data.genresIds,
  }).then((r) => r.data)

export const updateBookCount = (id: string, count: number) =>
  api.patch(`/book/patch/${id}`, [{ op: 'replace', path: '/count', value: count }])

export const deleteBook = (id: string) =>
  api.delete(`/book/delete/${id}`)

export const deleteReview = (id: string) =>
  api.delete(`/review/delete/${id}`)

// Admin registration
export const registerAdmin = (data: {
  name: string
  email: string
  password: string
  phone: string
  dateOfBirth: string
}) => api.post<AuthResponse>('/auth/register-admin', data).then((r) => r.data)

export const getAuthors = () =>
  api.get<{ id: string; name: string; year: number }[]>('/authors/all').then((r) => r.data)

export const addAuthor = (name: string, year: number) =>
  api.put<{ id: string; name: string; year: number }>('/author/add', { name, year }).then((r) => r.data)

export const addGenre = (name: string) =>
  api.post<string>('/genre/add', { name }).then((r) => r.data)
