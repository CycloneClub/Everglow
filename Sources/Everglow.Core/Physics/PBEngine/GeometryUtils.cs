using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine
{
	public static class GeometryUtils
    {
        public static bool ApproxEqual(float a, float b)
        {
            return Math.Abs(a - b) < 1e-6;
        }
        /// <summary>
        /// Compute ab^T
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix3x3 OuterProduct(Vector3 a, Vector3 b)
        {
            return new Matrix3x3
            {
                [0, 0] = a.X * b.X,
                [0, 1] = a.X * b.Y,
                [0, 2] = a.X * b.Z,
                [1, 0] = a.Y * b.X,
                [1, 1] = a.Y * b.Y,
                [1, 2] = a.Y * b.Z,
                [2, 0] = a.Z * b.X,
                [2, 1] = a.Z * b.Y,
                [2, 2] = a.Z * b.Z
            };
        }

        /// <summary>
        /// Compute ab^T
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix2x2 OuterProduct(Vector2 a, Vector2 b)
        {
            return new Matrix2x2
            {
                [0, 0] = a.X * b.X,
                [0, 1] = a.X * b.Y,
                [1, 0] = a.Y * b.X,
                [1, 1] = a.Y * b.Y,
            };
        }


        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }


        public static void Blit(Texture2D source, RenderTarget2D dest, Effect effect, string pass)
        {
            var sb = Main.spriteBatch;
            var gd = Main.graphics.GraphicsDevice;
            gd.SetRenderTarget(dest);
            gd.Clear(Color.Transparent);
            if (effect != null)
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                effect.CurrentTechnique.Passes[pass].Apply();
                sb.Draw(source, dest.Bounds, Color.White);
                sb.End();
            }
            else
            {
                sb.Begin();
                sb.Draw(source, dest.Bounds, Color.White);
                sb.End();
            }

        }

        /// <summary>
        /// The linear velocity corresponding to given angular velocity and arm
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static Vector2 AnuglarVelocityToLinearVelocity(Vector2 a, float w)
        {
            return new Vector2(-a.Y, a.X) * w;
        }

        public static Vector2 Rotate90(Vector2 a)
        {
            return new Vector2(-a.Y, a.X);
        }

        public static Vector2 ProjectPointOnSegment(Vector2 p, Vector2 start, Vector2 end)
        {
            var u = (end - start).SafeNormalize(Vector2.Zero);
            return start + Vector2.Dot(u, p - start) * u;
        }

        public static float PointSignedDistanceToLine(Vector2 p, Vector2 start, Vector2 end)
        {
            Vector2 ab = end - start;
            Vector2 ax = p - start;

            float crossProductMagnitude = GeometryUtils.Cross(ab, ax);
            float ABLength = ab.Length();

            return -crossProductMagnitude / ABLength;
        }

        public static float PointSignedDistanceToSegment(Vector2 p, Vector2 start, Vector2 end)
        {
            var u = (end - start).SafeNormalize(Vector2.Zero);
            float d = Vector2.Dot(u, p - start);
            float sign = GeometryUtils.Cross(u, p - start) > 0 ? -1 : 1;
            if (d <= 0)
            {
                return Vector2.Distance(p, start) * sign;
            }
            else if (d >= Vector2.Distance(start, end))
            {
                return Vector2.Distance(p, end) * sign;
            }
            return (p - (start + d * u)).Length() * sign;
        }

        public static float PointDistance2ToSegment(Vector2 p, Vector2 start, Vector2 end)
        {
            var u = (end - start).SafeNormalize(Vector2.Zero);
            float d = Vector2.Dot(u, p - start);
            if (d <= 0)
            {
                return Vector2.DistanceSquared(p, start);
            }
            else if (d * d >= Vector2.DistanceSquared(start, end))
            {
                return Vector2.DistanceSquared(p, end);
            }
            return (p - (start + d * u)).LengthSquared();
        }
        public static float PointDistance2ToSegmentWithClip(Vector2 p, Vector2 start, Vector2 end)
        {
            var u = (end - start).SafeNormalize(Vector2.Zero);
            float d = Vector2.Dot(u, p - start);
            if (d <= 0)
            {
                return float.PositiveInfinity;
            }
            else if (d * d >= Vector2.DistanceSquared(start, end))
            {
                return float.PositiveInfinity;
            }
            return (p - (start + d * u)).LengthSquared();
        }

        public static void CirclePolygonContactInfo(Vector2 cSphere, float radius, List<Edge2D> edges,
            List<Vector2> corners, Vector2 centerPoly,
            List<KeyValuePair<Vector2, Vector2>> localPositions)
        {
            float minDepth = float.PositiveInfinity;
            foreach (var edge in edges)
            {
                float d = GeometryUtils.PointDistance2ToSegmentWithClip(cSphere, edge._pA, edge._pB);
                if (d < minDepth)
                {
                    minDepth = d;
                }
            }
            foreach (var v in corners)
            {
                float d = Vector2.DistanceSquared(v, cSphere);
                if (d < minDepth)
                {
                    minDepth = d;
                }
            }

            foreach (var edge in edges)
            {
                float d = GeometryUtils.PointDistance2ToSegmentWithClip(cSphere, edge._pA, edge._pB);
                if (d == minDepth)
                {
                    var p = GeometryUtils.ProjectPointOnSegment(cSphere, edge._pA, edge._pB);
                    localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                         (p - cSphere).SafeNormalize(Vector2.Zero) * radius,
                         p - centerPoly
                    ));
                }
            }
            foreach (var v in corners)
            {
                float d = Vector2.DistanceSquared(v, cSphere);
                if (d == minDepth)
                {
                    localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                         (v - cSphere).SafeNormalize(Vector2.Zero) * radius,
                         v - centerPoly
                    ));
                }
            }
        }

        public static bool CirclePolygonCollisionInfo(Vector2 cSphere, float radius, List<Edge2D> edges, List<Vector2> corners,
            out float depth, out Vector2 normal)
        {
            float maxDepth = float.NegativeInfinity;
            float maxDepth2 = float.NegativeInfinity;
            Edge2D keyEdge = null;
            foreach (var edge in edges)
            {
                float d = GeometryUtils.PointSignedDistanceToLine(cSphere, edge._pA, edge._pB);
                float d2 = GeometryUtils.PointSignedDistanceToSegment(cSphere, edge._pA, edge._pB);
                if (d2 > maxDepth2)
                {
                    maxDepth2 = d2;
                }
                if (d > maxDepth)
                {
                    maxDepth = d;
                    keyEdge = edge;
                }
            }

            normal = keyEdge.GetNormal();
            depth = radius - maxDepth;
            if (maxDepth2 > radius)
            {
                return false;
            }

            foreach (var corner in corners)
            {
                float d = Vector2.Distance(corner, cSphere);
                if (d <= radius && d > maxDepth)
                {
                    maxDepth = d;
                    normal = Vector2.Normalize(cSphere - corner);
                    depth = radius - maxDepth;
                }
            }
            return maxDepth < radius;
        }

        public static bool CirclePolygonIsCollide(Vector2 cSphere, float radius, List<Edge2D> edges, Vector2 centerPoly)
        {
            float maxDepth = float.NegativeInfinity;
            foreach (var edge in edges)
            {
                float d = GeometryUtils.PointSignedDistanceToSegment(cSphere, edge._pA, edge._pB);
                if (d > maxDepth)
                {
                    maxDepth = d;
                }
            }
            return maxDepth < radius;
        }

        private static (float, float) GetProjectedInterval(List<Vector2> vertices, Vector2 start, Vector2 dir)
        {
            float v = Vector2.Dot(dir, vertices[0] - start);
            float minA = v, maxA = v;
            for (int i = 1; i < vertices.Count; i++)
            {
                float x = Vector2.Dot(dir, vertices[i] - start);
                minA = Math.Min(minA, x);
                maxA = Math.Max(maxA, x);
            }
            return (minA, maxA);
        }

        public static bool PolygonPolygonCollisionInfo(List<Edge2D> edgesA, List<Vector2> cornersA,
            List<Edge2D> edgesB, List<Vector2> cornersB, out float depth, out Vector2 normal)
        {
            depth = float.PositiveInfinity;
            normal = Vector2.Zero;
            for (int i = 0; i < edgesA.Count; i++)
            {
                Edge2D edge = edgesA[i];
                Vector2 axis = edge.GetNormal();

                var (amin, amax) = GetProjectedInterval(cornersA, edge._pA, axis);
                var (bmin, bmax) = GetProjectedInterval(cornersB, edge._pA, axis);

                if (Math.Max(amin, bmin) > Math.Min(amax, bmax))
                {
                    return false;
                }

                float d = Math.Min(amax, bmax) - Math.Max(amin, bmin);
                if (d < depth)
                {
                    depth = d;
                    normal = axis;
                }
            }
            for (int i = 0; i < edgesB.Count; i++)
            {
                Edge2D edge = edgesB[i];
                Vector2 axis = edge.GetNormal();

                var (amin, amax) = GetProjectedInterval(cornersA, edge._pA, axis);
                var (bmin, bmax) = GetProjectedInterval(cornersB, edge._pA, axis);

                if (Math.Max(amin, bmin) > Math.Min(amax, bmax))
                {
                    return false;
                }

                float d = Math.Min(amax, bmax) - Math.Max(amin, bmin);
                if (d < depth)
                {
                    depth = d;
                    normal = axis;
                }
            }
            return true;
        }

        public static void PolygonPolygonContactInfo(List<Edge2D> edgesA, List<Vector2> cornersA,
            List<Edge2D> edgesB, List<Vector2> cornersB,
            Vector2 polyCenterA, Vector2 polyCenterB, List<KeyValuePair<Vector2, Vector2>> localPositions)
        {
            float closestDistance = float.PositiveInfinity;
            foreach (var curPoint in cornersA)
            {
                foreach (var curEdge in edgesB)
                {
                    float dist = GeometryUtils.PointDistance2ToSegment(curPoint, curEdge._pA, curEdge._pB);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        localPositions.Clear();
                        localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                            curPoint - polyCenterA,
                            GeometryUtils.ProjectPointOnSegment(curPoint, curEdge._pA, curEdge._pB)
                                                   - polyCenterB
                        ));
                    }
                    else if (Math.Abs(dist - closestDistance) < 1e-6)
                    {
                        localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                            curPoint - polyCenterA,
                            GeometryUtils.ProjectPointOnSegment(curPoint, curEdge._pA, curEdge._pB)
                                                   - polyCenterB
                        ));
                    }
                }
            }
            foreach (var curPoint in cornersB)
            {
                foreach (var curEdge in edgesA)
                {
                    float dist = GeometryUtils.PointDistance2ToSegment(curPoint, curEdge._pA, curEdge._pB);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        localPositions.Clear();
                        localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                            GeometryUtils.ProjectPointOnSegment(curPoint, curEdge._pA, curEdge._pB) - polyCenterA,
                            curPoint - polyCenterB
                        ));
                    }
                    else if (Math.Abs(dist - closestDistance) < 1e-6)
                    {
                        localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                            GeometryUtils.ProjectPointOnSegment(curPoint, curEdge._pA, curEdge._pB) - polyCenterA,
                            curPoint - polyCenterB
                        ));
                    }
                }
            }
        }

        public static Vector2 ConvertToPhysicsSpace(Vector2 pos)
        {
            return new Vector2(pos.X, -pos.Y);
        }

        private static Random rng = new Random();

        public static void FisherYatesShuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
