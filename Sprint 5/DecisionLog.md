### DL-001 – Odabir IAM rješenja
**Datum:** 2026-04-25

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
**Datum:** 2026-04-25

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
**Datum:** 2026-04-25

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
**Datum:** 2026-04-25

**Opis problema:** Trebalo je odlučiti kako frontend i backend integrišu Keycloak – putem gotovih adaptera ili direktnom obradom JWT tokena.

**Razmatrane opcije:**
1. Keycloak JS adapter
2. Keycloak Spring Boot adapter
3. OAuth2 biblioteka (NextAuth, spring-security-oauth2...)
4. JWT direktno (bez adaptera)

**Odabrana opcija:** JWT token direktno (bez adaptera)

**Razlog izbora:** Smanjuje zavisnost o Keycloak-specifičnim adapterima, povećava fleksibilnost. Backend validira JWT potpis koristeći javni ključ s Keycloak JWKS endpointa.

**Posljedice odluke:** Tim mora sam implementirati JWT validaciju. 

**Status:** Aktivna
