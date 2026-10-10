using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class BurningLanternWreck : ModProjectile
{
	public override string Texture => "Everglow/" + ModAsset.DarkLanternBombExplosion_Path;

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.1f;
		Projectile.velocity *= MathF.Pow(0.998f, Projectile.velocity.Length());
		float length = Projectile.velocity.Length() / 8f;
		if (length > 1)
		{
			for (int i = 0; i < length; i++)
			{
				SmallFlame(Projectile.Center - Projectile.velocity.NormalizeSafe() * i * 8, 3);
			}
		}
		else
		{
			SmallFlame(Projectile.Center, 3);
		}
	}

	public override void OnKill(int timeLeft)
	{
		SmallFlame(Projectile.Center, 12);
	}

	public void SmallFlame(Vector2 pos, int count)
	{
		float timeDecay = 1f;
		if (Projectile.timeLeft < 120)
		{
			timeDecay = Projectile.timeLeft / 120f;
		}
		for (int i = 0; i < count; i++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 2f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new LanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(50, 75) * timeDecay + 3,
				Scale = Main.rand.NextFloat(10f, 20f) * timeDecay,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeDecay = 1f;
		if (Projectile.timeLeft < 120)
		{
			timeDecay = Projectile.timeLeft / 120f;
		}
		Texture2D star = Commons.ModAsset.StarSlashGray.Value;
		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Color drawColor = new Color(0.6f, 0.2f + 0.2f * MathF.Sin((float)Main.time * 0.03f + Projectile.whoAmI), 0, 0);
		var drawPos = Projectile.Center - Main.screenPosition;
		Main.EntitySpriteDraw(star, drawPos, null, drawColor, 0, star.Size() * 0.5f, timeDecay, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, drawPos, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, timeDecay, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(spot, drawPos, null, new Color(1f, 0.8f, 0.7f, 0), 0, spot.Size() * 0.5f, timeDecay, SpriteEffects.None, 0);
		return false;
	}
}
