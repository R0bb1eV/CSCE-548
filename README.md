# BookTracker

BookTracker is a small end-to-end app with:
- `BookTrackerApi` (ASP.NET Core Web API)
- `BookTrackerWebClient` (static HTML/JS frontend)
- `BookTracker` (shared business/models/data logic)
- `BookTrackerConsoleClient` (optional console client)

## AI Usage Summary (short)
- Prompts by layer: front end UI changes, API CORS fixes, data-layer JSON fix, docs updates.
- Changes to AI output: tightened CORS, enum alignment, removed OpenAPI package.
- Effectiveness: fast UI + docs; missed enum/port conflicts and required manual fixes.

## Quick Start (Local)
1. Open a terminal in the repo root.
2. Set environment variables:
   - `$env:SUPABASE_URL="https://your-project.supabase.co"`
   - `$env:SUPABASE_ANON_KEY="your-anon-key"`
3. Run the API:
   - `dotnet run --project .\BookTrackerApi\BookTrackerApi.csproj`
4. Run the web client:
   - `dotnet run --project .\BookTrackerWebClient\BookTrackerWebClient.csproj`
5. Open the web client URL and click "Load All Authors".

## Hosting Overview
- API hosting: Render
  - Env vars: `SUPABASE_URL`, `SUPABASE_ANON_KEY`, `ASPNETCORE_ENVIRONMENT=Production`
- Web client hosting: Vercel
  - Root directory: `BookTrackerWebClient`
  - Env var: `WEBCLIENT_API_BASE_URL=https://<your-render-service>.onrender.com`

## Success Checks
- API: `GET /` returns `BookTracker API is running.`
- API: `GET /api/ping` returns `{ status: "ok", ... }`
- Web client loads data without errors.

## Full Deployment Instructions
See `DEPLOYMENT.md`.

## AI Usage Summary (short)
- Prompts by layer: front end UI changes, API CORS fixes, data-layer JSON fix, docs updates.
- Changes to AI output: tightened CORS, enum alignment, removed OpenAPI package.
- Effectiveness: fast UI + docs; missed enum/port conflicts and required manual fixes.


## Screenshots
- GET All Items: `img/image.png`
- GET Single Items: `img/image4.png`
- Create: `img/image2.png`
- Update: `img/image3.png`
- Delete: (add screenshot if available)
