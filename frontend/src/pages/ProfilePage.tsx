import { useState, useEffect } from 'react'
import { useAuthStore } from '../store/authStore'
import { updateCustomer } from '../api/customer'
import { getMyOrders } from '../api/customer'
import { changePassword } from '../api/auth'
import OrderDetailModal from '../components/OrderDetailModal'
import type { Order } from '../types'

type Tab = 'profile' | 'orders' | 'password'

const STATUS_LABELS: Record<string, { label: string; color: string }> = {
  Placed: { label: 'Оформлен', color: '#2563eb' },
  Shipped: { label: 'Отправлен', color: '#d97706' },
  Delivered: { label: 'Доставлен', color: '#16a34a' },
  Cancelled: { label: 'Отменён', color: '#dc2626' },
}

export default function ProfilePage() {
  const { user, setAuth, token, refreshToken } = useAuthStore()
  const [tab, setTab] = useState<Tab>('profile')
  const [orders, setOrders] = useState<Order[]>([])
  const [ordersLoading, setOrdersLoading] = useState(false)
  const [selectedOrderId, setSelectedOrderId] = useState<number | null>(null)
  const [msg, setMsg] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  // Profile form
  const [form, setForm] = useState({
    name: user?.name ?? '',
    mail: user?.mail ?? '',
    phone: user?.phone ?? '',
    dateOfBirth: user?.dateOfBirth ?? '',
  })

  // Password form
  const [pwForm, setPwForm] = useState({ currentPassword: '', newPassword: '', confirm: '' })

  useEffect(() => {
    if (tab === 'orders' && user) {
      setOrdersLoading(true)
      getMyOrders(user.id)
        .then(setOrders)
        .catch(() => setOrders([]))
        .finally(() => setOrdersLoading(false))
    }
  }, [tab, user])

  const showMsg = (text: string, isError = false) => {
    if (isError) setError(text)
    else setMsg(text)
    setTimeout(() => { setMsg(null); setError(null) }, 3000)
  }

  const handleProfileSave = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!user) return
    try {
      const updated = await updateCustomer(user.id, form)
      setAuth(token!, refreshToken!, updated)
      showMsg('Профиль обновлён')
    } catch {
      showMsg('Ошибка при сохранении', true)
    }
  }

  const handlePasswordChange = async (e: React.FormEvent) => {
    e.preventDefault()
    if (pwForm.newPassword !== pwForm.confirm) {
      showMsg('Пароли не совпадают', true)
      return
    }
    try {
      await changePassword(pwForm.currentPassword, pwForm.newPassword)
      setPwForm({ currentPassword: '', newPassword: '', confirm: '' })
      showMsg('Пароль изменён')
    } catch {
      showMsg('Неверный текущий пароль', true)
    }
  }

  const activeOrders = orders.filter(o => o.status !== 'Delivered' && o.status !== 'Cancelled')
  const historyOrders = orders.filter(o => o.status === 'Delivered' || o.status === 'Cancelled')

  return (
    <div style={styles.page}>
      {msg && <div style={styles.toast}>{msg}</div>}
      {error && <div style={{ ...styles.toast, background: '#dc2626' }}>{error}</div>}
      {selectedOrderId !== null && (
        <OrderDetailModal orderId={selectedOrderId} onClose={() => setSelectedOrderId(null)} />
      )}

      <h1 style={styles.title}>Личный кабинет</h1>
      <div style={styles.layout}>
        <div style={styles.sidebar}>
          {(['profile', 'orders', 'password'] as Tab[]).map(t => (
            <button
              key={t}
              style={{ ...styles.tabBtn, ...(tab === t ? styles.tabBtnActive : {}) }}
              onClick={() => setTab(t)}
            >
              {t === 'profile' ? '👤 Профиль' : t === 'orders' ? '📦 Заказы' : '🔒 Пароль'}
            </button>
          ))}
        </div>

        <div style={styles.content}>
          {tab === 'profile' && (
            <form onSubmit={handleProfileSave} style={styles.form}>
              <h2 style={styles.sectionTitle}>Данные профиля</h2>
              {[
                { label: 'Имя', field: 'name', type: 'text' },
                { label: 'Email', field: 'mail', type: 'email' },
                { label: 'Телефон', field: 'phone', type: 'tel' },
                { label: 'Дата рождения', field: 'dateOfBirth', type: 'date' },
              ].map(({ label, field, type }) => (
                <div key={field} style={styles.fieldGroup}>
                  <label style={styles.label}>{label}</label>
                  <input
                    type={type}
                    value={form[field as keyof typeof form]}
                    onChange={e => setForm(prev => ({ ...prev, [field]: e.target.value }))}
                    style={styles.input}
                  />
                </div>
              ))}
              <button type="submit" style={styles.saveBtn}>Сохранить</button>
            </form>
          )}

          {tab === 'orders' && (
            <div>
              <h2 style={styles.sectionTitle}>Мои заказы</h2>
              {ordersLoading ? (
                <div style={styles.loading}>Загрузка...</div>
              ) : orders.length === 0 ? (
                <div style={styles.empty}>Заказов пока нет</div>
              ) : (
                <>
                  {activeOrders.length > 0 && (
                    <>
                      <h3 style={styles.subTitle}>Активные заказы</h3>
                      {activeOrders.map(order => <OrderCard key={order.id} order={order} onDetail={setSelectedOrderId} />)}
                    </>
                  )}
                  {historyOrders.length > 0 && (
                    <>
                      <h3 style={styles.subTitle}>История заказов</h3>
                      {historyOrders.map(order => <OrderCard key={order.id} order={order} onDetail={setSelectedOrderId} />)}
                    </>
                  )}
                </>
              )}
            </div>
          )}

          {tab === 'password' && (
            <form onSubmit={handlePasswordChange} style={styles.form}>
              <h2 style={styles.sectionTitle}>Смена пароля</h2>
              {[
                { label: 'Текущий пароль', field: 'currentPassword' },
                { label: 'Новый пароль', field: 'newPassword' },
                { label: 'Подтвердите новый пароль', field: 'confirm' },
              ].map(({ label, field }) => (
                <div key={field} style={styles.fieldGroup}>
                  <label style={styles.label}>{label}</label>
                  <input
                    type="password"
                    value={pwForm[field as keyof typeof pwForm]}
                    onChange={e => setPwForm(prev => ({ ...prev, [field]: e.target.value }))}
                    style={styles.input}
                    required
                  />
                </div>
              ))}
              <button type="submit" style={styles.saveBtn}>Изменить пароль</button>
            </form>
          )}
        </div>
      </div>
    </div>
  )
}

