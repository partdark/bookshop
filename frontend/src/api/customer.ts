import api from './client'
import type { Customer, Order, OrderDetail } from '../types'

export const getCustomer = (id: string) =>
  api.get<Customer>(`/customer/${id}`).then((r) => r.data)

export const updateCustomer = (id: string, data: {
  name: string
  mail: string
  phone: string
  dateOfBirth: string
}) => api.put<Customer>(`/customer/${id}`, data).then((r) => r.data)

export const getMyOrders = (id: string) =>
  api.get<Order[]>(`/customer/${id}/orders`).then((r) => r.data)

export const getOrderDetail = (id: number) =>
  api.get<OrderDetail>(`/order/${id}/detail`).then((r) => r.data)
