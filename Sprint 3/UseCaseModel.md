# Use Case Model

---

# UC-01: Registracija korisnika
### Akter:
Neregistrovani korisnik (Gost)
### Naziv use casea:
Registracija korisnika
### Kratak opis:
Novi korisnik kreira korisnički nalog u sistemu unosom osnovnih informacija kao što su email adresa, lozinka i lični podaci. Registracija omogućava korisniku da pristupi sistemu i koristi njegove funkcionalnosti u skladu sa dodijeljenom ulogom.
### Preduslovi:
* Korisnik nema postojeći nalog u sistemu
* Korisnik ima pristup formi za registraciju
* Korisnik posjeduje validnu email adresu
### Glavni tok:
1. Korisnik otvara formu za registraciju
2. Unosi sve potrebne podatke
3. Klikne dugme "Registracija"
4. Sistem validira unesene podatke
5. Sistem provjerava da li email već postoji
6. Sistem kreira korisnički nalog
7. Sistem obavještava korisnika o uspješnoj registraciji
### Alternativni tokovi:
A1: Email već postoji  
- Sistem prepoznaje duplikat  
- Prikazuje poruku o grešci  
- Korisnik unosi novi email  

A2: Nevalidni podaci  
- Sistem detektuje grešku  
- Prikazuje poruku  
- Korisnik ispravlja podatke  
### Ishod:
Korisnički nalog je uspješno kreiran i korisnik može pristupiti sistemu.

---

# UC-02: Prijava korisnika
### Akter:
Registrovani korisnik
### Naziv use casea:
Prijava korisnika
### Kratak opis:
Registrovani korisnik unosi svoje kredencijale kako bi pristupio sistemu i koristio njegove funkcionalnosti.
### Preduslovi:
* Korisnik ima validan nalog
* Sistem je dostupan
### Glavni tok:
1. Korisnik unosi email i lozinku
2. Klikne "Prijava"
3. Sistem provjerava tačnost podataka
4. Sistem autentifikuje korisnika
5. Sistem kreira sesiju
6. Sistem preusmjerava korisnika na početnu stranicu
### Alternativni tokovi:
A1: Pogrešni podaci  
- Sistem odbija prijavu  
- Prikazuje grešku  

A2: Neaktivan nalog  
- Sistem blokira pristup  
- Prikazuje obavještenje  
### Ishod:
Korisnik je uspješno prijavljen i može koristiti sistem.

---

# UC-03: Odjava iz sistema
### Akter:
Prijavljeni korisnik
### Naziv use casea:
Odjava iz sistema
### Kratak opis:
Korisnik prekida aktivnu sesiju kako bi završio rad i osigurao zaštitu svog naloga.
### Preduslovi:
* Korisnik je prijavljen u sistem
* Postoji aktivna sesija
### Glavni tok:
1. Korisnik klikne opciju "Odjava"
2. Sistem prima zahtjev
3. Sistem briše sesiju
4. Sistem uklanja privremene podatke
5. Sistem preusmjerava korisnika
6. Sistem prikazuje potvrdu
### Alternativni tokovi:
A1: Greška sistema  
- Sistem pokušava ponovo  
- Prikazuje grešku ako ne uspije  
### Ishod:
Korisnik je sigurno odjavljen iz sistema.

---

# UC-04: Kreiranje konferencije
### Akter:
Organizator
### Naziv use casea:
Kreiranje konferencije
### Kratak opis:
Organizator kreira novu konferenciju definisanjem osnovnih informacija o događaju.
### Preduslovi:
* Organizator je prijavljen
* Ima potrebna prava
### Glavni tok:
1. Organizator otvara formu
2. Unosi podatke
3. Klikne "Kreiraj"
4. Sistem validira podatke
5. Sistem sprema konferenciju
6. Sistem prikazuje potvrdu
### Alternativni tokovi:
A1: Nevalidni podaci  
- Sistem odbija unos  
- Traži ispravku  
### Ishod:
Konferencija je uspješno kreirana i dostupna korisnicima.

---

