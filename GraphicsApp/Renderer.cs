using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using System;

public class Renderer : IDisposable
{
    private SharpDX.Direct3D9.Device device;

    public Renderer(SharpDX.Direct3D9.Device device)
    {
        this.device = device;
    }

    public void InitializeRenderTarget(int width, int height)
    {
        // Set the viewport
        var viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
        device.Viewport = viewport;
    }

    public void Resize(int width, int height)
    {
        // Update the viewport with the new size
        var viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
        device.Viewport = viewport;
    }

    public void ClearScreen(RawColorBGRA color)
    {
        // Clear the render target and depth buffer
        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, color, 1.0f, 0);
    }

    public void Present()
    {
        // Present the back buffer to the screen
        device.Present();
    }

    public void Dispose()
    {
        // Release resources
        device?.Dispose();
    }
}
