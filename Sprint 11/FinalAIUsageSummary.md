# Final AI Usage Summary

## 1. Uvod

Tokom razvoja projekta **ConferenceHub — sistema za organizaciju konferencija**, tim je koristio AI alate kao pomoćno sredstvo u planiranju, implementaciji, testiranju, dokumentovanju i rješavanju tehničkih problema. AI alati nisu korišteni kao zamjena za rad članova tima, nego kao podrška za brže pronalaženje rješenja, generisanje početnih prijedloga, provjeru ideja i pomoć pri pisanju testova i dokumentacije.

Korišteni AI alati uključivali su:

* ChatGPT
* GitHub Copilot
* Claude AI
* Gemini

Sva rješenja predložena od strane AI alata bila su dodatno pregledana, prilagođena i testirana od strane članova tima prije uključivanja u projekat.

---

## 2. Za šta je AI korišten

AI alati su korišteni u više faza projekta i za različite vrste zadataka.

### 2.1 Planiranje i arhitektura sistema

AI je korišten za konsultacije prilikom donošenja ranih tehničkih odluka, posebno kod definisanja osnovnih entiteta, korisničkih rola i organizacije slojeva aplikacije. U početnim sprintovima AI je pomogao timu da potvrdi pristup inkrementalnog razvoja, odnosno da se prvo implementiraju osnovne funkcionalnosti poput korisnika, registracije i prijave, a da se složeniji moduli dodaju kroz kasnije sprintove.

AI je također korišten za razmatranje arhitekturnih opcija kao što su servisni sloj, repozitoriji, validacija DTO objekata, organizacija backend slojeva i povezivanje frontend i backend dijela aplikacije.

### 2.2 Implementacija backend funkcionalnosti

AI je korišten kao pomoć pri implementaciji backend logike u ASP.NET Core aplikaciji. Posebno je korišten za:

* kreiranje i organizaciju entiteta,
* pisanje DTO klasa,
* implementaciju servisnog sloja,
* definisanje repozitorija,
* implementaciju API kontrolera,
* dodavanje autorizacijskih pravila,
* pisanje EF Core migracija,
* dijagnostiku grešaka pri pokretanju migracija,
* organizaciju dependency injection konfiguracije u `Program.cs`.

Backend dijelovi u kojima je AI korišten kao pomoć uključuju autentifikaciju, korisničke role, konferencije, sesije, prijave na konferencije i sesije, dvorane, agendu, Q&A panel, notifikacije, materijale, logistiku, opremu i izvještaje.

### 2.3 Implementacija frontend funkcionalnosti

AI je korišten za pomoć pri pisanju React + TypeScript komponenti i povezivanju frontend dijela sa backend API-jem. Posebno je korišten za:

* forme za unos i uređivanje podataka,
* API servisne fajlove,
* upravljanje stanjem pomoću React hookova,
* prikaz konferencija, sesija, dvorana i agende,
* prikaz korisničkog dashboarda po ulozi,
* prikaz notifikacija,
* Q&A panel,
* upload materijala,
* prikaz logistike, opreme i izvještaja.

AI prijedlozi su služili kao početna verzija ili pomoć pri rješavanju pojedinačnih problema, dok je finalni izgled i ponašanje komponenti prilagođeno stvarnim zahtjevima projekta.

### 2.4 Testiranje sistema

AI je značajno korišten kao pomoć pri pisanju backend i frontend testova. Za backend su generisani prijedlozi unit testova koristeći xUnit i Moq, dok su za frontend korišteni Vitest i React Testing Library.

AI je pomogao pri pisanju testova za:

* autentifikaciju i korisničke profile,
* konferencije,
* sesije,
* prijave i odjave korisnika,
* dvorane i dodjelu dvorana sesijama,
* agendu,
* kapacitete konferencija i sesija,
* listu učesnika,
* Q&A panel,
* notifikacije,
* materijale,
* logistiku,
* opremu,
* izvještaje.

Testovi koje je AI predložio nisu preuzimani bez provjere. Tim je morao prilagođavati mock podatke, import putanje, nazive metoda, očekivane rezultate i assertions prema stvarnoj implementaciji.

### 2.5 Deployment i infrastruktura

AI je korišten za pomoć pri izboru i podešavanju deployment okruženja. Korišten je za konsultacije oko:

* izbora VPS provajdera,
* korištenja DigitalOcean servera,
* korištenja `nip.io` domene,
* dodavanja swap memorije zbog ograničenih resursa servera,
* Docker i Docker Compose konfiguracije,
* GitHub Actions pipeline-a,
* build i deploy procesa.

