using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class SpringOfQuicksand_Sandflow_subproj : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1200;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.penetrate = 8;
		Projectile.ignoreWater = true;
		Projectile.friendly = true;
		Projectile.scale = 0.5f;
		TrailTexture = Commons.ModAsset.Trail_16.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_16_black.Value;
		TrailWidth = 14;
		TrailLength = 12;
	}

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void Behaviors()
	{
		float mulTime = 1f;
		if (Projectile.timeLeft < 60f)
		{
			mulTime = Projectile.timeLeft / 60f;
		}
		for (int k = 0; k < 1; k++)
		{
			var dust = new SpringOfQuicksand_Dust
			{
				Velocity = Projectile.velocity.RotatedByRandom(0.05f) * Main.rand.NextFloat(0.55f, 0.82f),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.NextFloat(25f, 45f) * mulTime,
				Scale = Main.rand.NextFloat(0.5f, 1.5f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RanSeed = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(dust);
		}
		Projectile.velocity.Y += 0.15f;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int k = 0; k < Projectile.oldPos.Length; k++)
		{
			float fall = k * k * 0.5f;
			fall = Math.Min(fall, 150);
			if (targetHitbox.Intersects(new Rectangle((int)Projectile.oldPos[k].X, (int)Projectile.oldPos[k].Y, Projectile.width, (int)fall)))
			{
				return true;
			}
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color color = TrailColor;
		if (!SelfLuminous)
		{
			color = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		}
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
	}

	public override void DrawTrail() => base.DrawTrail();

	public override float TrailWidthFunction(float factor) => base.TrailWidthFunction(factor);

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		Color drawColor = Color.White;
		if (style == 0)
		{
			drawColor *= extraValue0;
		}
		if (style == 1)
		{
			drawColor = TrailColor;
			if (!SelfLuminous)
			{
				Color lightC = Lighting.GetColor(worldPos.ToTileCoordinates());
				drawColor.R = (byte)(lightC.R * drawColor.R / 255f * 0.75f);
				drawColor.G = (byte)(lightC.G * drawColor.G / 255f * 0.6f);
				drawColor.B = (byte)(lightC.B * drawColor.B / 255f * 0.2f);
			}
		}
		if (style == 2)
		{
			var normalDir = Projectile.velocity;
			if (index >= 1)
			{
				normalDir = SmoothedOldPos[index - 1] - SmoothedOldPos[index];
			}
			normalDir = normalDir.NormalizeSafe();
			drawColor = new Color(1 - (normalDir.X + 25f) / 50f, 1 - (normalDir.Y + 25f) / 50f, WarpStrength, 1);
		}
		return drawColor;
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.45f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}
}
