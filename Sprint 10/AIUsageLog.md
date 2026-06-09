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
Konsultacija oko planiranja entiteta za prvi sprint - odluka šta modelirati odmah, a šta odložiti.

### Kratak opis zadatka ili upita

> *"Hej, hajde ovako - posto je ovo tek prvi inkrement, a u ovom sprintu zelimo samo da ispunimo osnovne stvari oko registracije i login-a, mozda bi najbolje bilo da sada samo odradimo User entitet da bude kvalitetan, a kasnije cemo ostale kada budu zatrebali. Posto smo u timu od 8 ljudi smatram da ce to biti puno bolja kontrola i znat ce se tacno sta kada u kojem trenutku treba."*

### Šta je AI predložio ili generisao
- Potvrdio pristup - fokus na `User` entitetu u prvom sprintu je u skladu sa incremental delivery principima
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
- Bez posebnih rizika - AI je korišten kao sounding board za potvrdu već formiranog razmišljanja, a konačna odluka je donijeta od strane tima

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
- Generisana konfiguracija koristila je `latest` image verzije - potencijalni rizik za reproducibilnost builda, ručno ispravljeno
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
- Unit testove za frontend komponente (`LoginForm`, `RegisterForm`) koristeći Jest i React Testing Library - provjera renderovanja, validacije inputa i ponašanja pri submitu
- Backend unit testove za `UserService` - provjera registracije, login logike i hash-ovanja lozinke
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
- Copilot je na nekoliko mjesta generisao testove koji su prolazili, ali nisu testirali pravu stvar (lažno pozitivni rezultati) - zahtijevalo je pažljiv code review
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

## Unos #6

| Polje | Detalji |
| --- | --- |
| **Datum** | 4.05.2026 |
| **Sprint broj** | Sprint 6 |
| **Alat** | Github Copilot |
| **Ko je koristio alat** | Lamija Dženetić |

### Svrha korištenja
Pomoć pri implementaciji funkcionalnosti za upravljanje konferencijama (dodavanje, uređivanje i brisanje konferencija).

### Kratak opis zadatka ili upita
Copilot je korišten kao podrška pri razvoju i implementaciji endpointa POST/conference, PUT/conferences/:id i DELETE/conferences/:id, uključujući organizaciju logike kroz odgovarajuće slojeve aplikacije i usklađivanje sa postojećom arhitekturom projekta.

### Šta je AI predložio ili generisao
- Conference entitet s ConferenceStatus enumom
- CreateConferenceDto s Data Annotations validacijom
- ConferenceService s ručnim mapiranjem i postavljanjem Status = Active
- ConferenceRepository s EF Core implementacijom i snake_case mapiranjem
- ConferencesController s [Authorize(Policy = "AdminOrOrganizerPolicy")] i 201 Created odgovorom
- GlobalExceptionMiddleware za centralizovano upravljanje greškama
- Registraciju servisa u Program.cs

### Šta je tim prihvatio
- Većinu generisanog koda uz manje izmjene
- Strukturu foldera i nazive klasa
- Pattern za upravljanje greškama putem middleware-a

### Šta je tim izmijenio
- Prilagođen namespace prema postojećoj strukturi projekta

### Šta je tim odbacio
- Prilagođeni nazivi klasa i fajlova prema postojećim konvencijama projekta

### Rizici, problemi ili greške koje su uočene
- Copilot je kreirao duplikat IConferenceRepository interfejsa u pogrešnom sloju - ručno obrisano

---

## Unos #7

| Polje | Detalji |
| --- | --- |
| **Datum** | 4.05.2026 |
| **Sprint broj** | Sprint 6 |
| **Alat** | Github Copilot |
| **Ko je koristio alat** | Enela Pirija |

### Svrha korištenja
Pomoć pri implementaciji frontend-a za upravljanje konferencijama (dodavanje, uređivanje i brisanje konferencija).

### Kratak opis zadatka ili upita
Copilot je korišten kao podrška pri razvoju i implementaciji React + TypeScript komponenti koje koriste useState za upravljanje formama, axios za API pozive i prilagođeni CSS/Tailwind stil za korisnički interfejs.

### Šta je AI predložio ili generisao
- Početne verzije form komponenti za kreiranje i uređivanje konferencija
- Primjere validacije forme i upravljanja stanjem
- API pozive za CRUD operacije nad konferencijama
- Prijedloge za organizaciju hookova i komponenti
- Stilizaciju kartica i modalnih formi

### Šta je tim prihvatio
- Strukturu API funkcija baziranih na axios biblioteci
- Organizaciju komponenti i hookova
- Dio stilizacije i rasporeda UI elemenata
- Pattern za osvježavanje podataka nakon izmjena

### Šta je tim izmijenio
- Nije odbačeno ništa suštinsko - sve izmjene su bile tehničke korekcije verzija biblioteka.

### Šta je tim odbacio
- Prilagođeni su nazivi i tipovi polja kako bi odgovarali backend modelu
- Ručno su ispravljene TypeScript i build greške
- Validacija i upravljanje stanjem implementirani su pomoću useState umjesto dodatnih biblioteka

### Rizici, problemi ili greške koje su uočene
- Tokom razvoja Copilot je predlagao implementacije zasnovane na zastarjelim verzijama pojedinih biblioteka, što je dovelo do build i kompatibilnosnih grešaka


## Unos #8

| Polje | Detalji |
| --- | --- |
| **Datum** | 06.05.2026. |
| **Sprint broj** | Sprint 6 |
| **Alat** | GitHub Copilot |
| **Ko je koristio alat** | Tarik Babahmetović |

### Svrha korištenja
Pomoć pri implementaciji backend ruta i frontend integracije za upravljanje korisničkim postavkama i administratorskim funkcionalnostima.

### Kratak opis zadatka ili upita
Copilot je korišten kao podrška pri generisanju ASP.NET Core API ruta, kontrolera i frontend React komponenti za prikaz i izmjenu korisničkih postavki, uključujući povezivanje sa postojećim servisima i autentifikacijom.