Dio prijedloga je prihvaćen, ali je tim ručno prilagodio konfiguracije stvarnoj strukturi projekta, portovima, environment varijablama i produkcijskom okruženju.

### 2.6 Dokumentacija

AI je korišten i za pomoć pri pisanju i sređivanju projektne dokumentacije. To uključuje:

* AI Usage Log,
* Proof of Testing dokumente,
* Sprint Review Summary,
* Sprint Retrospective Summary,
* Release Notes,
* korisničku dokumentaciju,
* tehničku dokumentaciju,
* završni AI Usage Summary.

AI je korišten za oblikovanje teksta, strukturiranje sekcija i usklađivanje dokumentacije sa zahtjevima predmeta. Konačni sadržaj je pregledan i prilagođen od strane tima.

---

## 3. Šta je tim prihvatio

Tim je prihvatio one AI prijedloge koji su bili usklađeni sa arhitekturom projekta, stvarnim zahtjevima sprinta i postojećim kodom.

Prihvaćeno je:

* korištenje inkrementalnog pristupa razvoju,
* osnovna struktura pojedinih entiteta i DTO klasa,
* dio servisne logike za backend funkcionalnosti,
* početni prijedlozi za API kontrolere,
* dio validacijskih pravila,
* prijedlozi za organizaciju frontend komponenti,
* struktura API servisnih fajlova na frontendu,
* početne verzije testova,
* struktura mockovanja zavisnosti u testovima,
* osnovna Docker i deployment konfiguracija,
* dio prijedloga za GitHub Actions pipeline,
* prijedlozi za strukturu projektne dokumentacije.

Tim je posebno prihvatio AI kao alat za ubrzavanje rutinskih zadataka, kao što su generisanje sličnih testnih slučajeva, inicijalnih DTO klasa, osnovnih CRUD metoda i početne strukture dokumentacije.

---

## 4. Šta je tim izmijenio

Većina AI prijedloga nije direktno preuzeta bez izmjena. Tim je morao prilagođavati generisana rješenja stvarnom stanju projekta.

Najčešće izmjene su bile:

* usklađivanje namespace-ova sa postojećom strukturom projekta,
* promjena naziva klasa, metoda i fajlova prema konvencijama tima,
* prilagođavanje DTO objekata stvarnim poljima entiteta,
* ručno popravljanje import putanja,
* prilagođavanje mock podataka stvarnim modelima,
* izmjena testova koji nisu odgovarali ponašanju aplikacije,
* dodavanje dodatnih test case-ova koje AI nije predložio,
* uklanjanje nepotrebno kompleksnih dijelova koda,
* prilagođavanje Docker i CI/CD konfiguracije stvarnim putanjama u repozitoriju,
* zamjena generičkih vrijednosti stvarnim portovima, URL-ovima i environment varijablama,
* dorada dokumentacije tako da odgovara stvarno implementiranim funkcionalnostima.

Posebno je važno naglasiti da je tim više puta morao uskladiti AI prijedloge sa stvarnim backend i frontend stanjem, jer AI nije uvijek imao potpun kontekst projekta.

---

## 5. Šta je tim odbacio

Tim je odbacio AI prijedloge koji nisu odgovarali trenutnoj fazi projekta, bili su previše kompleksni ili nisu bili u skladu sa dogovorenom arhitekturom.

Odbačeni su:

* prijedlozi za napredne korisničke atribute koji nisu bili dio MVP-a,
* previše kompleksni integracioni testovi u ranim sprintovima,
* testovi koji su duplicirali postojeću logiku bez stvarne vrijednosti,
* prijedlog za korištenje kompleksnijih arhitekturnih patterna kada je jednostavniji service pattern bio dovoljan,
* određeni deployment prijedlozi koji nisu odgovarali ograničenjima servera,
* prijedlog za resetovanje svih migracija kada se problem mogao riješiti sigurnije,
* dijelovi generisanog koda koji su pretpostavljali funkcionalnosti koje još nisu bile implementirane,
* prijedlozi koji bi uvodili dodatne biblioteke bez jasne potrebe,
* generički UI prijedlozi koji nisu odgovarali dizajnu aplikacije.

Tim je odbacio sve prijedloge koji su mogli povećati tehnički dug ili narušiti stabilnost sistema.

---

## 6. Greške koje je AI napravio

Tokom rada uočeno je više tipičnih grešaka koje su AI alati pravili.

### 6.1 Generisanje koda bez potpunog konteksta

AI je ponekad generisao kod koji je bio tehnički ispravan, ali nije odgovarao stvarnoj strukturi projekta. Primjeri uključuju pogrešne namespace-ove, pogrešne putanje importovanja, pogrešne nazive servisa ili repozitorija i pretpostavljanje da određene klase već postoje.

