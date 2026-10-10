using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class HexaCrystalStaff_SubProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 20;
		Projectile.height = 20;
		Timer = 0;
	}

	public int Timer = 0;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
	}

	public override void AI()
	{
		Timer++;
		Projectile.rotation += 0.15f;
		var dustVFX = new TwilightCrystalVFXDust
		{
			Velocity = Projectile.velocity * 0.3f,
			Active = true,
			Visible = true,
			Position = Projectile.Center + Projectile.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * (0.5f - Projectile.timeLeft % 2) * 6,
			MaxTime = Main.rand.Next(3, 12),
			Scale = Main.rand.NextFloat(3, 12),
			Rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver4,
			ai = new float[] { 0, 0, 0 },
		};
		Ins.VFXManager.Add(dustVFX);

		for (int i = 0; i < 3; i++)
		{
			var dustVFXRight = new MagicalBoomerangDust
			{
				Velocity = Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f),
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-5, 5),
				MaxTime = Main.rand.Next(4, 16),
				Scale = Main.rand.NextFloat(3, 4),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFXRight);
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (target.HasBuff(ModContent.BuffType<HexaCrystalWeak>()))
		{
			modifiers.FinalDamage *= 2.5f;
		}
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D tex = ModAsset.HexaCrystalStaff_SubProj.Value;
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(0f, 0.7f, 1f, 1f), Projectile.rotation + MathHelper.PiOver2, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Texture2D texGlow = ModAsset.HexaCrystalStaff_SubProj_LightFace.Value;
		Texture2D texStar = Commons.ModAsset.StarSlash.Value;
		float glowValue = MathF.Sin(Projectile.rotation);
		if (Timer < 15)
		{
			glowValue = 1f;
		}
		if (glowValue > 0)
		{
			glowValue = MathF.Pow(glowValue, 8);
			Main.EntitySpriteDraw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * glowValue, Projectile.rotation + MathHelper.PiOver2, texGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

			glowValue = MathF.Pow(glowValue, 8);
			Main.EntitySpriteDraw(texStar, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * glowValue, MathHelper.PiOver2, texStar.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texStar, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * glowValue, 0, texStar.Size() * 0.5f, Projectile.scale * 0.24f, SpriteEffects.None, 0);
		}

		float glowValue2 = MathF.Sin(Projectile.rotation + MathHelper.Pi);
		if (Timer < 15)
		{
			glowValue2 = 1f;
		}
		if (glowValue2 > 0)
		{
			Main.EntitySpriteDraw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(0f, 0.5f, 1f, 0f) * glowValue2, Projectile.rotation + MathHelper.PiOver2, texGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
