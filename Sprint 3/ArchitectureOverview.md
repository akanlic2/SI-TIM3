# Architecture Overview

## Kratak opis arhitektonskog pristupa

Sistem za organizaciju konferencija zasnovan je na klasičnoj slojevitoj (layered) arhitekturi koja razdvaja odgovornosti između četiri horizontalna sloja: 
 - Prezentacijski sloj - upravlja komunikacijom s korisnikom, prima HTTP zahtjeve i vraća odgovore,
 - Aplikacijski sloj - upravlja izvršavanjem korisničkih zahtjeva i povezuje prezentacijski sloj s ostalim dijelovima sistema,
 - Domenski sloj - sadrži poslovna pravila i logiku sistema,
 - Sloj za pristup podacima - upravlja pristupom bazi podataka i omogućava čitanje i zapisivanje podataka.

Svaki sloj ima jasno definisanu odgovornost i komunicira isključivo sa susjednim slojevima. Ovakav pristup omogućava:
 - Razdvajanje odgovornosti - svaki sloj ima svoju jasnu ulogu što sistem čini preglednijim i lakšim za održavanje
 - Jednosmjerna zavisnost - zavisnosti idu samo od viših ka nižim slojevima, što smanjuje međusobnu povezanost komponenti
 - Zamjenjivost slojeva - slojevi komuniciraju preko definisanih interfejsa, pa se pojedine implementacije, poput baze podataka, mogu zamijeniti bez većih izmjena u ostatku sistema
 - Lakše testiranje - svaki sloj se može testirati zasebno
 - Bolja preglednost sistema - standardna struktura omogućava lakše snalaženje u projektu i brže razumijevanje koda
 - Lakše održavanje i izmjene - promjene su lokalizovane na određeni sloj, što smanjuje rizik od grešaka pri uvođenju novih funkcionalnosti

## Glavne komponente sistema

Sistem za organizaciju konferencija je organizovan u 4 sloja:

1. **Presentation layer**: Prima HTTP zahtjeve od klijenata i vraća odgovore. Odgovoran je za validaciju ulaznih podataka,
autentifikaciju korisnika i formatiranje odgovora. Ne sadrži nikakvu poslovnu logiku.
    - `UserController`
    - `ConferenceController`
    - `SessionController`
    - `MaterialController`
    - `EquipmentController`
    - `QuestionController`
    - `AgendaItemController`
    - ...
2. **Application layer**: Orkestrira tok jedne korisničke akcije od početka do kraja. Prima zahtjev od prezentacijskog sloja, zatim poziva module
iz domenskog sloja i sloja za pristup podacima kako bi izvršio zahtjev.
    - `UserService`
    - `ConferenceService`
    - `SessionService`
    - `MaterialService`
    - `EquipmentService`
    - `QuestionService`
    - `AgendaItemService`
    - ...
3. **Domain layer**: Sadrži poslovne entitete i sva pravila koja se na njih primjenjuju. Ovaj sloj ne zna ništa o bazi podataka niti o HTTP-u.
    - `User`
    - `Conference`
    - `Session`
    - `Material`
    - `Equipment`
    - `Question`
    - `AgendaItem`
    - ...
4. **Data access layer**: Ovo je jedini sloj koji direktno komunicira s bazom podataka.
Odgovoran je za čitanje i pisanje podataka, te za prevođenje između domenskih entiteta i strukture baze.
    - `UserRepository`
    - `ConferenceRepository`
    - `SessionRepository`
    - `MaterialRepository`
    - `EquipmentRepository`
    - `QuestionRepository`
    - `AgendaItemRepository`
    - ...

## Odgovornosti komponenti

### Presentation layer

U ovom sloju se definišu API rute za komunikaciju između korisnika i sistema. Svaki od Controllera sadrži rute za
osnovne CRUD operacije. Pa npr. ruta `/conference` ima definisane GET i POST zahtjeve koji redom vraća sve konferencije i kreira novu konferenciju, 
dok ruta `/conference/{id}` ima definisane GET, PUT i DELETE zahtjeve koji redom vraćaju konferenciju sa datim id, modificiraju konferenciju sa datim id i
brišu konferenciju sa datim id. Pored njih, definišu se sve ostale potrebne API rute za uspješnu komunikaciju između sistema i korisnika.

Ovaj sloj također vrši validaciju ulaznih podataka, dakle provjerava da li podaci koji stignu od korisnika su u ispravnom formatu,
da li su očekivanog tipa, da li su obavezna polja prisutna i sl. Pored toga, prezentacijski sloj autentificira korisnika provjerom JWT tokena
koji je stigao u headeru zahtjeva i poređenjem istog sa tokenom generisanim u `UserService`-u pri loginu. Na kraju, ovaj sloj formatira odgovor
te ga vraća korisniku.

### Application layer

Ovaj sloj upravlja izvršavanje korisničkih akcija. Njegov glavni cilj je ostvarivanje uspješne komunikacije između preostalih slojeva sistema.
On prima zahtjev od prezentacijskog sloja, zatim poziva potrebne module iz domenskog sloja i sloja za pristup podacima te na kraju vraća rezultat
prezentacijskom sloju koji to dalje prosljeđuje korisniku.

Također, u `UserService`-u se generišu JWT tokeni pri svakom loginu korisnika te refresh token kako se korisnik ne bi morao prijavljivati pri svakom
isteku trajanja JWT tokena.

### Domain layer

Ovaj sloj predstavlja ključni dio sistema. U njemu se definišu sva poslovna pravila koja se vežu za svaki od entiteta. Pa npr. ovdje se provjerava da li je
došlo do konflikta termina i prostora, da li je korisnik već prijavljen na drugu konferenciju u istom terminu, da li je preostalo išta od tehničke opreme
koju organizator želi dodjeliti određenoj konferenciji i sl.

