import { api } from '@shared/lib/api'
import type { UnitResponse } from '../models/unitModel'

export function fetchUnits(): Promise<UnitResponse[]> {
  return api.get<UnitResponse[]>('/units').then((r) => r.data)
}
