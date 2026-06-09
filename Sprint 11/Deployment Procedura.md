# Deployment Procedura — SI-TIM3: Sistem za organizaciju konferencija
 
## 1. Naziv aplikacije i opis arhitekture
 
**Naziv:** SI-TIM3 — Sistem za organizaciju konferencija
 
**Opis:** Web aplikacija za podršku organizaciji konferencija — prijava učesnika, upravljanje sesijama, rasporedom, dvoranama, kotizacijama, obavijestima i izvještajima za organizatore.
 
**Arhitektura:** Klasična četveroslojna (layered) arhitektura:
- **Presentation layer** — ASP.NET Core Controllers (HTTP, validacija, JWT provjera)
- **Application layer** — Services (orkestracija korisničkih akcija)
- **Domain layer** — Poslovni entiteti i pravila
- **Data access layer** — Repositories, EF Core, migracije, seeders
Sistem se pokreće kao skup Docker kontejnera orkestriranih putem Docker Compose.
 
---
 
## 2. Tehnologije
 
| Sloj | Tehnologija |
|---|---|
| Frontend | React 18 + TypeScript + Vite |
| Backend | ASP.NET Core 10 + C# |
| Baza podataka | PostgreSQL 16 |
| Autentifikacija | JWT + Refresh Token |
| Kontejnerizacija | Docker + Docker Compose |
| Web server / Reverse proxy | Nginx |
| SSL | Let's Encrypt (nip.io domena) |
| Image registry | Docker Hub (nije u aktivnoj upotrebi — sve se builda lokalno na serveru) |
| CI/CD | GitHub Actions |
 
---
 
## 3. Potrebni alati i verzije
 
| Alat | Verzija | Namjena |
|---|---|---|
| .NET SDK | 10 | Backend razvoj i pokretanje |
| Node.js | 22 | Frontend razvoj |
| Docker Desktop | Latest | Lokalno pokretanje kontejnera |
| Git | Latest | Verzioniranje koda |
| Visual Studio / VS Code | Latest | IDE |
 
---
 
## 4. Environment varijable
 
### Backend
 
| Varijabla | Vrijednost | Opis |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` | Okruženje |
| `ASPNETCORE_URLS` | `http://+:8080` | Port na kojem API sluša |
| `ConnectionStrings__DefaultConnection` | `Host=conferencemanagement.db;Port=5432;Database=conference_management_db;Username=si_tim3;Password=si_tim3_db_pass` | Konekcija na bazu |
| `Jwt__Key` | `ConferenceManagement_LocalAuth_SigningKey_2026_StrongKey` | JWT potpisni ključ |
| `Jwt__Issuer` | `ConferenceManagement.Api` | JWT issuer |
| `Jwt__Audience` | `ConferenceManagement.Client` | JWT audience |
| `RUN_MIGRATIONS_ONLY` | `true` (samo migrator kontejner) | Pokreće samo migracije |
 
### Baza podataka
 
| Varijabla | Vrijednost |
|---|---|
| `POSTGRES_DB` | `conference_management_db` |
| `POSTGRES_USER` | `si_tim3` |
| `POSTGRES_PASSWORD` | `si_tim3_db_pass` |
 
### Frontend (Docker build argument)
 
| Argument | Lokalno | Produkcija |
|---|---|---|
| `VITE_API_URL` | `http://localhost:8082` | `https://178.128.144.139.nip.io` |
 
> **Napomena:** `VITE_API_URL` se prosljeđuje kao Docker build argument (`ARG`) i ugrađuje se u JavaScript kod u build-time putem Vite. Zbog toga postoje dva odvojena docker-compose fajla — `docker-compose.yml` za lokalno i `docker-compose.prod.yml` za produkciju, svaki sa odgovarajućim `VITE_API_URL`.
 
### GitHub Secrets (potrebni za CI/CD)
 
| Secret | Opis |
|---|---|
| `SSH_HOST` | IP adresa VPS servera (`178.128.144.139`) |
| `SSH_USER` | SSH korisnik (`root`) |
| `SSH_PRIVATE_KEY` | Privatni SSH ključ za autentifikaciju |
| `DOCKERHUB_USERNAME` | Docker Hub korisničko ime |
| `DOCKERHUB_TOKEN` | Docker Hub access token |
 
