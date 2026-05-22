import { useState, useEffect } from 'react'
import { getAllOrders, updateOrderStatus, deleteBook, addBook, updateBookCount, registerAdmin, getAuthors, addAuthor, addGenre } from '../api/admin'
import { getCatalog, getGenres } from '../api/books'
import { getOrdersCount, getMoneyByType } from '../api/reports'
import OrderDetailModal from '../components/OrderDetailModal'
import PieChart from '../components/PieChart'
import type { Order, BookListItem } from '../types'

type Tab = 'orders' | 'books' | 'addBook' | 'addAuthor' | 'addGenre' | 'addAdmin' | 'reports'

const ORDER_STATUSES = ['Placed', 'Shipped', 'Delivered', 'Cancelled']
const STATUS_LABELS: Record<string, { label: string; color: string }> = {
  Placed:    { label: 'Оформлен',  color: '#2563eb' },
  Shipped:   { label: 'Отправлен', color: '#d97706' },
  Delivered: { label: 'Доставлен', color: '#16a34a' },
  Cancelled: { label: 'Отменён',   color: '#dc2626' },
}

export default function AdminPage() {
  const [tab, setTab] = useState<Tab>('orders')
  const [toast, setToast] = useState<{ msg: string; ok: boolean } | null>(null)

  const showToast = (msg: string, ok = true) => {
    setToast({ msg, ok })
    setTimeout(() => setToast(null), 3000)
  }

  return (
    <div style={s.page}>
      {toast && (
        <div style={{ ...s.toast, background: toast.ok ? '#16a34a' : '#dc2626' }}>{toast.msg}</div>
      )}
      <h1 style={s.title}>Панель администратора</h1>
      <div style={s.layout}>
        <div style={s.sidebar}>
          {([
            ['orders',    '📦 Заказы'],
            ['books',     '📚 Книги'],
            ['addBook',   '➕ Добавить книгу'],
            ['addAuthor', '✍️ Добавить автора'],
            ['addGenre',  '🏷️ Добавить жанр'],
            ['addAdmin',  '👤 Добавить сотрудника'],
            ['reports',   '📊 Отчеты'],
          ] as [Tab, string][]).map(([t, label]) => (
            <button key={t} style={{ ...s.tabBtn, ...(tab === t ? s.tabActive : {}) }} onClick={() => setTab(t)}>
              {label}
            </button>
          ))}
        </div>
        <div style={s.content}>
          {tab === 'orders'    && <OrdersTab showToast={showToast} />}
          {tab === 'books'     && <BooksTab showToast={showToast} />}
          {tab === 'addBook'   && <AddBookTab showToast={showToast} />}
          {tab === 'addAuthor' && <AddAuthorTab showToast={showToast} />}
          {tab === 'addGenre'  && <AddGenreTab showToast={showToast} />}
          {tab === 'addAdmin'  && <AddAdminTab showToast={showToast} />}
          {tab === 'reports'   && <ReportsTab showToast={showToast} />}
        </div>
      </div>
    </div>
  )
}


function OrdersTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [orders, setOrders] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)
  const [updating, setUpdating] = useState<number | null>(null)
  const [detailId, setDetailId] = useState<number | null>(null)

  useEffect(() => {
    getAllOrders()
      .then(setOrders)
      .catch(() => showToast('Ошибка загрузки заказов', false))
      .finally(() => setLoading(false))
  }, [])

  const handleStatus = async (id: number, status: string) => {
    setUpdating(id)
    try {
      await updateOrderStatus(id, status)
      setOrders(prev => prev.map(o => o.id === id ? { ...o, status } : o))
      showToast('Статус обновлён')
    } catch {
      showToast('Ошибка обновления статуса', false)
    } finally {
      setUpdating(null)
    }
  }

  if (loading) return <div style={s.empty}>Загрузка...</div>

  return (
    <div>
      {detailId !== null && (
        <OrderDetailModal orderId={detailId} onClose={() => setDetailId(null)} />
      )}
      <h2 style={s.sectionTitle}>Все заказы ({orders.length})</h2>
      {orders.length === 0 && <div style={s.empty}>Заказов нет</div>}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {orders.map(order => {
          const st = STATUS_LABELS[order.status] ?? { label: order.status, color: '#6b7280' }
          return (
            <div key={order.id} style={s.card}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12, flexWrap: 'wrap' }}>
                <span style={{ fontWeight: 600, fontSize: 14 }}>Заказ #{order.id}</span>
                <span style={{ fontSize: 12, border: `1px solid ${st.color}`, color: st.color, borderRadius: 20, padding: '2px 10px' }}>{st.label}</span>
                <span style={{ fontSize: 13, color: '#9ca3af' }}>{new Date(order.createdDate).toLocaleDateString('ru-RU')}</span>
                <span style={{ fontWeight: 700, marginLeft: 'auto' }}>{order.totalPrice.toLocaleString('ru-RU')} ₽</span>
                <button style={s.detailBtn} onClick={() => setDetailId(order.id)}>Подробнее</button>
              </div>
              <div style={{ marginTop: 10, display: 'flex', alignItems: 'center', gap: 8, flexWrap: 'wrap' }}>
                <span style={{ fontSize: 13, color: '#6b7280' }}>Статус:</span>
                {ORDER_STATUSES.map(st => (
                  <button
                    key={st}
                    disabled={order.status === st || updating === order.id}
                    onClick={() => handleStatus(order.id, st)}
                    style={{
                      ...s.statusBtn,
                      ...(order.status === st ? s.statusBtnActive : {}),
                    }}
                  >
                    {STATUS_LABELS[st]?.label ?? st}
                  </button>
                ))}
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}


function BooksTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [books, setBooks] = useState<BookListItem[]>([])
  const [loading, setLoading] = useState(true)
  const [editCount, setEditCount] = useState<Record<string, number>>({})
  const [saving, setSaving] = useState<string | null>(null)

  useEffect(() => {
    getCatalog({ pageCapacity: 100, countMoreThenZero: false })
      .then(r => setBooks(r.books))
      .catch(() => showToast('Ошибка загрузки книг', false))
      .finally(() => setLoading(false))
  }, [])

  const handleDeleteBook = async (id: string, title: string) => {
    if (!confirm(`Удалить книгу "${title}"?`)) return
    try {
      await deleteBook(id)
      setBooks(prev => prev.filter(b => b.id !== id))
      showToast('Книга удалена')
    } catch {
      showToast('Ошибка удаления', false)
    }
  }

  const handleSaveCount = async (id: string) => {
    const count = editCount[id]
    if (count === undefined) return
    setSaving(id)
    try {
      await updateBookCount(id, count)
      setBooks(prev => prev.map(b => b.id === id ? { ...b, count } : b))
      setEditCount(prev => { const n = { ...prev }; delete n[id]; return n })
      showToast('Количество обновлено')
    } catch {
      showToast('Ошибка обновления', false)
    } finally {
      setSaving(null)
    }
  }

  if (loading) return <div style={s.empty}>Загрузка...</div>

  return (
    <div>
      <h2 style={s.sectionTitle}>Книги ({books.length})</h2>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
        {books.map(book => (
          <div key={book.id} style={s.card}>
            <div style={{ display: 'flex', gap: 12, alignItems: 'flex-start' }}>
              {book.urlImage && <img src={book.urlImage} alt={book.title} style={{ width: 40, height: 56, objectFit: 'cover', borderRadius: 4, flexShrink: 0 }} />}
              <div style={{ flex: 1 }}>
                <div style={{ fontWeight: 600, fontSize: 14, marginBottom: 4 }}>{book.title}</div>
                <div style={{ fontSize: 12, color: '#6b7280', marginBottom: 8 }}>
                  {book.authors.map(a => a.name).join(', ')} · {book.publicationYear} · {book.price.toLocaleString('ru-RU')} ₽
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8, flexWrap: 'wrap' }}>
                  <span style={{ fontSize: 13, color: '#374151' }}>Кол-во:</span>
                  <input
                    type="number"
                    min={0}
                    value={editCount[book.id] ?? book.count}
                    onChange={e => setEditCount(prev => ({ ...prev, [book.id]: +e.target.value }))}
                    style={{ ...s.input, width: 70, padding: '4px 8px' }}
                  />
                  {editCount[book.id] !== undefined && (
                    <button
                      onClick={() => handleSaveCount(book.id)}
                      disabled={saving === book.id}
                      style={s.saveBtn}
                    >
                      {saving === book.id ? '...' : 'Сохранить'}
                    </button>
                  )}
                  <button onClick={() => handleDeleteBook(book.id, book.title)} style={s.deleteBtn}>
                    Удалить книгу
                  </button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}


function AddBookTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [genres, setGenres] = useState<{ id: string; name: string }[]>([])
  const [authors, setAuthors] = useState<{ id: string; name: string; year: number }[]>([])
  const [form, setForm] = useState({
    title: '', description: '', price: '', urlImage: '',
    count: '', publicationYear: new Date().getFullYear().toString(),
  })
  const [selectedAuthors, setSelectedAuthors] = useState<string[]>([])
  const [selectedGenres, setSelectedGenres] = useState<string[]>([])
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    getGenres().then(setGenres).catch(() => {})
    getAuthors().then(setAuthors).catch(() => {})
  }, [])

  const toggleItem = (id: string, list: string[], setList: (v: string[]) => void) => {
    setList(list.includes(id) ? list.filter(x => x !== id) : [...list, id])
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await addBook({
        title: form.title,
        description: form.description,
        price: parseFloat(form.price),
        urlImage: form.urlImage,
        count: parseInt(form.count),
        publicationYear: parseInt(form.publicationYear),
        authorsIds: selectedAuthors,
        genresIds: selectedGenres,
      })
      showToast('Книга добавлена')
      setForm({ title: '', description: '', price: '', urlImage: '', count: '', publicationYear: new Date().getFullYear().toString() })
      setSelectedAuthors([])
      setSelectedGenres([])
    } catch {
      showToast('Ошибка добавления книги', false)
    } finally {
      setLoading(false)
    }
  }

  const field = (label: string, key: keyof typeof form, type = 'text') => (
    <div style={s.fieldGroup}>
      <label style={s.label}>{label}</label>
      <input
        type={type}
        value={form[key]}
        onChange={e => setForm(prev => ({ ...prev, [key]: e.target.value }))}
        style={s.input}
        required
      />
    </div>
  )

  return (
    <form onSubmit={handleSubmit} style={{ maxWidth: 560 }}>
      <h2 style={s.sectionTitle}>Добавить книгу</h2>
      {field('Название', 'title')}
      <div style={s.fieldGroup}>
        <label style={s.label}>Описание</label>
        <textarea
          value={form.description}
          onChange={e => setForm(prev => ({ ...prev, description: e.target.value }))}
          style={{ ...s.input, minHeight: 80, resize: 'vertical' }}
        />
      </div>
      {field('Цена (₽)', 'price', 'number')}
      {field('Количество', 'count', 'number')}
      {field('Год издания', 'publicationYear', 'number')}
      {field('URL обложки', 'urlImage')}

      <div style={s.fieldGroup}>
        <label style={s.label}>Авторы</label>
        <div style={s.chipList}>
          {authors.map(a => (
            <button type="button" key={a.id}
              onClick={() => toggleItem(a.id, selectedAuthors, setSelectedAuthors)}
              style={{ ...s.chip, ...(selectedAuthors.includes(a.id) ? s.chipActive : {}) }}>
              {a.name}
            </button>
          ))}
        </div>
      </div>

      <div style={s.fieldGroup}>
        <label style={s.label}>Жанры</label>
        <div style={s.chipList}>
          {genres.map(g => (
            <button type="button" key={g.id}
              onClick={() => toggleItem(g.id, selectedGenres, setSelectedGenres)}
              style={{ ...s.chip, ...(selectedGenres.includes(g.id) ? s.chipActive : {}) }}>
              {g.name}
            </button>
          ))}
        </div>
      </div>

      <button type="submit" style={{ ...s.saveBtn, marginTop: 16, padding: '10px 24px', fontSize: 14 }} disabled={loading}>
        {loading ? 'Добавление...' : 'Добавить книгу'}
      </button>
    </form>
  )
}


function AddAuthorTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [name, setName] = useState('')
  const [year, setYear] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await addAuthor(name, parseInt(year))
      showToast('Автор добавлен')
      setName('')
      setYear('')
    } catch {
      showToast('Ошибка добавления автора', false)
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ maxWidth: 400 }}>
      <h2 style={s.sectionTitle}>Добавить автора</h2>
      <div style={s.fieldGroup}>
        <label style={s.label}>Имя автора</label>
        <input value={name} onChange={e => setName(e.target.value)} style={s.input} required />
      </div>
      <div style={s.fieldGroup}>
        <label style={s.label}>Год рождения</label>
        <input type="number" value={year} onChange={e => setYear(e.target.value)} style={s.input} required min={0} max={2100} />
      </div>
      <button type="submit" style={{ ...s.saveBtn, marginTop: 8, padding: '10px 24px', fontSize: 14 }} disabled={loading}>
        {loading ? 'Добавление...' : 'Добавить автора'}
      </button>
    </form>
  )
}


function AddGenreTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [name, setName] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await addGenre(name)
      showToast('Жанр добавлен')
      setName('')
    } catch {
      showToast('Ошибка добавления жанра', false)
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ maxWidth: 400 }}>
      <h2 style={s.sectionTitle}>Добавить жанр</h2>
      <div style={s.fieldGroup}>
        <label style={s.label}>Название жанра</label>
        <input value={name} onChange={e => setName(e.target.value)} style={s.input} required maxLength={100} />
      </div>
      <button type="submit" style={{ ...s.saveBtn, marginTop: 8, padding: '10px 24px', fontSize: 14 }} disabled={loading}>
        {loading ? 'Добавление...' : 'Добавить жанр'}
      </button>
    </form>
  )
}


function AddAdminTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [form, setForm] = useState({
    name: '', email: '', password: '', phone: '', dateOfBirth: '',
  })
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      await registerAdmin(form)
      showToast('Сотрудник зарегистрирован')
      setForm({ name: '', email: '', password: '', phone: '', dateOfBirth: '' })
    } catch {
      showToast('Ошибка регистрации. Email уже занят?', false)
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} style={{ maxWidth: 440 }}>
      <h2 style={s.sectionTitle}>Добавить сотрудника (Admin)</h2>
      {([
        ['Имя', 'name', 'text'],
        ['Email', 'email', 'email'],
        ['Пароль', 'password', 'password'],
        ['Телефон', 'phone', 'tel'],
        ['Дата рождения', 'dateOfBirth', 'date'],
      ] as [string, keyof typeof form, string][]).map(([label, key, type]) => (
        <div key={key} style={s.fieldGroup}>
          <label style={s.label}>{label}</label>
          <input
            type={type}
            value={form[key]}
            onChange={e => setForm(prev => ({ ...prev, [key]: e.target.value }))}
            style={s.input}
            required
          />
        </div>
      ))}
      <button type="submit" style={{ ...s.saveBtn, marginTop: 16, padding: '10px 24px', fontSize: 14 }} disabled={loading}>
        {loading ? 'Регистрация...' : 'Зарегистрировать'}
      </button>
    </form>
  )
}


function ReportsTab({ showToast }: { showToast: (m: string, ok?: boolean) => void }) {
  const [ordersData, setOrdersData] = useState<{ label: string; value: number; color: string }[]>([])
  const [moneyData, setMoneyData] = useState<{ label: string; value: number; color: string }[]>([])
  const [loading, setLoading] = useState(true)
  const [dateRange, setDateRange] = useState({
    startDate: '',
    endDate: '',
  })

  useEffect(() => {
    loadReports()
  }, [])

  const loadReports = async () => {
    setLoading(true)
    try {
      const formatDate = (date: string) => {
        const [year, month, day] = date.split('-')
        return `${day}.${month}.${year}`
      }
      
      const startDate = dateRange.startDate ? formatDate(dateRange.startDate) : undefined
      const endDate = dateRange.endDate ? formatDate(dateRange.endDate) : undefined
      
      const [ordersRes, moneyRes] = await Promise.all([
        getOrdersCount(startDate, endDate),
        getMoneyByType(startDate, endDate),
      ])
      
      console.log('Orders response:', ordersRes)
      console.log('Money response:', moneyRes)

      if (!Array.isArray(ordersRes) || !Array.isArray(moneyRes)) {
        showToast('Неверный формат данных от сервера', false)
        return
      }

      const colors = ['#2563eb', '#d97706', '#16a34a', '#dc2626', '#9333ea', '#0891b2', '#ea580c', '#15803d']

      setOrdersData(ordersRes.map((item, i) => ({
        label: item.name,
        value: item.count,
        color: colors[i % colors.length],
      })))

      setMoneyData(moneyRes.map((item, i) => ({
        label: item.name,
        value: Math.round(item.totalMoney),
        color: colors[i % colors.length],
      })))
    } catch (err: any) {
      console.error('Reports error:', err)
      showToast(`Ошибка загрузки отчетов: ${err?.response?.data || err.message}`, false)
    } finally {
      setLoading(false)
    }
  }

  const handleDateChange = (key: 'startDate' | 'endDate', value: string) => {
    setDateRange(prev => ({ ...prev, [key]: value }))
  }

  if (loading) return <div style={s.empty}>Загрузка...</div>

  return (
    <div>
      <h2 style={s.sectionTitle}>Отчеты</h2>
      <div style={{ ...s.fieldGroup, maxWidth: 400 }}>
        <label style={s.label}>Период (опционально)</label>
        <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}>
          <input
            type="date"
            value={dateRange.startDate}
            onChange={e => handleDateChange('startDate', e.target.value)}
            style={{ ...s.input, flex: 1 }}
          />
          <input
            type="date"
            value={dateRange.endDate}
            onChange={e => handleDateChange('endDate', e.target.value)}
            style={{ ...s.input, flex: 1 }}
          />
        </div>
        <button
          onClick={loadReports}
          style={{ ...s.saveBtn, marginTop: 8, padding: '8px 16px', fontSize: 14 }}
        >
          Обновить
        </button>
      </div>

      <div style={{ display: 'flex', gap: 32, flexWrap: 'wrap', marginTop: 24 }}>
        <div style={{ background: '#fff', padding: 20, borderRadius: 12, boxShadow: '0 1px 3px rgba(0,0,0,0.1)' }}>
          <PieChart
            title="Количество заказов по типам"
            data={ordersData}
          />
        </div>
        <div style={{ background: '#fff', padding: 20, borderRadius: 12, boxShadow: '0 1px 3px rgba(0,0,0,0.1)' }}>
          <PieChart
            title="Сумма денег по типам (₽)"
            data={moneyData}
          />
        </div>
      </div>
    </div>
  )
}


