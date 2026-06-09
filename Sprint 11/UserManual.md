# Korisnicko Uputstvo – ConferenceHub
### Sistem za organizaciju konferencija | ETF Sarajevo 2025/26 | Tim 3

---

## 1. Kome je sistem namijenjen

ConferenceHub je web aplikacija namijenjena svima koji ucestvuju u organizaciji ili pohadanju konferencija. Sistem podrzava cetiri tipa korisnika – od administratora koji ima potpunu kontrolu nad sistemom, do ucesnika koji se prijavljuje i prati program konferencije.

Sistem ne zahtijeva tehnicko znanje. Sve akcije prate jasne vizualne korake i poruke o uspjehu ili gresci.

---

## 2. Korisnicke uloge

| Uloga | Opis | Glavne mogucnosti |
|---|---|---|
| **Admin sistema** | Super admin s potpunim pristupom svim funkcijama sistema | Sve sto mogu organizator i ucesnik, plus upravljanje svim korisnicima i njihovim ulogama |
| **Organizator** | Upravlja konferencijama koje organizira | Kreiranje i uredivanje konferencija, sesija, agende, logistike; dodjela dvorana i opreme; preuzimanje izvjestaja |
| **Predavac** | Govornik dodijeljen na jednu ili vise sesija | Pregled vlastitih sesija, odgovaranje na Q&A pitanja ucesnika |
| **Ucesnik** | Registrovani korisnik koji prisustvuje dogadajima | Pretraga i prijava na konferencije i sesije, postavljanje Q&A pitanja |

> **Napomena:** Registracijom putem formulara svaki novi korisnik automatski dobija ulogu **Ucesnik**. Dodjelu uloga Organizator, Predavac ili Admin moze izvrsiti iskljucivo Administrator sistema.

---

## 3. Prijava i registracija

### 3.1 Prijava u sistem

1. Otvorite adresu aplikacije u pretrazivacu. Sistem vas automatski preusmjerava na stranicu za prijavu.
2. U polje **Username ili Email** unesite vase korisnicko ime ili email adresu.
3. U polje **Password** unesite vasu lozinku.
4. Kliknite dugme **Login**.

**Ocekivani rezultat:** Sistem vas preusmjerava na Dashboard. U gornjem desnom uglu prikazuje se vasa uloga kao badge. Ako ste unijeli pogresne podatke, pojavljuje se poruka *"Pogresni kredencijali"*.

---

### 3.2 Registracija novog naloga

Svaka osoba koja nema nalog moze se registrovati i automatski dobiti ulogu Ucesnik.

1. Na stranici za prijavu kliknite **"Nemate racun? Registrujte se"**.
2. Popunite formu: Ime, Prezime, korisnicko ime (Username), email i lozinku.
3. Kliknite dugme **Register**.

**Ocekivani rezultat:** Sistem kreira nalog i preusmjerava vas na stranicu za prijavu. Prijavite se s upravo unesenim kredencijalima.

---

## 4. Demo kredencijali

Za testiranje i demonstraciju sistema dostupni su sljedeci predefinisani nalozi:

| Uloga | Username | Password | Email |
|---|---|---|---|
| Admin sistema | `Administrator` | `Admin123` | administrator@gmail.com |
| Organizator | `Organizator` | `Org123` | organizator@gmail.com |
| Predavac | `Predavac` | `Pred123` | predavac@gmail.com |
| Ucesnik | `Ucesnik` | `Uces123` | ucesnik@gmail.com |

> Da biste vidjeli sve funkcionalnosti sistema, prijavite se redom sa svakim demo nalogom.

---

## 5. Opis glavnih ekrana

