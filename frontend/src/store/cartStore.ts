import { create } from 'zustand'
import { getCart } from '../api/cart'

interface CartState {
  count: number
  setCount: (n: number) => void
  increment: () => void
  decrement: () => void
  loadCount: (customerId: string) => Promise<void>
}

export const useCartStore = create<CartState>((set) => ({
  count: 0,
  setCount: (n) => set({ count: n }),
  increment: () => set((s) => ({ count: s.count + 1 })),
  decrement: () => set((s) => ({ count: Math.max(0, s.count - 1) })),
  loadCount: async (customerId) => {
    try {
      const items = await getCart(customerId)
      set({ count: items.reduce((s, i) => s + i.quantity, 0) })
    } catch {
      set({ count: 0 })
    }
  },
}))