### Šta je AI predložio ili generisao
- API rute za dohvat i izmjenu korisničkih postavki
- Primjere kontrolera sa `[Authorize]` atributima
- React + TypeScript komponente za prikaz forme korisničkih postavki
- Axios pozive prema backend endpointima

### Šta je tim prihvatio
- Generisane API rute
- HTML sadržaj korisničkih postavki
- Organizaciju komponenti i servisnih klasa na backendu

### Šta je tim izmijenio
- CSS forme za unos podataka
- Ostajanje na dashboard stranici umjesto redirect na posebnu /settings stranicu

### Šta je tim odbacio
- Strukturu koda po folderima na frontendu

### Rizici, problemi ili greške koje su uočene
- Copilot nije pratio folder strukturu projekta
- Copilot nije učio grešku u korištenju Expression izraza

---

## Unos #9

| Polje | Detalji |
| --- | --- |
| **Datum** | 07.05.2026. |
| **Sprint broj** | Sprint 6 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Tarik Babahmetović |

### Svrha korištenja
Pomoć pri radu sa Git granama i pravilnom prebacivanju commit-a između branch-eva korištenjem `git cherry-pick` komande.

### Kratak opis zadatka ili upita
Tražena je pomoć oko korištenja `git cherry-pick` komande kako bi se određeni commit-i sa razvojne grane pravilno prebacili na drugu granu jer git nije prepoznavao promjene pri merge-u.

### Šta je AI predložio ili generisao
- Objašnjenje rada `git cherry-pick` komande
- Primjere prebacivanja jednog i više commit-a
- Korake za rješavanje konflikata tokom cherry-pick procesa
- Savjete za provjeru historije commit-a pomoću `git log --oneline`
- Objašnjenje kada koristiti `cherry-pick`, a kada `merge` ili `rebase`

### Šta je tim prihvatio
- Predloženi workflow za selektivno prebacivanje commit-a
- Korištenje `git log --oneline` za identifikaciju commit hash-eva
- Postupak za rješavanje konflikata tokom cherry-pick-a

### Šta je tim izmijenio
- Commit-i su reorganizovani prije cherry-pick procesa kako bi historija bila preglednija
- Nisu korišteni merge commit-i u cherry-pick komandi

### Šta je tim odbacio
- Korištenje `git rebase` pristupa u ovom slučaju zbog mogućnosti komplikovanja timskog workflow-a

### Rizici, problemi ili greške koje su uočene
- Postojala je mogućnost konflikata zbog razlika između grana, pa su svi cherry-pick commit-i dodatno testirani prije push-a na remote repozitorij

---

## Unos #10

| Polje | Detalji |
| --- | --- |
| **Datum** | 07.05.2026. |
| **Sprint broj** | Sprint 6 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Tarik Babahmetović |

### Svrha korištenja
Pomoć pri konfiguraciji SSH pristupa serveru i pokretanju aplikacije na VPS infrastrukturi.

### Kratak opis zadatka ili upita
Claude AI je korišten za pomoć pri generisanju SSH komandi, konfiguraciji pristupa serveru pomoću SSH ključeva i pokretanju Docker servisa na udaljenom VPS serveru.

### Šta je AI predložio ili generisao
- Korake za SSH pristup serveru
- Komande za pokretanje i provjeru Docker kontejnera (`docker compose up -d`, `docker ps`, `docker logs`)
- Debug grešaka prilikom pokretanja kontejnera

### Šta je tim prihvatio
- Docker komande za pokretanje i nadzor aplikacije
- Organizaciju deployment workflow-a na serveru

### Šta je tim izmijenio
- Putanje i nazivi servisa prilagođeni stvarnoj infrastrukturi projekta
- Naziv i IP adresa servera u ssh komandi

### Šta je tim odbacio
- Korištenje HTTP protokola umjesto HTTPS za brzo rješavanje problema

### Rizici, problemi ili greške koje su uočene
- Nisu uočeni rizici


## Unos #11

| Polje | Detalji |
| --- | --- |
| **Datum** | 10.05.2026. |
| **Sprint broj** | Sprint 7 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Lamija Dženetić |

### Svrha korištenja
Pomoć pri implementaciji frontenda za upravljanje sesijama konferencije.

### Kratak opis zadatka ili upita
Claude AI je korišten kao konsultant i debugger tokom razvoja session modula na frontendu. Uključivalo je i debuggovanje build grešaka, ispravljanje DateTime problema sa PostgreSQL-om i rješavanje prefill problema edit forme.

### Šta je AI predložio ili generisao
- Rješenje za MISSING_EXPORT grešku (fetchUsers nije bio exportovan iz sessionApi.ts)
- Ispravljanje DateTime UTC greške - konverzija datuma u ISO format prije slanja na backend
- Ispravljanje datetime-local input format greške (.toISOString().slice(0, 16))
- Dijagnozu i rješenje za 400 Bad Request na PUT /api/sessions (DateTime Kind=Unspecified)
- Ispravljanje JSX sintaksne greške - modal van return bloka (dodavanje React Fragment)
- Rješenje za prefill predavača u edit formi - čekanje na učitavanje users liste (users.length > 0)
- Dijagnozu da backend SessionListDTO ne vraća roomId, roomName, speakerName, assignedSpeakerId

### Šta je tim prihvatio
- Sve predložene ispravke grešaka
- Proširenje SessionListDTO sa nedostajućim poljima
- Logiku prefilla edit forme sa uvjetom users.length > 0

### Šta je tim izmijenio
- Nazivi varijabli i funkcija usklađeni sa konvencijom projekta
- Prilagođeni endpointi prema stvarnoj backend strukturi projekta

### Šta je tim odbacio
- Određene prijedloge za refaktoring postojećeg koda koji nisu bili neophodni

### Rizici, problemi ili greške koje su uočene
- Build greške zbog sintaksnih problema zahtijevale su ručnu intervenciju

