# Architecture / Technical Overview — Conference Management System

> **Namjena dokumenta:** Pomoći osobi koja prvi put gleda projekat da brzo razumije tehničku strukturu sistema.

---

## 1. Kratak opis sistema

**Conference Management** je web aplikacija za upravljanje konferencijama. Podržava kreiranje konferencija, sesija, agende, registraciju učesnika, upravljanje salama, opremom, logističkim zadacima, materijalima, notifikacijama i izvještajima.

---

## 2. Tehnološki stack

| Sloj | Tehnologija |
|---|---|
| **Frontend** | React 19 + TypeScript, Vite, Tailwind CSS v4 |
| **Backend** | ASP.NET Core 9 (Web API), C# |
| **Baza podataka** | PostgreSQL 16 |
| **ORM** | Entity Framework Core (Npgsql provider, snake_case konvencija) |
| **Autentifikacija** | JWT (lokalni, HMAC-SHA256) |
| **Kontejnerizacija** | Docker + Docker Compose |
| **API dokumentacija** | OpenAPI + Scalar UI (development) |
| **Testovi (frontend)** | Vitest + Testing Library |

---

## 3. Dijagram arhitekture

```
┌─────────────────────────────────────────────────────────────────┐
│                        DOCKER COMPOSE                           │
│                                                                 │
│  ┌──────────────────┐        ┌──────────────────────────────┐  │
│  │   FRONTEND       │        │        BACKEND               │  │
│  │  React + Vite    │──HTTP──▶  ASP.NET Core 9 Web API      │  │
│  │  port: 3000      │  REST  │  port: 8082 (host)           │  │
│  │  nginx (prod)    │◀───────│  port: 8080 (container)      │  │
│  └──────────────────┘        └──────────┬───────────────────┘  │
│                                         │ EF Core / Npgsql      │
│                              ┌──────────▼───────────────────┐  │
│                              │     PostgreSQL 16             │  │
│                              │  conference_management_db     │  │
│                              │  port: 5432                   │  │
│                              └──────────────────────────────┘  │
│                                                                 │
│  ┌──────────────────┐                                          │
│  │    MIGRATOR      │  (jednokratni container, pokreće EF      │
│  │  (on-failure)    │   migracije, pa se gasi)                 │
│  └──────────────────┘                                          │
└─────────────────────────────────────────────────────────────────┘
```

**Napomena:** Nema vanjskih servisa (cloud, mail, plaćanje API, Keycloak i sl.). Sistem je u potpunosti self-contained.

---

## 4. Frontend

### Lokacija koda
```
Project/Frontend/conference-management/
├── src/
│   ├── app/            # Router, App.tsx, globalni store
│   ├── auth/           # JWT autentifikacija (AuthProvider, authService, httpInterceptor)
│   ├── features/       # Feature moduli po domenama
│   │   ├── agenda/
│   │   ├── conference/
│   │   ├── equipment/
│   │   ├── logistics/
│   │   ├── notification/
│   │   ├── report/
│   │   ├── room/
│   │   ├── session/
│   │   └── user/
│   ├── pages/          # Page komponente (DashboardPage, ConferencesPage, ...)
│   └── shared/         # Zajednički komponenti, hooks, types, utils
```

### Ključne datoteke

| Datoteka | Svrha |
|---|---|
| `src/app/router.tsx` | Ručni SPA router (bez React Router), zaštita ruta |
| `src/auth/authService.ts` | Login, register, logout, JWT parsing, `localStorage` |
| `src/auth/AuthProvider.tsx` | React Context za auth stanje |
| `src/auth/httpInterceptor.ts` | Axios interceptor — dodaje `Authorization: Bearer` header |
| `src/pages/DashboardPage.tsx` | Glavna stranica nakon prijave |

### Stranice (rute)

| Ruta | Komponenta | Pristup |
|---|---|---|
| `/login` | `LoginPage` | Javna |
| `/register` | `RegisterPage` | Javna |
| `/dashboard` | `DashboardPage` | Zaštićena |
| `/conferences` | `ConferencesPage` | Zaštićena |
| `/conferences/:id` | `ConferenceDetailsPage` | Zaštićena |
| `/conferences/:id/sessions` | `SessionsPage` | Zaštićena |
| `/conferences/:id/agenda` | `AgendaPage` | Zaštićena |
| `/conferences/:id/logistics` | `LogisticsPage` | Zaštićena |
| `/conferences/:id/report` | `ConferenceReportPage` | Zaštićena |
| `/sessions/:id` | `SessionDetailsPage` | Zaštićena |
| `/rooms` | `RoomsPage` | Zaštićena |
| `/equipment` | `EquipmentPage` | Zaštićena |

