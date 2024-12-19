using GraphicsApp;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;

internal class Resolution
{
    public int Width { get; }
    public int Height { get; }

    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public void ApplyRes(Form1 form, Renderer renderer, Camera camera, Device device)
    {
        form.ClientSize = new System.Drawing.Size(Width, Height);

        // Resize the renderer
        renderer.Resize(Width, Height);

        // Update the camera projection matrix
        float aspectRatio = Width / (float)Height;
        camera.UpdateProjection(aspectRatio);

        // Set the viewport
        var viewport = new Viewport(0, 0, Width, Height, 0.0f, 1.0f);
        device.Viewport = viewport; // Use the Viewport property instead of a method

        // Ensure the device is reset if needed
        renderer.InitializeRenderTarget(Width, Height);
    }

    public static void ChangeResolution(Form1 form, int resolutionIndex, List<Resolution> availableResolutions, Renderer renderer, Camera camera, Device device)
    {
        if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Count)
            return;

        var newResolution = availableResolutions[resolutionIndex];
        newResolution.ApplyRes(form, renderer, camera, device);

        // Log resolution change for debugging
        System.Diagnostics.Debug.WriteLine($"Resolution changed to: {newResolution.Width}x{newResolution.Height}");

        // Ensure the form regains focus
        form.Focus();
        form.Activate();
    }
}
