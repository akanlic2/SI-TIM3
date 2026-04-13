| ID  | Opis rizika | Uzrok  | Vjerovatnoća | Uticaj | Prioritet rizika | Plan mitigacije  | Odgovorna osoba/uloga  | Status |
|-----|-------------|--------|--------------|--------|------------------|------------------|------------------------|--------|
| R1  | Pad sistema | Preopterećenje servera ili greška u sistemu | Srednja | Visok | Visok | Preventivno održavanje, Monitoring i alarmi | DevOps inženjer | Otvoren |
| R2  | Preopterećenje sistema | Veliki broj istovremenih korisnika | Srednja | Visok | Visok | Redovno testiranje opterećenja (Load Testing), Optimizacija koda i baza podataka | DevOps inženjer | Otvoren |
| R3  | Nestanak internet konekcije | Nestabilna mreža korisnika ili servera | Srednja | Srednja | Srednji | Retry mehanizmi | DevOps inženjer | Otvoren |
| R4  | Gubitak podataka | Nedostatak backup mehanizma | Niska | Visok | Visok | Redovan backup | Backend developer / DBA | Otvoren |
| R5  | Neovlašten pristup podacima | Slaba autentifikacija ili loše role kontrole | Srednja | Visok | Visok | Enkripcija podataka, 2FA | Security inženjer | Otvoren |
| R6  | Curenje podataka korisnika | Sigurnosne rupe ili loša enkripcija | Niska | Visok | Visok | Enkripcija podataka, 2FA | Security inženjer | Otvoren |
| R7  | Kašnjenje u razvoju | Loša procjena vremena i resursa | Visoka | Srednja | Visok | Implementacija Agile metodologije | Projekt menadžer | Otvoren |
| R8  | Problemi s registracijom korisnika | Bug u validaciji ili backend logici | Srednja | Srednja | Srednji | Validacija inputa na frontend i backend strani | Backend developer | Otvoren |
| R9  | Neispravno slanje email notifikacija | Problemi u SMTP konfiguraciji ili email servisu | Srednja | Srednja | Srednji | Implementacija logova i alerta, Validacija e-mail adresa, Ažuriranje sistema | Backend developer | Otvoren |
| R10 | Korisnici ne znaju koristiti sistem | Neintuitivan UI i nedostatak uputstava | Srednja | Srednja | Srednji | Pisanje vodiča | UI dizajner | Otvoren |
| R11 | Loše korisničko iskustvo | Neoptimizovan UI dizajn | Srednja | Srednja | Srednji | User Acceptance Testing | UI dizajner | Otvoren |
| R12 | Nemogućnost kreiranja konferencije | Backend greška ili validacija | Srednja | Visok | Visok | Validacija podataka i error handling na API-ju | Backend developer | Otvoren |
| R13 | Neispravan prikaz konferencija | Frontend ili API greške | Srednja | Srednja | Srednji | Provjera integracije između backend-a i frontenda | Frontend developer | Otvoren |
| R14 | Preklapanje termina sesija | Nedostatak validacije rasporeda | Srednja | Visok | Visok | Logika provjere konflikta termina prije spremanja | Backend developer | Otvoren |
| R15 | Neispravna dodjela dvorana | Logička greška u povezivanju sesija i dvorana | Srednja | Visok | Visok | Provjera dostupnosti dvorane prije dodjele | Backend developer | Otvoren |
| R16 | Greške u rasporedu konferencija | Kompleksna logika planiranja | Srednja | Visok | Visok | Optimizacija algoritma raspoređivanja i testiranje | Backend developer | Otvoren |
| R17 | Prekoračenje kapaciteta konferencije | Nedostatak kontrole broja prijava | Srednja | Visok | Visok | Validacija kapaciteta prije prijave | Backend developer | Otvoren |
| R18 | Neuspjela prijava na konferenciju | Backend ili validacijska greška | Srednja | Visok | Visok | Error handling i retry logika | Backend developer | Otvoren |
| R19 | Neispravna evidencija korisnika | Greška u bazi ili duplikati podataka | Srednja | Visok | Visok | Validacija i constraint u bazi podataka | Backend developer / DBA | Otvoren |
| R20 | Q&A sistem ne radi tokom sesije | Problem real-time komunikacije | Srednja | Srednja | Srednji | Rezervni mehanizmi i monitoring sistema | Backend developer | Otvoren |
| R21 | Greška u izvještaju za organizatore | Pogrešna logika agregacije podataka | Srednja | Srednja | Srednji | Testiranje izvještaja i validacija podataka | Backend developer | Otvoren |
| R22 | Nedostupnost izvještaja za organizatore | Server ili API problem | Niska | Srednja | Srednji | Caching i fallback pristup | DevOps inženjer | Otvoren |
| R23 | Greška u dodjeli opreme | Logička greška ili konflikt resursa | Srednja | Srednja | Srednji | Validacija dostupnosti prije dodjele | Backend developer | Otvoren |
| R24 | Nedostupnost opreme | Nema dostupnih resursa ili greška sistema | Srednja | Srednja | Srednji | Upravljanje inventarom i rezervacijama | Organizator / Admin | Otvoren |
| R25 | Nemogućnost upload materijala | Storage limit ili server error | Srednja | Srednja | Srednji | Optimizacija storage sistema i limit kontrola | Backend developer | Otvoren |
| R26 | Nefunkcionisanje notifikacija | Greška u notification servisu | Srednja | Srednja | Srednji | Monitoring servisa i retry mehanizam | Backend developer | Otvoren |
| R27 | Promjena emaila bez verifikacije | Nedostatak sigurnosne provjere | Srednja | Visok | Visok | Verifikacija email promjene | Backend developer / Security | Otvoren |
| R28 | Nevalidni datumi | Nedostatak validacije unosa | Srednja | Srednja | Srednji | Validacija datuma na frontendu i backendu | Backend developer | Otvoren |
| R29 | Upload zlonamjernih fajlova | Nedostatak sigurnosne kontrole fajlova | Niska | Visok | Visok | Antivirus skeniranje i file validacija | Security inženjer | Otvoren |
| R30 | Dupla rezervacija opreme | Problem konkurentnog pristupa podacima | Srednja | Visok | Visok | Locking mehanizam pri rezervaciji | Backend developer | Otvoren |
| R31 | Neprimjeren sadržaj tokom Q&A | Nedostatak moderacije sadržaja | Srednja | Srednja | Srednji | Moderacija sadržaja i filter riječi | Moderator / Admin | Otvoren |
| R32 | Problem istovremenog pristupa podacima | Više korisnika mijenja iste podatke u isto vrijeme | Srednja | Visok | Visok | Locking mehanizam | Backend developer | Otvoren |
| R33 | Nemogućnost vraćanja sistema na prethodno stanje | Nedostatak rollback mehanizma | Niska | Visok | Visok | Backup i rollback strategija | DevOps inženjer | Otvoren |
| R34 | Loša skalabilnost sistema | Sistem ne podržava veći broj korisnika | Srednja | Visok | Visok | Skaliranje sistema i raspodjela opterećenja | DevOps inženjer | Otvoren |
| R35 | Pokušaji pogađanja lozinke | Brute-force napadi | Visoka | Visok | Visok | Ograničavanje pokušaja prijave i zaključavanje naloga | Security inženjer | Otvoren |
| R36 | Krađa korisničke sesije | Session hijacking | Srednja | Visok | Visok | Zaštita sesije i automatsko odjavljivanje nakon određenog vremena | Security inženjer | Otvoren |
| R37 | Nekonzistentnost podataka u sistemu | Nedovoljna validacija poslovnih pravila | Srednja | Visok | Visok | Poslovna pravila validacije | Backend developer | Otvoren |
| R38 | Netačni podaci u izvještajima | Pogrešna logika agregacije | Srednja | Srednja | Srednji | Validacija izvještajnih upita | Backend developer | Otvoren |
| R39 | Predugo učitavanje podataka u listi | Nedostatak paginacije | Srednja | Srednja | Srednji | Implementacija paginacije | Frontend developer | Otvoren |
| R40 | Greška u obradi kotizacije | Bug u payment logici ili integraciji | Srednja | Visok | Visok | Evidencija transakcija i zaštita od duplog izvršavanja API zahtjeva | Backend developer | Otvoren |
| R41 | Dupla naplata učesnika | Konkurentni pristup plaćanju | Srednja | Visok | Visok | Provjera statusa uplate i zaštita od duplog procesiranja transakcije | Backend developer | Otvoren |
| R42 | Nedostupnost sistema za plaćanje | Eksterni servis nedostupan | Srednja | Visok | Visok | Retry mehanizam, fallback poruka korisniku i evidencija neuspjelih transakcija | Backend developer / DevOps | Otvoren |
| R43 | Kršenje GDPR propisa | Neadekvatno upravljanje ličnim podacima | Niska | Visok | Visok | Implementacija privacy policy, enkripcija podataka, kontrola pristupa i pravo na brisanje podataka | Security inženjer / Pravni savjetnik | Otvoren |
