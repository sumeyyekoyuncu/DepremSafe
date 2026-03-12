# 🌍 DepremSafe

DepremSafe is a backend API built with **ASP.NET Core** that provides earthquake safety features including real-time earthquake data, location-based alerts, push notifications, and an AI-powered chat assistant.

---

## 🚀 Features

- 🔐 **Authentication** — JWT-based auth with Google OAuth2 support
- 🌐 **Real-time Earthquake Data** — Fetches and serves live earthquake information
- 📍 **User Location Tracking** — Stores and manages user locations for proximity-based alerts
- 🔔 **Push Notifications** — FCM (Firebase Cloud Messaging) integration for earthquake alerts
- 🛡️ **Safety Reports** — Users can submit and view safety status reports
- 🤖 **AI Assistant** — Chat-based assistant powered by AI for earthquake guidance
- 📡 **Mesh Network Support** — When internet connectivity is lost, devices form a mesh network to relay location data between users

---

## 🏗️ Architecture

The project follows an **N-Layer Architecture**:

```
DepremSafe/
├── DepremSafe.API          # Controllers, entry point
├── DepremSafe.Core         # Entities, DTOs, Interfaces (no external dependencies)
├── DepremSafe.Service      # Business logic
└── DepremSafe.Data         # Database access, repositories
```

---

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core | Web API framework |
| Entity Framework Core | ORM / Database access |
| JWT | Authentication & authorization |
| Google OAuth2 | Social login |
| Firebase FCM | Push notifications |
| AI Integration | Chat assistant (AiService) |

---

## 📦 Entities

| Entity | Description |
|---|---|
| `User` | Application user |
| `Earthquake` | Earthquake event data |
| `City` | City information for location mapping |
| `UserLocation` | User's stored location |
| `SafetyReport` | User-submitted safety status after an earthquake |

---

## 📡 API Endpoints

| Controller | Description |
|---|---|
| `/api/auth` | Register, login, Google OAuth |
| `/api/earthquakes` | List and query earthquake events |
| `/api/userlocations` | Manage user location data |
| `/api/notification` | Send and manage push notifications |
| `/api/safety` | Submit and retrieve safety reports |
| `/api/ai` | AI chat assistant |
| `/api/user` | User profile management |

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server or PostgreSQL
- Firebase project (for FCM notifications)

### Run Locally

```bash
git clone https://github.com/sumeyyekoyuncu/DepremSafe.git
cd DepremSafe
```

Update `appsettings.json` with your configuration:

```json
{
  "ConnectionStrings": {
    "Default": "your_connection_string"
  },
  "JwtSettings": {
    "SecretKey": "your_secret_key"
  },
  "Firebase": {
    "ServerKey": "your_fcm_key"
  }
}
```

```bash
dotnet restore
dotnet run --project DepremSafe.API
```

---
