# CanisUIForge

A Swagger + Contracts-driven code generator that produces fully structured **Blazor** and **.NET MAUI** applications from your API definition.

CanisUIForge enforces layered architecture, reusable components, centralised styling, contracts-first type resolution, safe regeneration, and full test scaffolding — all from a single configuration.

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A Swagger/OpenAPI JSON file (local or URL)
- A Contracts assembly (project reference or NuGet package) containing your request/response models

---

## Building

```bash
dotnet build CanisUIForge.sln
```

---

## Running

CanisUIForge can be used in two ways: **CLI** (command-line) or **Desktop** (Avalonia wizard UI).

### CLI Mode

Run the CLI from the build output or via `dotnet run`:

```bash
dotnet run --project src/CanisUIForge.Cli -- <command> [options]
```

#### Commands

| Command    | Description                                              |
|------------|----------------------------------------------------------|
| `generate` | Generate UI application from Swagger and Contracts       |
| `scan`     | Scan Swagger and display discovered resources/endpoints  |
| `preview`  | Preview the generation plan without writing any files    |

#### Using a Configuration File

The recommended approach is to create a JSON configuration file:

```bash
dotnet run --project src/CanisUIForge.Cli -- generate --config forge.json
```

#### Using Inline Arguments

You can also pass all options directly:

```bash
dotnet run --project src/CanisUIForge.Cli -- generate \
  --name MyApp \
  --swagger swagger.json \
  --contracts-mode project \
  --contracts-project ../MyApp.Contracts/MyApp.Contracts.csproj \
  --targets blazor,maui \
  --output ./generated \
  --namespace MyApp \
  --unit-tests \
  --playwright-tests \
  --appium-tests
```

#### CLI Options Reference

| Option                       | Description                                        |
|------------------------------|----------------------------------------------------|
| `--config <path>`            | Path to JSON configuration file                    |
| `--name <name>`              | Solution name                                      |
| `--swagger <path>`           | Path or URL to Swagger JSON                        |
| `--contracts-mode <mode>`    | `project` or `nuget`                               |
| `--contracts-project <path>` | Contracts `.csproj` path (project mode)             |
| `--contracts-package <id>`   | NuGet package ID (nuget mode)                      |
| `--contracts-version <ver>`  | NuGet package version (nuget mode)                 |
| `--contracts-feed <path>`    | Local NuGet feed path (optional, nuget mode)       |
| `--targets <platforms>`      | `blazor`, `maui`, or `both`                        |
| `--output <path>`            | Output directory for generated solution             |
| `--namespace <ns>`           | Root namespace (defaults to solution name)          |
| `--unit-tests`               | Enable unit test generation                        |
| `--playwright-tests`         | Enable Playwright test generation (Blazor)         |
| `--appium-tests`             | Enable Appium test generation (MAUI)               |

#### Scanning and Previewing

Inspect your Swagger before generating:

```bash
# List all discovered resources and endpoints
dotnet run --project src/CanisUIForge.Cli -- scan --swagger swagger.json

# Preview the full generation plan
dotnet run --project src/CanisUIForge.Cli -- preview --config forge.json
```

### Desktop Mode (Avalonia)

The Avalonia project provides a step-by-step wizard UI:

1. **Project Setup** — solution name, output path, namespace, target platforms, test options
2. **Swagger Input** — swagger source and contracts configuration
3. **Controller Selection** — choose which controllers to generate and their UI style
4. **Preview** — review the generation plan with validation warnings
5. **Generation** — execute with live progress log

The Avalonia UI uses the same generation pipeline as the CLI.

---

## Configuration File

Create a `forge.json` file:

```json
{
  "solutionName": "AdminPortal",
  "targets": ["Blazor", "Maui"],
  "swaggerSource": "./swagger.json",
  "outputPath": "./generated",
  "namespaceRoot": "AdminPortal",
  "contracts": {
    "mode": "ProjectReference",
    "projectPath": "../AdminPortal.Contracts/AdminPortal.Contracts.csproj"
  },
  "controllers": [
    { "name": "Users", "style": "FormAndGrid" },
    { "name": "Products", "style": "Grid" },
    { "name": "Orders", "style": "Search" }
  ],
  "tests": {
    "unit": true,
    "playwright": true,
    "appium": true
  }
}
```

### Contracts Modes

| Mode               | Required Fields                          | Description                              |
|--------------------|------------------------------------------|------------------------------------------|
| `ProjectReference` | `projectPath`                            | Reference a local `.csproj` containing your DTOs |
| `NuGetReference`   | `packageId`, `packageVersion`            | Restore a NuGet package containing your DTOs     |

For NuGet mode with a local feed:

```json
{
  "contracts": {
    "mode": "NuGetReference",
    "packageId": "MyApp.Contracts",
    "packageVersion": "1.0.0",
    "localFeed": "./packages"
  }
}
```

