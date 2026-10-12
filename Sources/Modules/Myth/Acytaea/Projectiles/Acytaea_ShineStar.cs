using Everglow.Commons.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class Acytaea_ShineStar : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 40;
		Projectile.height = 40;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		float value = Projectile.timeLeft / 60f;
		Vector2 newScale = new Vector2(1f, value * 2) * MathF.Sin(value * MathF.PI) * 2f;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D dark = Commons.ModAsset.Point_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * value, 0, dark.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 0, 0, 0), Projectile.timeLeft * 0.02f, star.Size() / 2f, newScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 0, 0, 0), Projectile.timeLeft * 0.02f + MathHelper.PiOver2, star.Size() / 2f, newScale, SpriteEffects.None, 0);
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float range = (60 - Projectile.timeLeft) / 60f;
		range = MathF.Sqrt(range);
		range *= 120f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			Vector2 radius = new Vector2(0, -range).RotatedBy(t / 30d * MathHelper.TwoPi);
			bars.Add(new Vertex2D(Projectile.Center + radius - Main.screenPosition, new Color(255, 0, 0, 0), new Vector3(t / 30f, 0, 0)));
			bars.Add(new Vertex2D(Projectile.Center + radius * 0.5f - Main.screenPosition, Color.Transparent, new Vector3(t / 30f, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
