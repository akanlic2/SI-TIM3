# UC-01: Registracija korisnika
### Akter:
Neregistrovani korisnik (Gost)

### Naziv use casea:
Registracija korisnika

### Kratak opis:
Novi korisnik, koji do sada nije imao kreiran nalog u sistemu, pokreće proces registracije kako bi unio svoje osnovne lične i autentifikacione podatke, uključujući email adresu, lozinku i eventualne dodatne informacije, pri čemu sistem nakon uspješne validacije tih podataka omogućava kreiranje korisničkog naloga i pristup svim relevantnim funkcionalnostima koje su dostupne u okviru njegove uloge.

### Preduslovi:
* Korisnik nije prethodno registrovan u sistemu, odnosno ne postoji zapis sa njegovim emailom u bazi podataka  
* Korisnik ima pristup korisničkom interfejsu aplikacije, odnosno formi za registraciju  
* Korisnik posjeduje validnu i aktivnu email adresu koju može koristiti za autentifikaciju  

### Glavni tok:
1. Korisnik pristupa stranici za registraciju putem korisničkog interfejsa sistema  
2. Sistem prikazuje formu za unos svih potrebnih podataka  
3. Korisnik unosi tražene informacije u odgovarajuća polja  
4. Nakon unosa podataka, korisnik pokreće proces registracije klikom na odgovarajuće dugme  
5. Sistem provodi validaciju svih unesenih podataka kako bi provjerio njihovu ispravnost i potpunost  
6. Sistem dodatno provjerava da li već postoji korisnik sa istom email adresom  
7. Ukoliko su svi uslovi zadovoljeni, sistem kreira novi korisnički nalog i pohranjuje podatke u bazu  
8. Sistem obavještava korisnika o uspješno izvršenoj registraciji i omogućava daljnje korištenje aplikacije  

### Alternativni tokovi:
A1: Email već postoji  
- Sistem detektuje da već postoji korisnički nalog sa unesenom email adresom  
- Sistem prekida proces registracije i prikazuje odgovarajuću poruku o grešci  
- Korisnik ima mogućnost da unese novu email adresu ili odustane od procesa  

A2: Nevalidni ili nepotpuni podaci  
- Sistem prepoznaje da neki od unesenih podataka nisu u ispravnom formatu ili nedostaju  
- Sistem obavještava korisnika o grešci i označava problematična polja  
- Korisnik ispravlja podatke i ponavlja proces  

### Ishod:
Korisnički nalog je uspješno kreiran, pri čemu korisnik dobija mogućnost pristupa sistemu i korištenja njegovih funkcionalnosti u skladu sa definisanom ulogom.

---

# UC-02: Prijava korisnika

### Akter:
Registrovani korisnik

### Naziv use casea:
Prijava korisnika

### Kratak opis:
Registrovani korisnik pokreće proces prijave u sistem unosom svojih autentifikacionih podataka, kao što su email adresa i lozinka, s ciljem pristupa zaštićenim funkcionalnostima aplikacije, pri čemu sistem vrši provjeru identiteta korisnika, kreira sigurnu korisničku sesiju i omogućava pristup svim opcijama u skladu sa njegovom ulogom i privilegijama.

### Preduslovi:
* Korisnik ima prethodno kreiran korisnički nalog u sistemu koji sadrži validne autentifikacione podatke  
* Sistem je dostupan i omogućava komunikaciju sa bazom podataka  
* Korisnik ima pristup korisničkom interfejsu putem kojeg može unijeti podatke za prijavu  
* Nalog korisnika nije blokiran ili deaktiviran od strane sistema ili administratora  

### Glavni tok:
1. Korisnik pristupa stranici za prijavu putem korisničkog interfejsa sistema  
2. Sistem prikazuje formu za unos autentifikacionih podataka  
3. Korisnik unosi svoju email adresu i lozinku u odgovarajuća polja  
4. Korisnik pokreće proces prijave klikom na dugme "Prijava"  
5. Sistem prima unesene podatke i započinje proces validacije  
6. Sistem provjerava da li uneseni podaci odgovaraju zapisima u bazi podataka  
7. Sistem vrši autentifikaciju korisnika na osnovu tačnih podataka  
8. Sistem kreira novu korisničku sesiju i generiše odgovarajuće identifikatore sesije  
9. Sistem evidentira prijavu korisnika u logovima sistema radi sigurnosti i praćenja aktivnosti  
10. Sistem preusmjerava korisnika na početnu ili odgovarajuću stranicu unutar aplikacije  
11. Sistem omogućava pristup funkcionalnostima u skladu sa korisničkom ulogom  

### Alternativni tokovi:
A1: Pogrešni autentifikacioni podaci  
- Sistem detektuje da uneseni email ili lozinka nisu ispravni  
- Sistem odbija prijavu  
- Prikazuje poruku o grešci korisniku  
- Korisnik ima mogućnost ponovnog unosa podataka  

A2: Nalog ne postoji  
- Sistem ne pronalazi korisnika sa unesenom email adresom  
- Sistem obavještava korisnika da nalog ne postoji  
- Korisnik može pokrenuti proces registracije  

A3: Nalog je blokiran ili deaktiviran  
- Sistem prepoznaje status naloga koji onemogućava prijavu  
- Sistem blokira pristup  
- Prikazuje odgovarajuću poruku o statusu naloga  

A4: Greška u sistemu ili bazi podataka  
- Dolazi do problema prilikom provjere podataka  
- Sistem ne može završiti proces prijave  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Korisnik je uspješno autentifikovan i prijavljen u sistem, pri čemu mu je omogućena sigurna sesija i pristup svim funkcionalnostima aplikacije u skladu sa njegovim pravima i ulogom.

---

# UC-03: Odjava iz sistema

### Akter:
Prijavljeni korisnik

### Naziv use casea:
Odjava iz sistema

