# Domain Model

## Glavni entiteti

Sistem za organizaciju konferencija zasniva se na sljedećim glavnim entitetima:

- **User**
- **Conference**
- **Session**
- **Room**
- **ConferenceRegistration**
- **SessionRegistration**
- **Payment**
- **Notification**
- **Material**
- **Equipment**
- **LogisticsTask**
- **Question**
- **AgendaItem**

Ovi entiteti pokrivaju osnovne funkcionalnosti sistema, uključujući upravljanje korisnicima, konferencijama, sesijama, rasporedom, dvoranama, prijavama, kotizacijama, obavijestima, materijalima, opremom, logističkim aktivnostima i Q&A segmentom.

---

## Ključni atributi

### 1. User
Predstavlja korisnika sistema.

**Ključni atributi:**
- `userId`
- `firstName`
- `lastName`
- `email`
- `password`
- `role`
- `accountStatus`

---

### 2. Conference
Predstavlja jednu konferenciju kao centralni događaj sistema.

**Ključni atributi:**
- `conferenceId`
- `title`
- `description`
- `startDate`
- `endDate`
- `location`
- `category`
- `maxParticipants`
- `status`

---

### 3. Session
Predstavlja pojedinačnu sesiju unutar konferencije.

**Ključni atributi:**
- `sessionId`
- `title`
- `description`
- `duration`
- `sessionType`
- `status`

---

### 4. Room
Predstavlja dvoranu ili prostoriju u kojoj se održavaju sesije.

**Ključni atributi:**
- `roomId`
- `name`
- `location`
- `capacity`
- `description`

---

### 5. ConferenceRegistration
Predstavlja prijavu korisnika na konferenciju.

**Ključni atributi:**
- `conferenceRegistrationId`
- `registrationDate`
- `registrationStatus`

---

### 6. SessionRegistration
Predstavlja vezu između korisnika i sesije, pri čemu korisnik može biti učesnik ili predavač.

**Ključni atributi:**
- `sessionRegistrationId`
- `registrationDate`
- `registrationStatus`
- `isSpeaker`

---

### 7. Payment
Predstavlja evidenciju uplate kotizacije za prijavu na konferenciju.

**Ključni atributi:**
- `paymentId`
- `amount`
- `paymentStatus`
- `paymentDate`
- `paymentMethod`

---

### 8. Notification
Predstavlja obavijest poslanu korisniku unutar sistema.

**Ključni atributi:**
- `notificationId`
- `title`
- `content`
- `notificationType`
- `sentDate`
- `isRead`

---

### 9. Material
Predstavlja materijale vezane za konferenciju ili sesiju.

**Ključni atributi:**
- `materialId`
- `title`
- `description`
- `fileUrl`
- `materialType`
- `uploadDate`

---

### 10. Equipment
Predstavlja tehničku opremu potrebnu za održavanje konferencije ili sesije.

**Ključni atributi:**
- `equipmentId`
- `name`
- `type`
- `quantity`
- `availabilityStatus`

---

### 11. LogisticsTask
Predstavlja logističke aktivnosti vezane za organizaciju konferencije.

**Ključni atributi:**
- `logisticsTaskId`
- `title`
- `description`
- `taskType`
- `dueDate`
- `status`

---

### 12. Question
Predstavlja pitanje postavljeno u okviru Q&A funkcionalnosti za sesiju.

**Ključni atributi:**
- `questionId`
- `content`
- `askedAt`
- `status`
- `answer`

---

### 13. AgendaItem
Predstavlja jednu stavku programa konferencije, koja može biti povezana sa sesijom ili predstavljati drugu aktivnost kao što su otvaranje, pauza, ručak, networking ili zatvaranje konferencije.

**Ključni atributi:**
- `agendaItemId`
- `title`
- `description`
- `startTime`
- `endTime`
- `type`
- `status`

---

## Veze između entiteta

### User i Conference
Jedan korisnik može biti organizator više konferencija, a jedna konferencija može imati više organizatora.

**Veza:** M : N

---

### Conference i Session
Jedna konferencija sadrži više sesija, dok svaka sesija pripada tačno jednoj konferenciji.

**Veza:** 1 : N

