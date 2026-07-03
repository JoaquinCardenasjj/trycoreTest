# Trycore EVM — Backend (.NET 8)

API REST para el seguimiento de proyectos mediante indicadores de **Valor Ganado**
(Earned Value Management — EVM), desarrollada para el desafío técnico de Ingeniero de
Desarrollo .NET de Trycore Colombia.

> Este repositorio contiene el **backend**. El frontend en Angular se gestiona como un
> proyecto aparte (ver sección "Frontend" al final).

## Arquitectura

Clean Architecture / Onion Architecture con 4 capas, cada una en su propio proyecto:

```
src/
  TrycoreEvm.Domain          Entidades, value objects y la calculadora EVM (lógica pura,
                              sin dependencias externas).
  TrycoreEvm.Application     Casos de uso (servicios), DTOs, mapeos e interfaces de
                              persistencia (puertos).
  TrycoreEvm.Infrastructure  Implementación de persistencia con EF Core + PostgreSQL
                              (adaptadores).
  TrycoreEvm.Api              Controladores REST, Swagger, middleware de errores.

tests/
  TrycoreEvm.UnitTests         Pruebas unitarias de dominio y aplicación (xUnit + Moq +
                                FluentAssertions).
  TrycoreEvm.IntegrationTests  Pruebas de integración de cada endpoint con
                                WebApplicationFactory + EF Core InMemory.
```

Regla de dependencia: `Api → Infrastructure → Application → Domain`. La lógica de
negocio (cálculo EVM, validaciones, invariantes) vive en `Domain`; los controladores
son delgados y delegan todo en `Application`.

### ¿Por qué el Valor Ganado?

La lógica central está en
[`EvmCalculator`](src/TrycoreEvm.Domain/Services/EvmCalculator.cs), una clase estática
y pura (mismas entradas → mismas salidas, sin efectos secundarios), lo que la hace
fácil de cubrir al 100% con pruebas unitarias. Calcula:

| Indicador | Fórmula                | Notas de implementación |
|-----------|-------------------------|--------------------------|
| PV        | % planificado × BAC     | — |
| EV        | % completado × BAC      | — |
| CV        | EV − AC                 | — |
| SV        | EV − PV                 | — |
| CPI       | EV / AC                 | Si AC = 0, el índice queda `null` (indefinido), no se lanza excepción. |
| SPI       | EV / PV                 | Si PV = 0, el índice queda `null` por la misma razón. |
| EAC       | BAC / CPI                | Si CPI es `null` o `0`, se usa el respaldo `AC + (BAC − EV)`. |
| VAC       | BAC − EAC                | `null` si EAC no pudo calcularse. |

## Cómo correr el proyecto localmente

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 14+ (o Docker, ver abajo)

### 1. Levantar PostgreSQL

Con Docker (recomendado):

```bash
docker compose up -d
```

Esto levanta PostgreSQL en `localhost:5432` con la base `trycore_evm`
(usuario/clave `postgres`/`postgres`, ver `docker-compose.yml`).

Si prefieres una instancia propia de PostgreSQL, ajusta la cadena de conexión
`ConnectionStrings:EvmDatabase` en `src/TrycoreEvm.Api/appsettings.json`.

### 2. Crear el esquema de base de datos

Opción A — con EF Core Migrations (recomendado, genera el historial de migraciones):

```bash
dotnet tool install --global dotnet-ef   # una sola vez
dotnet ef migrations add InitialCreate \
  --project src/TrycoreEvm.Infrastructure \
  --startup-project src/TrycoreEvm.Api
dotnet ef database update \
  --project src/TrycoreEvm.Infrastructure \
  --startup-project src/TrycoreEvm.Api
```

Opción B — ejecutar directamente el script SQL incluido:

```bash
psql -h localhost -U postgres -d trycore_evm -f scripts/init-db.sql
```

### 3. Restaurar, compilar y correr el API

```bash
dotnet restore
dotnet build
dotnet run --project src/TrycoreEvm.Api
```

El API queda disponible en `http://localhost:5080` y la documentación Swagger en:

```
http://localhost:5080/api-docs
```

