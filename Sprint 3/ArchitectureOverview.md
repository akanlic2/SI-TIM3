# Architecture Overview

## Glavne komponente sistema

Sistem za organizaciju sistema je oragnizovan u 4 sloja:

- **Presentation layer**: Prima HTTP zahtjeve od klijenata i vraća odgovore. Odgovoran je za validaciju ulaznih podataka,
autentifikaciju korisnika i formatiranje odgovora. Ne sadrži nikakvu poslovnu logiku.
- **Application layer**: Orkestrira tok jedne korisničke akcije od početka do kraja. Prima zahtjev od prezentacijskog sloja, zatim poziva module
iz domenskog sloja i sloja za pristup podacima kako bi izvršio zahtjev.
- **Domain layer**: Sadrži poslovne entitete i sva pravila koja se na njih primjenjuju. Ovaj sloj ne zna ništa o bazi podataka niti o HTTP-u.
- **Data access layer**: Ovo je jedini sloj koji direktno komunicira s bazom podataka. 
Odgovoran je za čitanje i pisanje podataka, te za prevođenje između domenskih entiteta i strukture baze.
