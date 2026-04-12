# Cilj testiranja

Cilj testiranja sistema za organizaciju konferencija je osigurati da sve funkcionalnosti sistema rade ispravno, pouzdano i sigurno u skladu sa definisanim zahtjevima i acceptance kriterijima.

| Cilj| Obim|Kriterij uspjeha|
|----------|-----------|--------|
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |

# Nivoi testiranja

## Unit testiranje
Unit testiranje se provodi na nivou pojedinačnih komponenti i funkcija sistema, izolovano od ostalih dijelova sistema.

Obuhvata testiranje:
- Validacione logike za registraciju korisnika (format emaila, jačina lozinke, obavezna polja)
- Business logike za provjeru duplikata (email adrese, nazivi dvorana, preklapanje termina sesija)
- Funkcija za upravljanje kapacitetima dvorana i sesija
- Logike za izračun popunjenosti kapaciteta i ažuriranje slobodnih mjesta
- Validacije kotizacija (iznosi, kategorije, status plaćanja)
- Funkcija za generisanje obavijesti

**Odgovorne osobe:** Razvojni tim

**Izlazni kriterij:** 
- 100% unit testova prolazi (0 failova)
- Pokrivenost koda (code coverage) ≥ 90% za business logiku
- Svi edge case-ovi za validaciju pokriveni testovima

## Integraciono testiranje
Integraciono testiranje provjerava ispravnu komunikaciju između modula sistema i ispravnost toka podataka između komponenti.

Ključne integracije koje se testiraju:
- Integracija autentifikacije i autorizacije — provjera da role ispravno ograničavaju pristup svim zaštićenim endpointima
- Integracija modula konferencije i sesija — provjera da sesije ispravno nasljeđuju kontekst konferencije
- Integracija dvorane i sesije — provjera da dodjela dvorane ispravno ažurira kapacitet i sprječava konflikt
- Integracija prijave učesnika i ažuriranja kapaciteta — provjera da se broj mjesta ažurira u realnom vremenu
- Integracija kotizacija i statusa prijave učesnika
- Integracija sistema obavijesti sa okidačkim događajima (prijava, promjena statusa, brisanje)
- Integracija Q&A modula sa sesijama i korisničkim profilima

**Odgovorne osobe:** Dev tim & QA

**Izlazni kriterij:**  
- Svi kritični integracioni tokovi prolaze bez grešaka
- Nema otvorenih defekata vezanih za komunikaciju između modula
- API response time unutar definisanog praga (< 3 sekunde za CRUD operacije)


## Sistemsko testiranje
Sistemsko testiranje provjerava ponašanje cjelokupnog sistema kao integrisane cjeline, uključujući end-to-end tokove i ne-funkcionalne zahtjeve.

Obuhvata:
- End-to-end testiranje kompletnog toka: registracija → prijava → kreiranje konferencije → dodavanje sesija → prijava učesnika → Q&A → izvještaj
- Testiranje sigurnosti: provjera da neautorizovani korisnici ne mogu pristupiti zaštićenim resursima (direktnim URL-om, manipulacijom zahtjeva)
- Testiranje konkurentnog pristupa: istovremena prijava više učesnika na sesiju sa ograničenim kapacitetom
- Testiranje konzistentnosti podataka nakon brisanja (konferencija, sesija, dvorane)
- Testiranje performansi pri pregledu lista sa većim brojem zapisa
- Testiranje integriteta sesije nakon odjave korisnika

**Odgovorne osobe:** QA

**Izlazni kriterij:** 
- Svi end-to-end scenariji prolaze
- Svi sigurnosni testovi prolaze (neovlašteni pristup blokiran u 100% slučajeva)
- Performansni testovi unutar prihvatljivih granica
- Konzistentnost podataka potvrđena nakon brisanja

  
## Prihvatno testiranje
Prihvatno testiranje se provodi na osnovu acceptance kriterija definisanih u user stories. Cilj je potvrditi da sistem ispunjava poslovne zahtjeve i da je spreman za puštanje u produkciju.

Ovo testiranje obuhvata verifikaciju svakog acceptance kriterija za sve user storije, uz sudjelovanje vlasnika proizvoda ili krajnjih korisnika u finalnom pregledu.

**Odgovorne osobe:** Product Owner, krajnji korisnici (organizator, predavač, učesnik), QA