U domenskom sloju se također definišu korisničke uloge te šta koja uloga smije raditi u sistemu

### Data access layer

Ovaj sloj služi za komunikaciju sa bazom podataka. U njemu se definiše izgled svih entiteta prisutnih u DB, kao i način na koji se ti entiteti prevode
u oblik koji koristi ostatak sistema.

Heširanje korisničkih podataka se također vrši u ovom sloju. Pri kreiranju novog korisnika, `UserRepository` hešira lozinku koju je korisnik unio te je takvu
spašava u bazu. Nakon toga se autentifikacija korisnika pri svakom loginu vrši dohvaćanjem te lozinke iz baze što se izvršava u ovom sloju.

## Tok podataka i interakcija

Svaki korisnički zahtjev prolazi kroz sva četiri sloja u tačno određenom redoslijedu - od prezentacijskog sloja prema sloju za pristup 
podacima pri čitanju/pisanju podataka, te u obrnutom smjeru pri vraćanju odgovora.

Tipičan tok zahtjeva:
- Klijent šalje HTTP zahtjev na određenu API rutu
- Controller (Presentation layer) prima zahtjev, validira ulazne podatke i provjerava JWT token
- Service (Application layer) prima validirani zahtjev te pravi pozive ka domenskim entitetima i repozitorijima
- Domain entitet (Domain layer) primjenjuje poslovna pravila i validacije
- Repository (Data access layer) izvršava operacije nad bazom podataka
- Rezultat se vraća istim putem nazad do klijenta

<img width="1198" height="323" alt="Tok podataka" src="https://github.com/user-attachments/assets/9089292b-5b99-4441-84cb-8f1a02f212ff" />

Kao konkretan primjer toka podataka, ovako izgleda proces kreiranja nove konferencije:
1. Klijent šalje POST zahtjev na rutu `/conference` s podacima o konferenciji i JWT tokenom u headeru
2. `ConferenceController` radi osnovnu validaciju nad ulaznim podacima (format, obavezna polja) i autentificira korisnika
3. `ConferenceService` prima zahtjev te poziva `Conference` domenski entitet
4. `Conference` provjerava poslovna pravila - konflikt termina i prostora, maksimalni broj učesnika validan i sl.
5. Ako su sva pravila zadovoljena, `ConferenceService` poziva `ConferenceRepository`
6. `ConferenceRepository` prevodi domenski entitet u DB strukturu i zapisuje podatke
7. Potvrda o uspješnom kreiranju vraća se kroz sve slojeve nazad do klijenta

## Ključne tehničke odluke

- Izbor četveroslojne arhitekture 

Odabrana je klasična četveroslojna arhitektura jer jasno razdvaja odgovornosti. Svaki sloj se može mijenjati ili testirati zasebno, bez kaskadnih promjena kroz cijeli sistem. 

- JWT autentifikacija s refresh tokenom

Autentifikacija je centralizirana u UserService-u (generisanje tokena) i UserController-u (validacija). Uveden je i refresh token kako se korisnik ne bi morao stalno prijavljivati pri isteku JWT tokena.

- Odvajanje domenskih entiteta od baze podataka

Sva poslovna pravila - provjera konflikta termina, kapaciteta, raspoloživosti opreme, korisničkih uloga - nalaze se isključivo u domain entitetima.
Domenski sloj treba da predstavlja isključivo poslovnu logiku organizacije konferencija, a ne tehničku strukturu tabela u bazi.

- Heširanje lozinki u Data access layeru

Lozinke se nikada ne spremaju u čistom tekstu nego se vrši heširanje prije upisa u bazu. Na ovaj način je osigurana zaštita privatnosti korisnika u slučaju kompromitovanja baze podataka.

- Repository kao jedina tačka pristupa bazi

Cijela komunikacija sa bazom podataka prolazi isključivo kroz repozitorije, što centralizira upite, sprječava dupliranje koda i olakšava održavanje. Ostatak sistema - servisi i domenski entiteti - ne zna ništa o tome kako su podaci pohranjeni, što omogućava zamjenu baze ili ORM-a bez izmjena u ostatku koda. Dodatna prednost je lakše testiranje, jer se repozitorij u testovima jednostavno zamijeni mock implementacijom, bez potrebe za pravom bazom.

## Ograničenja i rizici arhitekture

- Overhead za jednostavne operacije - slojevita arhitektura zahtijeva prolaz kroz sve slojeve čak i za trivijalne CRUD operacije.
- Stroga granica između slojeva može usporiti razvoj.
- Teže praćenje toka izvršavanja - prolazak kroz više slojeva otežava debugovanje i praćenje izvršavanja.
- Domain layer može postati preopterećen. Sva poslovna logika koncentriše se u domenskim entitetima i oni postaju pretrpani odgovornostima. 
Granica između onoga šta pripada domenskom, a šta aplikacijskom sloju nije uvijek jasna.
- Iako slojevi mogu biti dizajnirani da budu nezavisni, promjena zahtjeva može uzrokovati promjene u više slojeva.

## Otvorena pitanja
1. Koji algoritam koristiti za hashiranje lozinki?
2. Koliko dugo će važiti refresh token i hoćemo li implementirati "Refresh Token Rotation" (poništavanje starog pri svakoj generaciji novog JWT-a) radi dodatne sigurnosti u slučaju krađe tokena?
3. Kako će se rješavati konkurentni pristup podacima (npr. dvije osobe pokušavaju rezervisati isto mjesto u isto vrijeme)?
4. Da li ćemo imati centralizovan način obrade grešaka ili će svaki dio sistema to raditi zasebno?

