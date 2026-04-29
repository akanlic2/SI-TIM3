# AI Usage Log

> Ovaj dokument evidentira korištenje AI alata u sklopu projekta razvoja sistema za organizaciju kontejnera.  
> Svrha logiranja nije kontrola, već transparentnost i procjena zrelosti u korištenju alata.

---

## Unos #1

| Polje | Detalji |
|---|---|
| **Datum** | 24.04.2026. |
| **Sprint broj** | Sprint 5 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Ajdin Kanlić |

### Svrha korištenja
Konsultacija oko planiranja entiteta za prvi sprint — odluka šta modelirati odmah, a šta odložiti.

### Kratak opis zadatka ili upita

> *"Hej, hajde ovako — posto je ovo tek prvi inkrement, a u ovom sprintu zelimo samo da ispunimo osnovne stvari oko registracije i login-a, mozda bi najbolje bilo da sada samo odradimo User entitet da bude kvalitetan, a kasnije cemo ostale kada budu zatrebali. Posto smo u timu od 8 ljudi smatram da ce to biti puno bolja kontrola i znat ce se tacno sta kada u kojem trenutku treba."*

### Šta je AI predložio ili generisao
- Potvrdio pristup — fokus na `User` entitetu u prvom sprintu je u skladu sa incremental delivery principima
- Predložio strukturu `User` entiteta sa atributima: `id`, `username`, `email`, `passwordHash`, `role`, `createdAt`, `updatedAt`
- Preporučio da se odmah razmisli o `role` polju (enum: `ADMIN`, `USER`) kako bi se izbjegao refactoring kada autorizacija bude potrebna
- Napomenuo da `passwordHash` treba čuvati hashovan (npr. bcrypt), nikad plain text
- Predložio validacijska ograničenja na `email` i `username` (jedinstvenost, format)

### Šta je tim prihvatio
- Odluka da se u ovom sprintu radi isključivo `User` entitet
- Osnovna struktura entiteta sa predloženim atributima
- Uključivanje `role` polja već u ovoj fazi radi lakše nadogradnje

### Šta je tim izmijenio
- Nazivi polja usklađeni sa konvencijom projekta

### Šta je tim odbacio
- Naprednije prijedloge (`lastLoginAt`, `profilePicture`) odloženi za kasniji sprint kao neeesencijalni za registraciju/login

### Rizici, problemi ili greške koje su uočene
- Bez posebnih rizika — AI je korišten kao sounding board za potvrdu već formiranog razmišljanja, a konačna odluka je donijeta od strane tima

---

## Unos #2

| Polje | Detalji |
|---|---|
| **Datum** | 25.04.2026. |
| **Sprint broj** | Sprint 5 |
| **Alat** | GitHub Copilot |
| **Ko je koristio alat** | Ajdin Kanlić |

### Svrha korištenja
Podrška pri pisanju `docker-compose.yml` konfiguracije za spajanje frontend i backend servisa.

### Kratak opis zadatka ili upita
Tražena je pomoć u kreiranju `docker-compose` konfiguracije koja objedinjuje frontend i backend servis, sa ispravnim mrežnim vezama, portovima i varijablama okruženja.

### Šta je AI predložio ili generisao
- Kompletan `docker-compose.yml` sa definisanim servisima `frontend`, `backend` i `db`
- Konfiguraciju zajedničke Docker mreže između servisa
- Primjer `.env` fajla sa varijablama okruženja
- Prijedlog `Dockerfile` za frontend i backend servis

### Šta je tim prihvatio
- Osnovnu strukturu `docker-compose.yml` sa servisima i mrežom
- Definiciju portova i environment varijabli
- Strukturu `Dockerfile` za oba servisa

### Šta je tim izmijenio
- Nazivi servisa i image-a usklađeni sa internim konvencijama projekta
- Prilagođeni volumeni i putanje prema stvarnoj strukturi projekta
- Zamijenjeni generički `latest` tagovi sa specifičnim verzijama radi reproducibilnosti
- Dodane environment varijable koje AI nije mogao znati (interne URL adrese, portovi)

### Šta je tim odbacio
- Health check konfiguracija odložena za kasniji sprint
- Predložena produkcijska konfiguracija (fokus je bio na razvojnom okruženju)

### Rizici, problemi ili greške koje su uočene
- Generisana konfiguracija koristila je `latest` image verzije — potencijalni rizik za reproducibilnost builda, ručno ispravljeno
- Sve konfiguracije testirane lokalno prije commita

---

## Unos #3
 
| Polje | Detalji |
|---|---|
| **Datum** | 28.04.2026. |
| **Sprint broj** | Sprint 5 |
| **Alat** | GitHub Copilot |
| **Ko je koristio alat** | Ajdin Kanlić |
 
### Svrha korištenja
Generisanje unit i integracionih testova za frontend i backend dio aplikacije.
 
### Kratak opis zadatka ili upita
Traženo je od Copilota da na osnovu postojećeg koda napiše testove za frontend komponente (registracija i login forme) i backend logiku (autentifikacija, validacija korisničkih podataka, endpoint-i).
 
