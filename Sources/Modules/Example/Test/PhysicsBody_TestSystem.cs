using Everglow.Commons.Physics;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.Physics.PBEngine.Core;
using static Everglow.Commons.VFX.VFXBatchExtension;

namespace Everglow.Example.Test;

/// <summary>
/// Devs only.
/// </summary>
public class PhysicsBody_TestSystem : ModItem
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
			if (!Collision.IsWorldPointSolid(Main.MouseWorld))
			{
				var rigBody = new RigidBody2D(1)
				{
					MovementType = MovementType.Kinematic,
					UseGravity = false,
				};
				var box = new PhysicsObject(
				new BoxCollider(192, 16),
				null);

				PhysicsWorldSystem.Instance._realSimulation.AddPhysicsObject(box);
				box.Position = GeometryUtils.ConvertToPhysicsSpace(Main.MouseWorld);
				box.RigidBody.LinearVelocity = new Vector2(0, 0);

				var vfx = new VisualPhysicsUnit();
				vfx.Timer = 0;
				vfx.physicsObject = box;
				vfx.Active = true;
				vfx.Visible = true;
				vfx.RegisterCustomCode(Draw, Update);
				Ins.VFXManager.Add(vfx);
			}
		}
	}

	public void Draw(VisualPhysicsUnit vpu)
	{
		Vector2 pos = vpu.physicsObject.Position;
		pos.Y *= -1;
		Color c = Lighting.GetColor(pos.ToTileCoordinates());
		c.A = 200;
		Texture2D tex = ModAsset.MovePlatform.Value;
		float rot = vpu.physicsObject.Rotation;
		Ins.Batch.Draw(tex, pos, null, c, rot, tex.Size() * 0.5f, 1f, 0);
	}

	public void Update(VisualPhysicsUnit vpu)
	{
		Vector2 vel;
		var timer = vpu.Timer % 600;
		if (timer < 100)
		{
			vel = Vector2.zeroVector;
		}
		else if(timer >= 100 && timer < 300)
		{
			vel = new Vector2(0, -2);
		}
		else if (timer >= 300 && timer < 400)
		{
			vel = Vector2.zeroVector;
		}
		else
		{
			vel = new Vector2(0, 2);
		}
		vpu.physicsObject.Position += vel;
	}
}