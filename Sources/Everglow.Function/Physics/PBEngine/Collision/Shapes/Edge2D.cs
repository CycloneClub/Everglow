using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Physics.PBEngine.Core;
using Everglow.Commons.Utilities;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Collision.Shapes
{
	public class Edge2D
    {
        public Vector2 _pA, _pB;
        public Edge2D()
        {

        }

        public Edge2D(Vector2 A, Vector2 B)
        {
            _pA = A;
            _pB = B;
        }

        public Vector2 GetNormal()
        {
            return -GeometryUtils.Rotate90((_pB - _pA).SafeNormalize(Vector2.Zero));
        }

        public bool Segment_Segment_Collision(Vector2 a, Vector2 b, out CollisionEvent2D collisionEvent)
        {
            var M = new Matrix2x2
            {
                [0, 0] = _pB.X - _pA.X,
                [0, 1] = -(b.X - a.X),
                [1, 0] = _pB.Y - _pA.Y,
                [1, 1] = -(b.Y - a.Y),
            };

            double det = M.Determinant();
            if (Math.Abs(det) < 1e-7)
            {
                collisionEvent = new CollisionEvent2D();
                return false; // parallel lines
            }
            else
            {

                Vector2 st = M.Adjoint().Multiply(a - _pA) / (float)det;
                if (st.X >= 0 && st.X <= 1.0f && st.Y >= 0 && st.Y <= 1.0f)
                {
                    collisionEvent = new CollisionEvent2D
                    {
                        Time = st.Y,
                        Position = _pA + (_pB - _pA) * st.X,
                        Normal = -GeometryUtils.Rotate90((_pB - _pA).SafeNormalize(Vector2.Zero)),
                        Offset = Vector2.Zero
                    };
                    return true;
                }
                collisionEvent = new CollisionEvent2D();
                return false; // intersection is outside of the line segments
            }
        }
    }
}
