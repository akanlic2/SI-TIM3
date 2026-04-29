# Sprint Goal — Sprint 5

## Sprint cilj

Cilj ovog sprinta je isporuka prvog funkcionalnog inkrementa sistema — implementacija autentifikacije i upravljanja korisnicima kroz integraciju s Keycloak identity providerom. Ovaj inkrement (INC-01) čini temelj za sve ostale funkcionalnosti sistema, jer su sve zavisne od autentikovanog korisnika s dodijeljenom rolom. Pored funkcionalnosti, sprint uključuje i uspostavljanje ključnih procesnih artefakata: Decision Log i AI Usage Log.

---

## Ključne stavke koje tim želi završiti

- **AI Usage Log (S17)** — Dokumentovanje svakog korištenja AI alata s opisom svrhe, prihvaćenih prijedloga i uočenih rizika, u skladu s Definition of Done.
- **Decision Log (S16)** — Bilježenje ključnih projektnih odluka (uključujući odluku o usvajanju Keycloaka kao identity providera umjesto direktnog JWT pristupa).
- **Sign up (S21)** — Omogućavanje korisnicima kreiranja naloga putem Keycloak registracijskog toka, s automatskom sinhronizacijom korisničkog profila u lokalnoj bazi podataka.
- **Sign in (S22)** — Implementacija prijave korištenjem Keycloak PKCE Authorization Code Flow-a, uz sigurno čuvanje tokena i automatski refresh.
- **Log out (S23)** — Implementacija sigurne odjave korisnika s poništavanjem lokalne sesije i redirekcijom na Keycloak logout endpoint.

---

## Rizici

- **Integracija s Keycloakom** — Pogrešna konfiguracija Keycloak realma, klijenta ili PKCE parametara može blokirati cijeli autentifikacijski tok.
- **Sinhronizacija korisnika** — Middleware za sinhronizaciju Keycloak korisnika s lokalnom bazom može uzrokovati duplikate ili greške pri prvoj prijavi.
- **Sigurnost tokena** — Neispravno rukovanje JWT tokenima (npr. čuvanje u localStorage umjesto httpOnly cookie-a) može izložiti aplikaciju XSS napadima.
- **Neovlašten pristup podacima (R5/R6)** — Bez ispravno postavljenih Keycloak rola i middleware-a, korisnici mogu dobiti pristup resursima koji im ne pripadaju.
- **Brute-force napadi (R35)** — Keycloak pruža zaštitu, ali je potrebno verificirati da su politike zaključavanja naloga ispravno konfigurisane.

---

## Zavisnosti

- **Sprint 4** — Uspostavljeni tehnički skeleton (React + TypeScript, ASP.NET Core, PostgreSQL, Docker Compose), GitHub Flow branching strategija i Definition of Done kao osnova za ocjenu završenosti.
- **Keycloak** — Lokalna i produkcijska Keycloak instanca mora biti pokrenuta i konfigurisana (realm `conference-app`, klijent, korisničke role) prije nego što Sign in / Sign out mogu biti testirani end-to-end.
- **INC-01 kao preduslov** — Ovaj sprint je preduslov za sve naredne inkremente: Sign up (S21) je preduslov za Sign in (S22), a Sign in je preduslov za sve funkcionalnosti koje zahtijevaju prijavljenog korisnika (INC-02 i dalje).
