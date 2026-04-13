export interface Author {
  id: string
  name: string
  year: number
}

export interface Genre {
  id: string
  name: string
}

export interface ReviewCustomer {
  id: string
  name: string
}

export interface Review {
  id: string
  date: string
  rating: number
  reviewText: string
  customer: ReviewCustomer
}

export interface Book {
  id: string
  title: string
  description: string
  rating: number
  price: number
  urlImage: string
  count: number
  publicationYear: number
  authors: Author[]
  genres: Genre[]
  reviews: Review[]
}

export interface BookListItem {
  id: string
  title: string
  description: string
  price: number
  rating: number
  urlImage: string
  count: number
  publicationYear: number
  authors: Author[]
  genres: Genre[]
}

export interface CatalogResponse {
  books: BookListItem[]
  totalCount: number
  pageNumber: number
  pageCapacity: number
}

export interface Customer {
  id: string
  name: string
  mail: string
  phone: string
  dateOfBirth: string
}

export interface AuthResponse {
  token: string
  refreshToken: string
  customer: Customer
  role: string
}

export interface CartItem {
  book: Book
  quantity: number
}

export interface OrderItem {
  bookId: string
  count: number
  priceAtPurchase: number
}

export interface Order {
  id: number
  customerId: string
  createdDate: string
  totalPrice: number
  status: string
  items: OrderItem[]
}

export interface OrderItemDetail {
  bookId: string
  bookTitle: string
  bookUrlImage: string
  count: number
  priceAtPurchase: number
  subtotal: number
}

export interface OrderDetail {
  id: number
  customerId: string
  customerName: string
  customerEmail: string
  createdDate: string
  totalPrice: number
  status: string
  items: OrderItemDetail[]
}