| Ekran | URL putanja | Ko ima pristup |
|---|---|---|
| Login / Prijava | `/login` | Svi (bez prijave) |
| Registracija | `/register` | Svi (bez prijave) |
| Dashboard (Pocetna) | `/dashboard` | Svi prijavljeni |
| Lista konferencija | `/conferences` | Svi prijavljeni |
| Detalji konferencije | `/conferences/{id}` | Svi prijavljeni |
| Sesije konferencije | `/conferences/{id}/sessions` | Svi prijavljeni |
| Detalji sesije | `/sessions/{id}` | Svi prijavljeni |
| Agenda konferencije | `/conferences/{id}/agenda` | Svi prijavljeni |
| Logistika konferencije | `/conferences/{id}/logistics` | Svi prijavljeni (CRUD: org./admin) |
| Izvjestaj konferencije | `/conferences/{id}/report` | Organizator, Admin |
| Dvorane | `/rooms` | Organizator, Admin |
| Oprema | `/equipment` | Organizator, Admin |
| Postavke naloga | `/dashboard` → tab Postavke | Svi prijavljeni |

---

### 5.1 Dashboard

Centralna stranica koja se prikazuje odmah po prijavi. Izgled se mijenja ovisno o ulozi.

**Admin** vidi panel sa svim korisnicima sistema, stat kartice i listu nadolazecih konferencija:

![Dashboard – Admin](Slike%20ekrana/dashboard_admin.png)

**Predavac** vidi listu vlastitih dodijeljenih sesija s detaljima (konferencija, dvorana, termin):

![Dashboard – Predavac](Slike%20ekrana/dashboard_predavac.png)

**Ucesnik** vidi listu konferencija na koje je prijavljen s opcijom odjave i nadolazece konferencije:

![Dashboard – Ucesnik](Slike%20ekrana/dashboard_ucesnik.png)

Na lijevoj strani ekrana nalazi se **bocna navigacija (Sidebar)** s linkovima: Dashboard, Konferencije, Dvorane (samo org./admin), Oprema (samo org./admin), Postavke. Kliknite strelicu `<` za sazimanje sidebara.

U gornjem desnom uglu headera nalazi se **zvono notifikacija** s brojacem neprocitanih poruka:

![Panel notifikacija](Slike%20ekrana/notifikacije.png)

---

### 5.2 Lista konferencija

Prikazuje sve konferencije u sistemu s pretragom po nazivu, filterima po lokaciji i kategoriji te paginacijom. Klik na red otvara detalje konferencije. Organizator/Admin ima dugme **"Nova konferencija"** i dugmad **"Uredi"** uz svaki red.

![Lista konferencija](Slike%20ekrana/lista_konferencija.png)

---

### 5.3 Detalji konferencije

Prikazuje naziv, opis, lokaciju, datume, kategoriju, max. broj ucesnika i status. Organizator i Admin dodatno vide kapacitet (slobodna mjesta) i listu registrovanih ucesnika s pretragom i sortiranjem.

![Detalji konferencije](Slike%20ekrana/detalji_konferencije.png)

Dugmad za navigaciju:

- **Sesije** – lista sesija konferencije
- **Agenda** – hronoloski raspored aktivnosti
- **Logistika** – upravljanje logistickim zadacima
- **Izvjestaj** – PDF izvjestaj (samo org./admin)

---

### 5.4 Sesije konferencije

Prikazuje sve sesije jedne konferencije. Svaka sesija prikazuje naziv, opis, govornika, dvoranu, pocetak i kraj.

Kako sesije izgledaju ucesniku (s dugmetom **"Prijavi se"**):

![Sesije – Ucesnik](Slike%20ekrana/sesije_ucesnik.png)

Kako sesije izgledaju organizatoru (s dugmadima za upravljanje i **"Dodijeli Opremu"**):

![Sesije – Organizator](Slike%20ekrana/sesije_organizator.png)

---

### 5.5 QA panel prikaz korisniku

Izgled QA panela za postavljanje pitanja prilikom sesije.

![Detalji sesije – Q&A panel](Slike%20ekrana/sesija_qa_ucesnik.png)

---

### 5.6 Logistika konferencije

