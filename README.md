# ErrorReporter

A REST API for collecting and querying error reports from your applications. Services authenticate with API keys, submit errors, and you can filter, paginate, and summarize them through the API.

Built with ASP.NET Core, Entity Framework Core, and PostgreSQL.

## Features

- **Error reporting** — submit errors with service name, message, stack trace, severity, and timestamp
- **Filtering & pagination** — query errors by service, date range, with paginated responses
- **Summaries** — get error counts grouped by service with most recent occurrence
- **API key authentication** — each client gets a unique key, hashed with SHA-256 before storage
- **Client management** — create and revoke API keys through admin endpoints
- **Rate limiting** — 60 requests per minute per API key

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (local install or Docker)

### Setup

1. Clone the repo and navigate to the project:
   ```bash
   git clone https://github.com/your-username/ErrorReporter.git
   cd ErrorReporter/ErrorReporter
   ```

2. Set up your database connection in `appsettings.json` (or use the default `localhost:5432` with user `dev`).

3. Set the master API key using .NET User Secrets:
   ```bash
   dotnet user-secrets set "Authentication:MasterApiKey" "your-master-key-here"
   ```

4. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

5. Run the app:
   ```bash
   dotnet run
   ```

## API Usage

### Register a new client (admin only)

```bash
curl -X POST http://localhost:5087/admin/clients \
  -H "X-Api-Key: your-master-key-here" \
  -H "Content-Type: application/json" \
  -d '{"name": "MyApp"}'
```

Response:
```json
{
  "id": 1,
  "name": "MyApp",
  "apiKey": "abc123...",
  "message": "Store this key securely. It cannot be retrieved again."
}
```

### Submit an error

```bash
curl -X POST http://localhost:5087/errors \
  -H "X-Api-Key: abc123..." \
  -H "Content-Type: application/json" \
  -d '{
    "service": "PaymentService",
    "message": "Unhandled exception in checkout flow",
    "stackTrace": "at PaymentService.Process() line 42...",
    "severity": "Error",
    "occurredAt": "2026-03-17T14:30:00Z"
  }'
```

### Query errors

```bash
# Get all errors (paginated)
curl http://localhost:5087/errors?page=1&pageSize=10 \
  -H "X-Api-Key: abc123"

# Filter by service and date range
curl "http://localhost:5087/errors?service=PaymentService&from=2026-03-01&to=2026-03-31" \
  -H "X-Api-Key: abc123"
```

### Get a summary

```bash
curl http://localhost:5087/errors/summary \
  -H "X-Api-Key: abc123"
```

Response:
```json
[
  { "service": "PaymentService", "count": 12, "mostRecentOccurrence": "2026-03-17T14:30:00Z" },
  { "service": "AuthService", "count": 3, "mostRecentOccurrence": "2026-03-16T09:15:00Z" }
]
```

### Revoke a client

```bash
curl -X DELETE http://localhost:5087/admin/clients/1 \
  -H "X-Api-Key: your-master-key-here"
```

