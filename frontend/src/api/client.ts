import axios from 'axios'

const api = axios.create({ baseURL: '/api' })

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

let isRefreshing = false
let failedQueue: Array<{ resolve: (v: string) => void; reject: (e: unknown) => void }> = []

const processQueue = (error: unknown, token: string | null) => {
  failedQueue.forEach((p) => (error ? p.reject(error) : p.resolve(token!)))
  failedQueue = []
}

api.interceptors.response.use(
  (r) => r,
  async (err) => {
    const original = err.config

    if (err.response?.status !== 401 || original._retry) {
      return Promise.reject(err)
    }

    const storedRefresh = localStorage.getItem('refreshToken')
    if (!storedRefresh) {
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      window.location.href = '/login'
      return Promise.reject(err)
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject })
      }).then((token) => {
        original.headers.Authorization = `Bearer ${token}`
        return api(original)
      })
    }

    original._retry = true
    isRefreshing = true

    try {
      const res = await axios.post('/api/auth/refresh', { refreshToken: storedRefresh })
      const { token, refreshToken: newRefresh, customer } = res.data

      localStorage.setItem('token', token)
      localStorage.setItem('refreshToken', newRefresh)
      localStorage.setItem('user', JSON.stringify(customer))

      // Обновляем zustand store без циклического импорта
      window.dispatchEvent(new CustomEvent('auth:refreshed', { detail: { token, refreshToken: newRefresh, customer } }))

      api.defaults.headers.common.Authorization = `Bearer ${token}`
      original.headers.Authorization = `Bearer ${token}`
      processQueue(null, token)
      return api(original)
    } catch (refreshErr) {
      processQueue(refreshErr, null)
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      window.location.href = '/login'
      return Promise.reject(refreshErr)
    } finally {
      isRefreshing = false
    }
  }
)

export default api
