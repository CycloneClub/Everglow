using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics;
using Terraria.Audio;

namespace Everglow.Example.Test;

/// <summary>
/// Devs only.
/// </summary>
public class PhysicsBody_Square : ModItem
{
	public override void SetDefaults()
	{
		Item.useTime = 21;
		Item.useAnimation = 21;
	}

	public int soundID = 0;

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			var box = new Square_Physics(
				new BoxCollider(32, 32),
				new RigidBody2D(256));
			PhysicWorldSystem.Instance._realSimulation.AddPhysicsObject(box);
			box.Position = GeometryUtils.ConvertToPhysicsSpace(Main.MouseWorld);
			box.RigidBody.LinearVelocity = new Vector2(0, 0);
		}
		if(Main.mouseRight && Main.mouseRightRelease)
		{
			if (PhysicWorldSystem.EnableRigidBodyTestPlayground)
			{
				RigidBodyRenderer.Instance.ReInitPhysWorld();
			}
			PhysicWorldSystem.Instance.ReStart();
		}
	}
}