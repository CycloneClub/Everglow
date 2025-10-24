using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Physics.PBEngine.Core;

namespace Everglow.Commons.Physics.PBEngine.Collision
{
	/// <summary>
	/// 储存碰撞事件的对象，包括穿透深度和挤出方向，不包括接触点
	/// </summary>
	public struct CollisionInfo
    {
        public PhysicsObject Source;
        public PhysicsObject Target;
        public float Time;
        public float Depth;
        public Vector2 Normal;
    }
}
