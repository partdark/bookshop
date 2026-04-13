import { Link } from 'react-router-dom'
import type { BookListItem } from '../types'
import StarRating from './StarRating'

interface Props {
  book: BookListItem
  onAddToCart?: (book: BookListItem) => void
  isAuthenticated: boolean
  isAdding?: boolean
}

export default function BookCard({ book, onAddToCart, isAuthenticated, isAdding = false }: Props) {
  const imgSrc = book.urlImage || 'https://placehold.co/200x280?text=No+Image'

  return (
    <div style={styles.card}>
      <Link to={`/catalog/${book.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
        <img
          src={imgSrc}
          alt={book.title}
          style={styles.img}
          onError={(e) => { (e.target as HTMLImageElement).src = 'https://placehold.co/200x280?text=No+Image' }}
        />
        <div style={styles.body}>
          <div style={styles.title}>{book.title}</div>
          <div style={styles.authors}>
            {book.authors?.map((a) => a.name).join(', ') || '—'}
          </div>
          <div style={styles.genres}>
            {book.genres?.map((g) => (
              <span key={g.id} style={styles.genre}>{g.name}</span>
            ))}
          </div>
          <div style={styles.ratingRow}>
            <StarRating value={book.rating} size={14} />
            <span style={styles.ratingNum}>{book.rating.toFixed(1)}</span>
          </div>
        </div>
      </Link>
      <div style={styles.footer}>
        <span style={styles.price}>{book.price.toLocaleString('ru-RU')} ₽</span>
        {isAuthenticated ? (
          <button style={styles.addBtn} onClick={() => onAddToCart?.(book)} disabled={isAdding}>
            {isAdding ? '...' : 'В корзину'}
          </button>
        ) : (
          <Link to="/login" style={styles.loginBtn}>Войти</Link>
        )}
      </div>
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  card: {
    background: '#fff',
    border: '1px solid #e5e7eb',
    borderRadius: 10,
    overflow: 'hidden',
    display: 'flex',
    flexDirection: 'column',
    transition: 'box-shadow 0.2s',
  },
  img: {
    width: '100%',
    height: 220,
    objectFit: 'cover',
    display: 'block',
  },
  body: {
    padding: '12px 14px 8px',
    flex: 1,
  },
  title: {
    fontWeight: 600,
    fontSize: 15,
    marginBottom: 4,
    color: '#111827',
    display: '-webkit-box',
    WebkitLineClamp: 2,
    WebkitBoxOrient: 'vertical',
    overflow: 'hidden',
  },
  authors: {
    fontSize: 13,
    color: '#6b7280',
    marginBottom: 6,
  },
  genres: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: 4,
    marginBottom: 8,
  },
  genre: {
    background: '#eff6ff',
    color: '#2563eb',
    fontSize: 11,
    padding: '2px 8px',
    borderRadius: 20,
  },
  ratingRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 6,
  },
  ratingNum: {
    fontSize: 13,
    color: '#6b7280',
  },
  footer: {
    padding: '10px 14px',
    borderTop: '1px solid #f3f4f6',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  price: {
    fontWeight: 700,
    fontSize: 16,
    color: '#111827',
  },
  addBtn: {
    background: '#2563eb',
    color: '#fff',
    border: 'none',
    borderRadius: 6,
    padding: '6px 14px',
    fontSize: 13,
    cursor: 'pointer',
    fontWeight: 500,
  },
  loginBtn: {
    color: '#2563eb',
    fontSize: 13,
    textDecoration: 'none',
  },
}