---
 
## 5. Lokalno pokretanje backenda
 
### Preduvjeti
- Docker Desktop instaliran i pokrenut
- Git
### Koraci
 
```bash
# 1. Kloniraj repozitorij
git clone https://github.com/akanlic2/SI-TIM3.git
cd SI-TIM3
 
# 2. Idi u backend folder
cd Project/Backend/ConferenceManagement
 
# 3. Pokreni sve kontejnere (baza + migrator + API)
docker-compose up -d --build
```
 
API je dostupan na: `http://localhost:8082`
 
### Zaustavljanje
 
```bash
docker-compose down
```
 
---
 
## 6. Lokalno pokretanje frontenda
 
### Opcija A — putem Docker Compose (preporučeno)
 
Frontend se pokreće automatski zajedno sa backendom:
 
```bash
cd Project/Backend/ConferenceManagement
docker-compose up -d --build
```
 
Frontend je dostupan na: `http://localhost:3000`
 
### Opcija B — samostalno (development mode)
 
```bash
# Idi u frontend folder
cd Project/Frontend/conference-management
 
# Instaliraj zavisnosti
npm install
 
# Pokreni development server
npm run dev
```
 
Frontend je dostupan na: `http://localhost:5173`
 
> **Napomena:** U development modu, frontend proksira `/api` zahtjeve na `http://localhost:8082` (konfigurisano u `vite.config.ts`).
 
---
 
## 7. Pokretanje baze podataka
 
Baza se pokreće automatski kao dio Docker Compose stacka:
 
```bash
cd Project/Backend/ConferenceManagement
docker-compose up -d conferencemanagement.db
```
 
Baza je dostupna na: `localhost:5432`
 
| Parametar | Vrijednost |
|---|---|
| Database | `conference_management_db` |
| Username | `si_tim3` |
| Password | `si_tim3_db_pass` |
| Port | `5432` |
 
---
 
## 8. Migracije i seed podaci
 
### Migracije
 
Migracije se primjenjuju **automatski** pri pokretanju Docker Compose stacka putem `conferencemanagement.migrator` kontejnera. Nije potrebno ručno pokretati `Update-Database`.
 
Za dodavanje nove migracije (samo `Add-Migration`, **bez** `Update-Database`):
 
```bash
cd Project/Backend/ConferenceManagement
dotnet ef migrations add NazivMigracije --project ConferenceManagement.Dal --startup-project ConferenceManagement.Api
```
 
### Seed podaci
 
Seed podaci se primjenjuju automatski pri pokretanju aplikacije. Uključuju:
 
**Korisnici:**
 
| Username | Password | Email | Rola |
|---|---|---|---|
| Administrator | Admin123 | administrator@gmail.com | admin-sistema |
| Organizator | Org123 | organizator@gmail.com | organizator |
| Predavac | Pred123 | predavac@gmail.com | predavac |
| Ucesnik | Uces123 | ucesnik@gmail.com | ucesnik |
 
**Sale:**
 
| Naziv | Kapacitet | Lokacija |
|---|---|---|
| Amfiteatar 1 | 150 | ETF Sarajevo |
| Sala 203 (Lab) | 30 | ETF Sarajevo |
| Konferencijska Sala A | 50 | Hotel Hills |
 
---
 
## 9. Pokretanje testova
 
### Backend testovi
 
```bash
cd Project/Backend/ConferenceManagement
dotnet test ConferenceManagement.slnx
```
 
### Frontend testovi
 
```bash
cd Project/Frontend/conference-management
npm install
npm test
```
 
---
 
## 10. Produkcijski deployment
 
Produkcijski deployment se vrši automatski putem GitHub Actions CI/CD pipeline-a svaki put kada se izmjene merge-aju na `main` granu.
 
### Preduvjeti za produkcijski deployment
 
