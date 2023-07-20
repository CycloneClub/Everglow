using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine;
using ReLogic.Content;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Input;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.GameInteraction;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Constrains;
using Terraria;
using ChatGPT.Core;
using static Terraria.ModLoader.PlayerDrawLayer;
using System.Diagnostics.Contracts;
using ChatGPT.Core.PBEngine.Collision.Colliders;
using ReLogic.Graphics;
using Everglow.Commons.Vertex;

namespace Everglow.Example;

internal class PhysicsPlayer : ModPlayer
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
			&& PhysicsDemo.Instance._dummyPlayer.RigidBody.TangetRelativeVelocity.Count > 0)
		{
			Vector2 tangent = Vector2.Zero;
			foreach (var v in PhysicsDemo.Instance._dummyPlayer.RigidBody.TangetRelativeVelocity)
			{
				tangent += v;
			}
			tangent /= PhysicsDemo.Instance._dummyPlayer.RigidBody.TangetRelativeVelocity.Count;
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
		//Main.LocalPlayer.velocity = GeometryUtils.ConvertToPhysicsSpace(PhysicsDemo.Instance._dummyPlayer.RigidBody.LinearVelocity) * dt;
		// 
		//var t = PhysicsDemo.Instance._dummyPlayer.RigidBody.TangetRelativeVelocity;
		//Main.NewText(Main.LocalPlayer.velocity, Color.GreenYellow);

		Player.velocity -= _extraVelocity;

		float dt = 0.1f;
		var oldPos = Main.LocalPlayer.Center;
		var preVelY = Main.LocalPlayer.velocity.Y;
		var preVel = Main.LocalPlayer.velocity;

		var broadPhase = PhysicsDemo.Instance._realSimulation.BroadPhaseCollisionDetector;


		List<PhysicsObject> temporaryObjects = new List<PhysicsObject>();

		foreach (var proj in Main.projectile)
		{
			if (proj.active && proj.tileCollide)
			{

				if (broadPhase.TestSingleCollision(proj.Hitbox.ToAABBPhysSpace().EnLarge(8 * 16), Vector2.Zero, 0, new List<string>()
					{
						"Default"
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
						"Default"
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
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(obj);
		}

		// Main.NewText(Main.LocalPlayer.velocity, Color.Lime);
		PhysicsDemo.Instance._dummyPlayer.RigidBody.LinearVelocity = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.velocity / dt);
		PhysicsDemo.Instance._dummyPlayer.Position = GeometryUtils.ConvertToPhysicsSpace(oldPos);
		if (!PhysicsDemo.EnableRigidBodyTestPlayground)
		{
			PhysicsDemo.Instance._realSimulation.Update(dt);
		}

		foreach (var obj in temporaryObjects)
		{
			PhysicsDemo.Instance._realSimulation.RemoveObject(obj);
		}


		//Main.LocalPlayer.Center = GeometryUtils.ConvertToPhysicsSpace(PhysicsDemo.Instance._dummyPlayer.RigidBody.CentroidWorldSpace) - player.;
		//      Main.NewText(Main.LocalPlayer.velocity,Color.Cyan);

		// Find any valid, standable slope
		if (PhysicsDemo.Instance._dummyPlayer.RigidBody.ContactNormals.Count > 0)
		{
			foreach (var v in PhysicsDemo.Instance._dummyPlayer.RigidBody.ContactNormals)
			{
				if (v.Y > 0.9f)
				{
					_shouldStand = true;
					// Omit tangent velocity due to gravity
					float mg = -PhysicsDemo.Instance._realSimulation.Gravity;
					Vector2 velDueG = new Vector2(0, -Player.gravDir * Player.gravity / dt) * dt;
					Vector2 normalDir = Vector2.Dot(velDueG, v) * v;
					Vector2 tanDir = velDueG - normalDir;
					Player.velocity.Y = 0;
					break;
				}
			}
		}

		//if (_shouldStand)
		//{
		//    Main.LocalPlayer.velocity = Main.LocalPlayer.velocity - _prevVelocity;

		//}

		//      if (t.Length() > 0 && Math.Abs(GeometryUtils.Cross(new Vector2(1, 0), t)) < 0.3f)
		//{
		//	Main.LocalPlayer.velocity.Y = 0;
		//}
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
				var ball = new PhysicsObject(new BoxCollider(32, 32),
						new RigidBody2D(256));
				PhysicsDemo.Instance._realSimulation.AddPhysicsObject(ball);
				ball.Position = GeometryUtils.ConvertToPhysicsSpace(Main.MouseWorld) + new Vector2(i, 0);
				ball.RigidBody.LinearVelocity = new Vector2(0, 0);
			}
		}
		if (Main.keyState[Keys.R] == KeyState.Down && Main.oldKeyState[Keys.R] == KeyState.Up)
		{
			if (PhysicsDemo.EnableRigidBodyTestPlayground)
			{
				RigidBodyDisplay.Instance.ReInitPhysWorld();
			}
			PhysicsDemo.Instance.ReStart();
		}

		if (Main.keyState[Keys.T] == KeyState.Down && Main.oldKeyState[Keys.T] == KeyState.Up)
		{
			var dynamicBox = new PhysicsObject(
				new BoxCollider(128, 32), new RigidBody2D(256));
			//_movingPanel = dynamicBox;

			dynamicBox.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -200));
			dynamicBox.Rotation = 0.0f;
			dynamicBox.RigidBody.MovementType = MovementType.Dynamic;
			dynamicBox.RigidBody.UseGravity = true;
			dynamicBox.RigidBody.Drag = 0.1f;
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(dynamicBox);


			var staticPlane = new PhysicsObject(
			new BoxCollider(256, 128), null);
			staticPlane.Rotation = 0.3f;
			staticPlane.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(200, -200));
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(staticPlane);

			//var ball = new PhysicsObject(new BoxCollider(32, 32),
			//        new RigidBody2D(256));
			//Display.Instance._realSimulation.AddPhysicsObject(ball);
			//ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -300));

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(128, 32), null);
			staticPlane1.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(-200, -200));
			staticPlane1.Rotation = 0.0f;
			staticPlane1.RigidBody.MovementType = MovementType.Static;
			staticPlane1.RigidBody.UseGravity = false;
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(staticPlane1);
			// _movingPanel = staticPlane1;

			var joint = new SpringConstrain(staticPlane1, dynamicBox, 100f, 144f, new Vector2(64, -16), new Vector2(64, 16));
			PhysicsDemo.Instance._realSimulation.AddConstrain(joint);
			var joint11 = new SpringConstrain(staticPlane1, dynamicBox, 100f, 144f, new Vector2(-64, -16), new Vector2(-64, 16));
			PhysicsDemo.Instance._realSimulation.AddConstrain(joint11);

			//var joint2 = new SpringConstrain(staticPlane, dynamicBox, 100f, 144f, new Vector2(-64, 16), new Vector2(64, -16));
			//PhysicsDemo.Instance._realSimulation.AddConstrain(joint2);
			//var joint22 = new SpringConstrain(staticPlane, dynamicBox, 100f, 144f, new Vector2(-64, -16), new Vector2(64, 16));
			//PhysicsDemo.Instance._realSimulation.AddConstrain(joint22);

			//if (Main.rand.NextBool(2))
			//{
			//    var ball = new PhysicsObject(new BoxCollider(256, 32),
			//        new RigidBody2D(128));
			//    ball.RigidBody.MovementType = Physics.Collision.MovementType.Kinematic;
			//    ball.RigidBody.UseGravity = false;
			//    ball.RigidBody.AngularDrag = 0;
			//    ball.RigidBody.AngularVelocity = 0.9f;
			//    Display.Instance._realSimulation.AddPhysicsObject(ball);
			//    ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -100));
			//}
			//else
			//{
			//    var ball = new PhysicsObject(new SphereCollider(32),
			//            new RigidBody2D(256));
			//    Display.Instance._realSimulation.AddPhysicsObject(ball);
			//    ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(100, 0));
			//    ball.RigidBody.LinearVelocity = new Vector2(-78, 0);
			//}
		}
	}
}

