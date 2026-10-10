using Everglow.Myth.Acytaea.VFXs;
using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class Acytaea_FlamePost : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 50;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 40;
		Projectile.height = 300;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.height = (int)(300 * Projectile.ai[0]);
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		int times = 10;
		for (int x = 0; x < times; x++)
		{
			float size = Main.rand.NextFloat(0.4f, 0.96f);
			var acytaeaFlame = new DragonFlamePostDust
			{
				Velocity = (new Vector2(0, -12) + new Vector2(0, Main.rand.NextFloat(1f, 6f)).RotatedByRandom(MathHelper.TwoPi)) * Main.rand.NextFloat(Projectile.timeLeft / 50f + 0.3f) * Projectile.ai[0],
				Active = true,
				Visible = true,
				Position = Projectile.Bottom,
				MaxTime = Main.rand.Next(24, 36),
				Scale = 20f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = new float[] { Projectile.Bottom.X, Main.rand.NextFloat(-0.8f, 0.8f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float value = Projectile.timeLeft / 50f;
		Vector2 newScale = new Vector2(1f, value * 2) * MathF.Sin(value * MathF.PI) * 1.2f * Projectile.ai[0];
		Color flame;
		if (value > 0.5f)
		{
			flame = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(1f, 0.6f, 0f, 0), (value - 0.5f) * 2f);
		}
		else
		{
			flame = Color.Lerp(new Color(1f, 0.6f, 0f, 0), new Color(0.5f, 0f, 0f, 0), value * 2f);
		}
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(star, Projectile.Bottom - Main.screenPosition, null, flame, 0, star.Size() / 2f, newScale * 0.75f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Bottom - Main.screenPosition, null, flame, MathHelper.PiOver2, star.Size() / 2f, newScale, SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
	}
}
