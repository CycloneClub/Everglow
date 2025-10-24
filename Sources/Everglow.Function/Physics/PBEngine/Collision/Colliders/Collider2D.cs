using Everglow.Commons.Physics.PBEngine.Core;

namespace Everglow.Commons.Physics.PBEngine.Collision.Colliders
{
	/// <summary>
	/// 碰撞组件的基类
	/// </summary>
	public abstract class Collider2D
	{
		public PhysicsObject ParentObject
		{
			get => _bindObject;
			set => _bindObject = value;
		}

		protected PhysicsObject _bindObject;

		public abstract double InertiaTensor(float mass);

		public abstract List<Vector2> GetWireFrameWires();

		// public abstract List<CollisionEvent2D> GetCollisionEvents(Collider2D other, float deltaTime);
		public abstract bool TestCollisionCondition(Collider2D other, float deltaTime, out CollisionInfo info);

		public abstract void GetContactInfo(in CollisionInfo info, float deltaTime,
			out List<CollisionEvent2D> collisionEvents);

		// public abstract List<CollisionEvent2D> GetContactInfo(CollisionEvent2D e, float deltaTime);
		public abstract AABB GetAABB(float deltaTime);

		public Collider2D()
		{
		}
	}
}