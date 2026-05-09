# Sprint Goal — Sprint 7

## Sprint cilj

Cilj ovog sprinta je isporuka treceg funkcionalnog inkrementa sistema — kompletnog domain modela s migracijama, upravljanja sesijama konferencije (CRUD i dodjela predavaca) te prijave i odjave ucesnika na konferencije i sesije. Ovaj inkrement (INC-03) direktno se nadovezuje na CRUD konferencija uspostavljen u Sprintu 6 i postavlja temelj za sve napredne domenske funkcionalnosti koje slijede — raspored, materijale, notifikacije i Q&A.

---

## Kljucne stavke koje tim zeli zavrsiti

- **Migracije (MIG-01, MIG-02, MIG-03)** — Kreiranje tabela `sessions`, `registrations` i svih preostalih tabela domain modela. Ovo je blocker za sve ostale timove i realizuje se prvog dana sprinta.
- **Dokumentacija domain modela (DOM-01)** — Kompletna dokumentacija ERD dijagrama, veza izmedju entiteta i objasnjenja svakog atributa.
- **Sprint Goal i Sprint Backlog (PLAN-01, PLAN-02)** — Planiranje i dokumentovanje sprinta.
- **CRUD sesija (S32, S32.1–S32.4)** — Organizator i admin mogu kreirati, pregledati, urediti i obrisati sesije unutar vlastite konferencije. Predavac i ucesnik imaju read-only pristup.
- **Dodjela predavaca sesiji (S33)** — Organizator i admin mogu dodijeliti predavaca sesiji putem `is_speaker` flaga. Predavac vidi sesije kojima je dodijeljen.
- **Prijava ucesnika na konferenciju (S38)** — Ucesnik se moze prijaviti na konferenciju uz provjeru duplikata i evidenciju prijave.
- **Odjava ucesnika s konferencije (S39)** — Ucesnik se moze odjaviti s konferencije uz provjeru roka i azuriranje slobodnih mjesta.
- **Prijava ucesnika na sesiju (S40)** — Ucesnik se moze prijaviti na sesiju uz provjeru slobodnih mjesta i preklapanja termina.
- **Azurirani Decision Log i AI Usage Log** — Svi clanovi tima dokumentuju kljucne odluke i koristenje AI alata u skladu s Definition of Done.
- **Testovi i dokaz testiranja (TEST-BE-01, TEST-BE-02, TEST-FE-01, TEST-FE-02, DOC-01)** — Pokrivenost testovima za sve implementirane funkcionalnosti, sa svim rolama.
- **Sprint Review Summary (DOC-02)** — Pregled isporucenog za Sprint 6, ubaciti u folder Sprint 6.
- **Sprint Retrospective Summary (DOC-03)** — Pregled sta je proslo dobro, sta treba poboljsati i akcioni koraci za Sprint 8.

---

## Rizici

- **Migracije kao blocker** — MIG-01 i MIG-02 su preduslov za Tim A i Tarika. Moraju biti zavrsene prvog dana sprinta. Svako kasnjenje ovdje blokira cijeli tim za dan ili vise.
- **Provjera preklapanja termina sesija** — Poslovno pravilo da se dvije sesije ne mogu odrzavati u istoj dvorani u isto vrijeme zahtijeva pazljivu validaciju na backendu (S32.2, S40).
- **Role-based guard konzistentnost** — Tim A i Tarik implementiraju permisije nezavisno. Potrebno uskladiti zajednicki pristup (guard/middleware) na pocetku Faze 2 kako bi logika bila konzistentna.
- **API kontrakt izmedju timova** — Tim A pise `sessions`, Tarik cita istu tabelu za prijave. Potrebno dokumentovati request/response shape odmah nakon MIG-01.
- **Kapacitet dvorane i sesije** — Provjera da broj prijavljenih ne premasuje kapacitet dvorane mora biti implementirana na backendu (S40), a ne samo na frontendu.

---

## Zavisnosti

- **Sprint 6 (INC-02)** — CRUD konferencija mora biti funkcionalan. Sve sesije vise na postojecim konferencijama.
- **Tabela `conferences`** — Vec postoji iz Sprinta 6. MIG-01 dodaje tabelu `sessions` s FK na `conferences`.
- **Tabela `users`** — Vec postoji iz Sprinta 5. MIG-02 dodaje `registrations` s FK na `users`, `conferences` i `sessions`.
- **Role u JWT tokenu** — Role `admin`, `organizator`, `predavac`, `ucesnik` moraju biti ispravno postavljene i dostupne kao claims u JWT tokenu.
- **INC-03 kao preduslov** — Preduslov za Sprint 8 koji uvodi upravljanje materijalima, notifikacijama, opremom i Q&A modulom.
