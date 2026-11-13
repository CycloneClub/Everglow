using Everglow.Myth.LanternMoon.Gores;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

using static Everglow.Myth.Common.MythUtils;

public class ExplosiveLantern_Growing : ModNPC
{
	public override void SetDefaults()
	{
		NPC.width = 120;
		NPC.height = 120;
		NPC.aiStyle = -1;
		NPC.friendly = false;
		NPC.timeLeft = 200;
		NPC.alpha = 0;
		NPC.scale = 0f;
		NPC.noGravity = true;
		NPC.dontCountMe = true;
	}

	public int timer = 0;
	public int MaxTimer = 0;

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0.5f);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		Explosion();
		base.OnHitPlayer(target, info);
	}

	public override void OnSpawn(IEntitySource source)
	{
		timer = 0;
		NPC.life = NPC.lifeMax = 1;
		NPC.ai[0] = Main.rand.NextFloat(0.06f, 0.08f) / 10f;
		MaxTimer = Main.rand.Next(175, 300);
	}

	public override void AI()
	{
		NPC.TargetClosest();
		timer++;
		if (timer < 150)
		{
			NPC.scale += NPC.ai[0] * 1.5f;
			if (timer < 50)
			{
				NPC.width += 1;
				NPC.height += 1;
			}
		}
		if (NPC.scale >= 0.8f && timer < MaxTimer - 60)
		{
			NPC.velocity.Y += 0.1f;
			if (NPC.velocity.Length() > NPC.scale * 5)
			{
				NPC.velocity *= 0.99f;
			}
		}
		else
		{
			NPC.velocity *= 0.95f;
		}
		if (NPC.collideX || NPC.collideY)
		{
			Explosion();
		}
		if (timer > MaxTimer)
		{
			Explosion();
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture2D = (Texture2D)ModContent.Request<Texture2D>(Texture);

		float timeValue = (float)Main.time * 0.03f;
		var colorT = new Color(1f + (float)(Math.Sin(timeValue) + 2) / 3f, 1f + (float)(Math.Sin(timeValue) + 2) / 3f, 1f + (float)(Math.Sin(timeValue) + 2) / 3f, 0.5f + (float)(Math.Sin(timeValue) + 2) / 6f);

		spriteBatch.Draw(texture2D, NPC.Center - Main.screenPosition, null, colorT, NPC.rotation, texture2D.Size() * 0.5f, NPC.scale, SpriteEffects.None, 1f);
		float value = (timer - MaxTimer + 60) / 60f;
		if (value > 0)
		{
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			Vector2 orig = star.Size() / 2f;
			float mulSize = MathF.Sin((float)Main.timeForVisualEffects * 0.4f + NPC.whoAmI) * 0.05f + 1f;
			Vector2 offset = Vector2.zeroVector;
			spriteBatch.Draw(star, NPC.Center - Main.screenPosition + offset, null, new Color(1f, 0.1f, 0.05f, 0), 0, orig, new Vector2(value, 1.5f * mulSize) * mulSize, SpriteEffects.None, 0f);
			spriteBatch.Draw(star, NPC.Center - Main.screenPosition + offset, null, new Color(1f, 0.1f, 0.05f, 0), MathHelper.PiOver4, orig, new Vector2(value, 1.0f * mulSize) * mulSize, SpriteEffects.None, 0f);
			spriteBatch.Draw(star, NPC.Center - Main.screenPosition + offset, null, new Color(1f, 0.1f, 0.05f, 0), MathHelper.PiOver4 * 3, orig, new Vector2(value, 1.0f * mulSize) * mulSize, SpriteEffects.None, 0f);
			spriteBatch.Draw(star, NPC.Center - Main.screenPosition + offset, null, new Color(1f, 0.1f, 0.05f, 0), MathHelper.PiOver2, orig, new Vector2(value, 2.0f * mulSize) * mulSize, SpriteEffects.None, 0f);
		}
		float maxDistance = 70;
		if (Main.expertMode)
		{
			maxDistance = 82;
		}
		if (Main.masterMode)
		{
			maxDistance = 150;
		}
		DrawTexCircle(maxDistance * MathF.Pow(value, 0.2f), 22 * value + 22, new Color(0.75f, 0.05f, 0.0f, 0), NPC.Center - Main.screenPosition, Commons.ModAsset.Trail_0.Value, Main.time / 17);
		return false;
	}

	public override void OnKill()
	{
		Explosion();
	}

	public void Explosion()
	{
		ShakerManager.AddShaker(NPC.Center, new Vector2(0, -1), 40, 30f, 160, 0.9f, 0.8f, 120);
		var p = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<DarkLanternBombExplosion>(), 40, 5, -1, 1);
		p.friendly = false;
		p.hostile = true;
		NPC.active = false;
	}
}
