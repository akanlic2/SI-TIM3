| ID | Kategorija | Opis zahtjeva | Kako će se provjeravati | Prioritet | Napomena |
|----------|----------|----------|----------|----------|----------|
| NFR-01 | Performanse | Sistem mora učitati glavnu stranicu u roku od maksimalno 2 sekunde | Mjerenjem vremena učitavanja pomoću alata, npr. browser developer tools | Visok | Odnosi se na standardne uslove mreže |
| NFR-02 | Performanse | Sistem treba podržati najmanje 10000 istovremenih korisnika bez pada performansi | Load testiranje (simulacija korisnika) | Visok | Važno za veće konferencije |
| NFR-03 | Sigurnost | Lozinke korisnika moraju biti kriptovane u bazi podataka | Pregled implementacije (hash funkcije) | Visok | Koristiti sigurne hash algoritme, bez čuvanja lozinki u čistom tekstu  |
| NFR-04 | Sigurnost | Sistem mora omogućiti autentifikaciju i autorizaciju korisnika | Testiranje pristupa (različite uloge korisnika) | Visok | Uloge uključuju admina, učesnika, organizatora i slično |
| NFR-05 | Upotrebljivost | Korisnik mora moći izvršiti prijavu na konferenciju u maksimalno 3 koraka | Testiranje korisničkog interfejsa | Srednji | Fokus je na jednostavnosti korištenja |
| NFR-06 | Dostupnost | Garantovana dostupnost sistema iznosi 99% ili više | Praćenje uptime vremena sistema | Visok | Planirano održavanje mora biti najavljeno korisnicima unaprijed |
| NFR-07 | Pouzdanost | Sistem mora praviti backup podataka jednom dnevno | Provjera backup logova | Visok | Podaci se čuvaju minimum 30 dana |
| NFR-08 | Kompatibilnost | Sistem mora raditi na modernim web preglednicima (Chrome, Firefox, Edge) | Testiranje na različitim browserima | Srednji | Testiranje se provodi na posljednjim stabilnim verzijama browsera |
| NFR-09 | Održivost | Kod sistema mora biti modularan i dokumentovan | Pregled koda i dokumentacije | Srednji | Olakšava budući razvoj |
| NFR-10 | Privatnost | Sistem mora zaštititi lične podatke korisnika | Analiza pristupa podacima | Visok | U skladu sa GDPR principima |
| NFR-11 | Upotrebljivost | Dizajn sistema mora biti responzivan (potpuno funkcionalan i na mobilnim i na desktop uređajima)   | Testiranje na različitim vrstama uređaja | Srednji  | Koristiti fleksibilne layoute |
| NFR-12 | Upotrebljivost | Sve greške moraju biti prikazane jasno s konkretnim prijedlogom za rješavanje  | Testiranje scenarija grešaka u korisničkom interfejsu (npr. pogrešan unos, neuspješna prijava) | Visok | Greške trebaju biti prikazane na jeziku razumljivom za korisnika, bez nekih tehničkih kodova  |
| NFR-13 | Dostupnost | U slučaju djelimičnog kvara, ključne funkcije (pregled rasporeda, check-in) moraju ostati dostupne  | Simulacija kvara  | Visok | Kvar jedne komponente ne bi trebao uticati na rad osnovnih funkcionalnosti |
| NFR-14 | Upotrebljivost | Sistem mora imati konzistentan dizajn kroz sve stranice  | UI/UX evaluacija | Srednji | Konzistentnost u bojama, fontovima i rasporedu elemenata kroz cijeli sistem  |
| NFR-15 | Sigurnost | Sistem mora automatski odjaviti korisnika nakon perioda neaktivnosti (npr. 15 minuta)  | Testiranje sesija  | Srednji  | Spriječava zloupotrebu korisničkog računa  |
| NFR-16 | Integracija | Sistem mora omogućiti integraciju sa eksternim servisima (npr. email servis, servis za plaćanje)  | Testranje API integracija  | Srednji  | Servisi moraju biti pouzdani i podržavati stabilnu komunikaciju sa sistemom |
| NFR-17 | Lokalizacija | Sistem treba podržavati više jezika  | Promjena jezika u interfejsu | Nizak | Minimalno BHS i engleski jezik |
| NFR-18 | Skalabilnost  | Sistem mora podržavati dodavanje novih modula bez uticaja na postojeće  | Analiza arhitekture sistema | Srednji  | Novi moduli se mogu dodavati bez izmjene postojećeg koda |
| NFR-19 | Performanse  | Pretraga konferencija mora vratiti rezultate u roku od maksimalno 2 sekunde  | Mjerenje vremena odziva | Visok |  Moguće korištenje keširanja za ubrzanje rezultata pretrage |
| NFR-20 | Personalizacija | Sistem može omogućiti korisnicima da prilagode izgled interfejsa (npr. tamni/svijetli mod)  | Testiranje opcija u postavkama | Nizak | Poboljšava korisničko iskustvo |
| NFR-21 | Analitika | Sistem može prikupljati anonimne statistike o korištenju  | Pregled analitičkih podataka | Nizak | U skladu sa GDPR principima |
| NFR-22 | Upotrebljivost | Sistem treba omogućiti jednostavnu navigaciju kroz sve glavne funkcionalnosti  | Testiranje korisničkog interfejsa i scenarija |Srednji  | Fokus je na intuitivnom dizajnu |
| NFR-23 | Sigurnost | Sistem treba ograničiti broj neuspješnih pokušaja prijave (npr. maksimalno 3)  | Testiranje pokušaja prijave | Srednji | Zaštita od napada |
| NFR-24 | Upotrebljivost  | Sistem može sadržavati kratke tutorijale za nove korisnike  | Pregled onboarding funkcionalnosti | Nizak | Poboljšano iskustvo prilikom prvog korištenja sistema |
| NFR-25 | Održivost | Sistem treba omogućiti lako ažuriranje podataka bez potrebe za restartom sistema  | Testiranje izmjena u radu sistema  | Srednji |Ovo je bitno za administratore|
| NFR-26 | Pouzdanost | Sistem treba automatski ponoviti neuspjele operacije (npr. slanje emaila)  | Simulacija grešaka | Srednji | Retry logika za povećanje pouzdanosti sistema  |

