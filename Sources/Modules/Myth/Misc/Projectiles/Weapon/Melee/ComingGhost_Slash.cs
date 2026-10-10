using Everglow.Myth.Misc.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class ComingGhost_Slash : ModProjectile, IWarpProjectile
{
	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	private Vector2 startCenter;

	public override void OnSpawn(IEntitySource source)
	{
		startCenter = Projectile.Center;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 3;
	}

	public override void AI()
	{
		Projectile.velocity *= 0.93f;
		if (Main.rand.NextBool(4) && Projectile.timeLeft > 30)
		{
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * Main.rand.NextFloat(3f, 6f);
			Dust dust = Dust.NewDustDirect(Projectile.position - Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(0f, (120 - Projectile.timeLeft) * 3f), Projectile.width, Projectile.height, ModContent.DustType<Crow>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.7f));
			dust.velocity = vel;
		}
		if (Main.rand.NextBool(12) && Projectile.timeLeft > 30)
		{
			Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * Main.rand.NextFloat(3f, 6f);
			Dust dust = Dust.NewDustDirect(Projectile.position - Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(0f, (120 - Projectile.timeLeft) * 3f), Projectile.width, Projectile.height, DustID.LifeDrain, 0, 0, 0, default, Main.rand.NextFloat(0.45f, 0.7f));
			dust.velocity = vel;
			dust.noGravity = true;
		}
		if (Projectile.timeLeft == 114)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Vector2 hitCenter = startCenter + Vector2.Normalize(Projectile.velocity) * 120f;
		lightColor = Lighting.GetColor((int)(hitCenter.X / 16f), (int)(hitCenter.Y / 16f));
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 10f;
		Vector2 normalizedVelocity = Utils.SafeNormalize(Projectile.velocity, Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Color shadow = Color.White * 0.7f;
		List<Vertex2D> bars = new List<Vertex2D>
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition, shadow, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition, shadow, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition, shadow, new Vector3(1, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition, shadow, new Vector3(1, 1, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star2_black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Color light = new Color(1f, 0.005f, 0.003f, 0) * width;
		light *= width / 10f;
		normalize *= width * width / 450f;
		bars = new List<Vertex2D>
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition, light, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition, light, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition, light, new Vector3(1, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition, light, new Vector3(1, 1, 0)),
		};
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StabbingProjectile.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		return false;
	}

	public void DrawWarp(VFXBatch sb)
	{
		float time = (float)(Main.time * 0.03);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 5f;
		Vector2 normalizedVelocity = Utils.SafeNormalize(Projectile.velocity, Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Vector2 start = startCenter - Main.screenPosition;
		Vector2 end = Projectile.Center - Main.screenPosition;
		Vector2 middle = Vector2.Lerp(start, end, 0.5f);
		float rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		Color alphaColor = Color.White;
		alphaColor.A = 0;
		alphaColor.R = (byte)((rotation + Math.PI) % 6.283 / 6.283 * 255);
		alphaColor.G = 15;
		List<Vertex2D> bars = new List<Vertex2D>
		{
			new Vertex2D(start - normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.3f, 0)),
			new Vertex2D(start + normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.7f, 0)),
			new Vertex2D(middle - normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.3f, 0.5f)),
			new Vertex2D(middle + normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.7f, 0.5f)),
			new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
			new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
		};
		sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
	}
}
