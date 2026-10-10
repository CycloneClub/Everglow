using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities.BuffHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_Wing_Slash : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

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
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 2;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override bool PreAI()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (Projectile.timeLeft < 110 && Projectile.timeLeft > 60)
		{
			Projectile.hostile = true;
		}
		else
		{
			Projectile.hostile = false;
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

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, startCenter, startCenter * 0.4f + Projectile.Center * 0.6f, 2) && Projectile.timeLeft > 30)
		{
			return true;
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(ModContent.BuffType<ShortImmune12>(), 10);
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 18f;
		Vector2 normalizedVelocity = Projectile.velocity.SafeNormalize(Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Color shadow = Color.White * 0.4f;
		var bars = new List<Vertex2D>
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
		Color light = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(0f, 0.3f, 1f, 0), value0 * 3) * width;
		light *= width / 10f;
		normalize *= width * width / 450f;
		bars = new List<Vertex2D>()
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition, light, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition, light, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition, light, new Vector3(1, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition, light, new Vector3(1, 1, 0)),
		};
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;

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
		float time = (float)(Main.time * 0.03);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 5f;
		Vector2 normalizedVelocity = Projectile.velocity.SafeNormalize(Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Vector2 start = startCenter - Main.screenPosition;
		Vector2 end = Projectile.Center - Main.screenPosition;
		var middle = Vector2.Lerp(start, end, 0.5f);
		float rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		Color alphaColor = Color.White;
		alphaColor.A = 0;
		alphaColor.R = (byte)((rotation + Math.PI) % 6.283 / 6.283 * 255);
		alphaColor.G = 5;
		var bars = new List<Vertex2D>
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
