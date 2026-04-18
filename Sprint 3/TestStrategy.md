# Cilj testiranja

Cilj testiranja sistema za organizaciju konferencija je osigurati da sve funkcionalnosti sistema rade ispravno, pouzdano i sigurno u skladu sa definisanim zahtjevima i acceptance kriterijima.

| Cilj| Obim|Kriterij uspjeha|
|----------|-----------|--------|
|  Verifikacija ispravnosti toka autentifikacije i sigurnosti korisničke sesije (US-01, US-02, US-03)|Registracija, prijava i odjava za sve korisničke role (učesnik, organizator, administrator); invalidacija sesije i blokada zaštićenih ruta|Svi AC iz US-01, US-02 i US-03 zadovoljeni; neautorizovan pristup zaštićenim stranicama je onemogućen; povratak browserom nakon odjave ne otvara zaštićeni sadržaj|
|Provjera ispravnosti rolno-baziranog pristupa za sve korisničke role (US-05)|Pristup rutama, UI elementima i API endpointima za role: učesnik, organizator, administrator; direktan URL pristup kao neovlašteni korisnik|Korisnik ne može pristupiti funkcionalnostima van svoje role; svaki zahtjev prolazi provjeru permisija; HTTP 403 ili redirect za neovlaštene zahtjeve|
|Validacija upravljanja korisničkim profilom i izolacije podataka (US-04, US-04.1, US-04.2)|Pregled i izmjena profila (ime, prezime, email, lozinka); zabrana emaila koji je duplikat; potvrda lozinke pri promjeni; izolacija podataka po korisniku|Korisnik vidi isključivo vlastite podatke; izmjene se snimaju i odmah prikazuju; duplikat email i prazna polja su blokirani; promjena lozinke zahtijeva potvrdu|
|Validacija kompletne konferencije (US-06, US-07, US-08, US-09, US-10, US-11)|Kreiranje, pregled, detalji, uređivanje, brisanje i pretraga konferencija; vidljivost samo aktivnih konferencija; autorizacija na svim operacijama|Konferencija se kreira, prikazuje, ažurira i briše ispravno; obrisane/deaktivirane konferencije nisu vidljive; pretraga vraća samo relevantne rezultate; neovlašteni korisnici ne mogu vršiti izmjene|
|Provjera upravljanja sesijama, rasporedom i dodjele predavača (US-12.1, US-12.2, US-12.3, US-12.4, US-13, US-14)|CRUD operacije nad sesijama; zabrana duplikat termina u istoj konferenciji; dodjela predavača; prikaz rasporeda sesija sortiranog po vremenu|Sesija se ne može kreirati sa konfliktnim terminom; dodjela nepostojećeg predavača je blokirana; raspored prikazuje sve sesije tačno po terminu bez obrisanih stavki|
|Validacija upravljanja dvoranama i dodjele dvorane sesiji (US-15.1, US-15.2, US-15.3, US-15.4, US-16)|CRUD operacije nad dvoranama; zabrana duplikata naziva; blokada brisanja dvorane dodijeljene aktivnoj sesiji; dodjela dvorane sesiji|Duplikat naziva dvorane je blokiran; brisanje aktivne dvorane vraća grešku s porukom; dodjela nepostojeće dvorane je odbijena; promjene su odmah vidljive|
|Provjera toka prijave i odjave učesnika uz real-time ažuriranje kapaciteta (US-18, US-19, US-20, US-21)|Prijava na konferenciju i sesiju; odjava sa konferencije; zabrana duplikata i preklapajućih sesija; real-time ažuriranje slobodnih mjesta; prikaz popunjenosti|Istovremena prijava više korisnika ne prekoračuje kapacitet; prijava na popunjenu sesiju je blokirana; preklapajuće sesije nisu dozvoljene; kapacitet se ažurira nakon svake promjene|
|Validacija upravljanja učesnicima i kotizacijama (US-22, US-23.1, US-23.2)|Lista učesnika po konferenciji s pretragom i filterom; autorizacija pristupa listi; kreiranje, izmjena i brisanje kategorija i iznosa kotizacija; evidencija statusa plaćanja|Lista učesnika vidljiva samo organizatoru; korisnik (učesnik) nema pristup; kategorije i iznosi se korektno kreiraju, mijenjaju i brišu; status plaćeno/neplaćeno se evidentira|
|Validacija upravljanja materijalima i logističkim aktivnostima (US-25, US-26, US-27.1, US-27.2, US-27.3, US-27.4)|Pregled, dodavanje materijala za konferencije i sesije; CRUD logističkih aktivnosti; filtriranje po tipu aktivnosti; sprječavanje konkurentnih konflikata pri uređivanju|Materijali i aktivnosti se korektno dodaju, prikazuju i brišu; filter po tipu aktivnosti radi ispravno; sistem blokira konflikt pri istovremenom uređivanju od strane više organizatora|
|Provjera upravljanja tehničkom opremom i dodjele konferenciji (US-28.1, US-28.2, US-28.3, US-28.4)|Pregled, kreiranje i brisanje tehničke opreme; upravljanje dostupnom količinom; dodjela opreme konferenciji uz provjeru dostupnosti| Nije moguće dodijeliti više opreme nego što je dostupno; dostupan broj se ažurira nakon dodjele ili brisanja; novokreirana oprema je odmah vidljiva na listi|
|Validacija Q&A funkcionalnosti za interakciju učesnika i predavača (US-29.1, US-29.2)|Postavljanje pitanja bez prekidanja predavača; pristup samo kroz validan link za prisutne; prikaz svih pitanja predavaču; označavanje pitanja kao odgovorenih|Pitanja mogu postavljati samo korisnici s validnim pristupom sesiji; sva pitanja su vidljiva predavaču; označeno pitanje se uklanja iz aktivne liste|
|Provjera generisanja i preuzimanja izvještaja za organizatore (US-30)|Izvještaji o prijavama, kapacitetima i kotizacijama konferencije; pregled unutar sistema; preuzimanje izvještaja|Izvještaj sadrži tačne podatke o prijavama, kapacitetima i kotizacijama; preuzimanje funkcioniše korektno; dostupan isključivo organizatoru|

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

