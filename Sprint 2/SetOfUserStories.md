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

---

## Grupa 4: Upravljanje konferencijom (S28, S29, S30, S31)

---

### S28 — Detalji konferencije

- **ID storyja:** US-08  
- **Naziv storyja:** Pregled detalja konferencije  
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

## Grupa 5: Sesije i raspored (S32, S33, S34)

---

### S32 — Upravljanje sesijama

- **ID storyja:** US-12  
- **Naziv storyja:** Upravljanje sesijama  
- **Opis:** Kao organizator, želim upravljati sesijama, kako bih organizovao sadržaj konferencije.  
- **Poslovna vrijednost:** Omogućava organizaciju programa konferencije.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Konferencija postoji  
  - Koji podaci su obavezni za sesiju?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Kreiranje konferencije (US-26)  
- **Acceptance criteria:**
  - Kada organizator kreira sesiju, ako su podaci validni, tada se sesija sprema  
  - Sistem mora omogućiti kreiranje, izmjenu i brisanje sesija  
  - Sesije moraju biti povezane sa konferencijom  
  - Sistem ne smije dozvoliti kreiranje bez osnovnih podataka  

---

### S33 — Dodjela predavača sesiji

- **ID storyja:** US-13  
- **Naziv storyja:** Dodjela predavača  
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

## Grupa 6: Prostor i validacija (S35, S36, S37)

---

### S35 — Upravljanje dvoranama

- **ID storyja:** US-15  
- **Naziv storyja:** Upravljanje dvoranama  
- **Opis:** Kao organizator, želim upravljati dvoranama, kako bih organizovao prostor.  
- **Poslovna vrijednost:** Omogućava pravilno upravljanje prostorima.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Sistem podržava unos dvorana  
  - Da li dvorana ima ograničen kapacitet?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Dodjela dvorane (US-16)  
- **Acceptance criteria:**
  - Kada organizator doda dvoranu, ako podaci nisu duplikat, tada se sprema  
  - Sistem mora omogućiti dodavanje i izmjenu dvorana  
  - Sistem ne smije dozvoliti duplikate  
  - Sistem mora omogućiti pregled svih dvorana  

---

### S36 — Dodjela dvorane sesiji

- **ID storyja:** US-16  
- **Naziv storyja:** Dodjela dvorane  
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

### S37 — Validacija konflikata

- **ID storyja:** US-17  
- **Naziv storyja:** Validacija konflikata  
- **Opis:** Kao sistem, želim spriječiti preklapanje termina i prostora, kako bi se izbjegli konflikti.  
- **Poslovna vrijednost:** Sprječava greške i poboljšava organizaciju.  
- **Prioritet:** High  
- **Pretpostavke i otvorena pitanja:**
  - Postoji raspored sesija  
  - Da li sistem treba automatski predložiti drugi termin?  
- **Veze sa drugim storyjima ili zavisnostima:**
  - Upravljanje sesijama (US-12)  
  - Dodjela dvorane (US-16) 
- **Acceptance criteria:**
  - Kada postoji konflikt termina, tada sistem blokira akciju  
  - Sistem mora upozoriti korisnika na konflikt  
  - Sistem ne smije dozvoliti preklapanje sesija u istoj dvorani  
  - Sistem mora osigurati validan raspored  

---

## Grupa 7: Učesnici (S38, S39, S40, S41, S42)

---

### S38 — Prijava učesnika na konferenciju

- **ID storyja:** US-18  
- **Naziv storyja:** Prijava učesnika  
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

- **ID storyja:** US19
- **Naziv storyja:** Odjava učesnika sa konferencije
- **Opis:** Kao učesnik, želim se odjaviti sa konferencije, kako bih oslobodio svoje mjesto.
- **Poslovna vrijednost:** Omogućava preciznu evidenciju dolazaka i automatsko oslobađanje resursa.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:** 
  - Korisnik je prijavljen na konferenciju.
  - Da li postoji rok za odjavu?
  - Da li se vrši povrat novca nakon odjave?
- **Veze sa drugim storyjima ili zavisnostima:**
  - 
- **Acceptance criteria:**
  - Sistem mora omogućiti odjavu sa konferencije.
  - Sistem mora promijeniti status prijave u 'Otkazano'.
  - Sistem mora ažurirati broj slobodnih mjesta.
  - Ako postoji rok za odjavu, sistem ne smije omogućiti odjavu nakon isteka roka.

---

### S40 - Prijava učesnika na sesiju

- **ID storyja:** US20
- **Naziv storyja:** Prijava učesnika na sesiju
- **Opis:** Kao učesnik, želim se prijaviti na pojedinačne sesije konferencije, kako bih prisustvovao temama koje me zanimaju.
- **Poslovna vrijednost:** Omogućava bolju organizaciju sesija.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:** 
  - Korisnik je prethodno prijavljen na konferenciju i sesije su već definisane.
- **Veze sa drugim storyjima ili zavisnostima:**
  - 
- **Acceptance criteria:**
  - Kada postoji slobodno mjesto, ako se korisnik prijavi, tada se registruje na sesiju.
  - Sistem mora smanjiti broj dostupnih mjesta.
  - Sistem ne smije dozvoliti prijavu ako je sesija popunjena.
  - Sistem ne smije dozvoliti prijavu na vremenski preklapajuće sesije istom korisniku.
  - Korisnik treba dobiti potvrdu o prijavi.

---  

### S41 - Pregled popunjenosti kapaciteta

