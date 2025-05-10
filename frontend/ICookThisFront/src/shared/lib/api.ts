import axios from 'axios'

export const api = axios.create({
  //baseURL: import.meta.env.VITE_API_BASE_URL,
  baseURL: 'http://localhost:5284/api',
  headers: { 'Content-Type': 'application/json' },
})
