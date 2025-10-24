using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Core;

namespace Everglow.Commons.Physics.PBEngine.PlayGround.Contact
{
	/// <summary>
	/// 储存了一些物理引擎接触测试的场景
	/// </summary>
	public class ContactPlayGround1
	{
		public static PhysicsSimulation SimpleGround1()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 800);
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(300, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			return world;
		}

		public static PhysicsSimulation InclinedGround()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 800);
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(600, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0.22f;
			world.AddPhysicsObject(staticPlane1);

			return world;
		}

		public static PhysicsSimulation MultipleInclinedGround()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 900);
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(500, 32), null);
			staticPlane1.Position = new Vector2(300, 700);
			staticPlane1.Rotation = -0.32f;
			world.AddPhysicsObject(staticPlane1);

			var staticPlane2 = new PhysicsObject(
				new BoxCollider(500, 32), null);
			staticPlane2.Position = new Vector2(680, 200);
			staticPlane2.Rotation = 0.32f;
			world.AddPhysicsObject(staticPlane2);

			return world;
		}

		public static PhysicsSimulation MultipleInclinedGround2()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 900);
			world.AddPhysicsObject(dynamicBox);

			for (int i = 0; i < 5; i++)
			{
				var staticPlane1 = new PhysicsObject(
					new BoxCollider(500, 32), null);
				staticPlane1.Position = new Vector2(300, 700 - i * 200);
				staticPlane1.Rotation = -0.32f;
				world.AddPhysicsObject(staticPlane1);

				var staticPlane2 = new PhysicsObject(
					new BoxCollider(500, 32), null);
				staticPlane2.Position = new Vector2(680, 700 - i * 200 - 100);
				staticPlane2.Rotation = 0.32f;
				world.AddPhysicsObject(staticPlane2);
			}
			return world;
		}

		public static PhysicsSimulation MultipleInclinedGround3()
		{
			var world = new PhysicsSimulation();

			for (int i = 0; i < 20; i++)
			{
				var dynamicBox = new PhysicsObject(
					new SphereCollider(16), new RigidBody2D(256));
				dynamicBox.Position = new Vector2(512 + (i % 10) * 40, 900 + i / 10 * 40);
				world.AddPhysicsObject(dynamicBox);
			}

			for (int i = 0; i < 5; i++)
			{
				var staticPlane1 = new PhysicsObject(
					new BoxCollider(500, 32), null);
				staticPlane1.Position = new Vector2(300, 700 - i * 250);
				staticPlane1.Rotation = -0.38f;
				world.AddPhysicsObject(staticPlane1);

				var staticPlane2 = new PhysicsObject(
					new BoxCollider(500, 32), null);
				staticPlane2.Position = new Vector2(680, 700 - i * 250 - 125);
				staticPlane2.Rotation = 0.38f;
				world.AddPhysicsObject(staticPlane2);
			}
			return world;
		}

		public static PhysicsSimulation MultipleInclinedGroundSphere()
		{
			var world = new PhysicsSimulation();

			for (int i = 0; i < 20; i++)
			{
				var dynamicBall = new PhysicsObject(
					new SphereCollider(16), new RigidBody2D(256));
				dynamicBall.Position = new Vector2(512, 800 + i * 40);
				world.AddPhysicsObject(dynamicBall);
			}

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(400, 32), null);
			staticPlane1.Position = new Vector2(200, 400);
			staticPlane1.Rotation = -0.78f;
			world.AddPhysicsObject(staticPlane1);

			var staticPlane2 = new PhysicsObject(
				new BoxCollider(400, 32), null);
			staticPlane2.Position = new Vector2(400, 400);
			staticPlane2.Rotation = 0.78f;
			world.AddPhysicsObject(staticPlane2);

			return world;
		}

		public static PhysicsSimulation DoubleSlope()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
			   new BoxCollider(162, 32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(300, 600);
			world.AddPhysicsObject(dynamicBox);

			var dynamicBox1 = new PhysicsObject(
				new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox1.Position = new Vector2(300, 760);
			dynamicBox1.RigidBody.ApplyForce(new Vector2(100, 0));
			world.AddPhysicsObject(dynamicBox1);

			for (int i = 0; i < 100; i++)
			{
				var dynamicBox2 = new PhysicsObject(
					new BoxCollider(55, 32), new RigidBody2D(256));
				dynamicBox2.Position = new Vector2(350, 700 + 36 * i);
				dynamicBox2.RigidBody.ApplyForce(new Vector2(100, 0));
				world.AddPhysicsObject(dynamicBox2);
			}

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(400, 32), null);
			staticPlane1.Position = new Vector2(200, 400);
			staticPlane1.Rotation = -0.78f;
			world.AddPhysicsObject(staticPlane1);

			var staticPlane2 = new PhysicsObject(
				new BoxCollider(400, 32), null);
			staticPlane2.Position = new Vector2(400, 400);
			staticPlane2.Rotation = 0.78f;
			world.AddPhysicsObject(staticPlane2);

			return world;
		}

		public static PhysicsSimulation SimpleKinematic()
		{
			var world = new PhysicsSimulation();

			for (int i = 0; i < 200; i++)
			{
				var dynamicBox = new PhysicsObject(
					new BoxCollider(32, 32), new RigidBody2D(256));
				dynamicBox.Position = new Vector2(512 + (i % 10) * 35, 900 + i / 10 * 35);
				world.AddPhysicsObject(dynamicBox);
			}

			var kPlane = new PhysicsObject(
					new BoxCollider(500, 32), null);
			kPlane.Position = new Vector2(512, 512);
			kPlane.Rotation = 0;
			kPlane.RigidBody.ApplyAngularVelocity(0.9f);
			kPlane.RigidBody.MovementType = MovementType.Kinematic;
			world.AddPhysicsObject(kPlane);

			var sPlane = new PhysicsObject(
				new BoxCollider(32, 600), null);
			sPlane.Position = new Vector2(0, 512);
			sPlane.Rotation = 0;
			world.AddPhysicsObject(sPlane);

			var sPlane2 = new PhysicsObject(
				new BoxCollider(32, 600), null);
			sPlane2.Position = new Vector2(1024, 512);
			sPlane2.Rotation = 0;
			world.AddPhysicsObject(sPlane2);

			var sPlane1 = new PhysicsObject(
				new BoxCollider(1024, 32), null);
			sPlane1.Position = new Vector2(512, 0);
			sPlane1.Rotation = 0;
			world.AddPhysicsObject(sPlane1);
			return world;
		}

		public static PhysicsSimulation SimpleSphere()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new SphereCollider(32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 800);
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(300, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);
			return world;
		}

		public static PhysicsSimulation SlopeSphere()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new SphereCollider(32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 800);
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(300, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0.3f;
			world.AddPhysicsObject(staticPlane1);
			return world;
		}

		public static PhysicsSimulation SphereAndBox()
		{
			var world = new PhysicsSimulation();

			for (int i = 0; i < 10; i++)
			{
				var dynamicBox = new PhysicsObject(
					new SphereCollider(32), new RigidBody2D(256));
				dynamicBox.Position = new Vector2(512 + (i - 5) * 50, 800);
				world.AddPhysicsObject(dynamicBox);
			}

			for (int i = 0; i < 10; i++)
			{
				var dynamicBox = new PhysicsObject(
					new BoxCollider(32, 32), new RigidBody2D(256));
				dynamicBox.Position = new Vector2(512 + (i - 5) * 50, 700);
				world.AddPhysicsObject(dynamicBox);
			}

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(600, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);
			return world;
		}

		public static PhysicsSimulation Box2()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
						new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(512, 700);
			dynamicBox.RigidBody.Restitution = 0.3f;
			world.AddPhysicsObject(dynamicBox);

			var dynamicBox1 = new PhysicsObject(
			new BoxCollider(32, 32), new RigidBody2D(256));
			dynamicBox1.Position = new Vector2(512, 734);
			dynamicBox1.RigidBody.Restitution = 0.3f;
			world.AddPhysicsObject(dynamicBox1);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(600, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);
			return world;
		}

		public static PhysicsSimulation BoxStack()
		{
			var world = new PhysicsSimulation();

			for (int i = 0; i < 5; i++)
			{
				for (int j = 1; j <= i + 1; j++)
				{
					var dynamicBox = new PhysicsObject(
						new BoxCollider(32, 32), new RigidBody2D(256));
					dynamicBox.Position = new Vector2(512 + (j - 2) * 38 - 16 * i, 500 - i * 34);
					dynamicBox.RigidBody.Restitution = 0.3f;
					world.AddPhysicsObject(dynamicBox);
				}
			}

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(600, 32), null);
			staticPlane1.Position = new Vector2(512, 200);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);
			return world;
		}
	}
}