# User Stories

---

### S21 — Sign Up

- **ID storyja:** US-01
- **Naziv storyja:** Sign Up
- **Sprint:** 5
- **Opis:** Kao novi korisnik, želim kreirati nalog unosom svojih podataka, kako bih mogao koristiti funkcionalnosti sistema za konferencije.
- **Poslovna vrijednost:** Omogućava korisnicima pristup sistemu kroz kreiranje naloga.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Korisnik posjeduje validnu email adresu.
  - Da li je potrebna verifikacija emaila nakon registracije?
  - Koji su minimalni zahtjevi za lozinku?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Preduslov za S22 (Sign In).
  - Preduslov za S24 (Upravljanje korisničkim profilom).
  - Preduslov za S25 (Korisničke role i permisije).
- **Acceptance criteria:**
  - Sistem mora omogućiti unos podataka: ime, prezime, email adresa i lozinka.
  - Kada korisnik uspješno unese sve podatke i potvrdi registraciju, korisnik treba dobiti potvrdu o uspješnoj registraciji na ekranu.
  - Sistem ne smije dozvoliti registraciju sa već postojećom email adresom.
  - Sistem ne smije dozvoliti završetak registracije bez popunjenih svih obaveznih polja.

---

### S22 — Sign In

- **ID storyja:** US-02
- **Naziv storyja:** Sign In
- **Sprint:** 5
- **Opis:** Kao registrovani korisnik, želim se prijaviti na sistem koristeći svoju email adresu i lozinku, kako bih pristupio funkcionalnostima aplikacije.
- **Poslovna vrijednost:** Omogućava ovlaštenim korisnicima pristup zaštićenim funkcionalnostima sistema.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prethodno kreirao nalog putem S21
  - Da li sistem treba ograničiti broj neuspješnih pokušaja prijave?
  - Da li je potrebna opcija "Zapamti me"?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od S21 (Sign Up).
  - Preduslov za S24 (Upravljanje korisničkim profilom), S26 (Kreiranje konferencije), S27 (Pregled konferencija).
- **Acceptance criteria:**
  - Sistem mora omogućiti unos email adrese i lozinke na stranici za prijavu.
  - Kada korisnik unese ispravne kredencijale i klikne "Prijavi se", korisnik treba biti preusmjeren na početnu stranicu sistema.
  - Sistem ne smije dozvoliti pristup sistemu korisniku koji nije registrovan.
  - Sistem ne smije dozvoliti prijavu sa pogrešnom kombinacijom emaila i lozinke.
  - Korisnik treba dobiti jasnu poruku o grešci u slučaju neispravnih kredencijala, bez otkrivanja koji podatak je pogrešan.

---

### S23 — Log Out

- **ID storyja:** US-03
- **Naziv storyja:** Log Out
- **Sprint:** 5
- **Opis:** Kao prijavljeni korisnik, želim se odjaviti sa svog naloga, kako bi moji podaci i sesija bili sigurni nakon završetka rada.
- **Poslovna vrijednost:** Osigurava sigurnost korisničke sesije i zaštitu podataka.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je trenutno prijavljen na sistem
  - Da li se korisnik automatski odjavljuje nakon određenog perioda neaktivnosti?
  - Na koju stranicu se korisnik preusmjerava nakon odjave?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od S22 (Sign In).
- **Acceptance criteria:**
  - Sistem mora prikazati opciju za odjavu svim prijavljenim korisnicima.
  - Kada korisnik klikne na dugme za odjavu, korisnik treba biti preusmjeren na stranicu za prijavu.
  - Sistem ne smije dozvoliti pristup zaštićenim stranicama nakon odjave.
  - Sistem mora poništiti korisničku sesiju nakon odjave, tako da povratak na prethodnu stranicu putem browsera ne otvara zaštićeni sadržaj.

---

### S24 — Upravljanje korisničkim profilom

- **ID storyja:** US-04
- **Naziv storyja:** Upravljanje korisničkim profilom
- **Sprint:** 6
- **Opis:** Kao prijavljeni korisnik, želim upravljati podacima svog korisničkog profila, kako bih osigurao da su moje informacije tačne i ažurne.
- **Poslovna vrijednost:** Omogućava korisnicima kontrolu nad vlastitim podacima i povećava povjerenje u sistem.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem.
  - Koji podaci su vidljivi i koje je moguće izmijeniti?
  - Da li je potrebna potvrda lozinke pri promjeni osjetljivih podataka?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od S21 (Sign Up) i S22 (Sign In).
- **Acceptance criteria:**
  - Sistem mora omogućiti upravljanje korisničkim profilom.
  - Izmjene na korisničkom profilu moraju biti odmah vidljive.  

---

### S24.1 — Pregled korisničkog profila

- **ID storyja:** US-04.1
- **Naziv storyja:** Pregled korisničkog profila
- **Sprint:** 6
- **Opis:** Kao prijavljeni korisnik, želim vidjeti podatke svog korisničkog profila, kako bih provjerio svoje informacije.
- **Poslovna vrijednost:** Omogućava korisniku uvid u trenutno stanje vlastitih podataka.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem.
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje korisničkim profilom (US-24).
- **Acceptance criteria:**
  - Sistem mora prikazati trenutne podatke korisničkog profila (ime, prezime, email).
  - Kada korisnik otvori profil, tada vidi sve svoje trenutne podatke.
  - Sistem ne smije prikazivati podatke drugog korisnika.

---

### S24.2 — Izmjena korisničkog profila

