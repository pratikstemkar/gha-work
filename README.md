# App

[![CI](https://github.com/pratikstemkar/gha-work/actions/workflows/ci.yml/badge.svg)](https://github.com/pratikstemkar/gha-work/actions/workflows/ci.yml)

A full-stack starter with an ASP.NET Core 10 REST API and a Vue 3 (Vite + TypeScript + Tailwind) frontend. There is no database yet: users and items live in memory and reset when the API restarts.

## Requirements

- .NET SDK 10
- Node.js 22.18+ (24+ recommended) and npm

Install the .NET SDK on Ubuntu:

```bash
sudo apt install dotnet-sdk-10.0
```

Or install it for the current user only:

```bash
curl -fsSL https://dot.net/v1/install/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0 --install-dir "$HOME/.dotnet"
export PATH="$HOME/.dotnet:$PATH"
```

## Run it

Install the frontend dependencies once:

```bash
cd frontend
npm install
```

Then start everything with a single command from the repository root:

```bash
dotnet run --project backend/App.Api
```

This starts the API on `http://localhost:5201` and, via the SPA proxy, also starts the Vite dev server. Open `http://localhost:5201` in a browser; it redirects to Vite, and the browser is same-origin so API calls need no CORS.

### Alternative: run the two processes separately

Useful if you prefer the Vite URL directly.

```bash
# terminal 1
dotnet run --project backend/App.Api --no-launch-profile \
  --urls http://localhost:5201
```

```bash
# terminal 2
cd frontend
npm run dev
```

Then open `http://localhost:5173`. Vite proxies `/api` to the API on port 5201 (see `frontend/vite.config.ts`).

## Log in

| Username | Password   | Roles |
| -------- | ---------- | ----- |
| `demo`   | `demo123`  | User  |
| `admin`  | `admin123` | Admin |

The login page shows the demo credentials and can fill them in for you.

## Pages

- `/login` – sign in
- `/` – dashboard with item counts
- `/items` – add, complete, and delete items
- `/profile` – current user and log out

## API

All endpoints are under `/api`. Everything except `login` and `health` needs a `Bearer` token.

| Method | Path                | Auth | Description                     |
| ------ | ------------------- | ---- | ------------------------------- |
| GET    | `/api/health`       | no   | Health check                    |
| POST   | `/api/auth/login`   | no   | Username + password → token     |
| GET    | `/api/auth/me`      | yes  | Current user                    |
| GET    | `/api/dashboard`    | yes  | Greeting and item counts        |
| GET    | `/api/items`        | yes  | List items                      |
| POST   | `/api/items`        | yes  | Add an item `{ "title": "…" }`  |
| PATCH  | `/api/items/{id}`   | yes  | Set done `{ "done": true }`     |
| DELETE | `/api/items/{id}`   | yes  | Delete an item                  |

Sample requests live in `backend/App.Api/App.Api.http`. In development the OpenAPI document is served at `http://localhost:5201/openapi/v1.json`.

## Tests and linting

Backend (xUnit: unit tests for the stores and JWT issuer, plus integration tests over the HTTP pipeline):

```bash
dotnet test App.slnx
```

Frontend (Vitest: stores, API client, router guard, and components):

```bash
cd frontend
npm run test:unit            # single run
npm run test:unit:watch      # watch mode
npm run test:unit:coverage   # with coverage (enforces 50% thresholds)
npm run lint                 # ESLint (auto-fix)
npm run lint:ci              # ESLint (no fix, for CI)
```

End-to-end (Playwright). The first run needs the browsers once:

```bash
cd frontend
npx playwright install --with-deps chromium
npm run test:e2e
```

Playwright starts the API automatically (which also starts Vite via the SPA proxy). If the app is already running, set `PLAYWRIGHT_BASE_URL=http://localhost:5173` to reuse it.

## Continuous integration

Workflows live in `.github/workflows`:

| Workflow | Triggers | What it does |
| -------- | -------- | ------------ |
| `ci.yml` | push, PR | Calls the three reusable workflows below |
| `backend-ci.yml` | reusable | .NET restore/build, xUnit tests with coverage, coverage PR comment |
| `frontend-ci.yml` | reusable | npm ci, ESLint, Vitest with coverage, build, coverage PR comment |
| `e2e.yml` | reusable | Installs Playwright + Chromium and runs the E2E suite |
| `security.yml` | push/PR to main, weekly | CodeQL (C# + JS/TS), dependency review, NuGet/npm audit |

Dependency updates are proposed automatically by Dependabot (`.github/dependabot.yml`).

## Project structure

```text
.
├── App.slnx                    # solution
├── backend/
│   ├── App.Api/                # ASP.NET Core 10 Web API
│   │   ├── Endpoints/          # auth, dashboard, items
│   │   ├── Models/             # request/response DTOs
│   │   ├── Services/           # in-memory stores + JWT issuer
│   │   └── Program.cs
│   └── App.Api.Tests/          # xUnit unit + integration tests
└── frontend/                   # Vue 3 + Vite + TypeScript + Tailwind
    ├── eslint.config.ts        # ESLint flat config
    ├── vitest.config.ts        # Vitest config
    └── src/
        ├── api/                # fetch client and endpoint wrappers
        ├── layouts/            # app shell with navigation
        ├── router/             # routes and auth guard
        ├── stores/             # Pinia auth store
        └── views/              # login, dashboard, items, profile
```

Test files live in `__tests__` folders next to the code they cover.

## Notes and next steps

- Authentication is JWT-based but deliberately simple: passwords are stored in plain text in `UserStore` and the signing key is in `appsettings.json`. Do not use this as-is in production.
- Sessions are kept in `sessionStorage`, so closing the tab logs you out.
- To add a real database, replace `UserStore` and `ItemStore` with EF Core-backed implementations and keep the endpoint signatures.
- To ship a single deployable app, build the frontend (`npm run build`) and serve `frontend/dist` as static files from the API.
