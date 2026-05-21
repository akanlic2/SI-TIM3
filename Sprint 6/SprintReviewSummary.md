# Sprint Review Summary

## Sprint broj: 6

## Planirani sprint goal:

Isporuka drugog funkcionalnog inkrementa sistema (INC-02) — implementacija upravljanja konferencijama i korisničkim profilima. Sprint je obuhvatio funkcionalnosti pregleda i izmjene korisničkog profila, kao i kompletan tok upravljanja konferencijama: kreiranje, pregled, detalje, uređivanje, brisanje i pretragu konferencija. Sprint je uključivao i nastavak vođenja procesnih artefakata Decision Log i AI Usage Log.

## Šta je završeno?

Sve planirane stavke su uspješno završene:

- S24 — Upravljanje korisničkim profilom: Implementirana funkcionalnost pregleda i izmjene korisničkog profila uz prilagođavanje prikaza prema roli korisnika.
- S24.1 — Pregled korisničkog profila: Svaki korisnik može pregledati vlastiti profil sa relevantnim podacima.
- S24.2 — Izmjena korisničkog profila: Omogućena izmjena podataka profila i lozinke uz role-based prilagođavanje korisničkog interfejsa.
- S26 — Kreiranje konferencije: Organizator i administrator mogu kreirati novu konferenciju unosom obaveznih podataka.
- S27 — Pregled konferencija: Implementiran prikaz liste svih konferencija dostupnih korisnicima.
- S28 — Detalji konferencije: Implementiran prikaz detalja konferencije uz prilagođavanje prikaza prema roli korisnika.
- S29 — Uređivanje konferencije: Organizator može uređivati vlastite konferencije, dok administrator može uređivati sve konferencije.
- S30 — Brisanje konferencije: Omogućeno brisanje konferencije uz potvrdu akcije i role-based pristup.
- S31 — Pretraga konferencija: Implementirano filtriranje konferencija po nazivu, datumu i lokaciji.
- S16 — Ažurirani Decision Log: Dokumentovane ključne odluke donesene tokom sprinta.
- S17 — Ažurirani AI Usage Log: Dokumentovano korištenje AI alata tokom implementacije.

## Šta nije završeno?

Nema nezavršenih stavki u okviru planiranog sprinta.

## Demonstrirane funkcionalnosti ili artefakti:

Funkcionalnosti:
- Pregled korisničkog profila
- Izmjena korisničkog profila
- Kreiranje konferencije
- Pregled konferencija
- Detalji konferencije
- Uređivanje konferencije
- Brisanje konferencije
- Pretraga konferencija

Artefakti:
- AIUsageLog
- DecisionLog
- SprintBacklog
- SprintGoal

## Glavni problemi i blokeri:

Tokom sprinta nisu evidentirani značajni blokeri koji bi ugrozili isporuku funkcionalnosti. Najveći izazovi odnosili su se na implementaciju role-based pristupa za upravljanje konferencijama i prilagođavanje korisničkog interfejsa različitim rolama korisnika, ali su uspješno riješeni tokom implementacije.

## Ključne odluke donesene u sprintu:

Usvojen je role-based model upravljanja konferencijama pri kojem organizator ima pristup samo vlastitim konferencijama, dok administrator ima pristup svim konferencijama i korisničkim profilima. Ovakav pristup omogućava jasnu kontrolu pristupa i priprema sistem za naredne funkcionalnosti povezane sa konferencijama i sesijama.

## Povratna informacija Product Ownera:

Isporuka INC-02 je u skladu sa očekivanjima. Demonstrirane funkcionalnosti upravljanja konferencijama i korisničkim profilima predstavljaju stabilnu osnovu za naredne funkcionalnosti sistema i omogućavaju nastavak razvoja konferencijskih procesa.

## Zaključak za naredni sprint:

INC-02 je uspješno završen — upravljanje korisničkim profilima i konferencijama je funkcionalno i spremno za proširenje. Tim je spreman za razvoj narednog inkrementa, pri čemu će fokus biti na funkcionalnostima vezanim za sesije, prijave i upravljanje sadržajem konferencija. Također je potrebno nastaviti održavanje Decision Log i AI Usage Log artefakata u uspostavljenom formatu.