function OrderCard({ order, onDetail }: { order: Order; onDetail: (id: number) => void }) {
  const status = STATUS_LABELS[order.status] ?? { label: order.status, color: '#6b7280' }
  return (
    <div style={cardStyles.card}>
      <div style={cardStyles.header}>
        <span style={cardStyles.id}>Заказ #{order.id}</span>
        <span style={{ ...cardStyles.status, color: status.color, borderColor: status.color }}>
          {status.label}
        </span>
        <span style={cardStyles.date}>{new Date(order.createdDate).toLocaleDateString('ru-RU')}</span>
        <span style={cardStyles.total}>{order.totalPrice.toLocaleString('ru-RU')} ₽</span>
        <button style={cardStyles.detailBtn} onClick={() => onDetail(order.id)}>Подробнее</button>
      </div>
      <div style={cardStyles.items}>
        {order.items?.map((item, i) => (
          <span key={i} style={cardStyles.item}>
            {item.count} × {item.priceAtPurchase.toLocaleString('ru-RU')} ₽
          </span>
        ))}
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  page: { maxWidth: 1000, margin: '0 auto', padding: '32px 24px' },
  title: { fontSize: 28, fontWeight: 700, color: '#111827', marginBottom: 32 },
  layout: { display: 'flex', gap: 32, alignItems: 'flex-start', flexWrap: 'wrap' },
  sidebar: { width: 200, display: 'flex', flexDirection: 'column', gap: 4, flexShrink: 0 },
  tabBtn: { background: 'none', border: 'none', borderRadius: 8, padding: '10px 16px', cursor: 'pointer', fontSize: 14, textAlign: 'left', color: '#374151' },
  tabBtnActive: { background: '#eff6ff', color: '#2563eb', fontWeight: 600 },
  content: { flex: 1, minWidth: 280 },
  form: { display: 'flex', flexDirection: 'column', gap: 16, maxWidth: 480 },
  sectionTitle: { fontSize: 20, fontWeight: 600, color: '#111827', marginBottom: 20, marginTop: 0 },
  subTitle: { fontSize: 16, fontWeight: 600, color: '#374151', marginBottom: 12, marginTop: 24 },
  fieldGroup: { display: 'flex', flexDirection: 'column', gap: 4 },
  label: { fontSize: 13, fontWeight: 500, color: '#374151' },
  input: { border: '1px solid #d1d5db', borderRadius: 8, padding: '10px 14px', fontSize: 14, outline: 'none' },
  saveBtn: { background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '11px 24px', fontSize: 14, cursor: 'pointer', fontWeight: 600, alignSelf: 'flex-start' },
  loading: { color: '#6b7280', padding: '20px 0' },
  empty: { color: '#9ca3af', padding: '20px 0' },
  toast: { position: 'fixed', bottom: 24, right: 24, background: '#16a34a', color: '#fff', padding: '12px 20px', borderRadius: 8, fontSize: 14, zIndex: 999 },
}

const cardStyles: Record<string, React.CSSProperties> = {
  card: { border: '1px solid #e5e7eb', borderRadius: 10, padding: '16px 20px', marginBottom: 12 },
  header: { display: 'flex', alignItems: 'center', gap: 16, flexWrap: 'wrap', marginBottom: 8 },
  id: { fontWeight: 600, fontSize: 14, color: '#111827' },
  status: { fontSize: 12, border: '1px solid', borderRadius: 20, padding: '2px 10px' },
  date: { fontSize: 13, color: '#9ca3af' },
  total: { fontWeight: 700, fontSize: 15, marginLeft: 'auto' },
  items: { display: 'flex', flexWrap: 'wrap', gap: 8 },
  item: { fontSize: 12, color: '#6b7280', background: '#f9fafb', padding: '3px 10px', borderRadius: 6 },
  detailBtn: { background: 'none', border: '1px solid #2563eb', color: '#2563eb', borderRadius: 6, padding: '4px 12px', fontSize: 12, cursor: 'pointer', fontWeight: 500 },
}
