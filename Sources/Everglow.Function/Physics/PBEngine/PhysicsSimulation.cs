using Everglow.Commons.DataStructures;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Constraints;
using Everglow.Commons.Vertex;
using Terraria;

namespace Everglow.Commons.Physics.PBEngine
{
	/// <summary>
	/// 用于物理模拟的世界，可以设置各种世界属性，添加和管理物理对象
	/// 其中最重要的模拟部分也是由这个类完成
	/// </summary>
	public class PhysicsSimulation
	{
		public const double EPS = 1e-6;

		public int GAUSS_SEIDEL_ITERS
		{
			get => 8;
		}

		private List<PhysicsObject> _objects;
		private LinkedList<int> _objectFreeList;
		private int _maximumAllocatedId;
		private List<Constraint> _constraints;
		private IBroadPhase _broadPhase;
		private float _gravity;

		private Stopwatch _stopwatchPreIntegration;
		private Stopwatch _stopwatchBroadPhase;
		private Stopwatch _stopwatchNarrowPhase;
		private readonly double _ticksPerMillisecond;

		private long _numBroadPhasePairs;
		private long _numNarrowPhasePairs;

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

		/// <summary>
		/// 粗碰撞检测得到的碰撞对数量
		/// </summary>
		public long NumOfBroadPhasePairs
		{
			get => _numBroadPhasePairs;
		}

		/// <summary>
		/// 细碰撞检测实际碰撞出现的物体对数量
		/// </summary>
		public long NumOfNarrowPhasePairs
		{
			get => _numNarrowPhasePairs;
		}

		public float Gravity
		{
			get => _gravity;
		}

		public IBroadPhase BroadPhaseCollisionDetector
		{
			get => _broadPhase;
		}

		public PhysicsSimulation()
			: this(CollisionGraph.DefaultGraph)
		{
		}

		public void ClearPhysicsObjects()
		{
			foreach (PhysicsObject pobj in _objects)
			{
				if(pobj.IsActive)
				{
					RemoveObject(pobj);
				}
			}
		}

		public PhysicsSimulation(CollisionGraph graph)
		{
			_objectFreeList = new LinkedList<int>();
			_objects = new List<PhysicsObject>();
			_maximumAllocatedId = 0;
			_constraints = new List<Constraint>();
			_broadPhase = new HashGridMethod(graph);

			_gravity = 9.8f;

			_stopwatchPreIntegration = new Stopwatch();
			_stopwatchBroadPhase = new Stopwatch();
			_stopwatchNarrowPhase = new Stopwatch();

			_ticksPerMillisecond = Stopwatch.Frequency / 1000.0;

			_numBroadPhasePairs = 0;
			_numNarrowPhasePairs = 0;
		}

		/// <summary>
		/// 向世界添加物理对象，如果对象没有刚体部件，那么认为是静态物体
		/// </summary>
		/// <param name="pobj"></param>
		public void AddPhysicsObject(PhysicsObject pobj)
		{
			//Main.NewText(pobj.RigidBody.Mass, Color.Red);
			pobj.GUID = AllocateGUID();
			if (pobj.GUID == _objects.Count)
			{
				_objects.Add(pobj);
			}
			else
			{
				_objects[pobj.GUID] = pobj;
			}
			pobj.Initialize();
		}

		/// <summary>
		/// 向物理世界添加约束对象
		/// </summary>
		/// <param name="constrain"></param>
		public void AddConstrain(Constraint constrain)
		{
			_constraints.Add(constrain);
		}

		/// <summary>
		/// Initialize physical object properties
		/// </summary>
		public void Initialize()
		{
			foreach (PhysicsObject pobj in _objects)
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
			_stopwatchPreIntegration.Reset();
			_stopwatchBroadPhase.Reset();
			_stopwatchNarrowPhase.Reset();
			CleanUpThisFrame(deltaTime);
			float dt = deltaTime / GAUSS_SEIDEL_ITERS;
			for (int i = 0; i < GAUSS_SEIDEL_ITERS; i++)
			{
				_stopwatchPreIntegration.Start();
				PreIntegration(dt);
				_stopwatchPreIntegration.Stop();

				Resolve(dt, _stopwatchBroadPhase, _stopwatchNarrowPhase);
			}

			// foreach (var pobj in _objects)
			// {
			//    Main.NewText(pobj.RigidBody.LinearVelocity);
			// }
		}