**Izlazni kriterij:** 
- Više od 90% acceptance kriterija verifikovano kao zadovoljeno
- Nema otvorenih Critical ili High defekata
- Product Owner dao formalno odobrenje
- Sva otvorena pitanja iz user storija riješena ili svjesno odložena 



# Šta se testira u kojem nivou

| Funkcionalnost|Unit testiranje|Integraciono|Sistemsko|Prihvatno|
|----------|-----------|--------|-----------|--------|
|Registracija korisnika (US-01)|DA - vallidacija obaveznih polja, format emaila, jedinstvenost emaila, minimalna dužina lozinke|DA - upis u bazu, provjera duplikata na DB nivou|DA - forma → potvrda na ekranu, blokada duplikata, redirect na login|DA - potvrda za sve korisničke role|
|Prijava i odjava (US-02, US-03)|DA - autentifikacija kredencijala, hash provjere, generička poruka greške|DA - login API, baza korisnika i zaštita ruta, invalidacija sesije pri odjavi|DA - login → redirect na početnu; logout → blokada zaštićenih ruta, provjera povratka browserom|DA - potvrda za sve korisničke role|
|Upravljanje korisničkim profilom (US-04, US-04.1, US-04.2)|DA - format emaila, jedinstvenost, pravila definisanja lozinke, zabrana praznih obaveznih polja|DA - PATCH /profile, API i baza za ažuriranje podataka; potvrda lozinke pri promjeni|DA - korisnik mijenja podatke i dobija potvrdu; blokada duplikat emaila|NE - niži prioritet za UAT|
|Korisničke role i permisije (US-05)|DA - provjera svake role (učesnik, organizator, admin), zabrana pristupa van korisničke role|DA - autorizacijski middleware na svakom zaštićenom endpointu|DA - direktan pristup URL-u zaštićene stranice kao neovlašteni korisnik → HTTP 403|DA - potvrda za sve korisničke role|
|Kreiranje konferencije (US-06)|DA - validacija obaveznih polja (naziv, datum, lokacija), zabrana kreiranja bez organizator role|DA - POST /conferences, upis u bazu, provjera role na API nivou|DA - forma → kreirana konferencija → vidljiva u listi, blokada za učesnike|DA - ključna MVP funkcionalnost|
|Pregled i pretraga konferencija (US-07, US-08, US-11)|DA - logika filtriranja i pretrage, prikaz prazne liste|	DA - GET /conferences, GET /conferences/:id, query parametri pretrage|DA - korisnik vidi sve aktivne konferencije, klik na detalje, prazna lista s porukom, pretraga vraća relevantne rezultate|DA - bitno za otkrivanje konferencija|
|Uređivanje i brisanje konferencije (US-09, US-10)|DA - validacija izmijenjenih polja, potvrda brisanja, zabrana neautorizovanih izmjena|DA - PUT/PATCH /conferences/:id, DELETE /conferences/:id, kaskadne provjere|DA - organizator mijenja podatke i klikne na potvrdu, brisanje gdje konferencija nestaje iz liste, blokada za učesnike|DA - kritično za upravljanje sadržajem|
|Upravljanje sesijama (CRUD) (US-12.1, US-12.2, US-12.3, US-12.4)|DA - obavezna polja sesije, zabrana duplikata termina u istoj konferenciji, validacija izmjena|DA - CRUD endpointi za sesije, provjera veze sa konferencijom, zabrana konfliktnih termina na DB nivou|DA - kreiranje → sesija vidljiva u rasporedu, brisanje → seija nestaje iz rasporeda,  već postojeći termin → greška|DA - core programska funkcionalnost|
|Dodjela predavača i dvorane sesiji (US-13, US-16)|DA - provjera postojanja predavača/dvorane; zabrana dodjele nepostojećih entiteta|DA - PATCH /sessions/:id/speaker, /room; FK provjere u bazi|DA - dodjela vidljiva u sesiji; pokušaj dodjele nepostojeće dvorane → greška|DA - bitno za kompletnost rasporeda|
|Pregled rasporeda konferencije (US-14)	|NE|DA - GET /schedule, ispravan redoslijed sesija po terminu|DA - korisnik vidi sve sesije sortirane po vremenu; ne prikazuju se obrisane sesije|DA - planiranje prisustva|
|Upravljanje dvoranama (CRUD) (US-15.1, US-15.2, US-15.3, US-15.4)|DA - zabrana naziva dvorane koji je duplikat; blokada brisanja dvorane dodijeljene aktivnoj sesiji|DA - CRUD endpointi za dvorane; referentni integritet pri brisanju|DA - dodavanje → dvorana u listi; brisanje aktivne dvorane → greška s porukom|DA - upravljanje prostorom|
|Prijava učesnika na konferenciju i odjava (US-18, US-19)|DA - zabrana duple prijave; ažuriranje statusa pri odjavi; provjera roka odjave|DA - POST /registrations, DELETE/PATCH; ažuriranje slobodnih mjesta u bazi|DA - prijava → potvrda; duplikat → greška; odjava → ažuriran kapacitet|DA - osnovna funkcionalnost učesnika|
|Prijava na sesiju i kapacitet (US-20, US-21)|DA - zabrana preklapajućih sesija; blokada popunjene sesije; real-time ažuriranje slobodnih mjesta|DA - POST /session-registrations; transakcijska provjera kapaciteta; GET /capacity|DA - istovremena prijava više korisnika → kapacitet se ne prekorači; popunjena sesija → greška	|DA - kritično za organizaciju|
|Lista učesnika i popunjenost (US-22)|NE|DA - GET /conferences/:id/participants; provjera autorizacije (samo organizator)|DA - organizator vidi listu; učesnik ne može pristupiti; pretraga/filter liste|NE - niži prioritet za UAT|
|Upravljanje kotizacijama (US-23.1, US-23.2)|DA - validacija iznosa; veza kategorije i iznosa; status plaćeno/neplaćeno|DA - CRUD kotizacija; veza sa prijavom učesnika|DA - kreiranje kategorije i iznosa (postaje vidljivo), izmjena (ažurirano)|DA - finansijska funkcionalnost|
|Obavijesti za korisnike (US-24)|DA - okidačka logika za sve predviđene događaje, ispravno primanje obavijesti|DA - integracija servisa za obavijesti sa svim modulima koji okidaju obavijest|DA - prijava na konferenciju → obavijest stigne ispravnom korisniku, pregled historije obavijest|	DA - korisničko iskustvo|Obavijesti za korisnike (US-24)	DA - okidačka logika za sve predviđene događaje; ispravan primatelj obavijesti	DA - integracija notification servisa sa svim modulima koji okidaju obavijest	DA - prijava na konferenciju → obavijest stigne ispravnom korisniku; pregled historije	DA - korisničko iskustvo|
|Materijali za konferenciju i sesije (US-25, US-26)|	DA - obavezna polja pri dodavanju; validacija tipa materijala|	DA - GET/POST /materials; veza sa konferencijom/sesijom	|DA - dodavanje → materijal vidljiv u listi; filter i pretraga materijala|	NE - niži prioritet za UAT|
|Logističke aktivnosti (CRUD) (US-27.1, US-27.2, US-27.3, US-27.4)|	DA - validacija obaveznih polja; provjera konfliktnog uređivanja od više organizatora	|DA - CRUD /logistics; zabrana konkurentnih izmjena na DB nivou	|DA - kreiranje → vidljivo; filter po tipu aktivnosti; brisanje	|DA - operativna logistika|
|Tehnička oprema (CRUD + dodjela) (US-28.1, US-28.2, US-28.3, US-28.4)	|DA - validacija količine; zabrana dodjele više od dostupnog; ažuriranje dostupnog broja	|DA - CRUD /equipment; POST /conferences/:id/equipment; provjera dostupnosti u bazi|	DA - dodjela → dostupan broj se smanjuje; pokušaj dodjele više od dostupnog → greška	|DA - upravljanje resursima|
|Q&A sesija (US-29.1, US-29.2)	|DA - validacija pitanja; provjera da korisnik ima pristup sesiji; označavanje odgovorenih pitanja|	DA - POST /qa/questions; GET /qa; veza sa sesijom; real-time prikaz|	DA - učesnik šalje pitanje → vidljivo predavaču; predavač označava odgovoreno → uklanja se iz liste|	DA - interakcija predavač-učesnik|
|Izvještaji za organizatore (US-30)	|NE|DA - GET /reports/:conferenceId; agregacija prijava, kapaciteta i kotizacija|	DA - organizator vidi statistiku konferencije; preuzimanje izvještaja|	DA - povratna informacija organizatoru|