- **ID storyja:** US-04.2
- **Naziv storyja:** Izmjena korisničkog profila
- **Sprint:** 6
- **Opis:** Kao prijavljeni korisnik, želim izmijeniti podatke svog korisničkog profila, kako bih ih ažurirao.
- **Poslovna vrijednost:** Osigurava tačnost korisničkih podataka u sistemu.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem.
  - Da li je potrebna potvrda lozinke pri promjeni email adrese?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje korisničkim profilom (US-24).
- **Acceptance criteria:**
  - Kada korisnik izmijeni podatke i sačuva promjene, korisnik treba dobiti potvrdu o uspješnoj izmjeni.
  - Sistem ne smije dozvoliti postavljanje email adrese koja je već u upotrebi od strane drugog korisnika.
  - Sistem ne smije dozvoliti čuvanje profila sa praznim obaveznim poljima.
  - Sistem mora tražiti potvrdu trenutne lozinke pri promjeni lozinke.

---

### S25 — Korisničke role i permisije

- **ID storyja:** US-05
- **Naziv storyja:** Korisničke role i permisije
- **Sprint:** 6
- **Opis:** Kao administrator sistema, želim definisati korisničke role i odgovarajuća prava pristupa, kako bih osigurao da svaki korisnik može pristupiti samo onim funkcionalnostima koje odgovaraju njegovoj roli.
- **Poslovna vrijednost:** Osigurava sigurnost i kontrolu pristupa funkcionalnostima sistema prema tipu korisnika.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Ko dodjeljuje role — automatski sistem ili administrator ručno?
  - Da li jedna osoba može imati više od jedne role?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Preduslov za S26 (Kreiranje konferencije).
  - Zavisi od S21 (Sign Up) i S22 (Sign In).
- **Acceptance criteria:**
  - Sistem mora podržavati najmanje dvije različite korisničke role sa različitim pravima pristupa.
  - Kada korisniku bude dodijeljena određena rola, korisnik treba imati pristup samo onim funkcionalnostima koje su predviđene za tu rolu.
  - Sistem ne smije dozvoliti korisniku sa rolom učesnika pristup funkcionalnostima namijenjenim organizatorima (npr. kreiranje konferencije).
  - Sistem mora onemogućiti pristup administratorskim funkcionalnostima svim korisnicima koji nemaju admin rolu.
  - Sistem mora primijeniti provjeru permisija na svaki zahtjev.

---

### S26 — Kreiranje konferencije

- **ID storyja:** US-06
- **Naziv storyja:** Kreiranje konferencije
- **Sprint:** 7
- **Opis:** Kao organizator, želim kreirati novu konferenciju unosom osnovnih informacija, kako bih je učinio dostupnom korisnicima sistema.
- **Poslovna vrijednost:** Centralna funkcionalnost platforme koja organizatorima omogućava postavljanje konferencija.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen i ima rolu organizatora (S25)
  - Koji su obavezni podaci za konferenciju (naziv, datum, lokacija, opis)?
  - Da li konferencija postaje odmah vidljiva ili tek nakon odobravanja?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od S22 (Sign In) i S25 (Korisničke role i permisije)
  - Preduslov za S27 (Pregled konferencija)
- **Acceptance criteria:**
  - Sistem mora omogućiti organizatoru unos osnovnih podataka o konferenciji: naziv, datum, lokacija i kratki opis.
  - Kada organizator popuni sve obavezne podatke i potvrdi kreiranje, korisnik treba dobiti potvrdu o uspješnom kreiranju konferencije.
  - Sistem ne smije dozvoliti kreiranje konferencije bez popunjenih obaveznih polja.
  - Sistem ne smije dozvoliti kreiranje konferencije korisniku koji nema rolu organizatora.
  - Novokreirana konferencija mora biti vidljiva na listi konferencija (S27) odmah ili nakon odobravanja.

---

### S27 — Pregled konferencija

- **ID storyja:** US-07
- **Naziv storyja:** Pregled konferencija
- **Sprint:** 7
- **Opis:** Kao korisnik sistema, želim pregledati listu dostupnih konferencija, kako bih pronašao konferencije koje me interesuju i odlučio se za učešće.
- **Poslovna vrijednost:** Korisnicima omogućava otkrivanje dostupnih konferencija i donošenje odluke o učešću.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Da li je lista konferencija dostupna i neprijavljenim korisnicima?
  - Da li postoji filtriranje ili pretraga po nazivu, datumu ili lokaciji?
  - Koliko konferencija se prikazuje po stranici (paginacija)?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od S26 (Kreiranje konferencije) — lista je prazna bez kreiranih konferencija
- **Acceptance criteria:**
  - Sistem mora prikazati listu svih dostupnih konferencija sa osnovnim informacijama (naziv, datum, lokacija).
  - Kada korisnik pristupi listi konferencija, korisnik treba vidjeti sve aktivne konferencije koje su kreirane u sistemu.
  - Sistem mora prikazati jasnu poruku kada nema dostupnih konferencija.
  - Sistem ne smije prikazivati konferencije koje su obrisane ili deaktivirane.
  - Korisnik treba moći kliknuti na konferenciju i vidjeti njene detaljne informacije.

---
### S28 — Detalji konferencije

