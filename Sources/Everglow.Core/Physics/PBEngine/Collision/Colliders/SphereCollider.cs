using Everglow.Commons.Physics.PBEngine.GameInteraction;
using Everglow.Commons.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Collision.Colliders
{
	/// <summary>
	/// 圆形碰撞体
	/// </summary>
	public class SphereCollider : Collider2D
    {
        public float Radius
        {
            get => _radius; set => _radius = value;
        }
        private float _radius;
        public SphereCollider(float radius)
        {
            _radius = radius;
        }
        public override AABB GetAABB(float deltaTime)
        {
            var center = ParentObject.OldPosition + ParentObject.RigidBody.LinearVelocity * deltaTime;
            return new AABB() {
                MinPoint = center - new Vector2(_radius, _radius), 
                MaxPoint = center + new Vector2(_radius, _radius), 
                 };
        }

        public override void GetContactInfo(in CollisionInfo info, float deltaTime, out List<CollisionEvent2D> collisionEvents)
        {
            collisionEvents = new List<CollisionEvent2D>();
            if (info.Target.Collider is SphereCollider)
            {
                SphereCollider b = (SphereCollider)info.Target.Collider;
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

                var unit = (ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace).SafeNormalize(Vector2.Zero);
                collisionEvents.Add(new CollisionEvent2D(e)
                {
                    LocalOffsetSrc = -unit * _radius,
                    LocalOffsetTarget = unit * b.Radius,
                });
            }
            else if (info.Target.Collider is BoxCollider)
            {
                BoxCollider b = (BoxCollider)info.Target.Collider;
                var e = new CollisionEvent2D()
                {
                    Time = info.Time,
                    Source = info.Source,
                    Target = info.Target,
                    LocalOffsetSrc = Vector2.Zero,
                    LocalOffsetTarget = Vector2.Zero,
                    Normal = Vector2.Dot(info.Normal, ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace) < 0 ? -info.Normal : info.Normal,
                    Position = Vector2.Zero,
                    Depth = info.Depth,
                };

                List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
                GeometryUtils.CirclePolygonContactInfo(ParentObject.RigidBody.CentroidWorldSpace, _radius, b.GetEdges(deltaTime),
                    b.GetCornerPoints(deltaTime), b.ParentObject.RigidBody.CentroidWorldSpace, contacts);
                foreach (var c in contacts)
                {
                    collisionEvents.Add(new CollisionEvent2D(e)
                    {
                        LocalOffsetSrc = c.Key,
                        LocalOffsetTarget = c.Value,
                    });
                }
            }
            else if (info.Target.Collider is TileCollider)
            {
                TileCollisionUtils.GetSphereTileContactInfo(ParentObject.RigidBody.CentroidWorldSpace, _radius,
                   ParentObject, info.Target, deltaTime, collisionEvents);
            }
        }

        //public override List<CollisionEvent2D> GetContactInfo(CollisionEvent2D e, float deltaTime)
        //{
        //    List<CollisionEvent2D> events = new List<CollisionEvent2D>();
        //    if (e.Target.Collider is SphereCollider)
        //    {
        //        SphereCollider b = (SphereCollider)e.Target.Collider;
        //        var unit = (ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace).SafeNormalize(Vector2.Zero);
        //        events.Add(new CollisionEvent2D(e)
        //        {
        //            LocalOffsetSrc = -unit * _radius,
        //            LocalOffsetTarget = unit * b.Radius,
        //        });
        //    }
        //    else if (e.Target.Collider is BoxCollider)
        //    {
        //        BoxCollider b = (BoxCollider)e.Target.Collider;
        //        List<Edge2D> edgesB = b.GetEdges(deltaTime);
        //        List<Vector2> cornersB = b.GetCornerPoints(deltaTime);
        //        List<KeyValuePair<Vector2, Vector2>> contacts = new List<KeyValuePair<Vector2, Vector2>>();
        //        Utils.CirclePolygonContactInfo(ParentObject.RigidBody.CentroidWorldSpace, _radius, edgesB,
        //            cornersB, b.ParentObject.RigidBody.CentroidWorldSpace, contacts);
        //        foreach (var c in contacts)
        //        {
        //            events.Add(new CollisionEvent2D(e)
        //            {
        //                LocalOffsetSrc = c.Key,
        //                LocalOffsetTarget = c.Value,
        //            });
        //        }
        //    }
        //    else if (e.Target.Collider is TileCollider)
        //    {
        //        TileCollisionUtils.GetSphereTileContactInfo(ParentObject.RigidBody.CentroidWorldSpace, _radius, e, events);
        //    }
        //    return events;
        //}

        public override List<Vector2> GetWireFrameWires()
        {
            List<Vector2> lines = new List<Vector2>(); 
            float da = MathHelper.TwoPi / 64;
            for (int i = 0; i < 64; i++)
            {
                lines.Add((i * da).ToRotationVector2() * _radius);
                lines.Add(((i + 1) * da).ToRotationVector2() * _radius);
            }
            lines.Add(Vector2.Zero);
            lines.Add(Matrix2x2.CreateRotationMatrix(ParentObject.Rotation).Multiply(new Vector2(_radius, 0)));
            return lines;
        }

        public override double InertiaTensor(float mass)
        {
            return 0.5 * mass * _radius * _radius;
        }

        public override bool TestCollisionCondition(Collider2D other, float deltaTime, out CollisionInfo info)
        {
            info = new CollisionInfo();
            if (other is SphereCollider)
            {
                SphereCollider b = (SphereCollider)other;
                float d = Vector2.DistanceSquared(ParentObject.RigidBody.CentroidWorldSpace, b.ParentObject.RigidBody.CentroidWorldSpace);
                if (d < (_radius + b._radius) * (_radius + b._radius))
                {
                    info.Source = this.ParentObject;
                    info.Target = b.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = _radius + b._radius - (float)Math.Sqrt(d);
                    info.Normal = (ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace).SafeNormalize(Vector2.One);
                    return true;
                }
            }
            else if (other is BoxCollider)
            {
                BoxCollider b = (BoxCollider)other;
                float depth;
                Vector2 normal;
                if (GeometryUtils.CirclePolygonCollisionInfo(ParentObject.RigidBody.CentroidWorldSpace, _radius,
                   b.GetEdges(deltaTime), b.GetCornerPoints(deltaTime), out depth, out normal))
                {
                    info.Source = this.ParentObject;
                    info.Target = b.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = depth;
                    info.Normal = Vector2.Dot(normal, ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace) < 0 ? -normal : normal;
                    return true;
                }
            }
            else if (other is TileCollider)
            {
                float depth;
                Vector2 normal;
                if (TileCollisionUtils.GetSphereTileCollisionInfo(ParentObject.RigidBody.CentroidWorldSpace, _radius,
                   ParentObject, info.Target, out depth, out normal))
                {
                    info.Source = this.ParentObject;
                    info.Target = other.ParentObject;
                    info.Time = deltaTime;
                    info.Depth = depth;
                    info.Normal = normal;
                    return true;
                }
            }
            return false;
        }
    }
}
