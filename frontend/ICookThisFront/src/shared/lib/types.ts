// paged result
export interface PagedResult<T> {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  sortBy?: 'Name' | 'AvgRating' | 'AvgPreparationTime'
  sortOrder?: 'Asc' | 'Desc'
  search?: string
  items: T[]
}
