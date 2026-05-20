# Proof of Testing — Sprint 8
  
**Test framework backend:** xUnit  
**Biblioteka za mockovanje backend:** Moq  
**Test framework frontend:** Vitest  
**Biblioteka za UI testiranje:** React Testing Library  
**Ukupan broj backend testova dodanih u Sprintu 8:** 50  
**Ukupan broj frontend testova:** 46  
**Ukupan broj Sprint 8 testova:** 96

---

## 1. RoomsControllerTests

**Klasa koja se testira:** `RoomsController`  
**Zavisnosti koje se mockuju:** autorizacijski policy i test konfiguracija za role

### 1.1 Upravljanje dvoranama

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 1 | `RoomsActions_RequireAdminOrOrganizerPolicy` | CRUD akcije koriste `AdminOrOrganizerPolicy` | CRUD akcije dostupne samo adminu i organizatoru |
| 2 | `PostPutDelete_RequireAdminOrOrganizerPolicy` | Provjerava autorizaciju za POST, PUT i DELETE | Pristup imaju admin i organizator |
| 3 | `RoomsController_HasExpectedApiRoute` | Provjerava baznu rutu kontrolera | Koristi očekivanu baznu rutu |
| 4 | `GetAllRooms_UsesHttpGet` | Provjerava GET endpoint | Koristi HTTP GET |
| 5 | `CreateRoom_UsesHttpPost` | Provjerava POST endpoint | Koristi HTTP POST |
| 6 | `UpdateRoom_UsesHttpPutWithIdRoute` | Provjerava PUT endpoint | Koristi HTTP PUT sa ID parametrom |
| 7 | `DeleteRoom_UsesHttpDeleteWithIdRoute` | Provjerava DELETE endpoint | Koristi HTTP DELETE sa ID parametrom |
| 8 | `AdminOrOrganizerPolicy_AllowsOnlyExpectedRoles` | Provjerava role pristupa | `admin-sistema` i `organizator` imaju pristup |

<img width="670" height="401" alt="image" src="https://github.com/user-attachments/assets/99a39b21-b249-491d-8f62-4f2f24ebe25b" />

---

## 2. SessionRoomAssignmentTests

**Klasa koja se testira:** `SessionsController`  
**Zavisnosti koje se mockuju:** autorizacijski policy i test konfiguracija za role

### 2.1 Dodjela dvorane sesiji

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 9 | `AssignRoomToSession_UsesHttpPutWithExpectedRoute` | Provjerava endpoint za dodjelu dvorane | Koristi rutu `PUT /sessions/{id}/room` |
| 10 | `AssignRoomToSession_RequiresAdminOrOrganizerPolicy` | Provjerava autorizaciju | Dodjela dostupna adminu i organizatoru |
| 11 | `SessionsController_UsesExpectedBaseRoute` | Provjerava baznu rutu | Koristi očekivanu baznu rutu |
| 12 | `AdminOrOrganizerPolicy_AllowsOnlyExpectedRoles` | Provjerava role pristupa | Admin i organizator imaju pristup |

<img width="597" height="222" alt="image" src="https://github.com/user-attachments/assets/f5979335-217d-40b0-81e2-74ac8d500b18" />

---

## 3. AgendaItemCrudTests

**Klasa koja se testira:** `AgendaItemService`  
**Zavisnosti koje se mockuju:** repozitoriji i zavisnosti korištene u servisu

### 3.1 Agenda konferencije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 13 | `CreateAsync_SupportedNonSessionTypes_CreateAgendaItem` | Kreiranje Break, Lunch, Networking, Opening i Closing stavki | Stavke se uspješno kreiraju |
| 14 | `CreateAsync_SessionTypeWithoutSessionId_ThrowsArgumentException` | Session tip bez SessionId | Baca grešku |
| 15 | `CreateAsync_SessionTypeWithExistingSession_CreatesAgendaItem` | Session tip sa validnom sesijom | Agenda stavka se kreira |
| 16 | `GetByConferenceIdAsync_ReturnsMappedAgendaItemsIncludingSessionData` | Dohvatanje podataka o sesiji | DTO sadrži očekivane podatke |
| 17 | `UpdateAsync_ExistingAgendaItem_ChangesTimeTitleDescriptionAndType` | Izmjena stavke | Uspješna izmjena |
| 18 | `UpdateAsync_AgendaItemDoesNotExist_ThrowsKeyNotFoundException` | Stavka ne postoji | Baca grešku |
| 19 | `DeleteAsync_ExistingAgendaItem_DeletesAgendaItem` | Brisanje stavke | Stavka se briše |
| 20 | `DeleteAsync_AgendaItemDoesNotExist_ThrowsKeyNotFoundException` | Stavka ne postoji | Baca grešku |

