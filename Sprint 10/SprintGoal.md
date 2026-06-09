<<<<<<< HEAD
# Sprint Goal — Sprint 10
=======
# Sprint Goal — Sprint 10

## Sprint cilj

Cilj ovog sprinta je isporuka sestog funkcionalnog inkrementa sistema — upravljanja logistickim aktivnostima konferencije, upravljanja tehnickom opremom sa dodjelom sesijama, te izvjestaja za organizatore sa kljucnim statistikama konferencije. Ovaj inkrement (INC-06) direktno se nadovezuje na upravljanje sesijama i dvoranama uspostavljene u prethodnim sprintovima i zaokruzuje organizatorski set alata potrebnih za potpunu pripremu i pracenje konferencije.

---

## Kljucne stavke koje tim zeli zavrsiti

- **Logisticke aktivnosti (S46.1–S46.4)** — Organizator moze kreirati, pregledati, izmijeniti i obrisati logisticke aktivnosti konferencije (catering, video snimanje, registracija ucesnika na ulazu i sl.). Aktivnosti se filtriraju po tipu. Ovo je interni organizatorski alat — ucesnici i predavaci nemaju pristup.
- **Tehnicka oprema (S47.1–S47.4)** — Organizator moze kreirati i brisati tehnicku opremu u globalnom inventaru te je dodijeliti sesijama vlastite konferencije. Sistem provjerava dostupnu kolicinu prije dodjele. Predavac vidi opremu dodijeljenu sesijama kojima je dodijeljen.
- **Izvjestaji za organizatore (S49)** — Organizator moze pregledati statistiku konferencije (broj prijavljenih, popunjenost kapaciteta po sesijama, broj predavaca, pregled materijala) i preuzeti izvjestaj u PDF formatu.
- **Azurirani Decision Log i AI Usage Log** — Svi clanovi tima dokumentuju kljucne odluke i koristenje AI alata u skladu s Definition of Done.
- **Unit testovi i ProofOfTesting (S18)** — Pokrivenost unit testovima na BE i FE za sve implementirane funkcionalnosti, sa svim rolama (admin, organizator, predavac, ucesnik).
- **SprintReviewSummary — Sprint 9 (S19)** — Pregled isporucenog u Sprintu 9; ubaciti u folder Sprint 9.

---

## Rizici

- **Generisanje PDF izvjestaja** — PDF generisanje na backendu zahtijeva odabir i integraciju odgovarajuce biblioteke (npr. PDFKit, Puppeteer). Ova odluka mora biti donesena prvog dana kako ne bi blokirala ostatak implementacije S49.
- **Provjera dostupne kolicine opreme** — Logika koja sprecava dodjelu vise opreme nego sto je dostupno mora biti implementirana na backendu, a ne samo na frontendu. Greska ovdje moze dovesti do nekonzistentnih podataka.
- **Konkurentno uredjivanje logistickih aktivnosti** — User story S46.3 eksplicitno navodi da sistem mora sprijeciti konflikte pri istovremenom uredjivanju od strane vise organizatora. Potrebno implementirati optimistic locking ili timestamp provjeru.
- **Tim C ceka implementaciju** — Tim C ne moze pisati relevantne testove dok Tim A, Tim B i Osoba E ne zavrse implementaciju. Tim C pocinje sa SprintReview dokumentom za Sprint 9 u prvim danima sprinta.

---

## Zavisnosti

- **Sprint 7 (INC-03)** — Session tabela mora biti funkcionalna. Equipment se dodjeljuje postojecim sesijama.
- **Sprint 6 (INC-02)** — Conference tabela mora biti funkcionalna. LogisticsTask i izvjestaji su vezani za konferenciju.
- **Sprint 8 (INC-04) i Sprint 9 (INC-05)** — Podaci o prijavama, kapacitetima, materijalima i predavacima moraju biti dostupni u bazi kako bi izvjestaj mogao biti generisan.
- **ERD** — Tim A i Tim B trebaju pregledati ERD na pocetku sprinta kako bi razumjeli strukturu LogisticsTask i Equipment tabela i njihove FK veze.
- **INC-06 kao preduslov** — Preduslov za Sprint 11 koji ce uvesti preostale funkcionalnosti sistema.
>>>>>>> 955ac0957ae0345a6d233a3f54cb5a8249a8d4f9
