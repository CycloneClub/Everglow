using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Vertex;
using ReLogic.Content;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Input;
using Everglow.Commons.Physics.PBEngine.Collision.BroadPhase;
using Everglow.Commons.Physics.PBEngine.GameInteraction;
using Everglow.Commons.Physics.PBEngine.Collision;
using Everglow.Commons.Physics.PBEngine.Constrains;

namespace Everglow.Example;

internal class PhysicsPlayer : ModPlayer
{
	private PhysicsObject _movingPanel;
	private Vector2 _prevPos;
	private Vector2 _saveVelocity;
	public override void OnEnterWorld()
	{
	}
	public override void PostUpdate()
	{
	}

	public override void PreUpdateMovement()
	{
		_prevPos = Main.LocalPlayer.Center;
		float dt = 0.1f;
		var oldPos = Main.LocalPlayer.Center;
		var preVelY = Main.LocalPlayer.velocity.Y;
		PhysicsDemo.Instance._dummyPlayer.RigidBody.LinearVelocity = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.velocity / dt);
		PhysicsDemo.Instance._dummyPlayer.Position = GeometryUtils.ConvertToPhysicsSpace(_prevPos);
		PhysicsDemo.Instance._realSimulation.Update(dt);

		// Main.LocalPlayer.Center = Physics.Utils.ConvertToPhysicsSpace(Display.Instance._dummyPlayer.Position);
		Main.LocalPlayer.velocity = GeometryUtils.ConvertToPhysicsSpace(PhysicsDemo.Instance._dummyPlayer.RigidBody.LinearVelocity * dt);
		if (preVelY > 0 && Math.Abs(Main.LocalPlayer.velocity.Y) < 0.04f)
		{
			Main.LocalPlayer.velocity.Y = 0;
		}


		if (_movingPanel != null)
		{

			//if (Main.time % 1200 < 600)
			//{
			//	_movingPanel.RigidBody.LinearVelocity = new Vector2(-3, 0);
			//}
			//else if (Main.time % 1200 >= 600)
			//{
			//	_movingPanel.RigidBody.LinearVelocity = new Vector2(3, 0);
			//}
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
		if(Main.mouseLeft && Main.mouseLeftRelease)
		{
			var ball = new PhysicsObject(new SphereCollider(32),
					new RigidBody2D(256));
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(ball);
			ball.Position = GeometryUtils.ConvertToPhysicsSpace(Main.MouseWorld);
			ball.RigidBody.LinearVelocity = new Vector2(0, 0);
		}
		if (Main.keyState[Keys.R] == KeyState.Down && Main.oldKeyState[Keys.R] == KeyState.Up)
		{
			// RigidBodyDisplay.Instance.ReInitPhysWorld();
			PhysicsDemo.Instance.ReStart();
		}

		if (Main.keyState[Keys.T] == KeyState.Down && Main.oldKeyState[Keys.T] == KeyState.Up)
		{
			var dynamicBox = new PhysicsObject(
				new BoxCollider(256, 64), new RigidBody2D(1000));
			dynamicBox.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center - new Vector2(0, 200));
			dynamicBox.Rotation = 0.0f;
			dynamicBox.RigidBody.MovementType = MovementType.Dynamic;
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(dynamicBox);
			_movingPanel = dynamicBox;

			//var ball = new PhysicsObject(new BoxCollider(32, 32),
			//        new RigidBody2D(256));
			//Display.Instance._realSimulation.AddPhysicsObject(ball);
			//ball.Position = Physics.Utils.ConvertToPhysicsSpace(Main.LocalPlayer.Center + new Vector2(0, -300));

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(300, 32), null);
			staticPlane1.Position = GeometryUtils.ConvertToPhysicsSpace(Main.LocalPlayer.Center - new Vector2(0, 400));
			staticPlane1.Rotation = 0.0f;
			PhysicsDemo.Instance._realSimulation.AddPhysicsObject(staticPlane1);

			var joint = new SpringConstrain(staticPlane1, dynamicBox, 40f, 200f, new Vector2(100, 0), new Vector2(100, 0));
			PhysicsDemo.Instance._realSimulation.AddConstrain(joint);

			var joint1 = new SpringConstrain(staticPlane1, dynamicBox, 100f, 200f, new Vector2(-100, 0), new Vector2(-100, 0));
			PhysicsDemo.Instance._realSimulation.AddConstrain(joint1);

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

	public override void PostUpdateEverything()
	{
		//rigidBodyDisplay.Update();
	}
	public override void PostDrawTiles()
	{
		//rigidBodyDisplay.Draw(Main.spriteBatch);

		List<Vertex2D> vertices = new List<Vertex2D>();
		foreach (var p in _realSimulation.GetCurrentWireFrames())
		{
			vertices.Add(new Vertex2D(GeometryUtils.ConvertToPhysicsSpace(p) - Main.screenPosition, Color.White, Vector3.Zero));
		}

		if (vertices.Count > 0)
		{
			_renderShader.Value.Parameters["uColor"].SetValue(Color.White.ToVector4());
			_renderShader.Value.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
			_renderShader.Value.CurrentTechnique.Passes[0].Apply();
			Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
		}
		base.PostDrawTiles();
	}

	public override void Load()
	{
		//rigidBodyDisplay = new RigidBodyDisplay();
		//rigidBodyDisplay.Initialize();

		Instance = this;
		ReStart();
		_renderShader = ModContent.Request<Effect>("Everglow/Example/VFX/PureColor");
		base.Load();
	}

	public void ReStart()
	{
		var terrariaCollisionGroup = new CollisionGraph();
		_realSimulation = new PhysicsSimulation(terrariaCollisionGroup);
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
		rigidb.Stiffness = -1;
		rigidb.Friction = -1;
		_realSimulation.AddPhysicsObject(_dummyPlayer);
		_realSimulation.Initialize();
	}

	public override void Unload()
	{
		base.Unload();
	}
}
