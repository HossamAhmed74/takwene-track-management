# Track Distribution Management System

A full-stack application for a music distribution company to manage artists, tracks, and
distributions to Digital Service Providers (DSPs) like Spotify, Apple Music, and YouTube.

- **Backend:** .NET Web API (Clean Architecture, EF Core, JWT Authentication)
- **Frontend:** Angular (Single Page Application)

---

## 📋 Table of Contents

1. [Repository Structure](#-repository-structure)
2. [Prerequisites](#-prerequisites)
3. [Backend Setup & Migrations](#️-backend-setup--migrations)
4. [Frontend Setup](#-frontend-setup)
5. [Authentication — How to Obtain a JWT Token](#-authentication--how-to-obtain-a-jwt-token)
6. [API Endpoints](#-api-endpoints)
7. [Validation & Error Handling](#-validation--error-handling)
8. [Seed Data](#-seed-data)
9. [Quick Test Flow](#-quick-test-flow-postman--swagger)
10. [Decisions & Vibe Coding Notes](#-decisions--vibe-coding-notes)

---

## 📂 Repository Structure

```
├── src/                      # .NET Web API (Clean Architecture)
│   ├── API/                  # Controllers, Middleware, Swagger, JWT setup
│   ├── Core/                 # Entities, DTOs, Interfaces, Business logic
│   └── Infrastructure/       # EF Core DbContext, Repositories, Migrations, Seed data
└── CLIENT-APP/               # Angular frontend (SPA)
    ├── src/app/
    │   ├── core/
    │   │   ├── models/        # Artist, Track, Dsp, TrackDistribution, request DTOs
    │   │   ├── services/      # ArtistService, TrackService, DspService, AuthService
    │   │   └── interceptors/  # auth.interceptor.ts — attaches JWT to every request
    │   ├── features/tracks/
    │   │   ├── track-list/    # Track List view
    │   │   └── track-detail/  # Track Detail view
    │   ├── app.routes.ts
    │   ├── app.config.ts      # provideHttpClient + interceptor registration
    │   └── app.component.*    # shell with a "Set JWT token" control in the header
    ├── src/environments/      # apiBaseUrl config (dev/prod)
    ├── proxy.conf.json        # dev proxy forwarding /api to the backend
    └── README.md              # frontend-specific notes (superseded by this file)
```

---

## 🛠 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or .NET 9)
- [Node.js & npm](https://nodejs.org/) (v18+)
- [Angular CLI](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)
- SQL Server / LocalDB running locally
- EF Core CLI: `dotnet tool install --global dotnet-ef`

---

## ⚙️ Backend Setup & Migrations

### 1. Configure the connection string

In `src/API/appsettings.json`, point `DefaultConnection` at your local SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TakweneTrackDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. Run EF Core migrations (creates schema + seeds data)

```bash
cd src/API
dotnet restore
dotnet ef database update
```

> This creates the database, applies all migrations, and seeds realistic sample data (artists,
> tracks, DSPs, admin user).

### 3. Run the API

```bash
dotnet run
```

- API base URL: `https://localhost:7010`
- Swagger UI: `https://localhost:7010/swagger`

---

## 🎨 Frontend Setup

### 1. Point it at the backend

The dev proxy (`CLIENT-APP/proxy.conf.json`) forwards `/api` requests to
`https://localhost:7010` — adjust the `target` if your backend runs on a different port.
With the proxy, `CLIENT-APP/src/environments/environment.ts` should use:

```ts
export const environment = {
  production: false,
  apiBaseUrl: '/api'
};
```

(If you'd rather not use the proxy, set `apiBaseUrl` to the full backend URL, e.g.
`https://localhost:7010/api`, and enable CORS on the backend for `http://localhost:4200`
instead — see the note in [CORS](#cors-if-not-using-the-proxy) below.)

### 2. Install & run

```bash
cd CLIENT-APP
npm install
npm start          # uses proxy.conf.json automatically if configured in angular.json,
                    # otherwise: npm start -- --proxy-config proxy.conf.json
```

UI available at `http://localhost:4200`.

### 3. Frontend views

1. **Track List** — all tracks with artist name, genre, and status; filterable by status
   (plus genre/artist).
2. **Track Detail** — full track info plus per-DSP distribution statuses; actions to update
   status and submit to DSPs.

### 4. Build for production

```bash
npm run build
```
Output goes to `CLIENT-APP/dist/track-management-ui`. Update
`src/environments/environment.prod.ts` to match wherever the API is deployed.

#### CORS (if not using the proxy)

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```
and, in the middleware pipeline, **before** `UseAuthentication`:
```csharp
app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---

## 🔐 Authentication — How to Obtain a JWT Token

The `POST /api/tracks/{id}/distribute` and `PATCH /api/tracks/{id}/status` endpoints are
JWT-protected.

**Seeded admin credentials:**
- Email: `admin@takwene.com`
- Password: `Admin@123`

**Get a token (cURL):**

```bash
curl -k -X POST "https://localhost:7010/api/auth/login" \
     -H "Content-Type: application/json" \
     -d '{ "email": "admin@takwene.com", "password": "Admin@123" }'
```

**Response:**

```json
{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." }
```

**Using the token:**
- **Swagger UI:** click **Authorize 🔒** → paste the raw token → Authorize.
- **Postman:** Authorization tab → **Bearer Token** → paste the raw token (Postman adds the
  `Bearer ` prefix automatically — don't add it yourself, and don't put the token in the URL
  as a query param).
- **Header form:** `Authorization: Bearer <token>`
- **Angular app:** click **"Set JWT token"** in the top bar, paste the raw token, click Save.
  It's stored in `localStorage` and attached to every request automatically via
  `authInterceptor` (`CLIENT-APP/src/app/core/interceptors/auth.interceptor.ts`).

---

## 🚀 API Endpoints

### Public

| Method | Route | Description |
| :--- | :--- | :--- |
| `POST` | `/api/artists` | Create an artist |
| `GET` | `/api/artists` | List all artists |
| `POST` | `/api/tracks` | Create a track for an artist |
| `GET` | `/api/tracks` | List tracks with filters: `?artistId=&genre=&status=` |
| `GET` | `/api/tracks/{id}` | Track details including DSP distribution statuses |

### 🔒 JWT-Protected

| Method | Route | Description |
| :--- | :--- | :--- |
| `POST` | `/api/tracks/{id}/distribute` | Submit a track to one or more DSPs — body: `{ "dspIds": [1, 2] }` |
| `PATCH` | `/api/tracks/{id}/status` | Update a track's status — body: `{ "status": "submitted" }` |

> ⚠️ **Double-check before submitting:** the `ArtistsController` code shared earlier in this
> project actually exposes `GET /api/artists/GetAllArtist` (not the plain `GET /api/artists`
> listed above), and a Postman test hit `GET /api/tracks/GetAllTracks`. Confirm your final
> controller routes match this table exactly — if the suffixed routes are what's really
> deployed, update this table (and the Angular services in `CLIENT-APP/src/app/core/services/`)
> to match before you submit, so the README stays accurate for a reviewer following along.

### Example request bodies

**POST /api/artists**
```json
{ "name": "Sampa the Great", "email": "sampa@example.com", "country": "Zambia" }
```

**POST /api/tracks**
```json
{
  "title": "Energy",
  "artistId": 1,
  "isrc": "USUM72600001",
  "releaseDate": "2026-08-01",
  "genre": "Hip-Hop",
  "status": "draft"
}
```

---

## 🛡 Validation & Error Handling

- **400 Bad Request** — RFC 7807 `ProblemDetails` with field-level messages (e.g. invalid
  email format, bad ISRC length, invalid status enum).
- **401 Unauthorized** — missing/invalid/expired JWT on protected endpoints.
- **404 Not Found** — unknown track/artist id.
- **409 Conflict** — duplicate ISRC on track creation.

---

## 🌱 Seed Data

Applied automatically by `dotnet ef database update`:

- **Artists (3+):** Sampa the Great (Zambia), Tyla (South Africa), Burna Boy (Nigeria)
- **DSPs (3):** Spotify, Apple Music, YouTube
- **Tracks (8+):** across genres (Afrobeats, Amapiano, Hip-Hop, R&B) and all statuses
  (`draft`, `submitted`, `distributed`), including existing `TrackDistribution` records
  (`pending` / `live` / `rejected`)
- **Admin user:** `admin@takwene.com` / `Admin@123`

This gives the Angular Track List / Track Detail views real data to display immediately
after first run — no manual data entry required.

---

## 🧪 Quick Test Flow (Postman / Swagger)

1. `POST /api/auth/login` → save token
2. `GET /api/artists`
3. `POST /api/artists`
4. `POST /api/tracks`
5. `GET /api/tracks?status=draft`
6. `POST /api/tracks/1/distribute` *(Bearer token)*
7. `GET /api/tracks/1` → verify DSP distribution rows
8. `PATCH /api/tracks/1/status` *(Bearer token)*

To exercise the same flow through the UI: run both apps, open `http://localhost:4200`, click
**"Set JWT token"** and paste the token from step 1, then browse the Track List and click into
a track to submit it to a DSP from the Track Detail page.

---

## 📝 Decisions & Vibe Coding Notes

See [DECISIONS.md](./DECISIONS.md) for what AI generated vs. what was hand-written, security
issues found, and fixes applied.
