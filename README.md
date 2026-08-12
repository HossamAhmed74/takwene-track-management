# Track Management UI (Angular 19)

Angular front-end for the Takwene Track Management API (.NET Web API backend).
Implements the two required views:

1. **Track List** (`/tracks`) — all tracks with artist name, genre, status, filterable by status.
2. **Track Detail** (`/tracks/:id`) — full track info, DSP distribution statuses, and a form to
   submit the track to one or more DSPs (`POST /api/tracks/{id}/distribute`).

## Stack

- Angular 19 (standalone components, no NgModules)
- Signals for local component state
- `HttpClient` + a functional interceptor that attaches a JWT bearer token to every request
- Plain CSS, no UI framework (per the "functional is enough" brief)

## Project structure

```
src/app/
  core/
    models/track.models.ts        # Artist, Track, Dsp, TrackDistribution, request DTOs
    services/
      artist.service.ts           # GET/POST /api/artists
      track.service.ts            # GET/POST/PATCH /api/tracks...
      dsp.service.ts              # GET /api/dsps (used to populate the "submit to DSP" form)
      auth.service.ts             # stores the JWT in localStorage as a signal
    interceptors/auth.interceptor.ts
  features/tracks/
    track-list/                   # Track List view
    track-detail/                 # Track Detail view
  app.routes.ts
  app.config.ts                   # provideHttpClient + interceptor registration
  app.component.*                 # shell with a "Set JWT token" control in the header
```

## 1. Point it at your backend

Edit `src/environments/environment.ts`:

```ts
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:7001/api' // <-- your .NET API's base URL
};
```

Use whatever port your `dotnet run` / Kestrel profile prints (check `launchSettings.json` —
commonly `https://localhost:7xxx` or `http://localhost:5xxx`).

**CORS:** make sure the .NET API allows the Angular dev origin (`http://localhost:4200`), e.g.:

```csharp
builder.Services.AddCors(o => o.AddPolicy("dev", p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));
// ...
app.UseCors("dev");
```

Alternatively, run the Angular dev server through the included `proxy.conf.json`
(edit the `target` to match your API port) to avoid CORS entirely:

```bash
npm start -- --proxy-config proxy.conf.json
```
and set `apiBaseUrl: '/api'` in `environment.ts` when using the proxy.

## 2. Install & run

```bash
npm install
npm start          # ng serve, http://localhost:4200
```

## 3. Authenticating (JWT-protected endpoints)

At least one backend endpoint requires a JWT (per the task spec). Obtain a token the way
described in the **backend's** README (e.g. a seeded `POST /api/auth/login`, or a dev token
printed on startup), then in the Angular app:


## 4. Build for Development

```bash
npm run build
```
Output goes to `dist/track-management-ui`. Update `src/environments/environment.prod.ts`
(`apiBaseUrl`) to match wherever the API is deployed, or serve the built app behind the same
host as the API under `/api`.

## Notes on backend contract assumptions

The spec's endpoint table doesn't specify exact JSON field casing or a DSP list endpoint. This
UI assumes:
- JSON responses use camelCase (`artistName`, `releaseDate`, etc.) — standard for ASP.NET Core's
  default `System.Text.Json` serializer.
- `GET /api/tracks/{id}` returns the track plus a `distributions` array (id, dspId, dspName,
  submittedAt, status) — as implied by "Get track details including its DSP distribution
  statuses."
- A `GET /api/dsps` endpoint exists to populate the "submit to DSP" checkboxes. If your backend
  exposes DSPs differently (e.g. hardcoded/seeded and not via an endpoint), edit
  `src/app/core/services/dsp.service.ts` accordingly — the detail page degrades gracefully and
  simply hides the distribute panel if that call fails.

If any of these differ from your actual backend, the only files that need adjusting are the
model interfaces in `track.models.ts` and the three service files in `core/services/`.
