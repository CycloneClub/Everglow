using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.Collision
{
	public struct ImpulseEntry
    {
        public RigidBody2D Source;
        public RigidBody2D Target;
        public Vector2 ImpulseSource;
        public Vector2 RelativePositionSource;
        public Vector2 ImpulseTarget;
        public Vector2 RelativePositionTarget;
    }
}
