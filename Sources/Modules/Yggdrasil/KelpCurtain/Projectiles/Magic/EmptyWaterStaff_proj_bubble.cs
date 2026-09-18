using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class EmptyWaterStaff_proj_bubble : ModProjectile, IWarpProjectile_warpStyle2
{
	public NPC Target;

	public int Timer;

	public float BubbleScale = 1f;

	public List<Vector2> BubbleBound = new List<Vector2>();

	public override string Texture => ModAsset.EmptyWaterStaff_Mod;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 180;
		Projectile.penetrate = -1;
		Projectile.hide = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.npc.Length)
		{
			Target = Main.npc[(int)Projectile.ai[0]];
			Projectile.velocity = new Vector2(0, -2);
			if (Projectile.wet)
			{
				Projectile.timeLeft = 360;
			}
		}
		else
		{
			Projectile.active = false;
		}
	}

	public override void AI()
	{
		Timer++;
		if (!CanTrap(Target))
		{
			if (Timer > 5)
			{
				if (Projectile.timeLeft > 2)
				{
					Projectile.timeLeft = 1;
				}
			}
			return;
		}
		if (Timer < 20)
		{
			BubbleScale = (MathF.Sin(Timer / 20f * MathHelper.Pi * 1.5f - MathHelper.PiOver2) + 1) / 2f * CalculateTargetScale() * 1.5f;
		}
		else
		{
			BubbleScale = CalculateTargetScale();
		}
		if (Timer == 20)
		{
			Vector2 oldCenter = Projectile.Center;
			Projectile.width = Projectile.height = (int)(BubbleScale * 1.6f);
			Projectile.Center = oldCenter;
		}
		Projectile.velocity *= 0.99f;
		Vector2 targetToProj = Projectile.Center + new Vector2(0, -15f) - Target.Center - Target.velocity;
		targetToProj /= 30f;
		Target.velocity += targetToProj;
		Target.velocity *= 0.95f;
		if (Collision.IsWorldPointSolid(Target.Top + new Vector2(0, -4)) || (Target.collideY && !Collision.IsWorldPointSolid(Target.Bottom + new Vector2(0, 2))))
		{
			Projectile.velocity.Y *= 0;
		}
		if (Projectile.timeLeft == 1)
		{
			Projectile.friendly = true;
		}
	}

	public void GenerateBubbleBound()
	{
		BubbleBound = new List<Vector2>();
		float radius = BubbleScale;
		for (int k = 0; k < 121; k++)
		{
			Vector2 bound = new Vector2(0, radius + GetRandomValueMove(k) * 3).RotatedBy(k / 120f * MathHelper.TwoPi);
			BubbleBound.Add(bound);
		}
	}

	public float GetRandomValueMove(float index)
	{
		float value = 0;
		for (int i = 0; i < 2; i++)
		{
			float timeValue = (float)Main.time / 24f * MathF.Pow(2, i) + Projectile.whoAmI;
			timeValue = MathF.Cos(timeValue);
			value += MathF.Sin(index / 60f * MathHelper.TwoPi * MathF.Pow(2, i) * 0.5f + timeValue * MathF.Sin(index / 40f * MathHelper.TwoPi) * MathF.Pow(2, i)) * MathF.Pow(2, -i);
		}
		return value;
	}

	public bool CanTrap(NPC npc)
	{
		if (npc is null || !npc.active || npc.dontTakeDamage || npc.boss || npc.friendly || npc.knockBackResist == 0 || npc.type == NPCID.TargetDummy || npc.type == NPCID.WindyBalloon)
		{
			return false;
		}
		return true;
	}

	public float CalculateTargetScale()
	{
		float scale = Math.Max(Target.width, Target.height);
		return scale * 1.5f;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		GenerateBubbleBound();
		if (BubbleBound.Count <= 0)
		{
			return false;
		}
		List<Vertex2D> bars_dark = new List<Vertex2D>();
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> innerBound = new List<Vertex2D>();
		List<Vertex2D> highlightBound = new List<Vertex2D>();
		float width = 10f;

		float timeValue = (float)Main.time / 150f + Projectile.whoAmI / 7f;

		for (int k = 0; k < BubbleBound.Count; k++)
		{
			Vector2 bound = BubbleBound[k];
			Vector2 drawWorldPos = bound + Projectile.Center;
			Vector2 drawPos = drawWorldPos - Main.screenPosition;
			Vector2 radDir = Vector2.Normalize(bound) * width;
			Color drawColor = new Color(0.1f, 0.4f, 0.7f, 0);
			drawColor = Lighting.GetColor(drawWorldPos.ToTileCoordinates(), drawColor);
			drawColor.A = 0;

			bars_dark.Add(drawPos, Color.White * 0.5f, new Vector3(k / 30f, 0, 0));
			bars_dark.Add(drawPos - radDir, Color.White * 0.5f, new Vector3(k / 30f, 1, 0));

			bars.Add(drawPos, drawColor, new Vector3(k / 30f, 0, 0));
			bars.Add(drawPos - radDir, drawColor, new Vector3(k / 30f, 1, 0));

			Color drawRainbowColor = GetColorFromWavelength(580 + 200 * Math.Sin(k / 60f * MathHelper.TwoPi));
			drawRainbowColor = Color.Lerp(drawColor, drawRainbowColor, 0.9f);
			Color drawRainbowColor2 = GetColorFromWavelength(580 + 200 * Math.Sin((k + 30) / 60f * MathHelper.TwoPi));
			drawRainbowColor2 = Color.Lerp(drawColor, drawRainbowColor2, 0.1f);
			drawRainbowColor = Lighting.GetColor(drawWorldPos.ToTileCoordinates(), drawRainbowColor);
			drawRainbowColor2 = Lighting.GetColor(drawWorldPos.ToTileCoordinates(), drawRainbowColor2);
			drawRainbowColor.A = 0;
			drawRainbowColor2.A = 0;

			innerBound.Add(drawPos, drawRainbowColor * 0.9f, new Vector3(k / 60f + timeValue, 0, 0));
			innerBound.Add(drawPos - radDir * 2, drawRainbowColor2 * 0.1f, new Vector3(k / 60f + timeValue, 1, 0));

			Color highlightColor = Lighting.GetColor(drawWorldPos.ToTileCoordinates(), new Color(1f, 1f, 1f, 0)) * 2;
			highlightColor.A = 0;
			highlightBound.Add(drawPos, highlightColor, new Vector3(k / 40f - timeValue, 0.5f, 0));
			highlightBound.Add(drawPos - radDir * 2, highlightColor, new Vector3(k / 40f - timeValue, 0.7f, 0));
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Wave_full_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Wave_full.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_forceField_sparse.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, innerBound.ToArray(), 0, innerBound.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, highlightBound.ToArray(), 0, highlightBound.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		//Vector2 sunDir = GetSunPos() - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		//sunDir = sunDir.NormalizeSafe();
		Vector2 spotPos = Projectile.Center + new Vector2(-1, -1) * BubbleScale * 0.5f;
		Texture2D highlightSpot = Commons.ModAsset.LightPoint2.Value;
		Color highlightColor2 = Lighting.GetColor(spotPos.ToTileCoordinates(), new Color(1f, 1f, 1f, 0)) * 2;
		highlightColor2.A = 0;
		Main.EntitySpriteDraw(highlightSpot, spotPos - Main.screenPosition, null, highlightColor2, 0, highlightSpot.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}

	public Vector2 GetSunPos()
	{
		float HalfMaxTime = Main.dayTime ? 27000 : 16200;
		float bgTop = -Main.screenPosition.Y / (float)(Main.worldSurface * 16.0 - 600.0) * 200f;
		float value = 1 - (float)Main.time / HalfMaxTime;
		float StarX = (1 - value) * Main.screenWidth / 2f - 100 * value;
		float t = value * value;
		float StarY = bgTop + t * 250f + 180;
		if (Main.LocalPlayer != null)
		{
			if (Main.LocalPlayer.gravDir == -1)
			{
				return new Vector2(StarX, Main.screenHeight - StarY);
			}
		}

		return new Vector2(StarX, StarY);
	}

	public Color GetColorFromWavelength(double wavelength)
	{
		double r = 0, g = 0, b = 0;

		if (wavelength >= 380 && wavelength < 440)
		{
			r = -(wavelength - 440) / (440 - 380);
			g = 0;
			b = 1;
		}
		else if (wavelength >= 440 && wavelength < 490)
		{
			r = 0;
			g = (wavelength - 440) / (490 - 440);
			b = 1;
		}
		else if (wavelength >= 490 && wavelength < 510)
		{
			r = 0;
			g = 1;
			b = -(wavelength - 510) / (510 - 490);
		}
		else if (wavelength >= 510 && wavelength < 580)
		{
			r = (wavelength - 510) / (580 - 510);
			g = 1;
			b = 0;
		}
		else if (wavelength >= 580 && wavelength < 645)
		{
			r = 1;
			g = -(wavelength - 645) / (645 - 580);
			b = 0;
		}
		else if (wavelength >= 645 && wavelength <= 780)
		{
			r = 1;
			g = 0;
			b = 0;
		}
		else
		{
			r = 0;
			g = 0;
			b = 0;
		}
		return new Color((float)r, (float)g, (float)b, 0);
	}

	public override void OnKill(int timeLeft)
	{
		for (int k = 0; k < BubbleScale; k++)
		{
			Vector2 startVel = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1))).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * BubbleScale;
			var branch = new EmptyWaterStaff_BubbleBreak
			{
				velocity = startVel * 0.5f,
				Active = true,
				Visible = true,
				position = Projectile.Center + startVel,
				maxTime = Main.rand.Next(30, 40),
				scale = Main.rand.Next(10, 16),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
			};
			Ins.VFXManager.Add(branch);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		List<Vertex2D> bars_outer = new List<Vertex2D>();

		List<Vertex2D> bars_inner = new List<Vertex2D>();
		float width = BubbleScale * 0.25f;
		float timeValue = (float)Main.time / 450f;
		float warpValue = BubbleScale / 40f * 0.02f;
		for (int k = 0; k < BubbleBound.Count; k++)
		{
			Vector2 bound = BubbleBound[k] * 0.92f;
			Vector2 drawPos = bound + Projectile.Center - Main.screenPosition;
			Vector2 radDir = Vector2.Normalize(bound) * width;
			Vector2 warpDir = bound.RotatedBy(MathHelper.Pi);
			Color drawColor = new Color(0.5f, 0.5f, 0, 0);
			Color drawColorInner = new Color(0.5f + warpDir.X / BubbleScale, 0.5f + warpDir.Y / BubbleScale, warpValue, 0);
			bars_outer.Add(drawPos, drawColor, new Vector3(k / 40f + timeValue, 0, 0));
			bars_outer.Add(drawPos - radDir, drawColorInner, new Vector3(k / 40f + timeValue, 1, 0));

			bars_inner.Add(drawPos - radDir, drawColorInner, new Vector3(k / 40f + timeValue, 1, 0));
			bars_inner.Add(drawPos - radDir * 2, drawColor, new Vector3(k / 40f + timeValue, 0, 0));
		}
		spriteBatch.Draw(Commons.ModAsset.Noise_flame_2.Value, bars_outer, PrimitiveType.TriangleStrip);
		spriteBatch.Draw(Commons.ModAsset.Noise_flame_2.Value, bars_inner, PrimitiveType.TriangleStrip);
	}
}
