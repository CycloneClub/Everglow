using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.Collision.BroadPhase
{
	/// <summary>
	/// 粗粒度碰撞检测的基类
	/// </summary>
	public abstract class BroadPhase
    {
        protected CollisionGraph _collisionGraph;
        public BroadPhase(CollisionGraph graph)
        {
            _collisionGraph = graph;
        }
        public abstract void Prepare(List<PhysicsObject> dynamicObjects, float deltaTime);

        public abstract List<KeyValuePair<Collider2D, Collider2D>> GetCollisionPairs(float deltaTime);
    }
}
