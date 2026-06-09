# IZVJEŠTAJ O RADU NA PROJEKTU

## 1. Svrha projekta
Projekat je razvijen kao web aplikacija za organizaciju konferencija u okviru predmeta "Softver inženjering" na Elektrotehničkom fakultetu u Sarajevu. Cilj sistema je podrška cjelokupnog procesa organizacije - od prijave učesnika i rasporeda sesija, do upravljanja dvoranama, kotizacijama i slanjem obavijesti. Sistem je namijenjen organizatorima, predavačima i učesnicima, s definisanim ulogama i kontrolom pristupa koja svakome omogućava odgovarajući nivo interakcije.

## 2. Problem koji sistem rješava
Organizacija konferencija tradicionalno se oslanja na ručne procese - praćenje prijava u tabelama, koordinaciju putem emaila i ručno kreiranje rasporeda. Takav pristup je vremenski zahtjevan, podložan greškama i teško skalabilan. Sistem koji je razvijen rješava ove izazove centralizovanom platformom koja automatizira upravljanje konferencijama, sesijama, učesnicima, resursima i izvještajima.

## 3. Glavne korisničke uloge

### Admin sistema
Sistemski administrator s potpunom kontrolom nad platformom. Ima pristup svim funkcijama putem `AdminPolicy`, uključujući upravljanje konferencijama, sesijama, izvještajima, logistikom i opremom, kao i sve organizatorske akcije.

- Upravlja korisničkim računima i dodjeljuje uloge
- Kreira, uređuje i briše konferencije i sesije
- Pristupa svim izvještajima i logističkim podacima
- Izvršava sve akcije dostupne organizatoru

### Organizator
Odgovoran za kreiranje i vođenje konferencija. U kodu je podržan kroz `OrganizerPolicy`.

- Kreira, uređuje i briše konferencije i sesije
- Dodjeljuje predavače sesijama i dvorane sesijama
- Upravlja logistikom i tehničkom opremom konferencije
- Pregleda kapacitet konferencija i generiše izvještaje

### Predavač
Osoba koja drži sesiju ili prezentaciju. U kodu je podržan kroz `SpeakerPolicy` s ograničenim pristupom u odnosu na organizatora.

- Pristupa sesijama kojima je dodijeljen
- Pregleda opremu dodijeljenu njegovim sesijama

### Učesnik
Registrovana osoba koja prisustvuje konferenciji. U kodu je podržan kroz `AttendeePolicy`.

- Pregledava dostupne konferencije i sesije
- Prijavljuje se na konferencije i sesije
- Pristupa agendi konferencije


## 4. Glavne implementirane funkcionalnosti

### Backend

**Autentikacija i autorizacija**
- JWT autentikacija i autorizacija
- Lokalni JWT auth sistem (bez vanjskog identity providera)
- Endpointi za registraciju, prijavu, odjavu i dohvat trenutnog korisnika

**Upravljanje konferencijama i sesijama**
- CRUD operacije za konferencije
- CRUD operacije za sesije
- Registracija i otkazivanje učešća na konferencijama i sesijama
- Dodjela i uklanjanje predavača za sesije
- Dodjela sala sesijama
- Upravljanje agendom konferencije

**Resursi i logistika**
- Upravljanje logističkim zadacima i opremom
- Upload i pregled materijala (statičko serviranje fajlova)

**Izvještaji**
- Izvještaji o kapacitetu konferencije
- Preuzimanje izvještaja

**Infrastruktura**
- Automatska primjena EF Core migracija pri pokretanju aplikacije
- CORS konfiguracija za frontend

---

### Frontend

**Autentikacija**
- Auth provider s podrškom za JWT token (localStorage)
- Stranice za prijavu i registraciju

**Korisnički interfejs**
- Stranica s listom konferencija i forma za kreiranje/uređivanje
- Stranica s listom sesija i forma za sesiju
- Zaštićene rute za: dashboard, konferencije, sesije, agendu, logistiku i izvještaje
- Komunikacija s backend API-jem putem Axios/Fetch poziva


## 5. Pregled rada kroz sprintove
| Sprint|Ključne isporuke|
|----------|-----------|
|Sprint 1|Definicija problema, Product Vision, Stakeholder Map, inicijalni Product Backlog|
|Sprint 2|User storiji s acceptance kriterijima, prioritizacija backloga, NFR zahtjevi|
|Sprint 3|Domain Model, Use Case Model, Architecture Overview, Test Strategy, ERD, Risk Register|
|Sprint 4|Definition of Done, Initial Release Plan, Technical SetUp, Branching Strategy|
|Sprint 5| Isporuka prvog funkcionalnog inkrementa sistema (INC-01) - implementacija autentifikacije i upravljanja korisnicima kroz integraciju s Keycloak identity providerom, zajedno s uspostavljanjem procesnih artefakata Decision Log i AI Usage Log|
|Sprint 6| Isporuka drugog funkcionalnog inkrementa sistema (INC-02) - implementacija upravljanja konferencijama i korisničkim profilima, uključujući role-based pristup i kompletan CRUD za konferencije|
|Sprint 7| Isporuka trećeg funkcionalnog inkrementa sistema (INC-03) - kompletan domain model s migracijama, upravljanje sesijama konferencije (CRUD i dodjela predavača) te prijave i odjave učesnika na konferencije i sesije|
|Sprint 8|  Isporuka četvrtog funkcionalnog inkrementa sistema (INC-04) - upravljanje dvoranama s stvarnom dodjelom dvorane sesijama, pregled rasporeda konferencije putem agende, te uvid u popunjenost kapaciteta i listu učesnika|
|Sprint 9|Isporuka petog funkcionalnog inkrementa sistema (INC-05) - potpuno aktiviranje role predavača kroz vlastiti dashboard, upload materijala, Q&A panel po sesiji i in-app notifikacijski sistem|
|Sprint 10|Isporuka šestog funkcionalnog inkrementa sistema (INC-06) - upravljanje logističkim aktivnostima konferencije, upravljanje tehničkom opremom s dodjelom sesijama, te izvještaji za organizatore sa ključnim statistikama|

