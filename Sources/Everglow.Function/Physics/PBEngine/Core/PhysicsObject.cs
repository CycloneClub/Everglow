using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.Physics.PBEngine.Core
{
	/// <summary>
	/// 物理对象的容器，可以容纳Rigidbody，Collider等组件
	/// 此外，它还是物理模拟的主体
	/// </summary>
	public class PhysicsObject
	{
		/// <summary>
		/// 物理对象的几何组件，用于碰撞检测和质心计算等
		/// </summary>
		public Collider2D Collider
		{
			get => _collider;
		}

		/// <summary>
		/// 物理对象的刚体物理组件，用于存储和模拟刚体数据
		/// </summary>
		public RigidBody2D RigidBody
		{
			get => _rigidBody;
		}

		/// <summary>
		/// 物体的质心位置
		/// </summary>
		public Vector2 Position
		{
			get => _rigidBody.CentroidWorldSpace;
			set => _rigidBody.CentroidWorldSpace = value;
		}

		/// <summary>
		/// 物体的旋转，绕着质心
		/// </summary>
		public float Rotation
		{
			get => _rotation;
			set
			{
				_rotation = value;
				_cachedRotationalMatrix = Matrix2x2.CreateRotationMatrix(_rotation);
			}
		}

		internal Matrix2x2 CachedRotationalMatrix
		{
			get => _cachedRotationalMatrix;
		}

		private Matrix2x2 _cachedRotationalMatrix;

		/// <summary>
		/// 物体在迭代开始的时候的位置
		/// </summary>
		public Vector2 OldPosition
		{
			get => _oldPos;
			set => _oldPos = value;
		}

		/// <summary>
		/// 物体在迭代开始的时候的旋转
		/// </summary>
		public float OldRotation
		{
			get => _oldRot;
			set => _oldRot = value;
		}

		/// <summary>
		/// 物体的GUID
		/// </summary>
		public int GUID
		{
			get => _guid;
			set => _guid = value;
		}

		/// <summary>
		/// 物体的碰撞组名字
		/// </summary>
		public string Tag
		{
			get => _tag;
			set => _tag = value;
		}

		public bool IsActive
		{
			get => _isActive;
			set => _isActive = value;
		}

		private Collider2D _collider;
		private RigidBody2D _rigidBody;
		private Vector2 _oldPos;
		private float _rotation;
		private float _oldRot;
		private int _guid;
		private string _tag;
		private bool _isActive;

		public PhysicsObject(Collider2D collider, RigidBody2D rigidBody)
		{
			_collider = collider;
			_rigidBody = rigidBody;
			_tag = "Default";
			_isActive = true;

			if (_rigidBody == null)
			{
				_rigidBody = new RigidBody2D(1)
				{
					MovementType = MovementType.Static,
					UseGravity = false,
				};
			}

			Rotation = 0;
			Position = Vector2.Zero;

			_collider.ParentObject = this;
			_rigidBody.ParentObject = this;
		}

		public void Initialize()
		{
			_rigidBody?.Initialize();
		}

		public void RecordOldState()
		{
			_oldPos = Position;
			_oldRot = _rotation;
		}

		public List<(Vector2, Color)> GetWireFrameWires()
		{
			var wires_color = new List<(Vector2, Color)>();
			List<Vector2> wires = _collider.GetWireFrameWires();
			for (int i = 0; i < wires.Count; i++)
			{
				wires_color.Add((_cachedRotationalMatrix.Multiply(wires[i]) + Position, RigidBody.IsAwake ? Color.White : Color.Gray));
			}
			return wires_color;
		}

		///// <summary>
		///// Override to add vertex only, no sprite batch.
		///// </summary>
		///// <returns></returns>
		//public virtual List<Vertex2D> Draw()
		//{
		//	return new List<Vertex2D>();
		//}

		public void CleanThisFrame(float deltaTime)
		{
			_rigidBody?.CleanInformationThisFrame(deltaTime);
		}

		public void CleanThisSubstep(float deltaTime)
		{
			_rigidBody?.CleanInformationSubstep(deltaTime);
			Rotation = MathHelper.WrapAngle(Rotation);
		}

		public void Update(float deltaTime)
		{
			_rigidBody?.Update(deltaTime);
		}

		public void ApplyGravity(Vector2 gravity)
		{
			if (!_isActive)
			{
				return;
			}
			if (_rigidBody != null)
			{
				if (_rigidBody.UseGravity)
				{
					_rigidBody.ApplyForce(gravity * _rigidBody.Mass);
				}
			}
		}

		public Vector2 LocalToWorldPos(Vector2 localPos)
		{
			return _rigidBody.CentroidWorldSpace + _cachedRotationalMatrix.Multiply(localPos);
		}
	}
}