using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.NPCs;

public class CylindricalLantern : LanternMoonNPC
{
	public float FadeTimer = 0f;

	public float TeleportTimer = 0f;

	public float TeleportMax = 600;

	public override void SetDefaults()
	{
		NPC.damage = 72;
		NPC.lifeMax = 364;
		NPC.npcSlots = 2.5f;
		NPC.width = 40;
		NPC.height = 60;
		NPC.defense = 8;
		NPC.value = 0;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.7f;
		NPC.dontTakeDamage = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.NPCHit3;
		LanternMoonScore = 8f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		TeleportTimer = 0;
		FadeTimer = 0;
		TeleportMax = Main.rand.NextFloat(360, 1000);
	}

	public override void AI()
	{
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			return;
		}
		TeleportTimer++;
		NPC.TargetClosest(false);
		Player player = Main.player[NPC.target];
		if (ExplosionTimer > 0)
		{
			UpdateExplosion();
			return;
		}
		NPC.velocity *= 0;
		if (TeleportTimer == 30)
		{
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<CylindricalLantern_Flame>(), 9, 1f, -1);
		}
		if (TeleportTimer > 100)
		{
			float timeValue = TeleportTimer + NPC.whoAmI;
			timeValue *= 0.03f;
			NPC.velocity = new Vector2(MathF.Sin(timeValue), MathF.Cos(timeValue * 2));
		}
		float distance = (player.Center - NPC.Center).Length();
		if (TeleportTimer > TeleportMax || (distance < 220 && TeleportTimer > 120) || (distance > 800))
		{
			if (FadeTimer == 0)
			{
				TeleportTimer = 0;
				FadeTimer = 60;
				NPC.velocity *= 0;
				TeleportMax = Main.rand.NextFloat(360, 1000);
			}
		}

		// Fade and teleport
		if (FadeTimer > 0)
		{
			FadeTimer--;
		}
		else
		{
			NPC.alpha = 0;
		}
		if (FadeTimer > 30)
		{
			NPC.alpha += 10;
		}
		if (FadeTimer == 30)
		{
			NPC.alpha = 255;
			NPC.Center = player.Center + new Vector2(0, -Main.rand.NextFloat(200, 500)).RotatedBy(Main.rand.NextFloat(-1.6f, 1.6f));
		}
		if (FadeTimer < 30)
		{
			NPC.alpha -= 10;
		}
	}

	public void UpdateExplosion()
	{
		ExplosionTimer--;
		NPC.scale += 0.01f;
		NPC.velocity *= 0f;
		if (ExplosionTimer <= 0)
		{
			KillMe();
		}
	}

	public int ExplosionTimer = -1;

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			ExplosionTimer = 30;
			NPC.life = 1;
			NPC.dontTakeDamage = true;
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<CylindricalLantern_Explosion>(), 23, 1f, -1);
		}
	}

	public void KillMe()
	{
		for (int f = 0; f < 10; f++)
		{
			Vector2 v3 = new Vector2(0, Main.rand.NextFloat(0, 12f)).RotatedByRandom(MathHelper.TwoPi);
			int r = Dust.NewDust(NPC.Center - new Vector2(4, 4) - new Vector2(4, 4), 8, 8, ModContent.DustType<Dusts.Flame4>(), v3.X, v3.Y, 0, default, Main.rand.NextFloat(0.6f, 1.8f));
			Main.dust[r].noGravity = true;
			Main.dust[r].velocity = v3;
		}
		for (int g = 0; g < 8; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 8f, 0).RotatedByRandom(MathHelper.TwoPi);
			string texturePath = ModAsset.CylindricalLantern_Gore_0_Mod;
			if (texturePath is not null)
			{
				texturePath = texturePath.Remove(texturePath.Length - 1, 1);
				texturePath += g;
			}
			var gore = new NormalGore
			{
				Velocity = vel,
				Position = NPC.Center + vel,
				Texture = ModContent.Request<Texture2D>(texturePath).Value,
				RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Main.rand.NextFloat(0.8f, 1.2f),
				MaxTime = Main.rand.Next(300, 340),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(gore);
		}
		NPC.active = false;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		float mulColor = (255 - NPC.alpha) / 255f;
		SpriteEffects effects = SpriteEffects.None;
		NPC.spriteDirection = NPC.direction;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor * mulColor, NPC.rotation, texture.Size() * 0.5f, NPC.scale, effects, 0);
		Texture2D flame = ModAsset.CylindricalLantern_Flame.Value;
		Rectangle flameFrame = new Rectangle(0, NPC.frame.Y, 14, 28);
		spriteBatch.Draw(flame, NPC.Center - Main.screenPosition, flameFrame, new Color(0.7f, 0.6f, 0.6f, 0) * mulColor, NPC.rotation, flameFrame.Size() * 0.5f, NPC.scale, effects, 0);
		Lighting.AddLight(NPC.Center, new Vector3(1f, 0.6f, 0.4f) * mulColor);
		return false;
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter++;
		if (NPC.frameCounter > 4)
		{
			NPC.frameCounter = 0;
			NPC.frame.Y += 28;
			if (NPC.frame.Y >= 84)
			{
				NPC.frame.Y = 0;
			}
		}
	}
}
