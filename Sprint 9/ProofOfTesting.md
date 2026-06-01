# Proof of Testing — Sprint 9
  
**Test framework backend:** xUnit  
**Biblioteka za mockovanje backend:** Moq  
**Test framework frontend:** Vitest  
**Biblioteka za UI testiranje:** React Testing Library  
**Ukupan broj backend testova dodanih u Sprintu 9:** 29  
**Ukupan broj frontend testova dodanih u Sprintu 9:** 5  
**Ukupan broj Sprint 9 testova:** 34

---

## 1. NotificationServiceTests

**Klasa koja se testira:** `NotificationService`  
**Zavisnosti koje se mockuju:** `INotificationRepository`, `IUserContextService`, `IUserRepository`

### 1.1 Kreiranje i dohvat notifikacija

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 1 | `CreateNotificationAsync_UserNotFound_ThrowsKeyNotFoundException` | Notifikacija se kreira za korisnika koji ne postoji | Baca `KeyNotFoundException` |
| 2 | `CreateNotificationAsync_ValidData_CreatesUnreadNotification` | Kreira se validna notifikacija | Notifikacija se kreira kao nepročitana |
| 3 | `GetMyNotificationsAsync_ReturnsCurrentUserNotifications` | Trenutni korisnik dohvaća svoje notifikacije | Vraća listu njegovih notifikacija |

### 1.2 Označavanje notifikacija pročitanim

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 4 | `MarkAsReadAsync_NotificationNotFound_ThrowsKeyNotFoundException` | Notifikacija ne postoji | Baca `KeyNotFoundException` |
| 5 | `MarkAsReadAsync_NotificationBelongsToOtherUser_ThrowsUnauthorizedAccessException` | Korisnik pokušava označiti tuđu notifikaciju | Baca `UnauthorizedAccessException` |
| 6 | `MarkAsReadAsync_UnreadNotification_MarksAsRead` | Nepročitana notifikacija se označava pročitanom | `IsRead = true` |
| 7 | `MarkAllAsReadAsync_NoUnreadNotifications_DoesNotSave` | Korisnik nema nepročitanih notifikacija | Ne poziva se `SaveChangesAsync` |
| 8 | `MarkAllAsReadAsync_UnreadNotifications_MarksAllAsRead` | Korisnik ima više nepročitanih notifikacija | Sve notifikacije postaju pročitane |

<img width="1668" height="476" alt="image" src="https://github.com/user-attachments/assets/d9244ff3-53ec-4e9f-b432-8fa7e0151d02" />

---

## 2. QuestionServiceTests

**Klasa koja se testira:** `QuestionService`  
**Zavisnosti koje se mockuju:** `IQuestionRepository`, `ISessionRepository`, `IUserContextService`, `IUserRepository`, `INotificationService`

### 2.1 Postavljanje pitanja

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 9 | `CreateQuestionAsync_SessionNotFound_ThrowsKeyNotFoundException` | Korisnik postavlja pitanje za nepostojeću sesiju | Baca `KeyNotFoundException` |
| 10 | `CreateQuestionAsync_SessionNotStarted_ThrowsInvalidOperationException` | Korisnik pokušava postaviti pitanje prije početka sesije | Baca `InvalidOperationException` |
| 11 | `CreateQuestionAsync_EmptyContent_ThrowsArgumentException` | Pitanje je prazno | Baca `ArgumentException` |
| 12 | `CreateQuestionAsync_ContentLongerThan500_ThrowsArgumentException` | Pitanje ima više od 500 karaktera | Baca `ArgumentException` |
| 13 | `CreateQuestionAsync_ValidQuestion_CreatesQuestionAndNotifiesSpeaker` | Korisnik postavlja validno pitanje nakon početka sesije | Kreira se pitanje sa statusom `Open` i šalje notifikacija predavaču |

### 2.2 Dohvat pitanja

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 14 | `GetQuestionsBySessionAsync_SessionNotFound_ThrowsKeyNotFoundException` | Sesija ne postoji | Baca `KeyNotFoundException` |
| 15 | `GetQuestionsBySessionAsync_ExistingSession_ReturnsQuestions` | Sesija postoji i ima pitanja | Vraća listu pitanja sa autorom, statusom i odgovorom |

### 2.3 Odgovaranje na pitanja

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 16 | `AnswerQuestionAsync_EmptyAnswerAndNotOral_ThrowsArgumentException` | Predavač ne unese odgovor i ne označi odgovoreno usmeno | Baca `ArgumentException` |
| 17 | `AnswerQuestionAsync_QuestionNotFound_ThrowsKeyNotFoundException` | Pitanje ne postoji | Baca `KeyNotFoundException` |
| 18 | `AnswerQuestionAsync_QuestionDoesNotBelongToSession_ThrowsArgumentException` | Pitanje ne pripada datoj sesiji | Baca `ArgumentException` |
| 19 | `AnswerQuestionAsync_SessionNotFound_ThrowsKeyNotFoundException` | Sesija ne postoji | Baca `KeyNotFoundException` |
| 20 | `AnswerQuestionAsync_UserIsNotAssignedSpeaker_ThrowsUnauthorizedAccessException` | Korisnik nije dodijeljeni predavač sesije | Baca `UnauthorizedAccessException` |
| 21 | `AnswerQuestionAsync_AssignedSpeaker_AnswersQuestionAndNotifiesAuthor` | Dodijeljeni predavač odgovara na pitanje | Status pitanja postaje `Answered` i autor pitanja dobija notifikaciju |

