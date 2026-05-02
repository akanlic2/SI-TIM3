# Initial Release Plan

<br>

## INC-01: Autentifikacija i upravljanje korisnicima

### Cilj inkrementa:

Omogućiti osnovni pristup sistemu kroz registraciju, prijavu i upravljanje korisničkim profilom. Bez ovog inkrementa nijedna druga funkcionalnost 
sistema nije upotrebljiva, jer su sve zavisne od autentikovanog korisnika s dodijeljenom rolom.

### Glavne funkcionalnosti:
- US-01 - Sign Up: kreiranje korisničkog naloga (ime, prezime, email, lozinka)
- US-02 - Sign In: prijava registrovanog korisnika na sistem
- US-03 - Log Out: sigurna odjava i poništavanje sesije
- US-04 / US-04.1 / US-04.2 - Upravljanje korisničkim profilom: pregled i izmjena podataka
- US-05 - Korisničke role i permisije: dodjeljivanje rola (učesnik, organizator, administrator) i kontrola pristupa

### Zavisnosti:
- Ovaj inkrement je preduslov za sve ostale.
- US-01 (Sign Up) je preduslov za US-02, US-04 i US-05.
- US-02 (Sign In) je preduslov za sve funkcionalnosti koje zahtijevaju prijavljenog korisnika.
- US-05 (Role i permisije) je preduslov za US-06 (Kreiranje konferencije).

### Glavni rizici:

|ID|Opis rizika|Prioritet|Plan mitigacije|
|---|-----|-----|-----|
| R5  | Neovlašten pristup podacima | Visok | Enkripcija podataka, 2FA |
| R6  | Curenje podataka korisnika | Visok | Enkripcija podataka, 2FA |  
| R8  | Problemi s registracijom korisnika | Srednji | Validacija inputa na frontendu i backendu |
| R27 | Promjena emaila bez verifikacije | Visok | Verifikacija email promjene |
| R35 | Pokušaji pogađanja lozinke (Brute-force)| Visok | Ograničavanje pokušaja prijave i zaključavanje naloga |
| R36 | Krađa korisničke sesije | Visok | Zaštita sesije i automatsko odjavljivanje nakon određenog vremena |
| R40 | Kršenje GDPR propisa | Visok | Implementacija privacy policy, enkripcija podataka, kontrola pristupa i pravo na brisanje podataka |


### Okvirni sprintovi u kojima se očekuje realizacija:
- Sprint 5-6: US-01 (Sign Up), US-02 (Sign In), US-03 (Log Out), US-04 (Profil - pregled i izmjena), US-05 (Role i permisije)

<br>

## INC-02: Osnovno upravljanje konferencijama

### Cilj inkrementa:

Omogućiti organizatorima kreiranje i upravljanje konferencijama, a korisnicima otkrivanje i pregled dostupnih konferencija. Ovo je centralna funkcionalnost 
sistema bez koje isti nema poslovnu svrhu.

### Glavne funkcionalnosti:
- US-06 - Kreiranje konferencije: unos naziva, datuma, lokacije i opisa
- US-07 - Pregled konferencija: lista svih dostupnih konferencija
- US-08 - Detalji konferencije: prikaz kompletnih informacija o odabranoj konferenciji
- US-09 - Uređivanje konferencije: izmjena podataka postojeće konferencije
- US-10 - Brisanje konferencije: uklanjanje konferencije uz potvrdu
- US-11 - Pretraga konferencija: filtriranje i pretraga po ključnim riječima

### Zavisnosti:
- Zavisi od INC-01 - korisnik mora biti prijavljen i imati odgovarajuću rolu (organizator).
- US-06 je preduslov za US-07, US-08, US-09, US-10, US-11.
- US-07 je preduslov za US-08 (detalji konferencije).

### Glavni rizici:

