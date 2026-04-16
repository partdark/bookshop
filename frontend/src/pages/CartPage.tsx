import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { getCart, updateCartItem, removeFromCart, clearCart, checkout } from '../api/cart'
import { useAuthStore } from '../store/authStore'
import { useCartStore } from '../store/cartStore'
import type { CartItem } from '../types'

export default function CartPage() {
  const { user } = useAuthStore()
  const { setCount } = useCartStore()
  const [items, setItems] = useState<CartItem[]>([])
  const [loading, setLoading] = useState(true)
  const [msg, setMsg] = useState<{ text: string; ok: boolean } | null>(null)

  const showMsg = (text: string, ok = true) => {
    setMsg({ text, ok })
    setTimeout(() => setMsg(null), 3000)
  }
  const [checkingOut, setCheckingOut] = useState(false)

  const load = async () => {
    if (!user) return
    setLoading(true)
    try {
      const data = await getCart(user.id)
      setItems(data)
      setCount(data.reduce((s, i) => s + i.quantity, 0))
    } catch {
      setItems([])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [user])

  const handleUpdate = async (bookId: string, count: number) => {
    if (!user) return
    if (count <= 0) {
      await handleRemove(bookId)
      return
    }
    const item = items.find(i => i.book.id === bookId)
    if (item && count > item.book.count) {
      showMsg(`Доступно только ${item.book.count} шт.`, false)
      return
    }
    await updateCartItem(user.id, bookId, count)
    const updated = items.map(i => i.book.id === bookId ? { ...i, quantity: count } : i)
    setItems(updated)
    setCount(updated.reduce((s, i) => s + i.quantity, 0))
  }

  const handleRemove = async (bookId: string) => {
    if (!user) return
    await removeFromCart(user.id, bookId)
    const updated = items.filter(i => i.book.id !== bookId)
    setItems(updated)
    setCount(updated.reduce((s, i) => s + i.quantity, 0))
  }

  const handleClear = async () => {
    if (!user) return
    await clearCart(user.id)
    setItems([])
    setCount(0)
  }

  const handleCheckout = async () => {
    if (!user) return
    setCheckingOut(true)
    try {
      await checkout(user.id)
      setItems([])
      setCount(0)
      showMsg('Заказ оформлен!')
    } catch (err: any) {
      const serverMsg = err?.response?.data
      showMsg(typeof serverMsg === 'string' ? serverMsg : 'Ошибка при оформлении заказа', false)
    } finally {
      setCheckingOut(false)
    }
  }

  const total = items.reduce((sum, i) => sum + i.book.price * i.quantity, 0)
  const hasUnavailable = items.some(i => i.book.count === 0 || i.quantity > i.book.count)

  if (loading) return <div style={styles.center}>Загрузка...</div>

  return (
    <div style={styles.page}>
      {msg && <div style={{ ...styles.toast, background: msg.ok ? '#16a34a' : '#dc2626' }}>{msg.text}</div>}
      <h1 style={styles.title}>Корзина</h1>

      {items.length === 0 ? (
        <div style={styles.empty}>
          <div style={{ fontSize: 48, marginBottom: 16 }}>🛒</div>
          <div style={{ color: '#6b7280', marginBottom: 20 }}>Корзина пуста</div>
          <Link to="/catalog" style={styles.catalogLink}>Перейти в каталог</Link>
        </div>
      ) : (
        <div style={styles.layout}>
          <div style={styles.itemsList}>
            {items.map(item => (
              <div key={item.book.id} style={styles.item}>
                <img
                  src={item.book.urlImage || 'https://placehold.co/80x100?text=?'}
                  alt={item.book.title}
                  style={styles.itemImg}
                  onError={(e) => { (e.target as HTMLImageElement).src = 'https://placehold.co/80x100?text=?' }}
                />
                <div style={styles.itemInfo}>
                  <Link to={`/catalog/${item.book.id}`} style={styles.itemTitle}>{item.book.title}</Link>
                  <div style={styles.itemAuthors}>{item.book.authors?.map(a => a.name).join(', ')}</div>
                  <div style={styles.itemPrice}>{item.book.price.toLocaleString('ru-RU')} ₽</div>
                </div>
                <div style={styles.itemControls}>
                  <button style={styles.qtyBtn} onClick={() => handleUpdate(item.book.id, item.quantity - 1)}>−</button>
                  <span style={styles.qty}>{item.quantity}</span>
                  <button
                    style={{ ...styles.qtyBtn, ...(item.quantity >= item.book.count ? styles.qtyBtnDisabled : {}) }}
                    onClick={() => handleUpdate(item.book.id, item.quantity + 1)}
                    disabled={item.quantity >= item.book.count}
                  >+</button>
                </div>
                {item.quantity >= item.book.count && item.book.count > 0 && (
                  <div style={styles.stockWarn}>макс. {item.book.count}</div>
                )}
                {item.book.count === 0 && (
                  <div style={{ ...styles.stockWarn, color: '#dc2626' }}>нет в наличии</div>
                )}
                <div style={styles.itemTotal}>{(item.book.price * item.quantity).toLocaleString('ru-RU')} ₽</div>
                <button style={styles.removeBtn} onClick={() => handleRemove(item.book.id)}>✕</button>
              </div>
            ))}
            <button style={styles.clearBtn} onClick={handleClear}>Очистить корзину</button>
          </div>

          <div style={styles.summary}>
            <h2 style={styles.summaryTitle}>Итого</h2>
            <div style={styles.summaryRow}>
              <span>Товаров: {items.reduce((s, i) => s + i.quantity, 0)}</span>
            </div>
            <div style={styles.summaryTotal}>{total.toLocaleString('ru-RU')} ₽</div>
            <button style={{ ...styles.checkoutBtn, ...(hasUnavailable ? styles.checkoutBtnDisabled : {}) }}
              onClick={handleCheckout} disabled={checkingOut || hasUnavailable}>
              {checkingOut ? 'Оформление...' : 'Оформить заказ'}
            </button>
            {hasUnavailable && (
              <div style={{ fontSize: 12, color: '#dc2626', marginTop: 8, textAlign: 'center' }}>
                Уберите недоступные товары
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  page: { maxWidth: 1000, margin: '0 auto', padding: '32px 24px' },
  center: { textAlign: 'center', padding: 80, color: '#6b7280' },
  title: { fontSize: 28, fontWeight: 700, color: '#111827', marginBottom: 32 },
  empty: { textAlign: 'center', padding: '60px 0' },
  catalogLink: { background: '#2563eb', color: '#fff', textDecoration: 'none', padding: '10px 24px', borderRadius: 8, fontSize: 14 },
  layout: { display: 'flex', gap: 32, alignItems: 'flex-start', flexWrap: 'wrap' },
  itemsList: { flex: 1, minWidth: 300 },
  item: { display: 'flex', alignItems: 'center', gap: 16, padding: '16px 0', borderBottom: '1px solid #f3f4f6' },
  itemImg: { width: 64, height: 84, objectFit: 'cover', borderRadius: 6, border: '1px solid #e5e7eb', flexShrink: 0 },
  itemInfo: { flex: 1 },
  itemTitle: { fontWeight: 600, fontSize: 14, color: '#111827', textDecoration: 'none', display: 'block', marginBottom: 4 },
  itemAuthors: { fontSize: 12, color: '#9ca3af', marginBottom: 4 },
  itemPrice: { fontSize: 14, color: '#6b7280' },
  itemControls: { display: 'flex', alignItems: 'center', gap: 8 },
  qtyBtn: { width: 28, height: 28, border: '1px solid #d1d5db', borderRadius: 6, background: '#fff', cursor: 'pointer', fontSize: 16, display: 'flex', alignItems: 'center', justifyContent: 'center' },
  qty: { fontSize: 15, fontWeight: 600, minWidth: 24, textAlign: 'center' },
  itemTotal: { fontWeight: 600, fontSize: 15, minWidth: 80, textAlign: 'right' },
  removeBtn: { background: 'none', border: 'none', color: '#9ca3af', cursor: 'pointer', fontSize: 16, padding: 4 },
  clearBtn: { marginTop: 16, background: 'none', border: '1px solid #e5e7eb', borderRadius: 8, padding: '8px 16px', cursor: 'pointer', fontSize: 13, color: '#6b7280' },
  summary: { width: 280, background: '#f9fafb', border: '1px solid #e5e7eb', borderRadius: 12, padding: 24, flexShrink: 0 },
  summaryTitle: { fontSize: 18, fontWeight: 600, marginBottom: 16, marginTop: 0 },
  summaryRow: { fontSize: 14, color: '#6b7280', marginBottom: 8 },
  summaryTotal: { fontSize: 24, fontWeight: 700, color: '#111827', margin: '16px 0' },
  checkoutBtn: { width: '100%', background: '#16a34a', color: '#fff', border: 'none', borderRadius: 8, padding: '12px 0', fontSize: 15, cursor: 'pointer', fontWeight: 600 },
  checkoutBtnDisabled: { background: '#9ca3af', cursor: 'not-allowed' },
  toast: { position: 'fixed', bottom: 24, right: 24, color: '#fff', padding: '12px 20px', borderRadius: 8, fontSize: 14, zIndex: 999 },
  qtyBtnDisabled: { opacity: 0.4, cursor: 'not-allowed' },
  stockWarn: { fontSize: 11, color: '#d97706', whiteSpace: 'nowrap' as const },
}
