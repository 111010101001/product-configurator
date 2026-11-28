import createClient from 'openapi-fetch'
import type { paths, components } from './schema'

export const api = createClient<paths>({ baseUrl: import.meta.env.VITE_API_BASE_URL })

export type ProductSummary = components['schemas']['ProductSummaryResponse']