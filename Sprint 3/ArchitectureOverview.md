# Architecture Overview

## Glavne komponente sistema

Sistem za organizaciju konferencija je oragnizovan u 4 sloja:

1. **Presentation layer**: Prima HTTP zahtjeve od klijenata i vraća odgovore. Odgovoran je za validaciju ulaznih podataka,
autentifikaciju korisnika i formatiranje odgovora. Ne sadrži nikakvu poslovnu logiku.
    - `ConferenceController`
    - `SessionController`
    - `MaterialController`
    - `EquipmentController`
    - `QuestionController`
    - `AgendaItemController`
    - ...
2. **Application layer**: Orkestrira tok jedne korisničke akcije od početka do kraja. Prima zahtjev od prezentacijskog sloja, zatim poziva module
iz domenskog sloja i sloja za pristup podacima kako bi izvršio zahtjev.
    - `ConferenceService`
    - `SessionService`
    - `MaterialService`
    - `EquipmentService`
    - `QuestionService`
    - `AgendaItemService`
    - ...
3. **Domain layer**: Sadrži poslovne entitete i sva pravila koja se na njih primjenjuju. Ovaj sloj ne zna ništa o bazi podataka niti o HTTP-u.
    - `Conference`
    - `Session`
    - `Material`
    - `Equipment`
    - `Question`
    - `AgendaItem`
    - ...
4. **Data access layer**: Ovo je jedini sloj koji direktno komunicira s bazom podataka.
Odgovoran je za čitanje i pisanje podataka, te za prevođenje između domenskih entiteta i strukture baze.
    - `ConferenceRepository`
    - `SessionRepository`
    - `MaterialRepository`
    - `EquipmentRepository`
    - `QuestionRepository`
    - `AgendaItemRepository`
    - ...

## Odgovornosti komponenti

1. **Presentation layer**: U ovom sloju se definišu API rute za komunikaciju između korisnika i sistema. Svaki od Controllera sadrži rute za
osnovne CRUD operacije. Pa npr. ruta `/conference` ima definisane GET i POST zahtjeve koji redom vraća sve konferencije i kreira novu konferenciju, 
dok ruta `/conference/{id}` ima definisane GET, PUT i DELETE zahtjeve koji redom vraćaju konferenciju sa datim id, modificiraju konferenciju sa datim id i
brišu konferenciju sa datim id.
