# BookTracker

## Vercel deployment notes

- For `BookTrackerWebClient`, set the environment variable `WEBCLIENT_API_BASE_URL` to your API endpoint (e.g. `https://booktracker-api.vercel.app`).
- `BookTrackerWebClient` now checks in order: `window.WEBCLIENT_API_BASE_URL`, `window.BOOKTRACKER_API_BASE_URL`, `<meta name="webclient-api-base-url">`, localStorage value, then `window.location.origin`.
- Ensure API (`BookTrackerApi`) is deployed and reachable with CORS enabled (`WebClientCors` currently allows all origins).

If connection fails, open browser console network details and verify the resolved `apiBaseUrl` value in the top config field.