### Generation Styles

Each controller can be assigned a UI generation style:

| Style         | Generates                                       |
|---------------|--------------------------------------------------|
| `Grid`        | List page with data grid                         |
| `Form`        | Create and edit pages with form layouts           |
| `FormAndGrid` | Both grid list page and form create/edit pages    |
| `Search`      | Search page with filtering and results display    |

### Target Platforms

| Target   | Output                              |
|----------|--------------------------------------|
| `Blazor` | Blazor web application               |
| `Maui`   | .NET MAUI cross-platform application |
| `Both`   | Both Blazor and MAUI simultaneously  |

---

## Generated Output Structure

Running `generate` produces a complete solution:

```
generated/
├── AdminPortal.sln
├── AdminPortal.Blazor/
│   ├── Components/
│   │   ├── DataGrid/
│   │   ├── Forms/
│   │   ├── Panels/
│   │   └── Dialogs/
│   ├── Pages/
│   │   ├── Users/
│   │   └── Products/
│   ├── Services/
│   ├── Layout/
│   └── Styles/
├── AdminPortal.Maui/
│   ├── Views/
│   │   ├── Components/
│   │   └── Pages/
│   ├── Services/
│   └── Resources/
├── AdminPortal.Tests/
│   ├── Unit/
│   ├── Playwright/
│   └── Appium/
```

---

## Contracts-First Design

CanisUIForge does **not** generate DTOs from Swagger. Instead:

- **Swagger** provides endpoint discovery, routes, HTTP methods, and controller grouping
- **Contracts** provide request/response models, namespaces, and type correctness

Your Contracts assembly must contain the classes referenced by the Swagger schema names. The generator resolves schema names to CLR types from your assembly at generation time.

---

## Safe Regeneration

CanisUIForge is designed for repeated, safe regeneration:

### How It Works

Every generated file is stamped with an `<auto-generated>` header comment:

```csharp
// <auto-generated>
// This file was generated by CanisUIForge. Do not modify manually.
// Changes will be overwritten on next generation.
// </auto-generated>
```

On subsequent runs:

- **Files with the header** → overwritten (safe to regenerate)
- **Files without the header** → skipped (manual modifications preserved)
- **New files** → created normally

The CLI reports a summary after each run showing created, overwritten, and skipped counts.

### Extending Generated Code

To customise generated code without losing changes on regeneration:

1. **Do not edit files with the `<auto-generated>` header** — they will be overwritten
2. **Remove the header** from any generated file you want to take ownership of — the regenerator will skip it
3. **Use partial classes** to add behaviour alongside generated code
4. **Add new files** freely — the generator only writes to its own known paths

---

## Project Architecture

```
src/
├── CanisUIForge.Core          Core configuration, enums, shared models
├── CanisUIForge.OpenApi        Swagger loading, parsing, endpoint classification
├── CanisUIForge.Contracts      Assembly loading, type resolution, schema mapping
├── CanisUIForge.Generation     Plan building, template engine, file writing, logging, validation
├── CanisUIForge.Blazor         Blazor code generators and templates
├── CanisUIForge.Maui           MAUI code generators and templates
├── CanisUIForge.Testing        Unit, Playwright, and Appium test generators
├── CanisUIForge.Cli            Command-line interface
└── CanisUIForge.Avalonia       Desktop wizard UI (Avalonia)
```

### Dependency Flow

```
Core ← OpenApi
Core ← Contracts
Core ← Generation
Generation ← Blazor
Generation ← Maui
Generation ← Testing
Core, OpenApi, Contracts, Generation, Blazor, Maui, Testing ← Cli
Core, OpenApi, Contracts, Generation, Blazor, Maui, Testing ← Avalonia
```

### Generation Pipeline

1. **Load configuration** — JSON file or CLI arguments
2. **Validate configuration** — solution name, targets, swagger source, contracts
3. **Validate contracts source** — project file exists or NuGet package is accessible
4. **Scan Swagger** — parse OpenAPI definition into resources and endpoints
5. **Resolve contracts** — load assembly and map schema names to CLR types
6. **Validate schema resolution** — warn on unresolved schemas
7. **Build generation plan** — merge metadata, apply controller styles, filter resources
8. **Execute generation** — run platform-specific generators writing templated output

### Template System

Generators use embedded `.sbn` template files with `{{placeholder}}` string replacement. Templates are embedded as assembly resources in each generator project (Blazor, Maui, Testing).

---

## Coding Standards

All generated code follows these rules:

- Explicit types only (no `var`)
- Global usings via `GlobalUsings.cs`
- No `ConfigureAwait` calls
- Async/await patterns throughout
- Each class in its own file
- Nullable reference types enabled

---

## License

See [LICENSE](LICENSE) for details.
