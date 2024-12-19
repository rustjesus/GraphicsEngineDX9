using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GraphicsApp
{
    public partial class Form1 : Form
    {
        private SharpDX.Direct3D9.Device device9;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private Effect shaderEffect;
        private Matrix worldMatrix;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private float rotationSpeed;

        private Cube cube; // Cube instance
        public Form1()
        {
            InitializeComponent();
            InitializeGraphics();
            Application.Idle += RenderLoop;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Code to handle form load
            Console.WriteLine("Form1 loaded.");
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            // Code to handle form click
            Console.WriteLine("Form1 clicked.");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Code to handle key down
            Console.WriteLine($"Key pressed: {e.KeyCode}");
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Code to handle key up
            Console.WriteLine($"Key released: {e.KeyCode}");
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            // Code to handle form activation
            Console.WriteLine("Form1 activated.");
        }

        private void InitializeGraphics()
        {
            // Direct3D 9 device creation
            var presentParams = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.X8R8G8B8,
                BackBufferWidth = ClientSize.Width,
                BackBufferHeight = ClientSize.Height,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16
            };

            device9 = new SharpDX.Direct3D9.Device(
                new Direct3D(),
                0,
                DeviceType.Hardware,
                Handle,
                CreateFlags.HardwareVertexProcessing,
                presentParams
            );

            // Initialize matrices
            worldMatrix = Matrix.Identity;
            viewMatrix = Matrix.LookAtLH(new Vector3(0, 0, -10), Vector3.Zero, Vector3.UnitY);
            projectionMatrix = Matrix.PerspectiveFovLH((float)Math.PI / 4, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100.0f);

            LoadShaderEffect();

            // Initialize the cube
            cube = new Cube(device9);
        }
        private void LoadShaderEffect()
        {
                    // Locate the shader file

                    var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));
                    var shaderPath = Path.Combine(projectRoot, "Shaders", "CubeShader.fx");

                    // Debugging: Print the shader path
                    Console.WriteLine("Looking for shader file at: " + shaderPath);

                    if (!System.IO.File.Exists(shaderPath))
                        throw new Exception("Shader file not found: " + shaderPath);


                    // Load HLSL effect file
                    shaderEffect = Effect.FromFile(device9, shaderPath, ShaderFlags.None);
        }


        private void RenderLoop(object sender, EventArgs e)
        {
            while (Application.MessageLoop)
            {
                Render();
                Application.DoEvents();
            }
        }
        private void Render()
        {
            device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new RawColorBGRA(0, 128, 128, 255), 1.0f, 0);
            device9.BeginScene();

            // Set shader and matrices
            rotationSpeed += 0.01f;
            worldMatrix = Matrix.RotationY(rotationSpeed);
            Matrix worldViewProjection = worldMatrix * viewMatrix * projectionMatrix;

            shaderEffect.SetValue("WorldViewProjection", worldViewProjection);
            shaderEffect.Technique = "RenderTechnique";

            int numPasses = shaderEffect.Begin(FX.DoNotSaveState);

            for (int pass = 0; pass < numPasses; pass++)
            {
                shaderEffect.BeginPass(pass);
                cube.Render(); // Render the cube
                shaderEffect.EndPass();
            }

            shaderEffect.End();
            device9.EndScene();
            device9.Present();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            cube?.Dispose();
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            shaderEffect?.Dispose();
            device9?.Dispose();
            base.OnFormClosed(e);
        }
        private struct Vertex
        {
            public Vector3 Position;
            public int Color;

            public Vertex(Vector3 position, int color)
            {
                Position = position;
                Color = color;
            }
        }
    }
}