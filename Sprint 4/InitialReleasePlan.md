### Initial Release Plan

## INC-01: Autentifikacija i upravljanje korisnicima

## Cilj inkrementa:

Omogućiti osnovni pristup sistemu kroz registraciju, prijavu i upravljanje korisničkim profilom. Bez ovog inkrementa nijedna druga funkcionalnost 
sistema nije upotrebljiva, jer su sve zavisne od autentikovanog korisnika s dodijeljenom rolom.

## Glavne funkcionalnosti:
 - US-01 - Sign Up: kreiranje korisničkog naloga (ime, prezime, email, lozinka)
 - US-02 - Sign In: prijava registrovanog korisnika na sistem
 - US-03 - Log Out: sigurna odjava i poništavanje sesije
 - US-04 / US-04.1 / US-04.2 - Upravljanje korisničkim profilom: pregled i izmjena podataka
 - US-05 - Korisničke role i permisije: dodjeljivanje rola (učesnik, organizator, administrator) i kontrola pristupa

## Zavisnosti:
 - Ovaj inkrement je preduslov za sve ostale.
 - US-01 (Sign Up) je preduslov za US-02, US-04 i US-05.
 - US-02 (Sign In) je preduslov za sve funkcionalnosti koje zahtijevaju prijavljenog korisnika.
 - US-05 (Role i permisije) je preduslov za US-06 (Kreiranje konferencije).

## Glavni rizici:


## Okvirni sprintovi:
 - Sprint 5: US-01 (Sign Up), US-02 (Sign In), US-03 (Log Out)
 - Sprint 6: US-04 (Profil - pregled i izmjena), US-05 (Role i permisije)


## INC-02: Osnovno upravljanje konferencijama

## Cilj inkrementa:

Omogućiti organizatorima kreiranje i upravljanje konferencijama, a korisnicima otkrivanje i pregled dostupnih konferencija. Ovo je centralna funkcionalnost 
sistema bez koje isti nema poslovnu svrhu.

## Glavne funkcionalnosti:
 - US-06 - Kreiranje konferencije: unos naziva, datuma, lokacije i opisa
 - US-07 - Pregled konferencija: lista svih dostupnih konferencija
 - US-08 - Detalji konferencije: prikaz kompletnih informacija o odabranoj konferenciji
 - US-09 - Uređivanje konferencije: izmjena podataka postojeće konferencije
 - US-10 - Brisanje konferencije: uklanjanje konferencije uz potvrdu
 - US-11 - Pretraga konferencija: filtriranje i pretraga po ključnim riječima

## Zavisnosti:
 - Zavisi od INC-01 - korisnik mora biti prijavljen i imati odgovarajuću rolu (organizator).
 - US-06 je preduslov za US-07, US-08, US-09, US-10, US-11.
 - US-07 je preduslov za US-08 (detalji konferencije).

## Glavni rizici:


## Okvirni sprintovi:
 - Sprint 6: US-06 (Kreiranje), US-07 (Pregled), US-08 (Detalji)
 - Sprint 7: US-09 (Uređivanje), US-10 (Brisanje), US-11 (Pretraga)


## INC-03: Upravljanje sesijama i rasporedom  

## Cilj inkrementa:

Proširiti konferencije strukturiranim programom kroz definisanje sesija, dodjelu predavača i pregled rasporeda. Ovaj inkrement transformiše konferenciju iz osnovnog zapisa u kompletan, strukturiran događaj.

## Glavne funkcionalnosti:
 - US-12 / US-12.1 - Pregled sesija konferencije: lista svih sesija
 - US-12.2 - Kreiranje sesije: unos podataka o sesiji (naziv, termin, opis)
 - US-12.3 - Uređivanje sesije: izmjena podataka postojeće sesije
 - US-12.4 - Brisanje sesije: uklanjanje sesije uz potvrdu
 - US-13 - Dodjela predavača sesiji: povezivanje predavača s određenom sesijom
 - US-14 - Pregled rasporeda konferencije: prikaz svih sesija po hronološkom redoslijedu
 
## Zavisnosti:
 - Zavisi od Inkrementa 2 — konferencija mora postojati da bi se kreirale sesije.
 - US-12.2 (Kreiranje sesije) je preduslov za US-12.3, US-12.4, US-13 i US-14.
 - US-13 zahtijeva da predavač (korisnik s odgovarajućom rolom) postoji u sistemu.
 - US-14 (Raspored) ovisi o US-12 — raspored je prazan bez definisanih sesija.

## Glavni rizici:


## Okvirni sprintovi:
 - Sprint 8: Svi user storiji ovog inkrementa (US-12.1, US-12.2, US-12.3, US-12.4, US-13, US-14)





