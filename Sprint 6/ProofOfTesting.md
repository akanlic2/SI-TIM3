# Proof of Testing
  
**Test framework:** xUnit  
**Biblioteka za mockovanje:** Moq  
**Ukupan broj testova:** 58

---

## 1. UserServiceTests

**Klasa koja se testira:** `UserService`  
**Zavisnost koja se mockuje:** `IUserRepository`

### 1.1 Dohvatanje broja korisnika

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 1 | `GetUserCountAsync_Returns_Count_From_Repository` | Repozitorij vraća 5 korisnika | Vraća `5`; `GetCountAsync` pozvan tačno jednom |
| 2 | `GetUserCountAsync_Returns_Zero_When_No_Users_Exist` | Repozitorij vraća 0 | Vraća `0` |

### 1.2 Dohvatanje svih korisnika

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 3 | `GetAllUsersAsync_Returns_All_Users_Mapped_To_Dto` | Repozitorij vraća dva `User` entiteta | Vraća listu od 2 `UserDto` zapisa sa odgovarajućim UserId, Username i Email; `GetAllAsync` pozvan jednom |
| 4 | `GetAllUsersAsync_Returns_Empty_List_When_No_Users_Exist` | Repozitorij vraća praznu listu | Vraća nepraznu, praznu listu |
| 5 | `GetAllUsersAsync_Maps_All_User_Fields_Correctly` | Repozitorij vraća jednog korisnika sa svim popunjenim poljima | Sva polja na vraćenom `UserDto` (UserId, Username, FirstName, LastName, Email, Role, CreatedAt) tačno odgovaraju izvornom entitetu |

### 1.3 Dohvatanje korisnika po ID-u

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 6 | `GetUserByIdAsync_Returns_User_When_User_Exists` | Repozitorij vraća odgovarajući `User` za dati ID | Vraća neprazan `UserDto` sa odgovarajućim UserId i Username; `GetByIdAsync` pozvan jednom |
| 7 | `GetUserByIdAsync_Returns_Null_When_User_Does_Not_Exist` | Repozitorij vraća null za dati ID | Vraća `null` |

### 1.4 Dohvatanje korisnika po korisničkom imenu/emailu i lozinci

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 8 | `GetUserByUsernameOrEmailAndPasswordAsync_Returns_User_When_User_Exists` | Repozitorij pronalazi korisnika po korisničkom imenu i lozinci | Vraća neprazan `UserDto` sa ispravnim UserId i Username |
| 9 | `GetUserByUsernameOrEmailAndPasswordAsync_Returns_User_When_Email_And_Password_Match` | Repozitorij pronalazi korisnika po emailu i lozinci | Vraća neprazan `UserDto` sa odgovarajućim Email |
| 10 | `GetUserByUsernameOrEmailAndPasswordAsync_Returns_Null_When_Credentials_Invalid` | Repozitorij vraća null za neprepoznate podatke za prijavu | Vraća `null` |

### 1.5 Registracija korisnika

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 11 | `RegisterUserAsync_Creates_User_With_Default_Role_When_Role_Not_Provided` | DTO za registraciju ima `Role = null` | Vraćeni `UserDto` ima `Role = "ucesnik"`; `AddAsync` pozvan jednom |
| 12 | `RegisterUserAsync_Creates_User_With_Provided_Role_In_Lowercase` | DTO za registraciju ima `Role = "ORGANIZER"` | Vraćeni `UserDto` ima `Role = "organizer"` |
| 13 | `RegisterUserAsync_Trims_Whitespace_From_User_Fields` | Sva string polja u DTO-u sadrže razmake na početku i kraju | Korisnik sačuvan u repozitoriju ima sva polja otrimbovana; uloga normalizovana na mala slova |
| 14 | `RegisterUserAsync_Creates_User_With_New_UserId` | Poslan validan DTO za registraciju | Uhvaćeni `User` entitet i vraćeni `UserDto` imaju ne-prazan GUID kao UserId |
| 15 | `RegisterUserAsync_Sets_CreatedAt_To_Current_Time` | Poslan validan DTO za registraciju | `CreatedAt` na uhvaćenom entitetu pada unutar vremenskog prozora koji okružuje poziv metode |
| 16 | `RegisterUserAsync_Returns_UserDto_With_All_Fields` | Repozitorij vraća potpuno popunjen `User` | Sva polja na vraćenom `UserDto` tačno odgovaraju sačuvanom entitetu |
| 17 | `RegisterUserAsync_With_Empty_Role_String_Uses_Default_Role` | DTO za registraciju ima `Role = ""` | Uhvaćeni entitet ima `Role = "ucesnik"` |
| 18 | `RegisterUserAsync_With_Whitespace_Role_String_Uses_Default_Role` | DTO za registraciju ima `Role = "   "` | Uhvaćeni entitet ima `Role = "ucesnik"` |