1. VPS server sa Ubuntu 24.04 (DigitalOcean, 1GB RAM + 2GB swap)
2. Docker Engine instaliran na serveru
3. Repozitorij kloniran na server u `/root/SI-TIM3`
4. SSL certifikat konfigurisan za `178.128.144.139.nip.io` (Let's Encrypt)
5. Nginx konfiguracija postavljena u `Project/Backend/ConferenceManagement/nginx/nginx.conf`
6. GitHub Secrets postavljeni (SSH_HOST, SSH_USER, SSH_PRIVATE_KEY, DOCKERHUB_USERNAME, DOCKERHUB_TOKEN)
### Ručni produkcijski deployment (po potrebi)
 
```bash
# Spoji se na server
ssh root@178.128.144.139
 
# Idi u folder projekta
cd /root/SI-TIM3/Project/Backend/ConferenceManagement
 
# Preuzmi najnoviji kod
git pull origin main
 
# Preuzmi najnovije imageove za nginx i postgres
docker compose -f docker-compose.prod.yml pull
 
# Pokreni kontejnere (sve tri aplikacije se buildaju lokalno na serveru)
docker compose -f docker-compose.prod.yml up -d --build --remove-orphans
 
# Očisti stare images
docker image prune -af
```
 
### Zašto dva docker-compose fajla
 
Vite ugrađuje `VITE_API_URL` direktno u JavaScript kod u build-time kao Docker build argument, što znači da je potreban odvojen build za lokalno i produkcijsko okruženje:
 
| | `docker-compose.yml` | `docker-compose.prod.yml` |
|---|---|---|
| Namjena | Lokalni development | Produkcijski server |
| `VITE_API_URL` | `http://localhost:8082` | `https://178.128.144.139.nip.io` |
| Nginx | Ne | Da |
| SSL | Ne | Da (Let's Encrypt) |
 
---
 
## 11. Linkovi na deployment
 
| Servis | URL |
|---|---|
| Frontend (web aplikacija) | `https://178.128.144.139.nip.io` |
| API dokumentacija | `https://178.128.144.139.nip.io/scalar/v1` |
 
---
 
## 12. Poznata ograničenja deploymenta
 
- VPS server ima 1GB RAM — dodano 2GB swap memorije kao virtuelni RAM
- `ASPNETCORE_ENVIRONMENT` je `Development` i na produkcijskom serveru
- JWT signing key i DB kredencijali su vidljivi u `docker-compose.prod.yml`
- Lozinke u seed podacima nisu hashirane
- Frontend, backend i migrator se buildaju direktno na serveru pri svakom deploymentu jer `VITE_API_URL` mora biti ugrađen u build-time sa produkcijskim URL-om
---
 
## 13. Najčešći problemi i rješenja
 
| Problem | Uzrok | Rješenje |
|---|---|---|
| `docker-compose up` ne radi | Docker Desktop nije pokrenut | Pokrenuti Docker Desktop i pričekati da se engine pokrene |
| Baza se ne pokreće | Port 5432 je zauzet | Zaustaviti lokalni PostgreSQL servis |
| API ne odgovara | Kontejner se nije pokrenuo | `docker logs conferencemanagement.api` |
| Migracije padaju | Baza nije zdrava | Pričekati healthcheck, ponoviti `docker-compose up` |
| Frontend ne može dosegnuti API lokalno | Pogrešan `VITE_API_URL` | Provjeriti da `docker-compose.yml` ima `VITE_API_URL=http://localhost:8082` |
| Frontend na serveru ne može dosegnuti API | Pogrešan `VITE_API_URL` u prod buildu | Provjeriti `docker-compose.prod.yml` — `VITE_API_URL` mora biti `https://178.128.144.139.nip.io` |
| CORS greška u browseru | Frontend URL nije dozvoljen u backendu | Provjeriti CORS konfiguraciju u `Program.cs` |
| SSH konekcija odbijena | SSH ključ nije u `authorized_keys` | `cat ~/.ssh/id_ed25519.pub >> ~/.ssh/authorized_keys` |
| SSL certifikat ne radi | Certifikat je istekao | Obnoviti Let's Encrypt certifikat na serveru |
| Deploy pada zbog nedostatka memorije | 1GB RAM nije dovoljno | Provjeriti da je 2GB swap aktivan: `swapon --show` |
