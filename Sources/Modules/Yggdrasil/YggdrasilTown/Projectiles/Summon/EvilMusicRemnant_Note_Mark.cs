using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class EvilMusicRemnant_Note_Mark : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public int MaxTime = 600;

	public float mouseDistanceValue = 0;

	public override void SetDefaults()
	{
		Projectile.width = 26;
		Projectile.height = 26;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = MaxTime;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.scale = 0;
		mouseDistanceValue = (Main.MouseWorld - Main.player[Projectile.owner].Center).Length() / 6f + 30;
	}

	private string ProjectileTexture { get; set; } = ModAsset.EvilMusicRemnant_Projectile1_Mod;

	public override string Texture => ProjectileTexture;

	public override void AI()
	{
		if (Projectile.timeLeft > MaxTime - 32f)
		{
			float value = (MaxTime - Projectile.timeLeft) / 30f;
			Projectile.scale = MathF.Pow(value, 0.3f);
		}
		else
		{
			Projectile.scale = 1;
		}
		Projectile.velocity *= 0;

		var owner = Main.player[Projectile.owner];

		if (owner.ownedProjectileCounts[Type] > 3)
		{
			int minProj = MaxTime;
			int killWhoAmI = -1;
			int canKillCount = 0;
			foreach (Projectile proj in Main.projectile)
			{
				if (proj != null && proj.active && proj.type == Type)
				{
					if (proj.timeLeft > 60f)
					{
						canKillCount++;
						if (proj.timeLeft <= minProj)
						{
							minProj = proj.timeLeft;
							killWhoAmI = proj.whoAmI;
						}
					}
				}
			}
			if (killWhoAmI != -1 && canKillCount >= 4)
			{
				var killProj = Main.projectile[killWhoAmI];
				if (killProj != null && killProj.active && killProj.type == Type && killProj.timeLeft > 60)
				{
					killProj.timeLeft = 60;
				}
			}
		}
		float size = Main.rand.NextFloat(0.1f, 0.96f);
		if (Main.rand.NextBool(3))
		{
			var noteFlame = new EvilMusicRemnant_FlameDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(0.3f, 1f)).RotatedByRandom(MathHelper.TwoPi) * 0.8f,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-100, 100), Main.rand.NextFloat(-30, 30)),
				MaxTime = Main.rand.Next(54, 86),
				Scale = 14f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = [Main.rand.NextFloat(-0.8f, 0.8f)],
			};
			Ins.VFXManager.Add(noteFlame);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texture = Commons.ModAsset.StarSlash.Value;
		var textureBlack = Commons.ModAsset.StarSlash_black.Value;
		float timeValue = (float)Main.time * 0.02f;
		var drawPos = Projectile.Center - Main.screenPosition;
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
		}
		float staveFade = 1f;
		if (Projectile.timeLeft > MaxTime - mouseDistanceValue)
		{
			if (Projectile.timeLeft > MaxTime - mouseDistanceValue + 60)
			{
				staveFade = 0;
			}
			else
			{
				staveFade = (MaxTime - Projectile.timeLeft - mouseDistanceValue + 60) / 60f;
			}
		}
		float staveSize = MathF.Pow(3 - staveFade * 2, 0.5f);
		var bars_front_dark = new List<Vertex2D>();
		var bars_back_dark = new List<Vertex2D>();

		var bars_front = new List<Vertex2D>();
		var bars_back = new List<Vertex2D>();
		for (int stave = 0; stave <= 3; stave++)
		{
			for (int i = 0; i <= 100; i++)
			{
				var sinStave = (MathF.Sin(timeValue + stave / 3f * MathHelper.TwoPi) + 0.5f) * 0.5f;
				var ringWidth = 60f * Projectile.scale * staveSize;
				var phase = i / 100f * MathHelper.TwoPi;
				var x0 = MathF.Cos(timeValue * 2f + phase + Projectile.whoAmI + stave / 3f * MathHelper.TwoPi) * ringWidth;
				var y0 = MathF.Sin(timeValue * 2f + phase * 3 + Projectile.whoAmI + stave / 3f * MathHelper.TwoPi) * 10 * MathF.Sin(timeValue * 2f + stave / 3f + Projectile.whoAmI) * Projectile.scale * staveSize;
				var z0 = MathF.Sin(timeValue * 2f + phase + Projectile.whoAmI + stave / 3f * MathHelper.TwoPi) * ringWidth;
				var oldPhase = (i - 1) / 100f * MathHelper.TwoPi;
				var oldZ0 = MathF.Sin(timeValue * 2f + oldPhase + Projectile.whoAmI + stave / 3f * MathHelper.TwoPi) * ringWidth;
				var nextPhase = (i + 1) / 100f * MathHelper.TwoPi;
				var nextZ0 = MathF.Sin(timeValue * 2f + nextPhase + Projectile.whoAmI + stave / 3f * MathHelper.TwoPi) * ringWidth;
				var staveCurve = new Vector2(x0, y0);
				float scale = (ringWidth - z0 + 4 * ringWidth) / (5 * ringWidth);
				float z0Value = 1 - (z0 + ringWidth) / (2 * ringWidth);

				var drawColor = Color.Lerp(new Color(128, 45, 229, 0), new Color(24, 2, 150, 0), z0Value) * fade * staveFade * sinStave;
				var darkColor = Color.Lerp(new Color(255, 255, 255, 255), new Color(127, 127, 127, 127), z0Value) * fade * staveFade * sinStave;
				var height = 24f * Projectile.scale;
				if (i == 0)
				{
					bars_back.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_back.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

					bars_back_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_back_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

					bars_front.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_front.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

					bars_front_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_front_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
				}
				if (z0 > 0)
				{
					bars_front.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, drawColor, new Vector3(phase, 0, 0));
					bars_front.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, drawColor, new Vector3(phase, 1, 0));

					bars_front_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, darkColor, new Vector3(phase, 0, 0));
					bars_front_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, darkColor, new Vector3(phase, 1, 0));
					if (oldZ0 <= 0)
					{
						bars_back.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_back.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

						bars_back_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_back_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
					}
					if (nextZ0 <= 0)
					{
						bars_back.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_back.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
						bars_back.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, drawColor, new Vector3(phase, 0, 0));
						bars_back.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, drawColor, new Vector3(phase, 1, 0));

						bars_back_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_back_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
						bars_back_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, darkColor, new Vector3(phase, 0, 0));
						bars_back_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, darkColor, new Vector3(phase, 1, 0));
					}
				}
				else
				{
					bars_back.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, drawColor, new Vector3(phase, 0, 0));
					bars_back.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, drawColor, new Vector3(phase, 1, 0));

					bars_back_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, darkColor, new Vector3(phase, 0, 0));
					bars_back_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, darkColor, new Vector3(phase, 1, 0));
					if (oldZ0 > 0)
					{
						bars_front.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_front.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

						bars_front_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_front_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
					}
					if (nextZ0 > 0)
					{
						bars_front.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_front.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
						bars_front.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, drawColor, new Vector3(phase, 0, 0));
						bars_front.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, drawColor, new Vector3(phase, 1, 0));

						bars_front_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
						bars_front_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
						bars_front_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, darkColor, new Vector3(phase, 0, 0));
						bars_front_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, darkColor, new Vector3(phase, 1, 0));
					}
				}
				if (i == 100)
				{
					bars_back.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_back.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

					bars_back_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_back_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

					bars_front.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_front.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));

					bars_front_dark.Add(drawPos + staveCurve + new Vector2(0, -height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 0, 0));
					bars_front_dark.Add(drawPos + staveCurve * 0.8f + new Vector2(0, height) / scale, new Color(0, 0, 0, 0), new Vector3(phase, 1, 0));
				}
			}
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.EvilMusicRemnant_Stave_black.Value;
		if (bars_back_dark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_back_dark.ToArray(), 0, bars_back_dark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.EvilMusicRemnant_Stave.Value;
		if (bars_back.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_back.ToArray(), 0, bars_back.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(50, 232, 123, 0), 0, texture.Size() / 2, new Vector2(1f * Projectile.scale * fade, 0.5f * Projectile.scale), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(textureBlack, Projectile.Center - Main.screenPosition, null, Color.White, 0, textureBlack.Size() / 2, new Vector2(1f * Projectile.scale * fade, 0.5f * Projectile.scale), SpriteEffects.None, 0);
		for (int i = 0; i < 3; i++)
		{
			Main.spriteBatch.Draw(textureBlack, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(timeValue * 3 + i / 3f * MathHelper.TwoPi) * 16, 0), null, new Color(50, 16, 220, 100) * 0.6f * fade, 0, texture.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(textureBlack, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(timeValue * 3 + i / 3f * MathHelper.TwoPi) * 16, 0), null, new Color(50, 16, 220, 100) * 0.6f * fade, -0.7f, texture.Size() / 2, 0.3f * Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(textureBlack, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(timeValue * 3 + i / 3f * MathHelper.TwoPi) * 16, 0), null, new Color(50, 16, 220, 100) * 0.6f * fade, 0.7f, texture.Size() / 2, 0.3f * Projectile.scale, SpriteEffects.None, 0);

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(timeValue * 3 + i / 3f * MathHelper.TwoPi) * 16, 0), null, new Color(50, 16, 220, 0) * 0.6f * fade, 0, texture.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(timeValue * 3 + i / 3f * MathHelper.TwoPi) * 16, 0), null, new Color(50, 16, 220, 0) * 0.6f * fade, -0.7f, texture.Size() / 2, 0.3f * Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(MathF.Sin(timeValue * 3 + i / 3f * MathHelper.TwoPi) * 16, 0), null, new Color(50, 16, 220, 0) * 0.6f * fade, 0.7f, texture.Size() / 2, 0.3f * Projectile.scale, SpriteEffects.None, 0);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.EvilMusicRemnant_Stave_black.Value;
		if (bars_front_dark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_front_dark.ToArray(), 0, bars_front_dark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.EvilMusicRemnant_Stave.Value;
		if (bars_front.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_front.ToArray(), 0, bars_front.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public void SummonMinion()
	{
	}
}