### Kratak opis:
Prijavljeni korisnik pokreće proces odjave iz sistema s ciljem završetka svoje trenutne sesije korištenja aplikacije, pri čemu sistem osigurava sigurno zatvaranje aktivne sesije, uklanjanje svih privremenih podataka povezanih sa korisnikom i onemogućavanje daljnjeg pristupa zaštićenim funkcionalnostima bez ponovne autentifikacije, čime se dodatno doprinosi zaštiti korisničkog naloga i ukupnoj sigurnosti sistema.

### Preduslovi:
* Korisnik je prethodno uspješno izvršio proces prijave u sistem i ima aktivnu korisničku sesiju koja omogućava pristup funkcionalnostima aplikacije  
* Sistem je dostupan i funkcionalan, odnosno nije došlo do prekida rada servera ili mrežne konekcije  
* Korisnik ima pristup opciji za odjavu koja se nalazi unutar korisničkog interfejsa, kao što je navigacioni meni ili korisnički profil  

### Glavni tok:
1. Korisnik pronalazi opciju za odjavu unutar korisničkog interfejsa sistema, koja je dostupna nakon uspješne prijave  
2. Korisnik inicira proces odjave klikom na odgovarajuće dugme ili opciju  
3. Sistem prima zahtjev za prekid aktivne korisničke sesije  
4. Sistem validira zahtjev kako bi osigurao da je riječ o legitimnoj akciji prijavljenog korisnika  
5. Sistem započinje proces deaktivacije korisničke sesije i briše sve aktivne tokene ili identifikatore sesije  
6. Sistem uklanja ili poništava sve privremene podatke koji su vezani za korisničku sesiju, uključujući podatke u memoriji i kolačiće  
7. Sistem evidentira akciju odjave u logovima sistema radi sigurnosti i praćenja aktivnosti  
8. Sistem preusmjerava korisnika na početnu ili login stranicu aplikacije  
9. Sistem prikazuje korisniku poruku kojom potvrđuje da je odjava uspješno izvršena  

### Alternativni tokovi:
A1: Greška prilikom prekida sesije  
- Tokom procesa odjave dolazi do tehničke greške u sistemu  
- Sistem pokušava ponovo izvršiti operaciju prekida sesije  
- Ukoliko ni nakon ponovnog pokušaja operacija ne uspije, sistem prikazuje korisniku poruku o grešci i preporučuje ponovno učitavanje stranice  

A2: Prekid mrežne konekcije  
- Tokom procesa odjave dolazi do prekida internet konekcije  
- Sistem ne može odmah potvrditi uspješnu odjavu  
- Nakon isteka vremena sesije, sistem automatski zatvara korisničku sesiju iz sigurnosnih razloga  

A3: Korisnik slučajno inicira odjavu  
- Sistem može ponuditi dodatni korak potvrde prije konačne odjave (opcionalno)  
- Ukoliko korisnik odustane od akcije, sistem ga vraća na prethodnu stranicu bez prekida sesije  

### Ishod:
Korisnička sesija je u potpunosti i sigurno završena, svi podaci povezani sa sesijom su uklonjeni ili deaktivirani, te korisnik više nema pristup zaštićenim dijelovima sistema sve dok ponovo ne izvrši proces prijave, čime se osigurava zaštita podataka i integritet sistema.

---

# UC-04: Kreiranje konferencije

### Akter:
Organizator

### Naziv use casea:
Kreiranje konferencije

### Kratak opis:
Organizator, kao korisnik sa odgovarajućim privilegijama u sistemu, pokreće proces kreiranja nove konferencije unosom svih relevantnih informacija o događaju, uključujući naziv, datum, lokaciju, opis i druge dodatne parametre, pri čemu sistem omogućava validaciju, pohranu i kasnije upravljanje tim podacima, čime se stvara osnov za daljnju organizaciju sesija, učesnika i drugih elemenata konferencije.

### Preduslovi:
* Organizator je prethodno uspješno prijavljen u sistem i ima aktivnu korisničku sesiju  
* Korisnik posjeduje odgovarajuću ulogu (organizator) koja mu omogućava pristup funkcionalnosti kreiranja konferencija  
* Sistem je dostupan i omogućava pristup formi za unos podataka o konferenciji  
* Ne postoji tehničko ograničenje koje bi spriječilo kreiranje nove konferencije (npr. nedostupnost baze podataka)  

### Glavni tok:
1. Organizator pristupa sekciji sistema namijenjenoj za upravljanje konferencijama  
2. Organizator bira opciju za kreiranje nove konferencije  
3. Sistem prikazuje formu za unos svih potrebnih podataka o konferenciji  
4. Organizator unosi informacije kao što su naziv konferencije, datum održavanja, lokacija, opis i druge relevantne podatke  
5. Organizator potvrđuje unos klikom na dugme za kreiranje  
6. Sistem provodi validaciju unesenih podataka kako bi osigurao njihovu potpunost i ispravnost  
7. Sistem provjerava da li postoji konflikt sa već postojećim konferencijama (npr. isti termin ili naziv)  
8. Ukoliko su svi uslovi zadovoljeni, sistem sprema podatke o konferenciji u bazu podataka  
9. Sistem generiše jedinstveni identifikator konferencije  
10. Sistem obavještava organizatora o uspješnom kreiranju konferencije i omogućava daljnje upravljanje njome  

### Alternativni tokovi:
A1: Nevalidni ili nepotpuni podaci  
- Sistem detektuje da neki od obaveznih podataka nisu uneseni ili su u pogrešnom formatu  
- Sistem prekida proces kreiranja i prikazuje poruku o grešci  
- Organizator ispravlja podatke i ponavlja unos  