# UC-05: Pregled konferencija
### Akter:
Korisnik
### Naziv use casea:
Pregled konferencija
### Kratak opis:
Korisnik pregledava listu dostupnih konferencija kako bi pronašao događaj koji ga zanima.
### Preduslovi:
* Postoje konferencije u sistemu
### Glavni tok:
1. Korisnik otvara listu
2. Sistem učitava podatke
3. Sistem prikazuje konferencije
4. Korisnik pregledava listu
### Alternativni tokovi:
A1: Nema konferencija  
- Sistem prikazuje poruku  
### Ishod:
Lista konferencija je uspješno prikazana korisniku.

---

# UC-06: Pregled detalja konferencije
### Akter:
Korisnik
### Naziv use casea:
Pregled detalja konferencije
### Kratak opis:
Korisnik pregledava detaljne informacije o odabranoj konferenciji kako bi donio odluku o učešću.
### Preduslovi:
* Konferencija postoji
### Glavni tok:
1. Korisnik bira konferenciju
2. Sistem učitava podatke
3. Sistem prikazuje detalje
4. Korisnik analizira informacije
### Alternativni tokovi:
A1: Ne postoji konferencija  
- Sistem prikazuje grešku  
### Ishod:
Detalji konferencije su prikazani korisniku.

---

# UC-07: Uređivanje konferencije
### Akter:
Organizator
### Naziv use casea:
Uređivanje konferencije
### Kratak opis:
Organizator mijenja postojeće podatke konferencije kako bi ažurirao informacije.
### Preduslovi:
* Konferencija postoji
* Organizator ima prava
### Glavni tok:
1. Otvara uređivanje
2. Mijenja podatke
3. Sprema promjene
4. Sistem validira podatke
5. Sistem ažurira zapis
### Alternativni tokovi:
A1: Greška u unosu  
- Sistem odbija izmjene  
### Ishod:
Podaci konferencije su ažurirani.

---

# UC-08: Brisanje konferencije
### Akter:
Organizator
### Naziv use casea:
Brisanje konferencije
### Kratak opis:
Organizator uklanja konferenciju iz sistema kada više nije potrebna ili je otkazana.
### Preduslovi:
* Konferencija postoji
### Glavni tok:
1. Klikne "Obriši"
2. Sistem traži potvrdu
3. Korisnik potvrđuje
4. Sistem briše podatke
5. Sistem prikazuje potvrdu
### Alternativni tokovi:
A1: Otkazivanje akcije  
- Sistem ne briše podatke  
### Ishod:
Konferencija je uklonjena iz sistema.

---

# UC-09: Pretraga konferencija
### Akter:
Korisnik
### Naziv use casea:
Pretraga konferencija
### Kratak opis:
Korisnik koristi opciju pretrage kako bi brzo pronašao konferenciju prema određenim kriterijima.
### Preduslovi:
* Sistem sadrži konferencije
### Glavni tok:
1. Korisnik unosi kriterij
2. Sistem obrađuje zahtjev
3. Sistem filtrira rezultate
4. Sistem prikazuje listu
### Alternativni tokovi:
A1: Nema rezultata  
- Sistem prikazuje poruku  
### Ishod:
Rezultati pretrage su prikazani.

---

# UC-10: Upravljanje sesijama
### Akter:
Organizator
### Naziv use casea:
Upravljanje sesijama
### Kratak opis:
Organizator kreira i uređuje sesije kako bi definisao raspored konferencije.
### Preduslovi:
* Konferencija postoji
### Glavni tok:
1. Otvara sekciju sesija
2. Dodaje ili mijenja sesiju
3. Sprema podatke
4. Sistem validira
5. Sistem ažurira
### Alternativni tokovi:
A1: Nevalidni podaci  
- Sistem odbija unos  
### Ishod:
Sesije su uspješno ažurirane.

---

# UC-11: Dodjela predavača
### Akter:
Organizator
### Naziv use casea:
Dodjela predavača
### Kratak opis:
Organizator dodjeljuje predavača određenoj sesiji kako bi definisao ko vodi događaj.
### Preduslovi:
* Predavač postoji
### Glavni tok:
1. Bira sesiju
2. Bira predavača
3. Sistem povezuje
### Alternativni tokovi:
A1: Predavač ne postoji  
### Ishod:
Predavač je uspješno dodijeljen.