### 6.2 Dupliranje postojećih interfejsa ili klasa

U nekim slučajevima AI je predložio kreiranje interfejsa ili klasa koje su već postojale u projektu, ali u drugom sloju. Takvi prijedlozi su morali biti ručno uklonjeni ili usklađeni sa postojećom arhitekturom.

### 6.3 Testovi koji prolaze, ali ne testiraju pravu stvar

Jedan od važnijih rizika bio je da AI generiše testove koji formalno prolaze, ali ne provjeravaju stvarno ponašanje sistema. Takvi testovi mogu dati lažan osjećaj sigurnosti. Tim je zato morao ručno pregledati assertions, mock podatke i očekivane ishode.

### 6.4 Pogrešni ili nepotpuni mockovi

AI je često generisao mock podatke koji nisu odgovarali stvarnim entitetima ili ponašanju repozitorija. Ovo je posebno bilo vidljivo kod testova za backend servise i frontend komponente.

### 6.5 Predlaganje funkcionalnosti koje nisu dio sprinta

AI je ponekad predlagao naprednije funkcionalnosti koje nisu bile dio trenutnog sprinta, kao što su dodatna validacija, dodatni atributi, naprednije role ili dodatni endpointi. Takvi prijedlozi su odloženi ili odbijeni kako bi se zadržao fokus na planiranom scope-u sprinta.

### 6.6 Zastarjeli ili neodgovarajući prijedlozi za pakete

AI je u pojedinim slučajevima predlagao biblioteke ili verzije paketa koje nisu bile dostupne ili nisu odgovarale trenutnoj konfiguraciji projekta. Tim je morao provjeravati verzije i birati rješenja koja su kompatibilna sa projektom.

### 6.7 Deployment prijedlozi koji su zahtijevali prilagodbu

Kod deploymenta, AI je davao korisne smjernice, ali su mnogi prijedlozi morali biti prilagođeni stvarnom serveru, ograničenoj memoriji, Docker konfiguraciji i konkretnim GitHub Actions secrets vrijednostima.

---

## 7. Dijelovi sistema razvijani uz AI pomoć i šta tim mora znati objasniti

Sljedeći dijelovi sistema su razvijani uz određeni nivo AI pomoći i članovi tima ih moraju posebno dobro znati objasniti na odbrani.

### 7.1 Autentifikacija, registracija i korisničke role

AI je korišten za planiranje korisničkog entiteta, role-based pristupa i osnovnih testova za login i registraciju. Tim mora znati objasniti:

* kako korisnik pravi nalog,
* kako se korisnik prijavljuje,
* kako se čuva token,
* kako backend provjerava autorizaciju,
* koje role postoje u sistemu,
* koje funkcionalnosti su dostupne kojoj roli.

### 7.2 Upravljanje konferencijama

AI je korišten pri implementaciji dijela CRUD logike za konferencije, DTO klase, validaciju, servisni sloj i testove. Tim mora znati objasniti:

* kako se kreira konferencija,
* kako se uređuje i briše konferencija,
* kako se dohvaća lista konferencija,
* kako se provjerava rola korisnika,
* kako frontend komunicira sa backend endpointima,
* kako su implementirani testovi za ove funkcionalnosti.

### 7.3 Upravljanje sesijama i dodjela predavača

AI je korišten za dio implementacije i testiranja sesija, dodjele predavača i prijava na sesije. Tim mora znati objasniti:

* kako je sesija povezana sa konferencijom,
* kako se kreira, uređuje i briše sesija,
* kako se predavač dodjeljuje sesiji,
* kako se učesnik prijavljuje na sesiju,
* kako se provjeravaju kapacitet i termini.

### 7.4 Dvorane, agenda i kapaciteti

AI je korišten za pomoć pri pisanju testova i dijela implementacije za dvorane, dodjelu dvorane sesiji, agendu i pregled kapaciteta. Tim mora znati objasniti:

* kako se kreiraju i uređuju dvorane,
* kako se dvorana dodjeljuje sesiji,
* kako se sprječava konflikt termina i prostora,
* kako agenda prikazuje raspored konferencije,
* kako se računa i prikazuje popunjenost kapaciteta.

### 7.5 Q&A panel

AI je korišten kao pomoć pri testiranju i implementaciji Q&A funkcionalnosti. Tim mora znati objasniti:

* kada korisnik može postaviti pitanje,
* zašto se pitanje može postaviti tek nakon početka sesije,
* kako se pitanje povezuje sa sesijom i korisnikom,
* kako predavač odgovara na pitanje,
* kako se provjerava da samo dodijeljeni predavač može odgovoriti,
* kako se korisnik obavještava o odgovoru.

