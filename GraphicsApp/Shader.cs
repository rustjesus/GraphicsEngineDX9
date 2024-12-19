using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.IO;

public class Shader : IDisposable
{
    private SharpDX.Direct3D9.Device device;
    public VertexShader VertexShader { get; private set; }
    public PixelShader PixelShader { get; private set; }
    private string shaderPath;

    public Shader(SharpDX.Direct3D9.Device device, string shaderPath)
    {
        this.device = device;

        // Check if the shader file exists
        if (!File.Exists(shaderPath))
        {
            throw new IOException($"Shader file not found at: {shaderPath}");
        }

        // Compile vertex shader
        using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile(shaderPath, "VS", "vs_2_0"))
        {
            VertexShader = new VertexShader(device, vertexShaderByteCode);
        }

        // Compile pixel shader
        using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile(shaderPath, "PS", "ps_2_0"))
        {
            PixelShader = new PixelShader(device, pixelShaderByteCode);
        }

        this.shaderPath = shaderPath;
    }

    public void SetShaders()
    {
        device.VertexShader = VertexShader;
        device.PixelShader = PixelShader;
    }

    public void Dispose()
    {
        VertexShader?.Dispose();
        PixelShader?.Dispose();
    }
}