## Unos #12

| Polje | Detalji |
| --- | --- |
| **Datum** | 10.05.2026. |
| **Sprint broj** | Sprint 7 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Enela Pirija |

### Svrha korištenja
Pomoć pri implementaciji backend ruta za upravljanje sesijama (CRUD).

### Kratak opis zadatka ili upita
Claude AI je korišten kao podrška pri generisanju ASP.NET Core API ruta za upravljanje sesijama u SessionController, te kreiranju nove API putanje u UserController koja omogućava filtriranje korisnika po ulozi (role) radi popunjavanja frontend dropdown menija.

### Šta je AI predložio ili generisao
- Strukturu SessionsController sa individualnim atributima na svakoj akciji umjesto globalne polise.
- Novu metodu u UserService (GetUsersByRoleAsync) koja filtrira korisnike na osnovu uloge.
- Novu rutu u UserController (/api/users/by-role) sa [FromQuery] parametrom za dinamičko filtriranje.

### Šta je tim prihvatio
- Sve predložene ispravke.
- Dodavanje nove metode u servisni sloj umjesto modifikacije postojećih GetAll metoda.
- Korištenje fiksne rute /api/users/by-role radi konzistentnosti sa ostatkom API-ja.

### Šta je tim izmijenio
- Nazivi DTO objekata usklađeni sa postojećom šemom u projektu

### Šta je tim odbacio
- Prvobitni prijedlog korištenja [Allow Anonymus] na nivou cijelog kontrolera; odlučeno je da se svaka ruta štiti pojedinačno radi veće sigurnosti.
- Prijedlog filtriranja direktno u repozitoriju (DAL sloj) je odložen za kasniju optimizaciju, trenutno se koristi filtriranje u servisu zbog manje količine podataka.

### Rizici, problemi ili greške koje su uočene
- Build greške zbog sintaksnih problema zahtijevale su ručnu intervenciju
- Potrebno je osigurati da se role u bazi (mala slova) podudara sa onim što frontend šalje u query stringu.

## Unos #13

| Polje | Detalji |
|---|---|
| **Datum** | 18.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Emira Kurtović |

### Svrha korištenja
Konsultacije i pomoć oko popravke CI/CD pipeline-a.

### Kratak opis zadatka ili upita

> *"Korišten AI alat za pomoć pri shvatanju uzroka pada CI/CD i popravke istog."*

### Šta je AI predložio ili generisao
- Popravka testnog fajla
- Popravka fajla koji se testira
  
### Šta je tim prihvatio
- Izmijena testnog fajla

### Šta je tim izmijenio
- Način izmijene
  
### Šta je tim odbacio
- Popravku fajla koji se testira

### Rizici, problemi ili greške koje su uočene
- Čest pad testova na CI/CD.

## Unos #14

| Polje | Detalji |
|---|---|
| **Datum** | 18.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Emira Kurtović |

### Svrha korištenja
Konsultacije i pomoć oko implementacije backend taskova S41 i S42 — pregled kapaciteta konferencije/sesije i lista učesnika.

### Kratak opis zadatka ili upita

Korišten AI alat za pomoć pri implementaciji backend funkcionalnosti: kreiranje DTO-ova, interfejsa, servisa i controllera za endpoint kapaciteta konferencije (GET /conferences/:id/capacity), kapaciteta sesije (GET /sessions/:id/capacity) i liste učesnika (GET /conferences/:id/participants) sa podrškom za pretragu i filtriranje.

### Šta je AI predložio ili generisao
- CapacityDto.cs i ParticipantDto.cs — novi DTO-ovi
- IConferenceCapacityService.cs — novi interfejs
- ConferenceCapacityService.cs — implementacija servisa sa logikom kapaciteta i filtriranja učesnika
- ConferenceCapacityController.cs — novi controller sa endpointima za kapacitet i listu učesnika
- Dodavanje GetByIdWithRegistrationsAsync metode u ISessionRepository i SessionRepository
- Izmjena SessionsController.cs — dodavanje capacity endpointa i IConferenceCapacityService dependencya
- Registracija servisa u Program.cs
  
### Šta je tim prihvatio
- Kompletnu strukturu DTO-ova, interfejsa i servisa
- Pattern konzistentan sa ostatkom projekta (Policy-based autorizacija, CancellationToken, KeyNotFoundException handling)
- Korištenje postojećih metoda iz repozitorija (GetConfirmedCountForConferenceAsync, GetRegistrationsByConferenceAsync)
 
### Šta je tim izmijenio
- Prilagođeno korištenju postojećih metoda repozitorija umjesto novih koje je AI inicijalno predložio
  
### Šta je tim odbacio
- Inicijalni prijedlog servisa koji je pozivao nepostojeće metode (GetConfirmedRegistrationsForConferenceAsync) — zamijenjeno postojećim ekvivalentima

### Rizici, problemi ili greške koje su uočene
- Potrebno provjeriti da li RegistrationStatus vrijednosti u bazi odgovaraju onima koje servis koristi za filtriranje (npr. "Confirmed" vs "confirmed")
- Session capacity koristi MaxParticipants od konferencije, što treba validirati sa ostatkom tima jer sesija nema vlastiti limit

---
  
## Unos #15

| Polje | Detalji |
|---|---|
| **Datum** | 18.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Nejra Hodžić |

### Svrha korištenja

Pomoć pri implementaciji i organizaciji backend i frontend testova za Sprint 8 funkcionalnosti.

### Kratak opis zadatka ili upita

Tražena je pomoć pri planiranju i pisanju testova za funkcionalnosti upravljanja dvoranama, dodjele dvorane sesiji, agende konferencije, kapaciteta konferencije/sesije i liste učesnika.

### Šta je AI predložio ili generisao

- Prijedloge backend test scenarija za:
  - upravljanje dvoranama (S35)
  - dodjelu dvorane sesiji (S36)
  - agendu konferencije (S34)
  - kapacitet konferencije i listu učesnika (S41, S42)