A2: Konflikt termina ili podataka  
- Sistem prepoznaje da već postoji konferencija sa istim ili sličnim parametrima  
- Sistem obavještava organizatora o potencijalnom konfliktu  
- Organizator može izmijeniti podatke ili odustati od kreiranja  

A3: Tehnička greška sistema  
- Tokom procesa dolazi do greške u komunikaciji sa bazom podataka  
- Sistem ne može završiti operaciju  
- Prikazuje poruku o grešci i predlaže ponovno pokušavanje  

### Ishod:
Nova konferencija je uspješno kreirana i pohranjena u sistemu, pri čemu organizator dobija mogućnost daljnjeg upravljanja konferencijom, uključujući dodavanje sesija, upravljanje učesnicima i organizaciju cjelokupnog događaja.

---

# UC-05: Pregled konferencija

### Akter:
Korisnik

### Naziv use casea:
Pregled konferencija

### Kratak opis:
Korisnik pristupa funkcionalnosti pregleda konferencija kako bi dobio uvid u sve dostupne događaje koji su trenutno registrovani u sistemu, pri čemu sistem omogućava prikaz osnovnih informacija o svakoj konferenciji, kao što su naziv, datum održavanja, lokacija i kratak opis, čime korisnik može lakše pretraživati i odabrati konferenciju koja odgovara njegovim interesima ili potrebama.

### Preduslovi:
* U sistemu postoji najmanje jedna kreirana konferencija koja je dostupna za prikaz korisnicima  
* Sistem je dostupan i omogućava učitavanje podataka iz baze  
* Korisnik ima pristup korisničkom interfejsu putem kojeg može pregledati listu konferencija  

### Glavni tok:
1. Korisnik pristupa sekciji aplikacije namijenjenoj za pregled konferencija  
2. Sistem prima zahtjev za prikaz liste konferencija  
3. Sistem uspostavlja komunikaciju sa bazom podataka i dohvaća relevantne informacije  
4. Sistem obrađuje prikupljene podatke i priprema ih za prikaz u korisničkom interfejsu  
5. Sistem prikazuje listu svih dostupnih konferencija zajedno sa osnovnim informacijama  
6. Korisnik pregledava prikazane konferencije i analizira njihove karakteristike  
7. Korisnik može odabrati određenu konferenciju za detaljniji pregled ili daljnju interakciju  

### Alternativni tokovi:
A1: Nema dostupnih konferencija  
- Sistem ne pronalazi nijednu konferenciju u bazi podataka  
- Sistem prikazuje poruku kojom obavještava korisnika da trenutno nema dostupnih događaja  
- Korisnik može pokušati kasnije ili napustiti sekciju  

A2: Greška prilikom učitavanja podataka  
- Dolazi do problema u komunikaciji sa bazom podataka  
- Sistem ne može dohvatiti informacije o konferencijama  
- Sistem prikazuje poruku o grešci i omogućava ponovno učitavanje stranice  

A3: Veliki broj konferencija  
- Sistem detektuje veliki broj rezultata  
- Sistem koristi paginaciju ili filtriranje za prikaz podataka  
- Korisnik navigira kroz stranice ili koristi dodatne opcije filtriranja  

### Ishod:
Korisniku je omogućen pregled svih dostupnih konferencija, pri čemu može dobiti osnovne informacije o svakom događaju i nastaviti daljnju interakciju sa sistemom na osnovu prikazanih podataka.

---

# UC-06: Pregled detalja konferencije

### Akter:
Korisnik

### Naziv use casea:
Pregled detalja konferencije

### Kratak opis:
Korisnik pristupa detaljnim informacijama o odabranoj konferenciji kako bi dobio potpuni uvid u sve relevantne podatke, uključujući tačan naziv, datum i vrijeme održavanja, lokaciju, opis događaja, kao i eventualne dodatne informacije poput rasporeda ili učesnika, pri čemu sistem omogućava prikaz svih tih informacija na strukturiran i pregledan način kako bi korisnik mogao donijeti odluku o daljnjem učestvovanju ili interakciji.

### Preduslovi:
* Konferencija koju korisnik želi pregledati postoji u sistemu i ima definisane osnovne podatke  
* Sistem je dostupan i može dohvatiti detaljne informacije iz baze podataka  
* Korisnik ima pristup listi konferencija iz koje može odabrati željeni događaj  

### Glavni tok:
1. Korisnik pregledava listu dostupnih konferencija  
2. Korisnik odabire konkretnu konferenciju klikom na njen naziv ili odgovarajuću opciju  
3. Sistem prima zahtjev za prikaz detalja odabrane konferencije  
4. Sistem dohvaća sve relevantne informacije iz baze podataka, uključujući osnovne i dodatne podatke  
5. Sistem obrađuje podatke i priprema ih za prikaz u korisničkom interfejsu  
6. Sistem prikazuje detaljan pregled konferencije na ekranu korisnika  
7. Korisnik analizira prikazane informacije i može donijeti odluku o daljnjoj interakciji, kao što je prijava na konferenciju  

### Alternativni tokovi:
A1: Konferencija ne postoji ili je obrisana  
- Sistem ne može pronaći traženu konferenciju u bazi  
- Sistem prikazuje poruku o grešci  
- Korisnik se vraća na listu konferencija  

A2: Greška u dohvaćanju podataka  
- Dolazi do problema u komunikaciji sa bazom podataka  
- Sistem ne može učitati sve potrebne informacije  
- Sistem prikazuje poruku o grešci i omogućava ponovno učitavanje  

A3: Nepotpuni podaci o konferenciji  
- Neki podaci nisu uneseni ili nedostaju  
- Sistem prikazuje dostupne informacije uz napomenu da podaci nisu kompletni  

### Ishod:
Korisniku su uspješno prikazane sve dostupne i relevantne informacije o odabranoj konferenciji, što mu omogućava da donese informisanu odluku o daljnjem učestvovanju ili interakciji sa sistemom.

---

