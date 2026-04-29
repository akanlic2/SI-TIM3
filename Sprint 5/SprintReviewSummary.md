# Sprint Review Summary

## Sprint broj: 5

## Planirani sprint goal:

Isporuka prvog funkcionalnog inkrementa sistema (INC-01) — implementacija autentifikacije i upravljanja korisnicima kroz integraciju s Keycloak identity providerom, kao temelja za sve ostale funkcionalnosti sistema. Sprint je uključivao i uspostavljanje ključnih procesnih artefakata: Decision Log i AI Usage Log.

## Šta je završeno?

Sve planirane stavke su uspješno završene:

- S16 — Decision Log: Procesni artefakt je uspostavljen i dokumentuje ključne projektne odluke, uključujući odluku o usvajanju Keycloaka kao identity providera.
- S17 — AI Usage Log: Procesni artefakt je uspostavljen i dokumentuje svako korištenje AI alata s opisom svrhe, prihvaćenih prijedloga i uočenih rizika, u skladu s Definition of Done.
- S21 — Sign up: Implementiran je registracijski tok putem Keycloaka s automatskom sinhronizacijom korisničkog profila u lokalnoj bazi podataka.
- S22 — Sign in: Implementirana je prijava korištenjem Keycloak PKCE Authorization Code Flow-a, uz sigurno čuvanje tokena i automatski refresh.
- S23 — Log out: Implementirana je sigurna odjava s poništavanjem lokalne sesije i redirekcijom na Keycloak logout endpoint.

## Šta nije završeno?

Nema nezavršenih stavki u okviru planiranog sprinta.

## Demonstrirane funkcionalnosti ili artefakti:

Funkcionalnosti: 
- Sign up
- Sign in
- Log out

Artefakti:
- AIUsageLog
- DecisionLog
- SprintBacklog
- SprintGoal

## Glavni problemi i blokeri:

Tokom sprinta nisu evidentirani značajni blokeri. Identifikovani rizici (konfiguracija Keycloak-a, sinhronizacija korisnika, sigurnost tokena, kontrola pristupa, brute-force zaštita) su uzeti u obzir tokom implementacije i nisu prerasli u probleme koji bi ugrozili isporuku.

## Ključne odluke donesene u sprintu:

Odabir Keycloaka kao identity providera (evidentirana u Decision Logu): Tim je donio odluku da se autentifikacija temelji na Keycloaku umjesto direktnog JWT pristupa, čime se postiže veća sigurnost, standardizacija toka autentifikacije (PKCE) i lakše upravljanje korisnicima i rolama.

## Povratna informacija Product Ownera:

Isporuka INC-01 u potpunosti je u skladu s očekivanjima. Demonstrirani autentifikacijski tok funkcioniše kako je planirano i predstavlja stabilan temelj za razvoj funkcionalnosti u narednim inkrementima.

## Zaključak za naredni sprint:

INC-01 je uspješno zatvoren — autentifikacija i upravljanje korisnicima su funkcionalni i testirani. Tim je spreman za razvoj INC-02, pri čemu su sve funkcionalnosti koje zahtijevaju prijavljenog korisnika sada odblokirane. U narednom sprintu potrebno je osigurati nastavak vođenja Decision Log-a i AI Usage Log-a u formatu uspostavljenom u ovom sprintu. 