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
- Copilot je kreirao duplikat IConferenceRepository interfejsa u pogrešnom sloju — ručno obrisano

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


---

## Unos #11

| Polje | Detalji |
| --- | --- |
| **Datum** | 09.05.2026. |
| **Sprint broj** | Sprint 6 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Ajra Kerla |

### Svrha korištenja
Pomoć pri implementaciji backend read operacija za konferencije, uključujući paginaciju, filtriranje i role-based pristup podacima.

### Kratak opis zadatka ili upita
ChatGPT je korišten kao podrška pri implementaciji backend funkcionalnosti za pregled konferencija (`GET /Conference`), detalje konferencije (`GET /Conference/{id}`), paginaciju i filtriranje konferencija po nazivu, lokaciji i kategoriji, kao i za role-based prikaz aktivnih, draft i neaktivnih konferencija.

### Šta je AI predložio ili generisao
- Implementaciju `ConferenceQueryDto` klase za query parametre
- Repository metodu `GetPagedFilteredAsync`
- Logiku za paginaciju koristeći `Skip()` i `Take()`
- Role-based filtering logiku za admina, organizatora i učesnika
- Primjere query parametara za search/filter funkcionalnost
- Refaktorisanje `ConferenceService` klase
- Primjere backend testova za read operacije i role filtering

### Šta je tim prihvatio
- Strukturu paginacije i filtriranja
- Repository i service layer implementaciju
- Role-based prikaz konferencija
- Predložene backend unit testove

### Šta je tim izmijenio
- Nazivi role-a prilagođeni postojećem auth sistemu (`admin-sistema`, `organizator`, `ucesnik`)
- Query parametri i DTO klase prilagođeni postojećoj strukturi projekta
- Dio logike za prikaz edit/delete opcija prilagođen frontend zahtjevima

### Šta je tim odbacio
- Dio predložene logike za sakrivanje edit/delete opcija adminu, jer je odlučeno da admin ipak treba imati mogućnost uređivanja i brisanja konferencija

### Rizici, problemi ili greške koje su uočene
- Tokom implementacije pojavili su se problemi sa paginacijom i filtriranjem samo trenutne stranice podataka
- Role-based filtering inicijalno nije pravilno prikazivao draft i inactive konferencije admin korisnicima
- Potrebno ručno provjeravanje JWT role claim-ova i testiranje kroz DBeaver i Swagger prije finalnog merge-a

---

## Unos #12

| Polje | Detalji |
| --- | --- |
| **Datum** | 09.05.2026. |
| **Sprint broj** | Sprint 6 |
| **Alat** | ChatGPT |
| **Ko je koristio alat** | Nejra Hodžić |

### Svrha korištenja
Pomoć pri implementaciji frontend prikaza konferencija, search/filter funkcionalnosti, paginacije i testiranja aplikacije.

### Kratak opis zadatka ili upita
ChatGPT je korišten za pomoć pri implementaciji React + TypeScript frontend funkcionalnosti za prikaz konferencijskih kartica, paginaciju, pretragu i filtriranje konferencija.

### Šta je AI predložio ili generisao
- Implementaciju `useConferences` hook-a
- Axios API funkcije za paginirani dohvat konferencija
- Frontend logiku za search/filter i pagination
- Role-based prikaz UI elemenata (`Prijavi se`, `Uredi`, `Obriši`)
- Frontend i backend testove za read operacije

### Šta je tim prihvatio
- Hook i API strukturu za konferencije
- Frontend pagination i filter logiku
- Testove za frontend i backend read operacije


### Šta je tim izmijenio
- Pagination prilagođen da prikazuje 6 konferencija po stranici
- Frontend role-based UI prilagođen projektnim zahtjevima
- TypeScript tipovi prilagođeni postojećoj strukturi DTO objekata

### Šta je tim odbacio
- Dio AI prijedloga za dodatne biblioteke za state management i routing jer je projekat koristio postojeću lokalnu implementaciju routinga

### Rizici, problemi ili greške koje su uočene
- Pojavljivali su se TypeScript problemi zbog neusklađenosti tipova između backend DTO objekata i frontend modela
- Frontend inicijalno nije prikazivao konferencije zbog pogrešnog parsiranja paginiranog odgovora

---
  
*Dokument se ažurira tokom trajanja projekta. Svaki novi slučaj korištenja AI dodaje se kao novi unos.*
