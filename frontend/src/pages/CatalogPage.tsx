import { useState, useEffect, useCallback } from 'react'
import { useSearchParams } from 'react-router-dom'
import { getCatalog, getGenres } from '../api/books'
import { addToCart } from '../api/cart'
import { useAuthStore } from '../store/authStore'
import { useCartStore } from '../store/cartStore'
import BookCard from '../components/BookCard'
import type { BookListItem } from '../types'

const SORT_OPTIONS = [
  { value: 'Title', label: 'По названию' },
  { value: 'Price', label: 'По цене' },
  { value: 'Rating', label: 'По рейтингу' },
  { value: 'PublicationYear', label: 'По году' },
]

export default function CatalogPage() {
  const { user, isAuthenticated } = useAuthStore()
  const { increment } = useCartStore()
  const [searchParams, setSearchParams] = useSearchParams()

  const page = parseInt(searchParams.get('page') ?? '1', 10)
  const search = searchParams.get('search') ?? ''
  const orderBy = searchParams.get('orderBy') ?? 'Title'
  const desc = searchParams.get('desc') === 'true'
  const selectedGenre = searchParams.get('genre') ?? ''

  const [books, setBooks] = useState<BookListItem[]>([])
  const [total, setTotal] = useState(0)
  const [searchInput, setSearchInput] = useState(search)
  const [genres, setGenres] = useState<{ id: string; name: string }[]>([])
  const [loading, setLoading] = useState(false)
  const [addingId, setAddingId] = useState<string | null>(null)  // 7: индикатор
  const [cartMsg, setCartMsg] = useState<string | null>(null)
  const pageSize = 20

  const setParam = (updates: Record<string, string | null>) => {
    setSearchParams(prev => {
      const next = new URLSearchParams(prev)
      for (const [k, v] of Object.entries(updates)) {
        if (v === null || v === '') next.delete(k)
        else next.set(k, v)
      }
      return next
    }, { replace: true })
  }

  useEffect(() => {
    getGenres().then(setGenres).catch(() => {})
  }, [])

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const data = await getCatalog({
        pageNumber: page,
        pageCapacity: pageSize,
        titleContains: search || undefined,
        orderBy,
        desc,
      })
      // 4: фильтрация по жанру на клиенте (API не поддерживает фильтр по жанру)
      let result = data.books ?? []
      if (selectedGenre) {
        result = result.filter(b => b.genres?.some(g => g.id === selectedGenre))
      }
      setBooks(result)
      setTotal(data.totalCount ?? 0)
    } catch {
      setBooks([])
    } finally {
      setLoading(false)
    }
  }, [page, search, orderBy, desc, selectedGenre])

  useEffect(() => { load() }, [load])

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setParam({ search: searchInput || null, page: null })
  }

  const handleReset = () => {
    setSearchInput('')
    setSearchParams({}, { replace: true })
  }

  // 7: индикатор загрузки на кнопке
  const handleAddToCart = async (book: BookListItem) => {
    if (!user) return
    setAddingId(book.id)
    try {
      await addToCart(user.id, book.id, 1)
      increment()
      setCartMsg(`«${book.title}» добавлена в корзину`)
      setTimeout(() => setCartMsg(null), 2500)
    } catch {
      setCartMsg('Ошибка при добавлении в корзину')
      setTimeout(() => setCartMsg(null), 2500)
    } finally {
      setAddingId(null)
    }
  }

  const totalPages = Math.ceil(total / pageSize)

  return (
    <div style={styles.page}>
      {/* Поиск */}
      <div style={styles.header}>
        <h1 style={styles.title}>Каталог книг</h1>
        <form onSubmit={handleSearch} style={styles.searchForm}>
          <input
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            placeholder="Поиск по названию..."
            style={styles.searchInput}
          />
          <button type="submit" style={styles.searchBtn}>Найти</button>
        </form>
      </div>

      {/* 4: Фильтры */}
      <div style={styles.filters}>
        <select
          value={selectedGenre}
          onChange={(e) => setParam({ genre: e.target.value || null, page: null })}
          style={styles.select}
        >
          <option value="">Все жанры</option>
          {genres.map(g => <option key={g.id} value={g.id}>{g.name}</option>)}
        </select>

        <select
          value={orderBy}
          onChange={(e) => setParam({ orderBy: e.target.value, page: null })}
          style={styles.select}
        >
          {SORT_OPTIONS.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
        </select>

        <button
          style={styles.dirBtn}
          onClick={() => setParam({ desc: desc ? null : 'true', page: null })}
          title={desc ? 'По убыванию' : 'По возрастанию'}
        >
          {desc ? '↓' : '↑'}
        </button>

        {(search || selectedGenre || orderBy !== 'Title' || desc) && (
          <button style={styles.clearBtn} onClick={handleReset}>Сбросить</button>
        )}
      </div>

      {cartMsg && <div style={styles.toast}>{cartMsg}</div>}

      {loading ? (
        <div style={styles.loading}>Загрузка...</div>
      ) : books.length === 0 ? (
        <div style={styles.empty}>Книги не найдены</div>
      ) : (
        <>
          <div style={styles.grid}>
            {books.map((book) => (
              <BookCard
                key={book.id}
                book={book}
                onAddToCart={handleAddToCart}
                isAuthenticated={isAuthenticated()}
                isAdding={addingId === book.id}
              />
            ))}
          </div>
          {totalPages > 1 && (
            <div style={styles.pagination}>
              <button style={styles.pageBtn} disabled={page === 1} onClick={() => setParam({ page: String(page - 1) })}>← Назад</button>
              <span style={styles.pageInfo}>Страница {page} из {totalPages}</span>
              <button style={styles.pageBtn} disabled={page >= totalPages} onClick={() => setParam({ page: String(page + 1) })}>Вперёд →</button>
            </div>
          )}
        </>
      )}
    </div>
  )
}

