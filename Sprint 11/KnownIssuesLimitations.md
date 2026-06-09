## Poznati bugovi

- Organizator može uređivati konferencije koje nije on/ona kreirao/la
- Sesije konferencije se mogu kreirati van vremenskog opsega konferencije
- Nakon odabira file-a kod upload-a materijala, tekst Choose File ostaje
- Ukupan broj tehničke opreme može ići u negativne brojeve

## Tehnička ograničenja

- Nema validacije tipa fajla kod uploada materijala
- Upload materijala vjerovatno ima ograničenje veličine fajla koje nije komunicirano korisniku

## Sigurnosna ograničenja

- Lozinke korisnika su spašene direktno u bazi, bez ikakvog heširanja
- Organizatori mogu uređivati određene stavke konferencija koje oni nisu kreirali

## Nedovršene funkcionalnosti

- Govornici tab nije implementiran
- Pokušaj preuzimanja PDF materijala izbacuje 404 Stranica nije pronađena grešku

## Pretpostavke koje sistem pravi

- Pretpostavlja se da postoji samo jedan organizator po konferenciji (ili da je vlasništvo nad konferencijom jednoznačno određeno)
- Sistem pretpostavlja da svaki novoregistrovani korisnik želi ulogu učesnika — nema mogućnosti odabira uloge pri registraciji
- Pretpostavlja se da organizator i predavač su međusobno isključive uloge — jedan korisnik ne može biti oboje istovremeno
- Sistem pretpostavlja da kapacitet konferencije pokriva ukupan broj prijavljenih učesnika, bez razlikovanja po sesijama (učesnik prijavljen na konferenciju može prisustvovati svim sesijama)
- Sistem pretpostavlja da predavač mora postojati kao registrovani korisnik u sistemu — nije moguće dodati eksternog predavača samo sa imenom/biografijom
- Sistem pretpostavlja da učesnik može biti prijavljen na neograničen broj konferencija istovremeno

## Dijelovi sistema koje ne treba predstavljati kao potpuno završene

- Iako su izvještaji implementirani na stranici detalja konferencije, oni nisu povezani sa Izvještaji tab na početnoj stranici
- Mnoge poruke o uspešnoj/neuspješnoj akciji su još uvijek implementirane koristeći obični `alert()`
- Datumi su ostali u američkom formatu kod nekih formi za kreiranje/uređivanje
- Prikaz odabranog datuma u formama za kreiranje/uređivanje preklapa dugme za biranje datuma
- "Da" dugme kod potvrde akcije je potpuno bez CSS-a
