# Proof of Testing — Sprint 7
  
**Test framework backend:** xUnit  
**Biblioteka za mockovanje backend:** Moq  
**Test framework frontend:** Vitest  
**Biblioteka za UI testiranje:** React Testing Library  
**Ukupan broj backend testova dodanih u Sprintu 7:** 21  
**Ukupan broj frontend testova:** 15  
**Ukupan broj Sprint 7 testova:** 36

---

## 1. SessionServiceTests

**Klasa koja se testira:** `SessionService`  
**Zavisnosti koje se mockuju:** `ISessionRepository`, `IUserRepository`, `ISessionRegistrationRepository`, `IUserContextService`

### 1.1 Kreiranje sesije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 1 | `CreateSessionAsync_ValidData_CreatesSession` | Poslan validan DTO za kreiranje sesije bez preklapanja termina | Vraća novi `SessionId`; `AddAsync` i `SaveChangesAsync` pozvani jednom |
| 2 | `CreateSessionAsync_EndBeforeStart_ReturnsNull` | Vrijeme završetka je prije vremena početka | Vraća `null`; sesija se ne dodaje u repozitorij |
| 3 | `CreateSessionAsync_OverlappingSession_ReturnsNull` | Repozitorij javlja da postoji preklapanje termina u istoj sali | Vraća `null`; `AddAsync` nije pozvan |

### 1.2 Ažuriranje sesije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 4 | `UpdateSessionAsync_ExistingSession_UpdatesSuccessfully` | Postojeća sesija se ažurira validnim podacima | Vraća `true`; sesija dobija nove vrijednosti; `UpdateAsync` i `SaveChangesAsync` pozvani jednom |
| 5 | `UpdateSessionAsync_SessionNotFound_ReturnsFalse` | Sesija sa datim ID-em ne postoji | Vraća `false` |

### 1.3 Brisanje sesije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 6 | `DeleteSessionAsync_ExistingSession_DeletesSuccessfully` | Sesija postoji u repozitoriju | Vraća `true`; `DeleteAsync` i `SaveChangesAsync` pozvani jednom |
| 7 | `DeleteSessionAsync_NotFound_ReturnsFalse` | Sesija sa datim ID-em ne postoji | Vraća `false` |

### 1.4 Dodjela predavača

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 8 | `AssignSpeakerAsync_ValidSpeaker_AssignsSpeaker` | Sesija postoji i korisnik ima rolu `predavac` | Vraća `true`; kreira se `SessionRegistration` sa `IsSpeaker = true` i statusom `Confirmed` |
| 9 | `AssignSpeakerAsync_UserIsNotSpeaker_ReturnsFalse` | Korisnik postoji, ali nema rolu `predavac` | Vraća `false`; predavač se ne dodjeljuje |

### 1.5 Prijava i odjava na sesiju

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 10 | `RegisterAsync_AlreadyConfirmed_ThrowsInvalidOperationException` | Korisnik je već prijavljen na sesiju sa statusom `Confirmed` | Baca `InvalidOperationException` |
| 11 | `RegisterAsync_CancelledRegistration_ReactivatesRegistration` | Korisnik ima prethodno otkazanu prijavu na sesiju | Status se mijenja u `Confirmed`; `UpdateAsync` i `SaveChangesAsync` pozvani |
| 12 | `CancelRegistrationAsync_WrongUser_ThrowsUnauthorizedAccessException` | Trenutni korisnik pokušava otkazati tuđu prijavu | Baca `UnauthorizedAccessException` |


<img width="1338" height="528" alt="image" src="https://github.com/user-attachments/assets/0b1cfd63-b558-4a48-b05c-35912e9a34e3" />

---

## 2. ConferenceRegistrationServiceTests

**Klasa koja se testira:** `ConferenceRegistrationService`  
**Zavisnosti koje se mockuju:** `IConferenceRepository`, `IConferenceRegistrationRepository`, `IUserContextService`

