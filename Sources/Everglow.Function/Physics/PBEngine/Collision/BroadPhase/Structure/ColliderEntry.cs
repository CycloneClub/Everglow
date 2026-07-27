namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase.Structure
{
	public struct ColliderEntry
    {
        public int ColliderId;
        public AABB BoundingBox;
        public bool IsDynamic;
    }
}
