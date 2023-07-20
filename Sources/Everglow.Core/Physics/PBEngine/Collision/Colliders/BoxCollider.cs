using ChatGPT.Core.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.GameInteraction;
using Everglow.Commons.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Everglow.Commons.Physics.PBEngine.Collision.Colliders
{
	/// <summary>
	/// 四边形、盒子碰撞体
	/// </summary>
	public class BoxCollider : Collider2D
    {
        public Vector2 Size
        {
            get => new Vector2(_width, _height);
        }
        private float _width;
        private float _height;
        public BoxCollider(float width, float height)
        {
            _width = width;
            _height = height;
        }

        public override double InertiaTensor(float mass)
        {
            return mass / 12.0 * (_width * _width + _height * _height);
        }

        public override List<Vector2> GetWireFrameWires()
        {
            List<Vector2> wires = new List<Vector2>
            {
                new Vector2(-_width / 2, -_height / 2),
                new Vector2(_width / 2, -_height / 2),
                new Vector2(_width / 2, -_height / 2),
                new Vector2(_width / 2, _height / 2),
                new Vector2(_width / 2, _height / 2),
                new Vector2(-_width / 2, _height / 2),
                new Vector2(-_width / 2, _height / 2),
                new Vector2(-_width / 2, -_height / 2)
            };
            return wires;
        }

        public List<Edge2D> GetEdges(float dt)
        {
            List<Vector2> cornerWorldSpace = GetCornerPoints(dt);
            List<Edge2D> edges = new List<Edge2D>();
            for (int i = 0; i < 4; i++)
            {
                edges.Add(new Edge2D(cornerWorldSpace[i], cornerWorldSpace[(i + 1) % cornerWorldSpace.Count]));
            }
            return edges;

        }

        //public override List<CollisionEvent2D> GetCollisionEvents(Collider2D other, float deltaTime)
        //{
        //    List<CollisionEvent2D> events = new List<CollisionEvent2D>();

        //    Vector2[] localPoints = new Vector2[4]
        //    {
        //        new Vector2(-_width / 2, -_height / 2),
        //        new Vector2(_width / 2, -_height / 2),
        //        new Vector2(_width / 2, _height / 2),
        //        new Vector2(-_width / 2, _height / 2)
        //    };
        //    if (other is AABBCollider)
        //    {
        //        AABBCollider aabb_other = (AABBCollider)other;
        //        for (int i = 0; i < 4; i++)
        //        {
        //            var curPos = _bindObject.Position + Matrix2x2.CreateRotationMatrix(_bindObject.Rotation).Multiply(localPoints[i]);
        //            var oldPos = _bindObject.OldPosition + Matrix2x2.CreateRotationMatrix(_bindObject.OldRotation).Multiply(localPoints[i]);

        //            foreach (var edge in aabb_other.GetEdges())
        //            {
        //                CollisionEvent2D collision;
        //                if (edge.Segment_Segment_Collision(oldPos, curPos, out collision))
        //                {
        //                    collision.Source = this.ParentObject;
        //                    collision.Target = other.ParentObject;
        //                    collision.LocalOffsetSrc = Matrix2x2.CreateRotationMatrix(_bindObject.Rotation).Multiply(localPoints[i]);

        //                    events.Add(collision);
        //                }
        //            }
        //        }
        //    }
        //    if (events.Count > 1)
        //    {
        //        Vector2 averagedLocalPoint = new Vector2(0, 0);
        //        Vector2 averagedNormal = new Vector2(0, 0);
        //        float minTime = 1.0f;
        //        for (int i = 0; i < events.Count; i++)
        //        {
        //            averagedLocalPoint += events[i].LocalOffsetSrc;
        //            averagedNormal += events[i].Normal;
        //            minTime = Math.Min(minTime, events[i].Time);
        //        }
        //        averagedLocalPoint /= events.Count;
        //        averagedNormal  /= events.Count;
        //        CollisionEvent2D collision = new CollisionEvent2D();
        //        collision.Source = this.ParentObject;
        //        collision.Target = other.ParentObject;
        //        collision.LocalOffsetSrc = averagedLocalPoint;
        //        collision.Normal = Vector2.Normalize(averagedNormal);
        //        collision.Time = minTime;

        //        events.Clear();
        //        events.Add(collision);
        //    }
        //    return events;
        //}

        public List<Vector2> GetCornerPoints(float dt)
        {
            List<Vector2> cornerPoints = new List<Vector2>();
            Vector2[] localPoints = new Vector2[4]
            {
                new Vector2(-_width / 2, -_height / 2),
                new Vector2(_width / 2, -_height / 2),
                new Vector2(_width / 2, _height / 2),
                new Vector2(-_width / 2, _height / 2)
            };

            var M = ParentObject.CachedRotationalMatrix;
            for (int i = 0; i < 4; i++)
            {
                var curPos = ParentObject.Position + M.Multiply(localPoints[i]);
                cornerPoints.Add(curPos);
            }
            return cornerPoints;
        }

        public override bool TestCollisionCondition(Collider2D other, float deltaTime, out CollisionInfo info)
        {
            info = new CollisionInfo();
            if (other is BoxCollider)
            {
                BoxCollider b = (BoxCollider)other;
                List<Edge2D> edgesA = GetEdges(deltaTime);
                List<Edge2D> edgesB = b.GetEdges(deltaTime);
                List<Vector2> cornersA = GetCornerPoints(deltaTime);
                List<Vector2> cornersB = b.GetCornerPoints(deltaTime);
                float depth;
                Vector2 normal;
                if (GeometryUtils.ConvexPolygonPolygonCollisionInfo(edgesA, cornersA, edgesB, cornersB, out depth, out normal))
                {
                    info.Source = this.ParentObject;
                    info.Target = b.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = depth;
                    info.Normal = Vector2.Dot(normal, ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace) < 0 ? -normal : normal;
                    return true;
                }
            }
            else if (other is SphereCollider)
            {
                SphereCollider b = (SphereCollider)other;
                float depth;
                Vector2 normal;
                bool collided = GeometryUtils.SphereConvexPolygonCollisionInfo(b.ParentObject.Position, b.Radius, GetEdges(deltaTime),
                    GetCornerPoints(deltaTime),
                    out depth, out normal);
                if (collided)
                {
                    info.Source = this.ParentObject;
                    info.Target = b.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = depth;
                    info.Normal = Vector2.Dot(normal, ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace) < 0 ? -normal : normal;
                }
                return collided;
            }
            else if (other is TileCollider)
            {
                float depth;
                Vector2 normal;
                if (TileCollisionUtils.GetPolygonTileCollisionInfo(GetAABB(deltaTime), ParentObject.RigidBody.CentroidWorldSpace,
                    GetEdges(deltaTime), GetCornerPoints(deltaTime), out depth, out normal))
                {
                    info.Source = this.ParentObject;
                    info.Target = other.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = depth;
                    info.Normal = normal;
                    return true;
                }
            }
            else if(other is CapsuleCollider)
            {
                CapsuleCollider b = (CapsuleCollider)other;
                b.GetSegment(deltaTime, out Vector2 A, out Vector2 B);
                if (GeometryUtils.CapsuleConvexPolygonCollisionInfo(A, B, b.Radius, GetCornerPoints(deltaTime),
                    GetEdges(deltaTime),
                    out float depth, out Vector2 normal))
                {
                    info.Source = this.ParentObject;
                    info.Target = b.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = depth;
                    info.Normal = Vector2.Dot(normal, ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace) < 0 ? -normal : normal;
                    return true;
                }
            }
            return false;
        }

        public override void GetContactInfo(in CollisionInfo info, float deltaTime,
            out List<CollisionEvent2D> collisionEvents)
        {
            collisionEvents = new List<CollisionEvent2D>();
            if (info.Target.Collider is BoxCollider)
            {
                BoxCollider b = (BoxCollider)info.Target.Collider;
                List<Edge2D> edgesA = GetEdges(deltaTime);
                List<Edge2D> edgesB = b.GetEdges(deltaTime);
                List<Vector2> cornersA = GetCornerPoints(deltaTime);
                List<Vector2> cornersB = b.GetCornerPoints(deltaTime);
                var e = new CollisionEvent2D()
                {
                    Time = info.Time,
                    Source = info.Source,
                    Target = info.Target,
                    LocalOffsetSrc = Vector2.Zero,
                    LocalOffsetTarget = Vector2.Zero,
                    Normal = info.Normal,
                    Position = Vector2.Zero,
                    Depth = info.Depth,
                };
                List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
                GeometryUtils.ConvexPolygonPolygonContactInfo(edgesA, cornersA, edgesB, cornersB,
                    ParentObject.RigidBody.CentroidWorldSpace, b.ParentObject.RigidBody.CentroidWorldSpace,
                    contacts);
                foreach (var c in contacts)
                {
                    collisionEvents.Add(new CollisionEvent2D(e)
                    {
                        LocalOffsetSrc = c.Key,
                        LocalOffsetTarget = c.Value,
                    });
                }
            }
            else if (info.Target.Collider is SphereCollider)
            {
                SphereCollider b = (SphereCollider)info.Target.Collider;
                var e = new CollisionEvent2D()
                {
                    Time = 0,
                    Source = info.Source,
                    Target = info.Target,
                    LocalOffsetSrc = Vector2.Zero,
                    LocalOffsetTarget = Vector2.Zero,
                    Normal = info.Normal,
                    Position = Vector2.Zero,
                    Depth = info.Depth,
                };

                List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
                GeometryUtils.SphereConvexPolygonContactInfo(b.ParentObject.RigidBody.CentroidWorldSpace, b.Radius, GetEdges(deltaTime),
                    GetCornerPoints(deltaTime), ParentObject.RigidBody.CentroidWorldSpace, contacts);
                foreach (var c in contacts)
                {
                    collisionEvents.Add(new CollisionEvent2D(e)
                    {
                        LocalOffsetSrc = c.Value,
                        LocalOffsetTarget = c.Key,
                    });
                }

            }
            else if (info.Target.Collider is CapsuleCollider)
            {
                CapsuleCollider b = (CapsuleCollider)info.Target.Collider;
                var e = new CollisionEvent2D()
                {
                    Time = info.Time,
                    Source = this.ParentObject,
                    Target = b.ParentObject,
                    LocalOffsetSrc = Vector2.Zero,
                    LocalOffsetTarget = Vector2.Zero,
                    Normal = info.Normal,
                    Position = Vector2.Zero,
                    Depth = info.Depth,
                };
                b.GetSegment(deltaTime, out Vector2 A, out Vector2 B);
                List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
                GeometryUtils.CapsuleConvexPolygonContactInfo(A, B, b.Radius,
                    GetCornerPoints(deltaTime), GetEdges(deltaTime), ParentObject.RigidBody.CentroidWorldSpace, contacts);
                foreach (var c in contacts)
                {
                    collisionEvents.Add(new CollisionEvent2D(e)
                    {
                        LocalOffsetSrc = c.Value,
                        LocalOffsetTarget = c.Key                    
                    });
                }
            }
            else if (info.Target.Collider is TileCollider)
            {
                TileCollisionUtils.GetPolygonTileContactInfo(GetAABB(deltaTime), ParentObject.RigidBody.CentroidWorldSpace,
                    GetEdges(deltaTime), GetCornerPoints(deltaTime), ParentObject, info.Target, deltaTime, collisionEvents);
            }
        }

        public override AABB GetAABB(float deltaTime)
        {
            var points = GetCornerPoints(deltaTime);
            Vector2 minPoint = points[0];
            Vector2 maxPoint = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                minPoint.X = Math.Min(points[i].X, minPoint.X);
                minPoint.Y = Math.Min(points[i].Y, minPoint.Y);
                maxPoint.X = Math.Max(points[i].X, maxPoint.X);
                maxPoint.Y = Math.Max(points[i].Y, maxPoint.Y);
            }
            return new AABB() { MaxPoint = maxPoint, MinPoint = minPoint };
        }
    }
}


