using Everglow.Commons.Physics;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using static Everglow.Commons.VFX.VFXBatchExtension;

namespace Everglow.Example.Test;

/// <summary>
/// Devs only.
/// </summary>
public class Diamond_Physics_Item : ModItem
{
	public override void SetDefaults()
	{
		Item.useTime = 21;
		Item.useAnimation = 21;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			if (!Collision.IsWorldPointSolid(Main.MouseWorld))
			{
				var rigBody = new SphereCollider(12);

				var box = new PhysicsObject(
				new SphereCollider(12),
				new RigidBody2D(20) { });

				PhysicWorldSystem.Instance._realSimulation.AddPhysicsObject(box);
				box.Position = GeometryUtils.ConvertToPhysicsSpace(Main.MouseWorld);
				box.RigidBody.LinearVelocity = new Vector2(0, 0);

				var vfx = new VisualPhysicsUnit();
				vfx.physicsObject = box;
				vfx.Active = true;
				vfx.Visible = true;
				vfx.RegisterCustomCode(Draw, Update);
				Ins.VFXManager.Add(vfx);
			}
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			PhysicWorldSystem.Instance.ReStart();
		}
	}

	public void Draw(VisualPhysicsUnit vpu)
	{
		Vector2 pos = vpu.physicsObject.Position;
		pos.Y *= -1;
		Color c = Lighting.GetColor(pos.ToTileCoordinates());
		c.A = 200;
		Texture2D tex = ModAsset.Diamond.Value;
		float rot = vpu.physicsObject.Rotation;
		Ins.Batch.Draw(tex, pos, null, c, rot, tex.Size() * 0.5f, 1f, 0);
		for (int i = 0; i < 9; i++)
		{
			tex = ModContent.Request<Texture2D>(ModAsset.Diamond_Mod + "_glow" + i).Value;
			float value = (pos.X + pos.Y) * 0.03f + rot + i;
			float valueR = Math.Max(MathF.Sin(value), 0);
			float valueG = Math.Max(MathF.Sin(value + 0.5f), 0);
			float valueB = Math.Max(MathF.Sin(value + 1f), 0);
			valueR = MathF.Pow(valueR, 4);
			valueG = MathF.Pow(valueG, 4);
			valueB = MathF.Pow(valueB, 4);
			Color reflectColor = new Color(valueR, valueG, valueB, 0);
			reflectColor = Lighting.GetColor(pos.ToTileCoordinates(), reflectColor) * 2;
			reflectColor.A = 0;
			if (i == 3 || i == 4)
			{
				reflectColor *= 0.2f;
			}
			Ins.Batch.Draw(tex, pos, null, reflectColor, rot, tex.Size() * 0.5f, 1f, 0);
		}
	}

	public void Update(VisualPhysicsUnit vpu)
	{
		// if(vpu.Timer > 150)
		// {
		// vpu.Kill();
		// }
	}
}