- **ID storyja:** US-08  
- **Naziv storyja:** Pregled detalja konferencije  
- **Sprint:** 7
- **Opis:** Kao korisnik sistema, želim vidjeti detalje pojedinačne konferencije, kako bih dobio sve potrebne informacije prije prijave.  
- **Poslovna vrijednost:** Omogućava korisnicima donošenje odluke o učešću na osnovu potpunih informacija.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Konferencija je već kreirana u sistemu  
  - Da li svi korisnici imaju pristup svim detaljima konferencije?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferencije (US-26)  
  - Pregled konferencija (US-27)  
- **Acceptance criteria:**
  - Kada korisnik klikne na konferenciju, ako konferencija postoji, tada vidi sve detalje  
  - Sistem mora prikazati naziv, datum, lokaciju i opis  
  - Sistem ne smije prikazivati nepostojeće konferencije  
  - Korisnik treba dobiti jasan i pregledan prikaz informacija  

---

### S29 — Uređivanje konferencije

- **ID storyja:** US-09  
- **Naziv storyja:** Uređivanje konferencije  
- **Sprint:** 8
- **Opis:** Kao organizator, želim izmijeniti podatke postojeće konferencije, kako bih ažurirao informacije.  
- **Poslovna vrijednost:** Omogućava održavanje tačnih i ažurnih podataka.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Korisnik ima rolu organizatora  
  - Koja polja je dozvoljeno mijenjati?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferencije (US-26)  
- **Acceptance criteria:**
  - Kada organizator izmijeni podatke, ako su svi obavezni podaci uneseni, tada se promjene spremaju  
  - Sistem mora omogućiti izmjenu osnovnih podataka  
  - Sistem ne smije dozvoliti izmjene neovlaštenim korisnicima  
  - Korisnik treba dobiti potvrdu o uspješnoj izmjeni  

---

### S30 — Brisanje konferencije

- **ID storyja:** US-10  
- **Naziv storyja:** Brisanje konferencije
- **Sprint:** 8  
- **Opis:** Kao organizator, želim obrisati konferenciju, kako bih uklonio nevažeće događaje.  
- **Poslovna vrijednost:** Omogućava uklanjanje zastarjelih ili otkazanih konferencija.  
- **Prioritet:** Medium  
- **Pretpostavke i otvorena pitanja:**
  - Konferencija postoji  
  - Da li je moguće vratiti obrisanu konferenciju?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferencije (US-26)  
- **Acceptance criteria:**
  - Kada organizator klikne na brisanje, ako potvrdi akciju, tada se konferencija briše  
  - Sistem mora tražiti potvrdu prije brisanja  
  - Sistem ne smije prikazivati obrisane konferencije  
  - Korisnik treba dobiti informaciju da je brisanje uspješno  

---

### S31 — Pretraga konferencija

- **ID storyja:** US-11  
- **Naziv storyja:** Pretraga konferencija
- **Sprint:** 8  
- **Opis:** Kao korisnik, želim pretraživati konferencije, kako bih brzo pronašao željenu konferenciju.  
- **Poslovna vrijednost:** Povećava efikasnost i brzinu pronalaska konferencija.  
- **Prioritet:** Medium  
- **Pretpostavke i otvorena pitanja:**
  - Sistem podržava unos teksta  
  - Da li pretraga podržava djelimičan unos?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Pregled konferencija (US-27)  
- **Acceptance criteria:**
  - Kada korisnik unese pojam, ako postoje rezultati, tada sistem prikazuje odgovarajuće konferencije  
  - Sistem mora omogućiti unos teksta za pretragu  
  - Kada nema rezultata, sistem treba prikazati poruku  
  - Sistem mora prikazati samo relevantne konferencije  

---

### S32 — Upravljanje sesijama

- **ID storyja:** US-12  
- **Naziv storyja:** Upravljanje sesijama 
- **Sprint:** 9 
- **Opis:** Kao organizator, želim upravljati sesijama, kako bih organizovao sadržaj konferencije.  
- **Poslovna vrijednost:** Omogućava organizaciju programa konferencije.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Konferencija postoji  
  - Koji podaci su obavezni za sesiju?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferencije (US-26)  
- **Acceptance criteria:**
  - Sistem mora omogućiti upravljanje sesijama  
  - Sesije moraju biti povezane sa konferencijom  
  - Sistem ne smije dozvoliti kreiranje bez osnovnih podataka  
  - Promjene nad sesijama moraju biti odmah vidljive  

---

### S32.1 — Pregled sesija konferencije

- **ID storyja:** US-12.1  
- **Naziv storyja:** Pregled sesija konferencije 
- **Sprint:** 9  
- **Opis:** Kao organizator, želim vidjeti sve sesije unutar odabrane konferencije, kako bih imao jasan pregled programa konferencije.  
- **Poslovna vrijednost:** Omogućava bolju organizaciju i pregled sadržaja konferencije.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Konferencija je kreirana  
  - Organizator je prijavljen  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12)  
- **Acceptance criteria:**
  - Kada organizator otvori sesije, tada vidi listu svih sesija za konferenciju  
  - Sistem mora prikazati osnovne podatke o sesiji  
  - Kada nema sesija, tada sistem prikazuje odgovarajuću poruku  

---

### S32.2 — Kreiranje sesije

- **ID storyja:** US-12.2  
- **Naziv storyja:** Kreiranje sesije  
- **Sprint:** 9 
- **Opis:** Kao organizator, želim kreirati novu sesiju, kako bih definisao sadržaj konferencije.  
- **Poslovna vrijednost:** Omogućava izgradnju programa konferencije.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Organizator je prijavljen  
  - Konferencija postoji  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12)  
