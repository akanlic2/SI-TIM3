# Technical Setup
### Sistem za organizaciju konferencija
 
---
 
## 1. Tehnički stack
 
### Pregled stack-a
 
| Sloj | Tehnologija |
|------|-------------|
| Frontend | React + TypeScript |
| Backend | ASP.NET Core + C# |
| Baza podataka | PostgreSQL |
| Autentifikacija | JWT + Refresh Token |
| Kontejnerizacija | Docker + Docker Compose |
| Web server | Nginx |
| CI/CD | GitHub Actions |
 
---
 
### 1.1 Frontend — React + TypeScript
 
Odabir: React 18 + TypeScript
 
Obrazloženje:
- TypeScript pruža statičku provjeru tipova koja smanjuje runtime greške i poboljšava developer experience
- React je jedan od najzastupljenijih frontend frameworka — velika zajednica, bogat ekosistem, odlična dokumentacija
- Komponenta-baziran razvoj omogućava visoku ponovnu upotrebu UI elemenata kroz cijelu aplikaciju
- Odlična podrška za form-heavy i table-heavy interfejse kakvi su potrebni za upravljanje konferencijama
- Lako integrisanje s REST API-jem koji pruža ASP.NET Core backend
Ključne React biblioteke:
 
| Biblioteka | Namjena |
|------------|---------|
| `react-router-dom` | Routing između ekrana (konferencije, sesije, korisnici) |
| `axios` | HTTP komunikacija s backendom |
| `react-hook-form` | Upravljanje formama (registracija, kreiranje konferencija) |
| `@mui/material` | UI komponente (tabele, forme, dialozi, navigacija) |
| `recharts` | Grafikoni i vizualizacija podataka |
| `react-query` | Server state management, caching API poziva |
| `dayjs` | Manipulacija datumima i vremenskim zonama |
 
---
 
### 1.2 Backend — ASP.NET Core
 
Odabir: ASP.NET Core 8 + C#
 
Obrazloženje:
- ASP.NET Core pruža optimalan balans između strukturiranog pristupa i programerske slobode
- Nativna podrška za dependency injection, middleware, autentifikaciju i autorizaciju
- Odlične performanse — jedan od najbržih web frameworka prema industry benchmarkovima
- Bogat ekosistem NuGet paketa pokriva sve potrebe: JWT, validacija, ORM, logging
- Odlična integracija s Entity Framework Core ORM-om za rad s bazom podataka
Ključni NuGet paketi:
 
| Paket | Namjena |
|-------|---------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT autentifikacija |
| `Microsoft.EntityFrameworkCore` | ORM za pristup bazi podataka |
| `BCrypt.Net-Next` | Heširanje lozinki |
| `FluentValidation` | Validacija ulaznih podataka |
| `AutoMapper` | Mapiranje između domenskih entiteta i DTO-ova |
| `Serilog` | Strukturirani logging |
| `Swashbuckle (Swagger)` | Automatska API dokumentacija |
| `xUnit + Moq` | Testiranje |
 
---
 
### 1.3 Baza podataka — PostgreSQL
 
Odabir: PostgreSQL 16
 
Obrazloženje:
- ACID transakcije — kritično za integritet podataka pri rezervisanju mjesta i upravljanju opremom
- Odlična podrška za kompleksne agregacijske upite — izvještaji, poređenja po periodu i tipu
- JSON kolone za fleksibilno čuvanje dodatnih metapodataka o sesijama i učesnicima
- Open-source, bez licencnih troškova
- Nativna podrška u Entity Framework Core ORM-u
---
 
### 1.4 Autentifikacija — JWT + Refresh Token
 
Autentifikacija je implementirana kroz kombinaciju kratkoživućih JWT access tokena i dugoživućih refresh tokena.
 
Tok autentifikacije:
- Korisnik se prijavljuje s email/lozinkom — backend verificira i vraća JWT token (15 min) i refresh token (7 dana)
- JWT token se sprema u `httpOnly` cookie — zaštićen od XSS napada
- Svaki API zahtjev nosi JWT token u `Authorization` headeru
- Pri isteku JWT tokena, klijent automatski koristi refresh token za dobijanje novog JWT-a
- Refresh token se poništava pri svakom korišćenju (Refresh Token Rotation) radi sigurnosti
