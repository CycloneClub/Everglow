using System.Data;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Constraints;
using Everglow.Commons.Physics.PBEngine.GameInteraction;
using Everglow.Commons.Vertex;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Everglow.Commons.Physics;

public class PhysicsPlayer : ModPlayer
{
	private PhysicsObject _movingPanel;
	private Vector2 _prevVelocity;
	private float _simulationDt = 0.1f;
	public Vector2 _extraVelocity;
	public bool _shouldStand;

	public override void OnEnterWorld()
	{
	}

	public override void PostUpdate()
	{
		Main.LocalPlayer.velocity += _extraVelocity;
		_extraVelocity = Vector2.Zero;
		if (Main.LocalPlayer.velocity.Y == 0
			&& PhysicsWorldSystem.Instance._dummyPlayer.RigidBody.TangentRelativeVelocity.Count > 0)
		{
			Vector2 tangent = Vector2.Zero;
			foreach (var v in PhysicsWorldSystem.Instance._dummyPlayer.RigidBody.TangentRelativeVelocity)
			{
				tangent += v;
			}
			tangent /= PhysicsWorldSystem.Instance._dummyPlayer.RigidBody.TangentRelativeVelocity.Count;
			_extraVelocity = tangent * _simulationDt;
			_extraVelocity = Vector2.Dot(_extraVelocity, new Vector2(1, 0)) * new Vector2(1, 0);
		}
	}

	public override void PreUpdate()
	{
		_shouldStand = false;
		_prevVelocity = Main.LocalPlayer.velocity;
		base.PreUpdate();
	}

	public override void PreUpdateMovement()
	{
		//// Main.LocalPlayer.Center = Physics.Utils.ConvertToPhysicsSpace(Display.Instance._dummyPlayer.Position);
		// Main.LocalPlayer.velocity = GeometryUtils.ConvertToPhysicsSpace(PhysicWorldSystem.Instance._dummyPlayer.RigidBody.LinearVelocity) * dt;
		//
		// var t = PhysicWorldSystem.Instance._dummyPlayer.RigidBody.TangetRelativeVelocity;
		// Main.NewText(Main.LocalPlayer.velocity, Color.GreenYellow);
		Player.velocity -= _extraVelocity;

		float dt = 0.1f;
		var oldPos = Main.LocalPlayer.Center;
		var preVelY = Main.LocalPlayer.velocity.Y;
		var preVel = Main.LocalPlayer.velocity;

		var broadPhase = PhysicsWorldSystem.Instance._realSimulation.BroadPhaseCollisionDetector;

		var temporaryObjects = new List<PhysicsObject>();

		foreach (var proj in Main.projectile)
		{
			if (proj.active && proj.tileCollide)
			{
				if (broadPhase.TestSingleCollision(proj.Hitbox.ToAABBPhysSpace().EnLarge(8 * 16), Vector2.Zero, 0, new List<string>()
					{
						"Default",
					}))
				{
					var obj = new PhysicsObject(new BoxCollider(proj.width, proj.height), new RigidBody2D(64));
					obj.Position = GeometryUtils.ConvertToPhysicsSpace(proj.Center);
					obj.RigidBody.LinearVelocity = GeometryUtils.ConvertToPhysicsSpace(proj.velocity / dt);
					obj.RigidBody.MovementType = MovementType.Player;
					obj.Tag = "Player";
					obj.RigidBody.Drag = 0;
					temporaryObjects.Add(obj);
				}
			}
		}

		foreach (var npc in Main.npc)
		{
			if (npc.active && !npc.noTileCollide)
			{
				if (broadPhase.TestSingleCollision(npc.Hitbox.ToAABBPhysSpace().EnLarge(8 * 16), Vector2.Zero, 0, new List<string>()
					{
						"Default",
					}))
				{
					var obj = new PhysicsObject(new BoxCollider(npc.width, npc.height), new RigidBody2D(512));
					obj.Position = GeometryUtils.ConvertToPhysicsSpace(npc.Center);
					obj.RigidBody.LinearVelocity = GeometryUtils.ConvertToPhysicsSpace(npc.velocity / dt);
					obj.RigidBody.MovementType = MovementType.Player;
					obj.RigidBody.Drag = 0;
					obj.Tag = "Player";
					temporaryObjects.Add(obj);
				}
			}
		}

		foreach (var obj in temporaryObjects)
		{
			PhysicsWorldSystem.Instance._realSimulation.AddPhysicsObject(obj);
		}

		// Main.NewText(Main.LocalPlayer.velocity, Color.Lime);
		PhysicsWorldSystem.Instance._dummyPlayer.RigidBody.LinearVelocity = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.velocity / dt);
		PhysicsWorldSystem.Instance._dummyPlayer.Position = GeometryUtils.ConvertToPhysicsSpace(oldPos);
		PhysicsWorldSystem.Instance._realSimulation.Update(dt);

		foreach (var obj in temporaryObjects)
		{
			PhysicsWorldSystem.Instance._realSimulation.RemoveObject(obj);
		}

		// Main.LocalPlayer.Center = GeometryUtils.ConvertToPhysicsSpace(PhysicWorldSystem.Instance._dummyPlayer.RigidBody.CentroidWorldSpace) - player.;
		//      Main.NewText(Main.LocalPlayer.velocity,Color.Cyan);

