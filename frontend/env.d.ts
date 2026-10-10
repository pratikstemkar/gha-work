/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Optional absolute API base URL. Empty in development so requests stay same-origin. */
  readonly VITE_API_BASE_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
