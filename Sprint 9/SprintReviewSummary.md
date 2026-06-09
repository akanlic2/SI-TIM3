# Sprint Review Summary

## Sprint broj: 9

## Planirani sprint goal:
Isporuka petog funkcionalnog inkrementa sistema (INC-05) — potpuno aktiviranje role predavaca kroz vlastiti dashboard i upload materijala, uvodjenje Q&A panela po sesiji kao interaktivnog kanala komunikacije izmedju ucesnika i predavaca, te implementacija in-app notifikacijskog sistema koji povezuje sve kljucne evente u sistemu. Sprint je ukljucivao i nastavak odrzavanja procesnih artefakata Decision Log i AI Usage Log, te SprintReview dokument za Sprint 8.

## Sta je zavrseno?
Sve planirane stavke su uspjesno zavrsene:

- S43 — Predavac dashboard: Implementiran dashboard predavaca sa pregledom svih sesija kojima je dodijeljen. Predavac dobija iste mogucnosti pregleda kao ucesnik — konferencije, sesije, agenda.
- S44 — Upload materijala: Predavac moze uploadati materijale (PDF, prezentacije) za sesije kojima je dodijeljen. Organizator i admin mogu uploadati za bilo koju sesiju. Materijali su vidljivi svim korisnicima prijavljenim na tu sesiju.
- S47 — Q&A panel — postavljanje pitanja: Svi prijavljeni korisnici mogu postavljati pitanja vezana za sesiju nakon pocetka sesije. Pitanja su vidljiva svim korisnicima Q&A panela i osvjezavaju se polling mehanizmom.
- S48 — Q&A panel — odgovaranje predavaca: Predavac dodijeljen sesiji moze odgovarati na pitanja korisnika. Odgovor je odmah vidljiv svim korisnicima panela.
- S49 — In-app notifikacije: Ucesnik dobija notifikaciju pri prijavi na konferenciju i pri promjeni ili otkazivanju sesije/konferencije. Predavac dobija notifikaciju kada je dodijeljen sesiji i kada dobije novo pitanje u Q&A. Autor pitanja dobija notifikaciju kada predavac odgovori. Notifikacije su prikazane u navigaciji sa brojacem neprocitanih.
- S16 — Azurirani Decision Log: Dokumentovane kljucne odluke donesene tokom sprinta.
- S17 — Azurirani AI Usage Log: Dokumentovano koristenje AI alata tokom implementacije.
- S18 — ProofOfTesting: Dokaz testiranja svih funkcionalnosti sa pokrivenoscu svih rola.
- S19 — SprintReviewSummary Sprint 8: Pregled isporucenog u Sprintu 8, ubacen u folder Sprint 8.

## Sta nije zavrseno?
Nema nezavrsenih stavki u okviru planiranog sprinta.

## Demonstrirane funkcionalnosti ili artefakti:
Funkcionalnosti:
- Predavac dashboard sa pregledom dodijeljenih sesija
- Upload materijala za sesiju (predavac, organizator, admin)
- Q&A panel — postavljanje pitanja nakon pocetka sesije
- Q&A panel — odgovaranje predavaca na pitanja korisnika
- In-app notifikacije za sve kljucne evente u sistemu

Artefakti:
- AIUsageLog
- DecisionLog
- SprintBacklog
- SprintGoal
- ProofOfTesting
- SprintReviewSummary Sprint 8

## Glavni problemi i blokeri:
Implementacija notifikacijskih okidaca na vec postojecim endpointima (prijava na konferenciju, dodjela predavaca sesiji) zahtijevala je pazljivo modificiranje gotovog koda iz prethodnih sprintova kako se ne bi uvela regresija. Sve izmjene su uspjesno integrisane bez narusavanja postojecih funkcionalnosti.

Za Q&A panel bilo je potrebno implementirati logiku koja blokira postavljanje pitanja prije pocetka sesije, sto je zahtijevalo provjeru startTime sesije i na backendu i na frontendu. Validacija je ispravno implementirana i testirana.

Svi problemi su uspjesno rijeseni tokom razvoja i nisu utjecali na isporuku planiranih funkcionalnosti.

## Kljucne odluke donesene u sprintu:
Za Q&A panel usvojen je polling pristup umjesto websocketa — Q&A panel se osvjezava svakih 10 sekundi, a notifikacije svakih 15 sekundi. Ova odluka je donijeta radi jednostavnosti implementacije uz zadrzavanje funkcionalno zadovoljavajuceg korisnickog iskustva.

Za upload materijala definisana je strategija pohrane fajlova i dozvoljeni tipovi (PDF, PPT, PPTX). Materijali su vidljivi iskljucivo korisnicima prijavljenim na tu sesiju.

Notifikacijski servis implementiran je kao interni servis koji prima tip notifikacije, korisnika i payload, te sprema notifikaciju u vec postojecu Notification tabelu iz domain modela.

## Povratna informacija Product Ownera:
Isporuka INC-05 je uspjesno demonstrirana. Aktiviranje role predavaca kroz vlastiti dashboard i upload materijala zaokruzuje korisnicko iskustvo predavaca u sistemu. Q&A panel uvodi interaktivnost izmedju ucesnika i predavaca, a notifikacijski sistem povezuje sve kljucne evente i osigurava da su korisnici uvijek informisani o dogadjajima koji se ticu njihove uloge.

## Zakljucak za naredni sprint:
INC-05 je uspjesno zavrsen — predavac dashboard, upload materijala, Q&A panel i in-app notifikacije su implementirani i funkcionalni.

Tim je spreman za naredni inkrement koji uvodi upravljanje logistickim aktivnostima konferencije, tehnickom opremom sa dodjelom sesijama, te izvjestaje za organizatore sa kljucnim statistikama konferencije. Nastavit ce se i odrzavanje Decision Log i AI Usage Log artefakata.