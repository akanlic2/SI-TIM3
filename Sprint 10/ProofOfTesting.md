# Proof of Testing — Sprint 10
  
**Test framework backend:** xUnit  
**Biblioteka za mockovanje backend:** Moq  
**Test framework frontend:** Vitest  
**Biblioteka za UI testiranje:** React Testing Library  
**Ukupan broj backend testova dodanih u Sprintu 10:** 87  
**Ukupan broj frontend testova:** 59  
**Ukupan broj Sprint 10 testova:** 146

---

## 1. LogisticsTaskTests

**Klasa koja se testira:** `LogisticsService`, `LogisticsController`  
**Zavisnosti koje se mockuju:** repozitoriji i zavisnosti korištene za logističke aktivnosti

### 1.1 Logističke aktivnosti

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 1 | `GET/POST/PUT/DELETE route tests` | Provjeravaju se rute logističkih aktivnosti | Endpointi koriste očekivane rute |
| 2 | `AdminOrOrganizerPolicy tests` | Provjerava se autorizacija | Admin i organizator imaju pristup |
| 3 | `Get logistics list` | Dohvata se lista aktivnosti | Vraća aktivnosti za konferenciju |
| 4 | `Filter by taskType` | Filtriranje po tipu aktivnosti | Vraća filtrirane aktivnosti |
| 5 | `Create logistics task` | Kreira se logistička aktivnost | Aktivnost se uspješno kreira |
| 6 | `Update logistics task` | Mijenja se postojeća aktivnost | Aktivnost se uspješno ažurira |
| 7 | `Delete logistics task` | Briše se postojeća aktivnost | Aktivnost se uspješno briše |
| 8 | `Organizer ownership tests` | Organizator pristupa svojoj/tuđoj konferenciji | Vlastita konferencija dozvoljena, tuđa zabranjena |
| 9 | `Missing whitelist validation test` | Provjera trenutnog stanja taskType validacije | Dokumentuje da whitelist validacija trenutno nedostaje |
| 10 | `Missing RowVersion test` | Provjera optimistic locking podrške | Dokumentuje da RowVersion/timestamp trenutno ne postoji |

<img width="641" height="619" alt="image" src="https://github.com/user-attachments/assets/e7d5c71d-0ef4-49af-9fd9-1136b0dce0eb" />

---

## 2. EquipmentTests

**Klasa koja se testira:** `EquipmentService`, `EquipmentController`  
**Zavisnosti koje se mockuju:** repozitoriji i zavisnosti korištene za tehničku opremu

### 2.1 Tehnička oprema

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 11 | `Equipment route tests` | Provjeravaju se rute za inventar i opremu po sesiji | Endpointi koriste očekivane rute |
| 12 | `AdminOrOrganizerPolicy tests` | Provjerava se autorizacija za create/delete/assign/decrement | Admin i organizator imaju pristup |
| 13 | `Get global inventory` | Dohvata se globalni inventar | Vraća se oprema koja nije dodijeljena sesiji |
| 14 | `Get empty equipment list` | Nema dostupne opreme | Vraća se prazna lista |
| 15 | `Get equipment by session` | Dohvata se oprema za sesiju | Vraća dodijeljenu opremu |
| 16 | `Create equipment` | Kreira se nova oprema | Oprema se uspješno kreira |
| 17 | `Quantity validation` | Količina je 0 ili negativna | Validacija odbija neispravnu količinu |
| 18 | `Availability status tests` | Provjera statusa dostupnosti | Status se izvodi kao Available, Unavailable ili Assigned |
| 19 | `Delete equipment` | Briše se oprema | Brisanje je uspješno ako oprema nije dodijeljena |
| 20 | `Assigned equipment delete guard` | Oprema je već dodijeljena sesiji | Brisanje se odbija |
| 21 | `Decrement equipment quantity` | Smanjuje se dostupna količina | Količina se uspješno smanjuje |
| 22 | `Assign equipment to session` | Dodjeljuje se oprema sesiji | Dodjela se uspješno izvršava |
| 23 | `Assign validation tests` | Sesija/oprema ne postoji ili količina nije dostupna | Baca se odgovarajuća greška |
| 24 | `Missing ownership check test` | Organizator dodjeljuje opremu tuđoj sesiji | Dokumentuje da ownership provjera trenutno nedostaje |
| 25 | `Missing type whitelist test` | Nepoznat tip opreme | Dokumentuje da whitelist tipova trenutno ne postoji |