## Pruebas

```bash
dotnet test
```

- **Unitarias** (`tests/TrycoreEvm.UnitTests`): cubren toda la lógica de cálculo EVM,
  incluyendo los casos borde pedidos explícitamente en el reto:
  - AC = 0 (sin costo real registrado aún).
  - Proyecto sin actividades.
  - Avance real (EV) = 0%.
  - Avance real y costo real = 0 simultáneamente.
  También cubren las validaciones de las entidades de dominio y los servicios de
  aplicación (con repositorios mockeados vía Moq).
- **Integración** (`tests/TrycoreEvm.IntegrationTests`): cada endpoint tiene al menos
  una prueba que valida el contrato de respuesta (código HTTP + forma del payload),
  usando `WebApplicationFactory` con una base de datos EF Core InMemory aislada por
  prueba (no requiere PostgreSQL para correr).

### Cobertura de código

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Esto genera un reporte Cobertura en `tests/**/TestResults/**/coverage.cobertura.xml`.
Para verlo en HTML:

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool   # una sola vez
reportgenerator \
  -reports:"tests/**/TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html
```

El objetivo del reto es **80% mínimo sobre la capa de negocio** (`TrycoreEvm.Domain` y
`TrycoreEvm.Application`); `EvmCalculatorTests` por sí sola cubre el 100% de las rutas
de `EvmCalculator`, incluyendo todos los casos borde.

## Calidad de código / linter

El repositorio usa el analizador y el `.editorconfig` incluidos en la raíz
(`Directory.Build.props` habilita `EnableNETAnalyzers` + `EnforceCodeStyleInBuild`
para todos los proyectos). Para verlos en acción:

```bash
dotnet build -warnaserror
```

Reglas destacadas del `.editorconfig`: variables/miembros sin usar (`IDE0051`,
`IDE0052`, `CS0219`), `using` innecesarios (`IDE0005`) y convención de nombres para
campos privados (`_camelCase`).

## Documentación de la API (OpenAPI/Swagger)

Disponible en `/api-docs` mientras el API está corriendo localmente. Cada endpoint
documenta su descripción, esquema de request/response y códigos de error posibles
(`ProducesResponseType`), y el middleware
[`ExceptionHandlingMiddleware`](src/TrycoreEvm.Api/Middleware/ExceptionHandlingMiddleware.cs)
traduce las excepciones de dominio a respuestas `ProblemDetails` consistentes
(`400` para errores de validación, `404` para entidades no encontradas).

## Endpoints principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/projects` | Lista proyectos con sus indicadores EVM consolidados. |
| POST   | `/api/projects` | Crea un proyecto. |
| GET    | `/api/projects/{projectId}` | Detalle de un proyecto con sus actividades. |
| PUT    | `/api/projects/{projectId}` | Renombra un proyecto. |
| DELETE | `/api/projects/{projectId}` | Elimina un proyecto y sus actividades. |
| POST   | `/api/projects/{projectId}/activities` | Agrega una actividad. |
| PUT    | `/api/projects/{projectId}/activities/{activityId}` | Actualiza una actividad. |
| DELETE | `/api/projects/{projectId}/activities/{activityId}` | Elimina una actividad. |

## Frontend

El dashboard en Angular (tabla de actividades, indicadores consolidados, estado visual
de CPI/SPI y gráfica PV/EV/AC) se implementa como un proyecto Angular independiente que
consume esta API — típicamente en una carpeta `frontend/` al mismo nivel de `src/`, con
`environment.apiUrl` apuntando a `http://localhost:5080/api`. El CORS del API ya está
habilitado para `http://localhost:4200` (puerto por defecto de `ng serve`).

## Nota sobre este entorno de generación

Este proyecto fue escrito íntegramente a mano (sin ejecutar `dotnet build`/`dotnet
test`, porque el entorno donde se generó no tiene el SDK de .NET ni acceso a NuGet). Al
clonarlo, corre `dotnet restore && dotnet build && dotnet test` como primer paso para
confirmar que todo compila y las pruebas pasan en tu máquina; si algo no compila,
avísame y lo corregimos.