# UC-07: Uređivanje konferencije

### Akter:
Organizator

### Naziv use casea:
Uređivanje konferencije

### Kratak opis:
Organizator pristupa funkcionalnosti uređivanja postojeće konferencije kako bi izmijenio ili ažurirao već unesene informacije o događaju, uključujući naziv, datum, lokaciju, opis ili druge relevantne podatke, pri čemu sistem omogućava izmjenu postojećih zapisa uz prethodnu validaciju unesenih podataka, čime se osigurava tačnost i ažurnost informacija koje su dostupne ostalim korisnicima sistema.

### Preduslovi:
* Konferencija koju organizator želi urediti već postoji u sistemu i ima prethodno definisane podatke  
* Organizator je prijavljen u sistem i ima odgovarajuću korisničku ulogu koja mu omogućava izmjene  
* Sistem je dostupan i omogućava pristup funkcionalnosti uređivanja konferencije  

### Glavni tok:
1. Organizator pristupa sekciji za upravljanje konferencijama  
2. Sistem prikazuje listu konferencija koje organizator može uređivati  
3. Organizator bira željenu konferenciju i otvara opciju za uređivanje  
4. Sistem prikazuje formu sa postojećim podacima konferencije  
5. Organizator vrši izmjene potrebnih podataka u odgovarajućim poljima  
6. Organizator potvrđuje izmjene klikom na dugme za spremanje  
7. Sistem validira unesene izmjene kako bi provjerio njihovu ispravnost i potpunost  
8. Sistem ažurira podatke u bazi podataka  
9. Sistem obavještava organizatora o uspješno izvršenim izmjenama  

### Alternativni tokovi:
A1: Nevalidni ili nepotpuni podaci  
- Sistem detektuje greške u unesenim podacima  
- Sistem odbija spremanje izmjena  
- Prikazuje poruku o grešci i označava problematična polja  
- Organizator ispravlja podatke i ponavlja proces  

A2: Konferencija ne postoji  
- Sistem ne može pronaći traženu konferenciju u bazi  
- Sistem prikazuje poruku o grešci  
- Organizator se vraća na listu konferencija  

A3: Greška prilikom ažuriranja podataka  
- Dolazi do problema u komunikaciji sa bazom podataka  
- Sistem ne uspijeva spremiti izmjene  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Podaci o konferenciji su uspješno ažurirani u sistemu, pri čemu su izmjene odmah vidljive svim korisnicima koji imaju pristup tim informacijama.

---

# UC-08: Brisanje konferencije

### Akter:
Organizator

### Naziv use casea:
Brisanje konferencije

### Kratak opis:
Organizator pokreće proces brisanja postojeće konferencije iz sistema u situacijama kada događaj više nije relevantan, kada je otkazan ili kada više nije potrebno da bude prikazan korisnicima, pri čemu sistem osigurava kontrolisano uklanjanje svih podataka povezanih sa tom konferencijom, uključujući eventualne sesije, prijave učesnika i druge zavisne entitete.

### Preduslovi:
* Konferencija postoji u sistemu i može biti identificirana od strane organizatora  
* Organizator je prijavljen u sistem i ima odgovarajuća prava za brisanje konferencije  
* Sistem je dostupan i omogućava izvršavanje operacije brisanja  

### Glavni tok:
1. Organizator pristupa sekciji za upravljanje konferencijama  
2. Sistem prikazuje listu konferencija  
3. Organizator bira konferenciju koju želi obrisati  
4. Organizator klikne opciju za brisanje konferencije  
5. Sistem prikazuje dijalog za potvrdu akcije kako bi spriječio slučajno brisanje  
6. Organizator potvrđuje da želi obrisati konferenciju  
7. Sistem započinje proces uklanjanja konferencije iz baze podataka  
8. Sistem briše sve povezane podatke, uključujući sesije i prijave učesnika  
9. Sistem ažurira bazu podataka i uklanja sve reference na obrisanu konferenciju  
10. Sistem obavještava organizatora o uspješnom brisanju  

### Alternativni tokovi:
A1: Organizator otkazuje akciju  
- Nakon prikaza dijaloga za potvrdu, organizator odlučuje da ne želi obrisati konferenciju  
- Sistem prekida proces brisanja  
- Konferencija ostaje nepromijenjena  

A2: Greška prilikom brisanja  
- Tokom procesa dolazi do tehničke greške  
- Sistem ne uspijeva ukloniti sve podatke  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

A3: Postoje zavisni podaci koji sprječavaju brisanje  
- Sistem detektuje aktivne prijave ili sesije koje nisu moguće automatski ukloniti  
- Sistem upozorava organizatora i traži dodatnu akciju prije brisanja  

### Ishod:
Konferencija i svi povezani podaci su uspješno uklonjeni iz sistema, pri čemu više nisu dostupni korisnicima niti se pojavljuju u listi konferencija.

---

# UC-09: Pretraga konferencija

### Akter:
Korisnik

### Naziv use casea:
Pretraga konferencija

### Kratak opis:
Korisnik koristi funkcionalnost pretrage kako bi efikasno pronašao konferencije koje odgovaraju njegovim interesima ili specifičnim kriterijima, kao što su naziv, datum, lokacija ili ključne riječi, pri čemu sistem omogućava filtriranje i sortiranje rezultata, čime se značajno olakšava navigacija kroz veliki broj dostupnih konferencija i poboljšava korisničko iskustvo.

### Preduslovi:
* U sistemu postoji skup konferencija koje se mogu pretraživati  
* Sistem podržava funkcionalnost pretrage i filtriranja podataka  
* Korisnik ima pristup interfejsu koji omogućava unos kriterija za pretragu  

