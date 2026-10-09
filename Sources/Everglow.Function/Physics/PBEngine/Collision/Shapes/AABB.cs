using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.Collision
{
    public struct AABB
    {
        public Vector2 MinPoint;
        public Vector2 MaxPoint;

        public AABB()
        {
            MinPoint = new Vector2(float.PositiveInfinity);
            MaxPoint = new Vector2(float.NegativeInfinity);
        }

        public AABB(Vector2 minP, Vector2 maxP)
        {
            MinPoint = minP;
            MaxPoint = maxP;
        }

        public Vector2 Center
        {
            get => (MaxPoint + MinPoint) / 2;
        }

        public bool Intersects(in AABB other)
        {
            return Math.Max(MinPoint.X, other.MinPoint.X) <= Math.Min(MaxPoint.X, other.MaxPoint.X)
                && Math.Max(MinPoint.Y, other.MinPoint.Y) <= Math.Min(MaxPoint.Y, other.MaxPoint.Y);
        }

        public AABB Move(Vector2 v)
        {
            return new AABB() { MinPoint = MinPoint + v, MaxPoint = MaxPoint + v };
        }

        public AABB Merge(in AABB other)
        {
            return new AABB() { MinPoint = Vector2.Min(MinPoint, other.MinPoint), MaxPoint = Vector2.Max(MaxPoint, other.MaxPoint) };
        }

        public void MergeWith(in AABB other)
        {
            MinPoint = Vector2.Min(MinPoint, other.MinPoint);
            MaxPoint = Vector2.Max(MaxPoint, other.MaxPoint);
        }

        public double GetArea()
        {
            double w = MaxPoint.X - MinPoint.X;
            double h = MaxPoint.Y - MinPoint.Y;
            return w * h;
        }

        public bool CompletelyInside(in AABB other)
        {
            return other.MinPoint.X <= MinPoint.X && other.MaxPoint.X >= MaxPoint.X
                && other.MinPoint.Y <= MinPoint.Y && other.MaxPoint.Y >= MaxPoint.Y;
        }

        public AABB EnLarge(float size)
        {
            return new AABB() { MinPoint = MinPoint - new Vector2(size), MaxPoint = MaxPoint + new Vector2(size) };
        }
    }
}
