# Sprint Review Summary

## Sprint broj: 10

## Planirani sprint goal:
Isporuka sestog funkcionalnog inkrementa sistema (INC-06) — upravljanja logistickim aktivnostima konferencije, upravljanja tehnickom opremom sa dodjelom sesijama, te izvjestaja za organizatore sa kljucnim statistikama konferencije. Sprint je ukljucivao i nastavak odrzavanja procesnih artefakata Decision Log i AI Usage Log, te SprintReview dokument za Sprint 9.

## Sta je zavrseno?
Sve planirane stavke su uspjesno zavrsene:

- S46 — Logisticke aktivnosti: Implementiran potpuni CRUD nad logistickim aktivnostima konferencije (catering, video snimanje, registracija ucesnika i sl.). Aktivnosti se filtriraju po tipu. Dostupno iskljucivo organizatoru vlastite konferencije i adminu.
- S47 — Tehnicka oprema: Implementirano kreiranje i brisanje tehnicke opreme u globalnom inventaru te dodjela opreme sesijama. Sistem provjerava dostupnu kolicinu prije dodjele. Organizator moze dodijeliti opremu samo sesijama vlastite konferencije; predavac vidi opremu dodijeljenu sesijama kojima je dodijeljen.
- S49 — Izvjestaji za organizatore: Organizator moze pregledati statistiku konferencije (broj prijavljenih, popunjenost kapaciteta po sesijama, broj predavaca, pregled materijala) i preuzeti izvjestaj u PDF formatu.
- S16 — Azurirani Decision Log: Dokumentovane kljucne odluke donesene tokom sprinta.
- S17 — Azurirani AI Usage Log: Dokumentovano koristenje AI alata tokom implementacije.
- S18 — ProofOfTesting: Dokaz testiranja svih funkcionalnosti sa pokrivenoscu svih rola.
- S19 — SprintReviewSummary Sprint 9: Pregled isporucenog u Sprintu 9, ubacen u folder Sprint 9.

## Sta nije zavrseno?
Nema nezavrsenih stavki u okviru planiranog sprinta.

## Demonstrirane funkcionalnosti ili artefakti:
Funkcionalnosti:
- Kreiranje, pregled, uredjivanje i brisanje logistickih aktivnosti konferencije sa filtriranjem po tipu
- Kreiranje i brisanje tehnicke opreme u globalnom inventaru
- Dodjela tehnicke opreme sesijama sa provjerom dostupne kolicine
- Izvjestaj o konferenciji sa kljucnim statistikama i preuzimanjem u PDF formatu

Artefakti:
- AIUsageLog
- DecisionLog
- SprintBacklog
- SprintGoal
- ProofOfTesting
- SprintReviewSummary Sprint 9

## Glavni problemi i blokeri:
Sprint 10 je protekao bez vecih tehnickih ili organizacijskih problema. Svi timovi su radili paralelno od prvog dana bez medjusobnog blokiranja. Odluka o PDF biblioteci za generisanje izvjestaja donesena je prvog dana sprinta sto je sprijecilo potencijalni bloker za Osobu E.

## Kljucne odluke donesene u sprintu:
Oprema je vezana za sesiju (FK SessionId), a ne za konferenciju, sto je konzistentno sa domain modelom i poslovno logicnije — tehnicka oprema se priprema za konkretnu sesiju, ne za cijelu konferenciju.

Logisticke aktivnosti su dostupne iskljucivo organizatoru i adminu kao interni organizatorski alat — ucesnici i predavaci nemaju pristup ovim podacima.

Za generisanje PDF izvjestaja odabrana je odgovarajuca backend biblioteka koja omogucava formatiran prikaz statistike pogodan za stampu ili dijeljenje.

## Povratna informacija Product Ownera:
Isporuka INC-06 je uspjesno demonstrirana i dobila pozitivne komentare. Upravljanje logistickim aktivnostima i tehnickom opremom zaokruzuju organizatorski set alata, a izvjestaji daju organizatoru potpun uvid u kljucne metrike konferencije. Sistem je sada funkcionalno zreo za pracenje cijelog zivotnog ciklusa konferencije od planiranja do realizacije.

## Zakljucak za naredni sprint:
INC-06 je uspjesno zavrsen — logisticke aktivnosti, tehnicka oprema i izvjestaji su implementirani i funkcionalni.

Tim je spreman za naredni inkrement koji ce uvesti preostale funkcionalnosti sistema. Nastavit ce se i odrzavanje Decision Log i AI Usage Log artefakata.
