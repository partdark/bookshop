import api from './client'
import type { Book, CatalogResponse } from '../types'

export const getCatalog = (params: {
  pageNumber?: number
  pageCapacity?: number
  titleContains?: string
  orderBy?: string
  desc?: boolean
}) =>
  api.get<CatalogResponse>('/catalog', { params }).then((r) => r.data)

export const getBook = (id: string) =>
  api.get<Book>(`/catalog/${id}`).then((r) => r.data)

export const getGenres = () =>
  api.get<{ id: string; name: string }[]>('/genres').then((r) => r.data)