<img width="602" height="600" alt="image" src="https://github.com/user-attachments/assets/ecb90989-24f1-44a2-a04b-49a68f5cab76" />

---

## 4. ConferenceCapacityParticipantsTests

**Klasa koja se testira:** `ConferenceCapacityService`  
**Zavisnosti koje se mockuju:** repozitoriji za kapacitet i učesnike

### 4.1 Kapacitet konferencije i sesije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 21 | `GetConferenceCapacityAsync_ReturnsRegisteredMaxAvailableAndFullStatus` | Dohvatanje kapaciteta konferencije | Vraća prijavljene, maksimum i status |
| 22 | `GetConferenceCapacityAsync_ReturnsIsFullWhenRegisteredCountReachesCapacity` | Konferencija popunjena | `IsFull = true` |
| 23 | `GetConferenceCapacityAsync_ConferenceDoesNotExist_ThrowsKeyNotFoundException` | Konferencija ne postoji | Baca grešku |
| 24 | `GetSessionCapacityAsync_ReturnsRegisteredMaxAvailableAndFullStatus` | Session capacity | Vraća capacity podatke |
| 25 | `GetSessionCapacityAsync_ReturnsIsFullWhenSessionRegistrationCountReachesCapacity` | Sesija popunjena | `IsFull = true` |
| 26 | `GetSessionCapacityAsync_SessionDoesNotExist_ThrowsKeyNotFoundException` | Sesija ne postoji | Baca grešku |

### 4.2 Lista učesnika

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 27 | `GetConferenceParticipantsAsync_ReturnsRegisteredParticipantsWithNameEmailAndStatus` | Dohvatanje učesnika | Vraća ime, email i status |
| 28 | `GetConferenceParticipantsAsync_SupportsSearchByNameAndEmail` | Search funkcionalnost | Filtrira listu |
| 29 | `GetConferenceParticipantsAsync_SupportsStatusFilter` | Filter statusa | Filtrira listu |
| 30 | `GetConferenceParticipantsAsync_ReturnsEmptyListWhenNoParticipants` | Nema učesnika | Vraća praznu listu |
| 31 | `GetConferenceParticipantsAsync_ConferenceDoesNotExist_ThrowsKeyNotFoundException` | Konferencija ne postoji | Baca grešku |
| 32 | `GetParticipants_AdminCanSeeParticipantsForAnyConference` | Admin pristup | Admin vidi učesnike |
| 33 | `GetParticipants_OrganizerCanSeeOwnConferenceParticipants` | Organizator pristup | Organizator vidi svoje |
| 34 | `GetParticipants_OrganizerCannotSeeOtherConferenceParticipants` | Organizator pristupa tuđoj konferenciji | Pristup odbijen |

<img width="742" height="748" alt="image" src="https://github.com/user-attachments/assets/b4558a40-ce44-4032-a19c-7caa593f42cd" />

---

## 5. Frontend testovi

**Test runner:** Vitest  
**Biblioteke:** React Testing Library, user-event  
**Testirani fajlovi:** `RoomsPage.test.tsx`, `SessionForm.test.tsx`, `AgendaPage.test.tsx`, `ConferenceDetailsPage.test.tsx`

