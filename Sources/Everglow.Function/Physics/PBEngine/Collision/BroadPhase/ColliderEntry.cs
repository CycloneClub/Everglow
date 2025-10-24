namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase
{
	public struct ColliderEntry
    {
        public int ColliderId;
        public AABB BoundingBox;
        public bool IsDynamic;
    }
}
