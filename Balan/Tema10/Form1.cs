using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics;

namespace OpenTK_winforms_z02 {
    public partial class Form1 : Form {

        //Stări de control cameră.
        private float eyePosX, eyePosY, eyePosZ;//TEMA 10 SCHIMBAT LA FLOAT PENTRU CALCULUL DE CORECTIE AL PERSPECTIVEI

        private Point mousePos;
        private float camDepth;

        //Stări de control mouse.
        private bool statusControlMouse2D, statusControlMouse3D, statusMouseDown;

        //Stări de control axe de coordonate.
        private bool statusControlAxe;

        //Stări de control iluminare.
        private bool lightON;
        private bool lightON_0;

        //Stări de control obiecte 3D.
        private string statusCube;

        //Texturare.
        private int[] textures = new int[5];//TEMA 10 INCA 3 TEXTURI PENTRU ANIMATIE
        private bool brick;
        private int colorTex;

        //Stări de control obiecte 3D.
        private bool statusCubeT;
        private bool statusCubeTex1;
        private bool statusCubeTex2;
        private bool statusCubeTex3;
        private bool statusCubeTex4;

        //Structuri de stocare a vertexurilor și a listelor de vertexuri.
        private int[,] arrVertex = new int[50, 3];         //Stocam matricea de vertexuri; 3 coloane vor reține informația pentru X, Y, Z. Nr. de linii definește nr. de vertexuri.
        private int nVertex;

        private int[] arrQuadsList = new int[100];        //Lista de vertexuri pentru construirea cubului prin intermediul quadurilor. Ne bazăm pe lista de vertexuri.
        private int nQuadsList;

        private int[] arrTrianglesList = new int[100];    //Lista de vertexuri pentru construirea cubului prin intermediul triunghiurilor. Ne bazăm pe lista de vertexuri.
        private int nTrianglesList;

        //Fișiere de in/out pentru manipularea vertexurilor.
        private string fileVertex = "vertexList.txt";
        private string fileQList = "quadsVertexList.txt";
        private string fileTList = "trianglesVertexList.txt";
        private bool statusFiles;

        //Dim valuesAmbientTemplate0() As Single = {255, 0, 0, 1.0}      //Valori alternative ambientale(lumină colorată)
        //# SET 1
        private float[] valuesAmbientTemplate0 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesDiffuseTemplate0 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        private float[] valuesSpecularTemplate0 = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
        private float[] valuesPositionTemplate0 = new float[] { 0.0f, 0.0f, 5.0f, 1.0f };
        //# SET 2
        //private float[] valuesAmbientTemplate0 = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
        //private float[] valuesDiffuseTemplate0 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        //private float[] valuesSpecularTemplate0 = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        //private float[] valuesPositionTemplate0 = new float[] { 1.0f, 1.0f, 1.0f, 0.0f };

        private float[] valuesAmbient0 = new float[4];
        private float[] valuesDiffuse0 = new float[4];
        private float[] valuesSpecular0 = new float[4];
        private float[] valuesPosition0 = new float[4];
        
        //TEMA 10 VARIABILE FOLOSITE
        private List<Vector3> loadedVertices = new List<Vector3>();
        // Orbital parameters
        private float earthOrbitSpeed = 5.0f; // Speed of Earth's orbit around the Sun
        private float moonOrbitSpeed = 30.0f; // Speed of Moon's orbit around the Earth
        private float sunRotationSpeed = 10.0f;

        private float moonInnerRotationSpeed = 15;
        private float earthInnerRotationSpeed = 30;

        private float earthOrbitRadius = 125.0f; // Distance from Sun to Earth
        private float moonOrbitRadius = 25.0f; // Distance from Earth to Moon

        private float earthAngle = 0.0f; // Current angle of Earth in its orbit
        private float moonAngle = 0.0f; // Current angle of Moon in its orbit
        private float sunInnerRotationAngle = 0.0f;
        private float earthInnerRotationAngle = 0.0f;
        private float moonInnerRotationAngle = 0.0f;

        private bool textureLoaded =false;
        private Timer updateTimer;
        private float fieldOfView = 60.0f; // Field of view in degrees
        private float initialDistance; 
        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   ON_LOAD
        public Form1() {
            InitializeComponent();

            /// TODO:
            /// În fișierul <Form1.Designer.cs>, la linia 26 înlocuiți conțînutul original cu linia de mai jos:
            ///         this.GlControl1 = new OpenTK.GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 8));
            /// Acest mod de inițializare va activa antialiasing-ul (multisampling MSAA la 8x).
            /// ATENTȚIE!
            /// Veți pierde designerul grafic. Aplicația funcționează dar pentru a putea accesa designerul grafic va trebui să reveniți la constructorul
            /// implicit al controlului OpenTK!
            /// 
            //TEMA 10
            //TIMER PENTRU UPDATE AL ANIMATIEI
            updateTimer = new Timer();
            updateTimer.Interval = 16; // Approximately 60 FPS
            updateTimer.Tick += new EventHandler(OnTimerTick);
            updateTimer.Start();

        }

