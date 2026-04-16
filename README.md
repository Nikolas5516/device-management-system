# Device Management System

A full-stack web application for tracking company mobile devices, their details, 
locations, and assigned users.

## Features

- **Device Management** — Full CRUD for devices (phones, tablets) with validation
- **User Management** — Track users and their assigned devices
- **Authentication** — JWT-based register/login system
- **Device Assignment** — Authenticated users can assign devices to themselves
- **AI Descriptions** — Generate device descriptions using Google Gemini API
- **Ranked Search** — Free-text search with weighted relevance scoring

## Tech Stack

**Backend**
- C# / ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- SQL Server (via Docker)
- JWT Bearer Authentication
- BCrypt password hashing
- Google Gemini API

**Frontend**
- Angular 21 (standalone components)
- TypeScript
- Reactive Forms with validation
- HTTP Interceptors for JWT
- Router guards for protected routes

**Database**
- Microsoft SQL Server 2022 (Docker container)
- Idempotent SQL scripts for setup and seeding

## Prerequisites

Before running the project, install:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI](https://angular.dev/tools/cli): `npm install -g @angular/cli`
- A [Google Gemini API key](https://aistudio.google.com/apikey) (free tier)

## How to Run Locally

### 1. Clone the repository

```bash
git clone https://github.com/Nikolas5516/device-management-system.git
cd device-management-system
```

### 2. Start the SQL Server database

From the repo root:

```bash
docker compose up -d
```

Wait 15-20 seconds for SQL Server to fully start.

### 3. Initialize the database

```bash
docker cp db-scripts/01-create-database.sql device-mgmt-db:/tmp/
docker cp db-scripts/02-seed-data.sql device-mgmt-db:/tmp/

docker exec -it device-mgmt-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevManager2026!" -C -i /tmp/01-create-database.sql

docker exec -it device-mgmt-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "DevManager2026!" -C -i /tmp/02-seed-data.sql
```

The scripts are idempotent, it is safe to run multiple times.

### 4. Configure the backend

Copy the example settings file:

```bash
cp backend/appsettings.Example.json backend/appsettings.json
```

Open `backend/appsettings.json` and fill in:
- **Gemini API Key** — from [Google AI Studio](https://aistudio.google.com/apikey)
- The database connection string (password already set to match docker-compose)
- The JWT key can stay as-is for local testing

### 5. Run the backend

```bash
cd backend
dotnet restore
dotnet run
```

The API will start at `http://localhost:5154`. Swagger UI is available at `http://localhost:5154/swagger`.

### 6. Run the frontend

In a new terminal:

```bash
cd frontend
npm install
ng serve
```

Open your browser at `http://localhost:4200`.

### 7. Use the application

1. **Register** a new account at `/register`
2. **Login** at `/login`
3. Browse, search, create, edit, delete devices
4. Click "Assign to me" on an unassigned device
5. Click "Generate with AI" on any device detail page

## Project Structure

```
device-management-system/
├── backend/                    # ASP.NET Core Web API
│   ├── Controllers/            # API endpoints
│   ├── Services/               # Business logic
│   ├── Models/                 # EF Core entities
│   ├── DTOs/                   # API contracts
│   ├── Data/                   # DbContext
│   └── Program.cs              # App startup
├── frontend/                   # Angular application
│   └── src/app/
│       ├── components/         # Pages (list, detail, form, login, register)
│       ├── services/           # API clients + auth
│       └── models/             # TypeScript interfaces
├── db-scripts/                 # Idempotent SQL setup
└── docker-compose.yml          # SQL Server container
```

## API Endpoints

### Public
- `POST /api/auth/register` — Create account
- `POST /api/auth/login` — Sign in, get JWT token

### Authenticated
- `GET /api/devices` — List all devices
- `GET /api/devices/:id` — Get device details
- `GET /api/devices/search?q=query` — Ranked search
- `POST /api/devices` — Create device (409 if duplicate name)
- `PUT /api/devices/:id` — Update device
- `DELETE /api/devices/:id` — Delete device
- `POST /api/devices/:id/assign` — Assign device to current user
- `POST /api/devices/:id/unassign` — Unassign a device
- `POST /api/devices/:id/generate-description` — AI-generated description
- `GET /api/users` — List all users
- `GET /api/users/:id` — Get user details

## Search Relevance Ranking

The search endpoint uses weighted scoring:

| Field        | Weight |
|--------------|--------|
| Name         | 4      |
| Manufacturer | 3      |
| Processor    | 2      |
| RAM          | 1      |

Query normalization: case-insensitive, punctuation and extra spaces removed, 
then tokenized. Each token is checked against each field. Matches add weighted 
points to the device's score. Results are returned sorted by total score descending.

## Notes

- The `appsettings.json` file is gitignored for data protection. Use `appsettings.Example.json` as a template.
- Gemini API has a free tier with daily limits. If you hit the limit, wait for quota reset.