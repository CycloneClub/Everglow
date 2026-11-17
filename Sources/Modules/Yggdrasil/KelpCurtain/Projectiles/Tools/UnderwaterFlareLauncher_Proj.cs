using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Tools;

public class UnderwaterFlareLauncher_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TypelessProjectiles;

	public int WetTimer = 0;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.penetrate = 10;
		Projectile.tileCollide = true;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 7200;
	}

	public override void AI()
	{
		Projectile.rotation += 0.15f;
		if (Projectile.wet)
		{
			WetTimer++;
			if (WetTimer > Projectile.timeLeft)
			{
				WetTimer = Projectile.timeLeft;
			}
			Projectile.velocity *= 0.98f;
			float value = WetTimer / 180f;
			if (value > 1)
			{
				value = 1;
			}
			for (int t = 0; t < 3 * value; t++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1.0f, 12f)).RotatedByRandom(MathHelper.TwoPi) * value;
				var spark = new UnderwaterFlareLauncher_Spark
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					MaxTime = Main.rand.Next(70, 125),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(9f, 17.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					Temperature = value,
					noGravity = true,
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
				};
				Ins.VFXManager.Add(spark);
			}
			GradientColor gradientColor = new GradientColor();
			gradientColor.colorList.Add((new Color(0.3f, 0.2f, 0.3f, 1), 0));
			gradientColor.colorList.Add((new Color(0.7f, 0.1f, 0.2f, 1), 0.2f));
			gradientColor.colorList.Add((new Color(0.5f, 0.4f, 0.3f, 1), 0.5f));
			gradientColor.colorList.Add((new Color(0.4f, 0.4f, 0.7f, 1), 0.7f));
			gradientColor.colorList.Add((new Color(1f, 1f, 1f, 1), 1f));
			Vector3 light = gradientColor.GetColor(value).ToVector3() * MathF.Exp(value);
			Lighting.AddLight(Projectile.Center, light * 1.5f);
			if (value > 0.5f)
			{
				for (int k = 0; k < 32; k++)
				{
					float range = (value - 0.5f) * 2f;
					range = 1 - MathF.Cos(range * MathHelper.Pi * 1.2f);
					range += 0.15f * MathF.Sin((float)Main.time * 0.02f + Projectile.whoAmI);
					Vector2 pos = Projectile.Center + new Vector2(0, range * (180f - (k % 2) * 80f)).RotatedBy(k / 32f * MathHelper.TwoPi + (float)Main.time * 0.002f);
					var checkTile = TileUtils.SafeGetTile(pos.ToTileCoordinates());
					if (checkTile.LiquidAmount > 0 && (!Main.tileBlockLight[checkTile.TileType] || !checkTile.HasTile))
					{
						float dis = FindClosestDistanceOutOfLiquid(pos) / 5f;
						Lighting.AddLight(pos, light * dis);
					}
				}
			}
		}
		else
		{
			Projectile.velocity.Y += 0.15f;
			Projectile.velocity *= 0.99f;
		}
	}

	public float FindClosestDistanceOutOfLiquid(Vector2 position)
	{
		float minDistance = 5f;
		for (int i = -5; i <= 5; i++)
		{
			for (int j = -5; j <= 5; j++)
			{
				float dis = new Vector2(i, j).Length();
				if (dis > minDistance)
				{
					continue;
				}
				var checkTile = TileUtils.SafeGetTile(position.ToTileCoordinates() + new Point(i, j));
				if (checkTile.LiquidAmount <= 0 || (Main.tileBlockLight[checkTile.TileType] && checkTile.HasTile))
				{
					if (dis < minDistance)
					{
						minDistance = dis;
					}
				}
			}
		}
		return minDistance;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.penetrate--;
		if (Projectile.penetrate >= 2)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.velocity *= 0.98f;
		}
		if (Projectile.penetrate < 2)
		{
			Projectile.Kill();
			for (int t = 0; t < 5; t++)
			{
				Dust.NewDustDirect(Projectile.Center + new Vector2(4), 0, 0, DustID.Pot);
			}
		}
		return false;
	}

	public override void OnKill(int timeLeft) => base.OnKill(timeLeft);

	public override bool PreDraw(ref Color lightColor)
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var drawColorVec3 = lightColor.ToVector3() * new Vector3(0.3f, 0.2f, 0.3f);
		var drawColor = new Color(drawColorVec3.X, drawColorVec3.Y, drawColorVec3.Z, 1f);
		float value = WetTimer / 180f;
		if (value > 1)
		{
			value = 1;
		}
		if (WetTimer > 0)
		{
			GradientColor gradientColor = new GradientColor();
			gradientColor.colorList.Add((new Color(0.3f, 0.2f, 0.3f, 1), 0));
			gradientColor.colorList.Add((new Color(0.7f, 0.1f, 0.2f, 1), 0.2f));
			gradientColor.colorList.Add((new Color(0.5f, 0.4f, 0.3f, 1), 0.5f));
			gradientColor.colorList.Add((new Color(0.4f, 0.4f, 0.7f, 1), 0.7f));
			gradientColor.colorList.Add((new Color(1f, 1f, 1f, 1), 1f));
			drawColor = gradientColor.GetColor(value);
		}
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		drawColor.A = 0;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor * value * 0.5f, MathHelper.PiOver2, star.Size() / 2f, Projectile.scale * value, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor * value * 0.5f, 0, star.Size() / 2f, Projectile.scale * value, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor * value * 0.5f, MathHelper.PiOver4, star.Size() / 2f, Projectile.scale * value * 0.3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, drawColor * value * 0.5f, -MathHelper.PiOver4, star.Size() / 2f, Projectile.scale * value * 0.3f, SpriteEffects.None, 0);
		return false;
	}
}