		public List<(Vector2, Color)> GetCurrentWireFrames()
		{
			List<(Vector2, Color)> result = new List<(Vector2, Color)>();
			foreach (PhysicsObject pobj in _objects)
			{
				if (!pobj.IsActive)
				{
					continue;
				}

				result.AddRange(pobj.GetWireFrameWires());
			}
			foreach (Constraint joint in _constraints)
			{
				result.AddRange(joint.GetDrawMesh());
			}
			return result;
		}

		//public List<CustomPhysicsObject> GetCustomPhysicsObjects()
		//{
		//	List<CustomPhysicsObject> result = new List<CustomPhysicsObject>();
		//	foreach (PhysicsObject pobj in _objects)
		//	{
		//		if (!pobj.IsActive)
		//		{
		//			continue;
		//		}
		//		if(pobj is CustomPhysicsObject cobj)
		//		{
		//			result.Add(cobj);
		//		}
		//	}
		//	return result;
		//}

		/// <summary>
		/// 进行预积分，先模拟一个步长
		/// </summary>
		/// <param name="deltaTime"></param>
		private void PreIntegration(float deltaTime)
		{
			CleanUpThisSubstep(deltaTime);
			foreach (PhysicsObject pobj in _objects)
			{
				if (!pobj.IsActive)
				{
					continue;
				}
				pobj.ApplyGravity(new Vector2(0, -_gravity));
			}
			foreach (var constrain in _constraints)
			{
				constrain.ApplyForce(deltaTime);
			}
			foreach (PhysicsObject pobj in _objects)
			{
				if (!pobj.IsActive)
				{
					continue;
				}
				pobj.RecordOldState();
				pobj.Update(deltaTime);
			}
			foreach (var constrain in _constraints)
			{
				constrain.Apply(deltaTime);
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
			List<PhysicsObject> activeObjects = _objects.Where(obj => obj.IsActive).ToList();
			_broadPhase.Prepare(activeObjects, deltaTime);
			var pairs = _broadPhase.GetCollisionPairs(deltaTime);
			_numBroadPhasePairs = pairs.Count;
			broadPhase.Stop();

			narrowPhase.Start();
			_numNarrowPhasePairs = 0;
			List<CollisionEvent2D> contacts = new List<CollisionEvent2D>();
			foreach (var pair in pairs)
			{
				if (pair.Key.TestCollisionCondition(pair.Value, deltaTime, out CollisionInfo info))
				{
					// 非线性投影：预先移开以获得contact points
					float weightA = info.Source.RigidBody.InvMass / (info.Source.RigidBody.InvMass + info.Target.RigidBody.InvMass);
					float weightB = info.Target.RigidBody.InvMass / (info.Source.RigidBody.InvMass + info.Target.RigidBody.InvMass);

					info.Source.RigidBody.MoveBody(info.Normal * weightA * info.Depth, deltaTime);
					info.Target.RigidBody.MoveBody(-info.Normal * weightB * info.Depth, deltaTime);

					List<CollisionEvent2D> events;
					pair.Key.GetContactInfo(info, deltaTime, out events);
					contacts.AddRange(events);

					//// 非线性投影：回溯，计算实际投影后姿态
					// foreach (var e in events)
					// {
					//    double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(e.LocalOffsetSrc), e.Normal);
					//    double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(e.LocalOffsetTarget), e.Normal);
					//    double R1 = rAdotN * rAdotN * e.Source.RigidBody.GlobalInverseInertiaTensor;
					//    double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor;
					//    double effectiveMass = R1 + R2 + e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass;

					// double linearMoveA = info.Depth * e.Source.RigidBody.InvMass / effectiveMass;
					//    double linearMoveB = -info.Depth * e.Target.RigidBody.InvMass / effectiveMass;
					//    double angularMoveA = info.Depth * R1 / effectiveMass;
					//    double angularMoveB = -info.Depth * R2 / effectiveMass;

					// info.Source.RigidBody.MoveBody(info.Normal * (float)linearMoveA, deltaTime);
					//    info.Target.RigidBody.MoveBody(info.Normal * (float)linearMoveB, deltaTime);

					// double implusePerMoveA = GeometryUtils.Cross(e.LocalOffsetSrc, e.Normal) * e.Source.RigidBody.GlobalInverseInertiaTensor;
					//    double implusePerMoveB = GeometryUtils.Cross(e.LocalOffsetTarget, e.Normal) * e.Target.RigidBody.GlobalInverseInertiaTensor;
					//    if (R1 > 0)
					//    {
					//        info.Source.Rotation += (float)(angularMoveA * implusePerMoveA / R1);
					//    }
					//    if (R2 > 0)
					//    {
					//        info.Target.Rotation += (float)(angularMoveB * implusePerMoveB / R2);
					//    }
					// }

					// ApplyConstrains(events, pair.Key.ParentObject, deltaTime);
					_numNarrowPhasePairs++;
				}
			}

			contacts.Sort((a, b) =>
			{
				return -a.Depth.CompareTo(b.Depth);
			});

			foreach (var contact in contacts)
			{
				float stiffness = Math.Max(0, (contact.Source.RigidBody.Restitution + contact.Target.RigidBody.Restitution) / 2);
				SolveContact_Weak(contact, stiffness, true, deltaTime);
			}
			for (int i = 0; i < 16; i++)
			{
				bool stable = true;
				foreach (var contact in contacts)
				{
					if (!SolveContact_Weak(contact, 0, false, deltaTime))
					{
						stable = false;
					}
				}
				if (stable)
				{
					break;
				}
			}
			foreach (var contact in contacts)
			{
				SolveFriction_Weak(contact, deltaTime);
				contact.Source.RigidBody.MatchAwakeState(contact.Target.RigidBody, deltaTime);
			}

			foreach (PhysicsObject pobj in _objects)
			{
				if (!pobj.IsActive)
				{
					continue;
				}
				pobj.RigidBody.CheckAwake(deltaTime);
			}
			narrowPhase.Stop();
		}

		private void SolveFriction_Weak(CollisionEvent2D e, float deltaTime)
		{
			var ri = e.LocalOffsetSrc;
			var rb = e.LocalOffsetTarget;

			var va = e.Source.RigidBody.LinearVelocity + GeometryUtils.AngularVelocityToLinearVelocity(ri, e.Source.RigidBody.AngularVelocity);
			var vb = e.Target.RigidBody.LinearVelocity
				+ GeometryUtils.AngularVelocityToLinearVelocity(rb, e.Target.RigidBody.AngularVelocity);

			if (e.NormalVelOld == 0)
			{
				return;
			}
			float vrel_n = Vector2.Dot(va - vb, e.Normal);
			Vector2 vt = (va - vb) - vrel_n * e.Normal;

			float vel_t = vt.Length();
			float friction = Math.Max(0, (e.Source.RigidBody.Friction + e.Target.RigidBody.Friction) / 2);

			vt = vt.SafeNormalize(Vector2.Zero);
			if (vt.Length() == 0)
			{
				return;
			}
			double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ri), vt);
			double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), vt);
			double R1 = rAdotN * rAdotN * e.Source.RigidBody.GlobalInverseInertiaTensor; // Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(ri, (float)(GlobalInverseInertiaTensor

			// * Utils.Cross(ri, e.Normal))), e.Normal);
			double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor; // Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(rb, (float)(e.Target.RigidBody.GlobalInverseInertiaTensor

			// * Utils.Cross(rb, e.Normal))), e.Normal);
			double effectiveMass = e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass + R1 + R2;

			// (a × b) × c = (c • a)b - (c • b)a
			double J_n = -Math.Min(friction * e.NormalVelOld, vel_t / effectiveMass);
			Vector2 J = (float)J_n * vt; // + (float)J_t * va_t_unit;

			// var offset = e.Position - (_globalCentroid + ri);
			e.Source.RigidBody.AddImpulseImmediate(J, ri, e.Target.RigidBody, -J, rb);
			Debug.Assert(!float.IsNaN(J.X) && !float.IsNaN(J.Y));
		}

		private bool SolveContact_Weak(CollisionEvent2D e, float restitution, bool initial, float deltaTime)
		{
			var ri = e.LocalOffsetSrc;
			var rb = e.LocalOffsetTarget;

			var va = e.Source.RigidBody.LinearVelocity + GeometryUtils.AngularVelocityToLinearVelocity(ri, e.Source.RigidBody.AngularVelocity);
			var vb = e.Target.RigidBody.LinearVelocity
				+ GeometryUtils.AngularVelocityToLinearVelocity(rb, e.Target.RigidBody.AngularVelocity);

			float vrel_n = Vector2.Dot(va - vb, e.Normal);

			if (vrel_n >= 0)
			{
				return true;
			}

			if (initial)
			{
				if (e.Source.RigidBody.MovementType == MovementType.Player)
				{
					e.Source.RigidBody.ContactNormals.Add(e.Normal);
				}
				if (e.Target.RigidBody.MovementType == MovementType.Player)
				{
					e.Target.RigidBody.ContactNormals.Add(-e.Normal);
				}
			}

			float vnew_n = restitution * Math.Max(-vrel_n, 0);

			double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ri), e.Normal);
			double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), e.Normal);
			double R1 = rAdotN * rAdotN * e.Source.RigidBody.GlobalInverseInertiaTensor;
			double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor;
			double J_n = (vnew_n - vrel_n) / (e.Source.RigidBody.InvMass + e.Target.RigidBody.InvMass + R1 + R2);
			Vector2 J = (float)J_n * e.Normal; // + (float)J_t * va_t_unit;

			e.NormalVelOld += (float)J_n;

			// var offset = e.Position - (_globalCentroid + ri);
			e.Source.RigidBody.AddImpulseImmediate(J, ri, e.Target.RigidBody, -J, rb);
			Debug.Assert(!float.IsNaN(J.X) && !float.IsNaN(J.Y));

			return vrel_n > -0.1f;
		}

		private Dictionary<int, List<KeyValuePair<Collider2D, Collider2D>>> GroupCollisions(List<KeyValuePair<Collider2D, Collider2D>> pairs)
		{
			Dictionary<int, List<KeyValuePair<Collider2D, Collider2D>>> groupedPairs = new Dictionary<int, List<KeyValuePair<Collider2D, Collider2D>>>();
			UnionFind uf = new UnionFind(_objects.Count);
			foreach (var pair in pairs)
			{
				uf.Union(pair.Key.ParentObject.GUID, pair.Value.ParentObject.GUID);
			}
			foreach (var pair in pairs)
			{
				int group = uf.Find(pair.Key.ParentObject.GUID);
				if (groupedPairs.ContainsKey(group))
				{
					groupedPairs[group].Add(pair);
				}
				else
				{
					groupedPairs.Add(group, new List<KeyValuePair<Collider2D, Collider2D>>() { pair });
				}
			}
			return groupedPairs;
		}

		private void ApplyConstrains(List<CollisionEvent2D> events, PhysicsObject pobj, float deltaTime)
		{
			if (deltaTime == 0)
			{
				return;
			}
			pobj.RigidBody.RespondToEvents(events, deltaTime);
		}

		private void CleanUpThisFrame(float deltaTime)
		{
			foreach (PhysicsObject pobj in _objects)
			{
				pobj.CleanThisFrame(deltaTime);
			}
		}

		private void CleanUpThisSubstep(float deltaTime)
		{
			foreach (PhysicsObject pobj in _objects)
			{
				if (!pobj.IsActive)
				{
					continue;
				}
				pobj.CleanThisSubstep(deltaTime);
			}
		}

		public void RemoveObject(PhysicsObject obj)
		{
			_objectFreeList.AddLast(obj.GUID);
			obj.IsActive = false;
		}

		private void RemoveObject(int GUID)
		{
			Debug.Assert(_objects[GUID].IsActive);
			RemoveObject(_objects[GUID]);
		}

		private List<int> AllocateGUIDs(int size)
		{
			List<int> results = new List<int>();
			while (size > 0)
			{
				if (_objectFreeList.Count > 0)
				{
					results.Add(_objectFreeList.First());
					_objectFreeList.RemoveFirst();
				}
				else
				{
					results.Add(_maximumAllocatedId++);
				}
			}
			return results;
		}

		private int AllocateGUID()
		{
			if (_objectFreeList.Count == 0)
			{
				return _maximumAllocatedId++;
			}
			int id = _objectFreeList.First();
			_objectFreeList.RemoveFirst();
			return id;
		}
	}
}