### Glavni tok:
1. Korisnik pristupa sekciji za pregled ili pretragu konferencija  
2. Sistem prikazuje polje za unos kriterija pretrage  
3. Korisnik unosi željeni pojam ili kriterij (npr. naziv konferencije ili lokaciju)  
4. Korisnik pokreće pretragu klikom na dugme ili pritiskom na enter  
5. Sistem prima zahtjev i obrađuje unesene kriterije  
6. Sistem pretražuje bazu podataka i pronalazi konferencije koje odgovaraju kriterijima  
7. Sistem filtrira i sortira rezultate prema relevantnosti  
8. Sistem prikazuje listu rezultata korisniku  
9. Korisnik pregledava rezultate i može odabrati neku konferenciju za detaljniji pregled  

### Alternativni tokovi:
A1: Nema rezultata pretrage  
- Sistem ne pronalazi nijednu konferenciju koja odgovara kriterijima  
- Sistem prikazuje poruku da nema rezultata  
- Korisnik može promijeniti kriterije i ponoviti pretragu  

A2: Nevalidan unos kriterija  
- Korisnik unosi neispravan ili nepodržan format podataka  
- Sistem ne može izvršiti pretragu  
- Prikazuje poruku o grešci i traži ispravan unos  

A3: Greška u sistemu ili bazi podataka  
- Dolazi do problema prilikom dohvaćanja podataka  
- Sistem ne može prikazati rezultate  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Korisniku je prikazana lista konferencija koje odgovaraju zadanim kriterijima, čime mu je omogućeno da brzo pronađe željene događaje i nastavi daljnju interakciju sa sistemom.

---

# UC-10: Upravljanje sesijama

### Akter:
Organizator

### Naziv use casea:
Upravljanje sesijama

### Kratak opis:
Organizator koristi funkcionalnost upravljanja sesijama kako bi kreirao, uređivao i brisao pojedinačne sesije unutar konferencije, pri čemu sistem omogućava definisanje detalja kao što su naziv sesije, vrijeme održavanja, opis i drugi relevantni parametri, čime se omogućava strukturiranje i organizacija programa konferencije na jasan i pregledan način.

### Preduslovi:
* Postoji prethodno kreirana konferencija u okviru koje se mogu definisati sesije  
* Organizator je prijavljen u sistem i ima odgovarajuća prava za upravljanje sesijama  
* Sistem je dostupan i omogućava unos i izmjenu podataka o sesijama  

### Glavni tok:
1. Organizator pristupa sekciji za upravljanje sesijama unutar određene konferencije  
2. Sistem prikazuje listu postojećih sesija ili praznu listu ako sesije još nisu kreirane  
3. Organizator bira opciju za kreiranje nove sesije ili uređivanje postojeće  
4. Sistem prikazuje formu za unos ili izmjenu podataka o sesiji  
5. Organizator unosi ili mijenja podatke kao što su naziv sesije, vrijeme, opis i drugi parametri  
6. Organizator potvrđuje unos ili izmjene klikom na odgovarajuće dugme  
7. Sistem validira unesene podatke kako bi osigurao njihovu ispravnost  
8. Sistem sprema novu sesiju ili ažurira postojeću u bazi podataka  
9. Sistem ažurira listu sesija i prikazuje izmjene organizatoru  

### Alternativni tokovi:
A1: Nevalidni ili nepotpuni podaci  
- Sistem detektuje greške u unosu  
- Odbija spremanje sesije  
- Prikazuje poruku o grešci i traži ispravku  

A2: Konflikt termina sesije  
- Sistem prepoznaje da se nova ili izmijenjena sesija vremenski preklapa sa drugom sesijom  
- Sistem upozorava organizatora na konflikt  
- Organizator mijenja termin ili odustaje od izmjene  

A3: Greška u komunikaciji sa bazom  
- Sistem ne uspijeva spremiti podatke  
- Prikazuje poruku o grešci  
- Omogućava ponovni pokušaj  

### Ishod:
Sesije su uspješno kreirane ili ažurirane u okviru konferencije, čime je organizatoru omogućeno da organizuje i strukturira raspored događaja na efikasan i pregledan način.

---

# UC-11: Dodjela predavača

### Akter:
Organizator

### Naziv use casea:
Dodjela predavača

### Kratak opis:
Organizator koristi funkcionalnost dodjele predavača kako bi povezao određenog predavača sa konkretnom sesijom unutar konferencije, pri čemu sistem omogućava izbor predavača iz postojeće liste korisnika ili unesenih podataka, čime se osigurava da svaka sesija ima odgovornu osobu koja vodi sadržaj i učestvuje u realizaciji programa konferencije.

### Preduslovi:
* Postoji kreirana konferencija sa definisanim sesijama kojima se može dodijeliti predavač  
* Predavač postoji u sistemu ili je prethodno registrovan kao korisnik sa odgovarajućom ulogom  
* Organizator je prijavljen u sistem i ima prava za upravljanje sesijama i dodjelu predavača  
* Sistem je dostupan i omogućava pregled i izbor predavača  

### Glavni tok:
1. Organizator pristupa sekciji za upravljanje sesijama unutar određene konferencije  
2. Sistem prikazuje listu sesija koje su dostupne za uređivanje  
3. Organizator bira konkretnu sesiju kojoj želi dodijeliti predavača  
4. Organizator otvara opciju za dodjelu predavača  
5. Sistem prikazuje listu dostupnih predavača ili omogućava pretragu  
6. Organizator bira željenog predavača iz liste  
7. Organizator potvrđuje izbor predavača  
8. Sistem provjerava validnost odabranog predavača  
9. Sistem povezuje predavača sa odabranom sesijom i ažurira podatke u bazi  
10. Sistem prikazuje potvrdu o uspješnoj dodjeli  

### Alternativni tokovi:
A1: Predavač ne postoji u sistemu  
- Sistem ne pronalazi odabranog predavača u bazi podataka  
- Sistem prikazuje poruku o grešci  
- Organizator može pokušati ponovo ili odabrati drugog predavača  

