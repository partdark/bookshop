import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { useAuthStore } from './store/authStore'
import { useCartStore } from './store/cartStore'
import { useEffect } from 'react'
import Navbar from './components/Navbar'
import ErrorBoundary from './components/ErrorBoundary'
import CatalogPage from './pages/CatalogPage'
import BookPage from './pages/BookPage'
import CartPage from './pages/CartPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ProfilePage from './pages/ProfilePage'
import AdminPage from './pages/AdminPage'

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuthStore()
  return isAuthenticated() ? <>{children}</> : <Navigate to="/login" replace />
}

function AdminRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isAdmin } = useAuthStore()
  if (!isAuthenticated()) return <Navigate to="/login" replace />
  if (!isAdmin()) return <Navigate to="/catalog" replace />
  return <>{children}</>
}

export default function App() {
  const { user, setAuth } = useAuthStore()
  const { loadCount } = useCartStore()

  useEffect(() => {
    const handler = (e: Event) => {
      const { token, refreshToken, customer, role } = (e as CustomEvent).detail
      setAuth(token, refreshToken, customer, role)
    }
    window.addEventListener('auth:refreshed', handler)
    return () => window.removeEventListener('auth:refreshed', handler)
  }, [setAuth])

  useEffect(() => {
    if (user) loadCount(user.id)
  }, [])

  return (
    <BrowserRouter>
      <div style={{ minHeight: '100vh', background: '#f9fafb', fontFamily: "'Inter', system-ui, sans-serif" }}>
        <Navbar />
        <ErrorBoundary>
          <Routes>
            <Route path="/" element={<Navigate to="/catalog" replace />} />
            <Route path="/catalog" element={<CatalogPage />} />
            <Route path="/catalog/:id" element={<BookPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/cart" element={<PrivateRoute><CartPage /></PrivateRoute>} />
            <Route path="/profile" element={<PrivateRoute><ProfilePage /></PrivateRoute>} />
            <Route path="/admin" element={<AdminRoute><AdminPage /></AdminRoute>} />
            <Route path="*" element={<Navigate to="/catalog" replace />} />
          </Routes>
        </ErrorBoundary>
      </div>
    </BrowserRouter>
  )
}
