# Veritas

Veritas is a quiet, bilingual Catholic prayer website built with ASP.NET Core Razor Pages. It offers daily readings and reflection, traditional prayers, a Rosary guide, and a guide to the Order of Mass in a calm, responsive design.

## Run locally

Install the .NET 10 SDK, then run:

```bash
cd aspnet
dotnet run
```

Open the local URL printed by `dotnet run`.

## Run with Docker

From the repository root:

```bash
docker compose up --build
```

Open <http://localhost:3000>.

## Pages

| Page | English | 繁體中文 |
| --- | --- | --- |
| Home | `/` | `/zh-tw/` |
| Daily readings & reflection | `/readings` | `/zh-tw/readings` |
| Prayer library | `/prayer-library` | `/zh-tw/prayer-library` |
| The Rosary | `/rosary` | `/zh-tw/rosary` |
| Order of Holy Mass | `/holy-mass` | `/zh-tw/holy-mass` |
| About Veritas | `/about` | `/zh-tw/about` |

Every page is rendered as HTML by ASP.NET Core Razor Pages. Prayer intentions and their API endpoints have been removed from the ASP.NET site.

## Deploy to Render

The repository includes a Render Blueprint in `render.yaml`. Connect the GitHub repository in Render using **New → Blueprint**, then deploy the `veritas` web service. It uses the free plan; the service may spin down when idle. No database or persistent disk is required by the site.

## Project structure

- `aspnet/Pages` — Razor Pages and shared HTML partials
- `aspnet/wwwroot/css/site.css` — responsive site styles
- `aspnet/Dockerfile` — multi-stage .NET container build
- `render.yaml` — Render deployment configuration
- `docker-compose.yml` — local Docker configuration
