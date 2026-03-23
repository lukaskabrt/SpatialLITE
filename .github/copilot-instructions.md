# GitHub Copilot Instructions

## Build, Test, and Lint

All commands run from the repository root. The solution path is `src/SpatialLite.sln`.

```shell
# Build
dotnet build src/SpatialLite.sln

# Run all tests
dotnet test src/SpatialLite.sln

# Run a single test by fully qualified name
dotnet test src/SpatialLite.sln --filter "FullyQualifiedName~WkbReaderTests.Read_ReadsGeometry"

# Run tests in a single project
dotnet test src/Tests/SpatialLite.UnitTests

# Lint / format check (CI enforces this). Locally, install dotnet-format as a global tool first:
#   dotnet tool install -g dotnet-format
dotnet format src/SpatialLite.sln --verify-no-changes
```

Integration tests (`SpatialLite.IntegrationTests`) use `osmium-tool`. On Linux, a system `osmium` binary must be installed and available on `PATH`; on Windows, the project uses the bundled `Tools/Osmium/Windows/osmium.exe` by default.

## Architecture

SpatialLite is a .NET 8.0 library for spatial data processing, distributed as multiple NuGet packages.

**Project dependency graph:**

```
SpatialLite.Contracts  ← Core interfaces and value types (IGeometry, IPoint, Coordinate, Envelope)
        ↑                          ↑
SpatialLite.Core       ← Geometry implementations (Point, LineString, Polygon, Multi*) + WKT/WKB I/O
        ↑
SpatialLite.Gpx        ← GPX format reader/writer
SpatialLite.Osm        ← OpenStreetMap XML and PBF format readers/writers (depends on SpatialLite.Contracts + protobuf-net)
```

**Contracts vs. implementation separation:** Interfaces and value types live in `SpatialLite.Contracts`. Concrete geometry classes and I/O live in the other projects. Readers/writers accept and return interface types (`IGeometry`, `IPoint`, etc.).

**Reader/writer pattern** — I/O classes generally follow this structure:
- Two constructor overloads: `Stream` (caller manages lifecycle) and `string path` (class owns the `FileStream`)
- Instance `Read`/`Write` methods for streaming through a file
- Many provide static `Parse`/`Write` helpers for one-shot operations, but this is not required for all formats
- `IDisposable` implemented with a private `Dispose(bool disposing)` helper and no finalizer, unless unmanaged resources require the full pattern
- Some formats have an optional `Settings` class for configuration (e.g., `GpxWriterSettings`, `OsmReaderSettings`)

## Code Style

Follow `.editorconfig` — it is comprehensive and enforced as warnings at build time. Key points:

- File-scoped namespaces (`namespace X;` not `namespace X { }`)
- Always use `var`
- Always use braces on control flow
- Prefer pattern matching over `is`/`as` with null checks
- Private fields: `_camelCase`; private constants: `PascalCase`; private static readonly: `PascalCase`
- Use CRLF line endings, UTF-8 encoding, 4-space indent, and keep lines under 160 characters; configure your editor accordingly (not all of these are enforced by `.editorconfig`)
- `using` directives outside namespace
- Nullable enabled, implicit usings enabled, warnings treated as errors

## Testing Conventions

- **Framework:** xUnit only — use `Assert.*` methods, not FluentAssertions
- **Test naming:** `MethodName_Scenario_ExpectedResult` (e.g., `Constructor_IEnumerable_CreatesCollectionWithGivenTags`)
- **Parameterized tests:** `[Theory]` with `[InlineData(...)]`
- **Test data files:** accessed via static `TestDataReader` instances (`TestDataReader.CoreIO`, `TestDataReader.OsmXml`, etc.) which provide `Open(filename)`, `Read(filename)`, and `GetPath(filename)` methods
- **Large test classes:** split into partial classes by functionality (e.g., `EuclideanDistanceCalculatorTests.PointToPoint.cs`)
- **Test structure mirrors source:** `SpatialLite.UnitTests/Core/IO/` tests `SpatialLite.Core/IO/`

## Package Management

Central package management is enabled via `src/Directory.Packages.props`. When adding NuGet packages, add the version there and reference without version in the `.csproj`.
