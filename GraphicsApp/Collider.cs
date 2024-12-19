using SharpDX;
using System;
using System.Collections.Generic;

namespace GraphicsApp
{
    public class Collider
    {
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        public Collider(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        // Check for AABB collision with another collider
        public bool Intersects(Collider other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }
    }

    public class CubeWithCollider : Cube
    {
        public Collider Collider { get; private set; }

        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; private set; }
        public CubeWithCollider(SharpDX.Direct3D9.Device device, Vector3 position, float size, CollisionManager collisionManager)
            : base(device)
        {
            Position = position;
            Vector3 halfSize = new Vector3(size / 2);
            Vector3 min = position - halfSize;
            Vector3 max = position + halfSize;

            Collider = new Collider(min, max);

            // Add the cube to the collision manager
            collisionManager.AddCube(this);
        }

        public void UpdateColliderPosition(Vector3 position, float size)
        {
            Position = position;
            Vector3 halfSize = new Vector3(size / 2);
            Collider = new Collider(position - halfSize, position + halfSize);
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            Position = newPosition;
            // Additional logic for collider update if needed
        }
        public void Update(float deltaTime)
        {
            if (!isDestroyed)
            {
                Position += Velocity * deltaTime;
            }
        }
        public new void Destroy()
        {
            if (!isDestroyed)
            {
                base.Destroy(); // Call base destroy logic

                Console.WriteLine($"Cube at {Collider.Min} - {Collider.Max} is destroyed permanently.");

            }
        }
    }


    

}
