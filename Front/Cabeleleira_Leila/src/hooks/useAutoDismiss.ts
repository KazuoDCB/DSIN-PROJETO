import { useEffect } from 'react'

export const useAutoDismiss = (value: unknown, onClear: () => void, delay = 1500) => {
  useEffect(() => {
    if (!value) return
    if (Array.isArray(value)) return

    const timeoutId = window.setTimeout(onClear, delay)
    return () => window.clearTimeout(timeoutId)
  }, [delay, onClear, value])
}
