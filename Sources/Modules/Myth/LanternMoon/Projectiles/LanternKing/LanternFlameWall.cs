using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

/// <summary>
/// Relative speed > <see cref="SpeedThrethod"/> to prevent hurting by this projectile.
/// </summary>
public class LanternFlameWall : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => ModAsset.GoldLaser_Mod;

	public Vector2 StartPos = Vector2.zeroVector;

	public Vector2 GapStart = Vector2.zeroVector;

	public Vector2 GapEnd = Vector2.zeroVector;

	public Vector2 EndPos = Vector2.zeroVector;

	public Vector2 GapPosAndRange = Vector2.zeroVector;

	public float SpeedThrethod = 20f;

	public float Decay = 1f;

	public int Timer;

	public override void OnSpawn(IEntitySource source)
	{
		GapPosAndRange = new Vector2(Main.rand.NextFloat(-10, 10), 15);
	}

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10240;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		Projectile.timeLeft = 1500;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.scale = 1;
		Decay = 1f;
	}

	public override void AI()
	{
		// Main.NewText(Power());
		Timer++;
		var ringCenter = Projectile.Center;
		var ringRadius = 20000f;
		bool hasLanterKing = false;
		foreach (var npc in Main.npc)
		{
			if (npc != null && npc.active && npc.type == ModContent.NPCType<LanternGhostKing>())
			{
				LanternGhostKing lKing = npc.ModNPC as LanternGhostKing;
				ringCenter = lKing.RingCenter;
				ringRadius = lKing.RingRadius;
				hasLanterKing = true;
				break;
			}
		}
		if (!hasLanterKing)
		{
			Decay -= 0.01f;
		}
		if (Decay <= 0)
		{
			Projectile.Kill();
			return;
		}
		Vector2 projToCenter = ringCenter - Projectile.Center;
		if (projToCenter.Length() > ringRadius)
		{
			Projectile.Kill();
			return;
		}
		Vector2 checkUnit = Projectile.velocity.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * 16f;
		Vector2 checkStart = Projectile.Center;
		for (int k = 0; k < 1000; k++)
		{
			checkStart += checkUnit;
			Vector2 toCenter = ringCenter - checkStart;
			if (toCenter.Length() >= ringRadius)
			{
				StartPos = checkStart;
				break;
			}
		}
		Vector2 checkEnd = Projectile.Center;
		for (int k = 0; k < 1000; k++)
		{
			checkEnd -= checkUnit;
			Vector2 toCenter = ringCenter - checkEnd;
			if (toCenter.Length() >= ringRadius)
			{
				EndPos = checkEnd;
				break;
			}
		}
		Projectile.Center = (checkEnd + checkStart) * 0.5f;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (GapStart == Vector2.zeroVector || GapEnd == Vector2.zeroVector)
		{
			return false;
		}
		return CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, StartPos, GapEnd, 30) || CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, EndPos, GapStart, 30);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float distance = (StartPos - EndPos).Length() / 16f;
		distance = (int)distance;
		if (distance == 0)
		{
			return false;
		}
		DrawStar(StartPos);
		DrawStar(EndPos);
		Vector2 dir = Projectile.velocity.NormalizeSafe();
		float timeValue = (float)-Main.timeForVisualEffects * 0.001f;
		Vector2 checkGapStart = Vector2.zeroVector;
		Vector2 checkGapEnd = Vector2.zeroVector;
		List<Vertex2D> bars1 = new List<Vertex2D>();
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int h = 0; h <= distance; h++)
		{
			float value = h / distance;
			float coordX = (h - distance * 0.5f) * 0.006f;
			Vector2 pos = StartPos * value + EndPos * (1 - value);
			float mulColor = 1f;
			if (h == 0 || h == distance)
			{
				mulColor = 0;
			}
			var phase = h - distance * 0.5f;
			var toGap = MathF.Abs(phase - GapPosAndRange.X);
			if (toGap < GapPosAndRange.Y * 1.25f)
			{
				if (checkGapStart == Vector2.zeroVector)
				{
					checkGapStart = pos;
					GapStart = checkGapStart;
				}
				var phaseNext = h + 1 - distance * 0.5f;
				var toGapNext = MathF.Abs(phaseNext - GapPosAndRange.X);
				if (toGapNext >= GapPosAndRange.Y * 1.25f)
				{
					if (checkGapEnd == Vector2.zeroVector)
					{
						checkGapEnd = pos;
						GapEnd = checkGapEnd;
					}
				}
				mulColor *= MathF.Max((toGap - GapPosAndRange.Y * 1f) / (GapPosAndRange.Y * 0.25f), 0.3f);
			}
			if (toGap < GapPosAndRange.Y)
			{
				mulColor *= 0.3f;
			}
			mulColor *= Decay;
			float flameLength = 256f;
			if (h < 10)
			{
				flameLength *= h / 10f;
			}
			float toEnd = distance - h;
			if (toEnd < 10)
			{
				flameLength *= toEnd / 10f;
			}
			var drawColor = new Color(1f, 1f, 1f, 0);
			drawColor = Color.Lerp(drawColor, new Color(1f, 0, 0, 0), 1 - mulColor);
			bars.Add(pos, drawColor * mulColor, new Vector3(coordX, 0.5f, timeValue));
			bars.Add(pos - dir * flameLength, drawColor * 0, new Vector3(coordX, 1f, timeValue + 0.05f));
			bars1.Add(pos, drawColor * mulColor, new Vector3(coordX, 0.5f, timeValue));
			bars1.Add(pos + dir * 32, drawColor * 0, new Vector3(coordX, 1f, timeValue - 0.01f));
			Lighting.AddLight(pos, new Vector3(1f, 0.7f, 0.2f));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var effect = ModAsset.LanternFlameWall.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_rgb.Value);
		effect.Parameters["uDuration"].SetValue(0);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.Textures[1] = ModAsset.HeatMap_flameRing_lantern.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
		Main.graphics.GraphicsDevice.Textures[2] = Commons.ModAsset.Noise_cell.Value;
		Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		if (bars1.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars1.ToArray(), 0, bars1.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawStar(Vector2 pos)
	{
		Texture2D star = Commons.ModAsset.StarSlashGray.Value;
		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Color drawColor = new Color(1f, 0.8f + 0.2f * MathF.Sin((float)Main.time * 0.03f + Projectile.whoAmI), 0, 0);
		var drawPos = pos - Main.screenPosition;
		Main.EntitySpriteDraw(star, drawPos, null, drawColor, 0, star.Size() * 0.5f, 2f * Decay, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, drawPos, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, 2f * Decay, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(spot, drawPos, null, new Color(1f, 0.8f, 0.7f, 0), 0, spot.Size() * 0.5f, 2f * Decay, SpriteEffects.None, 0);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float distance = (StartPos - EndPos).Length() / 16f;
		distance = (int)distance;
		if (distance == 0)
		{
			return;
		}
		Vector2 dir = Projectile.velocity.NormalizeSafe();
		float timeValue = (float)Main.timeForVisualEffects * 0.001f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int h = 0; h <= distance; h++)
		{
			float value = h / distance;
			float coordX = (h - distance * 0.5f) * 0.006f;
			Vector2 pos = StartPos * value + EndPos * (1 - value) - Main.screenPosition;
			bars.Add(pos, new Color(1f, 1f, 0.9f, 0f), new Vector3(coordX, timeValue, 1));
			bars.Add(pos - dir * 256, new Color(1f, 1f, 0.9f, 0f), new Vector3(coordX, timeValue + 0.12f, 1));
		}

		// spriteBatch.Draw(Commons.ModAsset.Noise_rgb_large.Value, bars, PrimitiveType.TriangleStrip);
	}
}