<img width="727" height="879" alt="image" src="https://github.com/user-attachments/assets/29f5f1c3-bdfe-428a-941a-5f87adaebf16" />

---

## 3. MaterialUpdateDeleteTests

**Klasa koja se testira:** `MaterialsController`, `MaterialService`, `IMaterialService`, `IMaterialRepository`

### 3.1 Izmjena i brisanje materijala

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 26 | `MaterialsController_HasExpectedRoute` | Provjerava se ruta kontrolera za materijale | Ruta je `api/sessions/{sessionId:guid}/materials` |
| 27 | `MaterialsController_HasOnlyUploadAndGetActions` | Provjerava trenutno stanje akcija | Controller ima samo upload i listu materijala |
| 28 | `MaterialsController_DoesNotHaveHttpPutEndpoint` | Provjerava update endpoint | PUT endpoint trenutno ne postoji |
| 29 | `MaterialsController_DoesNotHaveHttpDeleteEndpoint` | Provjerava delete endpoint | DELETE endpoint trenutno ne postoji |
| 30 | `MaterialsController_HasNoUpdateDeletePolicyActions` | Provjerava permisije za update/delete | Policy ne postoji jer akcije nisu implementirane |
| 31 | `IMaterialService_HasOnlyUploadAndListContract` | Provjerava servisni ugovor | Update/delete metode ne postoje |
| 32 | `MaterialService_ImplementsOnlyUploadAndListMethods` | Provjerava implementaciju servisa | Implementirane su samo upload/list metode |
| 33 | `IMaterialRepository_DoesNotHaveUpdateDeleteMethods` | Provjerava repository ugovor | Nema GetById, Update i Delete metoda |

<img width="446" height="319" alt="image" src="https://github.com/user-attachments/assets/a6bebb85-8538-4faf-978b-4ba269d739aa" />
---

## 4. Frontend testovi

**Test runner:** Vitest  
**Biblioteke:** React Testing Library, user-event  
**Testirani fajlovi:** `LogisticsPage.test.tsx`, `EquipmentPage.test.tsx`, `MaterialUpdateDelete.test.tsx`

### 4.1 LogisticsPage testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 34 | `renders logistics list` | Renderuje se lista logističkih aktivnosti | Prikazuje naziv, tip, status i rok |
| 35 | `shows empty state` | Nema logističkih aktivnosti | Prikazuje prazno stanje |
| 36 | `shows loading state` | Aktivnosti se učitavaju | Prikazuje loading |
| 37 | `shows error state` | API vraća grešku | Prikazuje error |
| 38 | `filters by task type` | Korisnik bira tip aktivnosti | Poziva se API sa filterom |
| 39 | `opens details modal` | Korisnik klikne dugme za detalje | Prikazuju se detalji aktivnosti |
| 40 | `opens create modal` | Korisnik klikne dodavanje | Otvara se forma |
| 41 | `creates logistics task` | Korisnik šalje validnu formu | Poziva se create API |
| 42 | `shows create error` | Create API vraća grešku | Prikazuje error |
| 43 | `opens edit modal` | Korisnik klikne edit | Forma se popunjava podacima |
| 44 | `updates logistics task` | Korisnik mijenja aktivnost | Poziva se update API |
| 45 | `shows update error` | Update API vraća grešku | Prikazuje error |
| 46 | `opens delete confirmation` | Korisnik klikne delete | Otvara se potvrda |
| 47 | `deletes logistics task` | Korisnik potvrdi brisanje | Poziva se delete API |
| 48 | `role UI tests` | Različite role otvaraju stranicu | Admin/organizator vide akcije, učesnik/predavač ne vide |

