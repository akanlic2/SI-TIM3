# User Stories

---

## Grupa 1: Autentifikacija (S21, S22, S23)

**Poslovna vrijednost grupe:**
Autentifikacija je temelj sistema — bez nje nije moguće kontrolisati ko pristupa platformi. Registracija, prijava i odjava zajedno čine kompletan tok upravljanja korisničkom sesijom i osiguravaju da samo ovlašteni korisnici mogu koristiti funkcionalnosti sistema.

---

### S21 — Sign Up

- **Opis:** Kao novi korisnik, želim kreirati nalog unosom svojih podataka, kako bih mogao koristiti funkcionalnosti sistema za konferencije.
- **Prioritet:** High
- **Tip:** Feature
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Korisnik posjeduje validnu email adresu
  - Da li je potrebna verifikacija emaila nakon registracije?
  - Koji su minimalni zahtjevi za lozinku?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Preduslov za S22 (Sign in)
  - Preduslov za S24 (Upravljanje korisničkim profilom)
  - Preduslov za S25 (Korisničke role i permisije)
- **Acceptance criteria:**
  - Sistem mora omogućiti unos podataka: ime, prezime, email adresa i lozinka
  - Kada korisnik uspješno unese sve podatke i potvrdi registraciju, korisnik treba dobiti potvrdu o uspješnoj registraciji na ekranu
  - Sistem ne smije dozvoliti registraciju sa već postojećom email adresom
  - Sistem ne smije dozvoliti završetak registracije bez popunjenih svih obaveznih polja
  - Sistem ne smije dozvoliti lozinku kraću od 8 karaktera
  - Sistem ne smije dozvoliti unos email adrese u neispravnom formatu

---

### S22 — Sign In

- **Opis:** Kao registrovani korisnik, želim se prijaviti na sistem koristeći svoju email adresu i lozinku, kako bih pristupio funkcionalnostima aplikacije.
- **Prioritet:** High
- **Tip:** Feature
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prethodno kreirao nalog putem S21
  - Da li sistem treba ograničiti broj neuspješnih pokušaja prijave?
  - Da li je potrebna opcija "Zapamti me"?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Striktna zavisnost: zavisi od S21 (Sign up)
  - Prethodi svim storyjima koji zahtijevaju autorizaciju (S24, S26, S27)
- **Acceptance criteria:**
  - Sistem mora omogućiti unos email adrese i lozinke na stranici za prijavu
  - Kada korisnik unese ispravne kredencijale i klikne "Prijavi se", korisnik treba biti preusmjeren na početnu stranicu sistema
  - Sistem ne smije dozvoliti pristup sistemu korisniku koji nije registrovan
  - Sistem ne smije dozvoliti prijavu sa pogrešnom kombinacijom emaila i lozinke
  - Korisnik treba dobiti jasnu poruku o grešci u slučaju neispravnih kredencijala, bez otkrivanja koji podatak je pogrešan

---

### S23 — Log Out

- **Opis:** Kao prijavljeni korisnik, želim se odjaviti sa svog naloga, kako bi moji podaci i sesija bili sigurni nakon završetka rada.
- **Prioritet:** High
- **Tip:** Feature
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je trenutno prijavljen na sistem
  - Da li se korisnik automatski odjavljuje nakon određenog perioda neaktivnosti?
  - Na koju stranicu se korisnik preusmjerava nakon odjave?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od S22 (Sign in)
- **Acceptance criteria:**
  - Sistem mora prikazati opciju za odjavu svim prijavljenim korisnicima
  - Kada korisnik klikne na dugme za odjavu, korisnik treba biti preusmjeren na stranicu za prijavu
  - Sistem ne smije dozvoliti pristup zaštićenim stranicama nakon odjave
  - Sistem mora poništiti korisničku sesiju nakon odjave, tako da povratak na prethodnu stranicu putem browsera ne otvara zaštićeni sadržaj

---

## Grupa 2: Korisnički profil i role (S24, S25)

**Poslovna vrijednost grupe:**
Nakon što korisnik kreira nalog, potrebno je osigurati da može upravljati svojim podacima i da sistem prepoznaje različite tipove korisnika. Upravljanje profilom povećava povjerenje u sistem, dok role i permisije osiguravaju da svaki korisnik pristupa samo onim funkcionalnostima koje su mu namijenjene.

---

### S24 — Upravljanje korisničkim profilom

- **Opis:** Kao prijavljeni korisnik, želim pregledati i izmijeniti osnovne podatke svog korisničkog profila, kako bih osigurao da su moje informacije tačne.
- **Prioritet:** Medium
- **Tip:** Feature
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem
  - Koji podaci su vidljivi i koje je moguće izmijeniti?
  - Da li je potrebna potvrda lozinke pri promjeni osjetljivih podataka kao što je email adresa?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od S21 (Sign up) i S22 (Sign in)
