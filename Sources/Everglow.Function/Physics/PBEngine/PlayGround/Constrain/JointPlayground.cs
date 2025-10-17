using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Constrains;

namespace Everglow.Commons.Physics.PBEngine.PlayGround.Constrain
{
	/// <summary>
	/// 储存了一些测试约束的场景
	/// </summary>
	public class JointPlayground
	{
		public static PhysicsSimulation SimpleJoint()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(500, 660);
			dynamicBox.Rotation = 0.0f;
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(300, 32), null);
			staticPlane1.Position = new Vector2(512, 700);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			var joint = new JointConstrain(staticPlane1, dynamicBox, new Vector2(0, -40), new Vector2(0, 64));
			world.AddConstrain(joint);

			return world;
		}

		public static PhysicsSimulation SimpleJointMultiple()
		{
			var world = new PhysicsSimulation();

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(32, 32), null);
			staticPlane1.Position = new Vector2(512, 700);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			var dynamicBox = new PhysicsObject(
			new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(500, 600);
			dynamicBox.Rotation = 0.0f;
			world.AddPhysicsObject(dynamicBox);

			var dynamicBox1 = new PhysicsObject(
				new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox1.Position = new Vector2(500, 500);
			dynamicBox1.Rotation = 0.0f;
			world.AddPhysicsObject(dynamicBox1);

			var joint = new JointConstrain(staticPlane1, dynamicBox, new Vector2(0, -50), new Vector2(0, 50));
			world.AddConstrain(joint);

			var joint2 = new JointConstrain(dynamicBox, dynamicBox1, new Vector2(0, -70), new Vector2(0, 70));
			world.AddConstrain(joint2);

			// var kPlane = new PhysicsObject(
			//    new BoxCollider(200, 32), null);
			// kPlane.Position = new Vector2(512, 400);
			// kPlane.Rotation = 0;
			// kPlane.RigidBody.ApplyAngularVelocity(0.9f);
			// kPlane.RigidBody.MovementType = Collision.MovementType.Kinematic;
			// world.AddPhysicsObject(kPlane);
			return world;
		}

		public static PhysicsSimulation DynamicJoints()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
			new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(500, 500);
			dynamicBox.Rotation = 0.2f;
			world.AddPhysicsObject(dynamicBox);

			var dynamicBox1 = new PhysicsObject(
				new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox1.Position = new Vector2(500, 440);
			dynamicBox1.Rotation = 0.3f;
			world.AddPhysicsObject(dynamicBox1);

			var joint2 = new JointConstrain(dynamicBox, dynamicBox1, new Vector2(0, -50), new Vector2(0, 50));
			world.AddConstrain(joint2);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(700, 32), null);
			staticPlane1.Position = new Vector2(512, 300);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			return world;
		}

		public static PhysicsSimulation NewtonPendulum()
		{
			var world = new PhysicsSimulation();

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(512, 32), null);
			staticPlane1.Position = new Vector2(512, 800);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			for (int i = 0; i < 5; i++)
			{
				var dynamicBox = new PhysicsObject(
     			new SphereCollider(32), new RigidBody2D(256));
				dynamicBox.Position = new Vector2(512 + 64 * (i - 2), 600);
				if(i == 4)
				{
					dynamicBox.Position += new Vector2(60, 60);
				}
				dynamicBox.Rotation = 0.0f;
				world.AddPhysicsObject(dynamicBox);

				if (i == 0)
				{
					dynamicBox.RigidBody.LinearVelocity = new Vector2(-50, 0);
				}

				var joint1 = new JointConstrain(staticPlane1, dynamicBox, new Vector2(64 * (i - 2), -16), new Vector2(0, 200));
				world.AddConstrain(joint1);
			}

			// var dynamicBox1 = new PhysicsObject(
			//    new SphereCollider(32), new RigidBody2D(256));
			// dynamicBox1.Position = new Vector2(512, 584);
			// dynamicBox1.Rotation = 0.0f;
			// world.AddPhysicsObject(dynamicBox1);

			// var dynamicBox2 = new PhysicsObject(
			//    new SphereCollider(32), new RigidBody2D(256));
			// dynamicBox2.Position = new Vector2(576, 584);
			// dynamicBox2.Rotation = 0.0f;
			// world.AddPhysicsObject(dynamicBox2);

			// var joint2 = new Joint(staticPlane1, dynamicBox1, new Vector2(0, -16), new Vector2(0, 200));
			// world.AddConstrain(joint2);

			// var joint3 = new Joint(staticPlane1, dynamicBox2, new Vector2(64, -16), new Vector2(0, 200));
			// world.AddConstrain(joint3);

			return world;
		}

		public static PhysicsSimulation SimpleSpring()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(300, 100);
			dynamicBox.Rotation = 0.0f;
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(300, 32), null);
			staticPlane1.Position = new Vector2(512, 700);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			var joint = new SpringConstrain(staticPlane1, dynamicBox, 29f, 200);
			world.AddConstrain(joint);

			var dynamicBox1 = new PhysicsObject(
	        new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox1.Position = new Vector2(400, 10);
			dynamicBox1.Rotation = 0.0f;
			world.AddPhysicsObject(dynamicBox1);

			var joint1 = new SpringConstrain(dynamicBox, dynamicBox1, 29f, 200);
			world.AddConstrain(joint1);

			return world;
		}

		public static PhysicsSimulation SpringTriganluar()
		{
			var world = new PhysicsSimulation();

			var dynamicBox = new PhysicsObject(
				new BoxCollider(32, 64), new RigidBody2D(256));
			dynamicBox.Position = new Vector2(300, 100);
			dynamicBox.Rotation = 0.0f;
			world.AddPhysicsObject(dynamicBox);

			var staticPlane1 = new PhysicsObject(
				new BoxCollider(32, 32), null);
			staticPlane1.Position = new Vector2(512, 700);
			staticPlane1.Rotation = 0f;
			world.AddPhysicsObject(staticPlane1);

			var staticPlane2 = new PhysicsObject(
	        new BoxCollider(32, 32), null);
			staticPlane2.Position = new Vector2(212, 300);
			staticPlane2.Rotation = 0f;
			world.AddPhysicsObject(staticPlane2);

			var staticPlane3 = new PhysicsObject(
            new BoxCollider(32, 32), null);
			staticPlane3.Position = new Vector2(812, 300);
			staticPlane3.Rotation = 0f;
			world.AddPhysicsObject(staticPlane3);

			var joint = new SpringConstrain(staticPlane1, dynamicBox, 29f, 200);
			world.AddConstrain(joint);
			var joint1 = new SpringConstrain(staticPlane2, dynamicBox, 29f, 200);
			world.AddConstrain(joint1);

			var joint2 = new SpringConstrain(staticPlane3, dynamicBox, 29f, 200);
			world.AddConstrain(joint2);

			return world;
		}
	}
}