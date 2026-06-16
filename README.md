# Slotegrator.TechTask — Players API(`https://testslotegrator.com/automation`)

Automated REST API tests for the Players API(`https://testslotegrator.com/automation`), written in **.NET 9** with **NUnit**, **FluentAssertions**, and a custom `HttpClient` wrapper. 

## Tech stack

| Concern | Choice |
| --- | --- |
| Runtime | .NET 9  |
| Test framework | NUnit 4  |
| Assertions | FluentAssertions 7 |
| HTTP client | `HttpClient` + custom wrapper (`SlotegratorApiClient`) |
| Target system | `https://testslotegrator.com` |

## Project structure

| Project | Responsibility |
| --- | --- |
| `Slotegrator.Tools` | Reusable API layer: `SlotegratorApiClient` (HTTP + auth), `LoginApi`/`PlayerApi` (endpoint objects), request/response DTOs, `PlayerBuilder`/`PlayerFactory` test-data, config, constants, request/response logging. |
| `Slotegrator.ServiceTests` | NUnit test fixtures and the test classes. |

## Endpoints under test

| Action | Method | Path |
| --- | --- | --- | 
| Login | `POST` | `api/tester/login` |
| Create player | `POST` | `api/automationTask/create` | 
| Get one player | `POST` | `api/automationTask/getOne` |
| Get all players | `GET` | `api/automationTask/getAll` | 
| Delete one player | `DELETE` | `api/automationTask/deleteOne/{id}` |

> **Testing NOTES** (discovered while testing, captured here some strange behaviour):
> - Create/delete endpoints responses with `_id`(with underscore), while `getOne` respond with `id` - write custom handle for this in `PlayerResponseDto` 
> - "Player not found" on `getOne` returns `400 Bad Request` instead `404`.
> -  `api/automationTask/getOne` actually call `POST` method - handled in tests , but from a REST perspective, it is incorrect.
> -  `api/automationTask/deleteOne/{id}` supposed to accept a player ID as an `integer` according swagger, but it actually accepts a `string`

## How it works

1. Each fixture (`TestFixture`) loads `config.json`, creates one `SlotegratorApiClient` for the configured `BaseUrl`, and exposes `LoginApi`/`PlayerApi`.
2. Player test classes fetch a fresh Bearer token and reset the account to a clean state (delete all players) in `[SetUp]` (`DeleteAllPlayersAsync`), so every test starts from empty "start point".
3. Every request/response is logged in a readable form by `LoggingHandler` (visible in the test output).
4. Test data is generated with `PlayerFactory`/`PlayerBuilder`, using a unique email and username per player.

## Test coverage

| Test class | Scenarios |
| --- | --- |
| `LoginTests` | valid credentials → `201` + access token; invalid credentials → `401` |
| `RegisterPlayerTests` | single player → created + body matches request; 12 players (parallel) → all created |
| `GetPlayerDataTests` | existing player → matching data; missing player → `400`; `getAll` → `200` and contains the created players(with sorting) |
| `DeletePlayersTests` | existing player → deleted, then `getOne` → `400`; bulk delete (parallel) → none of the created players remain in `getAll` |

All player endpoints use a `Bearer` token obtained from the login endpoint.

## Configuration

Configuration lives in `Slotegrator.ServiceTests/config.json`:

```json
{
  "BaseUrl": "https://testslotegrator.com/",
  "UserCredentials": {
    "Email": "user@example.com",
    "Password": "your-password"
  }
}
```

> **Note on version control.** !!! Ideally `config.json` should be added to `.gitignore`, since it holds login credentials. In a real solution this file must **not** be committed to git.
> All sensitive data like that should be taken from CI/CD tools like KeyVault or ENV variables.
> Commited only for test purposes.

## Running the tests

Prerequisite: the [.NET 9 SDK](https://dotnet.microsoft.com/download) and network access to `https://testslotegrator.com`.

```bash
# Restore + build
dotnet build

# Run the whole suite
dotnet test
```

### Notes about Parallel Execution model 
Tests run **sequentially** (`[assembly: NonParallelizable]` in `Slotegrator.ServiceTests/AssemblyInfo.cs`), and each player test resets the account to a clean state in `[SetUp]`.

Tests runs against a **single shared API account**, where `getAll` exposes global state and there is no way to partition data per test. Under these conditions, true parallel isolation is impossible without introducing workarounds and logically separating data, which would complicate the architecture. 
"reset before each test" strategy gives the cleanest tests.

In a real environment with per-worker accounts, the tests would be parallelized for speed (each worker isolated to its own data).
