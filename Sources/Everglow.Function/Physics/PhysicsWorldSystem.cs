using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Core;
using Everglow.Commons.Physics.PBEngine.GameInteraction;

namespace Everglow.Commons.Physics;

public class PhysicsWorldSystem : ModSystem
{
	public PhysicsSimulation _realSimulation;
	public static PhysicsWorldSystem Instance;
	public PhysicsObject _dummyPlayer;

	public const float Simulation_DeltaTime = 0.1f;

	public override void Load()
	{
		Instance = this;
		ReStart();
		On_Player.PlayerFrame += On_Player_PlayerFrame;
		On_Collision.TileCollision += On_Collision_TileCollision;
	}

	public override void Unload()
	{
		Instance = null;
		On_Player.PlayerFrame -= On_Player_PlayerFrame;
		On_Collision.TileCollision -= On_Collision_TileCollision;
	}

	private Vector2 On_Collision_TileCollision(On_Collision.orig_TileCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, int gravDir)
	{
		Position = GeometryUtils.ConvertToPhysicsSpace(Position);
		Velocity = GeometryUtils.ConvertToPhysicsSpace(Velocity);
		var rect = new AABB(Position - new Vector2(0, Height), Position + new Vector2(Width, 0));
		var oldCenter = rect.Center;

		rect = rect.Move(Velocity);

		var collisions = _realSimulation.BroadPhaseCollisionDetector.GetSingleCollision(rect, Vector2.Zero, 0, new List<string>()
		{
			"Default",
		});

		if (collisions.Count > 0)
		{
			var dummyCollider = new BoxCollider(Width, Height);
			var dummyPObject = new PhysicsObject(dummyCollider, new RigidBody2D(64));
			dummyPObject.OldPosition = rect.Center;
			dummyPObject.RigidBody.CentroidWorldSpace = rect.Center;

			foreach (var collider in collisions)
			{
				CollisionInfo info;
				if (dummyCollider.TestCollisionCondition(collider, Simulation_DeltaTime, out info))
				{
					float weightA = info.Source.RigidBody.InvMass / (info.Source.RigidBody.InvMass + info.Target.RigidBody.InvMass);
					float weightB = info.Target.RigidBody.InvMass / (info.Source.RigidBody.InvMass + info.Target.RigidBody.InvMass);

					// if (weightA != 0 && weightB != 0)
					//    weightA = weightB = 0.5f;

					// Vector2 impluseVel = Vector2.Zero;
					//               List<CollisionEvent2D> events;
					//               dummyCollider.GetContactInfo(info, Simulation_DeltaTime, out events);
					// foreach(var e in events)
					// {
					//                   impluseVel += SolveContactImpluse(e, result + velChange, events.Count, Simulation_DeltaTime);
					//               }
					// dummyPObject.RigidBody.MoveBody(info.Normal * weightA * info.Depth, Simulation_DeltaTime);
					if (Math.Abs(info.Normal.Y) < 0.77f)
					{
						dummyPObject.RigidBody.MoveBody(info.Normal * weightA * info.Depth, Simulation_DeltaTime);
					}
					else
					{
						dummyPObject.RigidBody.MoveBody(info.Depth / info.Normal.Y * new Vector2(0, 1), Simulation_DeltaTime);
					}

					// if (float.IsNaN(dummyPObject.RigidBody.CentroidWorldSpace.X) || float.IsNaN(dummyPObject.RigidBody.CentroidWorldSpace.Y))
					// {
					// if (true)
					// ;
					// }
					// extraVel = collider.ParentObject.RigidBody.LinearVelocity * Simulation_DeltaTime;
					// var n = Vector2.Dot(result, info.Normal) * info.Normal;
					// result = result - n;
					// velChange += impluseVel;
				}
			}

			// Main.NewText(result + dummyCollider.ParentObject.Position - oldCenter, Color.Red);
			// var newDir = result + new Vector2(0, d);
			Velocity = dummyCollider.ParentObject.Position - oldCenter;
		}

		Vector2 result = orig(GeometryUtils.ConvertToPhysicsSpace(Position), GeometryUtils.ConvertToPhysicsSpace(Velocity), Width, Height, fallThrough, fall2, gravDir);
		return result;
	}

