using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class BrittleRockSlingshotStone_Explosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => ModAsset.CyanVineStaff_proj_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 160;
		Projectile.height = 160;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
		int n = Main.rand.Next(9, 10);
		for (int i = 0; i < n; i++)
		{
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, Main.rand.NextFloat(7, 14f)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<BrittleRockSlingshotStone>(), Projectile.damage / n, Projectile.knockBack / n, Projectile.owner, 0, 1, 0.8f);
			p.scale = Projectile.scale / MathF.Sqrt(n);
			p.penetrate = 1;
			p.ai[1] = 1;
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft == 199)
		{
			for (int g = 0; g < 75; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(14f, 15f)).RotatedByRandom(MathHelper.TwoPi);
				var somg = new RockSmogDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(22, 45),
					Scale = Main.rand.NextFloat(10f, 25f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			for (int x = 0; x < 12; x++)
			{
				var d = Dust.NewDustDirect(Projectile.position - new Vector2(4), Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 0, default, Main.rand.NextFloat(0.4f, 0.9f));
				d.velocity = new Vector2(0, Main.rand.NextFloat(4.7f, 14.1f)).RotatedByRandom(6.283);
			}
		}
		if (Projectile.timeLeft <= 190)
		{
			Projectile.friendly = false;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 180;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 180;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 180;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 180;
		return bool0 || bool1 || bool2 || bool3;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var c = new Color(3f * (1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue) * (1 - timeValue), 1.9f * (1 - timeValue) * (1 - timeValue), 0f);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI - Projectile.position.X) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 2.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 2.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, c, MathF.Sin(Projectile.whoAmI + Projectile.position.Y) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 2.62f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, light.Width / 2, light.Height), c, MathF.Sin(Projectile.type * 0.4f + Projectile.whoAmI) * 6 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 2.62f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radius / 2; h += 1)
		{
			c0.R = (byte)(h / radius * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 150f * Projectile.ai[0], 15 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.HitDirectionOverride = target.Center.X > Projectile.Center.X ? 1 : -1;
		base.ModifyHitNPC(target, ref modifiers);
	}
}
