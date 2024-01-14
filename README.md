# Laborator 2

1. Setarea viewportului determină aria de afișare unde se va realiza proiecția scenei 3D.
2 .Viewportul oferă posibilitatea de a emula o cameră, permițând vizualizarea obiectelor 3D din diverse perspective.
3. Rata de cadre pe secundă (FPS) indică numărul de imagini generate de aplicație în fiecare secundă.
4. Funcția OnUpdateFrame() este executată inițial o singură dată după OnLoad, apoi se repetă conform ratei stabilite de cadre pe secundă.
5. În abordarea de randare imediată, comenzi grafice sunt transmise direct către GPU pentru fiecare element individual de desenat.
6. De la versiunea OpenGL 3.0, modul imediat de randare nu mai este suportat, recomandându-se versiunea OpenGL 2.1.
7. Functia OnRenderFrame() este apelată după fiecare OnUpdateFrame, urmărind frecvența stabilită a cadrelor pe secundă.
8. OnResize() este necesară pentru a ajusta dimensiunea ferestrei la cea a ecranului, fiind executată cel puțin o dată.
9. Argumentele funcției CreatePerspectiveFieldOfView() includ:
   a) fieldOfView - unghiul de vizualizare între 0 și 180 grade;
   b) aspectRatio - proporția dintre lățime și înălțimea ferestrei;
   c) nearPlane - distanța minimă de randare de la cameră;
    d) farPlane - limita maximă a volumului de randare.