---

## 5. Backend

Backend koristi **Clean Architecture** s jasno odvojenim slojevima:

```
Project/Backend/ConferenceManagement/
├── ConferenceManagement.Api/           # Presentation layer (Controllers, Program.cs)
│   └── Controllers/                    # 16 controllera
├── ConferenceManagement.Application/   # Biznis logika (Services, Interfaces, DTOs)
│   ├── Services/                       # 14 servisa
│   ├── Interfaces/                     # Interfejsi servisa
│   └── DTOs/                           # Data Transfer Objects
├── ConferenceManagement.Domain/        # Domenske klase (Entities, Abstractions)
│   ├── Entities/                       # 13 entiteta
│   └── Abstractions/Repositories/     # Interfejsi repozitorija
├── ConferenceManagement.Dal/           # Data Access Layer (EF Core)
│   ├── Repositories/                   # 12 repozitorija
│   ├── Configurations/                 # EF Fluent API konfiguracije
│   ├── Migrations/                     # EF migracije
│   └── ApplicationDbContext.cs         # DbContext
└── ConferenceManagement.Tests/         # Unit/integracijski testovi
```

### API Controlleri

| Controller | Endpoint prefix | Opis |
|---|---|---|
| `UserController` | `/api/user` | Registracija, login, profil |
| `ConferenceController` | `/api/conference` | CRUD konferencija |
| `SessionController` | `/api/session` | CRUD sesija |
| `ConferenceRegistrationController` | `/api/conferenceregistration` | Prijava na konferencije |
| `SessionRegistrationController` | `/api/sessionregistration` | Prijava na sesije |
| `ConferenceCapacityController` | `/api/conferencecapacity` | Kapacitet konferencija |
| `ConferenceReportController` | `/api/conferencereport` | Izvještaji |
| `AgendaController` | `/api/agenda` | Agenda stavke |
| `RoomsController` | `/api/rooms` | Sale |
| `EquipmentController` | `/api/equipment` | Oprema |
| `LogisticsController` | `/api/logistics` | Logistički zadaci |
| `MaterialsController` | `/api/materials` | Materijali/fajlovi |
| `NotificationsController` | `/api/notifications` | Notifikacije |
| `QuestionsController` | `/api/questions` | Pitanja u sesijama |
| `SpeakersController` | `/api/speakers` | Predavači |
| `DashboardController` | `/api/dashboard` | Dashboard statistike |

### Domenske klase (entiteti)

`User`, `Conference`, `Session`, `Room`, `ConferenceRegistration`, `SessionRegistration`, `Payment`, `Notification`, `Material`, `Equipment`, `LogisticsTask`, `Question`, `AgendaItem`

---

## 6. Baza podataka

- **RDBMS:** PostgreSQL 16 (Alpine)
- **Naziv baze:** `conference_management_db`
- **Shema:** EF Core Code-First, snake_case konvencija imenovanja kolona
- **Migracije:** Automatski se primjenjuju pri pokretanju (dedicated `migrator` servis ili startup hook)
- **Retry logika:** `EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: 10s)`
- **Statički fajlovi:** `uploads/materials/` folder mauntovan kao Docker volume (`conferencemanagement-uploads`)

---

## 7. Autentifikacija i autorizacija

### Autentifikacija — JWT (lokalna)

> Sistem ne koristi externe Identity Providere (Keycloak, Auth0 i sl.). Auth je u potpunosti lokalan.

**Tok:**
1. Korisnik šalje `POST /api/user/login` s `{usernameOrEmail, password}`
2. Backend validira kredencijale iz baze (hash lozinke)
3. Backend generiše JWT token (HMAC-SHA256, 120 min trajanje)
4. Frontend čuva token u `localStorage` (`auth_token`)
5. Svaki zahtjev šalje `Authorization: Bearer <token>` header (axios interceptor)

**JWT Claims:** `NameIdentifier` (userId), `Name` (username), `Email`, `Role`

### Autorizacija — Role-Based (RBAC)