- Prijedloge frontend test scenarija za:
  - RoomsPage
  - SessionForm
  - AgendaPage
  - ConferenceDetailsPage

### Šta je tim prihvatio

- Strukturu backend i frontend testova
- Organizaciju testnih scenarija po funkcionalnostima

### Šta je tim izmijenio

- Testovi su prilagođeni postojećoj strukturi projekta

### Šta je tim odbacio

- Korištenje EF InMemory pristupa za finalne backend testove
- Testove koji nisu odgovarali postojećoj arhitekturi projekta
- Izmjene produkcijskog koda radi lakšeg testiranja

### Rizici, problemi ili greške koje su uočene

- Pojedini backend kontroleri nisu bili pogodni za potpune Moq unit testove zbog direktnog korištenja `ApplicationDbContext`

## Unos #16
 
| Polje | Detalji |
|---|---|
| **Datum** | 15.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | GitHub Copilot |
| **Ko je koristio alat** | Enela Pirija |
 
### Svrha korištenja
Pomoć pri implementaciji frontend modula za upravljanje dvoranama.
 
### Kratak opis zadatka ili upita
GitHub Copilot je korišten kao podrška pri razvoju React + TypeScript komponenti za upravljanje dvoranama, uključujući kreiranje stranice, liste i CRUD formi, uz usklađivanje sa postojećom arhitekturom projekta.
 
### Šta je AI predložio ili generisao
- Stavku "Dvorane" u sidebar navigaciji vidljivu samo Adminu i Organizatoru
- RoomsPage komponentu po uzoru na ConferencesPage i SessionsPage
- Listu dvorana sa karticama koje prikazuju naziv, lokaciju, kapacitet i opis
- AddRoomModal sa validacijom kapaciteta i prikazom backend grešaka ispod polja
- EditRoomModal sa prefill poljima i PUT /api/rooms/{id} pozivom
- Confirmation dialog za brisanje dvorane sa prikazom backend poruke greške
- useRooms hook i roomApi funkcije za komunikaciju sa backendom

### Šta je tim prihvatio
- Strukturu RoomsPage, AddRoomModal i EditRoomModal komponenti
- Pattern renderovanja modala i upravljanja stanjem
- Organizaciju fajlova unutar features/room foldera

### Šta je tim izmijenio
- Ispravljen useEffect koji je preuranjeno redirectao korisnika na dashboard prije završetka autentifikacije
- Modal premješten van <main> taga u React Fragment kako bi position: fixed ispravno funkcionisao
- useRooms hook ispravljen da koristi direktni fetch poziv sa Authorization headerom umjesto pogrešnog mehanizma za API pozive

### Šta je tim odbacio
- Inicijalni pristup API pozivima u useRooms hooku koji nije bio usklađen sa postojećim patternom projekta

### Rizici, problemi ili greške koje su uočene
- Generisani useRooms hook nije koristio ispravan pattern za API pozive, što je uzrokovalo ponavljajuće greške u konzoli
- Preuranjeni redirect u useEffect-u blokirao je prikaz stranice prije završetka učitavanja autentifikacije
- Modal nije bio vidljiv kada je renderovan unutar parent elementa sa određenim CSS propertyjem — riješeno premještanjem van <main> taga


## Unos #17
 
| Polje | Detalji |
|---|---|
| **Datum** | 14.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | Gemini |
| **Ko je koristio alat** | Lamija Dženetić |
 
### Svrha korištenja
Pomoć pri implementaciji backend funkcionalnosti za upravljanje dvoranama (S35) i dodjelu dvorane sesiji (S36)
 
### Kratak opis zadatka ili upita
Gemini je korišten kao podrška pri razvoju CRUD endpointa za dvorane (GET/POST/PUT/DELETE /rooms) i logike dodjele dvorane sesiji (PUT /sessions/:id/room), uključujući validaciju duplikata, provjeru zauzetosti termina i autorizaciju po rolama.
 
### Šta je AI predložio ili generisao
- Room entitet i odgovarajući DTO-ovi (CreateRoomDto, UpdateRoomDto, RoomDto)
- RoomsController sa CRUD rutama i [Authorize(Policy = "AdminOrOrganizerPolicy")] zaštitom
- IRoomService interfejs i RoomService implementacija sa validacijom duplikata (naziv + lokacija)
- IRoomRepository i RoomRepository sa EF Core implementacijom
- Logiku u SessionsController / servisnom sloju za PUT /sessions/:id/room — zamjena hardkodiranog seeda i provjera zauzetosti termina (ista dvorana ne smije imati dvije sesije u istom terminu)
- Registraciju servisa u Program.cs

### Šta je tim prihvatio
- Ukupnu strukturu RoomsController-a, servisa i repozitorija
- Pattern validacije duplikata (provjera naziva + lokacije u RoomService)
- Logiku provjere zauzetosti termina pri dodjeli dvorane sesiji
- Autorizacijski pristup konzistentan s ostatkom projekta

### Šta je tim izmijenio
- Nazivi klasa i namespace-ovi usklađeni sa konvencijom projekta
- Prilagođeni odgovori i HTTP status kodovi prema postojećem API standardu projekta
- Logika provjere zauzetosti termina dorađena prema stvarnoj strukturi Session entiteta (polja StartTime/EndTime)
  
### Šta je tim odbacio
- Inicijalne prijedloge koji su koristili nepostojeće metode repozitorija — zamijenjeno postojećim ekvivalentima
- Dio generisanog scaffolding koda koji nije odgovarao folder strukturi projekta

### Rizici, problemi ili greške koje su uočene
- Potrebno provjeriti da li provjera zauzetosti termina ispravno pokriva rubne slučajeve (sesije koje se tačno dotiču vremenski)
- Gemini inicijalno nije poznavao postojeću strukturu repozitorija, pa je predlagao nove metode umjesto korištenja postojećih

---

## Unos #18

| Polje | Detalji |
| --- | --- |
| **Datum** | 16.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | Gemini / Antigravity |
| **Ko je koristio alat** | Hamza Kovač |

