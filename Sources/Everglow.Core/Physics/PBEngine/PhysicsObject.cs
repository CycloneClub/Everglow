using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine
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
            set => _rotation = value;
        }

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

        private Collider2D _collider;
        private RigidBody2D _rigidBody;
        private Vector2 _oldPos;
        private float _rotation;
        private float _oldRot;
        private int _guid;
        private string _tag;

        public PhysicsObject(Collider2D collider, RigidBody2D rigidBody)
        {
            _collider = collider;
            _rigidBody = rigidBody;
            _tag = "Default";

            if (_rigidBody == null)
            {
                _rigidBody = new RigidBody2D(1)
                {
                    MovementType = MovementType.Static,
                    UseGravity = false,
                };
            }

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

        public List<Vector2> GetWireFrameWires()
        {
            List<Vector2> wires = _collider.GetWireFrameWires();
            var R = Matrix2x2.CreateRotationMatrix(_rotation);
            for (int i = 0; i < wires.Count; i++)
            {
                wires[i] = R.Multiply(wires[i]) + Position;
            }
            return wires;
        }
        public void CleanUp()
        {
            _rigidBody?.CleanUp();
            _rotation = MathHelper.WrapAngle(_rotation);
        }
        public void Update(float deltaTime)
        {
            _rigidBody?.Update(deltaTime);
        }

        public void ApplyGravity(Vector2 gravity)
        {
            if(_rigidBody != null)
            {
                if (_rigidBody.UseGravity)
                {
                    _rigidBody.ApplyForce(gravity * _rigidBody.Mass);
                }
            }
        }
        
        public Vector2 LocalToWorldPos(Vector2 localPos)
        {
            return _rigidBody.CentroidWorldSpace + Matrix2x2.CreateRotationMatrix(_rotation).Multiply(localPos);
        }
    }
}