## 6. Status završenosti
### Završeno

- Osnovna backend arhitektura i API funkcionalnosti za konferencije, sesije, registracije, agendu, logistiku i izvještaje
- Autentikacija i autorizacija s ulogama
- Frontend osnovne stranice za autentikaciju, konferencije i sesije
- Automatsko pokretanje migracija i inicijalno seedanje korisnika

### Djelimično završeno

- Frontend koristi ručno implementiran router umjesto standardne biblioteke za rutiranje
- Prisutan debug kod u pojedinim dijelovima backenda (npr. Console.WriteLine u auth događajima)
### Nije završeno

- Sigurnosna obrada lozinki - seed korisnici imaju plain text lozinke 
- Produkcijsko poliranje korisničkog iskustva
- Modul za notifikacije - nije potvrđena potpuna implementacija na UI nivou
- Modul za kotizacije - API postoji, ali UI pokrivenost nije potpuna

## 7. Glavne tehničke odluke

### Tehnološki stack

**Backend**
- .NET 10, ASP.NET Core Web API
- Entity Framework Core s PostgreSQL (`Npgsql`)

**Frontend**
- React + TypeScript + Vite

**Autentikacija**
- JWT bearer tokeni s lokalnim generisanjem - odabrano umjesto vanjskog identity providera radi jednostavnosti u akademskom kontekstu

### Arhitektura projekta
Projekat je podijeljen u sljedeće slojeve prema principima čiste arhitekture:

- `Api` - kontroleri i HTTP sloj
- `Application` - poslovna logika i servisi
- `Dal` - pristup bazi podataka (Data Access Layer)
- `Domain` - domenski modeli i entiteti
- `Tests` - testovi

### Infrastrukturne odluke

- **CORS** - konfigurisan za lokalni razvoj (Vite/React portovi)
- **Automatske migracije** - EF Core migracije se primjenjuju automatski pri pokretanju aplikacije; dostupna je i opcija pokretanja kroz zasebni migrator container
- **Statičko serviranje fajlova** - uploadovani materijali se serviraju direktno iz mape `uploads` na serveru

## 8. Najveći problemi tokom razvoja i način rješavanja

### Problem 1: Zamjena vanjske autorizacije lokalnim JWT auth modelom
Inicijalni plan je podrazumijevao korištenje Keycloaka kao vanjskog identity providera.
Tokom razvoja odlučeno je da se pređe na lokalni JWT auth model radi jednostavnosti
i lakšeg postavljanja u akademskom okruženju.

**Rješenje:** U `Program.cs` je konfigurisan lokalni JWT bearer auth, a proces
prijave je implementiran direktno u `UserController` s lokalnom validacijom
korisničkih kredencijala.

---

### Problem 2: Automatsko pokretanje migracija i seedanje podataka
Bilo je potrebno osigurati da se baza podataka ispravno inicijalizira pri svakom
pokretanju aplikacije, bez ručne intervencije.

**Rješenje:** Pri startanju API-ja poziva se `WaitForDatabaseAndApplyMigrationsAsync`
koji čeka dostupnost baze i automatski primjenjuje migracije. Uvedena je i
`RUN_MIGRATIONS_ONLY` konfiguracija koja omogućava pokretanje zasebnog migrator
containera neovisno od glavne aplikacije.

---

### Problem 3: Nesigurno seedanje korisničkih lozinki
Tokom razvoja uočeno je da seed korisnici u `UserSeeder.cs` imaju plain text lozinke,
što nije prihvatljivo za produkcijsko okruženje. Problem je prepoznat i označen
`TODO` komentarom u kodu, ali nije riješen do kraja projekta.

**Rješenje:** Djelimično - problem je dokumentovan u kodu. Potpuno rješenje
(hashiranje lozinki pri seedanju) ostaje kao otvorena stavka za produkcijsku verziju.

## 9. Šta bi tim unaprijedio da se projekat nastavlja

### Sigurnost
- Implementirati hashiranje lozinki u `UserSeeder.cs` i u cjelokupnom sistemu autentikacije
- Ukloniti debug output iz produkcijskih kontrolera i učvrstiti error handling u API sloju
- Provjeriti i unaprijediti sigurnost pri uploadu fajlova i statičkom serviranju sadržaja

### Frontend
- Preći na standardnu routing biblioteku (npr. React Router) radi bolje održivosti i skalabilnosti koda

### Testiranje
- Dodati end-to-end testove za ključne tokove: autentikacija, registracija, CRUD operacije i generisanje izvještaja

### Funkcionalnosti
- Dovršiti i testirati cjelokupan tok notifikacija i kotizacija s potpunom UI integracijom