//float maxDepth = -1;
//if (edgeBelongsToA)
//{
//    for (int i = 0; i < cornersB.Count; i++)
//    {
//        if (IsInsideEdge(cornersB[i], keyEdge))
//        {
//            float d = Utils.PointDistance2ToSegment(cornersB[i], keyEdge._pA, keyEdge._pB);
//            if (d > maxDepth)
//            {
//                maxDepth = d;
//            }
//        }
//    }
//    Debug.Assert(maxDepth >= 0);
//    for (int i = 0; i < cornersB.Count; i++)
//    {
//        if (IsInsideEdge(cornersB[i], keyEdge))
//        {
//            float d = Utils.PointDistance2ToSegment(cornersB[i], keyEdge._pA, keyEdge._pB);
//            if (d == maxDepth)
//            {
//                collisionEvents.Add(new CollisionEvent2D()
//                {
//                    Time = 0,
//                    Source = this.ParentObject,
//                    Target = b.ParentObject,
//                    LocalOffsetSrc = Utils.ProjectPointOnSegment(cornersB[i], keyEdge._pA, keyEdge._pB)
//                        - ParentObject.RigidBody.CentroidWorldSpace,
//                    LocalOffsetTarget = cornersB[i] - b.ParentObject.RigidBody.CentroidWorldSpace,
//                    Normal = normal,
//                    Position = Vector2.Zero,
//                    Depth = depth
//                });
//            }
//        }
//    }

