# DECISIONS.md

Honest account of AI usage on this project, as required by Task 3.

---

## 1. What did AI generate for you, and what did you write or modify yourself?

### Frontend (Angular) — AI-generated, then reviewed/adjusted by me

I used Claude to scaffold and build the entire Angular front-end (`CLIENT-APP/`):

- Project scaffold via `ng new` (Angular 19, standalone components, routing, CSS)
- `core/models/track.models.ts` — TypeScript interfaces for Artist, Track, Dsp,
  TrackDistribution, and request DTOs
- `core/services/` — `ArtistService`, `TrackService`, `DspService`, `AuthService`, each mapped
  to the endpoints described in the task spec
- `core/interceptors/auth.interceptor.ts` — functional HTTP interceptor that attaches the JWT
  as a Bearer token to every outgoing request
- The two required views: `features/tracks/track-list` and `features/tracks/track-detail`,
  including templates, component logic, and CSS
- `app.routes.ts`, `app.config.ts` (HttpClient + interceptor wiring)
- The root `app.component` shell, including a manual "Set JWT token" input used for testing
  protected endpoints without building a full login screen
- `proxy.conf.json` for local CORS-free development
- The project README (merged with my backend README into one root document)

**What I changed / had to fix myself:**
- The AI initially assumed REST-conventional routes straight from the task spec
  (`GET /api/artists`, `GET /api/tracks`). My actual controllers use different route names
  (`GET /api/artists/GetAllArtist`, `GET /api/tracks/GetAllTracks`), so I corrected
  `artist.service.ts` (and need to double check `track.service.ts`) to match my real backend
  once I shared the actual controller code.
- I verified the CORS configuration against my real `Program.cs` pipeline ordering
  (`UseCors` must run before `UseAuthentication`) rather than trusting the first draft blindly.
- After testing against the real running backend (via Postman), I found the generated
  services called REST-conventional URLs straight from the task spec
  (`GET /api/artists`, `GET /api/tracks`) that didn't match my actual controller routes.
  I updated `artist.service.ts` and `track.service.ts` so `getArtists()` and `getTracks()`
  call `GET /api/artists/GetAllArtist` and `GET /api/tracks/GetAllTracks` respectively —
  matching my real `[HttpGet("GetAllArtist")]` / `[HttpGet("GetAllTracks")]` controller
  actions instead of the spec's assumed plain routes.

### Backend (.NET)

- AI (Claude) helped with the **database seed data** (`DbSeeder`) and with **refactoring
  `Program.cs`** — specifically restructuring the middleware pipeline and adding the CORS
  policy registration/ordering (`UseCors` placed before `UseAuthentication`), and reviewing
  the JWT configuration wiring.
- Everything else in the backend — entities, EF Core configuration, controllers, JWT token
  generation/validation logic, and the overall Clean Architecture structure — was written by
  me before AI assistance was used on this project.

---

## 2. What security issues did you find (or introduce) in the AI-generated code? How did you handle them?

Issues identified in the AI-generated frontend, and how they were addressed:

- **JWT storage: AI suggested `localStorage`, which was wrong.** The AI-generated
  `AuthService` initially persisted the token in `localStorage` so it would survive page
  reloads during manual testing. I identified this as the wrong choice and switched to storing
  the token in a cookie instead, which is the safer approach against XSS-based token theft.
  This distinction matters much more at a larger scale than this project — here, JWT auth is
  only exercised on a single endpoint with hardcoded credentials (username/password checked
  against values from `appsettings`), which isn't representative of a real production
  authentication flow. In a real-world scenario with a genuine user base and login system,
  the storage mechanism (and the auth flow around it — refresh tokens, expiry, etc.) would
  need much more rigor than what this small project required.

No other security issues were found — this is a small, scoped project (a single JWT-protected
test endpoint, no real user accounts, no production deployment), so concerns like CORS
hardening, refresh-token flows, or backend authorization-boundary audits weren't applicable
here the way they would be on a larger system.

---

## 3. One thing the AI got wrong is that you had to fix. What was it and why was it wrong?

The AI's first pass at the Angular services assumed the backend would expose plain
REST-conventional routes exactly as listed in the task spec's endpoint table
(`GET /api/artists`, `GET /api/tracks`, etc.). This was wrong because it was inferred purely
from the spec document, without access to the actual backend source code — the real
`ArtistsController` uses `[HttpGet("GetAllArtist")]` instead of a bare `[HttpGet]`, and the
real Tracks endpoint is `GetAllTracks`. Had I not caught this by testing against the real
running API (via Postman, where the mismatch produced a routing error), the frontend would
have silently called the wrong URLs and failed with 404s that could easily be mistaken for a
CORS or auth problem instead of a routing mismatch. The fix was to update the service method
URLs to match the actual controller routes once I confirmed them, rather than trusting the
spec's table as ground truth.

