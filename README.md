# SentinelOps

An AI-powered DevOps incident management platform built with .NET 9 and Angular 18. Track incidents, triage alerts, and get GPT-4o–powered root cause analysis — all in real time.

---

## Features

- **Incident Management** — Create, assign, and track incidents through their full lifecycle (Open → Acknowledged → Investigating → Resolved → Closed)
- **AI Analysis** — One-click GPT-4o root cause analysis with suggested fixes and severity explanations
- **Real-time Updates** — SignalR push notifications for incident changes and AI analysis completion
- **Alert Tracking** — Monitor active alerts, acknowledge them, and link them to incidents
- **SLA Monitoring** — Deadline tracking with visual breach indicators on the dashboard
- **JWT Auth** — Role-based access (Admin / Engineer / Viewer) with secure token authentication

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 9, ASP.NET Core, Clean Architecture (CQRS + MediatR) |
| Database | PostgreSQL 16 + Entity Framework Core |
| AI | OpenAI GPT-4o via official .NET SDK |
| Real-time | SignalR |
| Frontend | Angular 18, Angular Material |
| Infrastructure | Docker Compose, Redis, RabbitMQ |

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 1. Start infrastructure

```bash
cp .env.example .env
# Edit .env and set a strong POSTGRES_PASSWORD
docker compose up -d
```

### 2. Configure backend

```bash
cp backend/SentinelOps.API/appsettings.json backend/SentinelOps.API/appsettings.Development.json
# Edit appsettings.Development.json — set DB password, JWT key, and OpenAI API key
```

Run migrations:

```bash
cd backend
dotnet ef database update --project SentinelOps.Infrastructure --startup-project SentinelOps.API
```

Start the API:

```bash
dotnet run --project SentinelOps.API
# Runs on http://localhost:5076
```

### 3. Start frontend

```bash
cd frontend/sentinelops-ui
npm install
npm start
# Runs on http://localhost:4200
```

## Project Structure

```
SentinelOps/
├── backend/
│   ├── SentinelOps.Domain          # Entities, enums, repository interfaces
│   ├── SentinelOps.Application     # CQRS commands/queries, DTOs, validators
│   ├── SentinelOps.Infrastructure  # EF Core, JWT, OpenAI, SignalR hub
│   └── SentinelOps.API             # Controllers, middleware, startup
├── frontend/sentinelops-ui         # Angular 18 standalone components
└── docker-compose.yml              # Postgres, Redis, RabbitMQ
```

## Environment Variables

Never commit real secrets. Copy `.env.example` to `.env` and fill in your values:

| Variable | Description |
|---|---|
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `RABBITMQ_USER` / `RABBITMQ_PASS` | RabbitMQ credentials |

Backend secrets go in `appsettings.Development.json` (gitignored):

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | Full Npgsql connection string |
| `Jwt:Key` | Signing key — minimum 32 characters |
| `OpenAI:ApiKey` | Your OpenAI API key |