A2: Greška prilikom spremanja podataka  
- Dolazi do problema u komunikaciji sa bazom  
- Sistem ne uspijeva izvršiti dodjelu  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

A3: Neispravan odabir predavača  
- Organizator ne odabere validnog predavača ili ostavi polje prazno  
- Sistem odbija unos i traži ispravku  

### Ishod:
Predavač je uspješno dodijeljen odabranoj sesiji, pri čemu se ta informacija trajno bilježi u sistemu i postaje vidljiva svim korisnicima koji imaju pristup toj sesiji.

---

# UC-12: Dodjela dvorane

### Akter:
Organizator

### Naziv use casea:
Dodjela dvorane

### Kratak opis:
Organizator koristi funkcionalnost dodjele dvorane kako bi odredio fizički ili virtualni prostor u kojem će se određena sesija održavati, pri čemu sistem omogućava izbor dostupne dvorane i provjeru njene zauzetosti u određenom terminu, čime se osigurava pravilna raspodjela resursa i izbjegavanje konflikata u rasporedu.

### Preduslovi:
* Postoji kreirana konferencija sa definisanim sesijama  
* Postoje dvorane koje su prethodno unesene u sistem  
* Organizator je prijavljen i ima prava za upravljanje sesijama i dvoranama  
* Sistem je dostupan i omogućava pregled dostupnih dvorana  

### Glavni tok:
1. Organizator pristupa sekciji za upravljanje sesijama  
2. Sistem prikazuje listu sesija unutar konferencije  
3. Organizator bira sesiju kojoj želi dodijeliti dvoranu  
4. Organizator otvara opciju za dodjelu dvorane  
5. Sistem prikazuje listu dostupnih dvorana zajedno sa njihovim kapacitetom i dostupnošću  
6. Organizator bira odgovarajuću dvoranu  
7. Organizator potvrđuje izbor  
8. Sistem provjerava da li je dvorana slobodna u odabranom terminu  
9. Sistem povezuje dvoranu sa sesijom i sprema podatke u bazu  
10. Sistem prikazuje potvrdu o uspješnoj dodjeli  

### Alternativni tokovi:
A1: Dvorana je zauzeta u odabranom terminu  
- Sistem detektuje konflikt u rasporedu  
- Sistem odbija dodjelu dvorane  
- Prikazuje poruku o grešci i predlaže odabir druge dvorane ili termina  

A2: Dvorana ne postoji  
- Sistem ne pronalazi odabranu dvoranu u bazi  
- Prikazuje poruku o grešci  
- Organizator bira drugu dvoranu  

A3: Greška u sistemu  
- Dolazi do problema prilikom spremanja podataka  
- Sistem ne može završiti operaciju  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Dvorana je uspješno dodijeljena odabranoj sesiji, pri čemu su svi podaci ažurirani u sistemu i omogućavaju pravilno planiranje i realizaciju konferencije bez preklapanja resursa.

---

# UC-13: Validacija konflikata

### Akter:
Sistem

### Naziv use casea:
Validacija konflikata

### Kratak opis:
Sistem automatski provodi proces validacije konflikata prilikom kreiranja ili izmjene sesija i dodjele resursa, pri čemu analizira vremenske termine, dodijeljene dvorane i druge relevantne parametre kako bi spriječio preklapanje događaja i osigurao konzistentnost i ispravnost rasporeda konferencije, čime se minimizira mogućnost grešaka i povećava pouzdanost sistema.

### Preduslovi:
* Postoje definisane sesije unutar konferencije sa određenim terminima i resursima  
* Sistem ima pristup svim relevantnim podacima o sesijama, dvoranama i vremenskim okvirima  
* Pokrenuta je operacija koja zahtijeva provjeru konflikata (npr. dodjela dvorane ili izmjena termina)  
* Sistem je funkcionalan i sposoban izvršiti validaciju podataka  

### Glavni tok:
1. Sistem prima zahtjev koji uključuje izmjenu ili kreiranje sesije ili dodjelu resursa  
2. Sistem identificira relevantne podatke koji trebaju biti validirani (vrijeme, dvorana, sesija)  
3. Sistem pristupa bazi podataka i dohvaća postojeće zapise koji mogu biti u konfliktu  
4. Sistem upoređuje novi ili izmijenjeni zapis sa postojećim podacima  
5. Sistem provjerava da li postoji preklapanje termina za istu dvoranu ili sesiju  
6. Sistem analizira dodatne uslove kao što su kapacitet ili dostupnost resursa  
7. Ukoliko nema konflikta, sistem dozvoljava nastavak operacije  
8. Ukoliko postoji konflikt, sistem blokira operaciju i generiše upozorenje  

### Alternativni tokovi:
A1: Nema konflikta  
- Sistem ne pronalazi preklapanje u rasporedu  
- Operacija se nastavlja bez ograničenja  
- Sistem ne prikazuje upozorenje  

A2: Detektovan konflikt termina  
- Sistem pronalazi da se dvije sesije preklapaju u istoj dvorani ili vremenskom periodu  
- Sistem blokira izvršenje akcije  
- Prikazuje detaljnu poruku o konfliktu  
- Organizator mora izmijeniti podatke  

A3: Greška u validaciji podataka  
- Sistem ne uspijeva izvršiti provjeru zbog tehničkog problema  
- Operacija se prekida  
- Sistem prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Sistem uspješno osigurava da raspored konferencije ostane konzistentan i bez konflikata, pri čemu se sve neispravne operacije blokiraju, a korisniku se pruža jasna povratna informacija o problemu.

---

# UC-14: Prijava na konferenciju

### Akter:
Učesnik

### Naziv use casea:
Prijava na konferenciju

