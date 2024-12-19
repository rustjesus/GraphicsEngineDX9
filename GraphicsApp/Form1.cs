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
        private SharpDX.Direct3D9.Device device;
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

            device = new SharpDX.Direct3D9.Device(
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

            // Initialize resources
            InitializeBuffers();
            LoadShaderEffect();

            // Initialize the cube
            cube = new Cube(device);
        }


        private void InitializeBuffers()
        {
            // Vertex buffer
            vertexBuffer = new VertexBuffer(
                device,
                Utilities.SizeOf<Vertex>() * 8,
                Usage.WriteOnly,
                VertexFormat.Position | VertexFormat.Diffuse,
                Pool.Default
            );

            var vertices = new[]
            {
                new Vertex(new Vector3(-1, -1, -1), unchecked((int)0xFFFF0000)), // Red
                new Vertex(new Vector3(-1, 1, -1), unchecked((int)0xFF00FF00)),  // Green
                new Vertex(new Vector3(1, 1, -1), unchecked((int)0xFF0000FF)),   // Blue
                new Vertex(new Vector3(1, -1, -1), unchecked((int)0xFFFFFF00)),  // Yellow
                new Vertex(new Vector3(-1, -1, 1), unchecked((int)0xFFFF00FF)),  // Magenta
                new Vertex(new Vector3(-1, 1, 1), unchecked((int)0xFF00FFFF)),   // Cyan
                new Vertex(new Vector3(1, 1, 1), unchecked((int)0xFFFFFFFF)),    // White
                new Vertex(new Vector3(1, -1, 1), unchecked((int)0xFF888888))    // Gray
            };

            DataStream vertexStream = vertexBuffer.Lock(0, 0, LockFlags.None);
            vertexStream.WriteRange(vertices);
            vertexBuffer.Unlock();

            // Index buffer
            indexBuffer = new IndexBuffer(
                device,
                sizeof(short) * 36,
                Usage.WriteOnly,
                Pool.Default,
                false
            );

            var indices = new short[]
            {
                0, 1, 2, 0, 2, 3,
                4, 6, 5, 4, 7, 6,
                4, 5, 1, 4, 1, 0,
                3, 2, 6, 3, 6, 7,
                1, 5, 6, 1, 6, 2,
                4, 0, 3, 4, 3, 7
            };

            DataStream indexStream = indexBuffer.Lock(0, 0, LockFlags.None);
            indexStream.WriteRange(indices);
            indexBuffer.Unlock();
        }

private void LoadShaderEffect()
{
    // Locate the shader file
    var shaderPath = Path.Combine(Environment.CurrentDirectory, "CubeShader.fx");

    // Debugging: Print the shader path
    Console.WriteLine("Looking for shader file at: " + shaderPath);

    if (!System.IO.File.Exists(shaderPath))
        throw new Exception("Shader file not found: " + shaderPath);

    // Load HLSL effect file
    shaderEffect = Effect.FromFile(device, shaderPath, ShaderFlags.None);
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
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new RawColorBGRA(0, 128, 128, 255), 1.0f, 0);
            device.BeginScene();

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
            device.EndScene();
            device.Present();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            cube?.Dispose();
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            shaderEffect?.Dispose();
            device?.Dispose();
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