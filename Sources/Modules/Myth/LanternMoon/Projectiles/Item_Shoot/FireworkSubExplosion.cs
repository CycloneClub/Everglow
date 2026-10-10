using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public class FireworkSubExplosion : FireworkProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TypelessProjectiles;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (FlameTrail flameTrail in Stars)
		{
			Vector3 drawPos3D = flameTrail.Postion;
			drawPos3D.X += (Projectile.Center - Main.screenPosition).X;
			drawPos3D.Y += (Projectile.Center - Main.screenPosition).Y;
			float scale;
			Vector2 v2Pos = Projection2D(drawPos3D, new Vector2(Main.screenWidth, Main.screenHeight) / 2, 1000, out scale);
			if (targetHitbox.Contains((int)v2Pos.X, (int)v2Pos.Y))
			{
				return true;
			}
		}
		return false;
	}

	public override void SetDefaults()
	{
		Projectile.timeLeft = 600;
		Projectile.aiStyle = -1;
		Projectile.scale = 1;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Stars = new List<FlameTrail>();
	}

	public Color LegacyColor;
	public Vector3 LegacyVelocity;
	public Vector3 LegacyPosition;
	public int LegacyStyle;

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(SoundID.DD2_GoblinBomb.WithVolume(Main.rand.NextFloat(0.4f, 0.8f)).WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center + new Vector2(LegacyPosition.X, LegacyPosition.Y));
		Timer = 0;
	}

	public override void SpawnStyle(int style)
	{
		var axis = new Vector3(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
		float rot = Main.rand.NextFloat(6.283f);
		int starsCount = 20;
		Color color = LegacyColor;
		switch (style)
		{
			// 球状
			case 0:
				for (int theta = -starsCount; theta <= starsCount; theta += 10)
				{
					float length = MathF.Cos(theta / (float)starsCount * MathF.PI);
					for (int phi = 0; phi <= length * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length / starsCount * MathHelper.TwoPi) * length, MathF.Sin(theta / (float)starsCount * MathF.PI), MathF.Sin(phi / length / starsCount * MathHelper.TwoPi) * length) * 3f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity + LegacyPosition, new List<Vector3>(), velocity + LegacyVelocity, color, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-20, 0);
						Stars.Add(flame);
					}
				}
				break;

			// 环状
			case 1:
				if (style == 1)
				{
					Color c0 = LegacyColor;
					starsCount = 160;
					int theta2 = 0;
					float length2 = MathF.Cos(theta2 / (float)starsCount * MathF.PI);
					for (int phi = 0; phi < length2 * starsCount; phi += 5)
					{
						Vector3 velocity = new Vector3(MathF.Cos(phi / length2 / starsCount * MathHelper.TwoPi) * length2, MathF.Sin(theta2 / (float)starsCount * MathF.PI), MathF.Sin(phi / length2 / starsCount * MathHelper.TwoPi) * length2) * 4f * Main.rand.NextFloat(0.95f, 1.05f);
						velocity = RodriguesRotate(velocity, axis, rot);
						var flame = new FlameTrail(-velocity, new List<Vector3>(), velocity, c0, Main.rand.NextFloat(0.85f, 1.15f) * 0.7f, true);
						flame.FlameTimer = Main.rand.Next(-10, 0);
						flame.ai[0] = 20;
						Stars.Add(flame);
					}
				}
				break;
		}
	}

	public override void AI()
	{
		if (Timer == 0)
		{
			SpawnStyle(LegacyStyle);
		}
		Timer++;
		bool allKilled = true;
		for (int i = 0; i < Stars.Count; i++)
		{
			Stars[i] = UpdateFlameTrail(Stars[i]);
			if (Stars[i].Active)
			{
				allKilled = false;
			}
		}
		if (allKilled)
		{
			Projectile.Kill();
		}
	}
}