### 5.1 RoomsPage testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 35 | `renders rooms list` | Renderuje se lista dvorana | Prikazuje naziv, lokaciju i kapacitet |
| 36 | `shows empty state` | Nema dvorana | Prikazuje prazno stanje |
| 37 | `shows loading state` | Učitavanje dvorana | Prikazuje loading |
| 38 | `shows error state` | API vraća grešku | Prikazuje error |
| 39 | `opens AddRoomModal` | Klik na dodavanje | Otvara modal |
| 40 | `creates room` | Dodavanje dvorane | Poziva create API |
| 41 | `updates room` | Izmjena dvorane | Poziva update API |
| 42 | `deletes room` | Brisanje dvorane | Poziva delete API |

### 5.2 SessionForm testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 43 | `loads rooms` | Učitavanje dvorana | Dvorane se prikazuju |
| 44 | `shows room dropdown` | Forma sesije | Dropdown postoji |
| 45 | `assigns room` | Dodjela dvorane | Poziva assign API |
| 46 | `shows room load error` | API greška | Prikazuje error |

### 5.3 AgendaPage testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 47 | `renders agenda list` | Prikaz agende | Agenda je prikazana |
| 48 | `shows empty agenda state` | Agenda ne postoji | Prikazuje prazno stanje |
| 49 | `creates agenda item` | Dodavanje stavke | Poziva create API |
| 50 | `updates agenda item` | Izmjena stavke | Poziva update API |
| 51 | `deletes agenda item` | Brisanje stavke | Poziva delete API |

### 5.4 ConferenceDetailsPage testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 52 | `renders capacity widget` | Capacity widget | Prikazuje podatke |
| 53 | `shows capacity values` | Capacity API | Prikazuje maksimum i prijavljene |
| 54 | `filters participants by name` | Search po imenu | Filtrira listu |
| 55 | `filters participants by email` | Search po emailu | Filtrira listu |
| 56 | `filters participants by status` | Filter statusa | Filtrira listu |
| 57 | `shows empty participants state` | Nema učesnika | Prikazuje prazno stanje |

<img width="892" height="115" alt="image" src="https://github.com/user-attachments/assets/b23d48b1-d516-4a90-a2f0-e2165137a45e" />

---

## 6. Pregled pokrivenosti Sprint 8 funkcionalnosti

| Oblast | Testovi | Kriterij prolaza |
|--------|---------|-----------------|
| Upravljanje dvoranama | 8 | CRUD rute i autorizacija pokriveni |
| Dodjela dvorane sesiji | 4 | Ruta i autorizacija pokriveni |
| Agenda konferencije | 8 | Kreiranje, izmjena i brisanje pokriveni |
| Kapacitet konferencije/sesije | 6 | Capacity funkcionalnosti pokrivene |
| Lista učesnika | 8 | Search, filter i autorizacija pokriveni |
| Frontend upravljanje dvoranama | 8 | Lista, modal i CRUD pokriveni |
| Frontend dodjela dvorane | 4 | Dropdown i assign API pokriveni |
| Frontend agenda | 5 | Prikaz i CRUD pokriveni |
| Frontend kapacitet i učesnici | 6 | Capacity i participants pokriveni |
| **Ukupno** | **96** | Backend i frontend testovi uspješno izvršeni |

---

## 5. Testno okruženje

| Postavka | Vrijednost |
|----------|------------|
| Backend test runner | xUnit |
| Backend mockovanje | Moq |
| Frontend test runner | Vitest |
| Frontend UI test biblioteka | React Testing Library |
| Frontend simulacija korisnika | `@testing-library/user-event` |
| Backend baza | Nije potrebna za unit testove |
| Frontend API pozivi | Mockovani kroz `vi.mock` |
| Autentifikacija | Mockovan `useAuth` hook |
| Pokretanje backend testova | `dotnet test` |
| Pokretanje frontend testova | `npm test` |

---

## 6. Zaključak

Sprint 8 testiranje pokriva funkcionalnosti upravljanja dvoranama, dodjele dvorane sesiji, agende konferencije, kapaciteta konferencije i liste učesnika.

Backend testovi provjeravaju CRUD rute, autorizaciju i poslovnu logiku za agendu, kapacitet i učesnike. Frontend testovi provjeravaju upravljanje dvoranama, dodjelu dvorane sesiji, agendu, capacity widget i pregled učesnika.

Svi frontend testovi su uspješno prošli, a backend testovi za Sprint 8 su također uspješno izvršeni.