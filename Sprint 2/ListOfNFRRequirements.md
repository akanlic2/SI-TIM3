| ID | Kategorija | Opis zahtjeva | Kako će se provjeravati | Prioritet | Napomena |
|----------|----------|----------|----------|----------|----------|
| NFR-01 | Performanse | Sistem mora učitati glavnu stranicu u roku od maksimalno 2 sekunde | Mjerenjem vremena učitavanja pomoću alata, npr. browser developer tools | Visok | Odnosi se na standardne uslove mreže |
| NFR-02 | Performanse | Sistem treba podržati najmanje 100 istovremenih korisnika bez pada performansi | Load testiranje (simulacija korisnika) | Visok | Važno za veće konferencije |
| NFR-03 | Sigurnost | Lozinke korisnika moraju biti kriptovane u bazi podataka | Pregled implementacije (hash funkcije) | Visok |  |
| NFR-04 | Sigurnost | Sistem mora omogućiti autentifikaciju i autorizaciju korisnika | Testiranje pristupa (različite uloge korisnika) | Visok | Uloge uključuju admina, učesnika, organizatora i slično |
| NFR-05 | Upotrebljivost | Korisnik mora moći izvršiti prijavu na konferenciju u maksimalno 3 koraka | Testiranje korisničkog interfejsa | Srednji | Fokus je na jednostavnosti korištenja |
| NFR-06 | Pouzdanost | Sistem mora imati dostupnost od najmanje 99% vremena | Praćenje uptime vremena sistema | Visok |  |
| NFR-07 | Pouzadnost | Sistem mora praviti backup podataka jednom dnevno | Provjera backup logova | Visok |  |
| NFR-08 | Kompatibilnost | Sistem mora raditi na modernim web preglednicima (Chrome, Firefox, Edge) | Testiranje na različitim browserima | Srednji |  |
| NFR-09 | Održivost | Kod sistema mora biti modularan i dokumentovan | Pregled koda i dokumentacije | Srednji | Olakšava budući razvoj |
| NFR-10 | Privatnost | Sistem mora zaštititi lične podatke korisnika | Analiza pristupa podacima | Visok | U skladu sa GDPR principima |
| NFR-11 | Upotrebljivost | Dizajn sistema mora biti responzivan (potpuno funkcionalan i na mobilnim i na desktop uređajima)   | Testiranje na različitim vrstama uređaja | Srednji  |  |
| NFR-12 | Upotrebljivost | Sve greške moraju biti prikazane jasno s konkretnim prijedlogom za rješavanje  |  | Visok |   |
| NFR-13 | Dostupnost | U slučaju djelimičnog kvara, ključne funkcije (pregled rasporeda, check-in) moraju ostati dostupne  |   | Srednji |  |
| NFR-14 | Upotrebljivost | Sistem mora imati konzistentan dizajn kroz sve stranice  | UI/UX evaluacija | Srednji |  |
| NFR-15 | Sigurnost | Sistem mora automatski odjaviti korisnika nakon perioda neaktivnosti (npr. 15 minuta)  | Testiranje sesija  | Srednji  | Spriječava zloupotrebu korisničkog računa  |
| NFR-16 | Integracija | Sistem mora omogućiti integraciju sa eksternim servisima (npr. email servis, servis za plaćanje)  |   | Srednji  |  |
| NFR-17 |  | Sistem treba podržavati više jezika  | Promjena jezika u interfejsu | Nizak |  |
| NFR-18 |  |   |  |  |  |
| NFR-19 |  |   |  |  |  |
| NFR-20 |  |   |  |  |  |
