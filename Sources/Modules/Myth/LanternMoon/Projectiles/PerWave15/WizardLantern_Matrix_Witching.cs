using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class WizardLantern_Matrix_Witching : ModProjectile
{
	public float Timer = 0;

	public NPC OwnerNPC;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 180;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0.1f;
	}

	public override void AI()
	{
		Timer++;
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<WizardLantern>())
		{
			Projectile.Center = OwnerNPC.Center;
			if (Timer is 10)
			{
				for (int i = 0; i < 8; i++)
				{
					float addRot = 0;
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, Timer * 1.4f + 80).RotatedBy(i / 8f * MathHelper.TwoPi + addRot), Vector2.zeroVector, ModContent.ProjectileType<WitchingSpell>(), 20, 0f, Main.myPlayer, i);
					p0.spriteDirection = -1;
					if (Timer == 10)
					{
						p0.spriteDirection = 1;
					}
					WitchingSpell wS = p0.ModProjectile as WitchingSpell;
					if (wS is not null)
					{
						wS.OwnerNPC = OwnerNPC;
					}
				}
			}
		}
		else
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
				Timer = 60;
			}
		}
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var drawPos = Projectile.Center;
		var drawColor = new Color(0.2f, 0.8f, 0.3f, 0);
		var drawColor2 = new Color(0.0f, 0.4f, 0.3f, 0);
		var drawColor3 = new Color(0.0f, 0.6f, 0.3f, 0);
		float disValue = 55;
		float fade = 1f;
		if (Projectile.timeLeft < 60)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		float colorFade = 1f;
		if (Timer < 30)
		{
			colorFade *= Timer / 30f;
		}
		if (Projectile.timeLeft < 30)
		{
			colorFade *= Projectile.timeLeft / 30f;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			Vector2 radius_out = radius * 2f;
			float xCoord = i / 60f * 6;
			bars.Add(drawPos + radius, drawColor2 * colorFade, new Vector3(xCoord + Timer * 0.003f, 1, fade));
			bars.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord + Timer * 0.003f, 0, fade));
			Lighting.AddLight(drawPos + radius * 1.5f, new Vector3(0.2f, 0.8f, 0.3f) * fade);
			Lighting.AddLight(drawPos + radius * 3f, new Vector3(0.2f, 0.8f, 0.3f) * fade);
		}

		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue * 0.8f).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			Vector2 radius_out = radius.NormalizeSafe() * (radius.Length() + 3);
			float xCoord = i / 60f;
			bars2.Add(drawPos + radius, drawColor2 * colorFade, new Vector3(xCoord + Timer * 0.003f, 1, fade));
			bars2.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord + Timer * 0.003f, 0, fade));
		}

		List<Vertex2D> bars3 = new List<Vertex2D>();
		for (int i = 0; i < 8; i++)
		{
			float value = i + 0.5f;
			float length = 33;
			float width = 1f;
			Vector2 pos = new Vector2(0, -60).RotatedBy(value * MathHelper.TwoPi / 8f);
			Vector2 dirUL = new Vector2(-width, -length).RotatedBy(value * MathHelper.TwoPi / 8f);
			Vector2 dirUR = new Vector2(width, -length).RotatedBy(value * MathHelper.TwoPi / 8f);
			Vector2 dirDL = new Vector2(-width, length).RotatedBy(value * MathHelper.TwoPi / 8f);
			Vector2 dirDR = new Vector2(width, length).RotatedBy(value * MathHelper.TwoPi / 8f);
			bars3.Add(drawPos + pos + dirUL, drawColor3 * colorFade, new Vector3(0 + Timer * 0.05f, 0, fade));
			bars3.Add(drawPos + pos + dirUR, drawColor3 * colorFade, new Vector3(1 + Timer * 0.05f, 0, fade));
			bars3.Add(drawPos + pos + dirDL, drawColor3 * colorFade, new Vector3(0 + Timer * 0.05f, 1, fade));

			bars3.Add(drawPos + pos + dirUR, drawColor3 * colorFade, new Vector3(1 + Timer * 0.05f, 0, fade));
			bars3.Add(drawPos + pos + dirDL, drawColor3 * colorFade, new Vector3(0 + Timer * 0.05f, 1, fade));
			bars3.Add(drawPos + pos + dirDR, drawColor3 * colorFade, new Vector3(1 + Timer * 0.05f, 1, fade));
		}

		List<Vertex2D> outlines = new List<Vertex2D>();
		for (int i = 0; i < 8; i++)
		{
			Texture2D piece = ModAsset.WizardLantern_Matrix_Witching_Outline.Value;
			Rectangle frame = new Rectangle(24 * i, 0, 24, 24);
			Vector2 pos = new Vector2(0, -60).RotatedBy(i * MathHelper.TwoPi / 8f);
			Vector2 dir = new Vector2(-12, -12).RotatedBy(i * MathHelper.TwoPi / 8f);
			outlines.Add(drawPos + pos + dir, drawColor2 * colorFade, new Vector3(frame.X / (float)piece.Width, frame.Y / (float)piece.Height, fade));
			outlines.Add(drawPos + pos + dir.RotatedBy(MathHelper.PiOver2), drawColor2 * colorFade, new Vector3((frame.X + frame.Width) / (float)piece.Width, frame.Y / (float)piece.Height, fade));
			outlines.Add(drawPos + pos + dir.RotatedBy(MathHelper.PiOver2 * 3), drawColor2 * colorFade, new Vector3(frame.X / (float)piece.Width, (frame.Y + frame.Height) / (float)piece.Height, fade));

			outlines.Add(drawPos + pos + dir.RotatedBy(MathHelper.PiOver2), drawColor2 * colorFade, new Vector3((frame.X + frame.Width) / (float)piece.Width, frame.Y / (float)piece.Height, fade));
			outlines.Add(drawPos + pos + dir.RotatedBy(MathHelper.PiOver2 * 3), drawColor2 * colorFade, new Vector3(frame.X / (float)piece.Width, (frame.Y + frame.Height) / (float)piece.Height, fade));
			outlines.Add(drawPos + pos + dir.RotatedBy(MathHelper.Pi), drawColor2 * colorFade, new Vector3((frame.X + frame.Width) / (float)piece.Width, (frame.Y + frame.Height) / (float)piece.Height, fade));
		}

		Color fishColor = new Color(0.0f, 0.4f, 0.3f, 0.5f);
		List<Vertex2D> fishes = new List<Vertex2D>();
		Vector2 fishRot = new Vector2(-40).RotatedBy(-Main.time * 0.009f + Projectile.whoAmI);
		fishes.Add(drawPos + fishRot, fishColor * colorFade, new Vector3(0, 0, fade));
		fishes.Add(drawPos + fishRot.RotatedBy(MathHelper.PiOver2), fishColor * colorFade, new Vector3(1, 0, fade));
		fishes.Add(drawPos + fishRot.RotatedBy(MathHelper.PiOver2 * 3), fishColor * colorFade, new Vector3(0, 1, fade));

		fishes.Add(drawPos + fishRot.RotatedBy(MathHelper.PiOver2), fishColor * colorFade, new Vector3(1, 0, fade));
		fishes.Add(drawPos + fishRot.RotatedBy(MathHelper.PiOver2 * 3), fishColor * colorFade, new Vector3(0, 1, fade));
		fishes.Add(drawPos + fishRot.RotatedBy(MathHelper.Pi), fishColor * colorFade, new Vector3(1, 1, fade));

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect0 = ModAsset.WizardLantern_Thunder_Matrix_Shader.Value;
		effect0.Parameters["uTransform"].SetValue(model * projection);
		effect0.Parameters["size1"].SetValue(Vector2.One);
		effect0.CurrentTechnique.Passes[0].Apply();

		if (bars.Count > 0)
		{
			var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (bars2.Count > 0)
		{
			var texture = Commons.ModAsset.Trail_5.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}

		if (bars3.Count > 0)
		{
			var texture = Commons.ModAsset.Trail_5.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars3.ToArray(), 0, bars3.Count / 3);
		}

		if (outlines.Count > 0)
		{
			var texture = ModAsset.WizardLantern_Matrix_Witching_Outline.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, outlines.ToArray(), 0, outlines.Count / 3);
		}

		if (fishes.Count > 0)
		{
			var texture = ModAsset.WizardLantern_Matrix_Witching_WhiteBlackFish.Value;
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, fishes.ToArray(), 0, fishes.Count / 3);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D blackSpot = Commons.ModAsset.LightPoint2_black.Value;
		Texture2D redSpot = Commons.ModAsset.LightPoint2.Value;
		for (int i = 0; i < 8; i++)
		{
			Vector2 pos = Projectile.Center + new Vector2(0, -80).RotatedBy(i / 8f * MathHelper.TwoPi);
			float scale = 0.5f * colorFade * (1 + MathF.Sin(i / 8f * MathHelper.Pi * 3 + (float)Main.time * 0.05f) * 0.25f);
			Main.EntitySpriteDraw(blackSpot, pos - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), 0, blackSpot.Size() * 0.5f, scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(redSpot, pos - Main.screenPosition, null, new Color(1f, 0.05f, 0.1f, 0), 0, redSpot.Size() * 0.5f, scale, SpriteEffects.None, 0);
			Lighting.AddLight(pos, new Vector3(0.75f, 0, 0) * colorFade);
		}
		for (int i = 0; i < 24; i++)
		{
			Vector2 pos = Projectile.Center + new Vector2(0, -120).RotatedBy(i / 24f * MathHelper.TwoPi - Main.time * 0.006f);
			float scale = 0.5f * colorFade * (1 + MathF.Sin(i / 8f * MathHelper.Pi * 3 + (float)Main.time * 0.05f) * 0.4f) * 0.25f;
			Main.EntitySpriteDraw(blackSpot, pos - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), 0, blackSpot.Size() * 0.5f, scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(redSpot, pos - Main.screenPosition, null, new Color(1f, 0.05f, 0.1f, 0), 0, redSpot.Size() * 0.5f, scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
