# Cilj testiranja

Cilj testiranja sistema za organizaciju konferencija je osigurati da sve funkcionalnosti sistema rade ispravno, pouzdano i sigurno u skladu sa definisanim zahtjevima i acceptance kriterijima.

| Cilj      | Obim   | Kriterij uspjeha |
|----------|-----------|--------|
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |
|          |           |        |

# Nivoi testiranja

## Unit testiranje
Unit testiranje se provodi na nivou pojedinačnih komponenti i funkcija sistema, izolovano od ostalih dijelova sistema.

Obuhvata testiranje:
- Validacione logike za registraciju korisnika (format emaila, jačina lozinke, obavezna polja)
- Business logike za provjeru duplikata (email adrese, nazivi dvorana, preklapanje termina sesija)
- Funkcija za upravljanje kapacitetima dvorana i sesija
- Logike za izračun popunjenosti kapaciteta i ažuriranje slobodnih mjesta
- Validacije kotizacija (iznosi, kategorije, status plaćanja)
- Funkcija za generisanje obavijesti

## Integraciono testiranje
Integraciono testiranje provjerava ispravnu komunikaciju između modula sistema i ispravnost toka podataka između komponenti.

Ključne integracije koje se testiraju:
- Integracija autentifikacije i autorizacije — provjera da role ispravno ograničavaju pristup svim zaštićenim endpointima
- Integracija modula konferencije i sesija — provjera da sesije ispravno nasljeđuju kontekst konferencije
- Integracija dvorane i sesije — provjera da dodjela dvorane ispravno ažurira kapacitet i sprječava konflikt
- Integracija prijave učesnika i ažuriranja kapaciteta — provjera da se broj mjesta ažurira u realnom vremenu
- Integracija kotizacija i statusa prijave učesnika
- Integracija sistema obavijesti sa okidačkim događajima (prijava, promjena statusa, brisanje)
- Integracija Q&A modula sa sesijama i korisničkim profilima

## Sistemsko testiranje
Sistemsko testiranje provjerava ponašanje cjelokupnog sistema kao integrisane cjeline, uključujući end-to-end tokove i ne-funkcionalne zahtjeve.

Obuhvata:
- End-to-end testiranje kompletnog toka: registracija → prijava → kreiranje konferencije → dodavanje sesija → prijava učesnika → Q&A → izvještaj
- Testiranje sigurnosti: provjera da neautorizovani korisnici ne mogu pristupiti zaštićenim resursima (direktnim URL-om, manipulacijom zahtjeva)
- Testiranje konkurentnog pristupa: istovremena prijava više učesnika na sesiju sa ograničenim kapacitetom
- Testiranje konzistentnosti podataka nakon brisanja (konferencija, sesija, dvorane)
- Testiranje performansi pri pregledu lista sa većim brojem zapisa
- Testiranje integriteta sesije nakon odjave korisnika
  
## Prihvatno testiranje
Prihvatno testiranje se provodi na osnovu acceptance kriterija definisanih u user stories. Cilj je potvrditi da sistem ispunjava poslovne zahtjeve i da je spreman za puštanje u produkciju.

Ovo testiranje obuhvata verifikaciju svakog acceptance kriterija za sve user storije, uz sudjelovanje vlasnika proizvoda ili krajnjih korisnika u finalnom pregledu.





# Šta se testira u kojem nivou

# Veza sa acceptance kriterijima

# Način evidentiranja rezultata testiranja

# Glavni rizici kvaliteta