# Veza sa acceptance kriterijima

|User Story|Acceptance Criteria| ID testnog slučaja|
|----------|-------------------|-------------------|
|US-01: Sign Up|Kada korisnik uspješno unese sve podatke i potvrdi registraciju, korisnik treba dobiti potvrdu o uspješnoj registraciji na ekranu.|TC-01|
|US-01: Sign Up|Sistem ne smije dozvoliti završetak registracije bez popunjenih svih obaveznih polja.|TC-02|
|US-01: Sign Up|Sistem ne smije dozvoliti registraciju sa već postojećom email adresom|TC-03|
|US-02: Sign In|Kada korisnik unese ispravne kredencijale i klikne "Prijavi se", korisnik treba biti preusmjeren na početnu stranicu sistema.|TC-04|
|US-02: Sign In	|Sistem ne smije dozvoliti pristup sistemu korisniku koji nije registrovan.|TC-04|
|US-02: Sign In|Sistem ne smije dozvoliti prijavu sa pogrešnom kombinacijom emaila i lozinke|TC-05|
|US-03: Log Out|Kada korisnik klikne na dugme za odjavu, korisnik treba biti preusmjeren na stranicu za prijavu.|TC-06|
|US-03: Log Out|Sistem ne smije dozvoliti pristup zaštićenim stranicama nakon odjave.|TC-07|
|US-03: Log Out|Sistem mora poništiti korisničku sesiju nakon odjave, tako da povratak na prethodnu stranicu putem browsera ne otvara zaštićeni sadržaj.|TC-08|
|US-04.1: Pregled korisničkog profila|Sistem prikazuje trenutne podatke korisničkog profila: ime, prezime, email|TC-09|
|US-04.2: Izmjena korisničkog profila|Kada korisnik izmijeni podatke i sačuva promjene, korisnik treba dobiti potvrdu o uspješnoj izmjeni.|TC-10|
|US-04.2: Izmjena korisničkog profila|Sistem mora tražiti potvrdu trenutne lozinke pri promjeni lozinke.|TC-11|
|US-05: Korisničke role i permisije|Kada korisniku bude dodijeljena određena rola, korisnik treba imati pristup samo onim funkcionalnostima koje su predviđene za tu rolu.|TC-12|
|US-05: Korisničke role i permisije|Sistem mora onemogućiti pristup administratorskim funkcionalnostima svim korisnicima koji nemaju admin rolu.|TC-13|
|US-06: Kreiranje konferencije|Kada organizator popuni sve obavezne podatke i potvrdi kreiranje, korisnik treba dobiti potvrdu o uspješnom kreiranju konferencije. |TC-14|
|US-06: Kreiranje konferencije | Sistem ne smije dozvoliti kreiranje konferencije bez popunjenih obaveznih polja.| TC-15|
|US-06: Kreiranje konferencije | Sistem ne smije dozvoliti kreiranje konferencije korisniku koji nema rolu organizatora.| TC-16|
|US-07: Pregled konferencija |Sistem mora prikazati listu svih dostupnih konferencija sa osnovnim informacijama (naziv, datum, lokacija). | TC-17|
|US-07: Pregled konferencija | Sistem mora prikazati jasnu poruku kada nema dostupnih konferencija.| TC-18|
|US-07: Pregled konferencija| Sistem ne smije prikazivati konferencije koje su obrisane ili deaktivirane.| TC-19|
|US-08: Pregled detalja konferencije |Kada korisnik klikne na konferenciju, ako konferencija postoji, tada vidi sve detalje. | TC-20|
|US-09: Uređivanje konferencije |Kada organizator izmijeni podatke, ako su svi obavezni podaci uneseni, tada se promjene spremaju | TC-21|
|US-10: Brisanje konferencije |Kada organizator klikne na brisanje, ako potvrdi akciju, tada se konferencija briše | TC-22|
|US-10: Brisanje konferencije | Korisnik treba dobiti informaciju da je brisanje uspješno| TC-23|
|US-11: Pretraga konferencija |Kada korisnik unese pojam, ako postoje rezultati, tada sistem prikazuje odgovarajuće konferencije | TC-24|
|US-11: Pretraga konferencija |Sistem mora prikazati samo relevantne konferencije | TC-25|
|US-11: Pretraga konferencija |Kada nema rezultata, sistem treba prikazati poruku | TC-26|
|US-12.1: Pregled sesija konferencije |Kada organizator otvori sesije, tada vidi listu svih sesija za konferenciju | TC-27|
| US-12.1: Pregled sesija konferencije|Sistem mora prikazati osnovne podatke o sesiji | TC-28|
|US-12.1: Pregled sesija konferencije |Kada nema sesija, tada sistem prikazuje odgovarajuću poruku| TC-29|
|US-12.2: Kreiranje sesije | Sistem ne smije dozvoliti kreiranje sesije sa istim terminom u istoj konferenciji| TC-30|
|US-12.2: Kreiranje sesije  |Sistem ne smije dozvoliti kreiranje bez obaveznih podataka | TC-31|
|US-12.2: Kreiranje sesije | Kada organizator unese validne podatke, tada sistem sprema sesiju| TC-32|
|US-12.3: Uređivanje sesije| Sistem ne smije dozvoliti nevalidne izmjene| TC-33|
|US-12.4: Brisanje sesije | Kada organizator izvrši brisanje sesije, tada se ona uklanja iz sistema| TC-34|
|US-13: Dodjela predavača sesiji |Sistem ne smije dozvoliti dodjelu nepostojećeg korisnika | TC-35|
|US-13: Dodjela predavača sesiji |Kada organizator dodijeli predavača, ako predavač postoji, tada se prikazuje u sesiji | TC-36|
|US-14: Pregled rasporeda | Sistem ne smije prikazivati nepostojeće sesije| TC-37|
|US-15.1: Pregled dvorana |Kada nema dvorana, tada sistem prikazuje odgovarajuću poruku | TC-38|
|US-15.2: Dodavanje dvorane| Sistem ne smije dozvoliti unos duplikata dvorane| TC-39|
|US-15.2: Dodavanje dvorane |Organizator treba dobiti potvrdu o uspješnom dodavanju | TC-40|
|US-15.4: Brisanje dvorane| Sistem ne smije dozvoliti brisanje dvorane koja je dodijeljena aktivnoj sesiji| TC-41|
|US-16: Dodjela dvorane|Kada organizator dodijeli dvoranu, ako postoji, tada se povezuje sa sesijom | TC-42|
|US-18: Prijava učesnika na konferenciju| Korisnik treba dobiti potvrdu o uspješnoj prijavi| TC-43|
|US-18: Prijava učesnika na konferenciju|Kada korisnik klikne prijavu, ako nije već prijavljen, tada se registruje | TC-44|
|US-19: Odjava učesnika s konferencije |Sistem mora promijeniti status prijave u 'Otkazano'. | TC-45|
|US-19: Odjava učesnika s konferencije| Sistem mora ažurirati broj slobodnih mjesta.| TC-46|
|US-20: Prijava učesnika na sesiju|Sistem ne smije dozvoliti prijavu ako je sesija popunjena.|TC-47|
|US-21: Pregled popunjenosti kapaciteta |Kada postoji konferencija ili sesija, sistem mora prikazati broj prijavljenih. | TC-48|
|US-21: Pregled popunjenosti kapaciteta |Prikaz se mora ažurirati nakon svake prijave ili odjave. | TC-49|
|US-21: Pregled popunjenosti kapaciteta|Sistem mora prikazati maksimalni kapacitet. | TC-50|
|US-22: Lista učesnika po konferenciji | Sistem ne smije dozvoliti pristup listi neautorizovanim korisnicima| TC-51|
|US-23.1: Upravljanje kategorijama kotizacija |Sistem mora omogućiti izmjenu kategorija | TC-52|
|US-23.1: Upravljanje kategorijama kotizacija|Sistem mora omogućiti kreiranje kategorija kotizacija | TC-53|
|US-23.1: Upravljanje kategorijama kotizacija |Sistem mora omogućiti brisanje kategorija | TC-54|
| US-23.2: Upravljanje iznosima kotizacija|Sistem mora evidentirati status (plaćeno / neplaćeno) | TC-55|
|US-23.2: Upravljanje iznosima kotizacija | Sistem mora omogućiti unos iznosa kotizacije| TC-56|
|US-23.2: Upravljanje iznosima kotizacija |Sistem mora povezati iznos sa kategorijom | TC-57|
|US-23.2: Upravljanje iznosima kotizacija |Sistem mora omogućiti izmjenu iznosa | TC-58|
|US-24: Obavijesti za korisnike |Kada se desi važan događaj, sistem mora poslati obavijest| TC-59|
|US-24: Obavijesti za korisnike |Korisnik treba imati mogućnost pregleda prethodnih obavijesti | TC-60|
|US25: Pregled materijala za konferenciju i sesije |Mogućnost pretrage, filtriranja, sortiranja | TC-61|
|US25: Pregled materijala za konferenciju i sesije |Sistem mora prikazati sve trenutno dodane materijale za odabranu konferenciju/sesiju | TC-62|
|US25: Pregled materijala za konferenciju i sesije|Klikom na neki od materijala se prikazuje više detalja| TC-63|
|US27.1: Pregled logističkih aktivnosti |Omogućiti filtriranje po tipu aktivnosti (catering, ručak, video sadržaj i dr.) | TC-64|
|US27.2: Dodavanje logističkih aktivnosti  |Sistem daje mogućnost odabira vrste aktivnosti iz liste opcija pri kreiranju| TC-65|
|US28.4:Dodjela tehničke opreme konferenciji |Moguće je dodijeliti onoliko opreme konferenciji koliko je dostupno | TC-66|
|US29.1: Postavljanje pitanja za Q&A sesiju |Sistem omogućava da učesnici na predavanju mogu postavljati pitanja bez prekidanja predavača | TC-67|
|US29.2: Prikaz pitanja predavaču| Predavač može označiti pitanja kao odgovorena, čime se uklanjaju iz liste| TC-68|
|US30: Izvještaji za organizatore|Organizator može pregledati izvještaj i preuzeti ga|TC-69|