### Kratak opis:
Registrovani korisnik, u ulozi učesnika, koristi funkcionalnost prijave na konferenciju kako bi se evidentirao kao učesnik određenog događaja, pri čemu sistem omogućava provjeru dostupnosti mjesta, validaciju statusa korisnika i pohranu prijave u bazu podataka, čime korisnik dobija pravo pristupa sadržajima i aktivnostima vezanim za odabranu konferenciju.

### Preduslovi:
* Korisnik je prethodno registrovan i prijavljen u sistem  
* Konferencija na koju se korisnik želi prijaviti postoji u sistemu  
* Postoji mogućnost prijave na konferenciju (npr. nije zatvorena ili otkazana)  
* Sistem je dostupan i omogućava izvršenje operacije prijave  

### Glavni tok:
1. Korisnik pristupa listi dostupnih konferencija  
2. Korisnik odabire konferenciju za koju želi izvršiti prijavu  
3. Sistem prikazuje detalje konferencije i opciju za prijavu  
4. Korisnik klikne dugme za prijavu  
5. Sistem provjerava da li je korisnik već prijavljen na tu konferenciju  
6. Sistem provjerava da li postoje slobodna mjesta za prijavu  
7. Ukoliko su uslovi zadovoljeni, sistem evidentira prijavu korisnika u bazi podataka  
8. Sistem ažurira broj prijavljenih učesnika  
9. Sistem prikazuje potvrdu o uspješnoj prijavi  

### Alternativni tokovi:
A1: Korisnik je već prijavljen  
- Sistem detektuje postojeću prijavu  
- Odbija novu prijavu  
- Prikazuje poruku da je korisnik već prijavljen  

A2: Nema slobodnih mjesta  
- Sistem utvrđuje da je kapacitet konferencije popunjen  
- Odbija prijavu  
- Prikazuje poruku o nedostupnosti mjesta  

A3: Greška u sistemu  
- Dolazi do problema prilikom spremanja prijave  
- Sistem ne može završiti operaciju  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Korisnik je uspješno prijavljen na konferenciju, pri čemu je njegova prijava evidentirana u sistemu i omogućava mu daljnje učešće u aktivnostima vezanim za događaj.

---

# UC-15: Odjava sa konferencije

### Akter:
Učesnik

### Naziv use casea:
Odjava sa konferencije

### Kratak opis:
Učesnik koristi funkcionalnost odjave sa konferencije kako bi otkazao svoje prethodno registrovano učešće na određenom događaju, pri čemu sistem omogućava uklanjanje korisnika sa liste učesnika, ažuriranje kapaciteta konferencije i oslobađanje mjesta za druge potencijalne učesnike, čime se osigurava fleksibilnost i tačnost evidencije prijavljenih korisnika.

### Preduslovi:
* Korisnik je prethodno registrovan i prijavljen u sistem  
* Korisnik je već evidentiran kao učesnik određene konferencije  
* Konferencija na koju je korisnik prijavljen i dalje postoji u sistemu  
* Sistem je dostupan i omogućava izvršenje operacije odjave  

### Glavni tok:
1. Korisnik pristupa sekciji koja prikazuje konferencije na koje je prijavljen  
2. Sistem prikazuje listu konferencija sa odgovarajućim informacijama  
3. Korisnik bira konferenciju sa koje želi izvršiti odjavu  
4. Korisnik pokreće proces odjave klikom na odgovarajuće dugme  
5. Sistem prima zahtjev za uklanjanje korisnika sa liste učesnika  
6. Sistem provjerava validnost zahtjeva i status korisnika  
7. Sistem briše zapis o prijavi korisnika iz baze podataka  
8. Sistem ažurira broj slobodnih mjesta na konferenciji  
9. Sistem prikazuje korisniku potvrdu o uspješno izvršenoj odjavi  

### Alternativni tokovi:
A1: Korisnik nije prijavljen na konferenciju  
- Sistem ne pronalazi zapis o prijavi korisnika  
- Sistem odbija operaciju odjave  
- Prikazuje poruku o grešci  

A2: Greška prilikom ažuriranja podataka  
- Dolazi do problema u komunikaciji sa bazom podataka  
- Sistem ne uspijeva ukloniti korisnika sa liste  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

A3: Konferencija je već završena  
- Sistem detektuje da je konferencija prošla ili zatvorena  
- Odjava više nije dozvoljena  
- Prikazuje odgovarajuću poruku korisniku  

### Ishod:
Korisnik je uspješno odjavljen sa konferencije, pri čemu je njegov zapis uklonjen iz sistema, a kapacitet događaja ažuriran i dostupan drugim korisnicima.

---

# UC-16: Pregled rasporeda

### Akter:
Korisnik

### Naziv use casea:
Pregled rasporeda

### Kratak opis:
Korisnik pristupa funkcionalnosti pregleda rasporeda kako bi dobio uvid u organizaciju sesija unutar određene konferencije, uključujući tačno vrijeme održavanja, redoslijed događaja, dodijeljene dvorane i predavače, pri čemu sistem omogućava pregled strukturiranog rasporeda na jasan i pregledan način, čime korisnik može planirati svoje prisustvo i aktivnosti tokom konferencije.

### Preduslovi:
* Postoje definisane sesije unutar konferencije koje imaju određene termine i resurse  
* Korisnik ima pristup konferenciji i njenim podacima  
* Sistem je dostupan i omogućava učitavanje rasporeda iz baze podataka  

### Glavni tok:
1. Korisnik pristupa sekciji za pregled konferencije ili rasporeda  
2. Korisnik odabire opciju za pregled rasporeda  
3. Sistem prima zahtjev za prikaz rasporeda  
4. Sistem dohvaća sve relevantne podatke o sesijama, uključujući vrijeme, dvorane i predavače  
5. Sistem obrađuje podatke i organizuje ih u logički raspored  
6. Sistem prikazuje raspored u preglednom formatu (lista, tabela ili kalendar)  
7. Korisnik pregledava raspored i analizira termine i sadržaj sesija  
8. Korisnik može dodatno filtrirati ili sortirati raspored prema određenim kriterijima  