internal class PhysicsDemo : ModSystem
{
	private RigidBodyDisplay rigidBodyDisplay;
	private Asset<Effect> _renderShader;
	public PhysicsSimulation _realSimulation;
	public static PhysicsDemo Instance;
	public PhysicsObject _dummyPlayer;

	public const bool EnableRigidBodyTestPlayground = true;
	public const float Simulation_DeltaTime = 0.1f;

	public override void PostUpdateEverything()
	{
		if (EnableRigidBodyTestPlayground)
		{
			rigidBodyDisplay.Update();
		}
	}


	public override void PostDrawTiles()
	{
		if (EnableRigidBodyTestPlayground)
		{
			rigidBodyDisplay.Draw(Main.spriteBatch);
		}

		List<Vertex2D> vertices = new List<Vertex2D>();
		foreach (var (p, c) in _realSimulation.GetCurrentWireFrames())
		{
			vertices.Add(new Vertex2D(GeometryUtils.ConvertToPhysicsSpace(p) - Main.screenPosition, c, Vector3.Zero));
		}

		if (vertices.Count > 0)
		{
			// _renderShader.Value.Parameters["uColor"].SetValue(Color.White.ToVector4());
			_renderShader.Value.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
			_renderShader.Value.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
		}

		_realSimulation.BroadPhaseCollisionDetector.DrawDebugInfo(Main.spriteBatch);
		base.PostDrawTiles();
	}

