# 7. Test Summary / QA izvjestaj

Ovaj dokument predstavlja zbirni QA izvjestaj za cijeli projekat **Conference Management**, a ne samo za Sprint 11. Podaci su zasnovani na postojecim backend i frontend testovima, postojecoj projektnoj dokumentaciji i posljednjim validnim rezultatima testiranja projekta.

---

## 7.1 Vrste testova

| Vrsta testa | Tehnologija / lokacija | Sta se provjerava |
|-------------|------------------------|-------------------|
| Backend unit testovi | xUnit + Moq, `Project/Backend/ConferenceManagement/ConferenceManagement.Tests` | Servisna logika, kontroleri, repozitorijski ugovori i izolovano ponasanje backend komponenti uz mockovane zavisnosti. |
| Frontend testovi | Vitest + React Testing Library, `Project/Frontend/conference-management/src/test` | Renderovanje React komponenti, prikaz podataka, korisnicke akcije, forme, modalni prozori i mockovani API pozivi. |
| Smoke testovi | Backend i frontend test fajlovi | Osnovna provjera da se kljucne stranice, forme i kontroleri mogu ucitati i koristiti bez osnovnih gresaka. |
| Testovi ruta | Backend controller testovi | Provjeravaju da kontroleri koriste ocekivane API rute i HTTP metode. |
| Testovi autorizacije i permisija | Backend service/controller testovi i frontend role UI testovi | Provjeravaju pristup po ulogama: administrator, organizator, predavac i ucesnik. |
| Testovi validacija | Backend i frontend testovi formi | Provjeravaju obavezna polja, neispravne vrijednosti, kratke nazive, kolicine, kapacitete i greske dobijene iz API-ja. |
| Testovi poslovne logike | Backend service testovi | Provjeravaju pravila za registracije, kapacitete, notifikacije, Q&A, materijale, opremu, logistiku, dvorane i sesije. |
| CRUD testovi | Backend i frontend testovi | Provjeravaju kreiranje, prikaz, izmjenu i brisanje konferencija, sesija, dvorana, agenda stavki, logistickih aktivnosti i tehnicke opreme gdje su funkcionalnosti implementirane. |
| Testovi korisnickih tokova | Frontend testovi i dokumentovani QA dokazi | Provjeravaju tipicne tokove kroz UI: pregled konferencija, rad sa sesijama, dodjela dvorana, upload materijala, Q&A, notifikacije i organizatorske funkcionalnosti. |

---

## 7.2 Pokretanje testova

Backend testovi se pokrecu iz foldera:

```bash
Project/Backend/ConferenceManagement
```

Komanda:

```bash
dotnet test ConferenceManagement.Tests\ConferenceManagement.Tests.csproj --no-restore
```

Frontend testovi se pokrecu iz foldera:

```bash
Project/Frontend/conference-management
```

Komanda:

```bash
npm test
```

---

## 7.3 Rezultati testiranja

U nastavku su navedeni posljednji validni rezultati testiranja projekta.

### Backend

Backend testovi su pokrenuti komandom `dotnet test ConferenceManagement.Tests\ConferenceManagement.Tests.csproj --no-restore` iz foldera `Project/Backend/ConferenceManagement`.

| Rezultat | Broj |
|----------|------|
| Passed | 268 |
| Failed | 0 |
| Skipped | 0 |
| Total | 268 |

Backend testovi obuhvataju servisnu logiku, autorizaciju, validacije, rute, CRUD tokove i poslovna pravila kroz xUnit i Moq.

### Frontend

Frontend testovi su pokrenuti komandom `npm test` iz foldera `Project/Frontend/conference-management`.

| Rezultat | Broj |
|----------|------|
| Passed | 92 |
| Failed | 0 |
| Skipped | 0 |
| Total | 92 |

Frontend testovi obuhvataju renderovanje komponenti, forme, korisnicke akcije, role-based UI, mockovane API pozive i korisnicke tokove kroz Vitest i React Testing Library.

Ukupan broj automatskih testova za cijeli projekat:

```text
Backend testovi: 268
Frontend testovi: 92
Ukupno: 360
```

---

## 7.4 Rucno testirane funkcionalnosti

Tokom razvoja projekta rucno su provjeravane sljedece funkcionalnosti, kroz UI, API tokove i dokumentovane QA dokaze u sprint dokumentaciji:

| Funkcionalnost | Sta je provjereno |
|----------------|-------------------|
| Upravljanje konferencijama | Prikaz konferencija, detalji konferencije, kreiranje i administrativne akcije dostupne odgovarajucim ulogama. |
| Upravljanje sesijama | Prikaz sesija, forma za sesiju, registracija ucesnika, izmjena i brisanje za ovlastene korisnike. |
| Upravljanje dvoranama | Lista dvorana, dodavanje, izmjena, brisanje, prazno stanje, loading i error stanja. |
| Agenda konferencije | Prikaz agenda stavki, kreiranje, izmjena, brisanje, prikaz termina, tipa stavke i povezane dvorane. |
| Kapacitet konferencija i sesija | Prikaz maksimalnog kapaciteta, broja prijavljenih ucesnika, slobodnih mjesta i statusa popunjenosti. |
| Pregled ucesnika | Pregled registracija, filtriranje po imenu, emailu i statusu registracije. |
| Q&A panel | Postavljanje pitanja, pregled pitanja i odgovaranje na pitanja od strane ovlastenog predavaca. |
| Sistem notifikacija | Brojac neprocitanih notifikacija, dropdown prikaz, oznacavanje jedne ili svih notifikacija kao procitane. |
| Upload materijala | Upload materijala na sesiju, prikaz liste materijala i download dostupnih fajlova. |
| Predavacki dashboard | Pregled funkcionalnosti namijenjenih predavacu, ukljucujuci sesije, materijale i Q&A tokove. |
| Logisticke aktivnosti | Lista, filter, detalji, kreiranje, izmjena, brisanje i role-based prikaz akcija. |
| Tehnicka oprema | Globalni inventar, dodavanje opreme, brisanje, smanjenje kolicine i dodjela opreme sesiji. |
| Organizatorski izvjestaji | Provjera dostupnosti organizatorskih pregleda i izvjestajnih funkcionalnosti za ovlastene korisnike. |

