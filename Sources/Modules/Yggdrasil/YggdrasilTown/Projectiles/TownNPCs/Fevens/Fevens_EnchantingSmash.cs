using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.Common.Elevator.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_EnchantingSmash : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	public Vector2 EndPosition = Vector2.zeroVector;

	public Vector2 Toward = Vector2.One;

	public override void OnSpawn(IEntitySource source)
	{
		Player target = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		Vector2 toTarget = target.Center - Projectile.Center;
		Toward = toTarget;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override void OnHitPlayer(Player target, Player.HurtInfo info) => base.OnHitPlayer(target, info);

	public override void AI()
	{
		Projectile.velocity *= 0;
		Player target = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		Vector2 toTarget = target.Center - Projectile.Center;
		float stepValue = 0.15f * (Projectile.timeLeft - 10f) / 110f;
		Toward = toTarget * stepValue + Toward * (1 - stepValue);
		if (Projectile.timeLeft > 10)
		{
			Projectile.rotation = Toward.ToRotation();
		}
		if (Projectile.timeLeft == 1)
		{
			var p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), EndPosition, Vector2.zeroVector, ModContent.ProjectileType<Fevens_EnchantingSmash_Explosion>(), 260, 2, default, Projectile.Center.X, Projectile.Center.Y);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var toTarget = new Vector2(1, 0).RotatedBy(Projectile.rotation);
		var timeValue = (float)Main.time * 0.04f;
		DrawStar(Projectile.Center + toTarget * 40 - Main.screenPosition);
		Texture2D wingTarget = ModAsset.Fevens_WingTarget.Value;
		Vector2 drawPos = EndPosition - Main.screenPosition;
		var targetColor = new Color(0.4f, 0.05f, 0.7f, 0);
		Main.EntitySpriteDraw(wingTarget, drawPos, null, targetColor, 0, wingTarget.Size() * 0.5f, 1f, SpriteEffects.None, 0);

		float sizeValue = Projectile.timeLeft % 30 / 10f + 1;
		Main.EntitySpriteDraw(wingTarget, drawPos, null, targetColor * (1 / sizeValue), 0, wingTarget.Size() * 0.5f, sizeValue, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(wingTarget, drawPos, null, targetColor * 0.5f, timeValue, wingTarget.Size() * 0.5f, 2, SpriteEffects.None, 0);
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var checkPos = Projectile.Center + toTarget * 40;
		var normal = toTarget.RotatedBy(MathHelper.PiOver2) * 15;
		var drawColor = new Color(0.2f, 0f, 0.4f, 0);
		var drawDark = new Color(0.4f, 0.3f, 0.3f, 0.8f);
		var predictLine = new List<Vertex2D>();
		var predictLineDark = new List<Vertex2D>();
		for (int i = 0; i < 120; i++)
		{
			predictLine.Add(checkPos + normal - Main.screenPosition, drawColor, new Vector3(i / 20f + timeValue, 0, 0));
			predictLine.Add(checkPos - normal - Main.screenPosition, drawColor, new Vector3(i / 20f + timeValue, 1, 0));

			predictLineDark.Add(checkPos + normal - Main.screenPosition, drawDark, new Vector3(i / 20f + timeValue, 0, 0));
			predictLineDark.Add(checkPos - normal - Main.screenPosition, drawDark, new Vector3(i / 20f + timeValue, 1, 0));
			checkPos += toTarget * 16;
			if (Collision.SolidCollision(checkPos, 2, 2))
			{
				break;
			}
		}
		EndPosition = checkPos;

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10_black.Value;
		if (predictLineDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineDark.ToArray(), 0, predictLineDark.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
		if (predictLine.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLine.ToArray(), 0, predictLine.Count - 2);
		}

		Vector2 rangeCenter = EndPosition - Main.screenPosition;
		float maxRange = 750;
		float value = (120 - Projectile.timeLeft) / 120f;
		value = MathF.Pow(value, 0.5f) * maxRange;
		var predictRange = new List<Vertex2D>();
		var predictRangeDark = new List<Vertex2D>();

		var predictDuration = new List<Vertex2D>();
		var predictDurationDark = new List<Vertex2D>();
		for (int i = 0; i < 200; i++)
		{
			var range = new Vector2(0, maxRange).RotatedBy(i / 200f * MathHelper.TwoPi);
			var rangeInner = new Vector2(0, maxRange - 30).RotatedBy(i / 200f * MathHelper.TwoPi);

			var duration = new Vector2(0, value).RotatedBy(i / 200f * MathHelper.TwoPi);
			var durationInner = new Vector2(0, value - 30).RotatedBy(i / 200f * MathHelper.TwoPi);
			predictRange.Add(rangeCenter + range, drawColor * 2, new Vector3(i / 20f + timeValue, 0, 0));
			predictRange.Add(rangeCenter + rangeInner, drawColor * 2, new Vector3(i / 20f + timeValue, 1, 0));

			predictRangeDark.Add(rangeCenter + range, drawDark * 2, new Vector3(i / 20f + timeValue, 0, 0));
			predictRangeDark.Add(rangeCenter + rangeInner, drawDark * 2, new Vector3(i / 20f + timeValue, 1, 0));

			predictDuration.Add(rangeCenter + duration, drawColor, new Vector3(i / 20f + timeValue, 0, 0));
			predictDuration.Add(rangeCenter + durationInner, drawColor, new Vector3(i / 20f + timeValue, 1, 0));

			predictDurationDark.Add(rangeCenter + duration, drawDark, new Vector3(i / 20f + timeValue, 0, 0));
			predictDurationDark.Add(rangeCenter + durationInner, drawDark, new Vector3(i / 20f + timeValue, 1, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10_black.Value;
		if (predictRangeDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictRangeDark.ToArray(), 0, predictRangeDark.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
		if (predictRange.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictRange.ToArray(), 0, predictRange.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10_black.Value;
		if (predictDurationDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictDurationDark.ToArray(), 0, predictDurationDark.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
		if (predictDuration.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictDuration.ToArray(), 0, predictDuration.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawStar(Vector2 position, float scale = 1f)
	{
		var timeValue = (float)Main.time * 0.04f;
		var timeLeftValue = 1 - Projectile.timeLeft / 120f;
		Vector2 drawPos = position;
		Texture2D star = Commons.ModAsset.StarSlashGray.Value;
		Texture2D starBlack = Commons.ModAsset.StarSlash_black.Value;
		var darkDraw = Color.Lerp(Color.White * 0.3f, Color.White * 0.8f, timeLeftValue);
		var starColor = Color.Lerp(new Color(0.2f, 0.05f, 0.4f, 0), new Color(0.6f, 0.25f, 1f, 0), timeLeftValue);
		var starColor2 = Color.Lerp(new Color(0.6f, 0.25f, 1f, 0), new Color(1f, 0.85f, 1f, 0), timeLeftValue);

		float width = (MathF.Sin(timeValue * 12) + 1.5f) / 2f;
		Main.EntitySpriteDraw(starBlack, drawPos, null, darkDraw, MathHelper.PiOver2, starBlack.Size() * 0.5f, new Vector2(width / 2f, 2f + timeLeftValue) * scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(starBlack, drawPos, null, darkDraw, 0, starBlack.Size() * 0.5f, new Vector2(width / 2f, 1f + timeLeftValue) * scale, SpriteEffects.None, 0);

		Main.EntitySpriteDraw(star, drawPos, null, starColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(width, 2f + timeLeftValue) * scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, drawPos, null, starColor, 0, star.Size() * 0.5f, new Vector2(width, 1f + timeLeftValue) * scale, SpriteEffects.None, 0);

		Main.EntitySpriteDraw(star, drawPos, null, starColor, MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(width, (1f + timeLeftValue) * 0.3f) * scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, drawPos, null, starColor, -MathHelper.PiOver4, star.Size() * 0.5f, new Vector2(width, (1f + timeLeftValue) * 0.3f) * scale, SpriteEffects.None, 0);

		Main.EntitySpriteDraw(star, drawPos, null, starColor2, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(1, 0.5f) * scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, drawPos, null, starColor2, 0, star.Size() * 0.5f, new Vector2(1, 0.5f) * scale, SpriteEffects.None, 0);
	}
}
