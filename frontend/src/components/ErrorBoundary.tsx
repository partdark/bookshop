import { Component, type ReactNode } from 'react'

interface Props { children: ReactNode }
interface State { hasError: boolean; error: Error | null }

export default class ErrorBoundary extends Component<Props, State> {
  state: State = { hasError: false, error: null }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error }
  }

  render() {
    if (this.state.hasError) {
      return (
        <div style={styles.page}>
          <div style={styles.box}>
            <div style={styles.icon}>⚠️</div>
            <h2 style={styles.title}>Что-то пошло не так</h2>
            <p style={styles.msg}>{this.state.error?.message ?? 'Неизвестная ошибка'}</p>
            <button style={styles.btn} onClick={() => window.location.reload()}>
              Перезагрузить страницу
            </button>
          </div>
        </div>
      )
    }
    return this.props.children
  }
}

const styles: Record<string, React.CSSProperties> = {
  page: { minHeight: '60vh', display: 'flex', alignItems: 'center', justifyContent: 'center' },
  box: { textAlign: 'center', padding: 40, maxWidth: 400 },
  icon: { fontSize: 48, marginBottom: 16 },
  title: { fontSize: 22, fontWeight: 700, color: '#111827', marginBottom: 8 },
  msg: { color: '#6b7280', fontSize: 14, marginBottom: 24 },
  btn: { background: '#2563eb', color: '#fff', border: 'none', borderRadius: 8, padding: '10px 24px', fontSize: 14, cursor: 'pointer' },
}
