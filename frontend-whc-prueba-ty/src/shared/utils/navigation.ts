export function toSafeRedirectPath(path: unknown, fallback = '/'): string {
  const isInternalPath =
    typeof path === 'string' && path.startsWith('/') && !path.startsWith('//') && !path.includes('\\')

  return isInternalPath ? path : fallback
}
