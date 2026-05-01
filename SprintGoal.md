## Sprint cilj

Cilj ovog sprinta je isporuka drugog funkcionalnog inkrementa sistema — implementacija korisnickog profila, role-based access control mehanizma i pocetnog dashboarda kao ulazne tacke za sve role u sistemu, te kompletnog CRUD-a za konferencije. Ovaj inkrement (INC-02) direktno se nadovezuje na autentifikacijsku infrastrukturu uspostavljenu u Sprintu 5 i cini temelj za sve domenske funkcionalnosti koje slijede.

---

## Kljucne stavke koje tim zeli zavrsiti

- **Azurirani Decision Log (S16)** — Biljezenjem kljucnih odluka donesenih tokom ovog sprinta.
- **Azurirani AI Usage Log (S17)** — Dokumentovanje svakog koristenja AI alata s opisom svrhe, prihvacenih prijedloga i uocenih rizika, u skladu s Definition of Done.
- **Korisnicki profil (S24, S24.1, S24.2)** — Svaki prijavljeni korisnik moze pregledati i izmijeniti vlastite podatke. Admin ima pregled svih profila.
- **Dashboard po rolama (DASH-01 do DASH-05)** — Svaka rola dobija personalizovanu ulaznu tacku nakon Keycloak logina.
- **Kreiranje konferencije (S26)** — Organizator i admin mogu kreirati novu konferenciju.
- **Pregled konferencija (S27)** — Svi korisnici mogu pregledati listu konferencija po roli.
- **Detalji konferencije (S28)** — Svi korisnici mogu otvoriti detalje odabrane konferencije.
- **Uredjivanje konferencije (S29)** — Organizator moze izmijeniti podatke vlastite konferencije, admin bilo koje.
- **Brisanje konferencije (S30)** — Organizator moze obrisati vlastitu konferenciju uz potvrdu, admin bilo koju.
- **Pretraga konferencija (S31)** — Filtriranje konferencija po nazivu, datumu i lokaciji.
- **Dokument dokaza o testiranju** — Dokaz testiranja svih funkcionalnosti sprinta sa pokrivenoscu svih rola.
- **Sprint Retrospective Summary** — Pregled sta je proslo dobro, sta treba poboljsati i akcioni koraci za naredni sprint.

---

## Rizici

- **Migracija kao blocker** — Ako MIG-01 kasni, Read tim gubi dan ili vise. MIG-01 je prvi task prvog dana.
- **Keycloak role sinhronizacija** — Dashboard routing (DASH-05) ovisi o ispravno postavljenim rolama u Keycloaku iz Sprinta 5. Potrebno verificirati da role dolaze u JWT tokenu prije pocetka rada.
- **Nekonzistentna role logika** — Tri cjeline implementiraju role-based UI nezavisno. Potrebno dogovoriti zajednicki helper/hook na pocetku sprinta.
- **API kontrakt izmedju timova** — CUD tim pise u `conferences`, Read tim cita iz iste tabele. Potrebno dokumentovati request/response shape odmah nakon MIG-01.

---

## Zavisnosti

- **Sprint 5 (INC-01)** — Keycloak autentifikacija mora biti funkcionalna. Svi taskovi pretpostavljaju autentikovanog korisnika s rolom u JWT tokenu.
- **Tabela `users`** — Vec postoji iz Sprinta 5. MIG-01 dodaje samo tabelu `conferences`.
- **Keycloak realm** — Role `admin`, `organizator`, `predavac`, `ucesnik` moraju biti konfigurisane i dostupne kao claims.
- **INC-02 kao preduslov** — Preduslov za Sprint 7 koji uvodi upravljanje sesijama (S32+) i prijavu ucesnika na konferencije.
