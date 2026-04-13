import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { login } from '../api/auth'
import { useAuthStore } from '../store/authStore'

type Mode = 'customer' | 'admin'

export default function LoginPage() {
  const { setAuth } = useAuthStore()
  const navigate = useNavigate()
  const [mode, setMode] = useState<Mode>('customer')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const data = await login(email, password)
      if (mode === 'admin' && data.role !== 'Admin') {
        setError('У этого аккаунта нет прав сотрудника')
        return
      }
      setAuth(data.token, data.refreshToken, data.customer, data.role)
      navigate(data.role === 'Admin' ? '/admin' : '/catalog')
    } catch {
      setError('Неверный email или пароль')
    } finally {
      setLoading(false)
    }
  }

  const isAdmin = mode === 'admin'

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        {/* Mode toggle */}
        <div style={styles.toggle}>
          <button
            type="button"
            onClick={() => setMode('customer')}
            style={{ ...styles.toggleBtn, ...(mode === 'customer' ? styles.toggleActive : {}) }}
          >
            👤 Покупатель
          </button>
          <button
            type="button"
            onClick={() => setMode('admin')}
            style={{ ...styles.toggleBtn, ...(mode === 'admin' ? { ...styles.toggleActive, ...styles.toggleActiveAdmin } : {}) }}
          >
            ⚙️ Сотрудник
          </button>
        </div>

        <h1 style={styles.title}>
          {isAdmin ? 'Вход для сотрудников' : 'Вход'}
        </h1>

        {isAdmin && (
          <div style={styles.adminHint}>
            Доступ только для авторизованных сотрудников магазина
          </div>
        )}

        {error && <div style={styles.error}>{error}</div>}

        <form onSubmit={handleSubmit} style={styles.form}>
          <label style={styles.label}>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            style={styles.input}
            required
            autoFocus
          />
          <label style={styles.label}>Пароль</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={styles.input}
            required
          />
          <button
            type="submit"
            style={{ ...styles.btn, ...(isAdmin ? styles.btnAdmin : {}) }}
            disabled={loading}
          >
            {loading ? 'Вход...' : isAdmin ? 'Войти как сотрудник' : 'Войти'}
          </button>
        </form>

        <div style={styles.footer}>
          {!isAdmin && (
            <>Нет аккаунта? <Link to="/register" style={styles.link}>Зарегистрироваться</Link></>
          )}
        </div>
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  page: { minHeight: '80vh', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 24 },
  card: { background: '#fff', border: '1px solid #e5e7eb', borderRadius: 12, padding: '36px 36px 32px', width: '100%', maxWidth: 400 },
  toggle: { display: 'flex', background: '#f3f4f6', borderRadius: 10, padding: 4, marginBottom: 24, gap: 4 },
  toggleBtn: { flex: 1, background: 'none', border: 'none', borderRadius: 7, padding: '8px 0', fontSize: 14, cursor: 'pointer', color: '#6b7280', fontWeight: 500, transition: 'all .15s' },
  toggleActive: { background: '#fff', color: '#2563eb', fontWeight: 600, boxShadow: '0 1px 4px rgba(0,0,0,0.08)' },
  toggleActiveAdmin: { color: '#7c3aed' },
  title: { fontSize: 22, fontWeight: 700, color: '#111827', marginBottom: 8, marginTop: 0, textAlign: 'center' },
  adminHint: { background: '#f5f3ff', border: '1px solid #ddd6fe', color: '#7c3aed', borderRadius: 8, padding: '8px 12px', fontSize: 13, marginBottom: 16, textAlign: 'center' },
  error: { background: '#fef2f2', border: '1px solid #fecaca', color: '#dc2626', borderRadius: 8, padding: '10px 14px', fontSize: 14, marginBottom: 16 },
  form: { display: 'flex', flexDirection: 'column', gap: 4 },
  label: { fontSize: 13, fontWeight: 500, color: '#374151', marginBottom: 4, marginTop: 12 },
  input: { border: '1px solid #d1d5db', borderRadius: 8, padding: '10px 14px', fontSize: 14, outline: 'none' },
  btn: { marginTop: 20, background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '12px 0', fontSize: 15, cursor: 'pointer', fontWeight: 600 },
  btnAdmin: { background: '#7c3aed' },
  footer: { textAlign: 'center', marginTop: 20, fontSize: 14, color: '#6b7280' },
  link: { color: '#2563eb', textDecoration: 'none', fontWeight: 500 },
}
