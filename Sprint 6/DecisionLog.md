### DL-001 – Odabir IAM rješenja
**Datum:** 25.04.2026

**Opis problema:** Projekt zahtijeva autentikaciju i autorizaciju korisnika za web aplikaciju (frontend + backend). Trebalo je odlučiti koji IAM sistem koristiti.

**Razmatrane opcije:**
1. Keycloak (open-source)
2. Auth0 (SaaS)
3. Custom JWT implementacija

**Odabrana opcija:** Keycloak

**Razlog izbora:** Open-source, self-hosted rješenje bez licencnih troškova, bogat skup funkcionalnosti (SSO, OAuth2, OIDC), puna kontrola nad podacima korisnika.

**Posljedice odluke:** Tim mora upravljati Keycloak instancom (nadogradnje, backup, monitoring). Svi korisnički podaci ostaju u okviru vlastite infrastrukture.

**Status:** Aktivna

---

### DL-002 – Realm struktura – jedan realm
**Datum:** 25.04.2026

**Opis problema:** Nakon postavljanja Keycloaka trebalo je odlučiti da li kreirati jedan realm ili odvojene realme po okruženjima ili aplikacijama.

**Razmatrane opcije:**
1. Jedan realm za sve
2. Odvojeni realmi po okruženjima (dev/staging/prod)
3. Odvojeni realmi po aplikacijama

**Odabrana opcija:** Jedan realm za sve

**Razlog izbora:** U trenutnoj fazi (dev okruženje, jedna web aplikacija) jedan realm je dovoljan i smanjuje složenost konfiguracije i održavanja.

**Posljedice odluke:** Svi klijenti dijele isti realm i skup korisnika.

**Status:** Aktivna

---

### DL-003 – Grant type – Client Credentials Flow
**Datum:** 25.04.2026

**Opis problema:** Web aplikacija treba se autentifikovati prema Keycloaku. Trebalo je odabrati odgovarajući OAuth2 grant type s obzirom na arhitekturu aplikacije.

**Razmatrane opcije:**
1. Authorization Code Flow
2. Authorization Code + PKCE
3. Client Credentials Flow
4. Implicit Flow (zastarjelo)

**Odabrana opcija:** Client Credentials Flow

**Razlog izbora:** Pogodan za server-to-server komunikaciju gdje backend direktno komunicira s Keycloakom koristeći client ID i client secret, bez interakcije krajnjeg korisnika.

**Posljedice odluke:** Nema korisničke sesije upravljane od strane Keycloaka. Potrebno je sigurno pohraniti client secret. Nije preporučeno za scenarije autentikacije krajnjih korisnika putem browsera.

**Status:** Aktivna

---

### DL-004 – Integracija – JWT direktno bez adaptera
**Datum:** 25.04.2026

**Opis problema:** Trebalo je odlučiti kako frontend i backend integrišu Keycloak – putem gotovih adaptera ili direktnom obradom JWT tokena.

**Razmatrane opcije:**
1. Keycloak JS adapter
2. Keycloak Spring Boot adapter
3. OAuth2 biblioteka (NextAuth, spring-security-oauth2...)
4. JWT direktno (bez adaptera)

**Odabrana opcija:** JWT token direktno (bez adaptera)

**Razlog izbora:** Smanjuje zavisnost o Keycloak-specifičnim adapterima, povećava fleksibilnost. Backend validira JWT potpis koristeći javni ključ s Keycloak JWKS endpointa.

---

### DL-005 – Deployment – Odabir VPS
**Datum:** 25.04.2026

**Opis problema:** Trebalo je odlučiti koji alat koristiti za VPS.

**Razmatrane opcije:**
1. DigitalOcean
2. Hetzner
3. Contabo


**Odabrana opcija:** DigitalOcean

**Razlog izbora:** Jednostavnost i povoljnost.

**Posljedice odluke:** Veća odgovornost za sigurnost, update OS i skaliranje ručno 

**Status:** Aktivna


---

### DL-006 – Deployment – Odabir domene
**Datum:** 25.04.2026

**Opis problema:** Trebalo je odabrati domenu za aplikaciju.

**Razmatrane opcije:**
1. nip.io sa Let's Encrypt
2. sslip.io



**Odabrana opcija:** nip.io

**Razlog izbora:** Jednostavnost, odmah radi.

