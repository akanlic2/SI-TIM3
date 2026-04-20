# Branching strategija

GitHub Flow je jednostavan i lagan branching model koji se zasniva na radu sa kratkotrajnim feature granama i jednoj glavnoj grani (main). Najčešće se koristi u timovima koji rade na web aplikacijama i koriste CI/CD pristup, gdje je cilj da se promjene brzo razvijaju, testiraju i isporučuju.

Ovaj model je posebno pogodan za manje i srednje timove, jer ne zahtijeva kompleksnu strukturu grana niti dodatne procese upravljanja verzijama, što omogućava bržu i jednostavniju saradnju.
S obzirom na to da je naš tim osmočlan, mi smo opredijelili baš za GitHub Flow.

## Zašto GitHub Flow?

Implementacija projekta podrazumijeva redovno unapređenje funkcionalnosti kao što su prijava učesnika, upravljanje sesijama, raspored konferencije, dodjela dvorana, obrada kotizacija, slanje obavijesti i generisanje osnovnih izvještaja za organizatore. Iz tog razloga, važno je omogućiti brzo razvijanje i redovno dodavanje novih funkcionalnosti, uz stabilan rad sistema. GitHub Flow se pokazao kao optimalan izbor zbog svoje jednostavnosti i jasno definisanog procesa.

## Kako funkcioniše GitHub Flow

Radni tok je jednostavan i pregledan:

- Kreiramo feature granu iz main grane  
- Commit-ovi se šalju na feature granu  
- Kreira se pull request za pregled koda  
- Nakon što drugi član tima odobri izmjene, one se mergeaju u main granu  
- Po potrebi se odmah vrši deploy (nije obavezno, ali se preporučuje)  

Sve što se nalazi u main grani uvijek treba biti spremno za produkciju.

## Struktura grana i proces rada sa izmjenama

U okviru našeg projekta koristimo jednu glavnu granu i više pomoćnih grana koje omogućavaju paralelan rad članova tima i bolju organizaciju izmjena.

### Glavna grana

- `main` predstavlja stabilnu verziju projekta  
- sadrži samo testirane i odobrene izmjene  
- direktan rad na ovoj grani nije dozvoljen  

### Pomoćne grane

- `feature/` – koristi se za razvoj novih funkcionalnosti sistema  
- `docs/` – koristi se za izradu i izmjenu dokumentacije  
- `fix/` – koristi se za ispravke grešaka  

Sve pomoćne grane su privremene i brišu se nakon merge-a.

### Pravila imenovanja grana

Nazivi grana trebaju biti jasni i opisivati zadatak na kojem se radi, kako bi svi članovi tima mogli lako razumjeti svrhu svake grane.

Koristimo sljedeći format:

- `feature/naziv-funkcionalnosti`
- `docs/naziv-dokumenta`
- `fix/opis-greske`

Primjeri:

main
│
├── feature/PB-21-registracija-korisnika
├── feature/PB-22-prijava-korisnika
├── feature/PB-26-kreiranje-konferencije
├── feature/PB-27-pregled-konferencija
├── feature/PB-30-upravljanje-sesijama
├── feature/PB-33-raspored-konferencije
├── docs/use-case-model
├── docs/domain-model
├── docs/branching-strategy
├── fix/ispravka-greske
└── chore/podesavanje-projekta


## Review proces (Pull Request)

Sve izmjene u projektu moraju proći kroz Pull Request prije nego što se spoje u glavnu granu `main`, čime se osigurava kvalitet i tačnost implementiranih promjena.

Pull Request omogućava:
- pregled izmjena od strane drugih članova tima  
- pronalazak potencijalnih grešaka  
- poboljšanje kvaliteta koda i dokumentacije  

### Pravila review-a:

- direktan push na `main` granu nije dozvoljen  
- svaki branch mora imati Pull Request  
- najmanje jedan član tima mora pregledati izmjene  
- izmjene se po potrebi koriguju na osnovu komentara  
- merge se vrši tek nakon odobrenja  

### Proces review-a

1. Član tima završi rad na svom branch-u  
2. Push-a branch na GitHub  
3. Otvara Pull Request prema `main` grani  
4. Drugi član tima pregledava izmjene  
5. Ako postoje komentari ili prijedlozi, vrše se potrebne ispravke  
6. Nakon odobrenja, Pull Request se merge-a u `main`  
7. Završeni branch se briše  


## Prednosti GitHub Flow-a

- Jednostavan i intuitivan za učenje, bez složenih pravila grananja  
- Idealan je za brzu iteraciju i brzo dobijanje povratnih informacija  
- Dobro se uklapa sa CI/CD pipeline-ovima  
- Podstiče čestu integraciju izmjena, što smanjuje potrebu za velikim i kompleksnim mergeovima
- Olakšava saradnju u timu i smanjuje kompleksnost upravljanja granama
- Pogodan je za manje i srednje timove zbog jednostavne strukture i lakog upravljanja granama  

## Nedostaci GitHub Flow-a

- Nije pogodan za rad na više produkcijskih verzija ili dugoročnu (LTS) podršku  
- U većim timovima može dovesti do čestih konflikata pri mergeu izmjena  
- Ne koristi posebne QA ili staging grane za testiranje i pripremu izdanja  
- Potrebno je dobro automatizovano testiranje kako bi se izbjegle greške u produkciji  

## Konflikti pri mergeu

Da bi se izbjegli konflikti pri mergeu ili rano otkrili, timovi trebaju pratiti nekoliko dobrih praksi.

- Često mergeanje izmjena. Da bi grane ostale usklađene, developeri više puta dnevno integrišu promjene u main. Tako se sprječava da se grane udalje jedna od druge i smanjuje količina različitog koda  
- Kratkotrajne grane. Male i fokusirane grane se manje preklapaju s radom drugih i lakše se mergeaju  
- Jasna odgovornost nad kodom. Svakom dijelu koda treba dodijeliti odgovorne osobe, kako više developera ne bi radilo na istim fajlovima u isto vrijeme  
- Aktivna komunikacija. Važno je da se timovi usklađuju kada rade na zajedničkom ili kritičnom dijelu sistema  
- CI/CD prakse. Automatizovani pipeline-ovi trebaju buildati i testirati svaki commit i svaki merge kako bi se problemi i konflikti otkrili na vrijeme


## Zaključak

Odabrani pristup omogućava stabilan, pregledan i fleksibilan proces razvoja, prilagođen potrebama projekta i veličini tima. Jednostavnost GitHub Flow-a, uz brzu integraciju izmjena i kontinuiranu isporuku, čini ga dobrim izborom za razvoj sistema za organizaciju konferencija.
