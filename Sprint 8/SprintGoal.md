# Sprint Goal — Sprint 8

## Sprint cilj

Cilj ovog sprinta je isporuka cetvrtog funkcionalnog inkrementa sistema — upravljanja dvoranama sa stvarnom dodjelom dvorane sesijama (zamjena hardkodiranog seeda), pregleda rasporeda konferencije putem agende, te uvida organizatora u popunjenost kapaciteta i listu ucesnika. Ovaj inkrement (INC-04) direktno se nadovezuje na CRUD sesija i registracije ucesnika uspostavljene u Sprintu 7 i postavlja temelj za napredne funkcionalnosti koje slijede — materijale, notifikacije, opremu i Q&A modul.

---

## Kljucne stavke koje tim zeli zavrsiti

- **CRUD dvorana (S35, S35.1–S35.4)** — Organizator i admin mogu kreirati, pregledati, izmijeniti i obrisati dvorane. Sistem sprecava duplikate i ne dozvoljava brisanje dvorane koja je dodijeljena aktivnoj sesiji.
- **Dodjela dvorane sesiji (S36)** — Hardkodirani seed se zamjenjuje stvarnim UI-jem i API-jem. Organizator dodjeljuje dvoranu sesiji putem dropdowna; sistem provjerava zauzetost termina i sprecava konflikt.
- **Kreiranje i upravljanje AgendaItemima (S34.1, S34.3)** — Organizator kreira stavke agende razlicitih tipova (sesija, pauza, rucak, networking, otvaranje, zatvaranje). Stavke tipa sesija referenciraju postojeci SessionId bez dupliciranja podataka.
- **Pregled rasporeda konferencije (S34, S34.2)** — Svi korisnici vide agendu konferencije sortiranu po terminu, sa nazivom, tipom i dvoranom gdje je primjenljivo.
- **Pregled popunjenosti kapaciteta (S41)** — Organizator i admin vide broj prijavljenih u odnosu na maksimalni kapacitet konferencije i sesije. Prikaz se azurira nakon svake prijave ili odjave.
- **Lista ucesnika po konferenciji (S42)** — Organizator i admin vide listu prijavljenih ucesnika sa pretragom i filtriranjem. Pristup je zakljucan za predavace i ucesnike.
- **Azurirani Decision Log i AI Usage Log** — Svi clanovi tima dokumentuju kljucne odluke i koristenje AI alata u skladu s Definition of Done.
- **Unit testovi i ProofOfTesting (S18)** — Pokrivenost unit testovima na BE i FE za sve implementirane funkcionalnosti, sa svim rolama (admin, organizator, predavac, ucesnik).
- **SprintReviewSummary — Sprint 6 (S19)** — Retroaktivni pregled isporucenog u Sprintu 6; ubaciti u folder Sprint 6.
- **SprintReviewSummary — Sprint 7 (S20)** — Retroaktivni pregled isporucenog u Sprintu 7; ubaciti u folder Sprint 7.

---

## Rizici

- **Interni blocker unutar Tim A** — S36 (dodjela dvorane) zavisi od S35 endpointa. Tim A mora prioritizovati GET i POST /rooms do kraja Dana 2 kako Tim B ne bi bio blokiran pri prikazu dvorane u agendi.
- **Agenda frontend zavisi od dvorana** — Tim B moze poceti bekend odmah, ali frontend koji prikazuje dvoranu u stavci agende ceka /rooms endpoint od Tim A. Dogovoriti API kontrakt prvog dana.
- **Validacija konflikta termina dvorane** — Poslovno pravilo da se ista dvorana ne moze dodijeliti dvjema sesijama u istom terminu zahtijeva pazljivu backend validaciju (S36). Greska ovdje moze poremetiti citav raspored konferencije.
- **Tim C ceka implementaciju** — Tim C ne moze pisati relevantne testove dok Tim A, Tim B i Osoba E ne zavrse implementaciju. Tim C pocinje sa SprintReview dokumentima (S19, S20) u prvim danima sprinta.
- **Role-based guard konzistentnost** — Tim A, Tim B i Osoba E implementiraju permisije nezavisno. Potrebno uskladiti zajednicki pristup (guard/middleware) na pocetku sprinta.

---

## Zavisnosti

- **Sprint 7 (INC-03)** — CRUD sesija i registracije ucesnika moraju biti funkcionalni. Dvorane se dodjeljuju postojecim sesijama, a agenda referencira iste.
- **Tabela `sessions`** — Vec postoji iz Sprinta 7. S36 dodaje stvarnu FK vezu rooms → sessions umjesto seeda.
- **Tabela `registrations`** — Vec postoji iz Sprinta 7. S41 i S42 citaju iste podatke za prikaz kapaciteta i liste ucesnika.
- **Role u JWT tokenu** — Role `admin`, `organizator`, `predavac`, `ucesnik` moraju biti ispravno postavljene; S41, S42 i S35 striktno ih provjeravaju.
- **INC-04 kao preduslov** — Preduslov za Sprint 9 koji uvodi materijale, notifikacije, opremu i Q&A modul.
