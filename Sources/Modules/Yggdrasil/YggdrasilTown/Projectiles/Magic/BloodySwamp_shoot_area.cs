using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BloodySwamp_shoot_area : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 12000;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.DamageType = DamageClass.Magic;
	}

	public Vector2 EndPos = default;

	public override void OnSpawn(IEntitySource source)
	{
		EndPos = Main.MouseWorld;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		var colorLight = new Vector3(0.9f, 0.0f, 0.12f);
		Vector2 velocity = new Vector2(0, 2).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) + Projectile.velocity * 0.25f;
		for (int i = 0; i < 4; i++)
		{
			var somg = new BloodSwampDust
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 90,
				Scale = 25,
				Rotation = Main.rand.NextFloat(6.283f),
				MaxScale = Main.rand.NextFloat(12.0f, 28.0f),
				ChasedProjectile = Projectile,
				ai = new float[] { Main.rand.NextFloat(-0.12f, 0.12f), Main.rand.NextFloat(MathHelper.TwoPi), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		Lighting.AddLight(Projectile.Center, colorLight);
		if ((Projectile.Center - EndPos).Length() < 20)
		{
			Projectile.Center = EndPos;
			Projectile.Kill();
		}
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<BloodySwamp_area>(), Projectile.damage, 0, Projectile.owner);
		float rotForTotal = Main.rand.NextFloat(6.283f);
		float size = 2.5f;
		for (int g = 0; g < 500; g++)
		{
			Vector2 velocity = new Vector2(0, 8f).RotatedBy(g / 500f * MathHelper.TwoPi);
			velocity = velocity.RotatedBy(Projectile.rotation + Projectile.whoAmI + MathHelper.Pi);
			velocity *= size;
			var somg = new BloodFlame
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 120,
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(8.0f, 12f), g / 125f * MathHelper.TwoPi + rotForTotal, MathF.Sin(g / 100f * MathHelper.TwoPi) },
			};
			Ins.VFXManager.Add(somg);
		}
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		return false;
	}
}
