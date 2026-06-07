# CD Pipeline Dokumentacija — SI-TIM3: Sistem za organizaciju konferencija
 
## 1. Pregled
 
Continuous Deployment pipeline je implementiran putem **GitHub Actions** i automatski deploya aplikaciju na DigitalOcean VPS server svaki put kada se izmjene merge-aju na `main` granu.
 
Pipeline se sastoji od tri faze:
1. **test** — pokreće backend i frontend testove
2. **build-and-push** — builda backend i frontend Docker imageove i šalje ih na Docker Hub (rezervna kopija)
3. **deploy** — server preuzima najnoviji kod, builda sve kontejnere lokalno i pokreće ih
> **Napomena:** Svi aplikacijski kontejneri (backend, frontend, migrator) se buildaju direktno na serveru jer `VITE_API_URL` mora biti ugrađen u frontend build-time sa produkcijskim URL-om (`https://178.128.144.139.nip.io`). Docker Hub se koristi za čuvanje imageova, ali server ih ne preuzima — `docker compose pull` preuzima samo `nginx` i `postgres` imageove.
 
---
 
## 2. Lokacija skripte
 
Pipeline fajl se nalazi u repozitoriju na putanji:
 
```
.github/workflows/deploy.yml
```
 
Produkcijski Docker Compose fajl se nalazi na:
 
```
Project/Backend/ConferenceManagement/docker-compose.prod.yml
```
 
---
 
## 3. Kako se pokreće
 
Pipeline se pokreće **automatski** pri svakom push/merge-u na `main` granu. Nije potrebno ručno pokretanje.
 
Može se i ručno pokrenuti:
1. Idi na GitHub repozitorij → **Actions** tab
2. Odaberi **"Deploy to VPS"** workflow
3. Klikni **"Run workflow"**
---
 
## 4. Preduvjeti
 
