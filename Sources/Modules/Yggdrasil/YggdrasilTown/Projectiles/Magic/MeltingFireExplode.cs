using Everglow.Commons.Graphics;
using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class MeltingFireExplode : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => "Terraria/Images/Projectile_0";

	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.timeLeft = 8;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.6f, 0.1f) * Projectile.timeLeft / 8);

		var flareColor = new GradientColor();
		flareColor.colorList.Add((new Color(1f, 0.8f, 0.6f), 0f));

		flareColor.colorList.Add((new Color(1f, 0.6f, 0.2f), 0.3f));
		flareColor.colorList.Add((new Color(0.6f, 0.2f, 0.1f), 1f));

		if (Projectile.timeLeft == 5)
		{
			for (int i = 1; i < 15; i++)
			{
				float factor = i / 15f;
				var flare = new Flare();
				flare.Color = flareColor;
				flare.Position = Projectile.Center - new Vector2(0, -25 + (float)Math.Pow(factor, 2.5f) * 80);
				flare.Scale = 0.3f + 0.3f * (1 - factor) * (1 - factor);
				flare.Gravity = -0.05f;
				flare.Velocity = Main.rand.NextVector2Circular(1, 1);
				flare.Velocity.Y -= 1;
				flare.Velocity *= 2;

				flare.MaxTimeLeft = 25f;
				flare.TimeLeft = 25 - factor * 10;

				Ins.VFXManager.Add(flare);
			}
		}
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, 15) - Main.screenPosition, null, new Color(1, 1, 1, 0f) * 0.8f, 0, tex.Size() / 2, 2, 0, 0);
		return false;
	}
}
