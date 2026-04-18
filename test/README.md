# CanisUIForge Test Infrastructure

End-to-end test infrastructure for verifying the CanisUIForge code generation pipeline.

## Structure

```
test/
├── docker-compose.yml              # SQL Server container for TestApi
├── docker-db.sh                    # Docker helper (up/down/status)
├── init-db.sql                     # Database schema + seed data
├── run-tests.sh                    # Full E2E test runner
├── swagger.json                    # OpenAPI spec for TestApi
├── config/
│   └── forge.json                  # CanisUIForge config for test generation
├── TestApi/                        # Small API mimicking CanisApiForge output
│   ├── TestApi.Contracts/          # Request/response models
│   ├── TestApi.Data/               # EF Core entities + DbContext
│   └── TestApi.Api/                # ASP.NET Core controllers
└── CanisUIForge.IntegrationTests/  # xUnit integration test project
```

## Quick Start

### Run Integration Tests (no Docker required)

```bash
./test/run-tests.sh
```

This builds the main solution, the TestApi, and runs all integration tests.

### Start Database (for running TestApi manually)

```bash
./test/docker-db.sh up      # Start SQL Server + init database
./test/docker-db.sh status   # Check container status
./test/docker-db.sh down     # Stop and remove container
```

### Run TestApi Locally

```bash
./test/docker-db.sh up
cd test/TestApi
dotnet run --project TestApi.Api
# API available at http://localhost:5200
# Swagger UI at http://localhost:5200/swagger
```

## Test API

The TestApi mimics the output of [CanisApiForge](https://github.com/RuanVermeulen/CanisApiForge) with three entities:

| Entity   | Endpoints                                    |
|----------|----------------------------------------------|
| Customer | GET, GET/{id}, POST, PUT/{id}, DELETE/{id}, POST/search |
| Product  | GET, GET/{id}, POST, PUT/{id}, DELETE/{id}   |
| Order    | GET, GET/{id}, POST, PUT/{id}, DELETE/{id}   |

## Integration Tests

The `CanisUIForge.IntegrationTests` project exercises the full pipeline:

- **SwaggerScanningTests** — Verifies OpenAPI scanning, resource detection, endpoint classification
- **ContractsResolutionTests** — Verifies assembly loading and type registry population
- **ConfigValidationTests** — Verifies configuration validation rules
- **PlanBuildingTests** — Verifies generation plan construction and metadata unification
- **FullPipelineTests** — End-to-end: scan → resolve → validate → plan → generate → verify output
