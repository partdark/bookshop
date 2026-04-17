import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useEffect } from 'react'
import { useAuthStore } from '../store/authStore'
import { useCartStore } from '../store/cartStore'
import { logout as apiLogout } from '../api/auth'

export default function Navbar() {
  const { user, logout, isAuthenticated, isAdmin } = useAuthStore()
  const { count, loadCount } = useCartStore()
  const navigate = useNavigate()
  const location = useLocation()

  
  useEffect(() => {
    if (user) loadCount(user.id)
  }, [user])

  const handleLogout = async () => {
    await apiLogout()
    logout()
    navigate('/')
  }

  const isActive = (path: string) =>
    location.pathname === path || location.pathname.startsWith(path + '/')

  return (
    <nav style={styles.nav}>
      <div style={styles.inner}>
        <Link to="/" style={styles.logo}>📚 Книжный магазин</Link>
        <div style={styles.links}>
          <Link to="/catalog" style={{ ...styles.link, ...(isActive('/catalog') ? styles.activeLink : {}) }}>
            Каталог
          </Link>
          {isAuthenticated() && (
            <Link to="/cart" style={{ ...styles.link, ...(isActive('/cart') ? styles.activeLink : {}) }}>
              Корзина
              {count > 0 && <span style={styles.badge}>{count}</span>}
            </Link>
          )}
          {isAdmin() && (
            <Link to="/admin" style={{ ...styles.link, ...(isActive('/admin') ? styles.activeLink : {}) }}>
              ⚙️ Админ
            </Link>
          )}
          {isAuthenticated() ? (
            <>
              <Link to="/profile" style={{ ...styles.link, ...(isActive('/profile') ? styles.activeLink : {}) }}>
                {user?.name}
              </Link>
              <button onClick={handleLogout} style={styles.logoutBtn}>Выйти</button>
            </>
          ) : (
            <Link to="/login" style={{ ...styles.link, ...(isActive('/login') ? styles.activeLink : {}) }}>
              Войти
            </Link>
          )}
        </div>
      </div>
    </nav>
  )
}

const styles: Record<string, React.CSSProperties> = {
  nav: { background: '#fff', borderBottom: '1px solid #e5e7eb', position: 'sticky', top: 0, zIndex: 100 },
  inner: { maxWidth: 1200, margin: '0 auto', padding: '0 24px', height: 60, display: 'flex', alignItems: 'center', justifyContent: 'space-between' },
  logo: { fontSize: 20, fontWeight: 700, color: '#1e3a5f', textDecoration: 'none' },
  links: { display: 'flex', alignItems: 'center', gap: 24 },
  link: { color: '#374151', textDecoration: 'none', fontSize: 15, fontWeight: 500, position: 'relative', display: 'inline-flex', alignItems: 'center', gap: 6 },
  activeLink: { color: '#2563eb', borderBottom: '2px solid #2563eb', paddingBottom: 2 },
  badge: {
    background: '#dc2626', color: '#fff', borderRadius: '50%',
    fontSize: 11, fontWeight: 700, minWidth: 18, height: 18,
    display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
    padding: '0 4px',
  },
  logoutBtn: { background: 'none', border: '1px solid #e5e7eb', borderRadius: 6, padding: '6px 14px', cursor: 'pointer', fontSize: 14, color: '#6b7280' },
}
