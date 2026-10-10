using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class AdamantiteClub_round_slash : ModProjectile, IWarpProjectile
{
	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	private Vector2 startCenter;
	private Vector2 startStickCenter;
	public Projectile StickProjectile;

	public override void OnSpawn(IEntitySource source)
	{
		if (StickProjectile == null)
		{
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile != null && projectile.active && projectile.owner == Projectile.owner)
				{
					if ((projectile.Center - Projectile.Center).Length() < 10)
					{
						StickProjectile = projectile;
						break;
					}
				}
			}
		}
		if (StickProjectile == null)
		{
			Projectile.active = false;
			return;
		}
		startStickCenter = StickProjectile.Center;
		startCenter = Projectile.Center;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 7;
	}

	public override bool PreAI()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		return base.PreAI();
	}

	public override bool ShouldUpdatePosition()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		return base.ShouldUpdatePosition();
	}

	public override void AI()
	{
		Projectile.velocity *= 0.85f;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (StickProjectile == null)
		{
			Projectile.active = false;
			return false;
		}
		Vector2 deltaPos = StickProjectile.Center - startStickCenter;
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 9f;
		Vector2 normalizedVelocity = Utils.SafeNormalize(Projectile.velocity, Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Color shadow = Color.White * 0.4f;
		List<Vertex2D> bars = new List<Vertex2D>
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition + deltaPos, shadow, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition + deltaPos, shadow, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition + deltaPos, shadow, new Vector3(1, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition + deltaPos, shadow, new Vector3(1, 1, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star2_black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		float mulColor = width / 10f;
		Color light = new Color(1.48f * lightColor.R / 255f, 0.02f * mulColor * mulColor * lightColor.G / 255f, 0.02f * mulColor * mulColor * lightColor.B / 255f, 0) * width * 0.2f;
		light *= width / 10f;
		normalize *= width * width / 450f;
		bars = new List<Vertex2D>
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition + deltaPos, light, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition + deltaPos, light, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition + deltaPos, light, new Vector3(1, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition + deltaPos, light, new Vector3(1, 1, 0)),
		};
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.AdamantiteSlash.Value;

		// Draw Twice to make the color more vivid.
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch sb)
	{
		if (StickProjectile == null)
		{
			Projectile.active = false;
			return;
		}
		Vector2 deltaPos = StickProjectile.Center - startStickCenter;
		float time = (float)(Main.time * 0.03);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 5f;
		Vector2 normalizedVelocity = Utils.SafeNormalize(Projectile.velocity, Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Vector2 start = startCenter - Main.screenPosition + deltaPos;
		Vector2 end = Projectile.Center - Main.screenPosition + deltaPos;
		Vector2 middle = Vector2.Lerp(start, end, 0.5f);
		float rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		Color alphaColor = Color.White;
		alphaColor.A = 0;
		alphaColor.R = (byte)((rotation + Math.PI) % 6.283 / 6.283 * 255);
		alphaColor.G = 5;
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
