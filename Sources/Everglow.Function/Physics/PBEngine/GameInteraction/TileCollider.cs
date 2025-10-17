using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;

namespace Everglow.Commons.Physics.PBEngine.GameInteraction
{
	/// <summary>
	/// 与TR世界地形碰撞的碰撞体
	/// </summary>
	public class TileCollider : Collider2D
	{
		public override AABB GetAABB(float deltaTime)
		{
			return new AABB()
			{
				MinPoint = new Vector2(float.NegativeInfinity, float.NegativeInfinity),
				MaxPoint = new Vector2(float.PositiveInfinity, 0),
			};
		}

		public override void GetContactInfo(in CollisionInfo info, float deltaTime, out List<CollisionEvent2D> collisionEvents)
		{
			// Leave empty since static object will not serve as collision source
			collisionEvents = new List<CollisionEvent2D>();
		}

		// public override List<CollisionEvent2D> GetContactInfo(CollisionEvent2D e, float deltaTime)
		// {
		//    return new List<CollisionEvent2D>();
		//    // Leave empty since static object will not serve as collision source
		// }
		public override List<Vector2> GetWireFrameWires()
		{
			return new List<Vector2>();
		}

		public override double InertiaTensor(float mass)
		{
			return 1;
		}

		public override bool TestCollisionCondition(Collider2D other, float deltaTime, out CollisionInfo info)
		{
			// Leave empty since static object will not serve as collision source
			info = default(CollisionInfo);
			return false;
		}
	}
}