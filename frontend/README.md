# Frontend

Vue 3 + Vite + TypeScript + Tailwind frontend for the App API. See the [root README](../README.md) for how to run the whole stack.

## Scripts

```bash
npm install       # install dependencies
npm run dev       # start the Vite dev server on http://localhost:5173
npm run build     # type-check and build for production
npm run preview   # preview the production build
```

In development, Vite proxies `/api` requests to the API at `http://localhost:5201` (configured in `vite.config.ts`).