//}
//else
//{
//    for (int i = 0; i < cornersA.Count; i++)
//    {
//        if (IsInsideEdge(cornersA[i], keyEdge))
//        {
//            float d = Utils.PointDistance2ToSegment(cornersA[i], keyEdge._pA, keyEdge._pB);
//            if (d > maxDepth)
//            {
//                maxDepth = d;
//            }
//        }
//    }
//    Debug.Assert(maxDepth >= 0);
//    for (int i = 0; i < cornersA.Count; i++)
//    {
//        if (IsInsideEdge(cornersA[i], keyEdge))
//        {
//            float d = Utils.PointDistance2ToSegment(cornersA[i], keyEdge._pA, keyEdge._pB);
//            if (d == maxDepth)
//            {
//                collisionEvents.Add(new CollisionEvent2D()
//                {
//                    Time = 0,
//                    Source = b.ParentObject,
//                    Target = this.ParentObject,
//                    LocalOffsetSrc = Utils.ProjectPointOnSegment(cornersA[i], keyEdge._pA, keyEdge._pB)
//                        - b.ParentObject.RigidBody.CentroidWorldSpace,
//                    LocalOffsetTarget = cornersA[i] - ParentObject.RigidBody.CentroidWorldSpace,
//                    Normal = -normal,
//                    Position = Vector2.Zero,
//                    Depth = depth
//                });
//            }
//        }
//    }
//}
//collisionEvents.Add(new CollisionEvent2D()
//{
//    Time = 0,
//    Source = this.ParentObject,
//    Target = b.ParentObject,
//    LocalOffsetSrc = curPoint - (b.ParentObject.RigidBody.CentroidWorldSpace + b.ParentObject.RigidBody.LinearVelocity * (float)ans),
//    LocalOffsetTarget = curPoint - (ParentObject.RigidBody.CentroidWorldSpace + ParentObject.RigidBody.LinearVelocity * (float)ans),
//    Normal = -Vector2.Normalize(Utils.Rotate90(closestEdge._pB - closestEdge._pA)),
//    Position = curPoint,
//});

