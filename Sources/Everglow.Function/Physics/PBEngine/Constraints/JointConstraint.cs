using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Physics.PBEngine.Core;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Constraints
{
	/// <summary>
	/// TODO: 待定约束
	/// </summary>
	public class JointConstraint : Constraint
    {
        private PhysicsObject _objA;
        private PhysicsObject _objB;
        private Vector2 _localPosA;
        private Vector2 _localPosB;
        public JointConstraint(PhysicsObject A, PhysicsObject B, Vector2 localPos1, Vector2 localPos2)
        {
            _objA = A;
            _objB = B;
            _localPosA = localPos1;
            _localPosB = localPos2;
        }

        public override void Apply(float deltaTime)
        {
            var pA = _objA.LocalToWorldPos(_localPosA);
            // var pB = _objB.LocalToWorldPos(_localPosB);
            float w = _objA.RigidBody.InvMass + _objB.RigidBody.InvMass;
            float weightA = _objA.RigidBody.InvMass / w;
            var newCenter = pA;
            var dirA = (newCenter - _objA.Position).SafeNormalize(Vector2.One);
            var dirB = (newCenter - _objB.Position).SafeNormalize(Vector2.One);

            float oldRotA = MathHelper.WrapAngle(_objA.Rotation);
            float oldRotB = MathHelper.WrapAngle(_objB.Rotation);
            //_objA.Rotation = (float)(Math.Atan2(dirA.Y, dirA.X) - Math.Atan2(_localPosA.Y, _localPosA.X));
            //_objB.Rotation = (float)(Math.Atan2(dirB.Y, dirB.X) - Math.Atan2(_localPosB.Y, _localPosB.X));

            Vector2 addA = newCenter - _objA.LocalToWorldPos(_localPosA);
            Vector2 addB = newCenter - _objB.LocalToWorldPos(_localPosB);
            Vector2 oldPosB = _objB.Position;
            _objA.Position += addA;
            _objB.Position = newCenter - dirB * 200;

            _objA.RigidBody.LinearVelocity = (_objA.Position - _objA.OldPosition) / deltaTime;
            _objB.RigidBody.LinearVelocity = (_objB.Position - _objB.OldPosition) / deltaTime;
            //_objA.RigidBody.AngularVelocity = MathHelper.WrapAngle(_objA.Rotation - _objA.OldRotation) / deltaTime;
            //_objB.RigidBody.AngularVelocity = MathHelper.WrapAngle(_objB.Rotation - _objB.OldRotation) / deltaTime;
        }

        public override void ApplyForce(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public override List<(Vector2, Color)> GetDrawMesh()
        {
            var drawMesh = new List<(Vector2, Color)>();
            drawMesh.Add((_objA.LocalToWorldPos(_localPosA), Color.Green));
            drawMesh.Add((_objB.RigidBody.CentroidWorldSpace, Color.Green));

            return drawMesh;
        }
    }
}