	/// <summary>
	/// 返回接触响应后的速度的变化量
	/// </summary>
	/// <param name="e"></param>
	/// <param name="count"></param>
	/// <param name="deltaTime"></param>
	/// <returns></returns>
	private Vector2 SolveContactImpulse(CollisionEvent2D e, Vector2 sourceVel, int count, float deltaTime)
	{
		float mass = 64;
		var ri = e.LocalOffsetSrc;
		var rb = e.LocalOffsetTarget;

		var va = sourceVel;
		var vb = e.Target.RigidBody.LinearVelocity
			+ GeometryUtils.AngularVelocityToLinearVelocity(rb, e.Target.RigidBody.AngularVelocity);

		float va_n = Vector2.Dot(va - vb, e.Normal);
		if (va_n >= 0)
		{
			e.NormalVelOld = 0;
			return Vector2.Zero;
		}
		float stiffness = 0;
		float vnew_n = stiffness * Math.Max(-va_n - 20 * deltaTime, 0);

		// (a × b) × c = (c • a)b - (c • b)a
		double rAdotN = Vector2.Dot(GeometryUtils.Rotate90(ri), e.Normal);
		double rBdotN = Vector2.Dot(GeometryUtils.Rotate90(rb), e.Normal);
		double R1 = 0; // Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(ri, (float)(GlobalInverseInertiaTensor

		// * Utils.Cross(ri, e.Normal))), e.Normal);
		double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor; // Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(rb, (float)(e.Target.RigidBody.GlobalInverseInertiaTensor

		// * Utils.Cross(rb, e.Normal))), e.Normal);
		double J_n = (vnew_n - va_n) / (1.0 / mass + e.Target.RigidBody.InvMass + R1 + R2);
		Vector2 J = (float)J_n * e.Normal / count; // + (float)J_t * va_t_unit;

		return J / mass;
	}

	private void On_Player_PlayerFrame(On_Player.orig_PlayerFrame orig, Player self)
	{
		Main.LocalPlayer.velocity += self.GetModPlayer<PhysicsPlayer>()._extraVelocity;

		if (self.GetModPlayer<PhysicsPlayer>()._shouldStand)
		{
			Main.LocalPlayer.velocity.Y = 0;
		}
		orig(self);
		Main.LocalPlayer.velocity -= self.GetModPlayer<PhysicsPlayer>()._extraVelocity;
	}

	public void ReStart()
	{
		if(_realSimulation is not null)
		{
			_realSimulation.ClearPhysicsObjects();
		}
		var terrariaCollisionGroup = new CollisionGraph();
		_realSimulation = new PhysicsSimulation(terrariaCollisionGroup);
		terrariaCollisionGroup.AddSingleEdge("Default", "Default");
		terrariaCollisionGroup.AddSingleEdge("Default", "Terrain");
		terrariaCollisionGroup.AddDoubleEdge("Player", "Default");

		var terrain = new PhysicsObject(new TileCollider(), null);
		_realSimulation.AddPhysicsObject(terrain);
		terrain.Tag = "Terrain";
		terrain.RigidBody.MovementType = MovementType.Static;

		var rigidb = new RigidBody2D(64);
		_dummyPlayer = new PhysicsObject(new BoxCollider(20, 42), rigidb);
		_dummyPlayer.Tag = "Player";
		rigidb.MovementType = MovementType.Player;
		rigidb.UseGravity = false;
		rigidb.AngularDrag = 0;
		rigidb.Drag = 0;
		rigidb.Restitution = -1;
		rigidb.Friction = -1;
		_realSimulation.AddPhysicsObject(_dummyPlayer);
		_realSimulation.Initialize();
	}
}