import api from './client'

export const addReview = (data: {
  rating: number
  reviewText: string
  bookId: string
  customerId: string
}) => api.post<string>('/review/add', data).then((r) => r.data)
