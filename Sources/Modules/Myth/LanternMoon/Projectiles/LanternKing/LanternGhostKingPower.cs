using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.LanternCommon;
using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class LanternGhostKingPower : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public bool ReachedTarget = false;

	public NPC OwnerNPC;

	public override void SetCustomDefaults()
	{
		TrailWidth = 100;
		TrailLength = 110;
		TrailTexture = Commons.ModAsset.Trail_13.Value;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = false;
	}

	public override void Behaviors()
	{
		if (OwnerNPC == null)
		{
			foreach (var npc in Main.npc)
			{
				if (npc is not null && npc.active && npc.type == ModContent.NPCType<LanternGhostKing>())
				{
					OwnerNPC = npc;
				}
			}
		}
		if (OwnerNPC == null || !OwnerNPC.active || OwnerNPC.type != ModContent.NPCType<LanternGhostKing>())
		{
			DestroyEntity();
			return;
		}
		Vector2 targetNPCPos = OwnerNPC.Center;
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		float musicTimer = musicSystem.Wave15StartTimer / (14 * 60f);
		if (musicTimer >= 1)
		{
			DestroyEntity();
		}
		if (musicTimer < 0.85f)
		{
			float moveTimer = (float)Main.time * 0.06f + MathHelper.PiOver2 * Projectile.ai[0];
			Vector2 targetPos = targetNPCPos + new Vector2(MathF.Cos(moveTimer) * 480, MathF.Sin(moveTimer * 2.4f) * 320) * (1.3f - musicTimer) + new Vector2(0, 150) * musicTimer;
			Vector2 toTarget = targetPos - Projectile.Center;
			toTarget = Vector2.Normalize(toTarget) * 20f;
			Projectile.velocity = toTarget * 0.05f + Projectile.velocity * 0.95f;
		}
		else
		{
			if (!ReachedTarget)
			{
				Vector2 targetPos = targetNPCPos + new Vector2(0, 150) * musicTimer;
				Vector2 toTarget = targetPos - Projectile.Center - Projectile.velocity;
				if (toTarget.Length() < 1)
				{
					Projectile.velocity *= 0;
					ReachedTarget = true;
				}
				else
				{
					toTarget = Vector2.Normalize(toTarget) * 20f;
					Projectile.velocity = Projectile.velocity.RotatedBy(0.1f);
					float value = musicTimer - 0.85f;
					Projectile.velocity = toTarget * value + Projectile.velocity * (1 - value);
				}
			}
		}
	}

	public override void DestroyEntityEffect() => base.DestroyEntityEffect();

	public override void DrawSelf()
	{
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 0)
		{
			return new Color(0, 0, 0, 0);
		}
		if (style == 1)
		{
			LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
			float mulColor = 0.1f;
			if (musicSystem.Wave15StartTimer > 300)
			{
				mulColor = 0.1f + (musicSystem.Wave15StartTimer - 300) / 540f;
				mulColor = Math.Min(mulColor, 1);
			}
			GradientColor colorG = new GradientColor();
			colorG.colorList.Add((new Color(1f, 1f, 0.9f, 0), 0f));
			colorG.colorList.Add((new Color(0.8f, 0.2f, 0.15f, 0), 0.07f));
			colorG.colorList.Add((new Color(0.5f, 0f, 0f, 0), 0.4f));
			colorG.colorList.Add((new Color(0.2f, 0f, 0.1f, 0), 0.7f));
			colorG.colorList.Add((new Color(0f, 0f, 0f, 0f), 1f));
			return colorG.GetColor(factor) * mulColor;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override float TrailWidthFunction(float factor)
	{
		if (factor < 0)
		{
			return 0;
		}
		factor = MathF.Pow(factor, 0.5f);
		return MathF.Sin(factor * MathHelper.PiOver2);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor + timeValue * 0;
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
}
