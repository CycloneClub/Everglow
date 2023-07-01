using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Constrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine
{
	/// <summary>
	/// 用于物理模拟的世界，可以设置各种世界属性，添加和管理物理对象
	/// 其中最重要的模拟部分也是由这个类完成
	/// </summary>
    public class PhysicsSimulation
    {
		public const double EPS = 1e-6;
		public const int GAUSS_SEIDEL_ITERS = 8;

		private List<PhysicsObject> _dynamicPhysObjects;
        private List<PhysicsObject> _staticPhysObjects;
        private List<Constrain> _constrains;
        private BroadPhase _broadPhase;
        private float _gravity;

        private Stopwatch _stopwatchPreIntegration;
        private Stopwatch _stopwatchBroadPhase;
        private Stopwatch _stopwatchNarrowPhase;
        private readonly double _ticksPerMillisecond;

		/// <summary>
		/// 用于性能检测，对各个阶段的耗时进行统计
		/// </summary>
        public double MeasuredPreIntegrationTimeInMs
        {
            get => _stopwatchPreIntegration.ElapsedTicks / _ticksPerMillisecond;
        }
        public double MeasuredBroadPhaseTimeInMs
        {
            get => _stopwatchBroadPhase.ElapsedTicks / _ticksPerMillisecond;
        }
        public double MeasuredNarrowPhaseTimeInMs
        {
            get => _stopwatchNarrowPhase.ElapsedTicks / _ticksPerMillisecond;
        }

        public PhysicsSimulation()
        {
            _dynamicPhysObjects = new List<PhysicsObject>();
            _staticPhysObjects = new List<PhysicsObject>();
            _constrains = new List<Constrain>();
            _broadPhase = new BruteForceDetect(CollisionGraph.DefualtGraph);
            _gravity = 9.8f;

            _stopwatchPreIntegration = new Stopwatch();
            _stopwatchBroadPhase = new Stopwatch();
            _stopwatchNarrowPhase = new Stopwatch();

            _ticksPerMillisecond = Stopwatch.Frequency / 1000.0;
        }

        public PhysicsSimulation(CollisionGraph graph)
            : this()
        {
            _broadPhase = new BruteForceDetect(graph);
        }

		/// <summary>
		/// 向世界添加物理对象，如果对象没有刚体部件，那么认为是静态物体
		/// </summary>
		/// <param name="pobj"></param>
        public void AddPhysicsObject(PhysicsObject pobj)
        {
            pobj.GUID = _dynamicPhysObjects.Count + _staticPhysObjects.Count;
            if (pobj.RigidBody.MovementType == MovementType.Static)
            {
                _staticPhysObjects.Add(pobj);
            }
            else
            {
                _dynamicPhysObjects.Add(pobj);
            }
            pobj.Initialize();
        }

		/// <summary>
		/// 向物理世界添加约束对象
		/// </summary>
		/// <param name="constrain"></param>
        public void AddConstrain(Constrain constrain)
        {
            _constrains.Add(constrain);
        }

        /// <summary>
        /// Initialize physical object properties
        /// </summary>
        public void Initialize()
        {
            foreach (PhysicsObject pobj in _dynamicPhysObjects)
            {
                pobj.Initialize();
            }
        }

		/// <summary>
		/// 模拟一个完整步长，内部会分为多个子步长
		/// </summary>
		/// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            foreach (PhysicsObject pobj in _dynamicPhysObjects)
            {
                pobj.ApplyGravity(new Vector2(0, -_gravity));
            }

            _stopwatchPreIntegration.Reset();
            _stopwatchBroadPhase.Reset();
            _stopwatchNarrowPhase.Reset();
            float dt = deltaTime / GAUSS_SEIDEL_ITERS;
            for (int i = 0; i < GAUSS_SEIDEL_ITERS; i++)
            {
                _stopwatchPreIntegration.Start();
                PreIntegration(dt);
                _stopwatchPreIntegration.Stop();

                Resolve(dt, _stopwatchBroadPhase, _stopwatchNarrowPhase);
            }
            CleanThisFrame();
        }

        public void CleanThisFrame()
        {
            CleanUp();
        }

        public List<Vector2> GetCurrentWireFrames()
        {
            List<Vector2> result = new List<Vector2>();
            foreach (PhysicsObject pobj in _dynamicPhysObjects)
            {
                result.AddRange(pobj.GetWireFrameWires());
            }
            foreach (PhysicsObject pobj in _staticPhysObjects)
            {
                result.AddRange(pobj.GetWireFrameWires());
            }
            foreach (Constrain joint in _constrains)
            {
                result.AddRange(joint.GetDrawMesh());
            }
            return result;
        }

		/// <summary>
		/// 进行预积分，先模拟一个步长
		/// </summary>
		/// <param name="deltaTime"></param>
        private void PreIntegration(float deltaTime)
        {
			foreach (var joint in _constrains)
			{
				joint.Apply(deltaTime);
			}
			foreach (PhysicsObject pobj in _staticPhysObjects)
            {
                pobj.RecordOldState();
            }
            foreach (PhysicsObject pobj in _dynamicPhysObjects)
            {
                pobj.RecordOldState();
                pobj.Update(deltaTime);
            }
        }

		/// <summary>
		/// 解算所有物体的约束条件，并且做出响应
		/// </summary>
		/// <param name="deltaTime"></param>
		/// <param name="broadPhase"></param>
		/// <param name="narrowPhase"></param>
        private void Resolve(float deltaTime, Stopwatch broadPhase, Stopwatch narrowPhase)
        {
            broadPhase.Start();
            _broadPhase.Prepare(_dynamicPhysObjects.Concat(_staticPhysObjects).ToList(), deltaTime);
            var pairs = _broadPhase.GetCollisionPairs(deltaTime);
            broadPhase.Stop();

            narrowPhase.Start();
            foreach (var pair in pairs)
            {
                CollisionInfo info;
                if (pair.Key.TestCollisionCondition(pair.Value, deltaTime, out info))
                {
                    float weightA = info.Source.RigidBody.InvMass / (info.Source.RigidBody.InvMass + info.Target.RigidBody.InvMass);
                    float weightB = info.Target.RigidBody.InvMass / (info.Source.RigidBody.InvMass + info.Target.RigidBody.InvMass);
                    //if (weightA != 0 && weightB != 0)
                    //    weightA = weightB = 0.5f;

                    info.Source.RigidBody.MoveBody(info.Normal * weightA * info.Depth, deltaTime);
                    info.Target.RigidBody.MoveBody(-info.Normal * weightB * info.Depth, deltaTime);

                    List<CollisionEvent2D> events;
                    pair.Key.GetContactInfo(info, deltaTime, out events);
                    ApplyConstrains(events, pair.Key.ParentObject, deltaTime);
                }
            }
            narrowPhase.Stop();
        }

        private void ApplyConstrains(List<CollisionEvent2D> events, PhysicsObject pobj, float deltaTime)
        {
            if (deltaTime == 0)
            {
                return;
            }
            pobj.RigidBody.RespondToEvents(events, deltaTime);
        }
        private void CleanUp()
        {
            foreach (PhysicsObject pobj in _dynamicPhysObjects)
            {
                pobj.CleanUp();
            }
        }
    }
}