- **Acceptance criteria:**
  - Kada organizator unese validne podatke, tada sistem sprema sesiju  
  - Sistem mora omogućiti unos osnovnih podataka  
  - Sistem ne smije dozvoliti kreiranje bez obaveznih podataka  
  - Sistem ne smije dozvoliti kreiranje sesije sa istim terminom u istoj konferenciji  
  - Nova sesija mora biti vidljiva u listi  

---

### S32.3 — Uređivanje sesije

- **ID storyja:** US-12.3  
- **Naziv storyja:** Uređivanje sesije  
- **Sprint:** 9 
- **Opis:** Kao organizator, želim izmijeniti postojeću sesiju, kako bih ažurirao program konferencije.  
- **Poslovna vrijednost:** Omogućava ažurnost podataka.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Sesija postoji  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12)  
- **Acceptance criteria:**
  - Kada organizator izmijeni podatke, tada sistem sprema promjene  
  - Sistem mora omogućiti izmjenu podataka  
  - Sistem ne smije dozvoliti nevalidne izmjene  
  - Promjene moraju biti odmah vidljive  

---

### S32.4 — Brisanje sesije

- **ID storyja:** US-12.4  
- **Naziv storyja:** Brisanje sesije  
- **Sprint:** 9 
- **Opis:** Kao organizator, želim obrisati sesiju, kako bih uklonio nevažeći sadržaj.  
- **Poslovna vrijednost:** Održava preglednost programa konferencije.  
- **Prioritet:** Medium  
- **Pretpostavke i otvorena pitanja:**
  - Sesija postoji  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12)  
- **Acceptance criteria:**
  - Kada organizator izvrši brisanje sesije, tada se ona uklanja iz sistema  
  - Sistem mora tražiti potvrdu prije brisanja  
  - Obrisana sesija se ne smije prikazivati  

--- 

### S33 — Dodjela predavača sesiji

- **ID storyja:** US-13  
- **Naziv storyja:** Dodjela predavača 
- **Sprint:** 9  
- **Opis:** Kao organizator, želim dodijeliti predavača sesiji, kako bih definisao ko vodi sesiju.  
- **Poslovna vrijednost:** Omogućava jasnu organizaciju i odgovornost.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Predavač postoji u sistemu  
  - Da li jedan predavač može imati više sesija?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12)  
- **Acceptance criteria:**
  - Kada organizator dodijeli predavača, ako predavač postoji, tada se prikazuje u sesiji  
  - Sistem mora omogućiti izbor predavača  
  - Sistem ne smije dozvoliti dodjelu nepostojećeg korisnika  
  - Korisnik treba vidjeti dodijeljenog predavača  

---

### S34 — Pregled rasporeda konferencije

- **ID storyja:** US-14  
- **Naziv storyja:** Pregled rasporeda  
- **Sprint:** 9 
- **Opis:** Kao korisnik, želim vidjeti raspored sesija, kako bih znao kada se šta dešava.  
- **Poslovna vrijednost:** Omogućava planiranje prisustva.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Sesije su već definisane  
  - Kako će raspored biti prikazan (lista, kalendar)?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12) 
- **Acceptance criteria:**
  - Kada korisnik otvori raspored, tada vidi sve sesije po vremenu  
  - Sistem mora prikazati tačne termine i nazive  
  - Sistem mora omogućiti pregled svih sesija  
  - Sistem ne smije prikazivati nepostojeće sesije  

---

### S35 — Upravljanje dvoranama

- **ID storyja:** US-15  
- **Naziv storyja:** Upravljanje dvoranama 
- **Sprint:** 10 
- **Opis:** Kao organizator, želim upravljati dvoranama, kako bih organizovao prostor konferencije.  
- **Poslovna vrijednost:** Omogućava pravilno upravljanje prostorima i raspored sesija.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Sistem podržava unos dvorana  
  - Da li dvorana ima ograničen kapacitet?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Dodjela dvorane (US-16)  
- **Acceptance criteria:**
  - Sistem mora omogućiti upravljanje dvoranama  
  - Sistem ne smije dozvoliti duplikate dvorana  
  - Sve dvorane moraju biti dostupne za dodjelu sesijama  
  - Promjene nad dvoranama moraju biti odmah vidljive  

---

### S35.1 — Pregled dvorana

- **ID storyja:** US-15.1  
- **Naziv storyja:** Pregled dvorana 
- **Sprint:** 10  
- **Opis:** Kao organizator, želim vidjeti sve dvorane, kako bih imao pregled raspoloživih prostora za održavanje sesija.  
- **Poslovna vrijednost:** Omogućava lakše planiranje rasporeda sesija.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Organizator je prijavljen  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje dvoranama (US-15)  
- **Acceptance criteria:**
  - Kada organizator otvori dvorane, tada vidi listu svih dvorana  
  - Sistem mora prikazati osnovne podatke o dvorani  
  - Kada nema dvorana, tada sistem prikazuje odgovarajuću poruku  

---

### S35.2 — Dodavanje dvorane

- **ID storyja:** US-15.2  
- **Naziv storyja:** Dodavanje dvorane  
- **Sprint:** 10 
- **Opis:** Kao organizator, želim dodati novu dvoranu, kako bih omogućio raspoređivanje sesija u odgovarajući prostor.  
- **Poslovna vrijednost:** Povećava mogućnost organizacije konferencije i raspodjele prostora.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Organizator je prijavljen  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje dvoranama (US-15)  
- **Acceptance criteria:**
  - Kada organizator unese validne podatke, tada sistem sprema dvoranu  
  - Sistem mora omogućiti unos osnovnih podataka o dvorani  
  - Sistem ne smije dozvoliti unos duplikata dvorane  
  - Nova dvorana mora biti prikazana u listi  
  - Organizator treba dobiti potvrdu o uspješnom dodavanju  

