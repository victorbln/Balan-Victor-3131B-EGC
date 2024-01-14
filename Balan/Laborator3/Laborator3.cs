using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Laborator3 : GameWindow
    {
    private Color currentColor = Color.White;
    private float[] vertices;
    private float rotationAngle = 0;
    private Vector2 cameraPosition = Vector2.Zero;
    KeyboardState lastKeyPress;
    public Laborator3(int width, int height) : base(width, height, GraphicsMode.Default, "Triunghi OpenTK") { }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

        // Coordonatele triunghiului
        vertices = ReadVerticesFromFile(@"data.txt");
        //Console.WriteLine(vertices);

    }
    //citire din fisier date
    public float[] ReadVerticesFromFile(string relativePath)
    {
        List<float> vertices = new List<float>();

        try
        {

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo projectRoot = Directory.GetParent(baseDirectory).Parent.Parent;
            string fullPath = Path.Combine(projectRoot.FullName, relativePath);

            string[] lines = File.ReadAllLines(fullPath);
            foreach (var line in lines)
            {
                string[] coords = line.Split(',');
                if (coords.Length == 2)
                {
                    float x = float.Parse(coords[0].Trim());
                    float y = float.Parse(coords[1].Trim());
                    vertices.Add(x);
                    vertices.Add(y);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading file: " + ex.Message);
        }

        return vertices.ToArray();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Key.R) && !keyboard.Equals(lastKeyPress))//asta face ca doar dupa release button sa faca update
        {
            // Generați o culoare aleatorie
            currentColor = GetRandomColor();
        }

        // Modificați unghiul camerei cu ajutorul mouse-ului
        var mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            rotationAngle += 0.01f * mouse.X;
        }
        lastKeyPress = keyboard;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
        GL.Translate(-cameraPosition.X, -cameraPosition.Y, 0);
        GL.Rotate(rotationAngle, 0, 0, 1);

        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(currentColor);

        for (int i = 0; i < vertices.Length; i += 2)
        {
            GL.Vertex2(vertices[i], vertices[i + 1]);
        }

        GL.End();

        Context.SwapBuffers();
    }

    private Color GetRandomColor()
    {
        Random rand = new Random();
        int r = rand.Next(0, 256);
        int g = rand.Next(0, 256);
        int b = rand.Next(0, 256);

        return Color.FromArgb(r, g, b);
    }

    static void Main()
    {
        using (var app = new Laborator3(800, 600))
        {
            app.Run(60.0);
        }
    }
}