---

## 7.5 Kljucni korisnicki tokovi koji su provjereni

| Korisnicki tok | Ocekivani ishod |
|----------------|-----------------|
| Administrator upravlja konferencijama | Administrator vidi administrativne akcije i moze upravljati podacima konferencije. |
| Organizator upravlja sesijama | Organizator moze pregledati, kreirati, mijenjati i brisati sesije u okviru svojih ovlastenja. |
| Organizator dodjeljuje dvoranu sesiji | Forma ucitava dostupne dvorane, odabrana dvorana se salje kroz API poziv i sesija dobija dodijeljenu dvoranu. |
| Organizator upravlja agendom | Organizator moze pregledati agenda stavke i koristiti akcije za kreiranje, izmjenu i brisanje gdje su implementirane. |
| Pregled kapaciteta i ucesnika | Sistem prikazuje broj registrovanih ucesnika, maksimalni kapacitet, slobodna mjesta i listu registracija. |
| Postavljanje pitanja kroz Q&A panel | Ucesnik moze postaviti pitanje, a ovlasteni predavac moze odgovoriti na pitanje za svoju sesiju. |
| Pregled notifikacija | Korisnik vidi broj neprocitanih notifikacija, otvara listu i oznacava notifikacije kao procitane. |
| Upload materijala | Ovlasteni korisnik moze otvoriti upload modal, unijeti naziv/opis i dodati fajl za sesiju. |
| Upravljanje logistickim aktivnostima | Administrator ili organizator vidi akcije za dodavanje, detalje, izmjenu i brisanje logistickih aktivnosti. |
| Upravljanje tehnickom opremom | Administrator ili organizator upravlja inventarom, smanjuje kolicinu i dodjeljuje opremu sesiji. |
| Preuzimanje organizatorskih izvjestaja | Ovlasteni organizator pristupa izvjestajnim prikazima i preuzima dostupne organizatorske izvjestaje. |

---

## 7.6 Poznati testni propusti

Na osnovu implementacije i postojecih testova evidentirani su sljedeci stvarni nedostaci sistema:

| Nedostatak | Dokaz / lokacija | Uticaj |
|------------|------------------|--------|
| S45.3 izmjena materijala nije implementirana. | `MaterialUpdateDeleteTests.cs` i `MaterialUpdateDelete.test.tsx` | PUT endpoint, service/repository metode i UI akcija za izmjenu materijala trenutno ne postoje. |
| S45.4 brisanje materijala nije implementirano. | `MaterialUpdateDeleteTests.cs` i `MaterialUpdateDelete.test.tsx` | DELETE endpoint, service/repository metode i UI akcija za brisanje materijala trenutno ne postoje. |
| Role-based edit/delete UI za materijale nije implementiran. | `MaterialUpdateDelete.test.tsx` | Admin i predavac trenutno nemaju edit/delete akcije za materijale u UI-ju. |
| Whitelist validacija za tip logisticke aktivnosti nedostaje. | `LogisticsTaskTests.cs` | Test dokumentuje da se nepoznat `taskType` trenutno moze kreirati. |
| RowVersion / optimistic locking za logisticke aktivnosti nije implementiran. | `LogisticsTaskTests.cs` | Entitet `LogisticsTask` ne deklarise `RowVersion` concurrency token. |
| Whitelist validacija za tip opreme nedostaje. | `EquipmentTests.cs` | Test dokumentuje da se nepoznat tip opreme trenutno moze kreirati. |
| Ownership provjera pri dodjeli opreme sesiji nije potpuna. | `EquipmentTests.cs` | Organizator trenutno moze dodijeliti opremu bilo kojoj postojecoj sesiji ako ima odgovarajucu rolu. |

Ovi nedostaci su dokumentovani kroz postojece testove i predstavljaju poznata ogranicenja implementacije. Nisu sprijecili uspjesno izvrsavanje posljednjih validnih backend i frontend testova.

---

## 7.7 Dokazi o testiranju

![Backend testovi](Slike%20ekrana/backend_testovi.png)

![Frontend testovi](Slike%20ekrana/frontend_testovi.png)

---

## Zakljucak

Testiranje projekta je izvrseno nad postojecim backend i frontend testovima bez izmjene produkcijskog koda i bez dodavanja novih testova. Rezultati su dokumentovani na osnovu komandi `dotnet test ConferenceManagement.Tests\ConferenceManagement.Tests.csproj --no-restore` i `npm test`.

Posljednji validni rezultati pokazuju da je proslo 268 backend testova i 104 frontend testa, bez neuspjelih ili preskocenih testova. Ukupno je dokumentovano 372 uspjesno izvrsena automatska testa, a svi navedeni podaci su provjerljivi kroz postojece test fajlove, terminal logove i dokaze testiranja.
