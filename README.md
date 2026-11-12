# Veritas — A quiet place to pray

Veritas is a bilingual Catholic prayer site built with ASP.NET Core Razor Pages and SQLite. Its quiet green and cream design is shared across the English and Traditional Chinese routes. Prayer intentions are stored in SQLite, grouped by language and month, and published only after the submitter gives consent.

## Run with Docker

```bash
docker compose up --build
```

Open <http://localhost:3000>. Docker stores the intentions database in the `veritas-intentions` volume at `/data/intentions.sqlite3`.

## Run locally

Install the .NET 10 SDK, then run:

```bash
cd aspnet
dotnet run
```

The app creates `data/intentions.sqlite3` on first start. Set `INTENTIONS_DB_PATH` to choose another location.

## Pages

- `/` — English home
- `/prayer-intentions` — English intention form and monthly archive
- `/zh-tw/` and `/zh-tw/prayer-intentions` — Traditional Chinese routes
- `/prayer-library`, `/rosary`, `/holy-mass`, and `/readings` — route placeholders retained for the remaining content migration

The ASP.NET app reuses the site's prayer intention table and data path, so an existing SQLite database at `INTENTIONS_DB_PATH` remains available. The API retains the existing contract:

- `GET /api/health/`
- `GET /api/intentions/?month=YYYY-MM&language=en|zh-TW`
- `POST /api/intentions/` with `name`, `location`, `text`, `consent: true`, and `language`

The intention API validates lengths, language, month format, and explicit publication consent. The site form is protected by ASP.NET's antiforgery token.

The ASP.NET port currently contains the shared site shell, home screen, and complete intentions flow. The prayer library, Rosary, Mass, readings, and about screen still need their legacy page content and interactions ported; those route placeholders currently render the home screen.
