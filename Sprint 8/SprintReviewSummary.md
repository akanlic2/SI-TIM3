# Sprint Review Summary

## Sprint broj: 8

## Planirani sprint goal:
Isporuka četvrtog funkcionalnog inkrementa sistema (INC-04) — implementacija upravljanja dvoranama sa stvarnom dodjelom dvorane sesijama (zamjena hardkodiranog seeda), pregleda rasporeda konferencije putem agende, te uvida organizatora u popunjenost kapaciteta i listu učesnika. Sprint je uključivao i nastavak održavanja procesnih artefakata Decision Log i AI Usage Log, te retroaktivne SprintReview dokumente za Sprint 6 i Sprint 7.

## Šta je završeno?
Sve planirane stavke su uspješno završene:

- S35 — Upravljanje dvoranama: Implementiran potpuni CRUD nad dvoranama za organizatora i admina, uz sprječavanje duplikata i zabranu brisanja dvorane dodijeljene aktivnoj sesiji.
- S36 — Dodjela dvorane sesiji: Hardkodirani seed zamijenjen stvarnim API-jem i UI-jem. Organizator dodjeljuje dvoranu sesiji putem dropdowna; sistem provjerava zauzetost termina i sprječava konflikt.
- S34 — Pregled rasporeda konferencije: Svi korisnici mogu vidjeti agendu konferencije sortiranu po terminu, sa nazivom, tipom stavke i dvoranom gdje je primjenljivo.
- S34.1 — Kreiranje AgendaItema: Organizator može kreirati stavke agende razlicitih tipova (sesija, pauza, ručak, networking, otvaranje, zatvaranje). Stavke tipa sesija referenciraju postojeći SessionId bez dupliciranja podataka.
- S34.2 — Pregled agende: Implementiran prikaz agende konferencije dostupan svim korisnicima.
- S34.3 — Uređivanje i brisanje AgendaItema: Organizator može mijenjati i brisati stavke agende; promjene su odmah vidljive.
- S41 — Pregled popunjenosti kapaciteta: Organizator i admin vide broj prijavljenih u odnosu na maksimalni kapacitet konferencije i sesije. Prikaz se ažurira nakon svake prijave ili odjave.
- S42 — Lista učesnika po konferenciji: Organizator i admin mogu pregledati listu prijavljenih učesnika sa pretragom i filtriranjem. Pristup je zaključan za predavače i učesnike.
- S16 — Ažurirani Decision Log: Dokumentovane ključne odluke donesene tokom sprinta.
- S17 — Ažurirani AI Usage Log: Dokumentovano korištenje AI alata tokom implementacije.
- S18 — ProofOfTesting: Dokaz testiranja svih funkcionalnosti sa pokrivenošću svih rola.
- S19 — SprintReviewSummary Sprint 6: Retroaktivni pregled isporučenog u Sprintu 6, ubačen u folder Sprint 6.
- S20 — SprintReviewSummary Sprint 7: Retroaktivni pregled isporučenog u Sprintu 7, ubačen u folder Sprint 7.

## Šta nije završeno?
Nema nezavršenih stavki u okviru planiranog sprinta.

## Demonstrirane funkcionalnosti ili artefakti:
Funkcionalnosti:
- Kreiranje, pregled, uređivanje i brisanje dvorana
- Dodjela dvorane sesiji putem stvarnog UI-ja (zamjena seeda)
- Kreiranje i upravljanje stavkama agende konferencije
- Pregled rasporeda konferencije za sve korisnike
- Pregled popunjenosti kapaciteta konferencije i sesije
- Lista učesnika po konferenciji sa pretragom i filtriranjem

Artefakti:
- AIUsageLog
- DecisionLog
- SprintBacklog
- SprintGoal
- ProofOfTesting
- SprintReviewSummary Sprint 6
- SprintReviewSummary Sprint 7

## Glavni problemi i blokeri:
Najveći izazov tokom sprinta bio je usklađivanje datuma i termina kod agende, na što je asistent ukazao tokom pregleda. Problem se ogledao u neusklađenosti između termina sesija i termina agenda stavki koje ih referenciraju, što je moglo dovesti do netačnog prikaza rasporeda. Problem je uspješno identificiran i ispravljen tokom sprinta — agenda stavke tipa sesija sada korektno preuzimaju i prikazuju termine iz referencirane sesije.

Dodjela dvorane sesiji zahtijevala je pažljivu backend validaciju zauzetosti termina kako bi se spriječilo da ista dvorana bude dodijeljena dvjema sesijama u preklapajućim terminima. Validacija je implementirana i testirana.

Svi problemi su uspješno riješeni tokom razvoja i nisu utjecali na isporuku planiranih funkcionalnosti.

## Ključne odluke donesene u sprintu:
Agenda stavke tipa sesija referenciraju SessionId bez dupliciranja podataka — naziv, termin i dvorana se preuzimaju direktno iz referencirane sesije, čime se osigurava konzistentnost rasporeda.

Za validaciju dvorane usvojena je provjera na backendu koja uzima u obzir startTime i endTime sesije, a ne samo datum, kako bi se ispravno detektovala preklapanja termina.

Lista učesnika je dostupna isključivo organizatoru vlastite konferencije i adminu, dok predavači i učesnici nemaju pristup ovim podacima.

## Povratna informacija Product Ownera:
Isporuka INC-04 je uspješno demonstrirana. Upravljanje dvoranama i dodjela dvorane sesijama zaokružuju prostornu komponentu organizacije konferencije. Uvođenje agende omogućava svim korisnicima pregledan uvid u raspored, a kapacitet i lista učesnika daju organizatoru potrebne alate za praćenje i upravljanje konferencijom.

## Zaključak za naredni sprint:
INC-04 je uspješno završen — upravljanje dvoranama, dodjela dvorane sesijama, agenda, kapacitet i lista učesnika su implementirani i funkcionalni.

Tim je spreman za naredni inkrement koji će aktivirati rolu predavača kroz vlastiti dashboard i upload materijala, uvesti Q&A panel po sesiji te implementirati in-app notifikacijski sistem koji povezuje sve ključne evente u sistemu. Nastavit će se i održavanje Decision Log i AI Usage Log artefakata.