---

### S35.3 — Uređivanje dvorane

- **ID storyja:** US-15.3  
- **Naziv storyja:** Uređivanje dvorane  
- **Sprint:** 10 
- **Opis:** Kao organizator, želim izmijeniti podatke postojeće dvorane, kako bih održavao tačne informacije o prostoru.  
- **Poslovna vrijednost:** Omogućava ažurnost i tačnost podataka.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Dvorana postoji  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje dvoranama (US-15)  
- **Acceptance criteria:**
  - Kada organizator izmijeni podatke, tada sistem sprema promjene  
  - Sistem mora omogućiti izmjenu podataka o dvorani  
  - Sistem ne smije dozvoliti izmjenu koja stvara duplikat  
  - Promjene moraju biti odmah vidljive  
  - Organizator treba dobiti potvrdu o uspješnoj izmjeni  

---

### S35.4 — Brisanje dvorane

- **ID storyja:** US-15.4  
- **Naziv storyja:** Brisanje dvorane  
- **Sprint:** 10 
- **Opis:** Kao organizator, želim obrisati dvoranu, kako bih uklonio prostor koji više nije dostupan za korištenje.  
- **Poslovna vrijednost:** Omogućava održavanje tačne evidencije prostora i uklanjanje nevažećih podataka.  
- **Prioritet:** Medium  
- **Pretpostavke i otvorena pitanja:**
  - Dvorana postoji  
  - Da li se dvorana može obrisati ako je već dodijeljena sesiji?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje dvoranama (US-15)  
  - Dodjela dvorane (US-16)  
- **Acceptance criteria:**
  - Kada organizator izvrši brisanje dvorane, tada se dvorana uklanja iz sistema  
  - Sistem mora tražiti potvrdu prije brisanja  
  - Sistem ne smije dozvoliti brisanje dvorane koja je dodijeljena aktivnoj sesiji  
  - Obrisana dvorana se ne smije prikazivati u listi  
  - Organizator treba dobiti potvrdu o uspješnom brisanju  

---  

### S36 — Dodjela dvorane sesiji

- **ID storyja:** US-16  
- **Naziv storyja:** Dodjela dvorane 
- **Sprint:** 10  
- **Opis:** Kao organizator, želim dodijeliti dvoranu sesiji, kako bih osigurao prostor.  
- **Poslovna vrijednost:** Omogućava pravilnu raspodjelu resursa.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Dvorane postoje u sistemu  
  - Da li jedna dvorana može imati više sesija u različitim terminima?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje dvoranama (US-15)  
  - Upravljanje sesijama (US-12)  
- **Acceptance criteria:**
  - Kada organizator dodijeli dvoranu, ako postoji, tada se povezuje sa sesijom  
  - Sistem mora omogućiti izbor dvorane  
  - Sistem ne smije dozvoliti nepostojeću dvoranu  
  - Dvorana mora biti prikazana u sesiji  

---

### S38 — Prijava učesnika na konferenciju

- **ID storyja:** US-18  
- **Naziv storyja:** Prijava učesnika 
- **Sprint:** 11 
- **Opis:** Kao učesnik, želim se prijaviti na konferenciju, kako bih učestvovao.  
- **Poslovna vrijednost:** Omogućava korisnicima aktivno učešće u sistemu.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen u sistem  
  - Da li postoji ograničen broj mjesta?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Login (US-22)  
- **Acceptance criteria:**
  - Kada korisnik klikne prijavu, ako nije već prijavljen, tada se registruje  
  - Sistem mora evidentirati korisnika  
  - Sistem ne smije dozvoliti duplikate  
  - Korisnik treba dobiti potvrdu o uspješnoj prijavi  

---

### S39 - Odjava učesnika sa konferencije

- **ID storyja:** US-19
- **Naziv storyja:** Odjava učesnika sa konferencije
- **Sprint:** 11
- **Opis:** Kao učesnik, želim se odjaviti sa konferencije, kako bih oslobodio svoje mjesto.
- **Poslovna vrijednost:** Omogućava preciznu evidenciju dolazaka i automatsko oslobađanje resursa.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:** 
  - Korisnik je prijavljen na konferenciju.
  - Da li postoji rok za odjavu?
  - Da li se vrši povrat novca nakon odjave?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od: Signin (US-02), Prijava učesnika (US-18)
- **Acceptance criteria:**
  - Sistem mora omogućiti odjavu sa konferencije.
  - Sistem mora promijeniti status prijave u 'Otkazano'.
  - Sistem mora ažurirati broj slobodnih mjesta.
  - Ako postoji rok za odjavu, sistem ne smije omogućiti odjavu nakon isteka roka.

---

### S40 - Prijava učesnika na sesiju

- **ID storyja:** US-20
- **Naziv storyja:** Prijava učesnika na sesiju
- **Sprint:** 11
- **Opis:** Kao učesnik, želim se prijaviti na pojedinačne sesije konferencije, kako bih prisustvovao temama koje me zanimaju.
- **Poslovna vrijednost:** Omogućava bolju organizaciju sesija.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:** 
  - Korisnik je prethodno prijavljen na konferenciju i sesije su definisane.
  - Da li korisnik vidi sesije koje su već popunjene ili su one skrivene/onemogućene za klik?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od: Sign in(US-02), Prijava učesnika (US-18), Kreiranje sesije (US-12.2)
