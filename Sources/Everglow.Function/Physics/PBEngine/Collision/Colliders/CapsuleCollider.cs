using Everglow.Commons.Physics.PBEngine.Core;
using Everglow.Commons.Physics.PBEngine.GameInteraction;
using Everglow.Commons.Utilities;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Collision.Colliders
{
	internal class CapsuleCollider : Collider2D
	{
		private float _length;
		private float _radius;

		public float Length
		{
			get => _length;
			set => _length = value;
		}

		public float Radius
		{
			get => _radius;
			set => _radius = value;
		}

		public CapsuleCollider(float length, float radius)
		{
			_length = length;
			_radius = radius;
		}

		public override AABB GetAABB(float deltaTime)
		{
			var M = Matrix2x2.CreateRotationMatrix(ParentObject.OldRotation + ParentObject.RigidBody.AngularVelocity * deltaTime);
			Vector2 P1 = ParentObject.OldPosition + ParentObject.RigidBody.LinearVelocity * deltaTime
				+ M.Multiply(new Vector2(_length / 2, 0));
			Vector2 P2 = ParentObject.OldPosition + ParentObject.RigidBody.LinearVelocity * deltaTime
				+ M.Multiply(new Vector2(-_length / 2, 0));

			if (P1.X > P2.X)
			{
				(P2.X, P1.X) = (P1.X, P2.X);
			}
			if (P1.Y > P2.Y)
			{
				(P2.Y, P1.Y) = (P1.Y, P2.Y);
			}
			return new AABB()
			{
				MinPoint = P1 - new Vector2(_radius, _radius),
				MaxPoint = P2 + new Vector2(_radius, _radius),
			};
		}

		public override void GetContactInfo(in CollisionInfo info, float deltaTime, out List<CollisionEvent2D> collisionEvents)
		{
			collisionEvents = new List<CollisionEvent2D>();
			if (info.Target.Collider is SphereCollider)
			{
				var b = (SphereCollider)info.Target.Collider;
				var e = new CollisionEvent2D()
				{
					Time = info.Time,
					Source = ParentObject,
					Target = b.ParentObject,
					LocalOffsetSrc = Vector2.Zero,
					LocalOffsetTarget = Vector2.Zero,
					Normal = info.Normal,
					Position = Vector2.Zero,
					Depth = info.Depth,
				};
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				var contacts = new List<KeyValuePair<Vector2, Vector2>>();
				GeometryUtils.SphereCapsuleContactInfo(
					b.ParentObject.RigidBody.CentroidWorldSpace,
					b.Radius, A, B, _radius, contacts);
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
				var b = (CapsuleCollider)info.Target.Collider;
				var e = new CollisionEvent2D()
				{
					Time = info.Time,
					Source = ParentObject,
					Target = b.ParentObject,
					LocalOffsetSrc = Vector2.Zero,
					LocalOffsetTarget = Vector2.Zero,
					Normal = info.Normal,
					Position = Vector2.Zero,
					Depth = info.Depth,
				};
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				b.GetSegment(deltaTime, out Vector2 A2, out Vector2 B2);
				var contacts = new List<KeyValuePair<Vector2, Vector2>>();
				GeometryUtils.CapsuleCapsuleContactInfo(A, B, _radius, A2, B2, b._radius, contacts);
				foreach (var c in contacts)
				{
					collisionEvents.Add(new CollisionEvent2D(e)
					{
						LocalOffsetSrc = c.Key,
						LocalOffsetTarget = c.Value,
					});
				}
			}
			else if (info.Target.Collider is BoxCollider)
			{
				var b = (BoxCollider)info.Target.Collider;
				var e = new CollisionEvent2D()
				{
					Time = info.Time,
					Source = ParentObject,
					Target = b.ParentObject,
					LocalOffsetSrc = Vector2.Zero,
					LocalOffsetTarget = Vector2.Zero,
					Normal = info.Normal,
					Position = Vector2.Zero,
					Depth = info.Depth,
				};
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				var contacts = new List<KeyValuePair<Vector2, Vector2>>();
				GeometryUtils.CapsuleConvexPolygonContactInfo(A, B, _radius,
					b.GetCornerPoints(deltaTime), b.GetEdges(deltaTime), b.ParentObject.RigidBody.CentroidWorldSpace, contacts);
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
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				GameInteraction.TileCollisionUtils.GetCapsuleTileContactInfo(GetAABB(deltaTime), A, B, _radius, ParentObject, info.Target, deltaTime, collisionEvents);
			}
		}

		public override List<Vector2> GetWireFrameWires()
		{
			var lines = new List<Vector2>();
			lines.Add(new Vector2(_length / 2, _radius));
			lines.Add(new Vector2(-_length / 2, _radius));

			float da = MathHelper.Pi / 32;
			var leftCenter = new Vector2(-_length / 2, 0);
			for (int i = 0; i < 32; i++)
			{
				lines.Add(leftCenter + (i * da + MathHelper.PiOver2).ToRotationVector2() * _radius);
				lines.Add(leftCenter + ((i + 1) * da + MathHelper.PiOver2).ToRotationVector2() * _radius);
			}
			lines.Add(new Vector2(-_length / 2, -_radius));
			lines.Add(new Vector2(_length / 2, -_radius));
			var rightCenter = new Vector2(_length / 2, 0);
			for (int i = 0; i < 32; i++)
			{
				lines.Add(rightCenter + (i * da - MathHelper.PiOver2).ToRotationVector2() * _radius);
				lines.Add(rightCenter + ((i + 1) * da - MathHelper.PiOver2).ToRotationVector2() * _radius);
			}
			lines.Add(Vector2.Zero);
			lines.Add(new Vector2(_length / 2 + _radius, 0));
			return lines;
		}

		public override double InertiaTensor(float mass)
		{
			float massOfBox = mass * 2 * _radius * _length / (2 * _radius * _length + MathHelper.Pi * _radius * _radius);
			float massOfSphere = mass - massOfBox;
			float moi_box = massOfBox / 12.0f * (_length * _length + 4 * _radius * _radius);
			float moi_sphere = 0.5f * massOfSphere * _radius * _radius;
			float d1 = 3.0f / 8.0f * _radius;
			float moi_hsphere = 0.5f * moi_sphere - 0.5f * massOfSphere * d1 * d1;
			float d2 = _length / 2 + d1;
			moi_hsphere += 0.5f * massOfSphere * d2 * d2;
			return moi_box + 2 * moi_hsphere;
		}

		public void GetSegment(float dt, out Vector2 segA, out Vector2 segB)
		{
			var M = ParentObject.CachedRotationalMatrix;
			var pos = ParentObject.Position;
			segA = pos + M.Multiply(new Vector2(_length / 2, 0));
			segB = pos + M.Multiply(new Vector2(-_length / 2, 0));
		}

		public override bool TestCollisionCondition(Collider2D other, float deltaTime, out CollisionInfo info)
		{
			info = default;
			if (other is SphereCollider)
			{
				var b = (SphereCollider)other;
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				if (GeometryUtils.SphereCapsuleCollisionInfo(b.ParentObject.RigidBody.CentroidWorldSpace, b.Radius,
					A, B, _radius, out float depth, out Vector2 normal))
				{
					info.Source = ParentObject;
					info.Target = b.ParentObject;
					info.Time = deltaTime;
					info.Depth = depth;
					info.Normal = -normal;
					return true;
				}
			}
			else if (other is CapsuleCollider)
			{
				var b = (CapsuleCollider)other;
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				b.GetSegment(deltaTime, out Vector2 A2, out Vector2 B2);
				if (GeometryUtils.CapsuleCapsuleCollisionInfo(A, B, _radius, A2, B2, b._radius,
					out float depth, out Vector2 normal))
				{
					info.Source = ParentObject;
					info.Target = b.ParentObject;
					info.Time = deltaTime;
					info.Depth = depth;
					info.Normal = Vector2.Dot(normal, ParentObject.RigidBody.CentroidWorldSpace - b.ParentObject.RigidBody.CentroidWorldSpace) < 0 ? -normal : normal;
					return true;
				}
			}
			else if (other is BoxCollider)
			{
				var b = (BoxCollider)other;
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				if (GeometryUtils.CapsuleConvexPolygonCollisionInfo(A, B, _radius, b.GetCornerPoints(deltaTime),
					b.GetEdges(deltaTime),
					out float depth, out Vector2 normal))
				{
					info.Source = ParentObject;
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
				GetSegment(deltaTime, out Vector2 A, out Vector2 B);
				if (GameInteraction.TileCollisionUtils.GetCapsuleTileCollisionInfo(GetAABB(deltaTime), A, B, _radius, out depth, out normal))
				{
					info.Source = ParentObject;
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