Prikazuje logisticke zadatke konferencije s filterom po tipu (Transport, Catering, Sigurnost...) i dugmetom za kreiranje novog zadatka.

![Logistika konferencije](Slike%20ekrana/logistika.png)

---

### 5.7 Izvjestaj konferencije

Prikazuje statistike konferencije (broj registracija, sesija itd.) s dugmetom za preuzimanje PDF-a.

![Izvjestaj konferencije](Slike%20ekrana/izvjestaj.png)

---

### 5.8 Dvorane

Prikazuje listu svih dvorana s kapacitetima. Dostupno samo Organizatoru i Adminu.

![Stranica Dvorane](Slike%20ekrana/dvorane.png)

---

### 5.9 Oprema

Prikazuje svu tehnicku opremu s filterima po tipu i dostupnosti. Dostupno samo Organizatoru i Adminu.

![Stranica Oprema](Slike%20ekrana/oprema.png)

---

## 6. Korisnicke akcije – korak po korak

### 6.1 Ucesnik: Prijava na konferenciju

1. U sidebaru kliknite **"Konferencije"**.
2. Pronadjite konferenciju koja vas zanima i kliknite na njen naziv.
3. Na stranici detalja kliknite dugme **"Prijavi se"**.

**Ocekivani rezultat:** Sistem potvrdjuje prijavu. Konferencija se pojavljuje u sekciji *"Moje prijave"* na Dashboardu. Ako je konferencija popunjena, sistem prikazuje odgovarajucu poruku i prijava nije moguca.

---

### 6.2 Ucesnik: Odjava s konferencije

1. Odite na **Dashboard**.
2. U sekciji *"Moje prijave"* pronadjite konferenciju.
3. Kliknite dugme **"Odjavi"** uz tu konferenciju.

**Ocekivani rezultat:** Konferencija nestaje iz sekcije *"Moje prijave"*. Jedno mjesto se oslobada za drugog ucesnika.

---

### 6.3 Ucesnik: Prijava na sesiju

Da bi ucesnik mogao postavljati Q&A pitanja, mora biti prijavljen na sesiju. Za to mora biti prethodno prijavljen i na konferenciju.

1. Prijavite se na konferenciju (vidi 6.1).
2. Na stranici detalja konferencije kliknite dugme **"Sesije"**.
3. U listi sesija pronadjite sesiju koja vas zanima.
4. Kliknite dugme **"Prijavi se"** uz tu sesiju.

**Ocekivani rezultat:** Dugme se mijenja u *"Odjavi"*, sto potvrdjuje uspjesnu prijavu na sesiju. Q&A panel za tu sesiju postaje dostupan.

---

### 6.4 Ucesnik: Postavljanje Q&A pitanja

Pitanja se mogu postavljati samo tokom trajanja sesije (ne prije pocetka ni nakon zavrsetka). Ucesnik mora biti **prijavljen na sesiju** da bi mogao postavljati pitanja.

1. Navigirajte na detalje sesije (kliknite na sesiju u listi).
2. Na dnu stranice nalazi se **Q&A panel**.
3. U tekstualno polje unesite tekst pitanja.
4. Kliknite dugme za slanje.

**Ocekivani rezultat:** Pitanje se pojavljuje u listi Q&A panela. Sistem primjenjuje cooldown od 30 sekundi – narednih 30 sekundi nije moguce poslati novo pitanje.

> Ako sesija jos nije pocela, Q&A panel prikazuje poruku da ce biti dostupan od pocetka sesije. Ako je sesija zavrsena, unos je onemogucen.

---

### 6.5 Predavac: Odgovaranje na Q&A pitanja

1. Na Dashboardu pronadjite svoju sesiju i kliknite **"Vidi Detalje"**.
2. Na dnu stranice nalazi se Q&A panel s listom pitanja.
3. Uz pitanje kliknite **"Odgovori"**, unesite tekst odgovora i kliknite **"Snimi"**.
   Alternativno, ako ste odgovorili usmeno tokom sesije, cekirajte checkbox **"Odgovoreno usmeno"**.

