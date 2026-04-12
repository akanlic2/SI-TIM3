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

## Sistemsko testiranje
Sistemsko testiranje provjerava ponašanje cjelokupnog sistema kao integrisane cjeline, uključujući end-to-end tokove i ne-funkcionalne zahtjeve.

Obuhvata:
- End-to-end testiranje kompletnog toka: registracija → prijava → kreiranje konferencije → dodavanje sesija → prijava učesnika → Q&A → izvještaj
- Testiranje sigurnosti: provjera da neautorizovani korisnici ne mogu pristupiti zaštićenim resursima (direktnim URL-om, manipulacijom zahtjeva)
- Testiranje konkurentnog pristupa: istovremena prijava više učesnika na sesiju sa ograničenim kapacitetom
- Testiranje konzistentnosti podataka nakon brisanja (konferencija, sesija, dvorane)
- Testiranje performansi pri pregledu lista sa većim brojem zapisa
- Testiranje integriteta sesije nakon odjave korisnika
  
## Prihvatno testiranje
Prihvatno testiranje se provodi na osnovu acceptance kriterija definisanih u user stories. Cilj je potvrditi da sistem ispunjava poslovne zahtjeve i da je spreman za puštanje u produkciju.

Ovo testiranje obuhvata verifikaciju svakog acceptance kriterija za sve user storije, uz sudjelovanje vlasnika proizvoda ili krajnjih korisnika u finalnom pregledu.





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

# Način evidentiranja rezultata testiranja

# Glavni rizici kvaliteta
| ID |Opis rizika |Vjerovatnoća |Uticaj| Strategija mitigacije|
|----|-----------|--------|-----------|--------|
|R01 |Neispravna implementacija rolno-baziranog pristupa omogućava neovlaštene akcije|Srednja |Visok |Detaljno testiranje svakog zaštićenog endpoint-a za svaku rolu|
|R02 |Race condition pri istovremenom prijavljivanju više učesnika na sesiju s ograničenim kapacitetom|Visoka| Visok|Konkurentno testiranje, provjera da se kapacitet ne prekorači ni u jednom scenariju |
|R03 |Sesija korisnika ostaje aktivna nakon odjave, pa je omogućen neovlašteni pristup|Niska|Visok|Testiranje invalidacije sesije i provjera zaštićenih ruta nakon odjave|
|R04 |Prikaz podataka jednog korisnika drugom korisniku|Niska|Visok|Testiranje izolacije podataka: provjera da korisnik vidi samo svoje podatke|
|R05 |Konflikti pri uređivanju iste konferencije/sesije od strane više organizatora| Srednja| Srednji|Testiranje konkurentnog uređivanja; provjera mehanizma za rješavanje konflikata (US-27.3)|
|R06 |Q&A pitanja dostupna korisnicima koji nisu prisutni na predavanju|Niska|Srednji|Testiranje autorizacije za Q&A, provjera da link funkcioniše isključivo u kontekstu sesije|