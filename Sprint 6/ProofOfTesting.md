# Proof of Testing
 
**Test Framework:** xUnit  
**Mocking Library:** Moq  
**Total Tests:** 34

---

## 1. ConferenceServiceTests

**Class under test:** `ConferenceService`  
**Dependencies mocked:** `IConferenceRepository`, `IUserContextService`

### 1.1 Get & Authorization

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 1 | `GetPagedAsync_AdminSeesActiveDraftAndInactive` | Admin user requests a paged list of conferences with `includeAll = true` | Returns a result with `TotalCount = 2` and `Items.Count = 2`, including both Active and Draft conferences |
| 2 | `GetByIdAsync_AdminCanSeeDraftConference` | Admin user requests a conference by ID where the conference has status `Draft` | Returns a non-null result with `Status = "Draft"` |

### 1.2 Create

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 3 | `CreateAsync_ValidData_ReturnsConferenceDto` | Creates a conference with valid title, dates, location, category, and participant count | Returns a non-null `ConferenceDto` with matching Title, Location, and MaxParticipants |
| 4 | `CreateAsync_InvalidDates_ThrowsArgumentException` | Attempts to create a conference where StartDate (day +5) is after EndDate (day +4) | Throws `ArgumentException` |
| 5 | `CreateAsync_StartDateAfterEndDate_ThrowsArgumentException` | Attempts to create a conference where StartDate (day +3) is after EndDate (day +1) | Throws `ArgumentException` |
| 6 | `CreateAsync_StartDateEqualsEndDate_ThrowsArgumentException` | Attempts to create a conference where StartDate equals EndDate | Throws `ArgumentException` |
| 7 | `CreateAsync_MaxParticipantsZero_ThrowsArgumentException` | Attempts to create a conference with MaxParticipants set to 0 | Throws `ArgumentException` |
| 8 | `CreateAsync_MaxParticipantsNegative_ThrowsArgumentException` | Attempts to create a conference with MaxParticipants set to -10 | Throws `ArgumentException` |

### 1.3 Update

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 9 | `UpdateAsync_ValidData_UpdatesSuccessfully` | Updates an existing conference with a new title, description, location, dates, and participant count | `UpdateAsync` on the repository is called exactly once |
| 10 | `UpdateAsync_ConferenceNotFound_ThrowsKeyNotFoundException` | Attempts to update a conference with an ID that does not exist in the repository | Throws `KeyNotFoundException` |

### 1.4 Delete

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 11 | `DeleteAsync_ExistingConference_DeletesSuccessfully` | Deletes a conference that exists in the repository | `DeleteAsync` on the repository is called exactly once |
| 12 | `DeleteAsync_ConferenceNotFound_ThrowsKeyNotFoundException` | Attempts to delete a conference with an ID that does not exist in the repository | Throws `KeyNotFoundException` |

<img width="688" height="327" alt="image" src="https://github.com/user-attachments/assets/4f8ecbe9-3d0d-4918-8510-50bb1a0f8ba1" />

---

## 2. UserControllerTests

**Class under test:** `UserController`  
**Dependencies mocked:** `IUserService`, `IUserContextService`, `IConfiguration` (JWT settings)

### 2.1 Registration

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 13 | `Register_Returns_BadRequest_When_Required_Fields_Missing` | Submits a registration request with an empty username field | Returns `400 Bad Request` |
| 14 | `Register_Returns_Conflict_When_Username_Exists` | Submits a registration request with a username that already exists in the system | Returns `409 Conflict` |
| 15 | `Register_Returns_Conflict_When_Email_Exists` | Submits a registration request with a unique username but an email address already in use | Returns `409 Conflict` |
| 16 | `Register_Returns_Ok_With_Valid_Data` | Submits a fully valid registration request; service confirms username and email are both available and creates the user | Returns `200 OK` with a non-null response body |

### 2.2 Authentication

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 17 | `Login_Returns_Unauthorized_For_Invalid_Credentials` | Attempts login with credentials the service does not recognize (returns null) | Returns `401 Unauthorized` |
| 18 | `Login_Returns_Token_For_Valid_Credentials` | Logs in with valid credentials; service returns a populated `UserDto` | Returns `200 OK` with a non-null response body |
| 19 | `Login_Returns_Ok_With_Token_And_User_Data` | Logs in with valid username and password; verifies response object is present | Returns `200 OK` with a non-null response body |

### 2.3 Logout

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 20 | `Logout_Returns_Ok` | Calls the logout endpoint | Returns `200 OK` with a non-null response body |

