import api from './client'
import type { CartItem, Order } from '../types'

export const getCart = (customerId: string) =>
  api.get<CartItem[]>(`/cart/${customerId}`).then((r) => r.data)

export const addToCart = (customerId: string, bookId: string, count = 1) =>
  api.post(`/cart/${customerId}/add`, null, { params: { bookId, count } })

export const updateCartItem = (customerId: string, bookId: string, count: number) =>
  api.put(`/cart/${customerId}/update`, null, { params: { bookId, count } })

export const removeFromCart = (customerId: string, bookId: string) =>
  api.delete(`/cart/${customerId}/remove/${bookId}`)

export const clearCart = (customerId: string) =>
  api.delete(`/cart/${customerId}/clear`)

export const checkout = (customerId: string) =>
  api.post<Order>(`/cart/${customerId}/checkout`).then((r) => r.data)