- **Acceptance criteria:**
  - Kada postoji slobodno mjesto, ako se korisnik prijavi, tada se registruje na sesiju.
  - Sistem mora smanjiti broj dostupnih mjesta.
  - Sistem ne smije dozvoliti prijavu ako je sesija popunjena.
  - Sistem ne smije dozvoliti prijavu na vremenski preklapajuće sesije istom korisniku.
  - Korisnik treba dobiti potvrdu o prijavi.

---  

### S41 - Pregled popunjenosti kapaciteta

- **ID storyja:** US-21
- **Naziv storyja:** Pregled popunjenosti kapaciteta
- **Sprint:** 11
- **Opis:** Kao organizator, želim vidjeti broj prijavljenih u odnosu na kapacitet, kako bih imao uvid u dostupnost mjesta.
- **Poslovna vrijednost:** Pomaže u boljem upravljanju resursima.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:** 
  - Da li je ovaj pregled javan ili samo za administratore/organizatore događaja?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferenicje (US-06), Dodavanje dvoranje (US-15.2), Prijava učesnika (US-20)
- **Acceptance criteria:**
  - Kada postoji konferencija ili sesija, sistem mora prikazati broj prijavljenih.
  - Sistem mora prikazati maksimalni kapacitet.
  - Sistem mora prikazati da li je kapacitet popunjen.
  - Prikaz se mora ažurirati nakon svake prijave ili odjave.

---

### S42 - Lista učesnika po konferenciji

- **ID storyja:** US-22
- **Naziv storyja:** Lista učesnika po konferenciji
- **Sprint:** 11
- **Opis:** Kao organizator, želim vidjeti listu prijavljenih učesnika, kako bih imao pregled i mogao upravljati događajem.
- **Poslovna vrijednost:** Omogućava lakšu organizaciju i administraciju konferencije.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:** 
  - Koje informacije o učesnicima su vidljive?
  - Da li lista treba biti sortirana po abecedi, vremenu prijave ili tipu kotizacije?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Sign in (US-02), Prijava učesnika (US-20), 
- **Acceptance criteria:**
  - Kada organizator otvori konferenciju, sistem mora prikazati listu učesnika
  - Sistem mora prikazati osnovne podatke (ime, email, ...)
  - Sistem mora omogućiti pretragu ili filtriranje liste
  - Sistem ne smije dozvoliti pristup listi neautorizovanim korisnicima

---

### S43 - Upravljanje kotizacijama

- **ID storyja:** US-23.1
- **Naziv storyja:** Upravljanje kategorijama kotizacija
- **Sprint:** 11
- **Opis:** Kao organizator, želim definisati kategorije, kako bih omogućio različite vrste kotizacija.
- **Poslovna vrijednost:** Omogućava definisanje tipova kotizacija.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:** 
  - Koje kategorije postoje?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferencije (US-06)
- **Acceptance criteria:**
  - Sistem mora omogućiti kreiranje kategorija kotizacija
  - Sistem mora omogućiti izmjenu kategorija
  - Sistem mora omogućiti brisanje kategorija
  - Sistem mora prikazati listu svih kategorija

---

  - **ID storyja:** US-23.2
  - **Naziv storyja:** Upravljanje iznosima kotizacija
  - **Sprint:** 11
  - **Opis:** Kao organizator, želim definisati iznose kotizacija, kako bih upravljao naplatom učešća.
  - **Poslovna vrijednost:** Omogućava kontrolu plaćanja.
  - **Prioritet:** High
  - **Pretpostavke i otvorena pitanja:** 
    - Da li postoji mogućnost unosa popusta ili promo kodova?
    - Da li sistem podržava više valuta ili samo jednu?
  - **Veze sa drugim storyjima ili zavisnostima:**
    - Zavisi od: Upravljanje kategorijama kotizacija (US-23.1)
  - **Acceptance criteria:**
    - Sistem mora omogućiti unos iznosa kotizacije
    - Sistem mora povezati iznos sa kategorijom
    - Sistem mora omogućiti izmjenu iznosa
    - Sistem mora evidentirati status (plaćeno / neplaćeno)
    - Sistem mora omogućiti brisanje kotizacija

### S44 - Obavijesti za korisnike

- **ID storyja:** US-24
- **Naziv storyja:** Obavijesti za korisnike
- **Sprint:** 11
- **Opis:** Kao korisnik, želim primati obavijesti iz sistema, kako bih bio informisan o važnim događajima.
- **Poslovna vrijednost:** Povećava informisanost i poboljšava korisničko iskustvo.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:** 
  - Da li su obavijesti samo u aplikaciji ili i email?
- **Veze sa drugim storyjima ili zavisnostima:**
  - Zavisi od: Sign up (US-01)
- **Acceptance criteria:**
  - Kada se desi važan događaj, sistem mora poslati obavijest
  - Sistem mora prikazati obavijesti u korisničkom interfejsu
  - Korisnik treba dobiti jasnu i razumljivu poruku
  - Korisnik treba imati mogućnost pregleda prethodnih obavijesti

---

### S45.1 — Pregled materijala za konferenciju i sesije

- **ID storyja:** US25
- **Naziv storyja:** Pregled materijala za konferenciju i sesije
- **Sprint:** 12
- **Opis:** Kao organizator, želim mogućnost pregleda dodanih materijala za određenu konferenciju/sesiju.
- **Poslovna vrijednost:** Olakšava planiranje i organizaciju.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S29 (Uređivanje konferencija)
  - Povezano sa S32 (Uređivanje sesija)