|ID|Opis rizika|Prioritet|Plan mitigacije|
|---|-----|-----|-----|
| R12 | Nemogućnost kreiranja konferencije | Visok | Validacija podataka i error handling na API-ju |
| R13 | Neispravan prikaz konferencija | Srednji | Provjera integracije između backend-a i frontenda |
| R19 | Neispravna evidencija korisnika | Visok | Validacija i constraint u bazi podataka |
| R28 | Nevalidni datumi | Srednji | Validacija datuma na frontendu i backendu | 
| R37 | Nekonzistentnost podataka u sistemu | Visok | Poslovna pravila validacije |
| R39 | Predugo učitavanje podataka u listi | Srednji | Implementacija paginacije |


### Okvirni sprintovi u kojima se očekuje realizacija:
- Sprint 6-7: US-06 (Kreiranje), US-07 (Pregled), US-08 (Detalji), US-09 (Uređivanje), US-10 (Brisanje), US-11 (Pretraga)

<br>

## INC-03: Upravljanje sesijama i rasporedom  

### Cilj inkrementa:

Proširiti konferencije strukturiranim programom kroz definisanje sesija, dodjelu predavača i pregled rasporeda. Ovaj inkrement transformiše konferenciju iz osnovnog zapisa u kompletan, strukturiran događaj.

### Glavne funkcionalnosti:
- US-12 / US-12.1 - Pregled sesija konferencije: lista svih sesija
- US-12.2 - Kreiranje sesije: unos podataka o sesiji (naziv, termin, opis)
- US-12.3 - Uređivanje sesije: izmjena podataka postojeće sesije
- US-12.4 - Brisanje sesije: uklanjanje sesije uz potvrdu
- US-13 - Dodjela predavača sesiji: povezivanje predavača s određenom sesijom
- US-14 - Pregled rasporeda konferencije: prikaz svih sesija po hronološkom redoslijedu
 
### Zavisnosti:
- Zavisi od INC-02 - konferencija mora postojati da bi se kreirale sesije.
- US-12.2 (Kreiranje sesije) je preduslov za US-12.3, US-12.4, US-13 i US-14.
- US-13 zahtijeva da predavač (korisnik s odgovarajućom rolom) postoji u sistemu.
- US-14 (Raspored) ovisi o US-12 - raspored je prazan bez definisanih sesija.

### Glavni rizici:

|ID|Opis rizika|Prioritet|Plan mitigacije|
|---|-----|-----|-----|
| R14 | Preklapanje termina sesija | Visok | Logika provjere konflikta termina prije spremanja | 
| R16 | Greške u rasporedu konferencija | Visok | Optimizacija algoritma raspoređivanja i testiranje |
| R19 | Neispravna evidencija korisnika | Visok | Validacija i constraint u bazi podataka | 
| R28 | Nevalidni datumi | Srednji | Validacija datuma na frontendu i backendu |
| R32 | Problem istovremenog pristupa podacima | Visok | Locking mehanizam | 
| R37 | Nekonzistentnost podataka u sistemu | Visok | Poslovna pravila validacije |


### Okvirni sprintovi u kojima se očekuje realizacija:
- Sprint 8: Svi user storiji ovog inkrementa (US-12.1, US-12.2, US-12.3, US-12.4, US-13, US-14)

<br>

## INC-04: Upravljanje dvoranama

### Cilj inkrementa: 

Uvesti upravljanje fizičkim prostorima i njihovo povezivanje sa sesijama. Ovaj inkrement omogućava kompletnu logistiku prostora konferencije i eliminiše konflikte u rasporedu dvorana.

### Glavne funkcionalnosti:

- US-15.1 - Pregled dvorana: lista svih raspoloživih dvorana
- US-15.2 - Dodavanje dvorane: unos podataka o novoj dvorani (naziv, kapacitet)
- US-15.3 - Uređivanje dvorane: izmjena podataka postojeće dvorane
- US-15.4 - Brisanje dvorane: uklanjanje dvorane uz provjeru da nije dodijeljena aktivnoj sesiji
- US-16 - Dodjela dvorane sesiji: povezivanje dvorane s određenom sesijom
  
