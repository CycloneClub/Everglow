using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class HexaCrystalStaff_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 36000000;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
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
	}

	public override void AI()
	{
		Timer++;
		Projectile.rotation += 0.15f;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (!npc.dontTakeDamage && !npc.friendly)
				{
					if ((npc.Center - Projectile.Center).Length() < 40)
					{
						Projectile.Kill();
						break;
					}
				}
			}
		}
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
		float rot = Main.rand.NextFloat(MathHelper.TwoPi);
		var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<HexaCrystalStaff_ProjExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
		proj.rotation = rot;
		for (int i = 0; i < 6; i++)
		{
			Vector2 vel = new Vector2(6, 0).RotatedBy(i / 6f * MathHelper.TwoPi + rot);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<HexaCrystalStaff_SubProj>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
			for (int j = 0; j < 6; j++)
			{
				Vector2 velNew = vel.RotatedBy(0.5f / 6f * MathHelper.TwoPi) * 3 + vel.RotatedBy(0.5f / 6f * MathHelper.TwoPi + MathHelper.PiOver2) * MathF.Sqrt(3) * (j - 3) / 3f;
				velNew *= 0.6f;
				float rotNew = velNew.ToRotation() - MathHelper.PiOver4;
				var dustVFX = new TwilightCrystalVFXDust
				{
					Velocity = velNew,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(20, 26),
					Scale = Main.rand.NextFloat(8, 12),
					Rotation = rotNew,
					ai = new float[] { 0, 0, 0 },
				};
				Ins.VFXManager.Add(dustVFX);
			}
		}
		for (int i = 0; i < 90; i++)
		{
			Vector2 velNew = new Vector2(0, Main.rand.NextFloat(7f, 12f)).RotatedByRandom(MathHelper.TwoPi);
			float rotNew = velNew.ToRotation() - MathHelper.PiOver4;
			var dustVFX = new MagicalBoomerangDust
			{
				Velocity = velNew,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(30, 46),
				Scale = Main.rand.NextFloat(2, 4),
				Rotation = rotNew,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		SoundEngine.PlaySound(SoundID.Shatter.WithVolume(0.4f), Projectile.Center);
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D tex = ModAsset.HexaCrystalStaff_Proj.Value;
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(0f, 0.7f, 1f, 1f), Projectile.rotation + MathHelper.PiOver2, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Texture2D texGlow = ModAsset.HexaCrystalStaff_Proj_LightFace.Value;
		Texture2D texStar = Commons.ModAsset.StarSlash.Value;
		float glowValue = MathF.Sin(Projectile.rotation);
		if (glowValue > 0)
		{
			glowValue = MathF.Pow(glowValue, 8);
			Main.EntitySpriteDraw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * glowValue, Projectile.rotation + MathHelper.PiOver2, texGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

			glowValue = MathF.Pow(glowValue, 8);
			Main.EntitySpriteDraw(texStar, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * glowValue, MathHelper.PiOver2, texStar.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texStar, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f) * glowValue, 0, texStar.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None, 0);
		}

		float glowValue2 = MathF.Sin(Projectile.rotation + MathHelper.Pi);
		if (glowValue2 > 0)
		{
			Main.EntitySpriteDraw(texGlow, Projectile.Center - Main.screenPosition, null, new Color(0f, 0.5f, 1f, 0f) * glowValue2, Projectile.rotation + MathHelper.PiOver2, texGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
