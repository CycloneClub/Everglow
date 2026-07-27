using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Physics.PBEngine.PlayGround.Contact
{
	/// <summary>
	/// 储存了一些物理引擎接触测试的场景
	/// </summary>
	public class ContactPlayGround2
    {
        public static PhysicsSimulation SphereStairCase()
        {
            var world = new PhysicsSimulation();


            var dynamicBox = new PhysicsObject(
                new SphereCollider(32), new RigidBody2D(256));
            dynamicBox.Position = new Vector2(512, 800);
            dynamicBox.RigidBody.LinearVelocity = new Vector2(12, 0);
            world.AddPhysicsObject(dynamicBox);

            var staticPlane1 = new PhysicsObject(
                new BoxCollider(300, 32), null);
            staticPlane1.Position = new Vector2(512, 400);
            staticPlane1.Rotation = 0f;
            world.AddPhysicsObject(staticPlane1);

            var staticPlane2 = new PhysicsObject(
                new BoxCollider(100, 16), null);
            staticPlane2.Position = new Vector2(700, 424);
            staticPlane2.Rotation = 0f;
            world.AddPhysicsObject(staticPlane2);

            var staticPlane3 = new PhysicsObject(
    new BoxCollider(100, 16), null);
            staticPlane3.Position = new Vector2(400, 424);
            staticPlane3.Rotation = 0f;
            world.AddPhysicsObject(staticPlane3);

            return world;
        }

        public static PhysicsSimulation ExtractionTest()
        {
            var world = new PhysicsSimulation();


            var dynamicBox = new PhysicsObject(
                new BoxCollider(128, 32), new RigidBody2D(128));
            dynamicBox.Position = new Vector2(300, 800);
            dynamicBox.RigidBody.LinearVelocity = new Vector2(0, 0);
            world.AddPhysicsObject(dynamicBox);

            var dynamicball = new PhysicsObject(
                new SphereCollider(32), new RigidBody2D(512));
            dynamicball.Position = new Vector2(300, 900);
            dynamicball.RigidBody.LinearVelocity = new Vector2(0, 0);
            world.AddPhysicsObject(dynamicball);

            var dynamicball2 = new PhysicsObject(
                new SphereCollider(32), new RigidBody2D(512));
            dynamicball2.Position = new Vector2(380, 900);
            dynamicball2.RigidBody.LinearVelocity = new Vector2(0, 0);
            world.AddPhysicsObject(dynamicball2);


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

        public static PhysicsSimulation MoveWithPad()
        {
            var world = new PhysicsSimulation();


            var dynamicBox = new PhysicsObject(
                new BoxCollider(32, 32), new RigidBody2D(256));
            dynamicBox.Position = new Vector2(400, 700);
            dynamicBox.RigidBody.LinearVelocity = new Vector2(0, 0);
            world.AddPhysicsObject(dynamicBox);

            var kPlane = new PhysicsObject(
                    new BoxCollider(500, 32), null);
            kPlane.Position = new Vector2(512, 512);
            kPlane.Rotation = 0;
            kPlane.RigidBody.Drag = 0;
            kPlane.RigidBody.LinearVelocity = new Vector2(10, 0);
            kPlane.RigidBody.MovementType = MovementType.Kinematic;
            world.AddPhysicsObject(kPlane);

            return world;
        }
    }
}