### Šta je AI predložio ili generisao
- Unit testove za frontend komponente (`LoginForm`, `RegisterForm`) koristeći Jest i React Testing Library — provjera renderovanja, validacije inputa i ponašanja pri submitu
- Backend unit testove za `UserService` — provjera registracije, login logike i hash-ovanja lozinke
- Integracione testove za REST endpoint-e (`POST /auth/register`, `POST /auth/login`) sa mock bazom podataka
- Test case-ove za granične situacije: dupli email, pogrešna lozinka, nepostojeći korisnik, prazna polja
### Šta je tim prihvatio
- Većinu generisanih unit testova za frontend komponente uz manje izmjene
- Strukturu i organizaciju test fajlova (naming convention, grupiranje po `describe` blokovima)
- Test case-ove za osnovne happy path i error scenarije
### Šta je tim izmijenio
- Prilagođeni mock podaci da odgovaraju stvarnoj strukturi `User` entiteta u projektu
- Ispravljeni import putevi koji nisu odgovarali strukturi projekta
- Neki assertions preciznije definisani prema stvarnom ponašanju aplikacije
- Dodani test case-ovi koje Copilot nije pokrio (npr. token expiry scenariji)
### Šta je tim odbacio
- Dio generisanih integracionih testova koji su bili previše kompleksni za trenutnu fazu i pretpostavljali funkcionalnosti koje još nisu implementirane
- Određeni test case-ovi koji su duplicirali logiku bez dodane vrijednosti
### Rizici, problemi ili greške koje su uočene
- Copilot je na nekoliko mjesta generisao testove koji su prolazili, ali nisu testirali pravu stvar (lažno pozitivni rezultati) — zahtijevalo je pažljiv code review
- Generisani mock-ovi nisu uvijek reflektirali stvarno ponašanje baze podataka, što je moglo dati lažan osjećaj sigurnosti
- Sve generisane testove pregledao i validirao developer prije nego su uključeni u codebase
---

---

## Unos #4

| Polje | Detalji |
|---|---|
| **Datum** | 25.04.2026. |
| **Sprint broj** | Sprint 5 |
| **Alat** | Gemini |
| **Ko je koristio alat** | Hamza Kovač |

### Svrha korištenja
Konfiguracija Keycloak IAM sistema, definisanje RBAC (Role-Based Access Control) strukture i priprema klijenta za backend integraciju.

### Kratak opis zadatka ili upita
Pomoć oko kreiranja realma `conference-app`, definisanja uloga (**Organizatori, Učesnici, Predavači, Administratori sistema**) i rješavanje problema sa permisijama kako bi backend mogao vršiti automatsku registraciju korisnika.

### Šta je AI predložio ili generisao
- Arhitekturu uloga koristeći **Realm Roles** umjesto Client Roles radi lakšeg upravljanja.
- Postupak kreiranja **Confidential** klijenta sa aktiviranim **Service Accounts** opcijama.
- Identifikaciju neophodne sistemske uloge `manage-users` (unutar `realm-management` klijenta) koja omogućava backendu da komunicira sa Keycloak Admin API-jem.
- Mapiranje uloga unutar JWT tokena (`realm_access` sekcija).

### Šta je tim prihvatio
- Model sa 4 osnovne uloge definisane na nivou realma.
- Korištenje klijenta `conference-backend` kao posrednika između koda i Keycloaka.
- Sigurnosni pristup sa **Client Secret** ključem za backend autentifikaciju.

### Šta je tim izmijenio
- Zbog specifičnosti nove verzije Keycloak interfejsa, navigacija do "Filter by client" opcije u *Service accounts roles* tabu je prilagođena ručno jer dugme nije bilo odmah vidljivo.
- Uloge su u Keycloaku nazvane opisno (npr. "Organizatori"), dok su u kodu zadržane kao slug-ovi radi lakše validacije.

### Šta je tim odbacio
- Inicijalni prijedlog korištenja **Client Roles** je odbačen kako bi se izbjeglo komplikovano parsiranje `resource_access` polja u tokenu u ranoj fazi razvoja.
- Napredna verifikacija emaila je odložena dok se ne podesi SMTP server.

### Rizici, problemi ili greške koje su uočene
- **UI barijera:** Keycloak interfejs (v19+) otežava pronalaženje sistemskih uloga (poput `manage-users`), što može dovesti do *403 Forbidden* greške na backendu ako se ne podesi ispravno.
- **Sinhronizacija:** Postoji rizik od desinhronizacije lokalne baze i Keycloaka ako middleware ne obradi grešku pri kreiranju korisnika na jednoj od dvije strane.

---

## Unos #5

| Polje | Detalji |
|---|---|
| **Datum** | 25.04.2026. |
| **Sprint broj** | Sprint 5 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Emira Kurtović |

### Svrha korištenja
Konsultacije i pomoć oko deploymenta aplikacije.

### Kratak opis zadatka ili upita

> *"Korišten AI alat za pomoć pri odabiru alata za deploy aplikacije ako i sami deploy"*

### Šta je AI predložio ili generisao
- Za VPS predložio DigitalOcean
- Za domenu koristiti nip.io sa Let's Encrypt
- Dodavanje 2GB swap memorije koja koristi disk prostor kao virtuelni RAM na serveru
  
### Šta je tim prihvatio
- Za VPS predložio DigitalOcean
- Za domenu koristiti nip.io sa Let's Encrypt
- Dodavanje 2GB swap memorije koja koristi disk prostor kao virtuelni RAM na serveru

### Šta je tim izmijenio
- Prebacili smo build sa VPS na GitHub Actions
  
### Šta je tim odbacio
- Plaćenu paket u DigitalOcean (više memorije, ...)

### Rizici, problemi ili greške koje su uočene
- Nedostatak memorije na serveru, već smo na samom početku morali dodavati 2GB swap memorije.

---
  
*Dokument se ažurira tokom trajanja projekta. Svaki novi slučaj korištenja AI dodaje se kao novi unos.*
