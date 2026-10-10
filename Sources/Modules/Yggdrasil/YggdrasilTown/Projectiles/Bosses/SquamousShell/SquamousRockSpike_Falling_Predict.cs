using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousRockSpike_Falling_Predict : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	public Vector2 EndPosition = Vector2.zeroVector;

	public override void OnSpawn(IEntitySource source)
	{
		for (int t = 0; t < 300; t++)
		{
			Projectile.position.Y -= 10;
			if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Projectile.position.Y += 100;
				break;
			}
		}
		Projectile.rotation = Projectile.velocity.ToRotation();
		Projectile.velocity *= 0;
		Projectile.timeLeft += Main.rand.Next(-5, 60);
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
		Projectile.extraUpdates = 0;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override void OnHitPlayer(Player target, Player.HurtInfo info) => base.OnHitPlayer(target, info);

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.timeLeft == 5)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 18), ModContent.ProjectileType<SquamousRockSpike_Falling>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.owner);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var toTarget = new Vector2(1, 0).RotatedBy(Projectile.rotation);
		var timeValue = (float)Main.time * 0.04f;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.DissolveWithLight.Value;
		float dissolveDuration = 1.2f;
		if (Projectile.timeLeft < 30)
		{
			dissolveDuration = Projectile.timeLeft / 30f - 0.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_1.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uLightColor"].SetValue(Color.White.ToVector4());
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.12f, 0.10f, 0.07f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(0.5f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		Texture2D mark = Commons.ModAsset.ExclamationMark.Value;
		var targetColor = new Color(0.25f, 0.05f, 0.15f, 1);
		float markSize = 0.5f;
		if (Projectile.timeLeft > 50f)
		{
			markSize *= (Projectile.timeLeft - 50f) / 20f + 1;
			targetColor *= 1f / MathF.Pow(markSize, 3);
		}
		else
		{
			Main.EntitySpriteDraw(mark, EndPosition + new Vector2(0, -100), null, targetColor, 0, mark.Size() * 0.5f, markSize, SpriteEffects.None, 0);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (Projectile.timeLeft > 50f)
		{
			Main.EntitySpriteDraw(mark, EndPosition - Main.screenPosition + new Vector2(0, -100), null, targetColor, 0, mark.Size() * 0.5f, markSize, SpriteEffects.None, 0);
		}

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
			var drawColor = new Color(1f, 0.05f, 0.15f, 0);
			var drawDark = new Color(0.4f, 0.3f, 0.3f, 0.7f);
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

			var drawColorSide = new Color(0.2f, 0.05f, 0.15f, 0);
			var drawDarkSide = new Color(0.4f, 0.3f, 0.3f, 0.3f);
			var drawColorArrow = new Color(0.30f, 0.15f, 0.08f, 0) * 0.4f;
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
