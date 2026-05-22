import { useEffect, useRef } from 'react'

interface DataItem {
  label: string
  value: number
  color: string
}

interface PieChartProps {
  data: DataItem[]
  title?: string
}

export default function PieChart({ data, title }: PieChartProps) {
  const canvasRef = useRef<HTMLCanvasElement>(null)

  useEffect(() => {
    const canvas = canvasRef.current
    if (!canvas) return
    const ctx = canvas.getContext('2d')
    if (!ctx) return

    const width = canvas.width
    const height = canvas.height
    const centerX = width / 2
    const centerY = height / 2
    const radius = Math.min(width, height) / 2 - 20

    ctx.clearRect(0, 0, width, height)

    const total = data.reduce((sum, item) => sum + item.value, 0)
    let startAngle = -Math.PI / 2

    data.forEach(item => {
      const sliceAngle = (item.value / total) * 2 * Math.PI
      ctx.beginPath()
      ctx.moveTo(centerX, centerY)
      ctx.arc(centerX, centerY, radius, startAngle, startAngle + sliceAngle)
      ctx.closePath()
      ctx.fillStyle = item.color
      ctx.fill()
      startAngle += sliceAngle
    })

    if (title) {
      ctx.fillStyle = '#111827'
      ctx.font = 'bold 16px Inter, system-ui, sans-serif'
      ctx.textAlign = 'center'
      ctx.fillText(title, centerX, 20)
    }

    let legendY = 40
    data.forEach(item => {
      const label = `${item.label || 'Без названия'}: ${item.value}`
      ctx.fillStyle = item.color
      ctx.fillRect(10, legendY, 12, 12)
      ctx.fillStyle = '#374151'
      ctx.font = '12px Inter, system-ui, sans-serif'
      ctx.textAlign = 'left'
      ctx.fillText(label, 28, legendY + 10)
      legendY += 20
    })

    ctx.fillStyle = '#111827'
    ctx.font = 'bold 14px Inter, system-ui, sans-serif'
    ctx.textAlign = 'left'
    ctx.fillText(`Total: ${total}`, 10, legendY + 20)
  }, [data, title])

  return <canvas ref={canvasRef} width={400} height={300} />
}
