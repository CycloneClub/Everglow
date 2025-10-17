using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.Collision
{
	/// <summary>
	/// 储存碰撞事件的对象，包括接触点信息
	/// </summary>
	public class CollisionEvent2D
    {
        public PhysicsObject Source;
        public PhysicsObject Target;
        public float Time;
        public float Depth;
        public Vector2 Position;
        public Vector2 Normal;
        public Vector2 Offset;

        /// <summary>
        /// From centroid of source object to hit point
        /// </summary>
        public Vector2 LocalOffsetSrc;

        /// <summary>
        /// From centroid of target object to hit point
        /// </summary>
        public Vector2 LocalOffsetTarget;

        public float NormalVelOld;
        public Vector2 TangentDir;

        public CollisionEvent2D(CollisionEvent2D e)
        {
            this.Source = e.Source;
            this.Target = e.Target;
            this.Time = e.Time;
            this.Depth = e.Depth;
            this.Position = e.Position;
            this.Normal = e.Normal;
            this.Offset = e.Offset;
            this.NormalVelOld = e.NormalVelOld;
        }

        public CollisionEvent2D()
        {
        }

        public CollisionEvent2D CreateMirrorEvent()
        {
            return new CollisionEvent2D()
            {
                Source = this.Target,
                Target = this.Source,
                Time = this.Time,
                Position = this.Position,
                Normal = -this.Normal,
                Offset = -this.Offset,
                LocalOffsetSrc = this.LocalOffsetTarget,
                LocalOffsetTarget = this.LocalOffsetSrc
            };
        }
    }
}
