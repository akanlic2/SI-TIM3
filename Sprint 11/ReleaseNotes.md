# Release Notes

## Šta je uključeno u finalnu verziju

* Autentifikacija i autorizacija (Sign Up, Sign In, Log Out).
* Upravljanje korisničkim profilima
* Modul za upravljanje konferencijama (kreiranje, pregled detalja, uređivanje, brisanje i pretraga konferencija)
* Modul za upravljanje sesijama unutar konferencija (kreiranje, pregled detalja, uređivanje i brisanje sesija)
* Modul za upravljanje dvoranama (dodavanje, uređivanje, brisanje i dodjela dvorana sesijama)
* Registracija učesnika na konferencije i pojedinačne sesije
* Kreiranje agende konferencije 
* Moduli za logistiku i resurse (tehnička oprema i logističke aktivnosti)
* Q&A panel za komunikaciju sa predavačima
* In-app notifikacije
* Modul za izvještaje namijenjen organizatorima sa PDF export-om statistike

<br>

## Najvažnije funkcionalnosti

* Upravljanje konferencijama: Organizatori mogu u potpunosti kreirati, ažurirati i brisati konferencije, dvorane i sesije.
* Dinamički raspored i agenda: Korisnici imaju uvid u tačne termine i lokacije sesija kroz pregled rasporeda i agende.
* Interakcija u realnom vremenu: Učesnici mogu postavljati pitanja tokom predavanja putem Q&A panela, a predavači na njih odgovarati direktno kroz svoj dashboard.
* Praćenje kapaciteta i prijava: Automatizovana provjera slobodnih mjesta u dvoranama.
* Logistička kontrola: Centralizovana evidencija tehničke opreme i pratećih usluga poput cateringa ili video snimanja.

<br>

## Poznata ograničenja

### Backend

* Lozinke korisnika se čuvaju i porede u nekriptovanom obliku umjesto da se hešuju.
* Mnogi API endpointi prihvataju DTO objekte bez sveobuhvatne validacije modela — prisutne su samo osnovne provjere.
* Nekoliko endpointa koji vraćaju liste podataka (uključujući sesije i upite na dashboard-u) vraća kompletne kolekcije bez paginacije.
* API logovi su minimalni i uglavnom ograničeni na poruke pri pokretanju/migraciji, te jedan ispis u konzolu pri neuspješnoj autentifikaciji.
* Globalni hendler za iznimke može propustiti sirove poruke o grešci u 500 odgovorima.
* Backend registruje memorijski keš, ali ga gotovo ne koristi - jedina iznimka je ograničavanje brzine postavljanja pitanja na sesijama.
* Neke rute kontrolera, poput dohvatanja detalja konferencije, dostupne su bez provjere autorizacije.
* CORS je konfigurisan samo za lokalne razvojne adrese (localhost), što otežava fleksibilnu konfiguraciju za produkcijsko okruženje.
* Dashboard dohvata podatke s hardkodiranim parametrom `pageSize=1000`, koji nije dinamički konfigurabilan.

### Frontend

* Rutiranje je implementirano ručnom manipulacijom `window.history` umjesto standardne biblioteke za rutiranje, što može biti nestabilno.
* Nekoliko stranica koristi hardkodirane putanje do API endpointa i parametre zahtjeva umjesto centralizovane konfiguracije.
* Registracija uvijek dodjeljuje ulogu `ucesnik` i ne nudi konfigurabilni odabir uloge.
* Aplikacija koristi `alert()` iskočne prozore za povratne informacije o nekim greškama API-ja umjesto konzistentnih poruka na ekranu.
* Neki obrasci koriste samo placeholder tekstove i nemaju odgovarajuće `<label>` elemente za kontrole obrazaca.

### Sigurnost

* Ne postoji globalno ograničavanje broja zahtjeva na endpointima za autentifikaciju i većini endpointa za pisanje - jedino ograničenje prisutno je pri postavljanju pitanja na sesijama.
* JWT tajni ključ se učitava direktno iz konfiguracije bez vidljivog obrasca za sigurno upravljanje tajnim podacima.
* Neki odgovori na greške s backenda sadrže detalje o iznimkama koji mogu otkriti interne implementacijske detalje.
* Određeni resursi imaju javno dostupne endpointe, iako su srodne operacije zaštićene autentifikacijom.

### Korisničko iskustvo (UX)

* User interface je dizajniran primarno za desktop uređaje sa bočnim trakama i fiksnom navigacijom, koja nije optimizovana za mobilne ekrane.
* Rukovanje greškama nije konzistentno po stranicama - neke akcije ne prikazuju korisniku razumljivu poruku kada zahtjev ne uspije.
* Pojedina dugmad za navigaciju i tokovi stranica oslanjaju se na ručno ažuriranje URL stanja, što može zbuniti historiju preglednika i duboke linkove.

<br>

## Poznati bugovi

* Preuzimanje materijala - Preuzimanje uploadovanih fajlova nije funkcionalno.
* Prikaz datuma i vremena - Vremenska zona se ne obrađuje ispravno; uneseno vrijeme se prikazuje umanjeno za 2 sata pri uređivanju konferencija i sesija.
* Prevod user interface-a - Na pojedinim mjestima u aplikaciji tekst nije preveden na bosanski jezik (status konferencije, tip konferencije pri kreiranju)
* Moguće je postavljanje termina sesije čak i kada taj termin izlazi van vremenskog opsega odgovarajuće konferencije.

<br>

## Šta nije dio finalne isporuke

Iako su planirane u backlog-u, sljedeće funkcionalnosti nisu uključene u finalnu verziju sistema:

* Upravljanje kotizacijama: Definisanje kategorija, iznosa i praćenje statusa plaćanja (Plaćeno/Neplaćeno) 
* Napredno upravljanje materijalima za učesnike: Dok predavači mogu uploadati materijale, napredni filteri za pretragu i sortiranje materijala od strane organizatora nisu finalizirani
* Verifikacija email adrese: Proces potvrde identiteta putem emaila nakon registracije 