- **Acceptance criteria:**
  - Sistem mora prikazati sve trenutno dodane materijale za odabranu konferenciju/sesiju
  - Mogućnost pretrage, filtriranja, sortiranja
  - Klikom na neki od materijala se prikazuje više detalja
 
---

### S45.2 — Dodavanje materijala za konferenciju i sesije

- **ID storyja:** US26
- **Naziv storyja:** Dodavanje materijala za konferenciju i sesije
- **Sprint:** 12
- **Opis:** Kao organizator, želim dodati nove materijale za određenu konferenciju/sesiju.
- **Poslovna vrijednost:** Omogućava organizaciju konferencije.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
  - Da li omogućiti dodavanje istog materijala za više konferencija?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S29 (Uređivanje konferencija)
  - Povezano sa S32 (Uređivanje sesija)
- **Acceptance criteria:**
  - Sistem mora omogućiti dodavanje materijala konferenciji/sesiji
  - Sistem traži unos podataka o novom materijalu
  - Obavezna polja moraju biti popunjena
  - Novi materijal se prikazuje na spisku dodanih materijala konferencije/sesije

---

### S46.1 — Pregled logističkih aktivnosti

- **ID storyja:** US27.1
- **Naziv storyja:** Pregled logističkih aktivnosti
- **Sprint:** 12
- **Opis:** Kao organizator, želim imati pregled logističkih aktivnosti konferencije.
- **Poslovna vrijednost:** Olakšava upravljanje pratećih aktivnosti konferencije.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
  - Da li staviti sve aktivnosti zajedno ili neke izdvojiti?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S29 (Uređivanje konferencija)
- **Acceptance criteria:**
  - Sistem mora omogućiti pregled svih logističkih aktivnosti povezanih sa određenom konferencijom
  - Omogućiti filtriranje po tipu aktivnosti (catering, ručak, video sadržaj i dr.)
  - Klikom na neku od aktivnosti se prikazuje više detalja
 
---

### S46.2 — Dodavanje logističkih aktivnosti

- **ID storyja:** US27.2
- **Naziv storyja:** Dodavanje logističkih aktivnosti
- **Sprint:** 12
- **Opis:** Kao organizator, želim dodati novu logističku aktivnost konferenciji.
- **Poslovna vrijednost:** Omogućava upravljanje pratećih aktivnosti konferencije.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
  - Da li staviti sve vrste aktivnosti zajedno pri kreiranju ili neke izdvojiti?
  - Da li omogućiti kreiranje logističke aktivnosti za više konferencija istovremeno?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S29 (Uređivanje konferencija)
  - Povezano sa S46.2 (Pregled logističkih aktivnosti)
- **Acceptance criteria:**
  - Sistem mora omogućiti kreiranje nove logističke aktivnosti za odabranu konferenciju
  - Sistem daje mogućnost odabira vrste aktivnosti iz liste opcija pri kreiranju
  - Obavezna polja moraju biti popunjena
  - Nova aktivnost se prikazuje u pregledu logističkih aktivnosti za tu konferenciju
 
---

### S46.3 — Uređivanje logističkih aktivnosti

- **ID storyja:** US27.3
- **Naziv storyja:** Uređivanje logističkih aktivnosti
- **Sprint:** 12
- **Opis:** Kao organizator, želim imati mogućnost uređivanja logističkih aktivnosti konferencije.
- **Poslovna vrijednost:** Omogućava upravljanje pratećih aktivnosti konferencije.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
  - Da li omogućiti organizatoru da promijeni kojoj konferenciji je aktivnost dodijeljena?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S29 (Uređivanje konferencija)
  - Povezano sa S46.2 (Pregled logističkih aktivnosti)
- **Acceptance criteria:**
  - Organizator može unijeti nove podatke za ona polja koja želi modificirati   
  - Sistem mora uspješno ažurirati logističku aktivnost
  - Sistem mora spriječiti konflikte pri uređivanju aktivnosti od strane više organizatora

---

### S46.4 — Brisanje logističkih aktivnosti

- **ID storyja:** US27.4
- **Naziv storyja:** Brisanje logističkih aktivnosti
- **Sprint:** 12
- **Opis:** Kao organizator, želim imati mogućnost brisanja logističkih aktivnosti konferencije.
- **Poslovna vrijednost:** Omogućava upravljanje pratećih aktivnosti konferencije.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S29 (Uređivanje konferencija)
  - Povezano sa S46.2 (Pregled logističkih aktivnosti)
  - Povezano sa S46.3 (Uređivanje logističkih aktivnosti)
- **Acceptance criteria:**
  - Sistem mora omogućiti brisanje logističkih aktivnosti povezanih sa određenom konferencijom
  - Sistem mora tražiti potvrdu prije brisanja   
  - Organizator treba dobiti informaciju da je brisanje uspješno 
 
---

### S47.1 — Pregled opreme

- **ID storyja:** US28.1
- **Naziv storyja:** Pregled tehničke opreme
- **Sprint:** 12
- **Opis:** Kao organizator, želim mogućnost pregleda tehničke opreme.
- **Poslovna vrijednost:** Olakšava upravljanje resursima.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S45 (Materijali za konferenciju i sesije)
- **Acceptance criteria:**
  - Sistem mora omogućiti pregled trenutno dostupne teničke opreme
  - Sistem mora omogućiti pregled sve opreme kao i one dodijeljene određenoj konferenciji
  - Klikom na jednu stavku opreme se prikazuje više detalja
 
