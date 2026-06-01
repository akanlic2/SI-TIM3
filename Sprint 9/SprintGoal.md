# Sprint Goal — Sprint 9

## Sprint cilj

Cilj ovog sprinta je isporuka petog funkcionalnog inkrementa sistema — potpunog aktiviranja role predavaca kroz vlastiti dashboard i upload materijala, uvodenja Q&A panela po sesiji kao interaktivnog kanala komunikacije izmedju ucesnika i predavaca, te implementacije in-app notifikacijskog sistema koji povezuje sve kljucne evente u sistemu. Ovaj inkrement (INC-05) direktno se nadovezuje na upravljanje sesijama i registracije uspostavljene u Sprintu 7 i raspored konferencije iz Sprinta 8, i zaokruzuje osnovnu korisnicku iskustvo svih cetiri rola u sistemu.

---

## Kljucne stavke koje tim zeli zavrsiti

- **Predavac dashboard (S43)** — Predavac vidi sve sesije kojima je dodijeljen sa osnovnim podacima (naziv, termin, konferencija, dvorana). Predavac dobija iste mogucnosti pregleda kao ucesnik — konferencije, sesije, agenda.
- **Upload materijala (S44)** — Predavac moze uploadati materijale (PDF, prezentacije) za sesije kojima je dodijeljen. Organizator i admin mogu uploadati za bilo koju sesiju. Materijali su vidljivi svim korisnicima prijavljenim na tu sesiju.
- **Q&A panel — postavljanje pitanja (S47)** — Svi prijavljeni korisnici mogu postavljati pitanja vezana za sesiju, ali tek nakon pocetka sesije. Pitanja su vidljiva svim korisnicima Q&A panela i osvjezavaju se polling mehanizmom.
- **Q&A panel — odgovaranje predavaca (S48)** — Predavac dodijeljen sesiji moze odgovarati na pitanja korisnika. Odgovor je odmah vidljiv svim korisnicima panela.
- **In-app notifikacije (S49)** — Ucesnik dobija notifikaciju pri prijavi na konferenciju i pri promjeni ili otkazivanju sesije/konferencije. Predavac dobija notifikaciju kada je dodijeljen sesiji i kada dobije novo pitanje u Q&A. Autor pitanja dobija notifikaciju kada predavac odgovori. Notifikacije su prikazane u navigaciji sa brojacem neprocitanih.
- **Azurirani Decision Log i AI Usage Log** — Svi clanovi tima dokumentuju kljucne odluke i koristenje AI alata u skladu s Definition of Done.
- **Unit testovi i ProofOfTesting (DOC-01)** — Pokrivenost unit testovima na BE i FE za sve implementirane funkcionalnosti, sa svim rolama (admin, organizator, predavac, ucesnik).
- **SprintReviewSummary — Sprint 8 (DOC-02)** — Pregled isporucenog u Sprintu 8; ubaciti u folder Sprint 8.

---

## Rizici

- **Q&A blokada prije pocetka sesije** — Logika koja sprecava postavljanje pitanja prije startTime sesije mora biti implementirana i na backendu i na frontendu. Greska ovdje narusava kljucno poslovno pravilo.
- **Notifikacije na postojecim endpointima** — Okidaci za prijavu na konferenciju, promjenu sesije i dodjelu predavaca zahtijevaju modifikaciju vec zavrsenih endpointa iz Sprinta 7. Potrebna pazljivost da se ne uvede regresija.
- **Upload fajlova** — Rukovanje fajlovima (PDF, PPT, PPTX) zahtijeva definisanje strategije pohrane (lokalni filesystem ili cloud storage) na pocetku sprinta. Ova odluka mora biti donesena prvog dana.
- **Polling konzistentnost** — I Q&A panel i notifikacije koriste polling. Potrebno uskladiti intervale i izbjeci preopterecenje servera ako su oba aktivna istovremeno.
- **Tim C ceka implementaciju** — Tim C ne moze pisati relevantne testove dok Tim A, Tim B i Osoba E ne zavrse implementaciju. Tim C pocinje sa SprintReview dokumentom za Sprint 8 u prvim danima sprinta.

---

## Zavisnosti

- **Sprint 7 (INC-03)** — SessionRegistration tabela sa isSpeaker flagom mora biti funkcionalna. Predavac dashboard cita iz iste tabele.
- **Sprint 8 (INC-04)** — Agenda i prikaz sesija moraju biti funkcionalni. Predavac pristupa istim stranicama kao ucesnik.
- **Tabela `Notification`** — Vec definisana u domain modelu od pocetka projekta. Ovaj sprint je prvi koji je koristi; potrebno kreirati migraciju ako tabela jos nije u bazi.
- **Role u JWT tokenu** — Rola `predavac` mora biti ispravno postavljena i dostupna kao claim; S43, S44, S47 i S48 striktno je provjeravaju.
- **INC-05 kao preduslov** — Preduslov za Sprint 10 koji uvodi upravljanje materijalima konferencije i sesije, logisticke aktivnosti i opremu.
