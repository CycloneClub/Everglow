using Everglow.Commons.MEAC;
using Everglow.Commons.MEAC.VFX;
using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.Utilities;
using Terraria.DataStructures;

namespace Everglow.Example.Projectiles;

public class ExampleMeleeProjectile : MeleeProj_3D
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void OnSpawn(IEntitySource source)
	{
		EnableSphereCoordDraw = false;
		SlashColor = new Color(0.5f, 0.4f, 0, 0);
	}

	public override void SetCustomDefaults()
	{
		Projectile.width = 82;
		Projectile.height = 82;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 5;
		BaseMeleeSpeed = 2.7;
		BaseDecaySpeed = 0.86;
	}

	public override void AddDust(Vector3 oldAxisTip, Vector3 oldAxisTail, Vector3 rotationAxis, float rotationSpeed, float trailFade)
	{
		// float maxCount = Math.Abs(rotationSpeed) * 100;
		// float rotSpeed = rotationSpeed / maxCount;
		// for (int i = 0; i < maxCount; i++)
		// {
		// if (Main.rand.NextBool(10))
		// {
		// float randValue = MathF.Sqrt(Main.rand.NextFloat());
		// var melee_dust = new MeleeProj_3D_Dust()
		// {
		// Active = true,
		// Visible = true,
		// Position_Space = oldAxisTip * randValue + oldAxisTail * (1 - randValue),
		// MaxTime = Main.rand.NextFloat(30, 60),
		// Scale = Main.rand.NextFloat(0.1f, 0.8f) * (randValue + 1f) * trailFade * 0.17f,
		// Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
		// RotSpeed = rotationSpeed * 0.05f * Main.rand.NextFloat(0.8f, 1.2f),
		// RotAxis = rotationAxis,
		// ParentProj = this,
		// };
		// melee_dust.RegisterBehavior(CustomDustBehavior);
		// melee_dust.RegisterDraw(CustomDustDraw);
		// Ins.VFXManager.Add(melee_dust);
		// }
		// RotateMainAxis(rotSpeed, rotationAxis, ref oldAxisTip);
		// RotateMainAxis(rotSpeed, rotationAxis, ref oldAxisTail);
		// }
		base.AddDust(oldAxisTip, oldAxisTail, rotationAxis, rotationSpeed, trailFade);
	}
}
