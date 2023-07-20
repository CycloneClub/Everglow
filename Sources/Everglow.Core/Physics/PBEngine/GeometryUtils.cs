using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine
{
	public static class GeometryUtils
    {

        public static AABB ToAABBPhysSpace(this Rectangle rect)
        {
            return new AABB()
            {
                MinPoint = new Vector2(rect.X, -(rect.Y + rect.Height)),
                MaxPoint = new Vector2(rect.X + rect.Width, -rect.Y)
            };
        }

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

        public static float PointSignedDistanceToSegmentGetNearest(Vector2 p, Vector2 start, Vector2 end, out Vector2 pointOnSeg)
        {
            var u = (end - start).SafeNormalize(Vector2.Zero);
            float d = Vector2.Dot(u, p - start);
            float sign = GeometryUtils.Cross(u, p - start) > 0 ? -1 : 1;
            if (d <= 0)
            {
                pointOnSeg = start;
                return Vector2.Distance(p, start) * sign;
            }
            else if (d >= Vector2.Distance(start, end))
            {
                pointOnSeg = end;
                return Vector2.Distance(p, end) * sign;
            }
            pointOnSeg = start + d * u;
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

        public static float PointDistance2ToSegmentGetNearest(Vector2 p, Vector2 start, Vector2 end, out Vector2 pointOnSeg)
        {
            var u = (end - start).SafeNormalize(Vector2.Zero);
            float d = Vector2.Dot(u, p - start);
            if (d <= 0)
            {
                pointOnSeg = start;
                return Vector2.DistanceSquared(p, start);
            }
            else if (d * d >= Vector2.DistanceSquared(start, end))
            {
                pointOnSeg = end;
                return Vector2.DistanceSquared(p, end);
            }
            pointOnSeg = start + d * u;
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

        public static void SphereConvexPolygonContactInfo(Vector2 cSphere, float radius, List<Edge2D> edges,
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

        public static bool SphereConvexPolygonCollisionInfo(Vector2 cSphere, float radius, List<Edge2D> edges, List<Vector2> corners,
            out float depth, out Vector2 normal)
        {
            float maxDepth = float.NegativeInfinity;
            normal = Vector2.Zero;
            bool anyOutside = false;
            foreach (var edge in edges)
            {
                float d = GeometryUtils.PointDistance2ToSegmentGetNearest(cSphere, edge._pA, edge._pB, out Vector2 pointOnSeg);
                // 如果点在某一个线段的外面，那么就说明点在多边形的外面
                if (d > maxDepth && GeometryUtils.Cross(edge._pB - edge._pA, cSphere - edge._pA) <= 0)
                {
                    maxDepth = d;
                    normal = (cSphere - pointOnSeg).SafeNormalize(Vector2.Zero);
                }
                
                //if (GeometryUtils.Cross(cSphere - edge._pA, edge._pB - edge._pA) <= 0)
                //{
                //    anyOutside = true;
                //}
                //if (d > maxDepth)
                //{
                //    maxDepth = d;
                //    keyEdge = edge;
                //}
            }
            if (maxDepth < 0)
            {
                depth = 0;
                return false;
            }
            maxDepth = (float)Math.Sqrt(maxDepth);
            depth = radius - maxDepth;
            if (maxDepth >= radius)
            {
                return false;
            }
            return true;
        }

        //public static bool SphereConvexPolygonIsCollide(Vector2 cSphere, float radius, List<Edge2D> edges, Vector2 centerPoly)
        //{
        //    float maxDepth = float.NegativeInfinity;
        //    foreach (var edge in edges)
        //    {
        //        float d = GeometryUtils.PointSignedDistanceToSegment(cSphere, edge._pA, edge._pB);
        //        if (d > maxDepth)
        //        {
        //            maxDepth = d;
        //        }
        //    }
        //    return maxDepth < radius;
        //}

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

        public static bool ConvexPolygonPolygonCollisionInfo(List<Edge2D> edgesA, List<Vector2> cornersA,
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

                float d = MinimumSeparatingDistance(amin, amax, bmin, bmax);
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

                float d = MinimumSeparatingDistance(amin, amax, bmin, bmax);
                if (d < depth)
                {
                    depth = d;
                    normal = axis;
                }
            }
            return true;
        }

        public static void ConvexPolygonPolygonContactInfo(List<Edge2D> edgesA, List<Vector2> cornersA,
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
                    else if (Math.Abs(dist - closestDistance) < 1e-3)
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

        public static bool SphereCapsuleCollisionInfo(Vector2 center, float radius, Vector2 segA, Vector2 segB, 
            float radius2, out float depth, out Vector2 normal)
        {
            float d = PointDistance2ToSegmentGetNearest(center, segA, segB, out Vector2 p);
            depth = radius + radius2 - (float)Math.Sqrt(d);
            normal = Vector2.Zero;
            if (d >= (radius + radius2) * (radius + radius2))
            {
                return false;
            }
            normal = (p - center).SafeNormalize(Vector2.Zero);
            return true;
        }

        public static void SphereCapsuleContactInfo(Vector2 center, float radius, Vector2 segA, Vector2 segB,
            float radius2, List<KeyValuePair<Vector2, Vector2>> localPositions)
        {
            var u = (segB - segA).SafeNormalize(Vector2.Zero);
            float d_plane = Vector2.Dot(u, center - segA);
            float length = (segA - segB).Length();
            Vector2 capsuleCenter = (segA + segB) / 2;
            d_plane = MathHelper.Clamp(d_plane, 0, length);

            Vector2 vcenter = segA + u * d_plane;
            Vector2 unit = (center - vcenter).SafeNormalize(Vector2.Zero);
            localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                  -unit * radius,
                  vcenter + unit * radius2 - capsuleCenter
            ));
        }

        public static bool CapsuleCapsuleCollisionInfo(Vector2 segA1, Vector2 segB1,
            float radius1, Vector2 segA2, Vector2 segB2,
            float radius2, out float depth, out Vector2 normal)
        {
            Vector2 pointA = segA2;
            bool flipSign = false;
            float d = PointDistance2ToSegmentGetNearest(segA2, segA1, segB1, out Vector2 pointB);
            float d2;
            if ((d2 = PointDistance2ToSegmentGetNearest(segB2, segA1, segB1, out Vector2 p)) < d)
            {
                d = d2;
                pointA = segB2;
                pointB = p;
            }

            if ((d2 = PointDistance2ToSegmentGetNearest(segA1, segA2, segB2, out p)) < d)
            {
                d = d2;
                pointA = segA1;
                pointB = p;
                flipSign = true;
            }

            if ((d2 = PointDistance2ToSegmentGetNearest(segB1, segA2, segB2, out p)) < d)
            {
                d = d2;
                pointA = segB1;
                pointB = p;
                flipSign = true;
            }


            depth = radius1 + radius2 - (float)Math.Sqrt(d);
            normal = (pointA - pointB).SafeNormalize(Vector2.UnitX);
            // 法线方向必须朝向Sphere碰撞体
            if (flipSign)
            {
                normal = -normal;   
            }
            if (depth <= 0)
                return false;
            return true;
        }

        public static void CapsuleCapsuleContactInfo(Vector2 segA1, Vector2 segB1,
           float radius1, Vector2 segA2, Vector2 segB2,
           float radius2, List<KeyValuePair<Vector2, Vector2>> localPositions)
        {
            Vector2 pointA = segA2;
            Vector2 capsuleCenter1 = (segA1 + segB1) / 2;
            Vector2 capsuleCenter2 = (segA2 + segB2) / 2;
            float d = PointDistance2ToSegmentGetNearest(segA2, segA1, segB1, out Vector2 pointB);
            Vector2 N = (pointA - pointB).SafeNormalize(Vector2.UnitX);
            localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                pointB + N * radius1 - capsuleCenter1,
                pointA - N * radius2 - capsuleCenter2
                ));
            float d2;
            if ((d2 = PointDistance2ToSegmentGetNearest(segB2, segA1, segB1, out Vector2 p)) < d)
            {
                d = d2;
                pointA = segB2;
                pointB = p;
                localPositions.Clear();
                N = (pointA - pointB).SafeNormalize(Vector2.UnitX);
                localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                pointB + N * radius1 - capsuleCenter1,
                pointA - N * radius2 - capsuleCenter2
                ));
            }
            else if (d2 == d)
            {
                pointA = segB2;
                pointB = p;
                N = (pointA - pointB).SafeNormalize(Vector2.UnitX);
                localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                    pointB + N * radius1 - capsuleCenter1,
                    pointA - N * radius2 - capsuleCenter2
               ));
            }

            // 防止四个点全都被取，最多只能保留两个接触点
            if ((d2 = PointDistance2ToSegmentGetNearest(segA1, segA2, segB2, out p)) <= d)
            {
                d = d2;
                pointA = segA1;
                pointB = p;
                N = (pointA - pointB).SafeNormalize(Vector2.UnitX);
                localPositions.Clear();
                localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                    pointA - N * radius1 - capsuleCenter1,
                    pointB + N * radius2 - capsuleCenter2
                ));
            }

            if ((d2 = PointDistance2ToSegmentGetNearest(segB1, segA2, segB2, out p)) < d)
            {
                d = d2;
                pointA = segB1;
                pointB = p;
                N = (pointA - pointB).SafeNormalize(Vector2.UnitX);
                localPositions.Clear();
                localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                    pointA - N * radius1 - capsuleCenter1,
                    pointB + N * radius2 - capsuleCenter2
                ));
            }
            else if (d2 == d)
            {
                pointA = segB1;
                pointB = p;
                N = (pointA - pointB).SafeNormalize(Vector2.UnitX);
                localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                    pointA - N * radius1 - capsuleCenter1,
                    pointB + N * radius2 - capsuleCenter2
               ));
            }

            if (localPositions.Count > 2)
            {
                // Remove all elements after the first two
                localPositions.RemoveRange(2, localPositions.Count - 2);
            }
        }


        public static bool CapsuleConvexPolygonCollisionInfo(Vector2 segA1, Vector2 segB1,
            float radius1, List<Vector2> corners, List<Edge2D> edges, out float depth, out Vector2 normal)
        {
            depth = float.PositiveInfinity;
            normal = Vector2.Zero;

            {
                // 中轴线段的法线和中轴线段本身都是关键分离轴
                Edge2D edge = new Edge2D(segA1, segB1);
                Vector2 axis = edge.GetNormal();
                var (amin, amax) = GetProjectedInterval(new List<Vector2>() { segA1, segB1 }, edge._pA, axis);
                var (bmin, bmax) = GetProjectedInterval(corners, edge._pA, axis);

                amin -= radius1;
                amax += radius1;

                if (Math.Max(amin, bmin) > Math.Min(amax, bmax))
                {
                    return false;
                }

                float d = MinimumSeparatingDistance(amin, amax, bmin, bmax);
                if (d < depth)
                {
                    depth = d;
                    normal = axis;
                }

                axis = (segB1 - segA1).SafeNormalize(Vector2.Zero);
                (amin, amax) = GetProjectedInterval(new List<Vector2>() { segA1, segB1 }, edge._pA, axis);
                (bmin, bmax) = GetProjectedInterval(corners, edge._pA, axis);

                amin -= radius1;
                amax += radius1;

                if (Math.Max(amin, bmin) > Math.Min(amax, bmax))
                {
                    return false;
                }

                d = MinimumSeparatingDistance(amin, amax, bmin, bmax);
                if (d < depth)
                {
                    depth = d;
                    normal = axis;
                }
            }

            var capsuleCorners = new List<Vector2>() { segA1, segB1 };
            for (int i = 0; i < edges.Count; i++)
            {
                Edge2D edge = edges[i];
                Vector2 axis = edge.GetNormal();

                var (amin, amax) = GetProjectedInterval(capsuleCorners, edge._pA, axis);
                var (bmin, bmax) = GetProjectedInterval(corners, edge._pA, axis);

                amin -= radius1;
                amax += radius1;

                if (Math.Max(amin, bmin) > Math.Min(amax, bmax))
                {
                    return false;
                }

                float d = MinimumSeparatingDistance(amin, amax, bmin, bmax);
                if (d < depth)
                {
                    depth = d;
                    normal = axis;
                }
            }
            return true;
        }

        public static void CapsuleConvexPolygonContactInfo(Vector2 segA1, Vector2 segB1,
            float radius1, List<Vector2> corners, List<Edge2D> edges, Vector2 polyCenter, List<KeyValuePair<Vector2, Vector2>> localPositions)
        {
            float closestDistance = float.PositiveInfinity;
            Edge2D edgeSeg = new Edge2D(segA1, segB1);
            List<Vector2> capsuleVertices = new List<Vector2>() { segA1, segB1 };
            Vector2 capsuleCenter = (segA1 + segB1) / 2;
            Vector2 ultimateDir = (polyCenter - capsuleCenter).SafeNormalize(Vector2.Zero);
            foreach (var curPoint in corners)
            {
                float dist = GeometryUtils.PointDistance2ToSegmentGetNearest(curPoint, edgeSeg._pA, edgeSeg._pB, out Vector2 p);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    localPositions.Clear();
                    localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                        p + (curPoint - p).SafeNormalize(ultimateDir) * radius1 - capsuleCenter,
                        curPoint - polyCenter
                    ));
                }
                else if (Math.Abs(dist - closestDistance) < 1e-6)
                {
                    localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                        p + (curPoint - p).SafeNormalize(ultimateDir) * radius1 - capsuleCenter,
                        curPoint - polyCenter
                    ));
                }

            }
            foreach (var capsulePoint in capsuleVertices)
            {
                foreach (var curEdge in edges)
                {
                    float dist = GeometryUtils.PointDistance2ToSegmentGetNearest(capsulePoint, curEdge._pA, curEdge._pB, out Vector2 p);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        localPositions.Clear();
                        localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                            capsulePoint + (p - capsulePoint).SafeNormalize(ultimateDir) * radius1 - capsuleCenter,
                            p - polyCenter
                        ));
                    }
                    else if (Math.Abs(dist - closestDistance) < 1e-6)
                    {
                        localPositions.Add(new KeyValuePair<Vector2, Vector2>(
                             capsulePoint + (p - capsulePoint).SafeNormalize(ultimateDir) * radius1 - capsuleCenter,
                            p - polyCenter
                        ));
                    }
                }
            }

            if (localPositions.Count > 2)
            {
                // Remove all elements after the first two
                localPositions.RemoveRange(2, localPositions.Count - 2);
            }
        }


        public static float MinimumSeparatingDistance(float amin, float amax, float bmin, float bmax)
        {
            float d = Math.Min(amax, bmax) - Math.Max(amin, bmin);
            if ((amin >= bmin && amax <= bmax) || (amin <= bmin && amax >= bmax))
            {
                d = Math.Min(amax - bmin, bmax - amin);
            }
            return d;
        }

        public static Vector2 ConvertToPhysicsSpace(Vector2 pos)
        {
            return new Vector2(pos.X, -pos.Y);
        }

        public static void FisherYatesShuffle<T>(this IList<T> list, Random rng)
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