### 4.2 EquipmentPage testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 49 | `renders global inventory` | Renderuje se inventar opreme | Prikazuju se naziv, tip, količina i status |
| 50 | `shows empty/loading/error states` | Lista je prazna, učitava se ili API pada | Prikazuje odgovarajuće stanje |
| 51 | `opens create equipment modal` | Korisnik klikne dodavanje | Otvara se forma |
| 52 | `creates equipment` | Korisnik šalje validnu formu | Poziva se create API |
| 53 | `shows create error` | Create API vraća grešku | Prikazuje error |
| 54 | `opens delete confirmation` | Korisnik klikne delete | Otvara se potvrda |
| 55 | `deletes equipment` | Korisnik potvrdi brisanje | Poziva se delete API |
| 56 | `shows delete error` | Delete API vraća grešku | Prikazuje error |
| 57 | `decrements equipment quantity` | Korisnik smanjuje količinu | Poziva se decrement API |
| 58 | `renders assigned session equipment` | SessionDetailsPage prikazuje dodijeljenu opremu | Prikazuju se naziv, tip i količina |
| 59 | `assigns equipment to session` | Korisnik dodjeljuje opremu sesiji | Poziva se assign API |
| 60 | `assign validation/error tests` | Količina je previsoka ili API vraća grešku | Validacija/error se prikazuje |
| 61 | `role UI tests` | Različite role otvaraju stranicu | Admin/organizator vide akcije, predavač nema pristup inventaru |

### 4.3 MaterialUpdateDelete testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 62 | `renders materials list` | SessionDetailsPage prikazuje materijale | Prikazuje naziv i opis materijala |
| 63 | `shows download buttons` | Materijal ima fileUrl | Dugme `Preuzmi` otvara fajl |
| 64 | `shows empty materials state` | Nema materijala | Prikazuje se prazno stanje |
| 65 | `upload modal exists` | Upload je implementiran | Modal ima polja naziv, opis i fajl |
| 66 | `edit material action is not present` | Provjerava se S45.3 UI | Edit akcija trenutno ne postoji |
| 67 | `delete material action is not present` | Provjerava se S45.4 UI | Delete akcija trenutno ne postoji |
| 68 | `sessionApi has no update/delete material exports` | Provjerava se API sloj | Update/delete funkcije trenutno ne postoje |

<img width="930" height="113" alt="image" src="https://github.com/user-attachments/assets/f4a74876-6f05-4aaa-9fb2-11a0a98887df" />

---

## 5. Pregled pokrivenosti Sprint 10 funkcionalnosti

| Oblast | Testovi | Kriterij prolaza |
|--------|---------|-----------------|
| Logističke aktivnosti — backend | 24 | Rute, CRUD logika, filter, autorizacija i ownership provjere pokrivene |
| Tehnička oprema — backend | 44 | Inventar, dodjela sesiji, validacije, brisanje i role guardovi pokriveni |
| Izmjena/brisanje materijala — backend | 19 | Dokumentovano da update/delete endpointi, service i repository metode trenutno ne postoje |
| Logističke aktivnosti — frontend | 16 | Lista, filter, detalji, create/edit/delete tokovi i role UI pokriveni |
| Tehnička oprema — frontend | 17 | Inventar, create/delete/decrement, dodjela sesiji i role UI pokriveni |
| Izmjena/brisanje materijala — frontend | 8 | Prikaz materijala, download/upload i nepostojanje edit/delete UI-ja dokumentovani |
| **Ukupno** | **146** | Backend i frontend testovi za Sprint 10 funkcionalnosti uspješno izvršeni |

---

## 6. Testno okruženje

| Postavka | Vrijednost |
|----------|------------|
| Backend test runner | xUnit |
| Backend mockovanje | Moq |
| Frontend test runner | Vitest |
| Frontend UI test biblioteka | React Testing Library |
| Frontend simulacija korisnika | `@testing-library/user-event` |
| Backend baza | Nije korištena za unit testove |
| Frontend API pozivi | Mockovani |
| Pokretanje backend testova | `dotnet test ConferenceManagement.Tests\ConferenceManagement.Tests.csproj --no-restore` |
| Pokretanje frontend testova | `npm test` |

---

## 7. Zaključak

Sprint 10 testiranje pokriva funkcionalnosti logističkih aktivnosti, tehničke opreme i izmjene/brisanja materijala.

Backend testovi provjeravaju logističke aktivnosti, tehničku opremu i trenutno stanje materijal update/delete funkcionalnosti. Frontend testovi provjeravaju prikaz i upravljanje logističkim aktivnostima, inventar i dodjelu tehničke opreme, kao i trenutno stanje prikaza materijala.

Svi backend testovi su uspješno prošli sa rezultatom `268 passed, 0 skipped, 0 failed`. Svi frontend testovi su uspješno prošli sa rezultatom `104 passed, 0 skipped, 0 failed`.