		// Find any valid, standable slope
		if (PhysicsWorldSystem.Instance._dummyPlayer.RigidBody.ContactNormals.Count > 0)
		{
			foreach (var v in PhysicsWorldSystem.Instance._dummyPlayer.RigidBody.ContactNormals)
			{
				if (v.Y > 0.9f)
				{
					_shouldStand = true;

					// Omit tangent velocity due to gravity
					float mg = -PhysicsWorldSystem.Instance._realSimulation.Gravity;
					Vector2 velDueG = new Vector2(0, -Player.gravDir * Player.gravity / dt) * dt;
					Vector2 normalDir = Vector2.Dot(velDueG, v) * v;
					Vector2 tanDir = velDueG - normalDir;
					Player.velocity.Y = 0;
					break;
				}
			}
		}

		// if (_shouldStand)
		// {
		//    Main.LocalPlayer.velocity = Main.LocalPlayer.velocity - _prevVelocity;

		// }

		// if (t.Length() > 0 && Math.Abs(GeometryUtils.Cross(new Vector2(1, 0), t)) < 0.3f)
		// {
		// Main.LocalPlayer.velocity.Y = 0;
		// }
		// Main.LocalPlayer.velocity -= _extraVelocity;
		if (_movingPanel != null)
		{
			if (Main.time % 1200 < 600)
			{
				_movingPanel.RigidBody.LinearVelocity = new Vector2(-4, 0);
			}
			else if (Main.time % 1200 >= 600)
			{
				_movingPanel.RigidBody.LinearVelocity = new Vector2(4, 0);
			}
		}

		base.PreUpdateMovement();
	}

	public override void UpdateEquips()
	{
		base.UpdateEquips();
	}

	public override void SetControls()
	{
		base.SetControls();
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			for (int i = 0; i < 2; i++)
			{
				// var ball = new PhysicsObject(
				// new BoxCollider(32, 32),
				// new RigidBody2D(256));
				// PhysicWorldSystem.Instance._realSimulation.AddPhysicsObject(ball);
				// ball.Position = GeometryUtils.ConvertToPhysicsSpace(Main.MouseWorld) + new Vector2(i, 0);
				// ball.RigidBody.LinearVelocity = new Vector2(0, 0);
			}
		}

		if (Main.keyState[Keys.T] == KeyState.Down && Main.oldKeyState[Keys.T] == KeyState.Up)
		{
			var dynamicBox = new PhysicsObject(
				new BoxCollider(128, 32), new RigidBody2D(256));

			// _movingPanel = dynamicBox;
			dynamicBox.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -200));
			dynamicBox.Rotation = 0.0f;
			dynamicBox.RigidBody.MovementType = MovementType.Dynamic;
			dynamicBox.RigidBody.UseGravity = true;
			dynamicBox.RigidBody.Drag = 0.1f;
			PhysicsWorldSystem.Instance._realSimulation.AddPhysicsObject(dynamicBox);

			var staticPlane = new PhysicsObject(
			new BoxCollider(256, 128), null);
			staticPlane.Rotation = 0.3f;
			staticPlane.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(200, -200));
			PhysicsWorldSystem.Instance._realSimulation.AddPhysicsObject(staticPlane);

			// var ball = new PhysicsObject(new BoxCollider(32, 32),
			//        new RigidBody2D(256));
			// Display.Instance._realSimulation.AddPhysicsObject(ball);
			// ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -300));
			var staticPlane1 = new PhysicsObject(
				new BoxCollider(128, 32), null);
			staticPlane1.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(-200, -200));
			staticPlane1.Rotation = 0.0f;
			staticPlane1.RigidBody.MovementType = MovementType.Static;
			staticPlane1.RigidBody.UseGravity = false;
			PhysicsWorldSystem.Instance._realSimulation.AddPhysicsObject(staticPlane1);

			// _movingPanel = staticPlane1;
			var joint = new SpringConstraint(staticPlane1, dynamicBox, 100f, 144f, new Vector2(64, -16), new Vector2(64, 16));
			PhysicsWorldSystem.Instance._realSimulation.AddConstrain(joint);
			var joint11 = new SpringConstraint(staticPlane1, dynamicBox, 100f, 144f, new Vector2(-64, -16), new Vector2(-64, 16));
			PhysicsWorldSystem.Instance._realSimulation.AddConstrain(joint11);

			// var joint2 = new SpringConstrain(staticPlane, dynamicBox, 100f, 144f, new Vector2(-64, 16), new Vector2(64, -16));
			// PhysicWorldSystem.Instance._realSimulation.AddConstrain(joint2);
			// var joint22 = new SpringConstrain(staticPlane, dynamicBox, 100f, 144f, new Vector2(-64, -16), new Vector2(64, 16));
			// PhysicWorldSystem.Instance._realSimulation.AddConstrain(joint22);

			// if (Main.rand.NextBool(2))
			// {
			//    var ball = new PhysicsObject(new BoxCollider(256, 32),
			//        new RigidBody2D(128));
			//    ball.RigidBody.MovementType = Physics.Collision.MovementType.Kinematic;
			//    ball.RigidBody.UseGravity = false;
			//    ball.RigidBody.AngularDrag = 0;
			//    ball.RigidBody.AngularVelocity = 0.9f;
			//    Display.Instance._realSimulation.AddPhysicsObject(ball);
			//    ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -100));
			// }
			// else
			// {
			//    var ball = new PhysicsObject(new SphereCollider(32),
			//            new RigidBody2D(256));
			//    Display.Instance._realSimulation.AddPhysicsObject(ball);
			//    ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(100, 0));
			//    ball.RigidBody.LinearVelocity = new Vector2(-78, 0);
			// }
		}
	}
}
