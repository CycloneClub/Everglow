using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class GlowMoonBlade : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.aiStyle = -1;
		Projectile.friendly = true; // 友好
		Projectile.melee = true; // 近战
		Projectile.ignoreWater = true; // 不被水影响
		Projectile.tileCollide = false; // 能穿墙，反义为false
		Projectile.timeLeft = 255; // 存在时间，60是1秒
		Projectile.alpha = 10;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 2;
		Projectile.scale = 1f; // 大小
		ProjectileID.Sets.TrailCacheLength[this.Projectile.type] = 15;
		ProjectileID.Sets.TrailingMode[this.Projectile.type] = 0;
	}

	public float Energy = 0;
	public bool HasHitTile = false;

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(SoundID.Item60);
		Energy = 400;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
		Energy *= 0.95f;
		Projectile.velocity *= 0.95f;
		if (Collision.SolidCollision(Projectile.position + new Vector2(10), Projectile.width - 20, Projectile.height - 20))
		{
			Projectile.scale *= 0.85f;
			Projectile.velocity *= 0.1f;
			if (Projectile.scale < 0.01f)
			{
				Projectile.active = false;
			}
			if (!HasHitTile && Energy > 50f)
			{
				HasHitTile = true;
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<GlowMoonBlade_ash>(), Projectile.damage * 3 / 2, Projectile.knockBack, player.whoAmI, player.direction);
				p0.rotation = Projectile.rotation + MathF.PI / 2f;
				for (int x = 0; x < 60; x++)
				{
					var spark = new RayDustDust
					{
						Velocity = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-25f, 25f),
						Active = true,
						Visible = true,
						Position = Projectile.Center + Main.rand.NextFloat(-8f, 8f) * Vector2.Normalize(Projectile.velocity),
						MaxTime = Main.rand.Next(137, 245),
						Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { 0, 0 },
					};
					Ins.VFXManager.Add(spark);
				}
			}
			if (!HasHitTile && Energy <= 50f)
			{
				HasHitTile = true;
				Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<GlowMoonBlade_ash>(), 0, Projectile.knockBack, player.whoAmI, player.direction);
				p0.rotation = Projectile.rotation + MathF.PI / 2f;
				p0.scale = Energy / 50f;
			}
		}
		if (Energy > 50f)
		{
			var spark = new RayDustDust
			{
				Velocity = Projectile.velocity * Main.rand.NextFloat(0.40f, 1.0f),
				Active = true,
				Visible = true,
				Position = Projectile.Center + Main.rand.NextFloat(-80f, 80f) * Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)),
				MaxTime = Main.rand.Next(137, 245),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(spark);
		}
		else if (Energy < 0.5f)
		{
			Projectile.Kill();
		}
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		double range = Projectile.scale * 2.2f;
		double range2 = Projectile.scale * 2.2f;
		Texture2D tex = ModAsset.GlowMoonBlade.Value;
		Vector2 drawOrigin = tex.Size() * 0.5f;
		float mulColor = Energy / 400f;
		for (int k = 0; k < Projectile.oldPos.Length; k++)
		{
			range2 *= 1.041;
			Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
			Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - (int)((k + 3) * 1.5)) / (float)Projectile.oldPos.Length) * mulColor;
			Main.EntitySpriteDraw(tex, drawPos, null, color, Projectile.rotation, drawOrigin, (float)range, SpriteEffects.None, 0f);

			Vector2 drawPos2 = Projectile.position - Main.screenPosition + drawOrigin;
			Color color2 = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - (int)((k + 8) * 1.5)) / (float)Projectile.oldPos.Length) * mulColor;
			Main.EntitySpriteDraw(tex, drawPos2, null, color2, Projectile.rotation, drawOrigin, (float)range2, SpriteEffects.None, 0f);
		}
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(mulColor, mulColor * mulColor * 3f, 0, 0), Projectile.rotation, drawOrigin, new Vector2(Projectile.scale * 1.5f, Projectile.scale), SpriteEffects.None, 0f);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.OnFire, 120);
		Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.zeroVector, ModContent.ProjectileType<GlowMoonBlade_ash>(), Projectile.damage / 2, Projectile.knockBack / 4f, Projectile.owner);
		p0.rotation = Projectile.rotation + Main.rand.NextFloat(-0.8f, 0.8f);
		p0.scale = 0.6f;
		if (Energy < 50)
		{
			p0.scale = Energy / 50f * 0.6f;
		}
		base.OnHitNPC(target, hit, damageDone);
	}
}