### Svrha korištenja
Pomoć pri end-to-end (fullstack) implementaciji modula "Agenda konferencije" (Tim B), te integracija sa postojećim frontend i backend rješenjima.

### Kratak opis zadatka ili upita
AI asistentu je dat zadatak da analizira kompletan postojeći kod (React frontend, .NET backend) i u potpunosti implementira CRUD operacije za novi entitet `AgendaItem`. Zahtijevano je striktno poštovanje postojećih stilova, arhitekture (Clean Architecture), `naming` konvencija, sigurnosnih polisa, te efektivna primjena na frontendu koristeći postojeće `features` organizacije i React komponente.

### Šta je AI predložio ili generisao
- **Backend**: Ažuriran `AgendaItem` entitet, generisani novi interfejsi i repozitoriji (`IAgendaItemRepository`, `AgendaItemRepository`), servisi za poslovnu logiku (`AgendaItemService` uz obaveznu validaciju vremena i tipova sesije), te API kontroleri (`AgendaController`) sa ispravnim autorizacijskim polisama (AdminOrOrganizerPolicy i ParticipantPolicy). Također je kreirana i EF Core baza-migracija.
- **Frontend**: Izgenerisani TypeScript tipovi, konfigurisan Axios API klijent (`agendaApi.ts`), custom React hook (`useAgenda.ts`), te interaktivne i responzivne komponente (`AgendaList.tsx`, `AgendaForm.tsx`) koje imaju uvjetno renderiranje na osnovu tipa `AgendaItem`-a. Uključene rute unutar globalnog routera.

### Šta je tim prihvatio
- Gotovo cjelokupno predloženo rješenje arhitekture modula.
- Predložena backend validacija, kao i integracija relacija baze (`RoomId` i `SessionId` unutar `AgendaItems`).
- Prijedlog UI/UX interfejsa na React-u i vizuelni "timeline" prikaz agende razdvojen po datumima uz bedževe po bojama.

### Šta je tim izmijenio
- Konfiguraciju i pokretanje komandi unutar backend foldera zbog specifičnosti kako su riješeni solution fajlovi, a koje AI inicijalno nije ispravno percipirao.

### Šta je tim odbacio
- Nema većih odbačenih stavki – kod je pratio dogovorene standarne.

### Rizici, problemi ili greške koje su uočene
- Zbog nedostatka globalne `tsc` komande na frontend folderu, AI komanda za testni build nije prošla u potpunosti, što je zahtijevalo razumijevanje lokalnog npm paketa tima za testiranje kompajliranja.
- AI je u prvom pokušaju pokretanja .NET build-a naišao na problem sa pronalaženjem tačnog `.sln` fajla, tako da se komanda morala usmjeriti direktno u `Api` folder.

---

## Unos #19

| Polje | Detalji |
| --- | --- |
| **Datum** | 17.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | GitHub Copilot |
| **Ko je koristio alat** | Tarik Babahmetović |

### Svrha korištenja
Pomoć pri implementaciji timeline dizajna za prikaz agende konferencije na frontendu.

### Kratak opis zadatka ili upita
Copilot je korišten kao podrška pri razvoju React + TypeScript komponente za vizualni prikaz agende konferencije u obliku timeline-a, uključujući organizaciju agenda itema po vremenskim slotovima i stilizaciju komponenti.

### Šta je AI predložio ili generisao
- Timeline komponentu za prikaz agende konferencije po hronološkom redoslijedu
- Stilizaciju vremenskih oznaka i kartica stavki agende
- Grupiranje stavka agende po vremenskim slotovima unutar jednog dana
- Responsive raspored komponenti

### Šta je tim prihvatio
- Osnovu timeline strukture i vizualnu organizaciju stavki agende
- Grupiranje stavki agende po vremenskim slotovima
- Prikaz relevantnih informacija unutar kartica stavki agende

### Šta je tim izmijenio
- Stilizacija prilagođena dizajnu sistema ostatka aplikacije
- Prevedeni nazivi tipova stavki agende na bosanski jezik

### Šta je tim odbacio
- Stari izgled agende kao jednostavne tabele

### Rizici, problemi ili greške koje su uočene
- Generisana komponenta nije inicijalno vodila računa o datumima stavki agende nego samo satnicama
- Nemogućnost Copilota da ispravno postavi padding za stavke u originalnoj implementaciji zasnovanoj na tabeli

---

## Unos #20

| Polje | Detalji |
| --- | --- |
| **Datum** | 17.05.2026. |
| **Sprint broj** | Sprint 8 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Tarik Babahmetović |

### Svrha korištenja
Pomoć pri generisanju nove EF Core migracije za bazu podataka.

### Kratak opis zadatka ili upita
Claude AI je korišten kao podrška pri kreiranju nove database migracije nakon izmjena na domenskim entitetima, uključujući dijagnozu grešaka pri generisanju migracije i usklađivanje sa postojećim stanjem baze.

### Šta je AI predložio ili generisao
- Komande za generisanje nove migracije (`dotnet ef migrations add`, `dotnet ef database update`)
- Dijagnozu grešaka koje su se javljale pri pokretanju migracije
- Prijedlog za provjeru konzistentnosti postojećih migracija sa trenutnim stanjem modela
- Upute za ručnu korekciju migracijskog fajla u slučajevima kada automatski generisani kod nije odgovarao očekivanom stanju

### Šta je tim prihvatio
- Predložene EF Core komande za generisanje i primjenu migracije
- Korake za dijagnozu i otklanjanje grešaka pri migraciji
- Pristup ručnoj korekciji dijela migracijskog fajla

### Šta je tim izmijenio
- Nazivi migracije i tabela usklađeni sa konvencijom imenovanja u projektu

### Šta je tim odbacio
- Prijedlog za resetovanje svih migracija i ponovnu inicijalizaciju baze