# Način evidentiranja rezultata testiranja

### Test case dokumentacija
Za svaki user story se definišu test case-ovi koji obuhvataju:
- Identifikator test case-a (TC-[redni broj])
- Opis test scenarija i preduslove 
- Korake izvršavanja
- Očekivane i stvarne rezultate
- Status: Passed / Failed / Blocked / Skipped
- Prioritet: Critical / High / Medium / Low
- Vezu sa acceptance kriterijem kojeg pokriva

### Evidencija defekta
Svaki pronađeni defekt se evidentira sa sljedećim informacijama:
- Jedinstveni ID defekta
- Naziv i opis defekta
- Koraci za reprodukciju
- Očekivano i stvarno ponašanje
- Severity: Critical / Major / Minor / Trivial
- Priority: High / Medium / Low
- Status: New / In Progress / Fixed / Verified / Closed
- Veza sa test case-om i user storyjem
- Screenshot ili log kao prilog (gdje je primjenljivo)

### Izvještaj o testiranju
Po završetku svake faze testiranja generiše se Test Report koji sadrži:
- Sažetak rezultata: ukupan broj test case-ova, Passed/Failed/Blocked
- Tabelu statusa po user storyjima i nivoima testiranja
- Listu otvorenih defekata sa severity/priority klasifikacijom
- Pokrivenost acceptance kriterija - postotak
- Preporuku: Go / No-Go za puštanje u produkciju