### Zavisnosti: 

- Zavisi od INC-03 - sesije moraju postojati da bi im se dodijelile dvorane.
- US-15.2 (Dodavanje dvorane) je preduslov za US-15.3, US-15.4 i US-16.
- US-16 (Dodjela dvorane) zavisi od US-12.2 (Kreiranje sesije) i US-15.2 (Dodavanje dvorane).
- US-15.4 (Brisanje dvorane) mora biti blokiran ako je dvorana dodijeljena aktivnoj sesiji.

### Glavni rizici:

|ID|Opis rizika|Prioritet|Plan mitigacije|
|---|-----|-----|-----|
|R15|Neispravna dodjela dvorana sesijama|Visok|Provjera dostupnosti dvorane prije dodjele|
|R17|Prekoračenje kapaciteta konferencije/dvorane|Visok|Validacija kapaciteta prije prijave|
|R32|Problem istovremenog pristupa podacima dvorana|Visok|Locking mehanizam|


### Okvirni sprintovi u kojima se očekuje realizacija:

- Sprint 8: Svi user storiji ovog inkrementa (US-15.1, US-15.2, US-15.3, US-15.4, US-16)

<br>

## INC-05: Učesnici, kotizacije i obavijesti

### Cilj inkrementa:

Aktivirati punu participaciju korisnika kroz registraciju na konferencije i sesije, definisanje finansijskog modela (kotizacije) te obavještavanje korisnika o relevantnim događajima. Ovaj inkrement čini sistem komercijalno upotrebljivim.

### Glavne funkcionalnsoti: 

- US-18 - Prijava učesnika na konferenciju: registracija i evidencija učesnika
- US-19 - Odjava učesnika sa konferencije: otkazivanje prijave uz ažuriranje kapaciteta
- US-20 - Prijava učesnika na sesiju: prijava na individualne sesije uz provjeru kapaciteta
- US-21 - Pregled popunjenosti kapaciteta: uvid u zauzetost mjesta za organizatora
- US-22 - Lista učesnika po konferenciji: pregled prijavljenih učesnika za organizatora
- US-23.1 - Upravljanje kategorijama kotizacija: definisanje tipova kotizacija
- US-23.2 - Upravljanje iznosima kotizacija: postavljanje cijena i praćenje statusa plaćanja
- US-24 - Obavijesti za korisnike: sistemske notifikacije o važnim događajima

### Zavisnosti: 

- Zavisi od INC-02 - konferencija mora postojati za prijavu učesnika.
- Zavisi od INC-03 - sesije moraju biti definirane za US-20 (Prijava na sesiju).
- US-18 (Prijava na konferenciju) je preduslov za US-19, US-20 i US-22.
- US-23.1 (Kategorije kotizacija) je preduslov za US-23.2 (Iznosi kotizacija).
- US-24 (Obavijesti) zavisi od US-01 (Sign Up) - samo registrovanim korisnicima.

### Glavni rizici:

|ID|Opis rizika|Prioritet|Plan mitigacije|
|---|-----|-----|-----|
|R17|Prekoračenje kapaciteta konferencije|Visok|Validacija kapaciteta prije prijave|
|R18|Neuspjela prijava na konferenciju|Visok|Error handling i retry logika|
|R19|Neispravna evidencija korisnika: greška u bazi ili duplikati podataka|Visok|Validacija i constraint u bazi podataka|
|R9|Neispravno slanje e-mail notifikacija|Srednji|Implementacija logova i alerta, validacija e-mail adresa|
|R26|Nefunkcionisanje notifikacija|Srednji|Monitoring servisa i retry logika|
|R32|Problem istovremenog pristupa podacima|Visok|Locking mehanizam|
|R40|Kršenje GDPR propisa: Neadekvatno upravljanje ličnim podacima|Visok|Implementacija privacy policy, enkripcija podataka, kontrola pristupa i pravo na brisanje podataka|

