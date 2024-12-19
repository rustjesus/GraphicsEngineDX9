using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.DXGI;
using System;

namespace GraphicsApp
{
    public struct VertexPositionColor
    {
        public Vector3 Position;
        public Color4 Color;

        public VertexPositionColor(Vector3 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }

    public class LightGizmo
    {
    }
}