### 1.6 Provjera postojanja korisničkog imena

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 19 | `UsernameExistsAsync_Returns_True_When_Username_Exists` | Repozitorij potvrđuje da je korisničko ime zauzeto | Vraća `true`; `AnyByUsernameAsync` pozvan jednom sa ispravnim argumentima |
| 20 | `UsernameExistsAsync_Returns_False_When_Username_Does_Not_Exist` | Repozitorij vraća false za korisničko ime | Vraća `false` |
| 21 | `UsernameExistsAsync_Passes_UserId_When_Provided` | Pozvan sa korisničkim imenom i opcionalnim UserId (za scenarije ažuriranja) | Vraća `true`; `AnyByUsernameAsync` pozvan sa i korisničkim imenom i UserId |

### 1.7 Provjera postojanja emaila

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 22 | `EmailExistsAsync_Returns_True_When_Email_Exists` | Repozitorij potvrđuje da je email zauzet | Vraća `true`; `AnyByEmailAsync` pozvan jednom sa ispravnim argumentima |
| 23 | `EmailExistsAsync_Returns_False_When_Email_Does_Not_Exist` | Repozitorij vraća false za email | Vraća `false` |
| 24 | `EmailExistsAsync_Passes_UserId_When_Provided` | Pozvan sa emailom i opcionalnim UserId (za scenarije ažuriranja) | Vraća `true`; `AnyByEmailAsync` pozvan sa i emailom i UserId |

### 1.8 Ažuriranje korisnika

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 25 | `UpdateUserAsync_Returns_False_When_User_Does_Not_Exist` | Repozitorij vraća null za dati UserId | Vraća `false`; `UpdateAsync` nikad pozvan |
| 26 | `UpdateUserAsync_Updates_FirstName_When_Provided` | DTO sadržava novi `FirstName` | Vraća `true`; uhvaćeni entitet ima ažurirani `FirstName`; `UpdateAsync` pozvan jednom |
| 27 | `UpdateUserAsync_Updates_LastName_When_Provided` | DTO sadržava novi `LastName` | Vraća `true`; uhvaćeni entitet ima ažurirani `LastName` |
| 28 | `UpdateUserAsync_Updates_Email_When_Provided` | DTO sadržava novi `Email` | Vraća `true`; uhvaćeni entitet ima ažurirani `Email` |
| 29 | `UpdateUserAsync_Updates_Username_When_Provided` | DTO sadržava novi `Username` | Vraća `true`; uhvaćeni entitet ima ažurirani `Username` |
| 30 | `UpdateUserAsync_Updates_Password_When_Provided` | DTO sadržava novu `Password` | Vraća `true`; uhvaćeni entitet ima ažuriranu `Password` |
| 31 | `UpdateUserAsync_Updates_Role_When_Provided` | DTO sadržava novu `Role` | Vraća `true`; uhvaćeni entitet ima ažuriranu `Role` |
| 32 | `UpdateUserAsync_Does_Not_Update_Fields_When_Not_Provided` | Sva DTO polja su null | Vraća `true`; originalni `FirstName` i `LastName` ostaju nepromijenjeni na uhvaćenom entitetu |
| 33 | `UpdateUserAsync_Does_Not_Update_Field_When_Empty_String_Provided` | DTO ima `FirstName = ""` | Vraća `true`; `FirstName` na uhvaćenom entitetu zadržava originalnu vrijednost |
| 34 | `UpdateUserAsync_Does_Not_Update_Field_When_Whitespace_Provided` | DTO ima `LastName = "   "` | Vraća `true`; `LastName` na uhvaćenom entitetu zadržava originalnu vrijednost |
| 35 | `UpdateUserAsync_Updates_Multiple_Fields_When_Provided` | DTO sadržava nove vrijednosti za FirstName, LastName, Email i Username istovremeno | Vraća `true`; sva četiri polja ispravno ažurirana na uhvaćenom entitetu |
| 36 | `UpdateUserAsync_Sets_UpdatedAt_Timestamp` | Poslan validan zahtjev za ažuriranje sa `UpdatedAt` inicijalno null | Vraća `true`; `UpdatedAt` na uhvaćenom entitetu nije null i pada unutar vremenskog prozora |
| 37 | `UpdateUserAsync_Calls_Repository_Update_Method` | Poslan validan zahtjev za ažuriranje | Vraća `true`; `UpdateAsync` pozvan tačno jednom |