### 2.1 Prijava na konferenciju

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 13 | `RegisterAsync_ConferenceNotFound_ThrowsKeyNotFoundException` | Korisnik pokušava prijavu na konferenciju koja ne postoji | Baca `KeyNotFoundException` |
| 14 | `RegisterAsync_UserAlreadyConfirmed_ThrowsInvalidOperationException` | Korisnik je već prijavljen na konferenciju sa statusom `Confirmed` | Baca `InvalidOperationException` |
| 15 | `RegisterAsync_NoFreePlaces_ThrowsInvalidOperationException` | Broj potvrđenih prijava je jednak maksimalnom broju učesnika | Baca `InvalidOperationException` |
| 16 | `RegisterAsync_ValidRegistration_AddsRegistration` | Konferencija postoji, ima slobodnih mjesta i korisnik nije ranije prijavljen | Kreira se nova prijava sa statusom `Confirmed`; `AddAsync` i `SaveChangesAsync` pozvani |
| 17 | `RegisterAsync_CancelledRegistration_ReactivatesRegistration` | Korisnik ima prethodno otkazanu prijavu na konferenciju | Status se vraća na `Confirmed`; `UpdateAsync` i `SaveChangesAsync` pozvani |

### 2.2 Odjava sa konferencije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 18 | `CancelAsync_RegistrationNotFound_ThrowsKeyNotFoundException` | Prijava sa datim ID-em ne postoji | Baca `KeyNotFoundException` |
| 19 | `CancelAsync_WrongUser_ThrowsUnauthorizedAccessException` | Korisnik pokušava otkazati prijavu drugog korisnika | Baca `UnauthorizedAccessException` |
| 20 | `CancelAsync_ValidRegistration_SetsCancelledStatus` | Korisnik otkazuje vlastitu potvrđenu prijavu | Status prijave se mijenja u `Cancelled`; `UpdateAsync` i `SaveChangesAsync` pozvani |

<img width="1250" height="354" alt="image" src="https://github.com/user-attachments/assets/ebc76045-376d-4fbe-8202-ffc21869bed3" />

---

## 3. Frontend testovi

**Test runner:** Vitest  
**Biblioteke:** React Testing Library, user-event  
**Testirani fajlovi:** `ConferenceList.test.tsx`, `SessionList.test.tsx`, `SessionForm.test.tsx`, `ConferenceDetailsPage.test.tsx`

### 3.1 ConferenceList testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 21 | `renders conference card` | Renderuje se lista konferencija | Kartica prikazuje naziv, lokaciju i kategoriju konferencije |
| 22 | `shows participant apply button` | Korisnik ima rolu `ucesnik` | Prikazuje se dugme `Prijavi se` |
| 23 | `shows edit and delete buttons for admin or organizer` | Komponenta dobija `isAdminOrOrganizer = true` | Prikazuju se dugmad `Uredi` i `Obriši` |

### 3.2 SessionList testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 24 | `renders session card` | Renderuje se lista sesija | Kartica prikazuje naziv sesije, tip sesije i salu |
| 25 | `shows register button for participant` | Korisnik ima rolu `ucesnik` | Prikazuje se dugme `Prijavi se` |
| 26 | `calls registerForSession when participant clicks register` | Učesnik klikne dugme za prijavu na sesiju | Poziva se `registerForSession` sa ID-em sesije |
| 27 | `shows edit and delete buttons for admin or organizer` | Admin ili organizator otvara listu sesija | Prikazuju se dugmad `Uredi` i `Obriši` |
| 28 | `opens delete confirmation modal` | Admin/organizator klikne `Obriši` | Otvara se modal potvrde brisanja sesije |

### 3.3 SessionForm testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 29 | `renders create session form` | Otvara se forma za kreiranje sesije | Prikazuju se polja za naziv, opis, predavača i ostale podatke |
| 30 | `shows validation error for short title` | Korisnik unese naziv kraći od 3 karaktera | Prikazuje se validacijska greška za naziv |
| 31 | `creates session with valid data` | Korisnik unese validne podatke i odabere predavača | Poziva se `createSession`, zatim `assignSpeaker`, i izvršava se `onSuccess` |