### Rizici, problemi ili greške koje su uočene
- Postojala je mogućnost konflikta između lokalno generisane migracije i stanja baze na VPS-u — migracija je testirana lokalno prije primjene na server
- Nisu uočeni dodatni rizici

---
## Unos #21

| Polje | Detalji |
|---|---|
| Datum | 25.05.2026. |
| Sprint broj | Sprint 9 |
| Alat | ChatGPT |
| Ko je koristio alat | Ajra Kerla |

### Svrha korištenja
Pomoć pri implementaciji i testiranju backend i frontend funkcionalnosti za Sprint 9, uključujući Q&A panel, sistem notifikacija, upload materijala i predavački dashboard.

### Kratak opis zadatka ili upita
ChatGPT je korišten kao podrška pri pisanju unit testova za backend servise i frontend komponente, dijagnostici grešaka u postojećim testovima, mockovanju zavisnosti i usklađivanju testova sa trenutnom implementacijom aplikacije.

### Šta je AI predložio ili generisao
- Backend unit testove za:
  - `NotificationService`
  - `QuestionService`
  - `MaterialService`
- Mock konfiguracije za repozitorije i servise korištenjem `Moq`
- Frontend testove za `NotificationBell` komponentu koristeći `Vitest` i `React Testing Library`
- Korekcije postojećih frontend testova (`SessionList.test.tsx`)
- Upute za rješavanje problema sa frontend testovima i nedostajućim dependency paketima
- Strukturu i sadržaj dokumentacije:
  - `ProofOfTesting.md`
  - `SprintRetrospectiveSummary.md`

### Šta je tim prihvatio
- Predložene backend unit testove
- Mock konfiguracije i test scenarije
- Frontend testove za notifikacije
- Korekcije postojećih testova
- Strukturu dokumentacije za Sprint 9

### Šta je tim izmijenio
- Nazive pojedinih testova i dijelove tekstualnih opisa radi usklađivanja sa stilom ostatka projekta
- Dio frontend testova prilagođen je trenutnom UI prikazu i postojećim komponentama

### Šta je tim odbacio
- Prijedlog za detaljnije refaktorisanje postojećih frontend testova iz prethodnih sprintova koji nisu bili direktno povezani sa Sprint 9 funkcionalnostima

### Rizici, problemi ili greške koje su uočene
- Postojala je mogućnost neusaglašenosti između postojećih frontend testova i novog UI ponašanja nakon izmjena komponenti
- Uočeni su problemi sa nedostajućim frontend dependency paketima (`react-datepicker`) koji su naknadno instalirani
- Backend testovi su dodatno prilagođeni nakon uvođenja novih zavisnosti (`INotificationService`) u postojeće servise
*Dokument se ažurira tokom trajanja projekta. Svaki novi slučaj korištenja AI dodaje se kao novi unos.*


## Unos #22
| Polje | Detalji |
|---|---|
| **Datum** | 30.05.2026. |
| **Sprint broj** | Sprint 10 |
| **Alat** | Claude AI |
| **Ko je koristio alat** | Emira Kurtović |
 
### Svrha korištenja
Konsultacije i pomoć oko implementacije backend i frontend taskova S49 — izvještaji za organizatora konferencije.
 
### Kratak opis zadatka ili upita
> *"Korišten AI alat za kompletnu implementaciju S49 story-a — generisanje izvještaja za organizatora konferencije (statistike prijava, popunjenost sesija, broj predavača i materijala) sa mogućnošću preuzimanja u PDF formatu. Implementacija uključuje backend (DTOs, interfejs, servis, controller) i frontend (API service, stranica izvještaja, routing, dugme na details stranici)."*
 
### Šta je AI predložio ili generisao
**Backend:**
- `ReportDto.cs` — novi DTO-ovi (`ConferenceReportDto`, `RegistrationStatsDto`, `SessionReportDto`)
- `IConferenceReportService.cs` — novi interfejs sa `GetReportAsync` i `GenerateReportPdfAsync`
- `ConferenceReportService.cs` — implementacija servisa sa logikom za agregaciju podataka i generisanje PDF-a putem QuestPDF biblioteke
- `ConferenceReportController.cs` — novi controller sa endpointima `GET /conferences/:id/report` i `GET /conferences/:id/report/download`
- Dodavanje `GetSessionsByConferenceIdWithDetailsAsync` metode u `ISessionRepository` i `SessionRepository` (eager loading `Room`, `SessionRegistrations`, `Materials`)
- Registracija servisa u `Program.cs`
**Frontend:**
- `reportApi.ts` — API service sa `fetchConferenceReport` i `downloadConferenceReport` funkcijama
- `ConferenceReportPage.tsx` — stranica izvještaja sa prikazom statistika prijava, sesija, predavača i materijala
- `src/pages/ConferenceReportPage.tsx` — wrapper stranica
- Dodavanje rute `/conferences/:id/report` u `router.tsx`
- Dodavanje dugmeta "Izvještaj" na `ConferenceDetailsPage.tsx`
### Šta je tim prihvatio
- Kompletnu strukturu DTO-ova, interfejsa i servisa
- Pattern konzistentan sa ostatkom projekta (Policy-based autorizacija, `AdminOrOrganizerPolicy`, `CancellationToken`, `KeyNotFoundException` handling)
- QuestPDF kao biblioteku za generisanje PDF-a
- Frontend pattern konzistentan sa postojećim stranicama (isti CSS klase, isti fetch pattern sa Bearer tokenom)
- Provjeru vlasništva nad konferencijom za organizatora (isti pattern kao u `ConferenceCapacityController`)
### Šta je tim izmijenio
- Prilagođeno korištenju postojećih metoda repozitorija (`GetRegistrationsByConferenceAsync` umjesto nepostojeće metode)
- Uklonjena ovisnost o `IMaterialRepository` u servisu — materijali se dohvataju kroz `Session.Materials` navigation property
- Ispravka parsiranja `Room.Capacity` — polje je `int`, ne `string`, pa je `TryParse` zamijenjen direktnim pristupom
- Dugme "Izvještaj" ograničeno na `isAdmin || isOwner` umjesto `canSeeCapacity` — da se ne prikazuje organizatorima tuđih konferencija
### Šta je tim odbacio
- Inicijalni prijedlog servisa koji je koristio `IMaterialRepository.GetByConferenceIdAsync` — metoda ne postoji, zamijenjeno eager loadingom kroz sesije
- Inicijalni prijedlog koji je pozivao nepostojeće metode repozitorija (`GetByConferenceIdAsync` na session i material repozitorijima)
### Rizici, problemi ili greške koje su uočene
- `ConferenceReportController.cs` je inicijalno kreiran kao prazan fajl — kod nije bio kopiran, što je uzrokovalo da controller nije bio registrovan i endpoint je vraćao 404
- QuestPDF je instaliran na pogrešnom projektu (`docker-compose.dcproj`) — trebalo ga je instalirati na `ConferenceManagement.Application.csproj`
- Ambiguous reference između `QuestPDF.Fluent.Document` i `System.Reflection.Metadata.Document` — riješeno dodavanjem eksplicitnog aliasa
- `Organizers` lista na konferenciji bila prazna za testnog korisnika, što je uzrokovalo da organizator nije mogao pristupiti report endpointu — problem sa test podacima, ne sa kodom


