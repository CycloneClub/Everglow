using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class GreenSungloShield_B : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.GreenSungloShield_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 48;
		Projectile.height = 64;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1200;
		Projectile.penetrate = -1;
	}

	private float timer = 0;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Main.player[Projectile.owner].Center;
		Projectile.timeLeft++;

		if (player.HeldItem.type == ModContent.ItemType<GreenSungloStaff>())
		{
			if (timer <= 157)
			{
				timer++;
			}
			else
			{
				timer = 157;
			}
		}
		else
		{
			timer -= 0.2f;
		}

		if (timer <= 0)
		{
			Projectile.Kill();
		}

		Lighting.AddLight(Projectile.Center, new Vector3(0.375f, 0.75f, 0.375f) * timer / 157);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect shineEffect = ModAsset.GreenSungloShield_VFX.Value;
		shineEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);
		shineEffect.Parameters["uNoise"].SetValue(Commons.ModAsset.NoiseWave.Value);
		shineEffect.CurrentTechnique.Passes["MagicCircle_Pixel"].Apply();

		Player Owner = Main.player[Projectile.owner];
		var CirTexture = Commons.ModAsset.Point.Value;
		var CirPosition = Owner.gravDir == 1 ? Owner.Bottom : Owner.Top;
		CirPosition = CirPosition - Main.screenPosition + new Vector2(0, Owner.gravDir);
		var CirScale = new Vector2(0.3f, 0.4f);
		var CirRotation = Owner.gravDir == 1 ? 0 : MathF.PI;
		float process = timer / 157;
		Main.spriteBatch.Draw(CirTexture, CirPosition, null, Color.White * process, CirRotation, new Vector2(CirTexture.Width / 2, CirTexture.Height), CirScale, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

		int a = 3;
		int b = 2;
		float Width = 15f;
		float count = Math.Min(628, timer * 4);

		var thorns = new List<Vertex2D>();
		for (float alpha = 10f; alpha >= 1; alpha--)
		{
			thorns.Clear();
			Color color = new Color(0.5f / alpha, 1f / alpha, 0.5f / alpha, 0);
			for (float i = 0; i <= count; i++)
			{
				Vector3 pos0 = new Vector3(MathF.Cos(a * (i + 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(b * (i + 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(a * (i + 1) * 0.01f - (float)Main.time * 0.025f)) * 25;
				Vector3 pos = new Vector3(MathF.Cos(a * i * 0.01f - (float)Main.time * 0.025f), MathF.Sin(b * i * 0.01f - (float)Main.time * 0.025f), MathF.Sin(a * i * 0.01f - (float)Main.time * 0.025f)) * 25;
				Vector3 pos2 = new Vector3(MathF.Cos(a * (i - 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(b * (i - 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(a * (i - 1) * 0.01f - (float)Main.time * 0.025f)) * 25;
				Vector3 normal3D = Vector3.Normalize(Vector3.Cross(pos - pos0, pos - pos2));
				Vector2 normal2D = new Vector2(normal3D.X, normal3D.Y);
				normal2D = MathUtils.Lerp(0.25f, normal2D, Vector2.UnitX);
				thorns.Add(new Vertex2D(Projectile.Center + new Vector2(pos.X, pos.Y) + normal2D * Width * (0.25f * alpha + 0.75f) - Main.screenPosition, color * 0.75f, new Vector3(0, (float)(i / 80f), pos.Z)));
				thorns.Add(new Vertex2D(Projectile.Center + new Vector2(pos.X, pos.Y) - normal2D * Width * (0.25f * alpha + 0.75f) - Main.screenPosition, color * 0.75f, new Vector3(1, (float)(i / 80f), pos.Z)));
			}

			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.GreenSungloShield.Value;

			if (thorns.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, thorns.ToArray(), 0, thorns.Count - 2);
			}
		}

		thorns.Clear();
		for (float i = 0; i <= count; i++)
		{
			Vector3 pos0 = new Vector3(MathF.Cos(a * (i + 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(b * (i + 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(a * (i + 1) * 0.01f - (float)Main.time * 0.025f)) * 25;
			Vector3 pos = new Vector3(MathF.Cos(a * i * 0.01f - (float)Main.time * 0.025f), MathF.Sin(b * i * 0.01f - (float)Main.time * 0.025f), MathF.Sin(a * i * 0.01f - (float)Main.time * 0.025f)) * 25;
			Vector3 pos2 = new Vector3(MathF.Cos(a * (i - 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(b * (i - 1) * 0.01f - (float)Main.time * 0.025f), MathF.Sin(a * (i - 1) * 0.01f - (float)Main.time * 0.025f)) * 25;
			Vector3 normal3D = Vector3.Normalize(Vector3.Cross(pos - pos0, pos - pos2));
			Vector2 normal2D = new Vector2(normal3D.X, normal3D.Y);
			normal2D = MathUtils.Lerp(0.25f, normal2D, Vector2.UnitX);
			thorns.Add(new Vertex2D(Projectile.Center + new Vector2(pos.X, pos.Y) + normal2D * Width - Main.screenPosition, Color.White * 0.6f, new Vector3(0, (float)(i / 80f), pos.Z)));
			thorns.Add(new Vertex2D(Projectile.Center + new Vector2(pos.X, pos.Y) - normal2D * Width - Main.screenPosition, Color.White * 0.6f, new Vector3(1, (float)(i / 80f), pos.Z)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.GreenSungloThorns.Value;

		if (thorns.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, thorns.ToArray(), 0, thorns.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		return false;
	}
}
