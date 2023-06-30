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

        public bool Intersects(in AABB other)
        {
            return Math.Max(MinPoint.X, other.MinPoint.X) <= Math.Min(MaxPoint.X, other.MaxPoint.X)
                && Math.Max(MinPoint.Y, other.MinPoint.Y) <= Math.Min(MaxPoint.Y, other.MaxPoint.Y);
        }
    }
}