**Odgovorne osobe:** Dev tim & QA inženjeri

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

**Odgovorne osobe:** QA inženjeri

**Izlazni kriterij:** 
- Svi end-to-end scenariji prolaze
- Svi sigurnosni testovi prolaze (neovlašteni pristup blokiran u 100% slučajeva)
- Performansni testovi unutar prihvatljivih granica
- Konzistentnost podataka potvrđena nakon brisanja

  
## Prihvatno testiranje
Prihvatno testiranje se provodi na osnovu acceptance kriterija definisanih u user stories. Cilj je potvrditi da sistem ispunjava poslovne zahtjeve i da je spreman za puštanje u produkciju.

Ovo testiranje obuhvata verifikaciju svakog acceptance kriterija za sve user storije, uz sudjelovanje vlasnika proizvoda ili krajnjih korisnika u finalnom pregledu.

**Odgovorne osobe:** Product Owner, krajnji korisnici (organizator, predavač, učesnik), QA inženjeri

**Izlazni kriterij:** 
- Više od 90% acceptance kriterija verifikovano kao zadovoljeno
- Nema otvorenih Critical ili High defekata
- Product Owner dao formalno odobrenje
- Sva otvorena pitanja iz user storija riješena ili svjesno odložena 

## UI testiranje
UI testiranje provjerava ispravnost i konzistentnost korisničkog interfejsa — vizualni prikaz, navigacija, responzivnost i ponašanje UI komponenti u skladu sa dizajn specifikacijama.

Obuhvata:
- Provjeru da su sva obavezna polja, dugmad i poruke prikazani na ispravnom mjestu i sa ispravnim labelama
- Testiranje validacijskih poruka greške - da se prikazuju na pravom polju, u pravom trenutku
- Provjeru navigacije između stranica (redirecti nakon prijave, odjave, kreiranja, brisanja)
- Testiranje prikaza praznih stanja (prazna lista konferencija, sesija, dvorana)
- Provjeru da su akcije dostupne samo korisnicima sa odgovarajućom rolom (npr. dugme "Kreiraj konferenciju" vidljivo samo organizatoru)
- Testiranje responzivnosti na različitim veličinama ekrana

**Odgovorne osobe**: QA inženjer & Frontend developer za pronađene UI defekte.

**Izlazni kriterij:** 
- Sve kritične stranice prikazuju ispravne elemente za svaku rolu 
- Validacijske poruke se pojavljuju na ispravnim mjestima 
- Navigacija funkcioniše bez grešaka
- Nema vizualnih lomova na standardnim rezolucijama

## Regresijsko testiranje
Regresijsko testiranje se provodi nakon svake izmjene ili ispravke u kodu kako bi se osiguralo da nove promjene nisu narušile postojeće funkcionalnosti.

Obuhvata:
- Ponovna izvršavanja smoke test skupa za sve kritične tokove (prijava, kreiranje konferencije, prijava učesnika) nakon svakog deploy-a
- Provjeru da ispravka jednog defekta nije uvela novi defekt u drugim modulima
- Regresiju autentifikacije i autorizacije nakon svake izmjene u upravljanju rolama
- Provjeru integriteta podataka (kapaciteti, kotizacije, liste učesnika) nakon izmjena u back-end logici

**Odgovorne osobe:** QA inženjer, Tech Lead odobrava release nakon prolaska regresije.
- 100% smoke testova prolazi
- Nema novih Critical ili High defekata uvedenih izmjenom
- Svi prethodno zatvoreni defekti ostaju zatvoreni



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

