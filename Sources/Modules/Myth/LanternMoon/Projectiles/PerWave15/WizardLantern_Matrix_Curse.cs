using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.VFX;
using Spine;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class WizardLantern_Matrix_Curse : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public float Timer = 0;

	public NPC OwnerNPC;

	public override void SetDefaults()
	{
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
		Projectile.hide = true;
	}

	public override void AI()
	{
		Timer++;
		if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<WizardLantern>())
		{
			Projectile.Center = OwnerNPC.Center;
			if (Timer is 10)
			{
				for (int i = 0; i < 6; i++)
				{
					float addRot = 0;
					Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, Timer * 1.4f + 80).RotatedBy(i / 6f * MathHelper.TwoPi + addRot), Vector2.zeroVector, ModContent.ProjectileType<CurseSpell>(), 20, 0f, Main.myPlayer, i);
					p0.spriteDirection = -1;
					if (Timer == 10)
					{
						p0.spriteDirection = 1;
					}
					CurseSpell cS = p0.ModProjectile as CurseSpell;
					if (cS is not null)
					{
						cS.OwnerNPC = OwnerNPC;
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

		for (int i = 0; i < 2; i++)
		{
			var somg = new CurseSpellSmoke
			{
				SpinSpeed = -0.03f * Main.rand.NextFloat(0.85f, 1.15f),
				Active = true,
				Visible = true,
				Position = new Vector2(0, Main.rand.NextFloat(90, 100)).RotatedByRandom(MathHelper.TwoPi) + Projectile.Center,
				SpinCenter = Projectile.Center,
				MaxTime = Main.rand.Next(60, 125),
				Scale = Main.rand.NextFloat(10f, 20f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { Main.rand.NextFloat(0f, 1f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int i = 0; i < 1; i++)
		{
			var somg = new CurseSpellDust_blue
			{
				SpinSpeed = -0.03f * Main.rand.NextFloat(0.85f, 1.15f),
				Active = true,
				Visible = true,
				Position = new Vector2(0, Main.rand.NextFloat(90, 100)).RotatedByRandom(MathHelper.TwoPi) + Projectile.Center,
				SpinCenter = Projectile.Center,
				MaxTime = Main.rand.Next(30, 45),
				Scale = 0,
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCs.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var drawPos = Projectile.Center;
		var drawColor = new Color(1f, 0.1f, 0.1f, 0);
		var drawColor2 = new Color(0.6f, 0.0f, 0.05f, 0);
		float disValue = 30;
		float fade = 1f;
		if (Projectile.timeLeft < 60)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		float fade2 = 1f;
		if (Projectile.timeLeft < 120)
		{
			fade2 *= Projectile.timeLeft / 120f;
		}
		fade2 = MathF.Pow(fade2, 0.5f);
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
			Vector2 radius_out = radius * 3f;
			float xCoord = i / 60f * 6;
			bars.Add(drawPos + radius, drawColor2 * colorFade, new Vector3(xCoord + Timer * 0.003f, 1, fade));
			bars.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord + Timer * 0.003f, 0, fade));
			Lighting.AddLight(drawPos + radius * 1.5f, new Vector3(0.7f, 0.01f, 0.03f) * fade);
		}

		var darkColor = new Color(0.52f, 0.57f, 1f, 0) * 0.75f;
		List<Vertex2D> bars_shadowRing = new List<Vertex2D>();
		List<Vertex2D> bars_shadowRing_dark = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue * 2f).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			Vector2 radius_out = new Vector2(0, disValue * 3.25f).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			float xCoord = i / 60f * 3;
			bars_shadowRing_dark.Add(drawPos + radius, Color.White * colorFade, new Vector3(xCoord + Timer * 0.0004f + 0.3f + Projectile.whoAmI * 0.35f, 1, 1));
			bars_shadowRing_dark.Add(drawPos + radius_out, Color.White * colorFade, new Vector3(xCoord + Timer * 0.0004f + 0.3f + Projectile.whoAmI * 0.35f, 0, 1));

			bars_shadowRing.Add(drawPos + radius, darkColor * colorFade, new Vector3(xCoord + Timer * 0.0009f + Projectile.whoAmI * 0.35f, 1, fade2));
			bars_shadowRing.Add(drawPos + radius_out * 1.1f, darkColor, new Vector3(xCoord + Timer * 0.0009f + Projectile.whoAmI * 0.35f, 0, fade2));
			Lighting.AddLight(drawPos + radius * 1.2f, new Vector3(0.0f, 0.01f, 0.4f) * fade2);
		}
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

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect1 = ModAsset.WizardLantern_CurseDarkPart_Matrix_Shader.Value;
		effect1.Parameters["uTransform"].SetValue(model * projection);
		effect1.Parameters["size1"].SetValue(Vector2.One * 0.33333f);
		effect1.Parameters["size2"].SetValue(Vector2.One * 0.33333f);
		effect1.Parameters["warpSize"].SetValue(Vector2.One * 0.56f);
		effect1.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.003f);
		effect1.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[2] = Commons.ModAsset.Noise_Sand_shallow.Value;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_rgb_large.Value;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Wave_full_black.Value;
		if (bars_shadowRing_dark.Count > 0)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_shadowRing_dark.ToArray(), 0, bars_shadowRing_dark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_9.Value;
		if (bars_shadowRing.Count > 0)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_shadowRing.ToArray(), 0, bars_shadowRing.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