### Okvirni sprintovi u kojima se očekuje realizacija:

- Sprint 9: Svi user storiji ovog inkrementa (US-18, US-19, US-20, US-21, US-22, US-23.1, US-23.2, US-24)

<br>

## INC-06: Napredno upravljanje i analitika

### Cilj inkrementa:

Proširiti sistem naprednim funkcionalnostima za upravljanje resursima, materijalima i logistikom, te uvesti interaktivnost kroz Q&A sistem i analitiku kroz organizatorske izvještaje. Ovaj inkrement podiže sistem na nivo zrele konferencijske platforme.

### Glavne funkcionalnosti:

- US-25 / US-26 - Upravljanje materijalima: pregled i dodavanje materijala konferencijama/sesijama
- US-27.1 / US-27.2 / US-27.3 / US-27.4 - Upravljanje logistikom: CRUD logističkih aktivnosti (catering, video sadržaj i dr.)
- US-28.1 / US-28.2 / US-28.3 / US-28.4 - Upravljanje tehničkom opremom: evidencija i dodjela opreme konferencijama
- US-29.1 - Postavljanje pitanja za Q&A: učesnici postavljaju pitanja predavaču u toku sesije
- US-29.2 - Prikaz pitanja predavaču: prikaz i upravljanje pitanjima tokom predavanja
- US-30 - Izvještaji za organizatore: statistički izvještaji o prijavama, kapacitetima i kotizacijama

### Zavisnosti:

- Zavisi od svih prethodnih inkremenata - posebno od INC-02 (konferencije) i INC-03 (sesije).
- US-25 i US-26 (Materijali) zavisni od US-09 (Uređivanje konferencije) i US-12 (Sesije).
- US-28.4 (Dodjela opreme) zavisi od US-09 (Uređivanje konferencije) i US-28.2 (Kreiranje opreme).
- US-29.1 (Q&A - pitanja) je preduslov za US-29.2 (Prikaz pitanja predavaču).
- US-29.1 zavisi od US-20 (Prijava na sesiju) - pitanja mogu postavljati samo prijavljeni učesnici.
- US-30 (Izvještaji) zavisi od US-06 (Kreiranje konferencije) i US-18 (Prijava učesnika).

### Glavni rizici:

|ID|Opis rizika|Prioritet|Plan mitigacije|
|---|-----|-----|-----|
|R20|Q&A sistem ne radi tokom sesije|Srednji|Rezervni mehanizmi i monitoring sistema|
|R21|Greška u izvještaju za oragnizatore|Srednji|Testiranje izvještaja i validacija podataka|
|R22|Nedostupnost izvještaja za organizatore|Srednji|Keširanje i fallback pristup|
|R23|Greška u dodjeli opreme|Srednji|Validacija dostupnosti prije dodjele|
|R24|Nedostupnost opreme|Srednji|Upravljanje inventarom i rezervacijama|
|R25|Nemogućnost upload materijala|Srednji|Optimizacija storage sistema i limit kontrola|
|R29|Upload zlonamjernih fajlova|Visok|Antivirus skeniranje i file validacija|
|R30|Dupla rezervacija opreme|Visok|Locking mehanizam pri rezervaciji|
|R31|Neprimjeren sadržaj tokom Q&A|Srednji|Moderacija sadržaja i filter riječi|


### Okvirni sprintovi u kojima se očekuje realizacija:

- Sprint 10: Svi user storiji ovog inkrementa - uz prioritizaciju unutar sprinta: **Visoki prioritet**: US-26 (Materijali), US-29.1, US-29.2 (Q&A), US-30 (Izvještaji); **Srednji prioritet**: US-27.1–US-27.4 (Logistika), US-28.1–US-28.4 (Oprema)







