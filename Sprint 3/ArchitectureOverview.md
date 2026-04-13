# Architecture Overview

## Glavne komponente sistema

Sistem za organizaciju konferencija je oragnizovan u 4 sloja:

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
