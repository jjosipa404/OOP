# Projekt kolegija Objektno orijentirano programiranje 2018/2019

2D igrica u C#

Igrač: loptica koja se skakanjem penje po platformama i skuplja dijamante

Kretanje: space za skakanje, strelice za lijevo i desno

Cilj igre: ostvariti što veći broj bodova i popeti se na vrh rang ljestvice

Kraj igre:
	
  -ako igrač ispadne ispod donjeg ruba ekrana ili ostvari negativne bodove ( bomba, -1000)
	
  -ako je igra završila otvorit će se menu gdje je moguće odabrati spremanje scora, ili restart,ili izlaz iz igrice

Gems:
	
  -obični dijamanti donose igraču 10 bodova,
	
  -srcoliki dijamanti donose 500 bodova, 
	
  -ako igrač pokupi bombu to mu odnosi 1000 bodova

Platforme: 
	
  2 vrste: jedne se kreću prema dolje, druge prema gore
	  
  -platforme koje idu prema dolje: skokom na ovakvu platformu igrač dobije 100 bodova, ako
   padne s nje gubi 100 bodova, također skokom na ovakvu platformu broje se prijeđeni katovi
	
  -platforme koje idu prema gore: skokom na njih igrač ne dobiva nikakve bodove i njima se ne 
        broji broj prijeđenih katova
	
  -nakon određenog broja katova mijenjaju se boje platformi koje idu prema gore(kat 50,150,300)

Štoperica:
	
  -pokreće se nakon što igrač dosegne na Y osi 225
	
  -svako 30 sekundi štoperica ubrzava kretanje platformi
	
  -od 30s do 20s štoperice padaju bombe
	
  -od 10s do 0s štoperice padaju extra gems (+500)
                 -od 10s do 0s pokreće se magnet koji privlači sve dijamante igraču(bodovi za svaki gem se tada smanjuju upola), osim extra gems

Spremanje bodova:
	
  -ispisuju se iz datoteke samo igrači s 10 najviših rezultata
	
  -igrač ne može spremiti negativan rezultat
	
  -ako igrač ne upiše ime prilikom spremanja dodijeli mu se random (npr. Player123)

Opcije:
	
  -prilikom pokretanja igrač može odabrati boju loptice (plava,bijela)
	
  -ako nije ništa odabrao onda je plava loptica

Pauza:
	
  -tijekom igre pritiskom na tipku "P" otvara se Menu
	
  -igrač može odabrati da se vrati na igru i nastavi gdje je stao ili da izađe iz igre