	public override void Load()
	{
		if (EnableRigidBodyTestPlayground)
		{
			rigidBodyDisplay = new RigidBodyDisplay();
			rigidBodyDisplay.Initialize();
		}

		Instance = this;
		ReStart();
		_renderShader = ModContent.Request<Effect>("Everglow/Example/VFX/Default");


		On_Player.PlayerFrame += On_Player_PlayerFrame;
		On_Collision.TileCollision += On_Collision_TileCollision;
		base.Load();
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
			"Default"
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
					//if (weightA != 0 && weightB != 0)
					//    weightA = weightB = 0.5f;

					//Vector2 impluseVel = Vector2.Zero;
					//               List<CollisionEvent2D> events;
					//               dummyCollider.GetContactInfo(info, Simulation_DeltaTime, out events);
					//foreach(var e in events)
					//{
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
					//if (float.IsNaN(dummyPObject.RigidBody.CentroidWorldSpace.X) || float.IsNaN(dummyPObject.RigidBody.CentroidWorldSpace.Y))
					//{
					//	if (true)
					//		;
					//}
					// extraVel = collider.ParentObject.RigidBody.LinearVelocity * Simulation_DeltaTime;
					//var n = Vector2.Dot(result, info.Normal) * info.Normal;
					//result = result - n;
					// velChange += impluseVel;
				}
			}

			// Main.NewText(result + dummyCollider.ParentObject.Position - oldCenter, Color.Red);
			// var newDir = result + new Vector2(0, d);
			Velocity = (dummyCollider.ParentObject.Position - oldCenter);
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
	private Vector2 SolveContactImpluse(CollisionEvent2D e, Vector2 sourceVel, int count, float deltaTime)
	{
		float mass = 64;
		var ri = e.LocalOffsetSrc;
		var rb = e.LocalOffsetTarget;

		var va = sourceVel;
		var vb = e.Target.RigidBody.LinearVelocity
			+ GeometryUtils.AnuglarVelocityToLinearVelocity(rb, e.Target.RigidBody.AngularVelocity);

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
		double R2 = rBdotN * rBdotN * e.Target.RigidBody.GlobalInverseInertiaTensor;//Vector2.Dot(Utils.AnuglarVelocityToLinearVelocity(rb, (float)(e.Target.RigidBody.GlobalInverseInertiaTensor
																					//* Utils.Cross(rb, e.Normal))), e.Normal);
		double J_n = (vnew_n - va_n) / (1.0 / mass + e.Target.RigidBody.InvMass + R1 + R2);
		Vector2 J = (float)J_n * e.Normal / count;// + (float)J_t * va_t_unit;

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
	public override void PostDrawInterface(SpriteBatch sb)
	{
		var start = Main.LocalPlayer.Center - new Vector2(1024 / 2, 1024 / 2) - Main.screenPosition;

		sb.DrawString(FontAssets.MouseText.Value, "[Profiler]", start, Color.Red);
		sb.DrawString(FontAssets.MouseText.Value, $"PreIntegration Time: {_realSimulation.MeasuredPreIntegrationTimeInMs} ms", start + new Vector2(0, 28), Color.Red);
		sb.DrawString(FontAssets.MouseText.Value, $"BroadPhase Time: {_realSimulation.MeasuredBroadPhaseTimeInMs} ms", start + new Vector2(0, 56), Color.Red);
		sb.DrawString(FontAssets.MouseText.Value, $"NarrowPhase Time: {_realSimulation.MeasuredNarrowPhaseTimeInMs} ms", start + new Vector2(0, 74), Color.Red);

		double percent = _realSimulation.NumOfBroadPhasePairs == 0 ? 1 : (double)_realSimulation.NumOfNarrowPhasePairs / _realSimulation.NumOfBroadPhasePairs;
		sb.DrawString(FontAssets.MouseText.Value, $"Detected Pairs: {_realSimulation.NumOfNarrowPhasePairs} / {_realSimulation.NumOfBroadPhasePairs}, {percent:F2}% of accuracy", start + new Vector2(0, 108), Color.Red);
	}
	public void ReStart()
	{
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

	public override void Unload()
	{
		base.Unload();
	}
}