---

### Session i Room
Jedna sesija se održava u jednoj dvorani, dok jedna dvorana može biti korištena za više sesija u različitim terminima.

**Veza:** N : 1

---

### User i ConferenceRegistration
Jedan korisnik može imati više prijava na konferencije, dok svaka prijava pripada jednom korisniku.

**Veza:** 1 : N

---

### Conference i ConferenceRegistration
Jedna konferencija može imati više prijava, dok svaka prijava pripada tačno jednoj konferenciji.

**Veza:** 1 : N

---

### User i SessionRegistration
Jedan korisnik može imati više prijava na sesije, dok svaka prijava pripada jednom korisniku.

**Veza:** 1 : N

---

### Session i SessionRegistration
Jedna sesija može imati više prijava, dok svaka prijava pripada tačno jednoj sesiji.

**Veza:** 1 : N

---

### ConferenceRegistration i Payment
Jedna prijava na konferenciju ima jednu evidenciju uplate, dok svaka evidencija uplate pripada jednoj prijavi.

**Veza:** 1 : 1

---

### User i Notification
Jedan korisnik može primiti više obavijesti, dok svaka obavijest pripada jednom korisniku.

**Veza:** 1 : N

---

### Conference i Material
Jedna konferencija može imati više materijala, dok materijal pripada tačno jednoj konferenciji.

**Veza:** 1 : N

---

### Session i Material
Jedna sesija može imati više materijala, dok materijal pripada tačno jednoj sesiji.

**Veza:** 1 : N

---

### Conference i LogisticsTask
Jedna konferencija može imati više logističkih zadataka, dok svaki logistički zadatak pripada jednoj konferenciji.

**Veza:** 1 : N

---

### Session i Equipment
Jedna sesija može zahtijevati više komada opreme, a jedan komad opreme može biti korišten u više sesija.

**Veza:** M : N

---

### User i Question
Jedan korisnik može postaviti više pitanja, dok svako pitanje postavlja tačno jedan korisnik.

**Veza:** 1 : N

---

### Session i Question
Jedna sesija može imati više pitanja, dok svako pitanje pripada tačno jednoj sesiji.

**Veza:** 1 : N

---

### Conference i AgendaItem
Jedna konferencija ima više stavki agende, dok svaka stavka agende pripada tačno jednoj konferenciji.

**Veza:** 1 : N

---

### AgendaItem i Session
Jedna agenda stavka može biti povezana sa jednom sesijom, dok jedna sesija može imati jednu odgovarajuću agenda stavku.

**Veza:** opcionalna 1 : 1

---

## Poslovna pravila važna za model

1. Jedan korisnik ne može biti prijavljen više puta na istu konferenciju.

2. Jedan korisnik ne može biti prijavljen više puta na istu sesiju.

3. Svaka sesija mora pripadati tačno jednoj konferenciji.

4. Svaka sesija se održava u jednoj dvorani.

5. Dvije sesije se ne mogu održavati u istoj dvorani u istom terminu.

6. Jedna konferencija može imati više organizatora.

7. Jedna sesija može imati više predavača, a to se evidentira kroz `SessionRegistration` entitet pomoću atributa `isSpeaker`.

8. Broj prijavljenih učesnika na konferenciju ne smije preći maksimalan broj učesnika konferencije.

9. Broj prijavljenih na sesiju ne smije preći kapacitet dvorane ili definisani limit sesije.

10. Svaka prijava na konferenciju može imati jednu evidenciju uplate kotizacije.

11. Obavijesti se šalju korisnicima u vezi sa prijavama, promjenama rasporeda i otkazivanjem sesija.

12. Materijali mogu biti vezani za konferenciju ili za pojedinačnu sesiju.

13. Logistički zadaci i oprema predstavljaju podršku realizaciji konferencije i njenih sesija.

14. Svako pitanje u Q&A modulu mora biti vezano za tačno jednu sesiju i jednog korisnika.

15. Svaka agenda stavka pripada tačno jednoj konferenciji i koristi se za prikaz programa konferencije.

16. Agenda stavka može biti povezana sa jednom sesijom, ali može predstavljati i drugu aktivnost koja nije sesija.