### 7.6 In-app notifikacije

AI je korišten za pisanje testova i dijela logike vezane za notifikacije. Tim mora znati objasniti:

* kada se notifikacije kreiraju,
* ko je primalac notifikacije,
* kako korisnik dohvaća svoje notifikacije,
* kako se notifikacija označava kao pročitana,
* kako radi brojač nepročitanih notifikacija na frontendu.

### 7.7 Upload i pregled materijala

AI je korišten za testove i dio implementacije upload funkcionalnosti. Tim mora znati objasniti:

* ko može uploadati materijal,
* zašto predavač smije uploadati samo za sesije kojima je dodijeljen,
* kako admin i organizator imaju širi pristup,
* kako se materijal povezuje sa sesijom,
* kako korisnici pristupaju materijalima.

### 7.8 Logistika i oprema

AI je korišten u sprintu gdje su implementirane i testirane logističke aktivnosti i tehnička oprema. Tim mora znati objasniti:

* kako organizator kreira logističke aktivnosti,
* kako se oprema evidentira,
* kako se oprema dodjeljuje sesiji,
* kako se provjerava dostupna količina opreme,
* koja su poznata ograničenja u validaciji i ownership provjerama.

### 7.9 Izvještaji i PDF export

AI je korišten kao pomoć pri razumijevanju i testiranju izvještaja za organizatore. Tim mora znati objasniti:

* koji podaci ulaze u izvještaj,
* kako se prikazuju statistike konferencije,
* kako se generiše PDF,
* zašto je ova funkcionalnost dostupna organizatoru i adminu.

### 7.10 Testovi

AI je korišten za generisanje i doradu velikog broja testova. Tim mora znati objasniti:

* razliku između backend i frontend testova,
* zašto se koriste xUnit i Moq na backendu,
* zašto se koriste Vitest i React Testing Library na frontendu,
* šta se mockuje u testovima,
* kako se provjerava očekivani ishod,
* zašto test koji prolazi ne znači nužno da je test dobar,
* kako su AI-generisani testovi ručno pregledani i prilagođeni.

### 7.11 Deployment i CI/CD

AI je korišten za konsultacije oko deploymenta i CI/CD procesa. Tim mora znati objasniti:

* zašto je odabran DigitalOcean VPS,
* kako se koristi Docker Compose,
* kako se pokreću backend, frontend i baza,
* kako GitHub Actions pokreće testove i deployment,
* zašto su environment varijable i secrets važni,
* koje prilagodbe su urađene zbog ograničenih resursa servera.

---

## 8. Kritički osvrt na korištenje AI-ja

AI alati su timu bili korisni jer su ubrzali razvoj, pomogli u pisanju početnih verzija koda, ponudili ideje za testove i olakšali pisanje dokumentacije. Međutim, AI nije bio potpuno pouzdan izvor gotovih rješenja.

Najveća vrijednost AI-ja bila je u tome što je pomagao kao asistent za prijedloge, strukturu i brže pronalaženje rješenja. Najveći rizik bio je mogućnost da AI generiše kod koji izgleda ispravno, ali nije u skladu sa stvarnim projektom ili ne testira stvarno ponašanje sistema.

Zbog toga je tim koristio AI kritički:

* svaki prijedlog je pregledan,
* kod je prilagođen postojećoj arhitekturi,
* testovi su ručno provjereni,
* sumnjivi prijedlozi su odbijeni,
* odluke su dokumentovane,
* funkcionalnosti su testirane prije završne predaje.

AI nije donosio konačne odluke umjesto tima. Konačne odluke su donosili članovi tima na osnovu zahtjeva projekta, dogovorene arhitekture i rezultata testiranja.

---

## 9. Zaključak

AI alati su korišteni kao pomoć u razvoju sistema ConferenceHub, ali ne kao zamjena za razumijevanje i implementaciju od strane tima. Najviše su pomogli u pisanju početnih verzija koda, testova, deployment konfiguracije i dokumentacije.

Tim je prihvatio korisne prijedloge, izmijenio sve što nije odgovaralo stvarnom projektu, odbacio prijedloge koji su bili previše kompleksni ili netačni, te dokumentovao greške i rizike. Posebna pažnja posvećena je tome da se dijelovi sistema razvijani uz AI pomoć mogu jasno objasniti na odbrani.

Završni zaključak je da je AI bio koristan alat za ubrzanje rada i podršku učenju, ali je kvalitet konačnog sistema zavisio od ljudske provjere, razumijevanja, testiranja i odgovornosti tima.
