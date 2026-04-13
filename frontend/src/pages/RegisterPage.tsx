import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../api/auth'
import { useAuthStore } from '../store/authStore'

export default function RegisterPage() {
  const { setAuth } = useAuthStore()
  const navigate = useNavigate()
  const [form, setForm] = useState({ name: '', email: '', password: '', phone: '', dateOfBirth: '' })
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const setField = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm(prev => ({ ...prev, [field]: e.target.value }))

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const data = await register(form)
      setAuth(data.token, data.refreshToken, data.customer, data.role)
      navigate('/catalog')
    } catch {
      setError('Ошибка регистрации. Возможно, email уже занят.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <h1 style={styles.title}>Регистрация</h1>
        {error && <div style={styles.error}>{error}</div>}
        <form onSubmit={handleSubmit} style={styles.form}>
          {[
            { label: 'Имя', field: 'name', type: 'text' },
            { label: 'Email', field: 'email', type: 'email' },
            { label: 'Пароль', field: 'password', type: 'password' },
            { label: 'Телефон', field: 'phone', type: 'tel' },
            { label: 'Дата рождения', field: 'dateOfBirth', type: 'date' },
          ].map(({ label, field, type }) => (
            <div key={field}>
              <label style={styles.label}>{label}</label>
              <input
                type={type}
                value={form[field as keyof typeof form]}
                onChange={setField(field)}
                style={styles.input}
                required
              />
            </div>
          ))}
          <button type="submit" style={styles.btn} disabled={loading}>
            {loading ? 'Регистрация...' : 'Зарегистрироваться'}
          </button>
        </form>
        <div style={styles.footer}>
          Уже есть аккаунт? <Link to="/login" style={styles.link}>Войти</Link>
        </div>
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  page: { minHeight: '80vh', display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 24 },
  card: { background: '#fff', border: '1px solid #e5e7eb', borderRadius: 12, padding: '40px 36px', width: '100%', maxWidth: 420 },
  title: { fontSize: 24, fontWeight: 700, color: '#111827', marginBottom: 24, marginTop: 0, textAlign: 'center' },
  error: { background: '#fef2f2', border: '1px solid #fecaca', color: '#dc2626', borderRadius: 8, padding: '10px 14px', fontSize: 14, marginBottom: 16 },
  form: { display: 'flex', flexDirection: 'column', gap: 12 },
  label: { fontSize: 13, fontWeight: 500, color: '#374151', display: 'block', marginBottom: 4 },
  input: { width: '100%', border: '1px solid #d1d5db', borderRadius: 8, padding: '10px 14px', fontSize: 14, outline: 'none', boxSizing: 'border-box' },
  btn: { marginTop: 8, background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '12px 0', fontSize: 15, cursor: 'pointer', fontWeight: 600 },
  footer: { textAlign: 'center', marginTop: 20, fontSize: 14, color: '#6b7280' },
  link: { color: '#2563eb', textDecoration: 'none', fontWeight: 500 },
}
