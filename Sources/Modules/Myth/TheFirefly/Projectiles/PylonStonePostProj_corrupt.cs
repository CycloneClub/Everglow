using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class PylonStonePostProj_corrupt : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = true;
		TrailColor = new Color(0.6f, 0.5f, 1f, 0);
		TrailTexture = Commons.ModAsset.Trail_3.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_3_black.Value;
		TrailWidth = 80;
		TrailLength = 60;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		Projectile.rotation = Projectile.velocity.ToRotation();

		if (Projectile.timeLeft >= 60)
		{
			Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
			Vector2 toPlayer = player.Center - Projectile.Center - Projectile.velocity;
			if (toPlayer.Length() > 80)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(toPlayer) * 5f, 0.05f + 0.01f * MathF.Sin((float)Main.time * 0.015f + Projectile.whoAmI) + Projectile.ai[0]);
			}
		}
		if (Projectile.timeLeft < 60)
		{
			DestroyEntity();
		}
	}

	// public override void DrawTrail()
	// {
	// List<Vector2> unSmoothPos = new List<Vector2>();
	// for (int i = 0; i < Projectile.oldPos.Length; ++i)
	// {
	// if (Projectile.oldPos[i] == Vector2.Zero)
	// {
	// break;
	// }

	// unSmoothPos.Add(Projectile.oldPos[i]);
	// }
	// List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(unSmoothPos); // 平滑
	// var SmoothTrail = new List<Vector2>();
	// for (int x = 0; x < SmoothTrailX.Count - 1; x++)
	// {
	// SmoothTrail.Add(SmoothTrailX[x]);
	// }
	// if (unSmoothPos.Count != 0)
	// {
	// SmoothTrail.Add(unSmoothPos[unSmoothPos.Count - 1]);
	// }

	// Vector2 halfSize = new Vector2(Projectile.width, Projectile.height) / 2f;
	// var bars = new List<Vertex2D>();
	// var bars2 = new List<Vertex2D>();
	// var bars3 = new List<Vertex2D>();
	// for (int i = 1; i < SmoothTrail.Count; ++i)
	// {
	// float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
	// if (mulFac > 1f)
	// {
	// mulFac = 1f;
	// }
	// float factor = i / (float)SmoothTrail.Count * mulFac;
	// float width = TrailWidthFunction(factor);
	// float timeValue = (float)Main.time * 0.03f;

	// Vector2 drawPos = SmoothTrail[i] + halfSize;
	// Color drawC = Color.Lerp(TrailColor, new Color(0.4f, 0f, 1f, 0), i / (float)SmoothTrail.Count);

	// bars.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * TrailWidth, drawC * (1 - factor), new Vector3(-factor * 1 + timeValue, 1, width)));
	// bars.Add(new Vertex2D(drawPos, drawC * (1 - factor), new Vector3(-factor * 1 + timeValue, 0.5f, width)));
	// bars2.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * TrailWidth, drawC * (1 - factor), new Vector3(-factor * 1 + timeValue, 0, width)));
	// bars2.Add(new Vertex2D(drawPos, drawC * (1 - factor), new Vector3(-factor * 1 + timeValue, 0.5f, width)));
	// bars3.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * TrailWidth, drawC * (1 - factor), new Vector3(-factor * 1 + timeValue, 1, width)));
	// bars3.Add(new Vertex2D(drawPos, drawC * (1 - factor), new Vector3(-factor * 1 + timeValue, 0.5f, width)));
	// }

	// Main.spriteBatch.End();
	// Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	// Effect effect = TrailShader;
	// var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
	// var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
	// effect.Parameters["uTransform"].SetValue(model * projection);
	// effect.CurrentTechnique.Passes[0].Apply();
	// Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
	// Main.graphics.GraphicsDevice.Textures[0] = TrailTexture;
	// Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
	// if (bars.Count > 3)
	// {
	// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	// }

	// if (bars2.Count > 3)
	// {
	// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
	// }

	// if (bars3.Count > 3)
	// {
	// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
	// }

	// Main.spriteBatch.End();
	// Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	// }
	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			Color drawC = Color.Lerp(TrailColor, new Color(0.4f, 0f, 1f, 0), index / (float)SmoothedOldPos.Count);
			return drawC;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.5f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawTrail();
		if (TimeAfterEntityDestroy <= 0)
		{
			var texDark = ModAsset.MothMiddleBullet_dark.Value;
			var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
			var texStar = Commons.ModAsset.StarSlash.Value;
			Main.spriteBatch.Draw(texDark, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texDark.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

			Vector2 addVel = Vector2.Normalize(Projectile.velocity) * 10;
			Main.spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + addVel, null, TrailColor, 0, texStar.Size() / 2f, 0.5f, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + addVel, null, TrailColor, MathHelper.PiOver2, texStar.Size() / 2f, new Vector2(0.5f, 1f), SpriteEffects.None, 0);
		}
		return false;
	}

	public override void DestroyEntityEffect()
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<PylonStonePostProj_corrupt_explosion>(), 50, 3, Projectile.owner, 10f);
	}
}