# Glavni rizici kvaliteta
| ID |Opis rizika |Vjerovatnoća |Uticaj| Strategija mitigacije|
|----|-----------|--------|-----------|--------|
|R01 |Neispravna implementacija rolno-baziranog pristupa omogućava neovlaštene akcije|Srednja |Visok |Detaljno testiranje svakog zaštićenog endpoint-a za svaku rolu|
|R02 |Race condition pri istovremenom prijavljivanju više učesnika na sesiju s ograničenim kapacitetom|Visoka| Visok|Konkurentno testiranje, provjera da se kapacitet ne prekorači ni u jednom scenariju |
|R03 |Sesija korisnika ostaje aktivna nakon odjave, pa je omogućen neovlašteni pristup|Niska|Visok|Testiranje invalidacije sesije i provjera zaštićenih ruta nakon odjave|
|R04 |Prikaz podataka jednog korisnika drugom korisniku|Niska|Visok|Testiranje izolacije podataka: provjera da korisnik vidi samo svoje podatke|
|R05 |Konflikti pri uređivanju iste konferencije/sesije od strane više organizatora| Srednja| Srednji|Testiranje konkurentnog uređivanja; provjera mehanizma za rješavanje konflikata (US-27.3)|
|R06 |Q&A pitanja dostupna korisnicima koji nisu prisutni na predavanju|Niska|Srednji|Testiranje autorizacije za Q&A, provjera da link funkcioniše isključivo u kontekstu sesije|