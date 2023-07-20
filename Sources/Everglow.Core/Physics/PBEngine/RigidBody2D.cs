using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Utilities;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine
{
    /// <summary>
    /// 2D刚体物理组件，用于实际的刚体运动模拟和响应的模块
    /// </summary>
    public class RigidBody2D
    {
        /// <summary>
        /// 指向物理对象的指针
        /// </summary>
        public PhysicsObject ParentObject
        {
            get => _bindObject;
            set => _bindObject = value;
        }

        /// <summary>
        /// 世界坐标下的质心坐标（指物理世界
        /// </summary>
        public Vector2 CentroidWorldSpace
        {
            get => _globalCentroid;
            set => _globalCentroid = value;
        }

        /// <summary>
        /// 质量
        /// </summary>
        public float Mass
        {
            get => _mass;
            set => _mass = value;
        }

        /// <summary>
        /// 质量的倒数
        /// </summary>
        public float InvMass
        {
            get => (MovementType == MovementType.Dynamic || MovementType == MovementType.Player) ? 1 / _mass : 0;
        }

        /// <summary>
        /// 世界坐标下惯性张量的倒数
        /// </summary>
        public double GlobalInverseInertiaTensor
        {
            get => MovementType == MovementType.Dynamic ? _globalInverseInertiaTensor : 0;
        }

        /// <summary>
        /// 是否启用重力
        /// </summary>
        public bool UseGravity
        {
            get; set;
        }

        /// <summary>
        /// 线速度
        /// </summary>
        public Vector2 LinearVelocity
        {
            get => MovementType == MovementType.Static ? Vector2.Zero : _linearVelocity;
            set => _linearVelocity = value;
        }


        /// <summary>
        /// 角速度
        /// </summary>
        public float AngularVelocity
        {
            get => MovementType == MovementType.Static ? 0 : _angularVelocity;
            set => _angularVelocity = value;
        }

        /// <summary>
        /// 移动模式
        /// </summary>
        public MovementType MovementType
        {
            get; set;
        }

        /// <summary>
        /// 线速度的阻尼
        /// </summary>
        public float Drag
        {
            get => _drag;
            set => _drag = value;
        }

        /// <summary>
        /// 角速度的阻尼
        /// </summary>
        public float AngularDrag
        {
            get => _angularDrag;
            set => _angularDrag = value;
        }

        /// <summary>
        /// 碰撞弹出的系数
        /// </summary>
        public float Restitution
        {
            get => _restitution;
            set => _restitution = value;
        }

        /// <summary>
        /// 摩擦力系数
        /// </summary>
        public float Friction
        {
            get => _friction;
            set => _friction = value;
        }

        public List<Vector2> TangetRelativeVelocity
        {
            get => _contactTangentVels;
        }

        public List<Vector2> ContactNormals
        {
            get => _contactNormals;
        }

        /// <summary>
        /// 物体是否是唤醒状态，如果唤醒那么可以进行运动模拟，注意唤醒时会重置运动值
        /// </summary>
        public bool IsAwake
        {
            get => _isAwake;
            set
            {
                if (value && !_isAwake)
                {
                    _motion = SleepEPS * 2;
                }
                else if (!value && _isAwake)
                {
                    _linearVelocity = Vector2.Zero;
                    _angularVelocity = 0;
                }
                _isAwake = value;
            }
        }

        /// <summary>
        /// 物体是否可以进入沉睡状态，用户能控制的物体应该设为false
        /// </summary>
        public bool CanSleep
        {
            get => _canSleep;
            set => _canSleep = value;
        }

        private double _localInverseInertiaTensor;
        private double _globalInverseInertiaTensor;

        private Vector2 _globalCentroid;

        private Vector2 _linearVelocity;
        private float _angularVelocity;

        private float _torque;
        private Vector2 _force;
        private float _mass;
        private float _drag;
        private float _angularDrag;
        private float _restitution;
        private float _friction;

        private List<Vector2> _contactTangentVels;
        private List<Vector2> _contactNormals;

        private List<ImpluseEntry> _impluses;

        private bool _isAwake;
        private bool _canSleep;
        private double _motion;

        private PhysicsObject _bindObject;

        private const double SleepEPS = 1e-2;

        public RigidBody2D(float mass)
        {
            _mass = mass;
            MovementType = MovementType.Dynamic;
            UseGravity = true;
            _isAwake = true;
            _canSleep = true;
            _linearVelocity = Vector2.Zero;
            _angularVelocity  = 0;
            _drag = 0.06f;
            _angularDrag = 0.06f;
            _impluses = new List<ImpluseEntry>();
            _restitution = 0.5f;
            _friction = 0.5f;
            _contactTangentVels = new List<Vector2>();
            _contactNormals = new List<Vector2>();

            _motion = SleepEPS * 2;
        }


        private void CalculateMassCentroidAndMoI()
        {
            _globalInverseInertiaTensor = _localInverseInertiaTensor = 1 / _bindObject.Collider.InertiaTensor(_mass);
        }

        public void Initialize()
        {
            CalculateMassCentroidAndMoI();
        }


		/// <summary>
		/// 进行一次运动积分
		/// </summary>
		/// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (!_isAwake || MovementType == MovementType.Static)
            {
                return;
            }

            _angularVelocity += (float)(_globalInverseInertiaTensor * deltaTime * _torque);
            _linearVelocity += deltaTime * _force / _mass;
            // _oldRot = _rotation;

            // _oldPos = _globalCentroid;
            _globalCentroid += deltaTime * _linearVelocity;
            _bindObject.Rotation += deltaTime * _angularVelocity;



            Debug.Assert(!float.IsNaN(_linearVelocity.X) && !float.IsNaN(_linearVelocity.Y));
            Debug.Assert(!float.IsNaN(_angularVelocity));

            StabilizeBody();

            // CollisionDetection(deltaTime);
            // CollisionResponse(deltaTime);
        }

        private void StabilizeBody()
        {
            if (Math.Abs(_linearVelocity.X) < PhysicsSimulation.EPS)
            {
                _linearVelocity.X = 0;
            }
            if (Math.Abs(_linearVelocity.Y) < PhysicsSimulation.EPS)
            {
                _linearVelocity.Y = 0;
            }

            if(Math.Abs(_angularVelocity) < PhysicsSimulation.EPS)
            {
                _angularVelocity = 0;
            }
        }

        public void ApplyForce(Vector2 force)
        {
            if (MovementType == MovementType.Static)
            {
                return;
            }
            _force += force;
        }

        public void CleanInformationSubstep(float deltaTime)
        {
            _torque = 0;
            _force = Vector2.Zero;
            _impluses.Clear();
            // Main.NewText((_linearVelocity.LengthSquared() * _mass * 0.5f + _mass * 9.8f * _globalCentroid.Y).ToString("F1"));
		}

        public void CleanInformationThisFrame(float deltaTime)
        {
            _linearVelocity *= 1 - _drag * _drag;
            _angularVelocity *= 1 - _angularDrag * _angularDrag;
            _contactTangentVels.Clear();
            _contactNormals.Clear();
        }

        public bool TryRespondTo2Events(List<CollisionEvent2D> events, float deltaTime)
        {
            Debug.Assert(events.Count == 2);
            Debug.Assert(events[0].Target == events[1].Target);
            var target = events[0].Target;

            var ri0 = events[0].LocalOffsetSrc;
            var rb0 = events[0].LocalOffsetTarget;
            var n0 = events[0].Normal;
            var va0 = _linearVelocity + GeometryUtils.AnuglarVelocityToLinearVelocity(ri0, _angularVelocity);
            var vb0 = events[0].Target.RigidBody._linearVelocity
                + GeometryUtils.AnuglarVelocityToLinearVelocity(rb0, events[0].Target.RigidBody._angularVelocity);
            float relv0_n = Vector2.Dot(va0 - vb0, n0);
            if (relv0_n >= 0)
            {
                return false;
            }

            var ri1 = events[1].LocalOffsetSrc;
            var rb1 = events[1].LocalOffsetTarget;
            var n1 = events[1].Normal;
            var va1 = _linearVelocity + GeometryUtils.AnuglarVelocityToLinearVelocity(ri1, _angularVelocity);
            var vb1 = events[1].Target.RigidBody._linearVelocity
                + GeometryUtils.AnuglarVelocityToLinearVelocity(rb1, events[1].Target.RigidBody._angularVelocity);
            float relv1_n = Vector2.Dot(va1 - vb1, n1);
            if (relv1_n >= 0)
            {
                return false;
            }
            Matrix2x2 matrix = new Matrix2x2()
            {
                [0, 0] = InvMass + events[0].Target.RigidBody.InvMass
                + Vector2.Dot(GeometryUtils.AnuglarVelocityToLinearVelocity(ri0, (float)(GlobalInverseInertiaTensor
                * GeometryUtils.Cross(ri0, n0))), n0)
                + Vector2.Dot(GeometryUtils.AnuglarVelocityToLinearVelocity(rb0, (float)(events[0].Target.RigidBody.GlobalInverseInertiaTensor
                * GeometryUtils.Cross(rb0, n0))), n0),
                [0, 1] = InvMass * Vector2.Dot(n0, n1) + Vector2.Dot(GeometryUtils.AnuglarVelocityToLinearVelocity(ri0, (float)(GlobalInverseInertiaTensor
                * GeometryUtils.Cross(ri1, n1))), n0),

                [1, 0] = InvMass * Vector2.Dot(n0, n1) + Vector2.Dot(GeometryUtils.AnuglarVelocityToLinearVelocity(ri1, (float)(GlobalInverseInertiaTensor
                * GeometryUtils.Cross(ri0, n0))), n1),
                [1, 1] = InvMass + events[1].Target.RigidBody.InvMass
                + Vector2.Dot(GeometryUtils.AnuglarVelocityToLinearVelocity(ri1, (float)(GlobalInverseInertiaTensor
                * GeometryUtils.Cross(ri1, n1))), n1)
                + Vector2.Dot(GeometryUtils.AnuglarVelocityToLinearVelocity(rb1, (float)(events[1].Target.RigidBody.GlobalInverseInertiaTensor
                * GeometryUtils.Cross(rb1, n1))), n1),
            };
            float C = 0.4f;
            var J = matrix.Inverse().Multiply(new Vector2(-(1 + C) * relv0_n, -(1 + C) * relv1_n));
            if (!J.HasNaNs())
            {
                AddImpluse(J.X * events[0].Normal, ri0, target.RigidBody, - J.X * events[0].Normal, rb0);
                AddImpluse(J.Y * events[1].Normal, ri1, target.RigidBody, - J.Y * events[1].Normal, rb1);
            }
            return true;
        }
        public void ApplyAngularVelocity(float w)
        {
            _angularVelocity += w;
        }



        private void SolveContactImpluse(CollisionEvent2D e, int count, float deltaTime)
        {
            var ri = e.LocalOffsetSrc;
            var rb = e.LocalOffsetTarget;

            var v2 = GeometryUtils.AnuglarVelocityToLinearVelocity(ri, _angularVelocity);
            var va = _linearVelocity + v2;
            var vb = e.Target.RigidBody._linearVelocity
                + GeometryUtils.AnuglarVelocityToLinearVelocity(rb, e.Target.RigidBody._angularVelocity);
            var bias = 0.1f * Math.Max(e.Depth - 0.02f, 0) / deltaTime;
            

            float va_n = Vector2.Dot(va - vb, e.Normal);
            if (MovementType == MovementType.Player)
            {
                _contactNormals.Add(e.Normal);
            }
            if(e.Target.RigidBody.MovementType == MovementType.Player)
            {
                e.Target.RigidBody._contactNormals.Add(-e.Normal);
            }
            if (va_n >= 0)
            {
                e.NormalVelOld = 0;
                return;
            }
            float stiffness = Math.Max(0, (_restitution + e.Target.RigidBody.Restitution) / 2);
            float vnew_n = stiffness * Math.Max(-va_n - 20 * deltaTime, 0);

            // (a × b) × c = (c • a)b - (c • b)a
            double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ri), e.Normal);
            double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), e.Normal);
            double R1 = rAdotN * rAdotN * GlobalInverseInertiaTensor; // Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(ri, (float)(GlobalInverseInertiaTensor
                // * Utils.Cross(ri, e.Normal))), e.Normal);
            double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor;//Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(rb, (float)(e.Target.RigidBody.GlobalInverseInertiaTensor
                                                                       //* Utils.Cross(rb, e.Normal))), e.Normal);
            double J_n = (vnew_n - va_n) / (InvMass + e.Target.RigidBody.InvMass + R1 + R2);
            Vector2 J = (float)J_n * e.Normal / count;// + (float)J_t * va_t_unit;
            e.NormalVelOld = (float)J_n;

            // var offset = e.Position - (_globalCentroid + ri);
            AddImpluse(J, ri, e.Target.RigidBody, -J, rb);
            Debug.Assert(!float.IsNaN(J.X) && !float.IsNaN(J.Y));
        }

        private void SolveFrictionImpluse(CollisionEvent2D e, int count, float deltaTime)
        {
            var ri = e.LocalOffsetSrc;
            var rb = e.LocalOffsetTarget;

            var v2 = GeometryUtils.AnuglarVelocityToLinearVelocity(ri, _angularVelocity);
            var va = _linearVelocity + v2;
            var vb = e.Target.RigidBody._linearVelocity
                + GeometryUtils.AnuglarVelocityToLinearVelocity(rb, e.Target.RigidBody._angularVelocity);

            if (e.NormalVelOld == 0)
            {
                return;
            }

            float vel_n = Vector2.Dot(va - vb, e.Normal);
            Vector2 vt = (va - vb) - vel_n * e.Normal;

            float vel_t = vt.Length();
            float friction = Math.Max(0, (_friction + e.Target.RigidBody.Friction) / 2);


            vt = vt.SafeNormalize(Vector2.Zero);
            if (vt.Length() == 0)
            {
                return;
            }

            _contactTangentVels.Add(-Vector2.Dot(vt, vb) * vt);
            e.Target.RigidBody._contactTangentVels.Add(-Vector2.Dot(vt, va) * vt);
            if (friction == 0)
            {
                return;
            }

            double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ri), vt);
            double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), vt);
            double R1 = rAdotN * rAdotN * GlobalInverseInertiaTensor; // Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(ri, (float)(GlobalInverseInertiaTensor
                                                                      // * Utils.Cross(ri, e.Normal))), e.Normal);
            double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor;//Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(rb, (float)(e.Target.RigidBody.GlobalInverseInertiaTensor
                                                                                        //* Utils.Cross(rb, e.Normal))), e.Normal);
            double effectiveMass = (InvMass + e.Target.RigidBody.InvMass + R1 + R2);

            // (a × b) × c = (c • a)b - (c • b)a
            double J_n = -Math.Min(friction * e.NormalVelOld, vel_t / effectiveMass);
            Vector2 J = (float)J_n * vt / count;// + (float)J_t * va_t_unit;

            AddImpluse(J, ri, e.Target.RigidBody, -J, rb);
            Debug.Assert(!float.IsNaN(J.X) && !float.IsNaN(J.Y));
        }

		/// <summary>
		/// 对接触事件进行响应
		/// </summary>
		/// <param name="events"></param>
		/// <param name="deltaTime"></param>
        public void RespondToEvents(List<CollisionEvent2D> events, float deltaTime)
        {
            foreach (var e in events)
            {
                SolveContactImpluse(e, events.Count, deltaTime);
            }
            ApplyImpluses();
            foreach (var e in events)
            {
                SolveFrictionImpluse(e, events.Count, deltaTime);
                MatchAwakeState(e.Target.RigidBody, deltaTime);
            }
            ApplyImpluses();
        }

        public void Integrate(float deltaTime)
        {
            if (MovementType == MovementType.Static)
            {
                return;
            }
            _bindObject.Rotation += deltaTime * _angularVelocity;
            _globalCentroid += deltaTime * _linearVelocity;
            _bindObject.Position = _globalCentroid;
        }

        //public void ResolveImpluse(float deltaTime)
        //{
        //    if (_impluses.Count == 0)
        //    {
        //        return;
        //    }
        //    //_bindObject.Rotation = _bindObject.OldRotation;
        //    //_globalCentroid = _bindObject.OldPosition;
        //    _impluses.Sort((a, b) =>
        //    {
        //        return a.Time.CompareTo(b.Time);
        //    });
        //    float lastTime = 0;
        //    int sameTimeCount = 0;
        //    for (int i = 0; i < _impluses.Count; i++)
        //    {
        //        sameTimeCount++;
        //        if (i == _impluses.Count - 1 || _impluses[i].Time != _impluses[i + 1].Time)
        //        {
        //            float dt = (_impluses[i].Time - lastTime);

        //            Vector2 linearVelocityChange = Vector2.Zero;
        //            float angularVelocityChange = 0;
        //            Vector2 normalAvg = Vector2.Zero;

        //            //linearVelocityChange /= sameTimeCount;
        //            //angularVelocityChange /= sameTimeCount;
        //            //normalAvg /= sameTimeCount;
        //            //normalAvg = normalAvg.SafeNormalize(Vector2.Zero);



        //            var resImpluses = ResolveConstrains();
        //            for (int j = i - sameTimeCount + 1; j <= i; j++)
        //            {
        //                linearVelocityChange += 1.0f / _mass * resImpluses[j] * _impluses[j].Normal;
        //                angularVelocityChange += (float)(Utils.Cross(_impluses[j].RelativePosition,
        //                    resImpluses[j] *_impluses[j].Normal * (float)_globalInverseInertiaTensor));
        //            }


        //            //var penetration = Math.Max(0, -Vector2.Dot(_linearVelocity * deltaTime, normalAvg));
        //            //_linearVelocity += normalAvg * penetration * 0.7f;

        //            deltaTime -= dt;
        //            lastTime = _impluses[i].Time;
        //            sameTimeCount = 0;
        //        }
        //    }
        //    _impluses.Clear();
        //}

        //private List<float> ResolveConstrains()
        //{
        //    List<float> impluseArray = new List<float>();
        //    List<float> impluseArrayTemp = new List<float>();
        //    for (int k = 0; k < _impluses.Count; k++)
        //    {
        //        impluseArray.Add(_impluses[k].Impluse.Length());
        //        impluseArrayTemp.Add(0f);
        //    }
        //    for (int iter = 0; iter < 8; iter++)
        //    {
        //        int sameSourceCount = 0;
        //        for (int k = 0; k < _impluses.Count; k++)
        //        {
        //            var ri0 = _impluses[k].CollisionEvent.LocalOffsetSrc;
        //            var rb0 = _impluses[k].CollisionEvent.LocalOffsetTarget;
        //            var n0 = _impluses[k].CollisionEvent.Normal;
        //            var va0 = _impluses[k].CollisionEvent.Source.RigidBody.LinearVelocity
        //                + Utils.AnuglarVelocityToLinearVelocity(ri0, _impluses[k].CollisionEvent.Source.RigidBody.AngularVelocity);
        //            var vb0 = _impluses[k].CollisionEvent.Target.RigidBody.LinearVelocity
        //                + Utils.AnuglarVelocityToLinearVelocity(rb0, _impluses[k].CollisionEvent.Target.RigidBody.AngularVelocity);
        //            float relv0_n = Vector2.Dot(va0 - vb0, n0);
        //            float relv0_plus = -(1 + 0.4f) * Vector2.Dot(va0 - vb0, n0);

        //            float Kj = _impluses[k].CollisionEvent.Source.RigidBody.InvMass + _impluses[k].CollisionEvent.Target.RigidBody.InvMass
        //                + Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(ri0, (float)(_impluses[k].CollisionEvent.Source.RigidBody.GlobalInverseInertiaTensor
        //                * Utils.Cross(ri0, n0))), n0)
        //                + Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(rb0, (float)(_impluses[k].CollisionEvent.Target.RigidBody.GlobalInverseInertiaTensor
        //                * Utils.Cross(rb0, n0))), n0);

        //            float KLU = 0f;
        //            for (int l = 0; l < _impluses.Count; l++)
        //            {
        //                if (l == k)
        //                {
        //                    continue;
        //                }
        //                var ri1 = _impluses[l].CollisionEvent.LocalOffsetSrc;
        //                var rb1 = _impluses[l].CollisionEvent.LocalOffsetTarget;
        //                var n1 = _impluses[l].CollisionEvent.Normal;

        //                KLU += (_impluses[k].CollisionEvent.Source.RigidBody.InvMass
        //                    * Vector2.Dot(n0, n1) + Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(ri0,
        //                    (float)(_impluses[k].CollisionEvent.Source.RigidBody.GlobalInverseInertiaTensor
        //                    * Utils.Cross(ri1, n1))), n0)) * impluseArray[l];
        //            }
        //            impluseArrayTemp[k] = 1 / Kj * (relv0_plus - KLU);
        //        }
        //        sameSourceCount = 0;
        //        // Deep copy
        //        for (int k = 0; k < impluseArray.Count; k++)
        //        {
        //            impluseArray[k] = impluseArrayTemp[k];
        //        }
        //    }

        //    return impluseArray;
        //}

		public void AddForce(Vector2 force, Vector2 relPos)
		{
			_force += force;
			_torque += GeometryUtils.Cross(relPos, force);
		}

		/// <summary>
		/// 把冲量数据累加到这个刚体上
		/// </summary>
		/// <param name="J"></param>
		/// <param name="relativePos"></param>
		/// <param name="other"></param>
		/// <param name="J2"></param>
		/// <param name="relativePos2"></param>
        public void AddImpluse(Vector2 J, Vector2 relativePos, RigidBody2D other, Vector2 J2, Vector2 relativePos2)
        {
            var entry = new ImpluseEntry()
            {
                Source = this,
                Target = other,
                ImpluseSource = J,
                RelativePositionSource = relativePos,
                ImpluseTarget = J2,
                RelativePositionTarget = relativePos2
            };
            _impluses.Add(entry);
        }

		/// <summary>
		/// 对累加的冲量进行应用，更新各种速度
		/// </summary>
        public void ApplyImpluses()
        {
            if (_impluses.Count > 0)
            {
                foreach (var imp in _impluses)
                {
                    if (imp.Source.MovementType == MovementType.Dynamic || imp.Source.MovementType == MovementType.Player)
                    {
                        _linearVelocity += 1.0f / _mass * imp.ImpluseSource;
                        if (imp.Source.MovementType != MovementType.Player)
                        {
                            _angularVelocity += GeometryUtils.Cross(imp.RelativePositionSource, imp.ImpluseSource * (float)_globalInverseInertiaTensor);
                        }
                    }
                    if (imp.Target.MovementType == MovementType.Dynamic || imp.Target.MovementType == MovementType.Player)
                    {
                        imp.Target._linearVelocity += imp.Target.InvMass * imp.ImpluseTarget;
                        if (imp.Target.MovementType != MovementType.Player)
                        {
                            imp.Target._angularVelocity += GeometryUtils.Cross(imp.RelativePositionTarget, imp.ImpluseTarget * (float)imp.Target.GlobalInverseInertiaTensor);
                        }
                    }

                }
            }

            Debug.Assert(!float.IsNaN(_linearVelocity.X) && !float.IsNaN(_linearVelocity.Y));
            Debug.Assert(!float.IsNaN(_angularVelocity));
            StabilizeBody();
            _impluses.Clear();
        }

		/// <summary>
		/// 立即施加一个冲量
		/// </summary>
		/// <param name="J"></param>
		/// <param name="relativePos"></param>
        public void AddImpluseImmediate(Vector2 J, Vector2 relativePos, RigidBody2D other, Vector2 J2, Vector2 relativePos2)
        {
            if (MovementType != MovementType.Static && MovementType != MovementType.Kinematic)
            {
                _linearVelocity += 1.0f / _mass * J;
                Debug.Assert(!float.IsNaN(_linearVelocity.X) && !float.IsNaN(_linearVelocity.Y));
                _angularVelocity += (float)(GeometryUtils.Cross(relativePos,
                    J * (float)GlobalInverseInertiaTensor));
                Debug.Assert(!float.IsNaN(_angularVelocity));
            }

            if (other.MovementType != MovementType.Static && MovementType != MovementType.Kinematic)
            {
                other._linearVelocity += other.InvMass * J2;
                other._angularVelocity += (float)(GeometryUtils.Cross(relativePos2,
                    J2 * (float)other.GlobalInverseInertiaTensor));
            }
        }

        public void MoveBody(Vector2 dir, float deltaTime)
        {
            _globalCentroid += dir;
            // _linearVelocity = (_globalCentroid - ParentObject.OldPosition) / deltaTime;
        }

        public void MatchAwakeState(RigidBody2D other, float deltaTime)
        {
            if (other.MovementType == MovementType.Static)
            {
                return;
            }
            if (!_isAwake && !other._isAwake)
            {
                return;
            }
            if (_isAwake && !other._isAwake)
            {
                other.IsAwake = true;
            }
            else if (!_isAwake && other._isAwake)
            {
                IsAwake = true;
            }
        }

        public void CheckAwake(float deltaTime)
        {
            if (MovementType == MovementType.Static || MovementType == MovementType.Kinematic)
            {
                return;
            }
            double bias = Math.Pow(0.6, deltaTime);
            _motion = bias * _motion + (1 - bias) * (_linearVelocity.LengthSquared() + 
                3 * _angularVelocity * _angularVelocity);

            if (_motion > 16 * SleepEPS)
                _motion = 16 * SleepEPS;

            if (_motion < SleepEPS)
            {
                IsAwake = false;
            }
        }
    }
}