| Policy | Uloga |
|---|---|
| `AdminPolicy` | `admin-sistema` |
| `OrganizerPolicy` | `organizator` |
| `AdminOrOrganizerPolicy` | `admin-sistema`, `organizator` |
| `SpeakerPolicy` | `predavac` |
| `AttendeePolicy` | `ucesnik` |
| `ParticipantPolicy` | Bilo koji autentifikovan korisnik |

---

## 8. Kommunikacija između komponenti

```
Frontend (React)
    │
    │  HTTP/REST (JSON)
    │  Authorization: Bearer <JWT>
    ▼
Backend (ASP.NET Core Web API)
    │
    │  Dependency Injection
    ├──▶ Application Services (biznis logika)
    │        │
    │        │  Repository Pattern
    │        ▼
    │    DAL Repositories
    │        │
    │        │  Entity Framework Core (Npgsql)
    │        ▼
    └──▶ PostgreSQL 16
```

**CORS:** Backend eksplicitno dozvoljava zahtjeve s:
- `http://localhost:3000`
- `http://localhost:5173`
- `http://localhost:5174`
- `http://localhost`

**Statički fajlovi:** Backend servirá fajlove iz `/uploads` direktorija direktno putem `UseStaticFiles` middlewarea.

---

## 9. Docker infrastruktura

**`docker-compose.yml` definira 4 servisa:**

| Servis | Image | Port (host) | Opis |
|---|---|---|---|
| `conferencemanagement.db` | `postgres:16-alpine` | `5432` | Baza podataka |
| `conferencemanagement.migrator` | Custom (API Dockerfile) | — | Pokreće EF migracije, pa se gasi |
| `conferencemanagement.api` | Custom (API Dockerfile) | `8082` | ASP.NET Core Web API |
| `conferencemanagement.frontend` | Custom (nginx) | `3000` | React SPA serviran putem nginx |

**Zavisnosti:**
- `migrator` čeka `db` (healthcheck)
- `api` čeka `db` (healthcheck) i `migrator` (completed_successfully)
- `frontend` čeka `api` (service_started)

**Volumes:**
- `conferencemanagement-db-data` — PostgreSQL data
- `conferencemanagement-uploads` — korisnički uploadovani fajlovi

---

## 10. Najvažnije sigurnosne odluke

| Odluka | Detalji |
|---|---|
| **JWT lokalni auth** | Nema eksternog IdP; token se generiše i validira lokalno (SymmetricSecurityKey, HMAC-SHA256) |
| **Lozinke** | Čuvaju se hashirane u bazi (ne plain text) |
| **Token storage** | JWT se čuva u `localStorage` (alternativa: `httpOnly` cookie bi bila sigurnija, ali nije implementirana) |
| **Token validacija** | `ValidateIssuerSigningKey`, `ValidateIssuer`, `ValidateAudience`, `ValidateLifetime` — sve uključeno |
| **RBAC** | Svaki endpoint ima eksplicitnu `[Authorize(Policy = "...")]` anotaciju |
| **CORS** | Striktno lista dozvoljenih origina (ne wildcard `*`) |
| **Global error handler** | `UseExceptionHandler` middleware mapira izuzetke na HTTP statusove bez curenja stack tracea |
| **Retry logika** | EF Core automatski pokušava ponovo pri prolaznim greškama baze (max 5 puta) |

---

## 11. Lokalno pokretanje

### Backend
```bash
cd Project/Backend/ConferenceManagement
docker-compose up -d
# API dostupan na http://localhost:8082
# Scalar API dokumentacija: http://localhost:8082/scalar
```

### Frontend (development)
```bash
cd Project/Frontend/conference-management
npm install
npm run dev
# Dostupno na http://localhost:5173
```

### Frontend (production via Docker)
```bash
# Pokreće se automatski kao dio docker-compose
# Dostupno na http://localhost:3000
```

---

## 12. Pregled zavisnosti projekta

### Backend (.NET)
- `Microsoft.EntityFrameworkCore` + `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.IdentityModel.Tokens`
- `EFCore.NamingConventions` (snake_case)
- `Scalar.AspNetCore` (API docs)

### Frontend (npm)
- `react` 19, `react-dom`
- `axios` (HTTP klijent + interceptori)
- `react-datepicker`
- `tailwindcss` v4
- `vitest` + `@testing-library/react` (testovi)
- `vite` + `@vitejs/plugin-react` (build tool)