### 1.9 Edge cases i sekvencijalne operacije

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 38 | `Multiple_Operations_Work_Sequentially` | Registracija korisnika, dohvatanje po ID-u, pa provjera emaila u nizu | Sve tri operacije vraćaju ispravne neprazne rezultate; UserId registrovanog i dohvaćenog korisnika se podudaraju |
| 39 | `UpdateUserAsync_Preserves_Unmodified_Fields` | Samo `FirstName` je navedeno u DTO-u; `Password` i `Role` su izostavljeni | Vraća `true`; originalni `Password` i `Role` ostaju nepromijenjeni na uhvaćenom entitetu |

<img width="699" height="579" alt="image" src="https://github.com/user-attachments/assets/45cba567-a595-449a-bf34-975facd722bd" />
<img width="680" height="427" alt="image" src="https://github.com/user-attachments/assets/832edf91-9d03-4832-bba6-1e40ccb0f260" />

---

## 2. ConferenceServiceTests

**Klasa koja se testira:** `ConferenceService`  
**Zavisnosti koje se mockuju:** `IConferenceRepository`, `IUserContextService`

### 2.1 Dohvatanje i autorizacija

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 40 | `GetPagedAsync_AdminSeesActiveDraftAndInactive` | Admin korisnik traži stranicu konferencija sa `includeAll = true` | Vraća `TotalCount = 2` i `Items.Count = 2`, uključujući i Active i Draft konferencije |
| 41 | `GetByIdAsync_AdminCanSeeDraftConference` | Admin traži konferenciju po ID-u čiji je status `Draft` | Vraća neprazan rezultat sa `Status = "Draft"` |

### 2.2 Kreiranje

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 42 | `CreateAsync_ValidData_ReturnsConferenceDto` | Navedeni su validan naslov, datumi, lokacija, kategorija i broj učesnika | Vraća neprazan `ConferenceDto` sa odgovarajućim Title, Location i MaxParticipants |
| 43 | `CreateAsync_InvalidDates_ThrowsArgumentException` | StartDate (dan +5) je nakon EndDate (dan +4) | Baca `ArgumentException` |
| 44 | `CreateAsync_StartDateAfterEndDate_ThrowsArgumentException` | StartDate (dan +3) je nakon EndDate (dan +1) | Baca `ArgumentException` |
| 45 | `CreateAsync_StartDateEqualsEndDate_ThrowsArgumentException` | StartDate je jednak EndDate | Baca `ArgumentException` |
| 46 | `CreateAsync_MaxParticipantsZero_ThrowsArgumentException` | MaxParticipants postavljen na 0 | Baca `ArgumentException` |
| 47 | `CreateAsync_MaxParticipantsNegative_ThrowsArgumentException` | MaxParticipants postavljen na -10 | Baca `ArgumentException` |

