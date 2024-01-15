# Raspunsuri tema 9 - Texturare


## 1.Utilizarea texturilor cu și fără transparență:
Când se utilizează imagini cu transparență (de exemplu, în format PNG cu un canal alfa),
zonele transparente ale texturii permit vizualizarea obiectelor sau culorilor care se află în spatele texturii.
În cazul imaginilor fără transparență, textura va acoperi complet suprafața pe care este aplicată,
fără a lăsa să se vadă ce se află în spatele ei.
Pentru a vedea efectul transparenței, trebuie să activați blending-ul în OpenGL prin GL.Enable(EnableCap.Blend)
și să setați modul de blending potrivit, de exemplu GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha).

## 2.Formate de imagine pentru texturare în OpenGL:
OpenGL suportă mai multe formate de imagine pentru texturi, inclusiv BMP, JPG, PNG, TGA, și altele.
Suportul pentru formatele de imagine depinde de biblioteca utilizată pentru încărcarea texturilor în OpenGL (de exemplu, stb_image.h sau SOIL).
PNG și TGA sunt populare pentru suportul lor pentru transparență prin canalul alfa.

## 3.Modificarea culorii obiectului texturat:
În OpenGL, dacă modificați culoarea unui obiect texturat prin manipularea canalelor RGB, efectul depinde 
de starea de blending și de modul în care textura este aplicată pe obiect. Dacă modificați culoarea înainte de a aplica textura, 
culoarea va servi ca o culoare de bază și va fi combinată cu culoarea texturii conform parametrilor de blending stabiliți.
De exemplu, dacă setați culoarea obiectului la roșu și textura conține verde, rezultatul final va depinde 
de cum sunt combinate aceste culori. Dacă blending-ul nu este activat, culoarea setată poate să nu aibă 
un efect vizibil, în timp ce cu blending activat, culorile pot fi mixate pentru a produce o nouă culoare.

## 4.Deosebiri între scena cu obiecte texturate cu iluminare activată vs. dezactivată:
4.1 Iluminare Activată: Când iluminarea este activată în OpenGL și aplicăm texturi pe obiecte, lumina din scenă 
afectează modul în care textura este percepută. Acest lucru înseamnă că umbrele, luminozitatea și 
specularitatea pot schimba aspectul texturii în funcție de poziția și intensitate
a surselor de lumină, precum și de proprietățile materialelor obiectului (cum ar fi reflexivitatea sau matitatea).
Texturile vor apărea mai luminoase sau mai întunecate și pot avea reflexii speculare în funcție de aceste setări.
4.2 Iluminare Dezactivată: Când iluminarea este dezactivată, texturile sunt afișate cu culorile lor originale,
indiferent de sursa de lumină sau de poziția acesteia în scenă. Textura va arăta la fel indiferent de unghiul
din care este privită sau de prezența surselor de lumină, ceea ce poate duce la o scenă cu aspect mai plat și 
lipsită de profunzime.
În general, activarea iluminării într-o scenă 3D adaugă realism, deoarece permite simularea interacțiunii dintre
lumină și suprafețele obiectelor. În contrast, dezactivarea iluminării poate fi utilă pentru stiluri vizuale care
nu necesită efecte realiste sau atunci când se dorește un control complet asupra aspectului fiecărui pixel, 
cum ar fi în cazul stilurilor artistice cel shading.