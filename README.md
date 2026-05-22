# SentinelOps

SentinelOps is a DevOps incident management platform with built-in AI analysis. You can create and track incidents, monitor alerts, and let GPT-4o suggest root causes and fixes. Status updates and AI results push in real time via SignalR.

## Stack

**Backend:** .NET 9 / ASP.NET Core, Clean Architecture with CQRS and MediatR, EF Core + PostgreSQL, SignalR, OpenAI SDK

**Frontend:** Angular 18, Angular Material

**Infrastructure:** Docker Compose (PostgreSQL 16, Redis, RabbitMQ)

## Running locally

**Start the infrastructure**

```bash
cp .env.example .env
# set a real POSTGRES_PASSWORD in .env
docker compose up -d
```

**Configure and run the API**

```bash
cp backend/SentinelOps.API/appsettings.json backend/SentinelOps.API/appsettings.Development.json
# fill in DB password, a 32+ char JWT key, and your OpenAI key

cd backend
dotnet ef database update --project SentinelOps.Infrastructure --startup-project SentinelOps.API
dotnet run --project SentinelOps.API
# http://localhost:5076
```

**Run the frontend**

```bash
cd frontend/sentinelops-ui
npm install
npm start
# http://localhost:4200
```

## Project layout

```
backend/
  SentinelOps.Domain          entities, enums, repository interfaces
  SentinelOps.Application     commands, queries, validators, DTOs
  SentinelOps.Infrastructure  EF Core, JWT, OpenAI, SignalR hub
  SentinelOps.API             controllers, startup
frontend/
  sentinelops-ui              Angular 18 standalone components
docker-compose.yml
```

## Secrets

Copy `.env.example` to `.env` for Docker and create `appsettings.Development.json` for the API. Neither file is committed. The committed `appsettings.json` and `docker-compose.yml` contain only placeholders.