**Ocekivani rezultat:** Odgovor postaje vidljiv ispod pitanja svim korisnicima koji pristupe Q&A panelu te sesije.

---

### 6.6 Organizator/Admin: Kreiranje nove konferencije

1. U sidebaru kliknite **"Konferencije"**.
2. Kliknite dugme **"Nova konferencija"** u gornjem desnom uglu.
3. Popunite formu:
   - **Naziv** (3–100 znakova)
   - **Opis** (10–500 znakova)
   - **Lokacija**
   - **Datum pocetka** (mora biti u buducnosti)
   - **Datum zavrsetka** (mora biti nakon pocetka)
   - **Max. broj ucesnika**
   - **Kategorija** (npr. IT)
4. Kliknite **"Sacuvaj"**.

**Ocekivani rezultat:** Nova konferencija se pojavljuje u listi i odmah je dostupna za prijave ucesnika. Greske validacije prikazuju se ispod odgovarajuceg polja.

---

### 6.7 Organizator/Admin: Kreiranje sesije

1. Kliknite na konferenciju u listi da otvorite detalje.
2. Kliknite dugme **"Sesije"**.
3. Kliknite dugme **"+ Kreiraj sesiju"**.
4. Popunite naziv, opis, tip sesije (npr. Talk, Workshop), pocetak i kraj.
5. Opcionalno: dodijelite govornika i dvoranu.
6. Kliknite **"Sacuvaj"**.

**Ocekivani rezultat:** Sesija se pojavljuje u listi sesija konferencije. Ako je dodijeljen predavac, sesija se pojavljuje na njegovom Dashboardu.

---

### 6.8 Organizator/Admin: Upravljanje dvoranama

**Kreiranje dvorane:**

1. U sidebaru kliknite **"Dvorane"**.
2. Kliknite **"Dodaj dvoranu"**.
3. Unesite naziv dvorane i kapacitet.
4. Kliknite **"Sacuvaj"**.

**Ocekivani rezultat:** Dvorana se pojavljuje u listi i dostupna je za dodjelu sesijama.

**Uredivanje dvorane:**

1. Uz dvoranu kliknite **"Uredi"**.
2. Promijenite naziv ili kapacitet.
3. Kliknite **"Sacuvaj promjene"**.

**Brisanje dvorane:**

1. Uz dvoranu kliknite **"Obrisi"**.
2. Potvrdite akciju u dijalogu.

**Ocekivani rezultat:** Dvorana je uklonjena iz liste. Ako je dvorana dodijeljena aktivnim sesijama, brisanje moze biti odbijeno.

---

### 6.9 Organizator/Admin: Dodjela opreme sesiji

Oprema se dodjeljuje sesiji direktno sa stranice sesija unutar konferencije.

1. Odite na stranicu konferencije → kliknite **"Sesije"**.
2. U listi sesija pronadjite sesiju kojoj zelite dodijeliti opremu.
3. Kliknite dugme **"Dodijeli Opremu"** uz tu sesiju.
4. U modalnom prozoru odaberite opremu s liste dostupnih stavki.
5. Potvrdite odabir.

**Ocekivani rezultat:** Odabrana oprema prikazuje se u sekciji *"Oprema"* na stranici detalja sesije. Dodjela smanjuje dostupnu kolicinu opreme za 1.

> Novu opremu mozete kreirati i upravljati njome na stranici `/equipment` (sidebar → **"Oprema"**). Na toj stranici moguce je dodati novu opremu, smanjiti kolicinu dugmetom **"−"** i obrisati stavku.

---

### 6.10 Organizator/Admin: Upravljanje logistikom

1. Otvorite detalje konferencije.
2. Kliknite dugme **"Aktivnosti"**.
3. Na stranici logistike koristite dropdown za filtriranje po tipu zadatka (Transport, Catering, Sigurnost...).
4. Kliknite **"Kreiraj aktivnost"**, popunite naziv, opis, tip i status, te kliknite **"Sacuvaj"**.