**Posljedice odluke:** Ako padne servis, pada aplikacija

**Status:** Aktivna

## DL-006 – Novi unosi

**Datum:** 4.04.2026

**Opis problema:** Pri implementaciji backend ruta trebalo je odlučiti koji pattern koristiti za servisni sloj - MediatR (CQRS) ili direktan Service pattern.

**Razmatrane opcije:**
- MediatR - CQRS pattern s Command/Handler objektima
- Direktan Service pattern - IConferenceService / ConferenceService

**Odabrana opcija:** Direktan Service pattern bez MediatR.

**Razlog izbora:** MediatR dodaje nepotrebnu složenost za projekat ove veličine. Direktan servisni poziv je lakši za razumjeti, debugirati i testirati.

**Posljedice odluke:**
- Svaki servis direktno poziva repozitorij
- Lakše za proširiti u kasnijem sprintu ako zatreba MediatR

**Status:** Aktivna

---

## DL-007 – Validacija podataka

**Datum:** 4.04.2026

**Opis problema:** Trebalo je odlučiti kako validirati ulazne podatke na POST/conferences, PUT/conferences/:id i DELETE/conferences/:id rutama.

**Razmatrane opcije:**
- FluentValidation - posebni Validator razredi
- Data Annotations - atributi direktno na DTO klasama
- Ručna validacija u servisnom sloju

**Odabrana opcija:** Kombinacija Data Annotations (za osnovna pravila) i ručne validacije u servisu (za poslovna pravila poput provjere datuma).

**Razlog izbora:** FluentValidation 11.9.2 verzija koju je Copilot predložio ne postoji u NuGet repozitoriju. Data Annotations su ugrađene u ASP.NET Core i ne zahtijevaju dodatne pakete.

**Posljedice odluke:**
- Manje zavisnosti u projektu
- Validacijska logika podijeljena između DTO-a i servisa

**Status:** Aktivna

---

## DL-008 – Soft delete ili hard delete konferencija

**Datum:** 4.04.2026

**Opis problema:** Pri implementaciji DELETE /conferences/{id} trebalo je odlučiti da li fizički brisati konferenciju iz baze ili je označiti kao otkazanu.

**Razmatrane opcije:**
- Hard delete - fizičko brisanje iz baze u svim slučajevima
- Soft delete - postavljanje statusa na Cancelled u svim slučajevima

**Odabrana opcija:** Hard delete — fizičko brisanje iz baze u svim slučajevima

**Razlog izbora:** Podaci trajno uklanjaju kroz DELETE operaciju.

**Posljedice odluke:**
- Konferencija se trajno uklanja iz baze podataka
- Nema potrebe za dodatnim statusima poput “Cancelled”

**Status:** Aktivna

---

## DL-009 – Frontend – Odabir biblioteke za forme

**Datum:** 5.04.2026

**Opis problema:** Za frontend forme (kreiranje konferencije, uređivanje) trebalo je odlučiti kako upravljati stanjem forme, validacijom i submit logikom.

**Razmatrane opcije:**
- react-hook-form
- Formik
- Ručno upravljanje state-om (useState)

**Odabrana opcija:** Ručno upravljanje state-om pomoću useState

**Razlog izbora:** Implementacija je jednostavna i konzistentna sa ostatkom aplikacije. S obzirom na mali broj polja u formi, dodatne biblioteke za forme nisu bile potrebne.

**Posljedice odluke:**
- Svaka forma koristi useState za upravljanje podacima
- Validacija se radi osnovnim HTML atributima (required)
- Submit logika je direktno implementirana u komponenti

**Status:** Aktivna

---

## DL-010 – Frontend - Odabir HTTP klijenta

**Datum:** 07.05.2026.

**Opis problema:** Trebalo je odabrati biblioteku za HTTP komunikaciju između React frontenda i ASP.NET Core backend-a.

**Razmatrane opcije:**
- axios
- fetch API 
- ky

**Odabrana opcija:** axios

**Razlog izbora:** Jednostavna konfiguracija Authorization headera, automatsko parsiranje JSON odgovora i jasna obrada grešaka putem try/catch mehanizma i axios error objekta.

**Posljedice odluke:** Svi API pozivi se izvršavaju putem axios biblioteke. Bearer token se ručno dodaje u svaki zahtjev putem Authorization headera.

**Status:** Aktivna
