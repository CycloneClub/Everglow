using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class Glow_Fall_Explosion : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.scale = 0;
		Projectile.ai[1] = 0.3f;
		Projectile.ai[2] = Main.rand.NextFloat(0.75f, 1.25f);
	}

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 20;
		Projectile.tileCollide = false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void AI()
	{
		Projectile.scale += Projectile.ai[1];
		Projectile.ai[1] *= 0.7f;
		Projectile.velocity *= 0;
		if (Projectile.timeLeft < 15)
		{
			for (int i = 0; i <= 7; ++i)
			{
				Vector2 star = new Vector2(0, 100 * Projectile.scale * Projectile.ai[2]).RotatedBy(i * Math.PI / 7 * 4 + Projectile.ai[0]);
				Vector2 star2 = new Vector2(0, 100 * Projectile.scale * Projectile.ai[2]).RotatedBy((i + 1) * Math.PI / 7 * 4 + Projectile.ai[0]);
				for (int t = 0; t < (15 - Projectile.timeLeft); t++)
				{
					Vector2 pos = Vector2.Lerp(star, star2, Main.rand.NextFloat(1));
					Dust d0 = Dust.NewDustDirect(Projectile.Center - new Vector2(4) + pos, 8, 8, DustID.MushroomSpray);
					d0.scale *= Main.rand.NextFloat(0.4f, 1f);
					d0.velocity = Utils.SafeNormalize(pos, Vector2.zeroVector) * Projectile.ai[1];
				}
			}
		}
		Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.4f, 0.8f) * Projectile.timeLeft / 5f);
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var bars = new List<Vertex2D>();
		for (int i = 0; i <= 7; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			Color drawC = new Color(0f, 0.4f, 0.8f, 0);
			Vector2 star = new Vector2(0, 100 * Projectile.scale * Projectile.ai[2]).RotatedBy(i * Math.PI / 7 * 4 + Projectile.ai[0]);
			float width = 20f;
			if (Projectile.timeLeft < 20)
			{
				width = Projectile.timeLeft;
			}
			Vector2 innerStar = Utils.SafeNormalize(star, Vector2.zeroVector) * width;
			if (star.Length() < width)
			{
				innerStar = Vector2.zeroVector;
			}
			else
			{
				innerStar = star - innerStar;
			}
			bars.Add(new Vertex2D(drawPos + star, drawC, new Vector3(i / 7f, 1, 0)));
			bars.Add(new Vertex2D(drawPos + innerStar, drawC, new Vector3(i / 7f, 0.5f, 0)));
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_6.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
	}
}
