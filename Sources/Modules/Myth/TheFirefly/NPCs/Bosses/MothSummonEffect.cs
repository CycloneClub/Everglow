using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.NPCs.Bosses;

public class MothSummonEffect : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => Commons.ModAsset.LightPoint_Mod;

	public NPC Moth;

	public override void SetCustomDefaults()
	{
		Projectile.timeLeft = 600;
		Projectile.tileCollide = false;
		TrailLength = 40;
		TrailTexture = Commons.ModAsset.Trail_10.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
	}

	public override void OnSpawn(IEntitySource source)
	{
		bool hasMoth = false;
		foreach (var npc in Main.npc)
		{
			if (npc.active && npc.type == ModContent.NPCType<CorruptMoth>())
			{
				if ((npc.Center - Projectile.Center).Length() < 2500)
				{
					hasMoth = true;
					Moth = npc;
					break;
				}
			}
		}
		if (!hasMoth)
		{
			Projectile.active = false;
		}
		Projectile.timeLeft = Main.rand.Next(510, 560);
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		if (!Moth.active || Moth.type != ModContent.NPCType<CorruptMoth>())
		{
			Projectile.active = false;
		}
		if (Projectile.timeLeft < 500)
		{
			Vector2 toTarget = Moth.Center - Projectile.Center - Projectile.velocity;
			if (toTarget.Length() < 35f)
			{
				DestroyEntity();
				CorruptMoth cMoth = Moth.ModNPC as CorruptMoth;
				if (cMoth != null)
				{
					cMoth.Timer += 10;
				}
			}
			Vector2 targetVel = Vector2.Normalize(toTarget) * 15f;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVel, 0.05f);
		}
		else
		{
			Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]);
			if (Projectile.velocity.Length() > 3f)
			{
				Projectile.velocity *= 0.9f;
			}
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			float value = index / (float)SmoothedOldPos.Count;
			Color color = Color.Lerp(new Color(0, 126, 255, 0), new Color(135, 0, 155, 0), value);
			return color;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, TrailColor, Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
	}
}
