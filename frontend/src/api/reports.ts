import api from './client'

export interface ReportOrderCount {
  name: string
  count: number
}

export interface ReportOrderMoney {
  name: string
  count: number
  totalMoney: number
}

export const getOrdersCount = (startDate?: string, endDate?: string) =>
  api.get<ReportOrderCount[]>(`/report/orders`, { params: { startDate, endDate } }).then(r => r.data)

export const getMoneyByType = (startDate?: string, endDate?: string) =>
  api.get<ReportOrderMoney[]>(`/report/money`, { params: { startDate, endDate } }).then(r => r.data)