const s: Record<string, React.CSSProperties> = {
  page: { maxWidth: 1100, margin: '0 auto', padding: '32px 24px' },
  title: { fontSize: 28, fontWeight: 700, color: '#111827', marginBottom: 32 },
  layout: { display: 'flex', gap: 32, alignItems: 'flex-start', flexWrap: 'wrap' },
  sidebar: { width: 210, display: 'flex', flexDirection: 'column', gap: 4, flexShrink: 0 },
  tabBtn: { background: 'none', border: 'none', borderRadius: 8, padding: '10px 16px', cursor: 'pointer', fontSize: 14, textAlign: 'left', color: '#374151' },
  tabActive: { background: '#eff6ff', color: '#2563eb', fontWeight: 600 },
  content: { flex: 1, minWidth: 300 },
  sectionTitle: { fontSize: 20, fontWeight: 600, color: '#111827', marginBottom: 20, marginTop: 0 },
  card: { border: '1px solid #e5e7eb', borderRadius: 10, padding: '14px 18px' },
  empty: { color: '#9ca3af', padding: '20px 0' },
  toast: { position: 'fixed', bottom: 24, right: 24, color: '#fff', padding: '12px 20px', borderRadius: 8, fontSize: 14, zIndex: 999 },
  statusBtn: { background: '#f9fafb', border: '1px solid #e5e7eb', borderRadius: 6, padding: '4px 12px', fontSize: 12, cursor: 'pointer', color: '#374151' },
  statusBtnActive: { background: '#eff6ff', border: '1px solid #2563eb', color: '#2563eb', fontWeight: 600 },
  detailBtn: { background: 'none', border: '1px solid #2563eb', color: '#2563eb', borderRadius: 6, padding: '3px 12px', fontSize: 12, cursor: 'pointer', fontWeight: 500 },
  fieldGroup: { display: 'flex', flexDirection: 'column', gap: 4, marginBottom: 12 },
  label: { fontSize: 13, fontWeight: 500, color: '#374151' },
  input: { border: '1px solid #d1d5db', borderRadius: 8, padding: '9px 12px', fontSize: 14, outline: 'none' },
  saveBtn: { background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '6px 14px', fontSize: 13, cursor: 'pointer', fontWeight: 600 },
  deleteBtn: { background: '#fef2f2', color: '#dc2626', border: '1px solid #fecaca', borderRadius: 8, padding: '6px 14px', fontSize: 13, cursor: 'pointer' },
  chipList: { display: 'flex', flexWrap: 'wrap', gap: 6, marginTop: 4 },
  chip: { background: '#f3f4f6', border: '1px solid #e5e7eb', borderRadius: 20, padding: '4px 12px', fontSize: 12, cursor: 'pointer', color: '#374151' },
  chipActive: { background: '#eff6ff', border: '1px solid #2563eb', color: '#2563eb', fontWeight: 600 },
}