## Unos #23
| Polje | Detalji |
|---|---|
| **Datum** | 31.05.2026. |
| **Sprint broj** | Sprint 10 |
| **Alat** | GitHub Copilot |
| **Ko je koristio alat** | Ajdin Kanlić |
 
### Svrha korištenja
Implementacija i ubrzanje razvoja frontend komponenti za upravljanje logističkim aktivnostima na stranici konferencije (User Stories S46.1 — S46.4).
 
### Kratak opis zadatka ili upita
> *"Korišten GitHub Copilot kao AI asistent za kompletnu frontend implementaciju funkcionalnosti vezanih za logističke aktivnosti unutar aplikacije. Implementacija obuhvata prikaz liste sa filtriranjem po tipu i detaljnim pregledom (S46.1), formu za kreiranje sa predefinisanim dropdown-om (S46.2), formu za izmjenu postojećih polja (S46.3), te reaktivno brisanje stavki uz potvrdu kroz modalni dijalog (S46.4), osiguravajući da su sve promjene odmah vidljive u UI-ju."*
 
### Šta je AI predložio ili generisao
**Frontend:**
- `LogisticActivitiesList.tsx` — Komponenta za prikaz liste logističkih aktivnosti sa ugrađenom logikom za filtriranje po tipu i uslovnim renderovanjem poruke kada nema unesenih aktivnosti.
- `LogisticActivityDetails.tsx` — Prošireni prikaz detalja pojedinačne logističke aktivnosti koji se aktivira na klik korisnika.
- `CreateLogisticActivityForm.tsx` — Forma za kreiranje nove aktivnosti sa predefinisanim dropdown menijem za odabir tipa i logikom za slanje podataka na backend.
- `EditLogisticActivityForm.tsx` — Forma za izmjenu logističke aktivnosti sa predistovremenim popunjavanjem polja trenutnim vrijednostima (re-populating state).
- `DeleteActivityModal.tsx` — Modalni dijalog za potvrdu brisanja aktivnosti prije slanja destruktivnog API zahtjeva.
- Logika za upravljanje lokalnim stanjem (state management) kako bi se osiguralo da se kreirane, izmijenjene ili obrisane aktivnosti odmah reaktivno ažuriraju u listi bez osvežavanja stranice.

### Šta je tim prihvatio
- Kompletnu TSX strukturu komponenti, formi i modalnih dijaloga.
- Logiku za klijentsko filtriranje liste na osnovu izabranog tipa aktivnosti.
- Reaktivni pattern ažuriranja stanja (state update) nakon uspješnih API poziva (POST, PUT, DELETE), čime je postignuto da su promjene odmah vidljive.
- Konzistentan UX pristup sa modalnim dijalogom za brisanje i jasnim notifikacijama o uspjehu.

### Šta je tim izmijenio
- Prilagođene su CSS klase i Tailwind stilovi kako bi se komponente vizuelno potpuno uskladile sa postojećim dizajnom stranice pojedinačne konferencije.
- TypeScript interfejsi (tipizacija) za logističke aktivnosti su izmješteni iz lokalnih fajlova u zajednički `types.ts` modul radi ponovne iskoristivosti.
- Dropdown za odabir tipa aktivnosti je povezan sa ENUM vrijednostima koje dolaze direktno sa backend API-ja, umjesto hardkodovanih stringova koje je Copilot inicijalno predložio.

### Šta je tim odbacio
- Inicijalni prijedlog za brisanje aktivnosti koji je koristio nativni `window.confirm()` pretraživača — odbačen je u korist custom napisanog modalnog dijaloga radi boljeg korisničkog iskustva (UX).
- Predloženu inline validaciju formi koja nije pratila validacioni pattern (React Hook Form / Yup) usvojen na ostatku projekta.

### Rizici, problemi ili greške koje su uočene
- Copilot je u formi za izmjenu (`EditLogisticActivityForm.tsx`) greškom propustio proslijediti ID aktivnosti kroz parametre API poziva, što je uzrokovalo `400 Bad Request` na backendu dok greška nije ručno ispravljena.
- Inicijalno generisana forma za kreiranje nije resetovala svoja polja nakon uspješnog slanja podataka, pa su stari unosi ostajali vidljivi u formi.
- Prilikom asinhronog učitavanja podataka, poruka "Nema aktivnosti" bi se nakratko prikazala prije nego što se podaci zapravo povuku s mreže — riješeno uvođenjem eksplicitnog `isLoading` stanja.

## Unos #24

| Polje | Detalji |
| --- | --- |
| **Datum** | 30.05.2026. |
| **Sprint broj** | Sprint 10 |
| **Alat** | Claude |
| **Ko je koristio alat** | Enela Pirija |