|User Story|Acceptance Criteria| ID testnog slučaja|Opis testnog slučaja|
|----------|-------------------|-------------------|--------------------|
|US-01: Sign Up|Kada korisnik uspješno unese sve podatke i potvrdi registraciju, korisnik treba dobiti potvrdu o uspješnoj registraciji na ekranu.|TC-01| **Preduslovi**: Korisnik nije registrovan; stranica za registraciju je otvorena.<br>**Koraci**: Unijeti ime, prezime, validnu email adresu i lozinku. Kliknuti 'Registruj se'.<br>**Očekivani rezultat**: Sistem prikazuje poruku o uspješnoj registraciji na ekranu.|
|US-01: Sign Up|Sistem ne smije dozvoliti završetak registracije bez popunjenih svih obaveznih polja.|TC-02|**Preduslovi**: Stranica za registraciju je otvorena.<br>**Koraci**: Ostaviti jedno ili više obaveznih polja praznim (testirati svako polje zasebno: ime, prezime, email, lozinka). Kliknuti 'Registruj se'.<br>**Očekivani rezultat**: Sistem blokira submit i prikazuje poruku o obaveznom polju za svako prazno polje. Registracija nije izvršena.|
|US-01: Sign Up|Sistem ne smije dozvoliti registraciju sa već postojećom email adresom|TC-03|**Preduslovi**: U sistemu postoji korisnik sa email adresom korisnik@test.com.<br>**Koraci**: Unijeti sve validne podatke, ali koristiti već postojeću email adresu (korisnik@test.com). Kliknuti 'Registruj se'.<br>**Očekivani rezultat**: Sistem prikazuje poruku greške da email adresa već postoji. Registracija nije izvršena.|
|US-02: Sign In|Kada korisnik unese ispravne kredencijale i klikne "Prijavi se", korisnik treba biti preusmjeren na početnu stranicu sistema.|TC-04|**Preduslovi**: Korisnik je registrovan u sistemu; stranica za prijavu je otvorena.<br>**Koraci**: Unijeti ispravnu email adresu i lozinku. Kliknuti 'Prijavi se'.<br>**Očekivani rezultat**: Korisnik je preusmjeren na početnu stranicu sistema. Sesija je aktivna.|
|US-02: Sign In	|Sistem ne smije dozvoliti pristup sistemu korisniku koji nije registrovan.|TC-05|**Preduslovi**: Stranica za prijavu je otvorena; email adresa ne postoji u sistemu.<br>**Koraci**: Unijeti email adresu koja nije registrovana i bilo koju lozinku. Kliknuti 'Prijavi se'.<br>**Očekivani rezultat**: Sistem prikazuje generičku poruku greške. Pristup sistemu nije dozvoljen.|
|US-02: Sign In|Sistem ne smije dozvoliti prijavu sa pogrešnom kombinacijom emaila i lozinke|TC-06|**Preduslovi**: Korisnik je registrovan u sistemu; stranica za prijavu je otvorena.<br>**Koraci**: Unijeti ispravnu email adresu, ali pogrešnu lozinku. Kliknuti 'Prijavi se'.<br>**Očekivani rezultat**: Sistem prikazuje generičku poruku greške bez otkrivanja koji podatak je pogrešan. Prijava nije izvršena.|
|US-03: Log Out|Kada korisnik klikne na dugme za odjavu, korisnik treba biti preusmjeren na stranicu za prijavu.|TC-07|**Preduslovi**: Korisnik je prijavljen na sistem.<br>**Koraci**: Kliknuti na dugme za odjavu.<br>**Očekivani rezultat**: Korisnik je preusmjeren na stranicu za prijavu.|
|US-03: Log Out|Sistem ne smije dozvoliti pristup zaštićenim stranicama nakon odjave.|TC-08|**Preduslovi**: Korisnik je prethodno bio prijavljen; odjava je izvršena.<br>**Koraci**: Nakon odjave pokušati direktno pristupiti URL-u zaštićene stranice (npr. /dashboard).<br>**Očekivani rezultat**: Sistem preusmjerava na stranicu za prijavu. Zaštićeni sadržaj nije prikazan.|
|US-03: Log Out|Sistem mora poništiti korisničku sesiju nakon odjave, tako da povratak na prethodnu stranicu putem browsera ne otvara zaštićeni sadržaj.|TC-09|**Preduslovi**: Korisnik je prethodno bio prijavljen; odjava je izvršena.<br>**Koraci**: Nakon odjave pritisnuti dugme 'Nazad' u browseru.<br>**Očekivani rezultat**: Browser ne otvara zaštićeni sadržaj. Korisnik je preusmjeren na stranicu za prijavu ili stranica nije dostupna.|
|US-04.1: Pregled korisničkog profila|Sistem prikazuje trenutne podatke korisničkog profila: ime, prezime, email|TC-10|**Preduslovi**: Korisnik je prijavljen na sistem.<br>**Koraci**: Navigirati na stranicu korisničkog profila.<br>**Očekivani rezultat**: Stranica prikazuje tačne podatke prijavljenog korisnika: ime, prezime i email adresu. Podaci drugog korisnika nisu vidljivi.|
|US-04.2: Izmjena korisničkog profila|Kada korisnik izmijeni podatke i sačuva promjene, korisnik treba dobiti potvrdu o uspješnoj izmjeni.|TC-11|**Preduslovi**: Korisnik je prijavljen; stranica za uređivanje profila je otvorena.<br>**Koraci**: Promijeniti ime ili prezime. Kliknuti 'Sačuvaj'.<br>**Očekivani rezultat**: Sistem prikazuje poruku o uspješnoj izmjeni. Novi podaci su odmah vidljivi na profilu.|
|US-04.2: Izmjena korisničkog profila|Sistem mora tražiti potvrdu trenutne lozinke pri promjeni lozinke.|TC-12|**Preduslovi**: Korisnik je prijavljen; stranica za uređivanje profila je otvorena.<br>**Koraci**: Unijeti novu lozinku bez unosa trenutne lozinke. Pokušati sačuvati promjene.<br>**Očekivani rezultat**: Sistem traži unos trenutne lozinke. Promjena lozinke nije moguća bez potvrde postojeće.|
|US-05: Korisničke role i permisije|Kada korisniku bude dodijeljena određena rola, korisnik treba imati pristup samo onim funkcionalnostima koje su predviđene za tu rolu.|TC-13|**Preduslovi**: Postoje korisnici sa rolama: učesnik i organizator.<br>**Koraci**: Prijaviti se kao učesnik. Pokušati pristupiti stranici za kreiranje konferencije (direktnim URL-om ili putem UI).<br>**Očekivani rezultat**: Pristup je odbijen. Sistem prikazuje grešku ili preusmjerava. Funkcionalnost organizatora nije dostupna učesniku.|
|US-05: Korisničke role i permisije|Sistem mora onemogućiti pristup administratorskim funkcionalnostima svim korisnicima koji nemaju admin rolu.|TC-14|**Preduslovi**: Postoji korisnik sa rolom organizatora (bez admin role).<br>**Koraci**: Prijaviti se kao organizator. Pokušati pristupiti admin panelu ili admin funkcionalnostima direktnim URL-om.<br>**Očekivani rezultat**: Pristup je odbijen. Admin funkcionalnosti nisu dostupne organizatoru.|
|US-06: Kreiranje konferencije|Kada organizator popuni sve obavezne podatke i potvrdi kreiranje, korisnik treba dobiti potvrdu o uspješnom kreiranju konferencije. |TC-15|**Preduslovi**: Korisnik je prijavljen sa rolom organizatora; forma za kreiranje konferencije je otvorena.<br>**Koraci**: Unijeti naziv, datum, lokaciju i opis konferencije. Kliknuti 'Kreiraj'.<br>**Očekivani rezultat**: Sistem prikazuje poruku o uspješnom kreiranju. Konferencija je vidljiva u listi konferencija.|
|US-06: Kreiranje konferencije | Sistem ne smije dozvoliti kreiranje konferencije bez popunjenih obaveznih polja.| TC-16|**Preduslovi**: Korisnik je prijavljen sa rolom organizatora; forma za kreiranje je otvorena.<br>**Koraci**: Ostaviti jedno ili više obaveznih polja praznim. Kliknuti 'Kreiraj'.<br>**Očekivani rezultat:** Sistem blokira kreiranje i prikazuje poruku o obaveznom polju. Konferencija nije kreirana.|
|US-06: Kreiranje konferencije | Sistem ne smije dozvoliti kreiranje konferencije korisniku koji nema rolu organizatora.| TC-17|**Preduslovi**: Korisnik je prijavljen sa rolom učesnika.<br>**Koraci**: Pokušati pristupiti formi za kreiranje konferencije direktnim URL-om ili putem UI.<br>**Očekivani rezultat**: Pristup je odbijen. Sistem prikazuje grešku ili preusmjerava. Konferencija nije kreirana.|
|US-07: Pregled konferencija |Sistem mora prikazati listu svih dostupnih konferencija sa osnovnim informacijama (naziv, datum, lokacija). | TC-18|**Preduslovi**: U sistemu postoje aktivne konferencije; korisnik je prijavljen.<br>**Koraci**: Navigirati na stranicu s listom konferencija.<br>**Očekivani rezultat**: Lista prikazuje sve aktivne konferencije sa nazivom, datumom i lokacijom za svaku.|
|US-07: Pregled konferencija | Sistem mora prikazati jasnu poruku kada nema dostupnih konferencija.| TC-19|**Preduslovi**: U sistemu ne postoji nijedna aktivna konferencija.<br>**Koraci**: Navigirati na stranicu s listom konferencija.<br>**Očekivani rezultat**: Sistem prikazuje jasnu poruku da nema dostupnih konferencija. Lista je prazna bez grešaka.|
|US-07: Pregled konferencija| Sistem ne smije prikazivati konferencije koje su obrisane ili deaktivirane.| TC-20|**Preduslovi**: U sistemu postoji konferencija koja je prethodno obrisana ili deaktivirana.<br>**Koraci**: Navigirati na stranicu s listom konferencija.<br>**Očekivani rezultat**: Obrisana ili deaktivirana konferencija nije vidljiva u listi.|
|US-08: Pregled detalja konferencije |Kada korisnik klikne na konferenciju, ako konferencija postoji, tada vidi sve detalje. | TC-21|**Preduslovi**: U sistemu postoji aktivna konferencija; korisnik je na listi konferencija.<br>**Koraci**: Kliknuti na naziv konferencije u listi.<br>**Očekivani rezultat**: Sistem prikazuje stranicu s detaljima: naziv, datum, lokacija i opis konferencije.|
|US-09: Uređivanje konferencije |Kada organizator izmijeni podatke, ako su svi obavezni podaci uneseni, tada se promjene spremaju | TC-22|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija postoji.<br>**Koraci**: Otvoriti formu za uređivanje konferencije. Promijeniti naziv ili datum. Kliknuti 'Sačuvaj'.<br>**Očekivani rezultat**: Sistem snima promjene i prikazuje potvrdu. Izmijenjeni podaci su vidljivi na stranici detalja.|
|US-10: Brisanje konferencije |Kada organizator klikne na brisanje, ako potvrdi akciju, tada se konferencija briše | TC-23|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija postoji u sistemu.<br>**Koraci**: Kliknuti na 'Obriši' za odabranu konferenciju. Potvrditi akciju u dijalogu za potvrdu.<br>**Očekivani rezultat**: Konferencija je obrisana i više nije vidljiva u listi konferencija.|
|US-10: Brisanje konferencije | Korisnik treba dobiti informaciju da je brisanje uspješno| TC-24|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija postoji u sistemu.<br>**Koraci**: Izvršiti brisanje konferencije i potvrditi akciju.<br>**Očekivani rezultat**: Sistem prikazuje obavijest o uspješnom brisanju konferencije.|
|US-11: Pretraga konferencija |Kada korisnik unese pojam, ako postoje rezultati, tada sistem prikazuje odgovarajuće konferencije | TC-25|**Preduslovi**: U sistemu postoji konferencija s nazivom 'Tech Summit 2026'.<br>**Koraci**: Unijeti pojam 'Tech' u polje za pretragu.<br>**Očekivani rezultat**: Sistem prikazuje konferenciju 'Tech Summit 2026' u rezultatima pretrage.|
|US-11: Pretraga konferencija |Sistem mora prikazati samo relevantne konferencije | TC-26|**Preduslovi**: U sistemu postoje dvije konferencije: 'Tech Summit' i 'Design Week'.<br>**Koraci**: Unijeti pojam 'Tech' u polje za pretragu.<br>**Očekivani rezultat**: Sistem prikazuje samo 'Tech Summit'. 'Design Week' nije prikazana u rezultatima|
|US-11: Pretraga konferencija |Kada nema rezultata, sistem treba prikazati poruku | TC-27|**Preduslovi**: Stranica pretrage je otvorena.<br>**Koraci**: Unijeti pojam koji ne odgovara nijednoj konferenciji u sistemu (npr. 'nepoznato').<br>**Očekivani rezultat**: Sistem prikazuje jasnu poruku da nema rezultata za uneseni pojam.|
|US-12.1: Pregled sesija konferencije |Kada organizator otvori sesije, tada vidi listu svih sesija za konferenciju | TC-28|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija ima definirane sesije.<br>**Koraci**: Otvoriti stranicu sesija za određenu konferenciju.<br>**Očekivani rezultat**: Sistem prikazuje listu svih sesija vezanih za tu konferenciju.|
| US-12.1: Pregled sesija konferencije|Sistem mora prikazati osnovne podatke o sesiji | TC-29|**Preduslovi**: Korisnik je prijavljen; konferencija ima sesije.<br>**Koraci**: Navigirati na listu sesija konferencije.<br>**Očekivani rezultat**: Za svaku sesiju su vidljivi osnovni podaci: naziv, termin i dvorana.|
|US-12.1: Pregled sesija konferencije |Kada nema sesija, tada sistem prikazuje odgovarajuću poruku| TC-30|**Preduslovi**: Konferencija postoji, ali nema definiranih sesija.<br>**Koraci**: Otvoriti stranicu sesija za tu konferenciju.<br>**Očekivani rezultat**: Sistem prikazuje jasnu poruku da konferencija nema sesija.|
|US-12.2: Kreiranje sesije | Sistem ne smije dozvoliti kreiranje sesije sa istim terminom u istoj konferenciji| TC-31|**Preduslovi**: Konferencija postoji i ima sesiju u terminu 10:00–11:00.<br>**Koraci**: Pokušati kreirati novu sesiju u istom terminu (10:00–11:00) unutar iste konferencije.<br>**Očekivani rezultat**: Sistem blokira kreiranje i prikazuje poruku o konfliktu termina. Nova sesija nije kreirana.|
|US-12.2: Kreiranje sesije  |Sistem ne smije dozvoliti kreiranje bez obaveznih podataka | TC-32|**Preduslovi**: Korisnik je prijavljen kao organizator; forma za kreiranje sesije je otvorena.<br>**Koraci**: Ostaviti jedno ili više obaveznih polja praznim. Kliknuti 'Kreiraj'.<br>**Očekivani rezultat**: Sistem blokira kreiranje i prikazuje poruku o obaveznom polju. Sesija nije kreirana.|
|US-12.2: Kreiranje sesije | Kada organizator unese validne podatke, tada sistem sprema sesiju| TC-33|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija postoji.<br>**Koraci**: Unijeti sve validne podatke za sesiju (naziv, termin, dvorana). Kliknuti 'Kreiraj'.<br>**Očekivani rezultat**: Sistem snima sesiju. Nova sesija je odmah vidljiva u listi sesija konferencije.|
|US-12.3: Uređivanje sesije| Sistem ne smije dozvoliti nevalidne izmjene| TC-34|**Preduslovi**: Korisnik je prijavljen kao organizator; sesija postoji.<br>**Koraci**: Otvoriti formu za uređivanje sesije. Obrisati naziv (ostaviti prazno). Kliknuti 'Sačuvaj'.<br>**Očekivani rezultat**: Sistem blokira snimanje i prikazuje poruku o nevalidnim podacima. Sesija ostaje nepromijenjena.|
|US-12.4: Brisanje sesije | Kada organizator izvrši brisanje sesije, tada se ona uklanja iz sistema| TC-35|**Preduslovi**: Korisnik je prijavljen kao organizator; sesija postoji.<br>**Koraci**: Kliknuti 'Obriši' za odabranu sesiju. Potvrditi akciju.<br>**Očekivani rezultat**: Sesija je uklonjena iz sistema i više nije vidljiva u listi sesija ni u rasporedu|
|US-13: Dodjela predavača sesiji |Sistem ne smije dozvoliti dodjelu nepostojećeg korisnika | TC-36|**Preduslovi**: Korisnik je prijavljen kao organizator; sesija postoji.<br>**Koraci**: U formi za dodjelu predavača unijeti ID ili email korisnika koji ne postoji u sistemu.<br>**Očekivani rezultat**: Sistem prikazuje poruku greške. Dodjela nije izvršena.|
|US-13: Dodjela predavača sesiji |Kada organizator dodijeli predavača, ako predavač postoji, tada se prikazuje u sesiji | TC-37|**Preduslovi**: Korisnik je prijavljen kao organizator; sesija postoji; korisnik s predavačkom rolom postoji.<br>**Koraci**: Odabrati postojećeg korisnika iz liste i dodijeliti ga sesiji. Potvrditi.<br>**Očekivani rezultat**: Dodijeljeni predavač je vidljiv u prikazu detalja sesije.|
|US-14: Pregled rasporeda | Sistem ne smije prikazivati nepostojeće sesije| TC-38|**Preduslovi**: Konferencija postoji; sesija je kreirana, a zatim obrisana.<br>**Koraci**: Navigirati na stranicu rasporeda konferencije.<br>**Očekivani rezultat**: Obrisana sesija nije prikazana u rasporedu. Prikazuju se samo aktivne sesije.|
|US-15.1: Pregled dvorana |Kada nema dvorana, tada sistem prikazuje odgovarajuću poruku | TC-39|**Preduslovi**: Korisnik je prijavljen kao organizator; u sistemu nema definiranih dvorana.<br>**Koraci**: Navigirati na stranicu s listom dvorana.<br>**Očekivani rezultat**: Sistem prikazuje jasnu poruku da nema dvorana.|
|US-15.2: Dodavanje dvorane| Sistem ne smije dozvoliti unos duplikata dvorane| TC-40|**Preduslovi**: Korisnik je prijavljen kao organizator; dvorana s nazivom 'Sala A' već postoji.<br>**Koraci**: Pokušati dodati novu dvoranu s istim nazivom 'Sala A'.<br>**Očekivani rezultat**: Sistem prikazuje poruku greške o duplikatu. Dvorana nije kreirana.|
|US-15.2: Dodavanje dvorane |Organizator treba dobiti potvrdu o uspješnom dodavanju | TC-41|**Preduslovi**: Korisnik je prijavljen kao organizator; forma za dodavanje dvorane je otvorena.<br>**Koraci**: Unijeti validne podatke za novu dvoranu. Kliknuti 'Dodaj'.<br>**Očekivani rezultat**: Sistem prikazuje potvrdu o uspješnom dodavanju. Dvorana je vidljiva u listi.|
|US-15.4: Brisanje dvorane| Sistem ne smije dozvoliti brisanje dvorane koja je dodijeljena aktivnoj sesiji| TC-42|**Preduslovi**: Korisnik je prijavljen kao organizator; dvorana 'Sala A' je dodijeljena aktivnoj sesiji.<br>**Koraci**: Pokušati obrisati dvoranu 'Sala A'.<br>**Očekivani rezultat**: Sistem prikazuje poruku greške da dvorana ne može biti obrisana jer je dodijeljena aktivnoj sesiji. Brisanje nije izvršeno.|
|US-16: Dodjela dvorane|Kada organizator dodijeli dvoranu, ako postoji, tada se povezuje sa sesijom | TC-43|**Preduslovi**: Korisnik je prijavljen kao organizator; sesija postoji; dvorana postoji u sistemu.<br>**Koraci**: Otvoriti formu za uređivanje sesije. Odabrati dvoranu iz liste. Sačuvati.<br>**Očekivani rezultat**: Odabrana dvorana je prikazana u detaljima sesije kao dodijeljena dvorana.|
|US-18: Prijava učesnika na konferenciju| Korisnik treba dobiti potvrdu o uspješnoj prijavi| TC-44|**Preduslovi**: Korisnik je prijavljen kao učesnik; konferencija postoji i ima slobodnih mjesta.<br>**Koraci**: Otvoriti detalje konferencije. Kliknuti 'Prijavi se'.<br>**Očekivani rezultat**: Sistem prikazuje poruku o uspješnoj prijavi. Korisnik je evidentiran kao učesnik konferencije.|
|US-18: Prijava učesnika na konferenciju|Kada korisnik klikne prijavu, ako nije već prijavljen, tada se registruje | TC-45|**Preduslovi**: Korisnik je prijavljen kao učesnik; korisnik nije prethodno prijavljen na konferenciju.<br>**Koraci**: Otvoriti detalje konferencije. Kliknuti 'Prijavi se'.<br>**Očekivani rezultat**: Korisnik je dodan na listu učesnika konferencije. Dugme za prijavu odražava novi status.|
|US-19: Odjava učesnika s konferencije |Sistem mora promijeniti status prijave u 'Otkazano'. | TC-46|Preduslovi: Korisnik je prijavljen kao učesnik; korisnik je prethodno prijavljen na konferenciju.<br>**Koraci**: Navigirati na listu svojih konferencija ili profil. Kliknuti 'Odjavi se' za odabranu konferenciju.<br>**Očekivani rezultat**: Status prijave je promijenjen u 'Otkazano'. Promjena je vidljiva u sistemu.|
|US-19: Odjava učesnika s konferencije| Sistem mora ažurirati broj slobodnih mjesta.| TC-47|**Preduslovi**: Korisnik je prijavljen; korisnik je prijavljen na konferenciju; kapacitet je poznat.<br>**Koraci**: Izvršiti odjavu korisnika sa konferencije.<br>**Očekivani rezultat**: Broj slobodnih mjesta na konferenciji se povećao za 1 nakon odjave.|
|US-20: Prijava učesnika na sesiju|Sistem ne smije dozvoliti prijavu ako je sesija popunjena.|TC-48|**Preduslovi**: Sesija postoji i kapacitet je popunjen (0 slobodnih mjesta).<br>**Koraci**: Pokušati se prijaviti na popunjenu sesiju.<br>**Očekivani rezultat**: Sistem prikazuje poruku da je sesija popunjena. Prijava nije izvršena.|
|US-21: Pregled popunjenosti kapaciteta |Kada postoji konferencija ili sesija, sistem mora prikazati broj prijavljenih. | TC-49|**Preduslovi**: Konferencija postoji i ima prijavljenih učesnika.<br>**Koraci**: Navigirati na pregled popunjenosti konferencije ili sesije.<br>**Očekivani rezultat**: Sistem prikazuje tačan broj trenutno prijavljenih učesnika.|
|US-21: Pregled popunjenosti kapaciteta |Prikaz se mora ažurirati nakon svake prijave ili odjave. | TC-50|**Preduslovi**: Konferencija postoji sa poznatim brojem prijavljenih; korisnik je prijavljen.<br>**Koraci**: Prijaviti se na konferenciju (ili odjaviti). Odmah provjeriti prikaz kapaciteta.<br>**Očekivani rezultat**: Broj prijavljenih je ažuriran u realnom vremenu — odražava novo stanje bez potrebe za osvježavanjem stranice.|
|US-21: Pregled popunjenosti kapaciteta|Sistem mora prikazati maksimalni kapacitet. | TC-51|**Preduslovi**: Konferencija postoji sa definiranim maksimalnim kapacitetom.<br>**Koraci**: Navigirati na pregled popunjenosti.<br>**Očekivani rezultat**: Sistem prikazuje maksimalni kapacitet konferencije/sesije pored broja prijavljenih.|
|US-22: Lista učesnika po konferenciji | Sistem ne smije dozvoliti pristup listi neautorizovanim korisnicima| TC-52|**Preduslovi**: Korisnik je prijavljen kao učesnik (bez organizatorske role).<br>**Koraci**: Pokušati pristupiti listi učesnika konferencije direktnim URL-om.<br>**Očekivani rezultat:** Pristup je odbijen. Sistem prikazuje grešku ili preusmjerava. Lista učesnika nije prikazana.|
|US-23.1: Upravljanje kategorijama kotizacija |Sistem mora omogućiti izmjenu kategorija | TC-53|**Preduslovi**: Korisnik je prijavljen kao organizator; kategorija kotizacije postoji.<br>**Koraci**: Otvoriti formu za uređivanje kategorije. Promijeniti naziv. Sačuvati.<br>**Očekivani rezultat**: Izmjena je snimljena. Ažurirani naziv kategorije je vidljiv u listi.|
|US-23.1: Upravljanje kategorijama kotizacija|Sistem mora omogućiti kreiranje kategorija kotizacija | TC-54|**Preduslovi**: Korisnik je prijavljen kao organizator.<br>**Koraci**: Otvoriti formu za kreiranje kategorije. Unijeti naziv (npr. 'Student'). Kliknuti 'Kreiraj'.<br>**Očekivani rezultat**: Nova kategorija je kreirana i vidljiva u listi kategorija kotizacija.|
|US-23.1: Upravljanje kategorijama kotizacija |Sistem mora omogućiti brisanje kategorija | TC-55|**Preduslovi**: Korisnik je prijavljen kao organizator; kategorija kotizacije postoji.<br>**Koraci**: Kliknuti 'Obriši' pored odabrane kategorije. Potvrditi akciju.<br>**Očekivani rezultat:** Kategorija je obrisana i više nije vidljiva u listi.|
| US-23.2: Upravljanje iznosima kotizacija|Sistem mora evidentirati status (plaćeno / neplaćeno) | TC-56|**Preduslovi**: Korisnik je prijavljen kao organizator; kotizacija postoji za učesnika.<br>**Koraci**: Otvoriti pregled kotizacija. Provjeriti status za određenog učesnika.<br>**Očekivani rezultat**: Sistem prikazuje status kotizacije: 'Plaćeno' ili 'Neplaćeno' za svakog učesnika.|
|US-23.2: Upravljanje iznosima kotizacija | Sistem mora omogućiti unos iznosa kotizacije| TC-57|**Preduslovi**: Korisnik je prijavljen kao organizator; kategorija kotizacije postoji.<br>**Koraci**: Otvoriti formu za unos iznosa. Unijeti numeričku vrijednost. Sačuvati.<br>**Očekivani rezultat**: Iznos je snimljen i vidljiv u pregledu kotizacija za odabranu kategoriju.|
|US-23.2: Upravljanje iznosima kotizacija |Sistem mora povezati iznos sa kategorijom | TC-58|**Preduslovi**: Korisnik je prijavljen kao organizator; postoje kategorije kotizacija.<br>**Koraci**: Kreirati novi iznos kotizacije i dodijeliti ga kategoriji 'Redovni'.<br>**Očekivani rezultat:** Iznos je vidljiv pod kategorijom 'Redovni'. Veza između iznosa i kategorije je ispravna.|
|US-23.2: Upravljanje iznosima kotizacija |Sistem mora omogućiti izmjenu iznosa | TC-59|**Preduslovi**: Korisnik je prijavljen kao organizator; iznos kotizacije postoji.<br>**Koraci**: Otvoriti formu za uređivanje iznosa. Promijeniti vrijednost. Sačuvati.<br>**Očekivani rezultat:** Izmijenjeni iznos je snimljen i prikazan u pregledu kotizacija.|
|US-24: Obavijesti za korisnike |Kada se desi važan događaj, sistem mora poslati obavijest| TC-60|**Preduslovi**: Korisnik je prijavljen; postoji konferencija na kojoj se može izvršiti akcija.<br>**Koraci:** Prijaviti se na konferenciju ili izvršiti drugu okidačku akciju (promjena statusa, brisanje).<br>**Očekivani rezultat**: Korisnik prima obavijest o izvršenoj akciji. Obavijest je vidljiva u UI interfejsu.|
|US-24: Obavijesti za korisnike |Korisnik treba imati mogućnost pregleda prethodnih obavijesti | TC-61|**Preduslovi**: Korisnik je prijavljen; postoji historija obavijesti.<br>**Koraci**: Navigirati na stranicu ili sekciju s obavijestima.<br>**Očekivani rezultat**: Sistem prikazuje listu prethodnih obavijesti za prijavljenog korisnika.|
|US25: Pregled materijala za konferenciju i sesije |Mogućnost pretrage, filtriranja, sortiranja | TC-62|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija ima dodane materijale.<br>**Koraci**: Otvoriti listu materijala. Koristiti polje za pretragu i filtre.<br>**Očekivani rezultat**: Sistem prikazuje filtrirane/pretražene rezultate. Sortiranje mijenja redoslijed prikaza.|
|US25: Pregled materijala za konferenciju i sesije |Sistem mora prikazati sve trenutno dodane materijale za odabranu konferenciju/sesiju | TC-63|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija/sesija ima dodane materijale.<br>**Koraci**: Otvoriti stranicu materijala za odabranu konferenciju ili sesiju.<br>**Očekivani rezultat:** Svi dodani materijali su prikazani u listi za odabranu konferenciju/sesiju.|
|US25: Pregled materijala za konferenciju i sesije|Klikom na neki od materijala se prikazuje više detalja| TC-64|**Preduslovi**: Korisnik je prijavljen; lista materijala je otvorena.<br>**Koraci**: Kliknuti na jedan od materijala u listi.<br>**Očekivani rezultat:** Sistem prikazuje detaljan prikaz odabranog materijala.|
|US27.1: Pregled logističkih aktivnosti |Omogućiti filtriranje po tipu aktivnosti (catering, ručak, video sadržaj i dr.) | TC-65|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija ima logističke aktivnosti različitih tipova.<br>**Koraci**: Otvoriti listu logističkih aktivnosti. Odabrati filter 'Catering'.<br>**Očekivani rezultat**: Sistem prikazuje samo logističke aktivnosti tipa 'Catering'. Ostali tipovi nisu prikazani.|
|US27.2: Dodavanje logističkih aktivnosti  |Sistem daje mogućnost odabira vrste aktivnosti iz liste opcija pri kreiranju| TC-66|**Preduslovi**: Korisnik je prijavljen kao organizator; forma za kreiranje logističke aktivnosti je otvorena.<br>**Koraci**: Otvoriti formu za kreiranje nove logističke aktivnosti. Provjeriti polje za tip aktivnosti.<br>**Očekivani rezultat**: Polje za tip aktivnosti prikazuje predefinisanu listu opcija (catering, ručak, video i sl.). Moguće je odabrati jednu od ponuđenih opcija.|
|US28.4:Dodjela tehničke opreme konferenciji |Moguće je dodijeliti onoliko opreme konferenciji koliko je dostupno | TC-67|**Preduslovi**: Korisnik je prijavljen kao organizator; postoje 3 dostupne jedinice projektora; konferencija postoji.<br>**Koraci**: Pokušati dodijeliti 5 projektora konferenciji (više od dostupnog). Zatim pokušati dodijeliti 3 (jednako dostupnom).<br>**Očekivani rezultat**: Dodjela 5 jedinica je odbijena s porukom o nedovoljnoj količini. Dodjela 3 jedinice je uspješna.|
|US29.1: Postavljanje pitanja za Q&A sesiju |Sistem omogućava da učesnici na predavanju mogu postavljati pitanja bez prekidanja predavača | TC-68|**Preduslovi**: Korisnik je prijavljen; sesija je aktivna; korisnik ima pristup Q&A linku.<br>**Koraci**: Otvoriti Q&A link za aktivnu sesiju. Unijeti pitanje i poslati ga.<br>**Očekivani rezultat**: Pitanje je uspješno poslano bez ometanja predavača. Pitanje je vidljivo u Q&A listi.|
|US29.2: Prikaz pitanja predavaču| Predavač može označiti pitanja kao odgovorena, čime se uklanjaju iz liste| TC-69|**Preduslovi**: Predavač je prijavljen; postoje pitanja u Q&A listi za aktivnu sesiju.<br>**Koraci**: Kliknuti na 'Označi kao odgovoreno' za jedno pitanje.<br>**Očekivani rezultat**: Pitanje je uklonjeno iz aktivne liste pitanja. Predavaču se prikazuju samo neodgovorena pitanja.|
|US30: Izvještaji za organizatore|Organizator može pregledati izvještaj i preuzeti ga|TC-70|**Preduslovi**: Korisnik je prijavljen kao organizator; konferencija postoji sa prijavama i kotizacijama.<br>**Koraci**: Navigirati na stranicu izvještaja za odabranu konferenciju. Pregledati izvještaj. Kliknuti 'Preuzmi'.<br>**Očekivani rezultat**: Izvještaj je prikazan u sistemu sa tačnim podacima (prijave, kapacitet, kotizacije). Preuzimanje pokreće download fajla.|




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