- **Acceptance criteria:**
  - Sistem mora prikazati trenutne podatke korisničkog profila (ime, prezime, email)
  - Kada korisnik izmijeni podatke i sačuva promjene, korisnik treba dobiti potvrdu o uspješnoj izmjeni
  - Sistem ne smije dozvoliti postavljanje email adrese koja je već u upotrebi od strane drugog korisnika
  - Sistem ne smije dozvoliti čuvanje profila sa praznim obaveznim poljima
  - Sistem mora tražiti potvrdu trenutne lozinke pri promjeni lozinke

---

### S25 — Korisničke role i permisije

- **Opis:** Kao administrator sistema, želim definisati korisničke role i odgovarajuća prava pristupa, kako bih osigurao da svaki korisnik može pristupiti samo onim funkcionalnostima koje odgovaraju njegovoj roli.
- **Prioritet:** High
- **Tip:** Technical Task
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Definirane su najmanje dvije role: organizator i učesnik (posjetilac)
  - Ko dodjeljuje role — automatski sistem ili administrator ručno?
  - Da li jedna osoba može imati više od jedne role?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Preduslov za S26 (Kreiranje konferencije) — zahtijeva rolu organizatora
  - Zavisi od S21 (Sign up) i S22 (Sign in)
- **Acceptance criteria:**
  - Sistem mora podržavati najmanje dvije različite korisničke role sa različitim pravima pristupa
  - Kada korisniku bude dodijeljena određena rola, korisnik treba imati pristup samo onim funkcionalnostima koje su predviđene za tu rolu
  - Sistem ne smije dozvoliti korisniku sa rolom učesnika pristup funkcionalnostima namijenjenim organizatorima (npr. kreiranje konferencije)
  - Sistem mora onemogućiti pristup administratorskim funkcionalnostima svim korisnicima koji nemaju admin rolu
  - Sistem mora primijeniti provjeru permisija na svaki zahtjev, a ne samo na nivou korisničkog sučelja

---

## Grupa 3: Konferencije (S26, S27)

**Poslovna vrijednost grupe:**
Kreiranje i pregled konferencija je centralna svrha platforme. Organizatori trebaju moći jednostavno postaviti konferenciju, a korisnici je pronaći i pregledati. Ove dvije funkcionalnosti zajedno čine osnovu oko koje se gradi ostatak sistema.

---

### S26 — Kreiranje konferencije

- **Opis:** Kao organizator, želim kreirati novu konferenciju unosom osnovnih informacija, kako bih je učinio dostupnom korisnicima sistema.
- **Prioritet:** High
- **Tip:** Feature
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen i ima rolu organizatora (S25)
  - Koji su obavezni podaci za konferenciju (naziv, datum, lokacija, opis)?
  - Da li konferencija postaje odmah vidljiva ili tek nakon odobravanja?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od S22 (Sign in) i S25 (Korisničke role i permisije)
  - Preduslov za S27 (Pregled konferencija)
- **Acceptance criteria:**
  - Sistem mora omogućiti organizatoru unos osnovnih podataka o konferenciji: naziv, datum, lokacija i kratki opis
  - Kada organizator popuni sve obavezne podatke i potvrdi kreiranje, korisnik treba dobiti potvrdu o uspješnom kreiranju konferencije
  - Sistem ne smije dozvoliti kreiranje konferencije bez popunjenih obaveznih polja
  - Sistem ne smije dozvoliti kreiranje konferencije korisniku koji nema rolu organizatora
  - Novokreirana konferencija mora biti vidljiva na listi konferencija (S27) odmah ili nakon odobravanja

---

### S27 — Pregled konferencija

- **Opis:** Kao korisnik sistema, želim pregledati listu dostupnih konferencija, kako bih pronašao konferencije koje me interesuju i odlučio se za učešće.
- **Prioritet:** High
- **Tip:** Feature
- **Sprint:** 6–10
- **Pretpostavke i otvorena pitanja:**
  - Da li je lista konferencija dostupna i neprijavljenim korisnicima?
  - Da li postoji filtriranje ili pretraga po nazivu, datumu ili lokaciji?
  - Koliko konferencija se prikazuje po stranici (paginacija)?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od S26 (Kreiranje konferencije) — lista je prazna bez kreiranih konferencija
- **Acceptance criteria:**
  - Sistem mora prikazati listu svih dostupnih konferencija sa osnovnim informacijama (naziv, datum, lokacija)
  - Kada korisnik pristupi listi konferencija, korisnik treba vidjeti sve aktivne konferencije koje su kreirane u sistemu
  - Sistem mora prikazati jasnu poruku kada nema dostupnih konferencija
  - Sistem ne smije prikazivati konferencije koje su obrisane ili deaktivirane
  - Korisnik treba moći kliknuti na konferenciju i vidjeti njene detaljne informacije