- **ID storyja:** US21
- **Naziv storyja:** Pregled popunjenosti kapaciteta
- **Opis:** Kao organizator ili učesnik, želim vidjeti broj prijavljenih u odnosu na kapacitet, kako bih imao uvid u dostupnost mjesta.
- **Poslovna vrijednost:** Pomaže u boljem upravljanju resursima.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:** 
  - Da li je ovaj pregled javan ili samo za administratore/organizatore doagađaja?
- **Veze sa drugim storyjima ili zavisnostima:**
  - 
- **Acceptance criteria:**
  - Kada postoji konferencija ili sesija, sistem mora prikazati broj prijavljenih.
  - Sistem mora prikazati maksimalni kapacitet.
  - Sistem mora prikazati da li je kapacitet popunjen.
  - Prikaz se mora ažurirati nakon svake prijave ili odjave.

---

### S42 - Lista učesnika po konferenciji

- **ID storyja:** US22
- **Naziv storyja:** Lista učesnika po konferenciji
- **Opis:** Kao organizator, želim vidjeti listu prijavljenih učesnika, kako bih imao pregled i mogao upravljati događajem.
- **Poslovna vrijednost:** Omogućava lakšu organizaciju i administraciju konferencije.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:** 
  - Koje informacije o učesnicima su vidljive?
- **Veze sa drugim storyjima ili zavisnostima:**
  - 
- **Acceptance criteria:**
  - Kada organizator otvori konferenciju, sistem mora prikazati listu učesnika
  - Sistem mora prikazati osnovne podatke (ime, email, ...)
  - Sistem mora omogućiti pretragu ili filtriranje liste
  - Sistem ne smije dozvoliti pristup listi neautorizovanim korisnicima

---

### S43 - Upravljanje kotizacijama

- **ID storyja:** US23
- **Naziv storyja:** Upravljanje kotizacijama
- **Opis:** Kao organizator, želim definisati kategorije i iznose kotizacija, kako bih upravljao naplatom učešća.
- **Poslovna vrijednost:** Omogućava kontrolu plaćanja.
- **Prioritet:** High
- **Pretpostavke i otvorena pitanja:** 
  - 
- **Veze sa drugim storyjima ili zavisnostima:**
  - 
- **Acceptance criteria:**
  - Sistem mora omogućiti kreiranje kategorija kotizacija
  - Sistem mora omogućiti unos iznosa
  - Sistem mora evidentirati status (plaćeno / neplaćeno)
  - Sistem mora omogućiti izmjenu i brisanje kotizacija

---

### S44 - Obavijesti za korisnike

- **ID storyja:** US24
- **Naziv storyja:** Obavijesti za korisnike
- **Opis:** Kao korisnik, želim primati obavijesti iz sistema, kako bih bio informisan o važnim događajima.
- **Poslovna vrijednost:** Povećava informisanost i poboljšava korisničko iskustvo.
- **Prioritet:** Medium
- **Pretpostavke i otvorena pitanja:** 
  - Da li su obavijesti samo u aplikaciji ili i email?
- **Veze sa drugim storyjima ili zavisnostima:**
  - 
- **Acceptance criteria:**
  - Kada se desi važan događaj, sistem mora poslati obavijest
  - Sistem mora prikazati obavijesti u korisničkom interfejsu
  - Korisnik treba dobiti jasnu i razumljivu poruku
  - Korisnik treba imati mogućnost pregleda prethodnih obavijesti

---

### S45 — Materijali za konferenciju i sesije

- **ID storyja:** US25
- **Naziv storyja:** Pregled materijala za konferenciju i sesije
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
 

- **ID storyja:** US26
- **Naziv storyja:** Dodavanje materijala za konferenciju i sesije
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
  - Sistem mora omogućiti dodavanje novog materijala konferenciji/sesiji
  - Novi materijal se prikazuje na spisku dodanih materijala konferencije/sesije

---

### S46 — Materijali za konferenciju i sesije

- **ID storyja:** US25
- **Naziv storyja:** Pregled materijala za konferenciju i sesije
- **Opis:** Kao korisnik, želim .
- **Poslovna vrijednost:** .
- **Prioritet:** 
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od
  - Povezano sa
- **Acceptance criteria:**
  - Sistem mora
 
---

### S47 — Materijali za konferenciju i sesije

- **ID storyja:** US25
- **Naziv storyja:** Pregled materijala za konferenciju i sesije
- **Opis:** Kao korisnik, želim .
- **Poslovna vrijednost:** .
- **Prioritet:** 
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od
  - Povezano sa
- **Acceptance criteria:**
  - Sistem mora
 
---

### S48 — Materijali za konferenciju i sesije

- **ID storyja:** US25
- **Naziv storyja:** Pregled materijala za konferenciju i sesije
- **Opis:** Kao korisnik, želim .
- **Poslovna vrijednost:** .
- **Prioritet:** 
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od
  - Povezano sa
- **Acceptance criteria:**
  - Sistem mora
 
---
### S49 — Materijali za konferenciju i sesije

- **ID storyja:** US25
- **Naziv storyja:** Pregled materijala za konferenciju i sesije
- **Opis:** Kao korisnik, želim .
- **Poslovna vrijednost:** .
- **Prioritet:** 
- **Pretpostavke i otvorena pitanja:**
  - Korisnik je prijavljen na sistem
- **Veza sa drugim storyjima ili zavisnostima:**
  - Zavisi od
  - Povezano sa
- **Acceptance criteria:**
  - Sistem mora