### 2.3 Ažuriranje

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 48 | `UpdateAsync_ValidData_UpdatesSuccessfully` | Postojeća konferencija ažurirana sa novim naslovom, opisom, lokacijom, datumima i brojem učesnika | `UpdateAsync` na repozitoriju pozvan tačno jednom |
| 49 | `UpdateAsync_ConferenceNotFound_ThrowsKeyNotFoundException` | ID konferencije nije pronađen u repozitoriju | Baca `KeyNotFoundException` |

### 2.4 Brisanje

| # | Naziv testa | Opis | Očekivani ishod |
|---|-------------|------|-----------------|
| 50 | `DeleteAsync_ExistingConference_DeletesSuccessfully` | Konferencija postoji u repozitoriju | `DeleteAsync` na repozitoriju pozvan tačno jednom |
| 51 | `DeleteAsync_ConferenceNotFound_ThrowsKeyNotFoundException` | ID konferencije nije pronađen u repozitoriju | Baca `KeyNotFoundException` |

<img width="675" height="325" alt="image" src="https://github.com/user-attachments/assets/6dea02bf-948f-4dfb-800e-e7baecee1b7c" />

---

## 3. Pregled pokrivenosti

| Oblast | Testovi | Kriterij prolaza |
|--------|---------|-----------------|
| Dohvatanje broja korisnika | 2 | Delegiranje repozitoriju i granični slučaj nule provjereni |
| Dohvatanje i mapiranje liste korisnika | 3 | Mapiranje svih DTO polja provjereno; prazna lista obrađena |
| Pretraga korisnika po ID-u | 2 | Slučajevi postojećeg i nepostojećeg korisnika obrađeni |
| Pretraga korisnika po podacima za prijavu | 3 | Podudaranje po korisničkom imenu, emailu i nevažeći podaci obrađeni |
| Registracija korisnika | 8 | Zadana uloga, normalizacija velikih slova, trimbovanje razmaka, generisanje ID-a, timestamp i mapiranje DTO-a provjereni |
| Provjera jedinstvenosti korisničkog imena | 3 | Postoji, ne postoji i opcionalno isključivanje po UserId pokriveno |
| Provjera jedinstvenosti emaila | 3 | Postoji, ne postoji i opcionalno isključivanje po UserId pokriveno |
| Ažuriranje korisnika | 13 | Ažuriranja po polju, zaštita od praznih vrijednosti, ažuriranje više polja, timestamp i poziv repozitorija provjereni |
| Granični slučajevi i sekvencijalne operacije | 2 | Višeoperacijski tokovi i čuvanje polja potvrđeni |
| Dohvatanje konferencija i autorizacija | 2 | Admin uloga daje pristup Draft konferencijama |
| Validacija kreiranja konferencije | 6 | Neispravni datumski rasponi i negativan/nulti broj učesnika odbijeni |
| Ažuriranje i brisanje konferencije | 4 | Interakcije sa repozitorijem provjerene; slučajevi nepostojanja bacaju ispravne iznimke |
| **Ukupno** | **51** | |

<img width="1612" height="482" alt="image" src="https://github.com/user-attachments/assets/0cedb13c-16ab-4214-b585-dbb9aba04664" />

---

## 4. Testno okruženje

| Postavka | Vrijednost |
|----------|------------|
| Test runner | xUnit |
| Framework za mockovanje | Moq |
| HTTP kontekst | `DefaultHttpContext` (in-memory) |
| Zadana uloga korisnika | `ucesnik` |
| Provjere timestampa | Ograđeni `DateTime.UtcNow` prije/poslije poziva metode |
| Uhvaćeni entiteti | Moq `Callback` korišten za inspekciju objekata proslijeđenih repozitoriju |
