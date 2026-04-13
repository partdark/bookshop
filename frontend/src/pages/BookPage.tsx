import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { getBook } from '../api/books'
import { addToCart } from '../api/cart'
import { addReview } from '../api/reviews'
import { useAuthStore } from '../store/authStore'
import StarRating from '../components/StarRating'
import type { Book } from '../types'

export default function BookPage() {
  const { id } = useParams<{ id: string }>()
  const { user, isAuthenticated } = useAuthStore()
  const [book, setBook] = useState<Book | null>(null)
  const [loading, setLoading] = useState(true)
  const [msg, setMsg] = useState<{ text: string; ok: boolean } | null>(null)

  const showMsg = (text: string, ok = true) => {
    setMsg({ text, ok })
    setTimeout(() => setMsg(null), 3000)
  }
  const [reviewRating, setReviewRating] = useState(5)
  const [reviewText, setReviewText] = useState('')
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    if (!id) return
    setLoading(true)
    getBook(id).then(setBook).catch(() => setBook(null)).finally(() => setLoading(false))
  }, [id])

  const handleAddToCart = async () => {
    if (!user || !book) return
    try {
      await addToCart(user.id, book.id, 1)
      showMsg('Добавлено в корзину')
    } catch {
      showMsg('Ошибка', false)
    }
  }

  const handleReview = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!user || !book) return
    setSubmitting(true)
    try {
      await addReview({ rating: reviewRating, reviewText, bookId: book.id, customerId: user.id })
      setReviewText('')
      setReviewRating(5)
      const updated = await getBook(book.id)
      setBook(updated)
      showMsg('Отзыв добавлен')
    } catch (err: any) {
      const serverMsg = err?.response?.data
      showMsg(typeof serverMsg === 'string' ? serverMsg : 'Ошибка при добавлении отзыва', false)
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) return <div style={styles.center}>Загрузка...</div>
  if (!book) return <div style={styles.center}>Книга не найдена</div>

  const imgSrc = book.urlImage || 'https://placehold.co/280x380?text=No+Image'

  return (
    <div style={styles.page}>
      {msg && <div style={{ ...styles.toast, background: msg.ok ? '#16a34a' : '#dc2626' }}>{msg.text}</div>}
      <Link to="/catalog" style={styles.back}>← Назад в каталог</Link>

      <div style={styles.top}>
        <img
          src={imgSrc}
          alt={book.title}
          style={styles.img}
          onError={(e) => { (e.target as HTMLImageElement).src = 'https://placehold.co/280x380?text=No+Image' }}
        />
        <div style={styles.info}>
          <h1 style={styles.title}>{book.title}</h1>
          <div style={styles.authors}>{book.authors?.map(a => a.name).join(', ')}</div>
          <div style={styles.genres}>
            {book.genres?.map(g => <span key={g.id} style={styles.genre}>{g.name}</span>)}
          </div>
          <div style={styles.ratingRow}>
            <StarRating value={book.rating} size={20} />
            <span style={styles.ratingNum}>{book.rating.toFixed(1)}</span>
            <span style={styles.reviewCount}>({book.reviews?.length ?? 0} отзывов)</span>
          </div>
          <div style={styles.year}>Год издания: {book.publicationYear}</div>
          <div style={styles.stock}>
            {book.count > 0
              ? <span style={{ color: '#16a34a' }}>✓ В наличии ({book.count} шт.)</span>
              : <span style={{ color: '#dc2626' }}>Нет в наличии</span>}
          </div>
          <div style={styles.price}>{book.price.toLocaleString('ru-RU')} ₽</div>
          {isAuthenticated() ? (
            <button style={styles.addBtn} onClick={handleAddToCart} disabled={book.count === 0}>
              {book.count > 0 ? 'Добавить в корзину' : 'Нет в наличии'}
            </button>
          ) : (
            <Link to="/login" style={styles.loginLink}>Войдите, чтобы купить</Link>
          )}
        </div>
      </div>

      <div style={styles.desc}>
        <h2 style={styles.sectionTitle}>Описание</h2>
        <p style={styles.descText}>{book.description || 'Описание отсутствует'}</p>
      </div>

      <div style={styles.reviews}>
        <h2 style={styles.sectionTitle}>Отзывы ({book.reviews?.length ?? 0})</h2>
        {isAuthenticated() && (
          <form onSubmit={handleReview} style={styles.reviewForm}>
            <div style={styles.reviewFormRow}>
              <span style={{ fontSize: 14, color: '#374151' }}>Оценка:</span>
              <StarRating value={reviewRating} size={24} onChange={setReviewRating} />
            </div>
            <textarea
              value={reviewText}
              onChange={(e) => setReviewText(e.target.value)}
              placeholder="Напишите отзыв..."
              style={styles.textarea}
              required
              rows={3}
            />
            <button type="submit" style={styles.submitBtn} disabled={submitting}>
              {submitting ? 'Отправка...' : 'Оставить отзыв'}
            </button>
          </form>
        )}
        {book.reviews?.length === 0 && <div style={styles.noReviews}>Отзывов пока нет</div>}
        {book.reviews?.map(r => (
          <div key={r.id} style={styles.reviewCard}>
            <div style={styles.reviewHeader}>
              <span style={styles.reviewAuthor}>{r.customer?.name}</span>
              <StarRating value={r.rating} size={14} />
              <span style={styles.reviewDate}>{new Date(r.date).toLocaleDateString('ru-RU')}</span>
            </div>
            <p style={styles.reviewText}>{r.reviewText}</p>
          </div>
        ))}
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  page: { maxWidth: 1000, margin: '0 auto', padding: '32px 24px' },
  center: { textAlign: 'center', padding: 80, color: '#6b7280' },
  back: { color: '#2563eb', textDecoration: 'none', fontSize: 14, display: 'inline-block', marginBottom: 24 },
  top: { display: 'flex', gap: 40, marginBottom: 40, flexWrap: 'wrap' },
  img: { width: 240, height: 320, objectFit: 'cover', borderRadius: 10, border: '1px solid #e5e7eb', flexShrink: 0 },
  info: { flex: 1, minWidth: 260 },
  title: { fontSize: 26, fontWeight: 700, color: '#111827', marginBottom: 8, marginTop: 0 },
  authors: { color: '#6b7280', fontSize: 15, marginBottom: 10 },
  genres: { display: 'flex', flexWrap: 'wrap', gap: 6, marginBottom: 14 },
  genre: { background: '#eff6ff', color: '#2563eb', fontSize: 12, padding: '3px 10px', borderRadius: 20 },
  ratingRow: { display: 'flex', alignItems: 'center', gap: 8, marginBottom: 10 },
  ratingNum: { fontSize: 16, fontWeight: 600, color: '#111827' },
  reviewCount: { fontSize: 13, color: '#9ca3af' },
  year: { fontSize: 14, color: '#6b7280', marginBottom: 8 },
  stock: { fontSize: 14, marginBottom: 12 },
  price: { fontSize: 28, fontWeight: 700, color: '#111827', marginBottom: 16 },
  addBtn: { background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '12px 28px', fontSize: 15, cursor: 'pointer', fontWeight: 600 },
  loginLink: { color: '#2563eb', fontSize: 14 },
  desc: { marginBottom: 40 },
  sectionTitle: { fontSize: 20, fontWeight: 600, color: '#111827', marginBottom: 16 },
  descText: { color: '#374151', lineHeight: 1.7, fontSize: 15 },
  reviews: {},
  reviewForm: { background: '#f9fafb', border: '1px solid #e5e7eb', borderRadius: 10, padding: 20, marginBottom: 24 },
  reviewFormRow: { display: 'flex', alignItems: 'center', gap: 12, marginBottom: 12 },
  textarea: { width: '100%', border: '1px solid #d1d5db', borderRadius: 8, padding: '10px 14px', fontSize: 14, resize: 'vertical', boxSizing: 'border-box', outline: 'none' },
  submitBtn: { marginTop: 10, background: '#16a34a', color: '#fff', border: 'none', borderRadius: 8, padding: '10px 22px', fontSize: 14, cursor: 'pointer', fontWeight: 500 },
  noReviews: { color: '#9ca3af', fontSize: 14, padding: '20px 0' },
  reviewCard: { border: '1px solid #e5e7eb', borderRadius: 10, padding: '16px 20px', marginBottom: 12 },
  reviewHeader: { display: 'flex', alignItems: 'center', gap: 12, marginBottom: 8 },
  reviewAuthor: { fontWeight: 600, fontSize: 14, color: '#111827' },
  reviewDate: { fontSize: 12, color: '#9ca3af', marginLeft: 'auto' },
  reviewText: { color: '#374151', fontSize: 14, margin: 0, lineHeight: 1.6 },
  toast: { position: 'fixed', bottom: 24, right: 24, background: '#16a34a', color: '#fff', padding: '12px 20px', borderRadius: 8, fontSize: 14, zIndex: 999 },
}