### 2.4 Current User

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 21 | `Current_Returns_Unauthorized_When_Invalid_Token` | Calls `/current` with an unauthenticated (empty) `ClaimsPrincipal` | Returns `401 Unauthorized` |
| 22 | `Current_Returns_NotFound_When_User_Does_Not_Exist` | Calls `/current` with a valid JWT identity but the user ID is not found in the service | Returns `404 Not Found` |
| 23 | `Current_Returns_Ok_When_User_Exists` | Calls `/current` with a valid JWT identity and the service returns a matching `UserDto` | Returns `200 OK` with a non-null user object |

### 2.5 Get All Users

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 24 | `GetAllUsers_Returns_Ok_With_Users` | Requests the full list of users; service returns two `UserDto` records | Returns `200 OK` with a non-null response body |

### 2.6 Get User By ID

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 25 | `GetById_Returns_Forbidden_When_Unauthorized` | Authenticated user requests a profile belonging to a different user ID without admin role | Returns `403 Forbidden` |
| 26 | `GetById_Returns_NotFound_When_User_Does_Not_Exist` | Authenticated user requests their own profile but the service returns null | Returns `404 Not Found` |
| 27 | `GetById_Returns_Ok_When_User_Exists_And_Authorized` | Authenticated user requests their own profile and the service returns a matching `UserDto` | Returns `200 OK` with a non-null user object |
| 28 | `GetById_Returns_Ok_When_Admin_Requests_Different_User` | Admin user (role `admin-sistema`) requests a profile belonging to a different user ID | Returns `200 OK` with a non-null user object |

### 2.7 Update User

| # | Test Name | Description | Expected Outcome |
|---|-----------|-------------|-----------------|
| 29 | `Update_Returns_Forbidden_When_Unauthorized` | Authenticated user attempts to update a profile belonging to a different user ID without admin role | Returns `403 Forbidden` |
| 30 | `Update_Returns_Conflict_When_Username_Exists` | User attempts to update their profile to a username already taken by another account | Returns `409 Conflict` |
| 31 | `Update_Returns_Conflict_When_Email_Exists` | User attempts to update their profile to an email address already in use by another account | Returns `409 Conflict` |
| 32 | `Update_Returns_NotFound_When_User_Does_Not_Exist` | User submits a valid update request but the service cannot find the user record (returns false) | Returns `404 Not Found` |
| 33 | `Update_Returns_NoContent_When_User_Updated_Successfully` | User submits a valid update request and the service confirms the update (returns true) | Returns `204 No Content` |
| 34 | `Update_Returns_NoContent_When_Admin_Updates_Different_User` | Admin user (role `admin-sistema`) successfully updates the profile of a different user | Returns `204 No Content` |

<img width="676" height="576" alt="image" src="https://github.com/user-attachments/assets/1c4b6101-d5af-43fb-8306-f480d9139b60" />

---

## 3. Coverage Summary

| Area | Tests | Pass Criteria |
|------|-------|---------------|
| Conference retrieval & authorization | 2 | Admin role grants access to Draft and inactive conferences |
| Conference creation validation | 6 | Invalid date ranges and non-positive participant counts are rejected |
| Conference update & delete | 4 | Repository interactions verified; not-found cases throw correct exceptions |
| User registration | 4 | Missing fields, duplicate username, and duplicate email all rejected correctly |
| User authentication | 3 | Invalid credentials blocked; valid credentials produce a token response |
| Logout | 1 | Endpoint responds with success |
| Current user resolution | 3 | Unauthenticated, missing, and valid user cases handled correctly |
| Get all users | 1 | Full user list returned successfully |
| Get user by ID | 4 | Authorization enforced; admin bypass confirmed; not-found handled |
| Update user | 6 | Authorization, uniqueness constraints, not-found, and success cases all covered |
| **Total** | **34** | |

<img width="1603" height="659" alt="image" src="https://github.com/user-attachments/assets/8c0c5f75-4437-49f8-a030-777310d167c8" />

---

## 4. Test Environment

| Setting | Value |
|---------|-------|
| Test runner | xUnit |
| Mock framework | Moq |
| JWT Issuer (mock) | `ConferenceManagement.Api` |
| JWT Audience (mock) | `ConferenceManagement.Client` |
| JWT Expiry (mock) | 120 minutes |
| HTTP context | `DefaultHttpContext` (in-memory) |
| Auth identity | `ClaimsIdentity` / `ClaimsPrincipal` (manually constructed per test) |
