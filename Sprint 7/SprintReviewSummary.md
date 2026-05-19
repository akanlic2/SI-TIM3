# Sprint Review Summary

## Sprint broj: 7

## Planirani sprint goal:

Isporuka trećeg funkcionalnog inkrementa sistema (INC-03) — implementacija upravljanja sesijama i prijavama učesnika. Sprint je obuhvatio funkcionalnosti upravljanja sesijama konferencije, dodjelu predavača sesijama, prijavu i odjavu učesnika sa konferencije, kao i prijavu učesnika na sesije uz validaciju kapaciteta i vremenskih preklapanja. Sprint je uključivao i nastavak održavanja procesnih artefakata Decision Log i AI Usage Log.

## Šta je završeno?

Sve planirane stavke su uspješno završene:

- S32 — Upravljanje sesijama: Implementirana funkcionalnost upravljanja sesijama konferencije.
- S32.1 — Pregled sesija konferencije: Organizator može pregledati sve sesije unutar odabrane konferencije sa osnovnim informacijama.
- S32.2 — Kreiranje sesije: Omogućeno kreiranje novih sesija uz validaciju i sprječavanje duplikata termina.
- S32.3 — Uređivanje sesije: Organizator može uređivati postojeće sesije, a promjene su odmah vidljive.
- S32.4 — Brisanje sesije: Implementirano brisanje sesija uz potvrdu akcije.
- S33 — Dodjela predavača sesiji: Organizator može dodijeliti predavača sesiji, a predavač je vidljiv u prikazu sesije.
- S38 — Prijava učesnika na konferenciju: Učesnik se može prijaviti na konferenciju, uz evidenciju prijave i sprječavanje duplikata.
- S39 — Odjava učesnika sa konferencije: Implementirana mogućnost odjave učesnika sa konferencije.
- S40 — Prijava učesnika na sesiju: Omogućena prijava na sesije uz provjeru kapaciteta i preklapanja termina.
- S16 — Ažurirani Decision Log: Dokumentovane ključne odluke donesene tokom sprinta.
- S17 — Ažurirani AI Usage Log: Dokumentovano korištenje AI alata tokom implementacije.

## Šta nije završeno?

Nema nezavršenih stavki u okviru planiranog sprinta.

## Demonstrirane funkcionalnosti ili artefakti:

Funkcionalnosti:
- Pregled sesija konferencije
- Kreiranje sesije
- Uređivanje sesije
- Brisanje sesije
- Dodjela predavača sesiji
- Prijava učesnika na konferenciju
- Odjava učesnika sa konferencije
- Prijava učesnika na sesije

Artefakti:
- AIUsageLog
- DecisionLog
- SprintBacklog
- SprintGoal

## Glavni problemi i blokeri:

Najveći izazovi tokom sprinta odnosili su se na implementaciju validacija za prijavu učesnika na sesije. Bilo je potrebno osigurati provjeru kapaciteta sesije, sprječavanje duplih prijava i validaciju vremenskih preklapanja između sesija. Dodatno, implementacija dodjele predavača zahtijevala je usklađivanje prikaza podataka između sesija i korisničkih uloga.

Problemi su uspješno riješeni tokom razvoja i nisu uticali na isporuku planiranih funkcionalnosti.

## Ključne odluke donesene u sprintu:

Usvojen je model prijava pri kojem se učesnik prvo mora prijaviti na konferenciju prije prijave na pojedinačne sesije. Također je uvedena validacija kapaciteta i provjera vremenskih konflikata kako bi se spriječile neispravne prijave.

Za upravljanje sesijama usvojen je pristup u kojem organizator upravlja sesijama i dodjelom predavača, dok se prikaz sesija prilagođava ulozi korisnika.

## Povratna informacija Product Ownera:

Isporuka INC-03 je uspješno demonstrirana. Funkcionalnosti upravljanja sesijama i prijava učesnika predstavljaju važan korak ka potpunoj podršci procesu organizacije konferencija i omogućavaju nastavak razvoja naprednijih konferencijskih funkcionalnosti.

## Zaključak za naredni sprint:

INC-03 je uspješno završen — upravljanje sesijama, prijave i odjave učesnika, kao i prijave na sesije su implementirane i funkcionalne.

Tim je spreman za naredni inkrement koji će proširiti konferencijski sistem dodatnim funkcionalnostima vezanim za raspored, kapacitete, dvorane i naprednije upravljanje sadržajem konferencije. Također će se nastaviti održavanje Decision Log i AI Usage Log artefakata.