### Na VPS serveru
- Ubuntu 24.04 LTS
- Docker Engine instaliran (`curl -fsSL https://get.docker.com | sh`)
- Repozitorij kloniran u `/root/SI-TIM3`
- SSH ključ dodan u `authorized_keys`
- 2GB swap memorije (server ima 1GB RAM — swap je neophodan)
- SSL certifikat konfigurisan za `178.128.144.139.nip.io` (Let's Encrypt)
- Nginx konfiguracija u `Project/Backend/ConferenceManagement/nginx/nginx.conf`
### Na Docker Hub
Kreiran repozitorij:
- `ekurtovic5/conferencemanagement-api`
- `ekurtovic5/conferencemanagement-frontend`
### Na GitHubu
Sljedeći GitHub Secrets moraju biti postavljeni u **Settings → Secrets and variables → Actions**:
 
| Secret | Opis |
|---|---|
| `SSH_HOST` | IP adresa VPS servera (`178.128.144.139`) |
| `SSH_USER` | SSH korisnik na serveru (`root`) |
| `SSH_PRIVATE_KEY` | Privatni SSH ključ za autentifikaciju |
| `DOCKERHUB_USERNAME` | Docker Hub korisničko ime (`ekurtovic5`) |
| `DOCKERHUB_TOKEN` | Docker Hub access token |
 
---
 
## 5. Sadržaj pipeline-a
 
```yaml
name: Deploy to VPS
 
on:
  push:
    branches:
      - main
 
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
 
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0'
 
      - name: Run Backend tests
        run: dotnet test Project/Backend/ConferenceManagement/ConferenceManagement.slnx
 
      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '22'
 
      - name: Install Frontend dependencies
        run: npm install
        working-directory: ./Project/Frontend/conference-management
 
      - name: Run Frontend tests
        run: npm test
        working-directory: ./Project/Frontend/conference-management
 
  build-and-push:
    runs-on: ubuntu-latest
    needs: test
    steps:
      - name: Checkout code
        uses: actions/checkout@v4
 
      - name: Login to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}
 
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3
 
      - name: Build and push API image
        uses: docker/build-push-action@v6
        with:
          context: ./Project/Backend/ConferenceManagement
          file: ./Project/Backend/ConferenceManagement/ConferenceManagement.Api/Dockerfile
          push: true
          provenance: false
          tags: ekurtovic5/conferencemanagement-api:latest
 
      - name: Build and push Frontend image
        uses: docker/build-push-action@v6
        with:
          context: ./Project/Frontend/conference-management
          file: ./Project/Frontend/conference-management/Dockerfile
          push: true
          provenance: false
          tags: ekurtovic5/conferencemanagement-frontend:latest
 
  deploy:
    runs-on: ubuntu-latest
    needs: build-and-push
    steps:
      - name: Deploy via SSH
        uses: appleboy/ssh-action@v1.0.0
        with:
          host: ${{ secrets.SSH_HOST }}
          username: ${{ secrets.SSH_USER }}
          key: ${{ secrets.SSH_PRIVATE_KEY }}
          command_timeout: 10m
          script: |
            cd /root/SI-TIM3/Project/Backend/ConferenceManagement
            git pull origin main
            docker compose -f docker-compose.prod.yml pull
            docker compose -f docker-compose.prod.yml up -d --remove-orphans
            docker image prune -af
```
 
---
 
## 6. Šta se tačno deploya
 
### Job 1: `test`
- Preuzima kod iz repozitorija
- Instalira .NET 10 SDK i pokreće **backend testove**
- Instalira Node.js 22 i pokreće **frontend testove**
- **Ako testovi padnu — build i deploy se ne izvršavaju**
### Job 2: `build-and-push` (pokreće se samo ako `test` prođe)
- Prijavljuje se na Docker Hub
- Builda **backend** i **frontend** Docker imageove i šalje ih na Docker Hub
- Ovi imageovi služe kao rezervna kopija i provjera da Dockerfileovi nemaju grešaka
- **Napomena:** Server ne preuzima ove imageove — sve se builda lokalno na serveru pri deploymentu
### Job 3: `deploy` (pokreće se samo ako `build-and-push` prođe)
- Spaja se SSH-om na VPS server
- Preuzima najnoviji kod (`git pull`)
- Preuzima najnovije `nginx` i `postgres` imageove (`docker compose pull`)
- Builda i pokreće sve kontejnere lokalno (`docker compose -f docker-compose.prod.yml up -d --remove-orphans`):
  - `conferencemanagement.db` — PostgreSQL baza
  - `conferencemanagement.migrator` — primjenjuje migracije automatski, zatim se gasi
  - `conferencemanagement.api` — ASP.NET Core API (port 8082), builda se lokalno
  - `conferencemanagement.frontend` — React frontend (port 3000), builda se lokalno sa `VITE_API_URL=https://178.128.144.139.nip.io`
  - `nginx` — reverse proxy sa SSL (port 80/443)
- Briše stare Docker images (`docker image prune -af`)
---
 
## 7. Koje varijable i secrets se koriste
 
| Naziv | Tip | Gdje se koristi |
|---|---|---|
| `SSH_HOST` | GitHub Secret | IP adresa servera za SSH konekciju |
| `SSH_USER` | GitHub Secret | Korisničko ime za SSH |
| `SSH_PRIVATE_KEY` | GitHub Secret | Autentifikacija putem SSH ključa |
| `DOCKERHUB_USERNAME` | GitHub Secret | Prijava na Docker Hub |
| `DOCKERHUB_TOKEN` | GitHub Secret | Autentifikacija na Docker Hub |
| `VITE_API_URL` | Docker build ARG u `docker-compose.prod.yml` | Ugrađuje se u frontend build u build-time na serveru |
| `Jwt__Key` | `docker-compose.prod.yml` environment | JWT potpisni ključ za backend |
 
---
 
## 8. Povezanost servisa
 
```
GitHub (main branch)
        ↓ push/merge
GitHub Actions
    ├── Job 1: test (backend + frontend testovi)
    ├── Job 2: build-and-push → Docker Hub (rezervna kopija)
    │         ekurtovic5/conferencemanagement-api:latest
    │         ekurtovic5/conferencemanagement-frontend:latest
    └── Job 3: deploy via SSH
                    ↓
        VPS Server (178.128.144.139)
        ├── git pull (preuzima novi kod)
        ├── docker compose pull (preuzima nginx i postgres imageove)
        └── docker compose up --build (builda sve aplikacije lokalno, pokreće sve)
                    ↓
┌──────────────────────────────────────────────┐
│  nginx (port 80/443)                         │
│  SSL: https://178.128.144.139.nip.io         │
└───────────┬──────────────────────────────────┘
            │
    ┌───────┴───────┐
    ▼               ▼
┌─────────┐   ┌──────────────────────┐
│Frontend │   │  API (port 8082)     │
│port 3000│   │  ASP.NET Core 10     │
└─────────┘   └──────────┬───────────┘
                         │
              ┌──────────▼───────────┐
              │  PostgreSQL 16       │
              │  port 5432           │
              │  (interna mreža)     │
              └──────────────────────┘
```
 
---
 
## 9. Kako provjeriti rezultat deploymenta
 
### Putem GitHub Actions
1. Idi na GitHub repozitorij → **Actions** tab
2. Klikni na zadnji workflow run
3. Zelena kvačica = uspješan deployment, crveni X = greška
### Putem browsera (osnovna provjera dostupnosti)
 
| Šta | URL | Očekivani rezultat |
|---|---|---|
| Frontend | `https://178.128.144.139.nip.io` | Web aplikacija se učitava |
| API dokumentacija | `https://178.128.144.139.nip.io/scalar/v1` | Scalar API dokumentacija |
 
### Putem SSH-a
```bash
ssh root@178.128.144.139
docker ps
```
 
Trebaju biti aktivna 4 kontejnera:
- `conferencemanagement.api`
- `conferencemanagement.db`
- `conferencemanagement.frontend`
- `nginx`
---
 
## 10. Ručni koraci pri inicijalnom setupu (jednom)
 
Sljedeći koraci su obavljeni jednom i nisu potrebni pri svakom deploymentu:
 
1. Kreiranje VPS servera na DigitalOcean (Ubuntu 24.04, 1GB RAM)
2. Dodavanje 2GB swap memorije na serveru
3. Instalacija Docker Enginea na serveru
4. Kloniranje repozitorija u `/root/SI-TIM3`
5. Generisanje SSH ključa i dodavanje u `authorized_keys`
6. Postavljanje GitHub Secrets
7. Kreiranje Docker Hub repozitorija i access tokena
8. Konfiguracija SSL certifikata (Let's Encrypt za `178.128.144.139.nip.io`)
9. Kreiranje Nginx konfiguracije (`nginx/nginx.conf`)
---
 
## 11. Poznata ograničenja pipeline-a
 
- Sve aplikacije se buildaju lokalno na serveru pri svakom deploymentu — deployment traje duže nego da se preuzimaju gotovi imageovi
- `ASPNETCORE_ENVIRONMENT` je `Development` na produkcijskom serveru
- JWT signing key i DB kredencijali su vidljivi u `docker-compose.prod.yml`
- Server ima samo 1GB RAM — swap memorija je neophodna za rad