const styles: Record<string, React.CSSProperties> = {
  page: { maxWidth: 1200, margin: '0 auto', padding: '32px 24px' },
  header: { display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 16, marginBottom: 16 },
  title: { fontSize: 28, fontWeight: 700, color: '#111827', margin: 0 },
  searchForm: { display: 'flex', gap: 8 },
  searchInput: { border: '1px solid #d1d5db', borderRadius: 8, padding: '8px 14px', fontSize: 14, width: 260, outline: 'none' },
  searchBtn: { background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '8px 18px', cursor: 'pointer', fontSize: 14 },
  filters: { display: 'flex', alignItems: 'center', gap: 10, marginBottom: 24, flexWrap: 'wrap' },
  select: { border: '1px solid #d1d5db', borderRadius: 8, padding: '7px 12px', fontSize: 14, background: '#fff', cursor: 'pointer', outline: 'none' },
  dirBtn: { border: '1px solid #d1d5db', borderRadius: 8, padding: '7px 14px', fontSize: 16, background: '#fff', cursor: 'pointer' },
  clearBtn: { background: '#f3f4f6', border: 'none', borderRadius: 8, padding: '7px 14px', cursor: 'pointer', fontSize: 13, color: '#6b7280' },
  grid: { display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(200px, 1fr))', gap: 20 },
  loading: { textAlign: 'center', padding: 60, color: '#6b7280' },
  empty: { textAlign: 'center', padding: 60, color: '#6b7280' },
  pagination: { display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 20, marginTop: 40 },
  pageBtn: { background: '#fff', border: '1px solid #d1d5db', borderRadius: 8, padding: '8px 18px', cursor: 'pointer', fontSize: 14 },
  pageInfo: { color: '#6b7280', fontSize: 14 },
  toast: { position: 'fixed', bottom: 24, right: 24, background: '#16a34a', color: '#fff', padding: '12px 20px', borderRadius: 8, fontSize: 14, zIndex: 999 },
}
