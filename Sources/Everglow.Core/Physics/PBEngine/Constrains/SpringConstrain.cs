using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Utilities;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine.Constrains
{
	/// <summary>
	/// 弹簧约束
	/// </summary>
	public class SpringConstrain : Constrain
    {
        private PhysicsObject _objA;
        private PhysicsObject _objB;
        private float _compliance;
        private float _restLength;
        private float _lambda;
        private Vector2 _localPosA;
        private Vector2 _localPosB;

        public SpringConstrain(PhysicsObject A, PhysicsObject B, float elasticity, float restLength)
        {
            _objA = A;
            _objB = B;
            _compliance = 1 / elasticity;
            _restLength = restLength;
            _lambda = 0;
            _localPosA = Vector2.Zero;
            _localPosB = Vector2.Zero;
        }

        public SpringConstrain(PhysicsObject A, PhysicsObject B, float elasticity, float restLength,
            Vector2 localPosA,
            Vector2 localPosB) : this(A, B, elasticity, restLength)
        {
            _localPosA = localPosA;
            _localPosB = localPosB;
        }

        public override void Apply(float deltaTime)
        {
            var pA = _objA.LocalToWorldPos(_localPosA);
            var pB = _objB.LocalToWorldPos(_localPosB);
            var L = (pB - pA).Length();
            var unit = (pB - pA).SafeNormalize(Vector2.Zero);
            float C = L - _restLength;
            float alpha = _compliance / deltaTime / deltaTime;
            float d_lambda = (-C - alpha * _lambda) / (_objA.RigidBody.InvMass + _objB.RigidBody.InvMass + alpha);

            _lambda += d_lambda;
            var curPosA = pA + _lambda * _objA.RigidBody.InvMass * -unit;
            var curPosB = pB + _lambda * _objB.RigidBody.InvMass * unit;

            var ra = Matrix2x2.CreateRotationMatrix(_objA.Rotation).Multiply(_localPosA);
            var rb = Matrix2x2.CreateRotationMatrix(_objB.Rotation).Multiply(_localPosB);
            var da = (_lambda * _objA.RigidBody.InvMass * -unit).SafeNormalize(Vector2.Zero);
            var db = (_lambda * _objB.RigidBody.InvMass * unit).SafeNormalize(Vector2.Zero);
            var oldVelA = Vector2.Dot(_objA.RigidBody.LinearVelocity + GeometryUtils.AnuglarVelocityToLinearVelocity(ra, _objA.RigidBody.AngularVelocity), da);
            var oldVelB = Vector2.Dot(_objB.RigidBody.LinearVelocity + GeometryUtils.AnuglarVelocityToLinearVelocity(rb, _objB.RigidBody.AngularVelocity), db);

            double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ra), da);
            double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), db);
            double R1 = rAdotN * rAdotN * _objA.RigidBody.GlobalInverseInertiaTensor;
            double R2 = rBdotN * rBdotN * _objB.RigidBody.GlobalInverseInertiaTensor;
            var impluseA = ((curPosA - pA).Length() / deltaTime - oldVelA) / (_objA.RigidBody.InvMass + (float)R1);
            var impluseB = ((curPosB - pB).Length() / deltaTime - oldVelB) / (_objB.RigidBody.InvMass + (float)R2);

            if (_objA.RigidBody.InvMass != 0)
            {
                _objA.RigidBody.AddImpluseImmediate(impluseA * da, ra);
                _objA.Position += _objA.RigidBody.LinearVelocity * deltaTime;
                _objA.Rotation += _objA.RigidBody.AngularVelocity * deltaTime;
            }
            if (_objB.RigidBody.InvMass != 0)
            {
                _objB.RigidBody.AddImpluseImmediate(impluseB * db, rb);
                _objB.Position += _objB.RigidBody.LinearVelocity * deltaTime;
                _objB.Rotation += _objB.RigidBody.AngularVelocity * deltaTime;
            }
            //_objA.RigidBody.AngularVelocity = Utils.Cross(_localPosA, _objA.RigidBody.LinearVelocity);
            //_objB.RigidBody.AngularVelocity = Utils.Cross(_localPosB, _objB.RigidBody.LinearVelocity);
        }

        public override List<Vector2> GetDrawMesh()
        {
            List<Vector2> drawMesh = new List<Vector2>();
            drawMesh.Add(_objA.LocalToWorldPos(_localPosA));
            drawMesh.Add(_objB.LocalToWorldPos(_localPosB));

            return drawMesh;
        }
    }
}