### Svrha korištenja
Pomoć pri implementaciji i testiranju backend CRUD modula za upravljanje logističkim aktivnostima konferencije (S46.1 do S46.4).

### Kratak opis zadatka ili upita
AI je korišten kao podrška pri razvoju endpoint-a za logističke zadatke. To je uključivalo kreiranje, uređivanje, brisanje i vraćanje svih logističkih zadataka 
za određenu konferenciju., kao i njihovo filtritanje.

### Šta je AI predložio ili generisao
- **Domenski i DAL sloj:** `ILogisticsRepository` i njegovu EF Core implementaciju `LogisticsRepository` sa sirovim SQL upitom za provjeru organizatora konferencije.
- **Aplikacijski sloj:** `ILogisticsService` i `LogisticsService` koji upravljaju biznis logikom i mapiranjem podataka u DTO-ove.
- **API sloj:** `LogisticsController` sa 4 osnovne CRUD rute zaštićene putem `[Authorize(Policy = "AdminOrOrganizerPolicy")]`.
- **Arhitektonsko rješenje:** Premještanje hvatanja `DbUpdateConcurrencyException` iz aplikacijskog u DAL sloj radi očuvanja čistih zavisnosti.

### Šta je tim prihvatio
- Cjelokupnu strukturu i organizaciju CRUD operacija kroz sve arhitektonske slojeve.
- Arhitektonski pristup rješavanju EF Core greške izmještanjem concurrency logike unutar repozitorija.
- JSON format povratnih poruka (HTTP 200) prilikom uspješnog brisanja aktivnosti.

### Šta je tim izmijenio
- **Prelazak na LINQ:** Sirovi SQL upit za provjeru organizatora unutar `IsUserOrganizerOfConferenceAsync` zamijenjen je čistim LINQ upitom koristeći navigacijsku kolekciju `Organizers` nakon analize strukture entiteta `Conference` i `User`.
- **Ispravka grešaka u kucanju:** Ručno je korigovan nepostojeći status kod `StatusCodes.Status211Created` u ispravni `StatusCodes.Status201Created` unutar POST metode kontrolera.

### Šta je tim odbacio
- Prijedlog o opcionalnoj instalaciji `Microsoft.EntityFrameworkCore` paketa direktno u aplikacijski sloj aplikacije, kako se ne bi prekršila pravila strogo razdvojenih slojeva (Clean Architecture).

### Rizici, problemi ili greške koje su uočene
- **Presretanje izuzetaka u Debuggeru:** Prilikom testiranja brisanja već obrisanih stavki, Visual Studio debugger je prekidao izvršavanje hvatajući `KeyNotFoundException` prije nego što bi ga middleware obradio. Problem je riješen puštanjem aplikacije u rad čime je potvrđen ispravan `404 Not Found` odgovor na frontendu.

<<<<<<< HEAD
## Unos #25

| Polje | Detalji |
|---|---|
| **Datum** | 01.06.2026. |
| **Sprint broj** | Sprint 10 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Nejra Hodžić |

### Svrha korištenja

Pomoć pri planiranju, organizaciji i implementaciji backend i frontend testova za Sprint 10 funkcionalnosti.

### Kratak opis zadatka ili upita

Tražena je pomoć pri analizi implementiranih funkcionalnosti logističkih aktivnosti, tehničke opreme i upravljanja materijalima, kao i pri definisanju testnih scenarija za backend i frontend dio sistema. 

### Šta je AI predložio ili generisao

- Prijedloge backend test scenarija za:
  - logističke aktivnosti (S46)
  - tehničku opremu (S47)
  - upravljanje materijalima (S45.3 i S45.4)

- Prijedloge frontend test scenarija za:
  - LogisticsPage
  - EquipmentPage
  - SessionDetailsPage
  - MaterialUpdateDelete funkcionalnosti

### Šta je tim prihvatio

- Predložene backend i frontend test scenarije
- Organizaciju testnih klasa i testnih slučajeva

### Šta je tim izmijenio

- Testovi su prilagođeni postojećoj implementaciji projekta

### Šta je tim odbacio

- Izmjene produkcijskog koda radi lakšeg testiranja
- Refaktorisanje kontrolera, servisa i repozitorija koje nije bilo dio sprint zadataka
- Test scenarije koji nisu bili primjenjivi na postojeću implementaciju

### Rizici, problemi ili greške koje su uočene

- Funkcionalnosti izmjene i brisanja materijala nisu implementirane

---

## Unos #26

| Polje | Detalji |
|---|---|
| **Datum** | 01.06.2026. |
| **Sprint broj** | Sprint 10 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Ajra Kerla |

### Svrha korištenja

Pomoć pri analizi implementiranih funkcionalnosti.

### Kratak opis zadatka ili upita

AI alat je korišten za provjeru implementacije korisničkih priča iz Sprint 10 backlog-a, identifikaciju funkcionalnosti koje nisu implementirane, pripremu testnih scenarija.

### Šta je AI predložio ili generisao

- Analizu implementacije funkcionalnosti Sprinta 10
- Prijedloge backend i frontend test scenarijai

### Šta je tim prihvatio

- Analizu postojećeg stanja implementacije
- Predložene testne scenarije
- Strukturu sprint dokumentacije

### Šta je tim izmijenio

- Opisi funkcionalnosti su usklađeni sa Sprint 10 backlogom
- Testni slučajevi su prilagođeni postojećoj implementaciji sistema

### Šta je tim odbacio

- Predložene izmjene produkcijskog koda koje nisu bile dio sprint ciljeva
- Implementaciju dodatnih funkcionalnosti koje nisu planirane za Sprint 10

### Rizici, problemi ili greške koje su uočene

- Ownership validacije nisu bile prisutne za sve slučajeve korištenja
- Potrebne su dodatne validacije za pojedine tipove podataka i poslovna pravila
=======
>>>>>>> 955ac0957ae0345a6d233a3f54cb5a8249a8d4f9
