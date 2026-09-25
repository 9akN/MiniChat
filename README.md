# MiniChat

A WhatsApp-style chat app: a .NET MAUI client backed by an ASP.NET Core Web API with realtime
messaging over SignalR.

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — full architecture and pattern write-up

## Features

- Register / log in with a username and password (JWT bearer auth, passwords hashed with BCrypt)
- Contacts tab — see every other registered user, tap to start a 1:1 chat
- Chats tab — list of your conversations with a last-message preview and unread badge
- Realtime messaging over SignalR (send/receive, read receipts) with a REST fallback for history
- Settings tab — profile summary and logout

## Project structure

```
ChatApp/
├── ChatApp.slnx
├── src/
│   ├── ChatApp.Api/         ASP.NET Core 10 Web API (REST + SignalR)
│   └── ChatApp.Client/      .NET MAUI 10 client (Android, iOS, MacCatalyst, Windows)
└── docs/
```

## Prerequisites

- .NET 10 SDK
- MAUI workload: `dotnet workload install maui`
- A local SQL Server instance reachable as `(local)` — e.g. SQL Server Developer/Express, or the
  instance that ships with Visual Studio. (LocalDB also works if you'd rather use it — see
  *Database* below.)
- A trusted local HTTPS dev certificate: `dotnet dev-certs https --trust`

## Running the API

```powershell
cd src/ChatApp.Api
dotnet ef database update
dotnet run
```

The API listens on `https://localhost:7072` (and `http://localhost:5203`) by default — see
[`Properties/launchSettings.json`](src/ChatApp.Api/Properties/launchSettings.json). Swagger UI is
available at `/swagger` in the Development environment.

### Database

`ChatAppDb` lives on your default local SQL Server instance, `(local)` — a real Windows-service-hosted
instance rather than a per-user LocalDB instance, which sidesteps LocalDB's tendency to misbehave
when tools connect to it from a different user session (e.g. an elevated SSMS). No password either
way — Windows Authentication (`Trusted_Connection=true`).

The connection string lives in [`appsettings.json`](src/ChatApp.Api/appsettings.json):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(local);Database=ChatAppDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```

To connect with SSMS or Visual Studio's SQL Server Object Explorer, use server name `(local)` (or
your machine name) with Windows Authentication — `ChatAppDb` will show up under Databases once
you've run `dotnet ef database update` at least once.

If you'd rather use LocalDB instead (e.g. on a machine without a full SQL Server instance
installed), change `Server=(local)` to `Server=(localdb)\mssqllocaldb` and re-run
`dotnet ef database update`.

Replace `Jwt:Key` with your own secret before deploying anywhere beyond local development.

## Running the client

```powershell
cd src/ChatApp.Client
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

Or open `ChatApp.slnx` in Visual Studio and run `ChatApp.Client` with your target platform selected
(Windows Machine, Android emulator, etc.).

The client's API base URL is set in
[`Services/ApiConfig.cs`](src/ChatApp.Client/Services/ApiConfig.cs) — `https://localhost:7072` on
Windows/iOS/MacCatalyst, and `http://10.0.2.2:5203` (plain HTTP) on Android specifically. If you
run the API on a different machine or port, update that file.

> Android emulators don't trust the API's local dev HTTPS certificate out of the box, and
> installing it manually (Settings → Encryption & credentials → Install a certificate → CA
> certificate) is finicky — it's a known source of "Private key required" errors on some emulator
> images. Android is configured to hit the API over plain HTTP instead, which Android otherwise
> blocks by default since API 28; [`Platforms/Android/Resources/xml/network_security_config.xml`](src/ChatApp.Client/Platforms/Android/Resources/xml/network_security_config.xml)
> permits cleartext traffic scoped to `10.0.2.2` only. This is a local-dev convenience — a real
> deployed API should be reached over HTTPS from a release build. iOS/MacCatalyst simulators trust
> the host's dev cert automatically and don't need this workaround.

## Try it

1. Start the API (`dotnet run` in `src/ChatApp.Api`).
2. Run the client on two different targets/instances (e.g. Windows + Android emulator), or run it
   twice with two different accounts.
3. Register two accounts.
4. On one account, go to **Contacts**, tap the other user to start a chat.
5. Send a message — it should appear on the other account's **Chats** tab and open chat in realtime.