### 3.4 ConferenceDetailsPage testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 32 | `renders conference details` | Otvara se stranica detalja konferencije | Prikazuju se naziv, lokacija i kategorija konferencije |
| 33 | `shows sessions button` | Korisnik je na detaljima konferencije | Prikazuje se dugme `Sesije` |
| 34 | `renders admin registrations section` | Admin otvara detalje konferencije | Prikazuje se sekcija `Prijavljeni učesnici` i podaci o prijavljenom korisniku |
| 35 | `filters registrations by search input` | Admin unosi email u search polje prijava | Prijavljeni korisnik ostaje prikazan nakon filtriranja |

<img width="1488" height="176" alt="image" src="https://github.com/user-attachments/assets/43edd1ee-7ffd-43d7-b1af-d07135ed0b8b" />

---

## 4. Pregled pokrivenosti Sprint 7 funkcionalnosti

| Oblast | Testovi | Kriterij prolaza |
|--------|---------|-----------------|
| Kreiranje sesija | 3 | Validno kreiranje, neispravan vremenski raspon i preklapanje termina pokriveni |
| Ažuriranje sesija | 2 | Uspješno ažuriranje i slučaj nepostojeće sesije pokriveni |
| Brisanje sesija | 2 | Uspješno brisanje i slučaj nepostojeće sesije pokriveni |
| Dodjela predavača | 2 | Validan predavač i korisnik bez role `predavac` pokriveni |
| Prijava/odjava na sesiju | 3 | Dupla prijava, reaktivacija otkazane prijave i zabrana otkazivanja tuđe prijave pokriveni |
| Prijava na konferenciju | 5 | Nepostojeća konferencija, dupla prijava, popunjena konferencija, validna prijava i reaktivacija otkazane prijave pokriveni |
| Odjava sa konferencije | 3 | Nepostojeća prijava, tuđa prijava i validna odjava pokriveni |
| Frontend prikaz konferencija | 3 | Kartice, dugme prijave i role-based edit/delete dugmad pokriveni |
| Frontend prikaz sesija | 5 | Kartice sesija, prijava na sesiju, edit/delete dugmad i modal potvrde brisanja pokriveni |
| Frontend forma sesije | 3 | Render forme, validacija i dodjela predavača kroz formu pokriveni |
| Frontend detalji konferencije | 4 | Detalji konferencije, dugme sesija, admin pregled prijava i filtriranje prijava pokriveni |
| **Ukupno** | **35+** | Backend i frontend testovi za Sprint 7 funkcionalnosti uspješno izvršeni |

---

## 5. Testno okruženje

| Postavka | Vrijednost |
|----------|------------|
| Backend test runner | xUnit |
| Backend mockovanje | Moq |
| Frontend test runner | Vitest |
| Frontend UI test biblioteka | React Testing Library |
| Frontend simulacija korisnika | `@testing-library/user-event` |
| Backend baza | Nije potrebna za unit testove jer se koriste mock repozitoriji |
| Frontend API pozivi | Mockovani kroz `vi.mock` i `vi.stubGlobal('fetch')` |
| Autentifikacija | Mockovan `useAuth` hook |
| Pokretanje backend testova | `dotnet test` |
| Pokretanje frontend testova | `npm test` |

---

## 6. Zaključak

Sprint 7 testiranje pokriva funkcionalnosti koje su implementirali Tim A i Tim B. Backend testovi provjeravaju poslovnu logiku za CRUD sesija, dodjelu predavača, prijavu/odjavu na konferenciju i prijavu/odjavu na sesiju. Frontend testovi provjeravaju prikaz sesija i konferencija, role-based dugmad, formu za kreiranje sesije, modal potvrde brisanja i admin pregled prijavljenih učesnika.

Svi frontend testovi su uspješno prošli, a backend testovi za Sprint 7 su također uspješno izvršeni.