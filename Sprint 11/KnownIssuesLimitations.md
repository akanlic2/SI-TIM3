## Poznati bugovi

- Organizator može uređivati konferencije koje nije on/ona kreirao/la
- Sesije konferencije se mogu kreirati van vremenskog opsega konferencije
- Nakon odabira file-a kod upload-a materijala, tekst Choose File ostaje
- Ukupan broj tehničke opreme može ići u negativne brojeve

## Tehnička ograničenja

## Sigurnosna ograničenja

- Lozinke korisnika su spašene direktno u bazi, bez ikakvog heširanja

## Nedovršene funkcionalnosti

- Govornici tab nije implementiran
- Pokušaj preuzimanja PDF materijala izbacuje 404 Stranica nije pronađena grešku

## Pretpostavke koje sistem pravi

## Dijelovi sistema koje ne treba predstavljati kao potpuno završene

- Iako su izvještaji implementirani na stranici detalja konferencije, oni nisu povezani sa Izvještaji tab na početnoj stranici
- Mnoge poruke o uspešnoj/neuspješnoj akciji su još uvijek implementirane koristeći obični `alert()`
- Datumi su ostali u američkom formatu kod nekih formi za kreiranje/uređivanje
- Prikaz odabranog datuma u formama za kreiranje/uređivanje preklapa dugme za biranje datuma
- "Da" dugme kod potvrde akcije je potpuno bez CSS-a
