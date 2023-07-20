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
        private double _lambda;
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

		public static double CalculateShortestAngleDifference(double angle1, double angle2)
		{
			double difference = angle2 - angle1;
			difference = (difference + Math.PI) % (2 * Math.PI) - Math.PI;

			// Adjust the difference to the shortest rotating radians
			if (difference < -Math.PI)
			{
				difference += 2 * Math.PI;
			}
			else if (difference >= Math.PI)
			{
				difference -= 2 * Math.PI;
			}

			return difference;
		}

		public override void Apply(float deltaTime)
		{
            //var pA = _objA.LocalToWorldPos(_localPosA);
            //var pB = _objB.LocalToWorldPos(_localPosB);
            //var ra = Matrix2x2.CreateRotationMatrix(_objA.Rotation).Multiply(_localPosA);
            //var rb = Matrix2x2.CreateRotationMatrix(_objB.Rotation).Multiply(_localPosB);
            //var L = (pB - pA).Length();
            //var unit = (pB - pA).SafeNormalize(Vector2.Zero);
            //float C = L - _restLength;
            //if (C == 0)
            //{
            //    return;
            //}

            //double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ra), -(pB - pA));
            //double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), (pB - pA));
            //double R1 = rAdotN * rAdotN * _objA.RigidBody.GlobalInverseInertiaTensor;
            //double R2 = rBdotN * rBdotN * _objB.RigidBody.GlobalInverseInertiaTensor;
            //float effMassA = (float)(_objA.RigidBody.InvMass + R1);
            //float effMassB = (float)(_objB.RigidBody.InvMass + R2);
            //float alpha = _compliance / deltaTime / deltaTime;
            //double d_lambda = (-C - alpha * _lambda) / (effMassA + effMassB + alpha);
            //_lambda = _lambda + d_lambda;

            ////var posA = pA + (float)(_lambda * effMassA ) * -unit ;
            ////var posB = pB + (float)(_lambda * effMassB) * unit;
            ////float rotA_p = (pA - _objA.RigidBody.CentroidWorldSpace).ToRotation() - _localPosA.ToRotation();
            ////float rotA = (float)CalculateShortestAngleDifference((pA - _objA.RigidBody.CentroidWorldSpace).ToRotation(), 
            ////    (pB - _objA.RigidBody.CentroidWorldSpace).ToRotation());
            ////float rotB_p = (pB - _objB.RigidBody.CentroidWorldSpace).ToRotation() - _localPosB.ToRotation();
            ////float rotB = (float)CalculateShortestAngleDifference((pB - _objB.RigidBody.CentroidWorldSpace).ToRotation(),
            ////    (posB - _objB.RigidBody.CentroidWorldSpace).ToRotation());

            //if (_objB.RigidBody.GlobalInverseInertiaTensor != 0)
            //{
            //    _objB.RigidBody.AddImpluseImmediate((float)(d_lambda ) * (pB - pA), rb);
            //    //_objB.Rotation += _objB.RigidBody.AngularVelocity * deltaTime;
            //    //_objB.RigidBody.CentroidWorldSpace += _objB.RigidBody.LinearVelocity * deltaTime;
            //    //_objB.RigidBody.LinearVelocity = (_objB.RigidBody.CentroidWorldSpace - _objB.OldPosition) / deltaTime;
            //    //_objB.RigidBody.AngularVelocity = (float)CalculateShortestAngleDifference(_objB.OldRotation, _objB.Rotation) / deltaTime;
            //}
            //if (_objA.RigidBody.GlobalInverseInertiaTensor != 0)
            //{
            //    _objA.RigidBody.AddImpluseImmediate((float)(d_lambda) * -(pB - pA), ra);
            //    //_objA.Rotation += _objA.RigidBody.AngularVelocity * deltaTime;
            //    //_objA.RigidBody.CentroidWorldSpace += _objA.RigidBody.LinearVelocity * deltaTime;
            //    //_objA.Rotation += rotA;
            //    //_objA.RigidBody.CentroidWorldSpace += (posA - _objA.LocalToWorldPos(_localPosA));
            //    //_objA.RigidBody.LinearVelocity = (_objA.RigidBody.CentroidWorldSpace - _objA.OldPosition) / deltaTime;
            //    //_objA.RigidBody.AngularVelocity = (float)CalculateShortestAngleDifference(_objA.OldRotation, _objA.Rotation) / deltaTime;
            //}

            

            

            //var ra = Matrix2x2.CreateRotationMatrix(_objA.Rotation).Multiply(_localPosA);
            //var rb = Matrix2x2.CreateRotationMatrix(_objB.Rotation).Multiply(_localPosB);
            //var da = (_lambda * _objA.RigidBody.InvMass * -unit).SafeNormalize(Vector2.Zero);
            //var db = (_lambda * _objB.RigidBody.InvMass * unit).SafeNormalize(Vector2.Zero);
            //var oldVelA = Vector2.Dot(_objA.RigidBody.LinearVelocity + GeometryUtils.AnuglarVelocityToLinearVelocity(ra, _objA.RigidBody.AngularVelocity), da);
            //var oldVelB = Vector2.Dot(_objB.RigidBody.LinearVelocity + GeometryUtils.AnuglarVelocityToLinearVelocity(rb, _objB.RigidBody.AngularVelocity), db);

            //double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ra), da);
            //double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), db);
            //double R1 = rAdotN * rAdotN * _objA.RigidBody.GlobalInverseInertiaTensor;
            //double R2 = rBdotN * rBdotN * _objB.RigidBody.GlobalInverseInertiaTensor;
            //var impluseA = ((curPosA - pA).Length() / deltaTime - oldVelA) / (_objA.RigidBody.InvMass + (float)R1);
            //var impluseB = ((curPosB - pB).Length() / deltaTime - oldVelB) / (_objB.RigidBody.InvMass + (float)R2);

            //if (_objA.RigidBody.InvMass != 0)
            //         {
            //             // _objA.RigidBody.AddImpluseImmediate(impluseA * da, ra);
            //             _objA.Position += _objA.RigidBody.LinearVelocity * deltaTime;
            //             _objA.Rotation += _objA.RigidBody.AngularVelocity * deltaTime;

            //	_objA.RigidBody.LinearVelocity = (_objA.Position - _objA.OldPosition) / deltaTime;
            //}
            //         if (_objB.RigidBody.InvMass != 0)
            //         {
            // _objB.RigidBody.AddImpluseImmediate(impluseB * db, rb);
            //_objB.Position += _objB.RigidBody.LinearVelocity * deltaTime;
            //_objB.Rotation += _objB.RigidBody.AngularVelocity * deltaTime;
            //var test = _objB.LocalToWorldPos(_localPosB);
            //if (true)
            //	;

            //_objB.RigidBody.LinearVelocity = (_objB.Position - _objB.OldPosition) / deltaTime;
            //_objB.RigidBody.AngularVelocity = (_objB.Rotation - _objB.OldRotation) / deltaTime;
            // }
            //_objA.RigidBody.AngularVelocity = Utils.Cross(_localPosA, _objA.RigidBody.LinearVelocity);
            //_objB.RigidBody.AngularVelocity = Utils.Cross(_localPosB, _objB.RigidBody.LinearVelocity);
        }

        public override void ApplyForce(float deltaTime)
        {
            var pA = _objA.LocalToWorldPos(_localPosA);
            var pB = _objB.LocalToWorldPos(_localPosB);
            var L = (pB - pA).Length();
            var unit = (pB - pA).SafeNormalize(Vector2.Zero);
            float C = L - _restLength;
            var ra = _objA.CachedRotationalMatrix.Multiply(_localPosA);
            var rb = _objB.CachedRotationalMatrix.Multiply(_localPosB);
            _objB.RigidBody.AddForce(C * -unit / _compliance, rb);
            _objA.RigidBody.AddForce(C * unit / _compliance, ra);
        }

        public override List<(Vector2, Color)> GetDrawMesh()
        {
            List<(Vector2, Color)> drawMesh = new List<(Vector2, Color)>();
            drawMesh.Add((_objA.LocalToWorldPos(_localPosA), Color.Green));
            drawMesh.Add((_objB.LocalToWorldPos(_localPosB), Color.Green));

            return drawMesh;
        }
    }
}