//AABBCollider b = (AABBCollider)other;
//double l = 0, r = deltaTime;
//double ans = r;
//while (r - l > deltaTime * 1e-2)
//{
//    double midTime = (r + l) / 2.0;
//    if (IsCollideWith(b, (float)midTime))
//    {
//        ans = Math.Min(ans, midTime);
//        r = midTime;
//    }
//    else
//    {
//        l = midTime;
//    }
//}

//double t = Math.Max(0, ans);
//var curPointsA = GetCornerPoints((float)ans);
//var curEdgesA = GetEdges((float)ans);
//var curPointsB = b.GetCornerPoints((float)ans);
//var curEdgesB = b.GetEdges((float)ans);

//// Vertex/Edge collision, A to B
//foreach (var curPoint in curPointsA)
//{
//    bool isInside = true;
//    Edge2D closestEdge = null;
//    float closestDistance = float.PositiveInfinity;
//    foreach(var curEdge in curEdgesB)
//    {
//        if (!IsInsideEdge(curPoint, curEdge))
//        {
//            isInside = false;
//            break;
//        }
//        float dist = Utils.PointDistance2ToSegment(curPoint, curEdge._pA, curEdge._pB);
//        if (dist < closestDistance)
//        {
//            closestDistance = dist;
//            closestEdge = curEdge;
//        }
//    }

//    if(isInside)
//    {
//        collisionEvents.Add(new CollisionEvent2D()
//        {
//            Time = (float)(t),
//            Source = this.ParentObject,
//            Target = b.ParentObject,
//            LocalOffsetSrc = curPoint - (ParentObject.RigidBody.CentroidWorldSpace + ParentObject.RigidBody.LinearVelocity * (float)ans),
//            LocalOffsetTarget = curPoint - (b.ParentObject.RigidBody.CentroidWorldSpace + b.ParentObject.RigidBody.LinearVelocity * (float)ans),
//            Normal = -Vector2.Normalize(Utils.Rotate90(closestEdge._pB - closestEdge._pA)),
//            Position = curPoint,

//        });
//    }

//}

//// Vertex/Edge collision, B to A
//foreach (var curPoint in curPointsB)
//{
//    bool isInside = true;
//    Edge2D closestEdge = null;
//    float closestDistance = float.PositiveInfinity;
//    foreach (var curEdge in curEdgesA)
//    {
//        if (!IsInsideEdge(curPoint, curEdge))
//        {
//            isInside = false;
//            break;
//        }
//        float dist = Utils.PointDistance2ToSegment(curPoint, curEdge._pA, curEdge._pB);
//        if (dist < closestDistance)
//        {
//            closestDistance = dist;
//            closestEdge = curEdge;
//        }
//    }

//    if (isInside)
//    {
//        collisionEvents.Add(new CollisionEvent2D()
//        {
//            Time = (float)(t),
//            Source = b.ParentObject,
//            Target = this.ParentObject,
//            LocalOffsetSrc = curPoint - (b.ParentObject.RigidBody.CentroidWorldSpace + b.ParentObject.RigidBody.LinearVelocity * (float)ans),
//            LocalOffsetTarget = curPoint - (ParentObject.RigidBody.CentroidWorldSpace + ParentObject.RigidBody.LinearVelocity * (float)ans),
//            Normal = -Vector2.Normalize(Utils.Rotate90(closestEdge._pB - closestEdge._pA)),
//            Position = curPoint,
//        });
//    }

//}