**Ocekivani rezultat:** Zadatak se pojavljuje u listi logistickih aktivnosti. Uz svaki zadatak dostupna su dugmad **"Uredi"** i **"Obrisi"**.

---

### 6.11 Organizator/Admin: Preuzimanje PDF izvjestaja

1. Otvorite detalje konferencije.
2. Kliknite dugme **"Izvjestaj"**.
3. Pregledajte statistike na stranici (broj registracija, sesija itd.).
4. Kliknite **"Preuzmi PDF"**.

**Ocekivani rezultat:** PDF fajl izvjestaja preuzima se na vas racunar. Izvjestaj sadrzi ukupan pregled konferencije pogodan za arhiviranje.

---

### 6.12 Admin: Upravljanje korisnicima

1. Prijavite se Admin nalogom i odite na **Dashboard**.
2. U sekciji **"Svi korisnici"** nalazi se panel s listom svih korisnika sistema.
3. Kliknite na korisnika u lijevoj koloni da otvorite njegove detalje.
4. U desnom panelu mozete azurirati ulogu, ime, prezime ili email.
5. Kliknite **"Sacuvaj"**.

**Ocekivani rezultat:** Promjene su odmah aktivne. Korisnik pri sljedecoj prijavi ima azurirane podatke i ulogu s odgovarajucim pravima pristupa.

---

## 7. Notifikacijski sistem

Sistem automatski salje in-app notifikacije za sljedece dogadaje:

- Predavac prima notifikaciju kada ucesnik postavi Q&A pitanje
- Ucesnik prima notifikaciju kada predavac odgovori na pitanje
- Nova registracija na konferenciju
- Izmjena konferencije ili sesije
- Dodjela govornicke uloge sesiji

Sve notifikacije dostupne su u panelu koji se otvara klikom na **zvono** u headeru. Neprocitane notifikacije oznacene su drugacijom bojom. Kliknite na notifikaciju da je oznacite kao procitanu i budete preusmjereni na relevantnu stranicu. Dugme **"Oznaci sve kao procitano"** cisti sve notifikacije odjednom.

---

## 8. Ogranicenja sistema

### Sta korisnik ne moze raditi

**Ucesnik ne moze:**
- Kreirati konferencije, sesije, dvorane ni opremu
- Vidjeti listu registrovanih ucesnika na konferenciji
- Postavljati Q&A pitanja ako nije prijavljen na sesiju
- Postavljati Q&A pitanja izvan vremenskog okvira sesije (prije pocetka ili nakon zavrsetka)
- Slati novo pitanje u Q&A prije nego sto istekne cooldown od 30 sekundi
- Preuzeti PDF izvjestaj konferencije

**Predavac ne moze:**
- Kreirati ili uredivati konferencije i sesije
- Pristupiti stranicama Dvorana i Opreme
- Vidjeti konferencije i sesije koji se ne odnose na njega

**Organizator ne moze:**
- Mijenjati uloge korisnicima (iskljucivo pravo Admina)
- Vidjeti panel svih korisnika sistema

**Svi korisnici ne mogu:**
- Mijenjati vlastitu ulogu
- Brisati sopstveni nalog putem suclja
- Pristupiti stranicama za koje uloga nema pravo (automatsko preusmjeravanje)

---

### Tehnicka ogranicenja

- Sistem ne salje email notifikacije – sve notifikacije su iskljucivo in-app
- Nema podrske za reset lozinke putem emaila – kontaktirajte Administratora
- Korisnicki interfejs dostupan je samo na bosanskom jeziku
- Sekcije *"Govornici"* i *"Izvjestaji"* u sidebaru trenutno su u razvoju
- Materijali sesije nemaju pregled unutar aplikacije – preuzimaju se lokalno
