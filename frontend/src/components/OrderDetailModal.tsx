import { useEffect, useState } from 'react'
import { getOrderDetail } from '../api/customer'
import type { OrderDetail } from '../types'

const STATUS_LABELS: Record<string, { label: string; color: string }> = {
  Placed:    { label: 'Оформлен',  color: '#2563eb' },
  Shipped:   { label: 'Отправлен', color: '#d97706' },
  Delivered: { label: 'Доставлен', color: '#16a34a' },
  Cancelled: { label: 'Отменён',   color: '#dc2626' },
}

interface Props {
  orderId: number
  onClose: () => void
}

export default function OrderDetailModal({ orderId, onClose }: Props) {
  const [detail, setDetail] = useState<OrderDetail | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(false)

  useEffect(() => {
    getOrderDetail(orderId)
      .then(setDetail)
      .catch(() => setError(true))
      .finally(() => setLoading(false))
  }, [orderId])

  // Close on backdrop click
  const handleBackdrop = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) onClose()
  }

  const status = detail ? (STATUS_LABELS[detail.status] ?? { label: detail.status, color: '#6b7280' }) : null

  return (
    <div style={s.backdrop} onClick={handleBackdrop}>
      <div style={s.modal}>
        <button style={s.closeBtn} onClick={onClose} aria-label="Закрыть">✕</button>

        {loading && <div style={s.center}>Загрузка...</div>}
        {error && <div style={s.center}>Не удалось загрузить заказ</div>}

        {detail && (
          <>
            <div style={s.header}>
              <h2 style={s.title}>Заказ #{detail.id}</h2>
              {status && (
                <span style={{ ...s.badge, color: status.color, borderColor: status.color }}>
                  {status.label}
                </span>
              )}
            </div>

            <div style={s.meta}>
              <span>📅 {new Date(detail.createdDate).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' })}</span>
              <span>👤 {detail.customerName}</span>
              <span>✉️ {detail.customerEmail}</span>
            </div>

            <div style={s.itemsHeader}>
              <span style={{ flex: 3 }}>Книга</span>
              <span style={{ flex: 1, textAlign: 'center' }}>Кол-во</span>
              <span style={{ flex: 1, textAlign: 'right' }}>Цена</span>
              <span style={{ flex: 1, textAlign: 'right' }}>Сумма</span>
            </div>

            <div style={s.itemsList}>
              {detail.items.map((item) => (
                <div key={item.bookId} style={s.itemRow}>
                  <div style={{ ...s.itemBook, flex: 3 }}>
                    {item.bookUrlImage && (
                      <img src={item.bookUrlImage} alt={item.bookTitle} style={s.cover} />
                    )}
                    <span style={s.bookTitle}>{item.bookTitle || 'Книга'}</span>
                  </div>
                  <span style={{ flex: 1, textAlign: 'center', color: '#374151' }}>{item.count}</span>
                  <span style={{ flex: 1, textAlign: 'right', color: '#6b7280' }}>
                    {item.priceAtPurchase.toLocaleString('ru-RU')} ₽
                  </span>
                  <span style={{ flex: 1, textAlign: 'right', fontWeight: 600, color: '#111827' }}>
                    {item.subtotal.toLocaleString('ru-RU')} ₽
                  </span>
                </div>
              ))}
            </div>

            <div style={s.footer}>
              <span style={s.totalLabel}>Итого</span>
              <span style={s.totalValue}>{detail.totalPrice.toLocaleString('ru-RU')} ₽</span>
            </div>
          </>
        )}
      </div>
    </div>
  )
}

const s: Record<string, React.CSSProperties> = {
  backdrop: {
    position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.45)',
    display: 'flex', alignItems: 'center', justifyContent: 'center',
    zIndex: 1000, padding: 16,
  },
  modal: {
    background: '#fff', borderRadius: 14, padding: '28px 32px',
    width: '100%', maxWidth: 640, maxHeight: '90vh',
    overflowY: 'auto', position: 'relative', boxShadow: '0 20px 60px rgba(0,0,0,0.2)',
  },
  closeBtn: {
    position: 'absolute', top: 16, right: 16,
    background: 'none', border: 'none', fontSize: 18,
    cursor: 'pointer', color: '#9ca3af', lineHeight: 1,
  },
  header: { display: 'flex', alignItems: 'center', gap: 12, marginBottom: 12 },
  title: { fontSize: 20, fontWeight: 700, color: '#111827', margin: 0 },
  badge: { fontSize: 12, border: '1px solid', borderRadius: 20, padding: '3px 12px' },
  meta: { display: 'flex', flexWrap: 'wrap', gap: 16, fontSize: 13, color: '#6b7280', marginBottom: 24 },
  itemsHeader: {
    display: 'flex', gap: 8, padding: '8px 0',
    borderBottom: '2px solid #f3f4f6',
    fontSize: 12, fontWeight: 600, color: '#9ca3af', textTransform: 'uppercase',
  },
  itemsList: { display: 'flex', flexDirection: 'column', gap: 0 },
  itemRow: {
    display: 'flex', alignItems: 'center', gap: 8,
    padding: '12px 0', borderBottom: '1px solid #f3f4f6', fontSize: 14,
  },
  itemBook: { display: 'flex', alignItems: 'center', gap: 10 },
  cover: { width: 36, height: 50, objectFit: 'cover', borderRadius: 4, flexShrink: 0 },
  bookTitle: { color: '#111827', fontWeight: 500 },
  footer: {
    display: 'flex', justifyContent: 'space-between', alignItems: 'center',
    marginTop: 20, paddingTop: 16, borderTop: '2px solid #f3f4f6',
  },
  totalLabel: { fontSize: 16, fontWeight: 600, color: '#374151' },
  totalValue: { fontSize: 20, fontWeight: 700, color: '#111827' },
  center: { textAlign: 'center', padding: '40px 0', color: '#9ca3af' },
}