---

# UC-12: Dodjela dvorane
### Akter:
Organizator
### Naziv use casea:
Dodjela dvorane
### Kratak opis:
Organizator dodjeljuje odgovarajući prostor za održavanje sesije.
### Preduslovi:
* Dvorana postoji
### Glavni tok:
1. Bira sesiju
2. Bira dvoranu
3. Sistem provjerava dostupnost
4. Sistem povezuje
### Alternativni tokovi:
A1: Konflikt termina  
- Sistem odbija dodjelu  
### Ishod:
Dvorana je dodijeljena sesiji.

---

# UC-13: Validacija konflikata
### Akter:
Sistem
### Naziv use casea:
Validacija konflikata
### Kratak opis:
Sistem automatski provjerava da li postoji preklapanje termina ili dvorana.
### Preduslovi:
* Postoje sesije
### Glavni tok:
1. Sistem analizira raspored
2. Identifikuje konflikte
3. Blokira neispravne akcije
### Alternativni tokovi:
A1: Nema konflikta  
- Sistem dozvoljava akciju  
### Ishod:
Raspored je validan i bez konflikata.

---

# UC-14: Prijava na konferenciju
### Akter:
Učesnik
### Naziv use casea:
Prijava na konferenciju
### Kratak opis:
Korisnik se prijavljuje na konferenciju kako bi učestvovao i pratio njen sadržaj.
### Preduslovi:
* Korisnik je prijavljen
* Postoji konferencija
### Glavni tok:
1. Klikne prijavu
2. Sistem provjerava status
3. Sistem evidentira korisnika
4. Sistem potvrđuje prijavu
### Alternativni tokovi:
A1: Već prijavljen  
- Sistem odbija  
A2: Nema mjesta  
- Sistem prikazuje poruku  
### Ishod:
Korisnik je prijavljen na konferenciju.

---

# UC-15: Odjava sa konferencije
### Akter:
Učesnik
### Naziv use casea:
Odjava sa konferencije
### Kratak opis:
Korisnik otkazuje svoju prijavu na konferenciju.
### Preduslovi:
* Korisnik je prijavljen
### Glavni tok:
1. Klikne odjava
2. Sistem briše zapis
3. Sistem potvrđuje
### Alternativni tokovi:
A1: Greška sistema  
### Ishod:
Korisnik je uspješno odjavljen sa konferencije.

---

# UC-16: Pregled rasporeda
### Akter:
Korisnik
### Naziv use casea:
Pregled rasporeda
### Kratak opis:
Korisnik pregledava raspored sesija kako bi planirao svoje učešće.
### Preduslovi:
* Postoje sesije
### Glavni tok:
1. Otvara raspored
2. Sistem učitava podatke
3. Sistem prikazuje raspored
### Alternativni tokovi:
A1: Nema sesija  
### Ishod:
Raspored je prikazan korisniku.

---

# UC-17: Postavljanje pitanja
### Akter:
Učesnik
### Naziv use casea:
Postavljanje pitanja
### Kratak opis:
Korisnik postavlja pitanje tokom sesije kako bi dobio dodatne informacije.
### Preduslovi:
* Sesija postoji
### Glavni tok:
1. Unosi pitanje
2. Sistem validira
3. Sistem sprema pitanje
### Alternativni tokovi:
A1: Prazno pitanje  
### Ishod:
Pitanje je evidentirano u sistemu.

---

# UC-18: Generisanje izvještaja
### Akter:
Administrator / Organizator
### Naziv use casea:
Generisanje izvještaja
### Kratak opis:
Korisnik generiše izvještaj o konferenciji, učesnicima i sesijama radi analize i pregleda podataka.
### Preduslovi:
* Postoje relevantni podaci
### Glavni tok:
1. Pokreće generisanje
2. Sistem prikuplja podatke
3. Sistem kreira izvještaj
4. Sistem prikazuje rezultat
### Alternativni tokovi:
A1: Nema podataka  
- Sistem prikazuje poruku  
### Ishod:
Izvještaj je uspješno generisan i dostupan korisniku.