### Alternativni tokovi:
A1: Nema definisanih sesija  
- Sistem ne pronalazi nijednu sesiju u okviru konferencije  
- Sistem prikazuje poruku da raspored nije dostupan  

A2: Greška prilikom učitavanja podataka  
- Dolazi do problema u komunikaciji sa bazom  
- Sistem ne može prikazati raspored  
- Prikazuje poruku o grešci i omogućava ponovno učitavanje  

A3: Nepotpuni podaci o sesijama  
- Neki elementi rasporeda nisu definisani (npr. dvorana ili vrijeme)  
- Sistem prikazuje dostupne informacije uz upozorenje o nepotpunosti podataka  

### Ishod:
Korisniku je uspješno prikazan raspored sesija, pri čemu dobija jasan pregled organizacije konferencije i može planirati svoje aktivnosti u skladu sa dostupnim informacijama.

---

# UC-17: Postavljanje pitanja

### Akter:
Učesnik

### Naziv use casea:
Postavljanje pitanja

### Kratak opis:
Učesnik koristi funkcionalnost postavljanja pitanja tokom trajanja sesije kako bi dobio dodatna pojašnjenja, informacije ili interakciju sa predavačem, pri čemu sistem omogućava unos, validaciju i pohranu pitanja, kao i njihovo prikazivanje predavaču ili drugim učesnicima, čime se unapređuje interaktivnost i kvalitet same konferencije.

### Preduslovi:
* Korisnik je registrovan i prijavljen u sistem  
* Korisnik je prijavljen na konferenciju i ima pristup sesiji  
* Sesija tokom koje se postavlja pitanje postoji i aktivna je  
* Sistem je dostupan i omogućava unos i obradu pitanja  

### Glavni tok:
1. Korisnik pristupa sesiji u okviru konferencije  
2. Sistem prikazuje interfejs za interakciju, uključujući opciju za postavljanje pitanja  
3. Korisnik unosi tekst pitanja u predviđeno polje  
4. Korisnik pokreće slanje pitanja klikom na odgovarajuće dugme  
5. Sistem prima uneseno pitanje i provodi osnovnu validaciju (npr. provjera da li je polje prazno)  
6. Sistem sprema pitanje u bazu podataka zajedno sa informacijama o korisniku i sesiji  
7. Sistem prikazuje potvrdu korisniku da je pitanje uspješno postavljeno  
8. Sistem omogućava predavaču ili moderatoru da vidi postavljeno pitanje  

### Alternativni tokovi:
A1: Prazno ili neispravno pitanje  
- Korisnik pokušava poslati prazno polje ili nevalidan sadržaj  
- Sistem odbija unos  
- Prikazuje poruku o grešci i traži ispravku  

A2: Sesija nije aktivna  
- Sistem detektuje da sesija još nije počela ili je završena  
- Onemogućava postavljanje pitanja  
- Prikazuje odgovarajuću poruku korisniku  

A3: Greška prilikom spremanja pitanja  
- Dolazi do tehničkog problema u sistemu  
- Sistem ne može pohraniti pitanje  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

### Ishod:
Pitanje korisnika je uspješno evidentirano u sistemu i dostupno predavaču ili drugim učesnicima, čime se omogućava daljnja interakcija i razmjena informacija tokom sesije.

---

# UC-18: Generisanje izvještaja

### Akter:
Administrator / Organizator

### Naziv use casea:
Generisanje izvještaja

### Kratak opis:
Administrator ili organizator koristi funkcionalnost generisanja izvještaja kako bi dobio detaljan pregled podataka o konferenciji, uključujući broj učesnika, raspored sesija, prisutnost i druge relevantne informacije, pri čemu sistem prikuplja, obrađuje i prezentuje podatke u strukturiranom obliku, što omogućava analizu uspješnosti događaja i donošenje budućih odluka.

### Preduslovi:
* Postoje relevantni podaci o konferenciji, učesnicima i sesijama u sistemu  
* Korisnik je prijavljen i ima odgovarajuća prava za pristup izvještajima  
* Sistem je dostupan i omogućava obradu i generisanje podataka  

### Glavni tok:
1. Korisnik pristupa sekciji za izvještaje unutar sistema  
2. Sistem prikazuje dostupne opcije za generisanje izvještaja  
3. Korisnik odabire tip izvještaja ili kriterije (npr. određena konferencija)  
4. Korisnik pokreće proces generisanja izvještaja  
5. Sistem prikuplja sve relevantne podatke iz baze podataka  
6. Sistem obrađuje i analizira prikupljene podatke  
7. Sistem generiše izvještaj u odgovarajućem formatu (npr. tabela ili dokument)  
8. Sistem prikazuje izvještaj korisniku ili omogućava njegovo preuzimanje  

### Alternativni tokovi:
A1: Nema dostupnih podataka  
- Sistem ne pronalazi relevantne informacije za generisanje izvještaja  
- Prikazuje poruku korisniku  
- Onemogućava generisanje izvještaja  

A2: Greška u obradi podataka  
- Dolazi do tehničkog problema tokom obrade  
- Sistem ne može generisati izvještaj  
- Prikazuje poruku o grešci i omogućava ponovni pokušaj  

A3: Neodgovarajuća prava korisnika  
- Korisnik nema dozvolu za pristup izvještajima  
- Sistem blokira pristup  
- Prikazuje poruku o nedozvoljenoj akciji  

### Ishod:
Izvještaj je uspješno generisan i prikazan korisniku, pri čemu pruža detaljan i strukturiran uvid u podatke o konferenciji, što omogućava daljnju analizu i donošenje odluka.

---