        private void Form1_Load(object sender, EventArgs e) {

            SetupValues();
            SetupWindowGUI();
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   SETARI INIȚIALE
        private void SetupValues() {
            eyePosX = 100;
            eyePosY = 100;
            eyePosZ = 50;
            //TEMA 10 DISTANTA INITIALA A CAMEREI PENTRU CORECTIA PERSPECTIVEI
            initialDistance = (float)Math.Sqrt(eyePosX * eyePosX + eyePosY * eyePosY + eyePosZ * eyePosZ);

            camDepth = 10f;

            setLight0Values();

            numericXeye.Value = (decimal)eyePosX;
            numericYeye.Value = (decimal)eyePosY;
            numericZeye.Value = (decimal)eyePosZ;

            statusCubeTex1 = false;
            statusCubeTex2 = false;
            statusCubeTex3 = false;
            statusCubeTex4 = false;
            brick = true;
            colorTex = 0;
        }

        private void SetupWindowGUI() {

            setControlMouse2D(false);
            setControlMouse3D(false);

            numericCameraDepth.Value = (int)camDepth;

            setControlAxe(false);

            setCubeStatus("OFF");
            setIlluminationStatus(false);
            setSource0Status(false);

            setTrackLigh0Default();
            setColorAmbientLigh0Default();
            setColorDifuseLigh0Default();
            setColorSpecularLigh0Default();
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   MANIPULARE VERTEXURI ȘI LISTE DE COORDONATE.
        //Încărcarea coordonatelor vertexurilor și lista de compunere a obiectelor 3D.
        private void loadVertex() {

            //Testăm dacă fișierul există
            try {
                StreamReader fileReader = new StreamReader((fileVertex));
                nVertex = Convert.ToInt32(fileReader.ReadLine().Trim());
                Console.WriteLine("Vertexuri citite: " + nVertex.ToString());

                string tmpStr = "";
                string[] str = new string[3];
                for (int i = 0; i < nVertex; i++) {
                    tmpStr = fileReader.ReadLine();
                    str = tmpStr.Trim().Split(' ');
                    arrVertex[i, 0] = Convert.ToInt32(str[0].Trim());
                    arrVertex[i, 1] = Convert.ToInt32(str[1].Trim());
                    arrVertex[i, 2] = Convert.ToInt32(str[2].Trim());
                }
                fileReader.Close();

            } catch (Exception e) {
                statusFiles = false;
                Console.WriteLine("Fisierul cu informații vertex <" + fileVertex + "> nu exista!");
                MessageBox.Show("Fisierul cu informații vertex <" + fileVertex + "> nu exista!");
            }
        }

        private void loadQList() {

            //Testăm dacă fișierul există
            try {
                StreamReader fileReader = new StreamReader(fileQList);

                int tmp;
                string line;
                nQuadsList = 0;

                while ((line = fileReader.ReadLine()) != null) {
                    tmp = Convert.ToInt32(line.Trim());
                    arrQuadsList[nQuadsList] = tmp;
                    nQuadsList++;
                }

                fileReader.Close();
            } catch (Exception e) {
                statusFiles = false;
                MessageBox.Show("Fisierul cu informații vertex <" + fileQList + "> nu exista!");
            }

        }

        private void loadTList() {

            //Testăm dacă fișierul există
            try {
                StreamReader fileReader = new StreamReader(fileTList);

                int tmp;
                string line;
                nTrianglesList = 0;

                while ((line = fileReader.ReadLine()) != null) {
                    tmp = Convert.ToInt32(line.Trim());
                    arrTrianglesList[nTrianglesList] = tmp;
                    nTrianglesList++;
                }

                fileReader.Close();
            } catch (Exception e) {
                statusFiles = false;
                MessageBox.Show("Fisierul cu informații vertex <" + fileTList + "> nu exista!");
            }

        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   CONTROL CAMERĂ

        //Controlul camerei după axa X cu spinner numeric (un cadran).
        private void numericXeye_ValueChanged(object sender, EventArgs e) {
            eyePosX = (int)numericXeye.Value;
            GlControl1.Invalidate(); //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
        }
        //Controlul camerei după axa Y cu spinner numeric (un cadran).
        private void numericYeye_ValueChanged(object sender, EventArgs e) {
            eyePosY = (int)numericYeye.Value;
            GlControl1.Invalidate(); //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
        }
        //Controlul camerei după axa Z cu spinner numeric (un cadran).
        private void numericZeye_ValueChanged(object sender, EventArgs e) {
            eyePosZ = (int)numericZeye.Value;
            GlControl1.Invalidate(); //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
        }
        //Controlul adâncimii camerei față de (0,0,0).
        private void numericCameraDepth_ValueChanged(object sender, EventArgs e) {
            camDepth = 1 + ((float)numericCameraDepth.Value) * 0.1f;

            // Update camera position based on camDepth
            //TEMA 10 UPDATE AL CAMEREI BAZAT PE ADANCIMEA CAMEREI, ACEST UPDATE ADUCE O CORECTIE PERSPECTIVEI ALTFEL APARE EFECT DE FISH EYE
            eyePosZ = camDepth * initialDistance; // initialDistance is the initial distance of the camera from the target

            GlControl1.Invalidate();
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   CONTROL MOUSE
        //Setăm variabila de stare pentru rotația în 2D a mouseului.
        private void setControlMouse2D(bool status) {
            if (status == false) {
                statusControlMouse2D = false;
                btnMouseControl2D.Text = "2D mouse control OFF";
            } else {
                statusControlMouse2D = true;
                btnMouseControl2D.Text = "2D mouse control ON";
            }
        }
        //Setăm variabila de stare pentru rotația în 3D a mouseului.
        private void setControlMouse3D(bool status) {
            if (status == false) {
                statusControlMouse3D = false;
                btnMouseControl3D.Text = "3D mouse control OFF";
            } else {
                statusControlMouse3D = true;
                btnMouseControl3D.Text = "3D mouse control ON";
            }
        }

        //Controlul mișcării setului de coordonate cu ajutorul mouseului (în plan 2D/3D)
        private void btnMouseControl2D_Click(object sender, EventArgs e) {
            if (statusControlMouse2D == true) {
                setControlMouse2D(false);
            } else {
                setControlMouse3D(false);
                setControlMouse2D(true);
            }
        }
        private void btnMouseControl3D_Click(object sender, EventArgs e) {
            if (statusControlMouse3D == true) {
                setControlMouse3D(false);
            } else {
                setControlMouse2D(false);
                setControlMouse3D(true);
            }
        }

        //Mișcarea lumii 3D cu ajutorul mouselui (click'n'drag de mouse).
        private void GlControl1_MouseMove(object sender, MouseEventArgs e) {
            if (statusMouseDown == true) {
                mousePos = new Point(e.X, e.Y);
                GlControl1.Invalidate();     //Forțează redesenarea întregului control OpenGL. Modificările vor fi luate în considerare (actualizare).
            }
        }
        private void GlControl1_MouseDown(object sender, MouseEventArgs e) {
            statusMouseDown = true;
        }
        private void GlControl1_MouseUp(object sender, MouseEventArgs e) {
            statusMouseDown = false;
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   CONTROL ILUMINARE
        //Setăm variabila de stare pentru iluminare.
        private void setIlluminationStatus(bool status) {
            if (status == false) {
                lightON = false;
                btnLights.Text = "Iluminare OFF";
            } else {
                lightON = true;
                btnLights.Text = "Iluminare ON";
            }
        }

        //Activăm/dezactivăm iluminarea.
        private void btnLights_Click(object sender, EventArgs e) {
            if (lightON == false) {
                setIlluminationStatus(true);
            } else {
                setIlluminationStatus(false);
            }
            GlControl1.Invalidate();
        }

        //Identifică numărul maxim de lumini pentru implementarea curentă a OpenGL.
        private void btnLightsNo_Click(object sender, EventArgs e) {
            int nr = GL.GetInteger(GetPName.MaxLights);
            MessageBox.Show("Nr. maxim de lumini pentru aceasta implementare OpenGL este <" + nr.ToString() + ">.");
        }

        //Setăm variabila de stare pentru sursa de lumină 0.
        private void setSource0Status(bool status) {
            if (status == false) {
                lightON_0 = false;
                btnLight0.Text = "Sursa 0 OFF";
            } else {
                lightON_0 = true;
                btnLight0.Text = "Sursa 0 ON";
            }
        }

        //Activăm/dezactivăm sursa 0 de iluminare (doar dacă iluminarea este activă).
        private void btnLight0_Click(object sender, EventArgs e) {
            if (lightON == true) {
                if (lightON_0 == false) {
                    setSource0Status(true);
                } else {
                    setSource0Status(false);
                }
                GlControl1.Invalidate();
            }
        }

        //Schimbăm poziția sursei 0 de iluminare după axele XYZ.
        private void setTrackLigh0Default() {
            trackLight0PositionX.Value = (int)valuesPosition0[0];
            trackLight0PositionY.Value = (int)valuesPosition0[1];
            trackLight0PositionZ.Value = (int)valuesPosition0[2];
        }
        private void trackLight0PositionX_Scroll(object sender, EventArgs e) {
            valuesPosition0[0] = trackLight0PositionX.Value;
            GlControl1.Invalidate();
        }
        private void trackLight0PositionY_Scroll(object sender, EventArgs e) {
            valuesPosition0[1] = trackLight0PositionY.Value;
            GlControl1.Invalidate();
        }
        private void trackLight0PositionZ_Scroll(object sender, EventArgs e) {
            valuesPosition0[2] = trackLight0PositionZ.Value;
            GlControl1.Invalidate();
        }

        //Schimbăm culoarea sursei de lumină 0 (ambiental) în domeniul RGB.
        private void setColorAmbientLigh0Default() {
            numericLight0Ambient_Red.Value = (decimal)valuesAmbient0[0];
            numericLight0Ambient_Green.Value = (decimal)valuesAmbient0[1];
            numericLight0Ambient_Blue.Value = (decimal)valuesAmbient0[2];
        }
        private void numericLight0Ambient_Red_ValueChanged(object sender, EventArgs e) {
            valuesAmbient0[0] = (float)numericLight0Ambient_Red.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Ambient_Green_ValueChanged(object sender, EventArgs e) {
            valuesAmbient0[1] = (float)numericLight0Ambient_Green.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Ambient_Blue_ValueChanged(object sender, EventArgs e) {
            valuesAmbient0[2] = (float)numericLight0Ambient_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //Schimbăm culoarea sursei de lumină 0 (difuză) în domeniul RGB.
        private void setColorDifuseLigh0Default() {
            numericLight0Difuse_Red.Value = (decimal)valuesDiffuse0[0];
            numericLight0Difuse_Green.Value = (decimal)valuesDiffuse0[1];
            numericLight0Difuse_Blue.Value = (decimal)valuesDiffuse0[2];
        }
        private void numericLight0Difuse_Red_ValueChanged(object sender, EventArgs e) {
            valuesDiffuse0[0] = (float)numericLight0Difuse_Red.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Difuse_Green_ValueChanged(object sender, EventArgs e) {
            valuesDiffuse0[1] = (float)numericLight0Difuse_Green.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Difuse_Blue_ValueChanged(object sender, EventArgs e) {
            valuesDiffuse0[2] = (float)numericLight0Difuse_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //Schimbăm culoarea sursei de lumină 0 (specular) în domeniul RGB.
        private void setColorSpecularLigh0Default() {
            numericLight0Specular_Red.Value = (decimal)valuesSpecular0[0];
            numericLight0Specular_Green.Value = (decimal)valuesSpecular0[1];
            numericLight0Specular_Blue.Value = (decimal)valuesSpecular0[2];
        }
        private void numericLight0Specular_Red_ValueChanged(object sender, EventArgs e) {
            valuesSpecular0[0] = (float)numericLight0Specular_Red.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Specular_Green_ValueChanged(object sender, EventArgs e) {
            valuesSpecular0[1] = (float)numericLight0Specular_Green.Value / 100;
            GlControl1.Invalidate();
        }
        private void numericLight0Specular_Blue_ValueChanged(object sender, EventArgs e) {
            valuesSpecular0[2] = (float)numericLight0Specular_Blue.Value / 100;
            GlControl1.Invalidate();
        }

        //Resetare stare sursă de lumină nr. 0.
        private void setLight0Values() {
            for (int i = 0; i < valuesAmbientTemplate0.Length; i++) {
                valuesAmbient0[i] = valuesAmbientTemplate0[i];
            }
            for (int i = 0; i < valuesDiffuseTemplate0.Length; i++) {
                valuesDiffuse0[i] = valuesDiffuseTemplate0[i];
            }
            for (int i = 0; i < valuesPositionTemplate0.Length; i++) {
                valuesPosition0[i] = valuesPositionTemplate0[i];
            }
        }
        private void btnLight0Reset_Click(object sender, EventArgs e) {
            setLight0Values();
            setTrackLigh0Default();
            setColorAmbientLigh0Default();
            setColorDifuseLigh0Default();
            setColorSpecularLigh0Default();
            GlControl1.Invalidate();
        }

        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   CONTROL OBIECTE 3D
        //Setăm variabila de stare pentru afișarea/scunderea sistemului de coordonate.
        private void setControlAxe(bool status) {
            if (status == false) {
                statusControlAxe = false;
                btnShowAxes.Text = "Axe Oxyz OFF";
            } else {
                statusControlAxe = true;
                btnShowAxes.Text = "Axe Oxyz ON";
            }
        }

        //Controlul axelor de coordonate (ON/OFF).
        private void btnShowAxes_Click(object sender, EventArgs e) {
            if (statusControlAxe == true) {
                setControlAxe(false);
            } else {
                setControlAxe(true);
            }
            GlControl1.Invalidate();
        }

        //Setăm variabila de stare pentru desenarea cubului. Valorile acceptabile sunt:
        //TRIANGLES = cubul este desenat, prin triunghiuri.
        //QUADS = cubul este desenat, prin quaduri.
        //OFF (sau orice altceva) = cubul nu este desenat.
        private void setCubeStatus(string status) {
            if (status.Trim().ToUpper().Equals("TRIANGLES")) {
                statusCube = "TRIANGLES";
            } else if (status.Trim().ToUpper().Equals("QUADS")) {
                statusCube = "QUADS";
            } else if(status.Trim().ToUpper().Equals("CUSTOM_QUADS"))
            {
                statusCube = "CUSTOM_QUADS";
            }
            else {
                statusCube = "OFF";
            }
        }
        private void btnCubeQ_Click(object sender, EventArgs e) {
            statusFiles = true;
            loadVertex();
            loadQList();
            setCubeStatus("QUADS");
            GlControl1.Invalidate();
        }
        //TEMA 10 LOAD CUSTOM QUADS CUBE
        private void customBtnCubeQ_Click(object sender, EventArgs e) {
            statusFiles = true;
            setCubeStatus("CUSTOM_QUADS");
            GlControl1.Invalidate();
        }

        private void btnCubeT_Click(object sender, EventArgs e) {
            statusFiles = true;
            loadVertex();
            loadTList();
            setCubeStatus("TRIANGLES");
            GlControl1.Invalidate();
        }
        private void btnResetObjects_Click(object sender, EventArgs e) {
            setCubeStatus("OFF");
            GlControl1.Invalidate();
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   ADMINISTRARE MOD 3D (METODA PRINCIPALĂ)
        private void GlControl1_Paint(object sender, PaintEventArgs e) {
            //Resetează buffer-ele la valori default.
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            //Culoarea default a mediului.
            GL.ClearColor(Color.Black);

            //TEMA 10 SCHIMBARI CE ADUC O CORECTIE PERSPECTIVEI
            //Setări preliminară pentru mediul 3D.
            // In the GlControl1_Paint method
            float fovRadians = fieldOfView * (float)Math.PI / 180.0f;
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fovRadians, 4 / 3, 1, 10000);

            //Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView((float)camDepth, 4 / 3, 1, 10000);    //Declararea perspectivei spatiale.
            Matrix4 lookat = Matrix4.LookAt(eyePosX, eyePosY, eyePosZ, 0, 0, 0, 0, 1, 0);                   //Declararea camerei (stare inițială).
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);                                                             //Încărcarea modelului camerei.
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, GlControl1.Width, GlControl1.Height);                                         //Mărimea suprafeței randate (scena 3D este proiectată pe aceasta).
            GL.Enable(EnableCap.DepthTest);                                                                 //Corecții de adâncime.
            GL.DepthFunc(DepthFunction.Less);                                                               //Corecții de adâncime.

            //Pornim iluminarea (daca avem permisiunea să o facem).
            if (lightON == true) {
                //Permite utilizarea iluminării. Fara aceasta corecție iluminarea nu functioneaza.
                GL.Enable(EnableCap.Lighting);
            } else {
                //Dezactivează utilizarea iluminării.
                GL.Disable(EnableCap.Lighting);
            }

            //Se creeaza sursa de iluminare. In acest caz folosim o singura sursa.
            //Numarul de surse de lumini depinde de implementarea OpenGL, dar de obicei cel putin 8 surse sunt posibile simultan.
            GL.Light(LightName.Light0, LightParameter.Ambient, valuesAmbient0);
            GL.Light(LightName.Light0, LightParameter.Diffuse, valuesDiffuse0);
            GL.Light(LightName.Light0, LightParameter.Specular, valuesSpecular0);
            GL.Light(LightName.Light0, LightParameter.Position, valuesPosition0);

            if ((lightON == true) && (lightON_0 == true)) {
                //Activam sursa 0 de lumina. Fara aceasta actiune nu avem iluminare.
                GL.Enable(EnableCap.Light0);
            } else {
                //Dezactivam sursa 0 de lumina.
                GL.Disable(EnableCap.Light0);
            }

            //Controlul rotației cu mouse-ul (2D).
            if (statusControlMouse2D == true) {
                GL.Rotate(mousePos.X, 0, 1, 0);
            }

            //Controlul rotației cu mouse-ul (3D).
            if (statusControlMouse3D == true) {
                GL.Rotate(mousePos.X, 0, 1, 1);
            }
            //---------------------------
            //---------------------------
            //Texturare.
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //TEMA 10 AM ADAUGAT UN CHECK AICI PENTRU A NU INCARCA TEXTURILE LA FIECARE UPDATE. 
            //FARA ACEST IF AM AVEA UN MEMORY LEAK IMENS DEOARECE TEXTURILE SE INCARCA DIN NOU SI DIN NOW LA NESFARSIT
            if (!textureLoaded)
            {
                LoadTextures();
                textureLoaded = true;
            }
            

            //---------------------------
            //---------------------------
            //Descrierea obiectelor 3D!!! Axe de coordonate.
            if (statusControlAxe == true) {
                DeseneazaAxe();
            }

            //Desenăm obiectele 3D (cub format din quads sau triunghiuri).
            if (statusCube.ToUpper().Equals("QUADS")) {
                DeseneazaCubQ();
            } else if (statusCube.ToUpper().Equals("TRIANGLES")) {
                DeseneazaCubT();
            } else if (statusCube.ToUpper().Equals("CUSTOM_QUADS"))
            {
                DrawLoadedCubeQuads();
            }

            if (statusCubeTex1 == true) {
                DeseneazaCubQ_Tex1();
            }
            if (statusCubeTex2 == true) {
                DeseneazaCubQ_Tex2();
            }
            if (statusCubeTex3 == true) {
                DeseneazaCubT_Tex3();
            }
            if (statusCubeTex4 == true) {
                DeseneazaCubT_Tex4();
            }

            //Limitează viteza de randare pentru a nu supraîncarca unitatea GPU (în caz contrar randarea se face cât de rapid este posibil, pe 100% din resurse). 
            //Dezavantajul este că o viteză prea mică poate afecta negativ cursivitatea animației!
            //GraphicsContext.CurrentContext.SwapInterval = 1;                                         //Testati cu valori din 10 in 10!!!
            //GraphicsContext.CurrentContext.VSync = True;
            
            
            //TEMA 10 DESENARE OBIECTE ANIMATIE
            //DESENARE SOARE
            DrawSphere(50.0, 30, 30, new Vector3(0, 0, 0), textures[2], sunInnerRotationAngle);


            //DESENARE PAMANT
            DrawOrbitingSphere(12.5, 30, 30, earthOrbitRadius, earthAngle, new Vector3(0, 0, 0), textures[3],earthInnerRotationAngle);

            //CALCULARE POZITIE CURENTA PAMANT
            Vector3 earthPosition = new Vector3(
                earthOrbitRadius * (float)Math.Cos(earthAngle * Math.PI / 180.0),
                0,
                earthOrbitRadius * (float)Math.Sin(earthAngle * Math.PI / 180.0)
            );

            //DESENARE LUNA BAZATA PE POZITIA CURENTA A PAMANTULUI
            DrawOrbitingSphere(5.0, 30, 30, moonOrbitRadius, moonAngle, earthPosition, textures[4],moonInnerRotationAngle);
            GlControl1.SwapBuffers();
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   DESENARE OBIECTE 3D
        //Desenează axe XYZ.
        private void DeseneazaAxe() {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(75, 0, 0);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 75, 0);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 75);
            GL.End();
        }

        //Desenează cubul - quads.
        private void DeseneazaCubQ() {
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < nQuadsList; i++) {
                switch (i % 4) {
                    case 0:
                        GL.Color3(Color.Blue);
                        break;
                    case 1:
                        GL.Color3(Color.Red);
                        break;
                    case 2:
                        GL.Color3(Color.Green);
                        break;
                    case 3:
                        GL.Color3(Color.Yellow);
                        break;
                }
                int x = arrQuadsList[i];
                GL.Vertex3(arrVertex[x, 0], arrVertex[x, 1], arrVertex[x, 2]);
            }
            GL.End();
        }

        //Desenează cubul - triunghuri.
        private void DeseneazaCubT() {
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < nTrianglesList; i++) {
                switch (i % 3) {
                    case 0:
                        GL.Color3(Color.Blue);
                        break;
                    case 1:
                        GL.Color3(Color.Red);
                        break;
                    case 2:
                        GL.Color3(Color.Green);
                        break;
                }
                int x = arrTrianglesList[i];
                GL.Vertex3(arrVertex[x, 0], arrVertex[x, 1], arrVertex[x, 2]);
            }
            GL.End();
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //   TEXTURARE
        private void LoadTextures() {
            GL.GenTextures(textures.Length, textures);
            LoadTexture(textures[0], "brickTexture.jpg");
            LoadTexture(textures[1], "OpenGLTexture.png");
            LoadTexture(textures[2], "2k_sun.jpg");
            LoadTexture(textures[3], "2k_earth.jpg");
            LoadTexture(textures[4], "2k_moon.jpg");
        }

        private void LoadTexture(int textureId, string filename)
        {
            Bitmap bmp = new Bitmap(filename);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          bmp.Width, bmp.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                          PixelType.UnsignedByte, data.Scan0);

            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
        }


        //Selecția imaginii texturii.
        private void rbTexture0_CheckedChanged(object sender, EventArgs e) {
            brick = true;
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) {
            brick = false;
        }
        //Selecția culorii texturii.
        private void rbColorWhite_CheckedChanged(object sender, EventArgs e) {
            colorTex = 0;
        }
        private void rbColorRed_CheckedChanged(object sender, EventArgs e) {
            colorTex = 1;
        }
        private void rbColorBlue_CheckedChanged(object sender, EventArgs e) {
            colorTex = 2;
        }

        //Încărcare cub triunghiuri.
        private void btnTexT_1_Click(object sender, EventArgs e) {
            resetTexCubes();
            loadVertex();
            loadTList();
            statusCubeTex3 = true;
            GlControl1.Invalidate();
        }

        private void btnTexT_2_Click(object sender, EventArgs e) {
            resetTexCubes();
            loadVertex();
            loadTList();
            statusCubeTex4 = true;
            GlControl1.Invalidate();
        }

        private void btnReset_Tex2_Click(object sender, EventArgs e) {
            resetTexCubes();
        }

        //Încărcare cub quad.
        private void btnTexQ_1_Click(object sender, EventArgs e) {
            resetTexCubes();
            loadVertex();
            loadQList();
            statusCubeTex1 = true;
            GlControl1.Invalidate();
        }

        private void btnTexQ_2_Click(object sender, EventArgs e) {
            resetTexCubes();
            loadVertex();
            loadQList();
            statusCubeTex2 = true;
            GlControl1.Invalidate();
        }

        private void btnReset_Tex1_Click(object sender, EventArgs e) {
            resetTexCubes();
        }

        private void resetTexCubes() {
            setCubeStatus("OFF");
            statusCubeTex1 = false;
            statusCubeTex2 = false;
            statusCubeTex3 = false;
            statusCubeTex4 = false;
            GlControl1.Invalidate();
        }
        //TEMA 10 CITIRE VERTEXURI DINTR-UN FISIER
        private void loadFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt"; // Text file filter 
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Reads and proccesses file
                LoadCoordinatesFromFile(openFileDialog.FileName);
            }
        }

        private void LoadCoordinatesFromFile(string filePath)
        {

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] coords = line.Split(' ');
                    if (coords.Length == 3)
                    {
                        float x = float.Parse(coords[0]);
                        float y = float.Parse(coords[1]);
                        float z = float.Parse(coords[2]);
                        loadedVertices.Add(new Vector3(x, y, z));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }
        }
        //TEMA 10 RANDOM COLOR GENERATOR
        private Color GenerateRandomColor()
        {
            Random rand = new Random();
            return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        //TEMA 10
        // Function to draw the cube with quads based on loaded vertices
        private void DrawLoadedCubeQuads()
        {
            if (loadedVertices.Count != 8)
            {
                MessageBox.Show("The number of vertices loaded is not correct for a cube. Expected 8 vertices.");
                return;
            }

            GL.Begin(PrimitiveType.Quads);

            // Front face
            GL.Color3(GenerateRandomColor());
            GL.Vertex3(loadedVertices[0]);
            GL.Vertex3(loadedVertices[1]);
            GL.Vertex3(loadedVertices[2]);
            GL.Vertex3(loadedVertices[3]);

            // Back face
            GL.Color3(GenerateRandomColor());
            GL.Vertex3(loadedVertices[4]);
            GL.Vertex3(loadedVertices[5]);
            GL.Vertex3(loadedVertices[6]);
            GL.Vertex3(loadedVertices[7]);

            // Top face
            GL.Color3(GenerateRandomColor());
            GL.Vertex3(loadedVertices[3]);
            GL.Vertex3(loadedVertices[2]);
            GL.Vertex3(loadedVertices[6]);
            GL.Vertex3(loadedVertices[7]);

            // Bottom face
            GL.Color3(GenerateRandomColor());
            GL.Vertex3(loadedVertices[0]);
            GL.Vertex3(loadedVertices[1]);
            GL.Vertex3(loadedVertices[5]);
            GL.Vertex3(loadedVertices[4]);

            // Right face
            GL.Color3(GenerateRandomColor());
            GL.Vertex3(loadedVertices[1]);
            GL.Vertex3(loadedVertices[2]);
            GL.Vertex3(loadedVertices[6]);
            GL.Vertex3(loadedVertices[5]);

            // Left face
            GL.Color3(GenerateRandomColor());
            GL.Vertex3(loadedVertices[0]);
            GL.Vertex3(loadedVertices[3]);
            GL.Vertex3(loadedVertices[7]);
            GL.Vertex3(loadedVertices[4]);

            GL.End();
        }
        //TEMA 10 DESENARE SPHERA CU TEXTURA, CULOARE WHITE CA TEXTURA SA FIE CEA naturala
        //SI CU UN ANUME UNGHI DE ROTATIE SETAT
        private void DrawSphere(double r, int lats, int longs, Vector3 position, int textureId, float innerRotationAngle)
        {
            GL.PushMatrix();
            GL.Translate(position);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Color3(Color.White);
            GL.Rotate(innerRotationAngle, 0.0f, 1.0f, 0.0f);
            for (int i = 0; i <= lats; i++)
            {
                double lat0 = Math.PI * (-0.5 + (double)(i - 1) / lats);
                double z0 = Math.Sin(lat0);
                double zr0 = Math.Cos(lat0);

                double lat1 = Math.PI * (-0.5 + (double)i / lats);
                double z1 = Math.Sin(lat1);
                double zr1 = Math.Cos(lat1);

                GL.Begin(PrimitiveType.QuadStrip);
                for (int j = 0; j <= longs; j++)
                {
                    double lng = 2 * Math.PI * (double)(j - 1) / longs;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);

                    GL.TexCoord2((double)j / longs, (double)(i - 1) / lats);
                    GL.Vertex3(r * x * zr0, r * y * zr0, r * z0);
                    GL.TexCoord2((double)j / longs, (double)i / lats);
                    GL.Vertex3(r * x * zr1, r * y * zr1, r * z1);
                }
                GL.End();
            }

            GL.PopMatrix();
        }

        //TEMA 10 DESENARE SPHERA CARE ORBITEAZA
        void DrawOrbitingSphere(double r, int lats, int longs, float orbitRadius, float angle, Vector3 orbitCenter, int textureId, float innerRotationAngle)
        {
            float x = orbitCenter.X + orbitRadius * (float)Math.Cos(angle * Math.PI / 180.0);
            float y = orbitCenter.Y;
            float z = orbitCenter.Z + orbitRadius * (float)Math.Sin(angle * Math.PI / 180.0);

            DrawSphere(r, lats, longs, new Vector3(x, y, z), textureId, innerRotationAngle);
        }

        //TEMA 10 TIMER
        private void OnTimerTick(object sender, EventArgs e)
        {
            // Calculate the elapsed time in seconds
            float deltaTime = updateTimer.Interval / 1000.0f;

            UpdateOrbitalPositions(deltaTime);


            GlControl1.Invalidate(); // Trigger a repaint
        }
        //TEMA 10 UPDATE ORBIT POSITIONS OF THE OBJECTS
        void UpdateOrbitalPositions(float deltaTime)
        {
            earthAngle += earthOrbitSpeed * deltaTime;
            moonAngle += moonOrbitSpeed * deltaTime;
            sunInnerRotationAngle += sunRotationSpeed * deltaTime;
            moonInnerRotationAngle += moonInnerRotationSpeed * deltaTime;
            earthInnerRotationAngle += earthInnerRotationSpeed * deltaTime;

            earthAngle = earthAngle % 360;
            moonAngle = moonAngle % 360;
            sunInnerRotationAngle %= 360;
            earthInnerRotationAngle %= 360;
            moonInnerRotationAngle %= 360;
        }


        //Desenează cubul - quads - textura 1 - 100%.
        private void DeseneazaCubQ_Tex1() {
            if (brick == true) {
                GL.BindTexture(TextureTarget.Texture2D, textures[0]);
            } else {
                GL.BindTexture(TextureTarget.Texture2D, textures[1]);
            }

            switch (colorTex) {
                case 0:
                    GL.Color3(Color.White);      //Culoarea albă permite maparea texturii fără alterarea culorii originale.
                    break;
                case 1:
                    GL.Color3(Color.FromArgb(0, 255, 0, 0));
                    break;
                case 2:
                    GL.Color3(Color.FromArgb(0, 0, 0, 255));
                    break;
            }

            //GL.Normal3(0.0, 0.0, 1.0);
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < nQuadsList; i = i + 4) {
                //Atenție la modul de specificare a coordonatelor texturilor vis-a-vis de lista de vertexuri de intrare!
                GL.TexCoord2(0.0, 1.0);
                GL.Vertex3(arrVertex[arrQuadsList[i], 0], arrVertex[arrQuadsList[i], 1], arrVertex[arrQuadsList[i], 2]);
                GL.TexCoord2(1.0, 1.0);
                GL.Vertex3(arrVertex[arrQuadsList[i + 1], 0], arrVertex[arrQuadsList[i + 1], 1], arrVertex[arrQuadsList[i + 1], 2]);
                GL.TexCoord2(1.0, 0.0);
                GL.Vertex3(arrVertex[arrQuadsList[i + 2], 0], arrVertex[arrQuadsList[i + 2], 1], arrVertex[arrQuadsList[i + 2], 2]);
                GL.TexCoord2(0.0, 0.0);
                GL.Vertex3(arrVertex[arrQuadsList[i + 3], 0], arrVertex[arrQuadsList[i + 3], 1], arrVertex[arrQuadsList[i + 3], 2]);
            }
            GL.End();
        }

        //Desenează cubul - quads - textura 1 - 50%.
        private void DeseneazaCubQ_Tex2() {
            if (brick == true) {
                GL.BindTexture(TextureTarget.Texture2D, textures[0]);
            } else {
                GL.BindTexture(TextureTarget.Texture2D, textures[1]);
            }

            switch (colorTex) {
                case 0:
                    GL.Color3(Color.White);      //Culoarea albă permite maparea texturii fără alterarea culorii originale.
                    break;
                case 1:
                    GL.Color3(Color.FromArgb(0, 255, 0, 0));
                    break;
                case 2:
                    GL.Color3(Color.FromArgb(0, 0, 0, 255));
                    break;
            }

            //GL.Normal3(0.0, 0.0, 1.0);
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < nQuadsList; i = i + 4) {
                GL.TexCoord2(0.0, 0.5);
                GL.Vertex3(arrVertex[arrQuadsList[i], 0], arrVertex[arrQuadsList[i], 1], arrVertex[arrQuadsList[i], 2]);
                GL.TexCoord2(0.5, 0.5);
                GL.Vertex3(arrVertex[arrQuadsList[i + 1], 0], arrVertex[arrQuadsList[i + 1], 1], arrVertex[arrQuadsList[i + 1], 2]);
                GL.TexCoord2(0.5, 0.0);
                GL.Vertex3(arrVertex[arrQuadsList[i + 2], 0], arrVertex[arrQuadsList[i + 2], 1], arrVertex[arrQuadsList[i + 2], 2]);
                GL.TexCoord2(0.0, 0.0);
                GL.Vertex3(arrVertex[arrQuadsList[i + 3], 0], arrVertex[arrQuadsList[i + 3], 1], arrVertex[arrQuadsList[i + 3], 2]);
            }
            GL.End();
        }

        //Desenează cubul - triangles - textura 1 - 100%.
        private void DeseneazaCubT_Tex3() {
            if (brick == true) {
                GL.BindTexture(TextureTarget.Texture2D, textures[0]);
            } else {
                GL.BindTexture(TextureTarget.Texture2D, textures[1]);
            }

            switch (colorTex) {
                case 0:
                    GL.Color3(Color.White);      //Culoarea albă permite maparea texturii fără alterarea culorii originale.
                    break;
                case 1:
                    GL.Color3(Color.FromArgb(0, 255, 0, 0));
                    break;
                case 2:
                    GL.Color3(Color.FromArgb(0, 0, 0, 255));
                    break;
            }

            //GL.Normal3(0.0, 0.0, 1.0);
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < nTrianglesList; i = i + 6) {
                //Atenție la modul de specificare a coordonatelor texturilor vis-a-vis de lista de vertexuri de intrare!
                //La maparea texturii pe triughi pot apare deformări in imaginea texturată, porțiuni lipsă, etc. Pentru o eficiență crescută se pot folosi 2 triughiuri
                //ce formează un pătrat pentru procesare.

                GL.TexCoord2(0.0, 0.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i], 0], arrVertex[arrTrianglesList[i], 1], arrVertex[arrTrianglesList[i], 2]);
                GL.TexCoord2(1.0, 0.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 1], 0], arrVertex[arrTrianglesList[i + 1], 1], arrVertex[arrTrianglesList[i + 1], 2]);
                GL.TexCoord2(0.0, 1.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 2], 0], arrVertex[arrTrianglesList[i + 2], 1], arrVertex[arrTrianglesList[i + 2], 2]);
                GL.TexCoord2(1.0, 0.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 3], 0], arrVertex[arrTrianglesList[i + 3], 1], arrVertex[arrTrianglesList[i + 3], 2]);
                GL.TexCoord2(0.0, 1.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 4], 0], arrVertex[arrTrianglesList[i + 4], 1], arrVertex[arrTrianglesList[i + 4], 2]);
                GL.TexCoord2(1.0, 1.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 5], 0], arrVertex[arrTrianglesList[i + 5], 1], arrVertex[arrTrianglesList[i + 5], 2]);
            }
            GL.End();
        }

        //Desenează cubul - triunghuri.
        private void DeseneazaCubT_Tex4() {
            if (brick == true) {
                GL.BindTexture(TextureTarget.Texture2D, textures[0]);
            } else {
                GL.BindTexture(TextureTarget.Texture2D, textures[1]);
            }

            switch (colorTex) {
                case 0:
                    GL.Color3(Color.White);      //Culoarea albă permite maparea texturii fără alterarea culorii originale.
                    break;
                case 1:
                    GL.Color3(Color.FromArgb(0, 255, 0, 0));
                    break;
                case 2:
                    GL.Color3(Color.FromArgb(0, 0, 0, 255));
                    break;
            }

            //GL.Normal3(0.0, 0.0, 1.0);
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < nTrianglesList; i = i + 6) {
                //Atenție la modul de specificare a coordonatelor texturilor vis-a-vis de lista de vertexuri de intrare!
                //La maparea texturii pe triughi pot apare deformări in imaginea texturată, porțiuni lipsă, etc. Pentru o eficiență crescută se pot folosi 2 triughiuri
                //ce formează un pătrat pentru procesare.

                GL.TexCoord2(0.0, 0.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i], 0], arrVertex[arrTrianglesList[i], 1], arrVertex[arrTrianglesList[i], 2]);
                GL.TexCoord2(0.5, 0.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 1], 0], arrVertex[arrTrianglesList[i + 1], 1], arrVertex[arrTrianglesList[i + 1], 2]);
                GL.TexCoord2(0.0, 0.5);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 2], 0], arrVertex[arrTrianglesList[i + 2], 1], arrVertex[arrTrianglesList[i + 2], 2]);
                GL.TexCoord2(0.5, 0.0);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 3], 0], arrVertex[arrTrianglesList[i + 3], 1], arrVertex[arrTrianglesList[i + 3], 2]);
                GL.TexCoord2(0.0, 0.5);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 4], 0], arrVertex[arrTrianglesList[i + 4], 1], arrVertex[arrTrianglesList[i + 4], 2]);
                GL.TexCoord2(0.5, 0.5);
                GL.Vertex3(arrVertex[arrTrianglesList[i + 5], 0], arrVertex[arrTrianglesList[i + 5], 1], arrVertex[arrTrianglesList[i + 5], 2]);
            }
            GL.End();
        }
    }
}
