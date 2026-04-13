interface Props {
  value: number
  max?: number
  size?: number
  onChange?: (v: number) => void
}

export default function StarRating({ value, max = 5, size = 16, onChange }: Props) {
  return (
    <span style={{ display: 'inline-flex', gap: 2 }}>
      {Array.from({ length: max }, (_, i) => i + 1).map((star) => (
        <span
          key={star}
          onClick={() => onChange?.(star)}
          style={{
            fontSize: size,
            cursor: onChange ? 'pointer' : 'default',
            color: star <= Math.round(value) ? '#f59e0b' : '#d1d5db',
            lineHeight: 1,
          }}
        >
          ★
        </span>
      ))}
    </span>
  )
}