---

## 3. MaterialServiceTests

**Klasa koja se testira:** `MaterialService`  
**Zavisnosti koje se mockuju:** `ISessionRepository`, `ISessionRegistrationRepository`, `IUserContextService`, `IMaterialRepository`

### 3.1 Upload materijala

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 22 | `UploadMaterialAsync_SessionNotFound_ThrowsKeyNotFoundException` | Upload se pokušava za sesiju koja ne postoji | Baca `KeyNotFoundException` |
| 23 | `UploadMaterialAsync_AttendeeWithoutPermission_ThrowsUnauthorizedAccessException` | Učesnik bez permisije pokušava upload | Baca `UnauthorizedAccessException` |
| 24 | `UploadMaterialAsync_SpeakerNotAssignedToSession_ThrowsUnauthorizedAccessException` | Predavač nije dodijeljen toj sesiji | Baca `UnauthorizedAccessException` |
| 25 | `UploadMaterialAsync_AssignedSpeaker_UploadsMaterial` | Dodijeljeni predavač uploaduje materijal | Materijal se dodaje i čuva u repozitorij |
| 26 | `UploadMaterialAsync_AdminOrOrganizer_UploadsMaterial` | Admin ili organizator uploaduje materijal | Upload je uspješan |

### 3.2 Pregled materijala

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 27 | `GetMaterialsBySessionIdAsync_UserNotRegisteredAndNotAdmin_ThrowsUnauthorizedAccessException` | Korisnik nije prijavljen na sesiju i nije admin/organizator | Baca `UnauthorizedAccessException` |
| 28 | `GetMaterialsBySessionIdAsync_RegisteredUser_ReturnsMaterials` | Prijavljeni korisnik pregleda materijale sesije | Vraća listu materijala |
| 29 | `GetMaterialsBySessionIdAsync_AdminOrOrganizer_ReturnsMaterials` | Admin ili organizator pregleda materijale | Vraća listu materijala |

---

## 4. Frontend testovi

**Test runner:** Vitest  
**Biblioteke:** React Testing Library, user-event  
**Testirani fajlovi:** `NotificationBell.test.tsx`

### 4.1 NotificationBell testovi

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 30 | `shows unread notifications counter` | Postoji jedna nepročitana notifikacija | Prikazuje se brojač `1` |
| 31 | `opens notification dropdown on click` | Korisnik klikne na ikonu notifikacija | Otvara se dropdown sa listom notifikacija |
| 32 | `shows mark all as read button when unread notifications exist` | Postoje nepročitane notifikacije | Prikazuje se dugme `Označi sve kao pročitano` |
| 33 | `calls markAllAsRead when clicking mark all button` | Korisnik klikne na označavanje svih kao pročitanih | Poziva se `markAllAsRead` |
| 34 | `marks unread notification as read on click` | Korisnik klikne na nepročitanu notifikaciju | Poziva se `markAsRead` sa ID-em notifikacije |

---

## 5. Pregled pokrivenosti Sprint 9 funkcionalnosti

| Oblast | Testovi | Kriterij prolaza |
|--------|---------|-----------------|
| Notifikacije backend | 8 | Kreiranje, dohvat, pojedinačno i grupno označavanje pročitanim pokriveno |
| Q&A backend | 13 | Postavljanje pitanja, zabrana prije početka sesije, validacija sadržaja, odgovaranje i permisije predavača pokriveni |
| Materijali backend | 8 | Upload materijala, permisije predavača/admina/organizatora i pregled materijala pokriveni |
| Notifikacije frontend | 5 | Brojač, dropdown, označavanje pročitanim i klik na notifikaciju pokriveni |
| **Ukupno** | **34** | Backend i frontend testovi za Sprint 9 funkcionalnosti izvršeni |

---

## 6. Testno okruženje

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
| Pokretanje backend testova | `dotnet test ConferenceManagement.Tests/ConferenceManagement.Tests.csproj` |
| Pokretanje frontend testova | `npm test` |

---

## 7. Zaključak

Sprint 9 testiranje pokriva funkcionalnosti predavačkog dashboarda, Q&A panela, upload materijala i notifikacija.

Backend testovi provjeravaju poslovnu logiku za notifikacije, postavljanje i odgovaranje na pitanja, permisije predavača, upload materijala i pristup materijalima. Frontend testovi provjeravaju prikaz notifikacija, brojač nepročitanih notifikacija, dropdown listu i označavanje notifikacija pročitanim.

Backend testovi su uspješno prošli, dok frontend testovi za notifikacije prolaze odvojeno. Ostali postojeći frontend testovi iz prethodnih sprintova zahtijevaju manje prilagodbe zbog promjena UI-ja, ali nisu dio Sprint 9 taskova.