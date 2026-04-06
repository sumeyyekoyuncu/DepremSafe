# DepremSafe Backend

![CI](https://github.com/sumeyyekoyuncu/DepremSafe/actions/workflows/ci.yml/badge.svg)

A REST API built with ASP.NET Core that delivers earthquake safety infrastructure — real-time seismic data, proximity-based push alerts, user safety reporting, and an AI-powered guidance assistant. Designed to remain partially functional even when internet connectivity is unavailable through mesh network support.

---

## Features

| Domain | Capabilities |
|---|---|
| **Authentication** | JWT-based auth with Google OAuth2 social login |
| **Earthquake Data** | Fetches and serves live seismic event information |
| **Location Tracking** | Stores and manages user locations for proximity-based alerting |
| **Push Notifications** | Firebase Cloud Messaging (FCM) integration for real-time earthquake alerts |
| **Safety Reports** | Users can submit and query post-earthquake safety status |
| **AI Assistant** | Chat-based guidance assistant powered by AI for earthquake preparedness and response |
| **Mesh Network** | When internet is unavailable, devices form a local mesh network to relay location data between users |

---

## Architecture

DepremSafe follows an **N-Layer Architecture**, separating concerns across four distinct layers:

```
DepremSafe/
├── DepremSafe.API      # HTTP controllers and application entry point
├── DepremSafe.Core     # Entities, DTOs, and interfaces (no external dependencies)
├── DepremSafe.Service  # Business logic and use case orchestration
└── DepremSafe.Data     # Repository implementations and database access
```

---

## Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core | Web API framework |
| Entity Framework Core | ORM and database access |
| SQL Server | Relational data store |
| JWT | Authentication and authorization |
| Google OAuth2 | Social login |
| Firebase FCM | Push notification delivery |
| AI Integration | Chat assistant via `AiService` |
| xUnit | Unit testing |
| GitHub Actions | CI pipeline — build and test on every push |

---

## Domain Model

| Entity | Description |
|---|---|
| `User` | Application user account |
| `Earthquake` | Seismic event record with magnitude, location, and timestamp |
| `City` | City data used for location mapping and regional alerts |
| `UserLocation` | User's stored geographic position |
| `SafetyReport` | User-submitted safety status following an earthquake event |

---

## API Reference

| Controller | Endpoints |
|---|---|
| `/api/auth` | Register, login, Google OAuth2 callback |
| `/api/earthquakes` | List and query seismic events |
| `/api/userlocations` | Create, update, and retrieve user location data |
| `/api/notification` | Send and manage push notifications |
| `/api/safety` | Submit and retrieve safety reports |
| `/api/ai` | AI assistant chat interface |
| `/api/user` | User profile management |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server instance (local or remote)
- Firebase project with FCM enabled

### Local Setup

**1. Clone the repository**

```bash
git clone https://github.com/sumeyyekoyuncu/DepremSafe.git
cd DepremSafe
```

**2. Configure the application**

Update `appsettings.json` with your environment values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=depremsafe;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "your_secret_key",
    "Issuer": "DepremSafeAPI",
    "Audience": "DepremSafeClient"
  },
  "Firebase": {
    "ServerKey": "your_fcm_server_key"
  }
}
```

**3. Restore, migrate, and run**

```bash
dotnet restore
dotnet ef database update --project DepremSafe.Data --startup-project DepremSafe.API
dotnet run --project DepremSafe.API
```

---

## Running Tests

```bash
dotnet test
```

Unit tests cover core business logic including seismic distance calculations and proximity-based city detection — the most critical algorithms in a real-time earthquake alert system.