---

### S47.2 — Kreiranje opreme

- **ID storyja:** US28.2
- **Naziv storyja:** Kreiranje tehničke opreme
- **Sprint:** 12
- **Opis:** Kao organizator, želim mogućnost kreiranja nove tehničke opreme kako bih imao/la evidenciju inventara.
- **Poslovna vrijednost:** Omogućava upravljanje resursima.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Da li omogućiti i uređivanje opreme ili je dovoljno dodavanje/brisanje?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S45 (Materijali za konferenciju i sesije)
- **Acceptance criteria:**
  - Sistem mora omogućiti kreiranje tehničke opreme
  - Organizator navodi koliko odabrane opreme je trenutno dostupno
  - Organizator može povećati dostupan broj odabrane opreme
  - Obavezna polja moraju biti popunjena
  - Sistem dodaje novokreiranu opremu na listu sve tehničke opreme koju organizatori imaju na raspolaganju
 
---

### S47.3 — Brisanje opreme

- **ID storyja:** US28.3
- **Naziv storyja:** Brisanje tehničke opreme
- **Sprint:** 12
- **Opis:** Kao organizator, želim mogućnost brisanja tehničke opreme koja više nije na raspolaganju.
- **Poslovna vrijednost:** Omogućava upravljanje resursima.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Da li omogućiti i uređivanje opreme ili je dovoljno dodavanje/brisanje?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S45 (Materijali za konferenciju i sesije)
- **Acceptance criteria:**
  - Sistem mora omogućiti brisanje tehničke opreme
  - Sistem mora tražiti potvrdu prije brisanja
  - Organizator može smanjiti dostupan broj odabrane opreme
  - Organizator treba dobiti informaciju da je brisanje uspješno
 
---

### S47.4 — Dodjela opreme konferenciji

- **ID storyja:** US28.4
- **Naziv storyja:** Dodjela tehničke opreme konferenciji
- **Sprint:** 12
- **Opis:** Kao organizator, želim mogućnost dodjele tehničke opreme konferencijama kako bih pojednostavio pripremu za konferenciju.
- **Poslovna vrijednost:** Olakšava upravljanje resursima.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
  - Postoji najmanje jedna aktivna konferencija
  - Mora postojati najmanje jedna dostupna jedinica opreme koju je moguće dodijeliti konferenciji
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S45 (Materijali za konferenciju i sesije)
  - Zavisi od S29 (Uređivanje konferencija)
- **Acceptance criteria:**
  - Sistem mora omogućiti dodjelu tehničke opreme konferencijama
  - Moguće je dodijeliti onoliko opreme konferenciji koliko je dostupno
  - Sistem dodaje odabranu opremu na listu tehničke opreme konferencije
 
---

### S48.1 — Postavljanje pitanja za Q&A sesiju

- **ID storyja:** US29.1
- **Naziv storyja:** Postavljanje pitanja za Q&A sesiju
- **Sprint:** 12
- **Opis:** Kao korisnik, želim imati mogućnost slanja pitanja za predavača u toku njegove/njene prezentacije bez ometanja predavača.
- **Poslovna vrijednost:** Omogućava i olakšava interakciju između predavača i učesnika.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem
  - Korisnik je prisutan na predavanju
  - Da li dodati filter koji će spriječiti da zlonamjerna pitanja dođu do predavača?
  - Da li dodati limit za broj pitanja po učesniku?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S40 (Prijava učesnika na sesiju)
- **Acceptance criteria:**
  - Sistem omogućava da učesnici na predavanju mogu postavljati pitanja bez prekidanja predavača
  - Sistem daje link preko kojeg prisutni na predavanju mogu postavljati svoja pitanja
  - Pitanja mogu postaviti samo prisutni na predavanju
 
---

### S48.2 — Prikaz pitanja predavaču

- **ID storyja:** US29.1
- **Naziv storyja:** Prikaz pitanja predavaču
- **Sprint:** 12
- **Opis:** Kao predavač, želim imati prikaz postavljnih pitanja na koja ću odgovoriti pri završetku predavanju.
- **Poslovna vrijednost:** Omogućava i olakšava interakciju između predavača i učesnika.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Da li pitanja stižu direktno predavaču ili organizatoru koji ih pokaže predavaču?
  - Da li predavač mora biti prijavljen na sistem?
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S48.1 (Postavljanje pitanja za Q&A sesiju)
- **Acceptance criteria:**
  - Sistem omogućava prikaz svih postavljenih pitanja
  - Predavač može označiti pitanja kao odgovorena, čime se uklanjaju iz liste
 
---

### S49 — Izvještaji za organizatore

- **ID storyja:** US30
- **Naziv storyja:** Izvještaji za organizatore
- **Sprint:** 12
- **Opis:** Kao organizator, želim dobiti izvještaje o konferenciji sa njenom statistikom.
- **Poslovna vrijednost:** Daje povratnu informaciju organizatoru.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem sa rolom organizatora
- **Veza sa drugim storyjima ili zavisnostima:**
  - Povezano sa S26 (Kreiranje konferencije)
- **Acceptance criteria:**
  - Sistem šalje izvještaje o prijavama, kapacitetima, kotizacijama i drugim relevantnim stavkama organizatorima konferencije
  - Organizator može pregledati izvještaj i preuzeti ga
