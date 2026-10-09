using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousMoonBladePredict : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	public Vector2 EndPosition = Vector2.zeroVector;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		Projectile.velocity *= 0;
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
		Projectile.timeLeft = 95;
		Projectile.extraUpdates = 3;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override void OnHitPlayer(Player target, Player.HurtInfo info) => base.OnHitPlayer(target, info);

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var toTarget = new Vector2(1, 0).RotatedBy(Projectile.rotation);
		var timeValue = (float)Main.time * 0.04f;

		Texture2D slashHit = Commons.ModAsset.StarSlashGray.Value;
		Vector2 drawPos = EndPosition - Main.screenPosition;
		var targetColor = new Color(0.0f, 0.65f, 0.4f, 0);
		Main.EntitySpriteDraw(slashHit, drawPos, null, targetColor, Projectile.rotation, slashHit.Size() * 0.5f, new Vector2(Projectile.timeLeft / 90f, 2), SpriteEffects.None, 0);

		float sizeValue = Projectile.timeLeft % 30 / 10f + 1;
		Main.EntitySpriteDraw(slashHit, drawPos, null, targetColor * (1 / sizeValue), Projectile.rotation, slashHit.Size() * 0.5f, new Vector2(Projectile.timeLeft / 90f, 2), SpriteEffects.None, 0);
		Main.EntitySpriteDraw(slashHit, drawPos, null, targetColor * 0.5f, timeValue + Projectile.whoAmI, slashHit.Size() * 0.5f, 2, SpriteEffects.None, 0);

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var checkPos = Projectile.Center + toTarget * 40;
		var normal = toTarget.RotatedBy(MathHelper.PiOver2) * 15;

		var predictLine = new List<Vertex2D>();
		var predictLineDark = new List<Vertex2D>();
		var predictLineWidth = new List<Vertex2D>();

		var predictLineUp = new List<Vertex2D>();
		var predictLineDown = new List<Vertex2D>();
		var predictLineDarkUp = new List<Vertex2D>();
		var predictLineDarkDown = new List<Vertex2D>();
		for (int i = 0; i < 120; i++)
		{
			var drawColor = new Color(0.0f, 0.65f, 0.4f, 0);
			var drawDark = new Color(0.4f, 0.3f, 0.3f, 0.3f);
			if (Projectile.timeLeft < 90f)
			{
				float value90_60 = (Projectile.timeLeft - 60f) / 30f;
				drawColor *= value90_60;
			}
			if (Projectile.timeLeft < 60f)
			{
				drawColor *= 0;
				drawDark *= 0;
			}
			float fade = 1f;
			if (i < 4)
			{
				fade = i / 4f;
			}
			drawColor *= fade;
			drawDark *= fade;

			var drawColorSide = new Color(0.0f, 0.65f, 0.4f, 0);
			var drawDarkSide = new Color(0.4f, 0.3f, 0.3f, 0.3f);
			var drawColorArrow = new Color(0.0f, 0.45f, 0.8f, 0) * 0.4f;
			float moveSize = 0f;
			if (Projectile.timeLeft < 90f)
			{
				float value90 = 1 - Projectile.timeLeft / 90f;
				drawColorSide *= MathF.Sin(value90 * MathHelper.Pi);
				drawDarkSide *= MathF.Sin(value90 * MathHelper.Pi);
				drawColorArrow *= MathF.Sin(value90 * MathHelper.Pi);
				if (Projectile.timeLeft > 60)
				{
					float value60 = 3 - Projectile.timeLeft / 30f;
					moveSize = MathF.Sin(value60 * MathHelper.PiOver2);
				}
				else
				{
					moveSize = 1;
				}
			}
			else
			{
				drawColorSide *= 0;
				drawDarkSide *= 0;
				drawColorArrow *= 0;
			}
			drawColorSide *= fade;
			drawDarkSide *= fade;
			drawColorArrow *= fade;
			var normal2 = toTarget.RotatedBy(MathHelper.PiOver2) * 70 * moveSize;
			predictLine.Add(checkPos + normal - Main.screenPosition, drawColor, new Vector3(i / 20f + timeValue, 0, 0));
			predictLine.Add(checkPos - normal - Main.screenPosition, drawColor, new Vector3(i / 20f + timeValue, 1, 0));

			predictLineWidth.Add(checkPos + normal2 - Main.screenPosition, drawColorArrow, new Vector3(i / 20f - timeValue + Projectile.whoAmI / 7f, 0, 0));
			predictLineWidth.Add(checkPos - normal2 - Main.screenPosition, drawColorArrow, new Vector3(i / 20f - timeValue + Projectile.whoAmI / 7f, 1, 0));

			predictLineDark.Add(checkPos + normal - Main.screenPosition, drawDark, new Vector3(i / 20f + timeValue, 0, 0));
			predictLineDark.Add(checkPos - normal - Main.screenPosition, drawDark, new Vector3(i / 20f + timeValue, 1, 0));

			predictLineUp.Add(checkPos + normal + normal2 - Main.screenPosition, drawColorSide, new Vector3(i / 20f + timeValue, 0, 0));
			predictLineUp.Add(checkPos - normal + normal2 - Main.screenPosition, drawColorSide, new Vector3(i / 20f + timeValue, 1, 0));

			predictLineDown.Add(checkPos + normal - normal2 - Main.screenPosition, drawColorSide, new Vector3(i / 20f + timeValue, 0, 0));
			predictLineDown.Add(checkPos - normal - normal2 - Main.screenPosition, drawColorSide, new Vector3(i / 20f + timeValue, 1, 0));

			predictLineDarkUp.Add(checkPos + normal + normal2 - Main.screenPosition, drawDarkSide, new Vector3(i / 20f + timeValue, 0, 0));
			predictLineDarkUp.Add(checkPos - normal + normal2 - Main.screenPosition, drawDarkSide, new Vector3(i / 20f + timeValue, 1, 0));

			predictLineDarkDown.Add(checkPos + normal - normal2 - Main.screenPosition, drawDarkSide, new Vector3(i / 20f + timeValue, 0, 0));
			predictLineDarkDown.Add(checkPos - normal - normal2 - Main.screenPosition, drawDarkSide, new Vector3(i / 20f + timeValue, 1, 0));
			checkPos += toTarget * 16;
			if (Collision.SolidCollision(checkPos, 2, 2))
			{
				for (int j = 0; j < 4; j++)
				{
					int t = i + j;
					fade = (3 - j) / 4f;
					drawColor *= fade;
					drawDark *= fade;
					drawColorSide *= fade;
					drawDarkSide *= fade;
					drawColorArrow *= fade;
					predictLine.Add(checkPos + normal - Main.screenPosition, drawColor, new Vector3(t / 20f + timeValue, 0, 0));
					predictLine.Add(checkPos - normal - Main.screenPosition, drawColor, new Vector3(t / 20f + timeValue, 1, 0));

					predictLineWidth.Add(checkPos + normal2 - Main.screenPosition, drawColorArrow, new Vector3(t / 20f - timeValue + Projectile.whoAmI / 7f, 0, 0));
					predictLineWidth.Add(checkPos - normal2 - Main.screenPosition, drawColorArrow, new Vector3(t / 20f - timeValue + Projectile.whoAmI / 7f, 1, 0));

					predictLineDark.Add(checkPos + normal - Main.screenPosition, drawDark, new Vector3(t / 20f + timeValue, 0, 0));
					predictLineDark.Add(checkPos - normal - Main.screenPosition, drawDark, new Vector3(t / 20f + timeValue, 1, 0));

					predictLineUp.Add(checkPos + normal + normal2 - Main.screenPosition, drawColorSide, new Vector3(t / 20f + timeValue, 0, 0));
					predictLineUp.Add(checkPos - normal + normal2 - Main.screenPosition, drawColorSide, new Vector3(t / 20f + timeValue, 1, 0));

					predictLineDown.Add(checkPos + normal - normal2 - Main.screenPosition, drawColorSide, new Vector3(t / 20f + timeValue, 0, 0));
					predictLineDown.Add(checkPos - normal - normal2 - Main.screenPosition, drawColorSide, new Vector3(t / 20f + timeValue, 1, 0));

					predictLineDarkUp.Add(checkPos + normal + normal2 - Main.screenPosition, drawDarkSide, new Vector3(t / 20f + timeValue, 0, 0));
					predictLineDarkUp.Add(checkPos - normal + normal2 - Main.screenPosition, drawDarkSide, new Vector3(t / 20f + timeValue, 1, 0));

					predictLineDarkDown.Add(checkPos + normal - normal2 - Main.screenPosition, drawDarkSide, new Vector3(t / 20f + timeValue, 0, 0));
					predictLineDarkDown.Add(checkPos - normal - normal2 - Main.screenPosition, drawDarkSide, new Vector3(t / 20f + timeValue, 1, 0));
					checkPos += toTarget * 16;
				}
				break;
			}
		}
		EndPosition = checkPos;

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10_black.Value;
		if (predictLineDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineDark.ToArray(), 0, predictLineDark.Count - 2);
		}
		if (predictLineDarkUp.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineDarkUp.ToArray(), 0, predictLineDarkUp.Count - 2);
		}
		if (predictLineDarkDown.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineDarkDown.ToArray(), 0, predictLineDarkDown.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.ArrowTexture.Value;
		if (predictLineWidth.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineWidth.ToArray(), 0, predictLineWidth.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
		if (predictLine.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLine.ToArray(), 0, predictLine.Count - 2);
		}
		if (predictLineUp.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineUp.ToArray(), 0, predictLineUp.Count - 2);
		}
		if (predictLineDown.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, predictLineDown.ToArray(), 0, predictLineDown.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
