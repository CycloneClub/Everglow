using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.NPCs;

public class GreenFlameLantern : LanternMoonNPC
{
	public Vector2 StayPosition = Vector2.zeroVector;

	public int Timer;

	public int MoveTime = 300;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 5;
	}

	public override void SetDefaults()
	{
		NPC.damage = 56;
		NPC.lifeMax = 1400;
		NPC.npcSlots = 2.5f;
		NPC.width = 60;
		NPC.height = 60;
		NPC.defense = 55;
		NPC.value = 200;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.2f;
		NPC.dontTakeDamage = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.NPCHit3;
		LanternMoonScore = 40;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter++;
		frameHeight = 130;
		if (NPC.frameCounter >= 6)
		{
			NPC.frameCounter = 0;
			NPC.frame.Y += frameHeight;
			if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
			{
				NPC.frame.Y = 0;
			}
		}
	}

	public override void AI()
	{
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			return;
		}
		Timer++;
		NPC.TargetClosest(false);
		Player player = Main.player[NPC.target];
		if (StayPosition == Vector2.zeroVector || Timer == 1)
		{
			Vector2 toPlayer = player.Center - NPC.Center;
			toPlayer = -toPlayer.NormalizeSafe() * Main.rand.NextFloat(180, 420);
			toPlayer = toPlayer.RotatedByRandom(MathHelper.PiOver4);
			if (toPlayer.Y > 0)
			{
				toPlayer.Y *= -1;
			}
			StayPosition = player.Center + toPlayer;
		}
		if (Timer > 1 && Timer < 60)
		{
			NPC.Center = Vector2.Lerp(NPC.Center, StayPosition, 0.02f);
		}
		if (Timer >= 60 && Timer % 150 == 0)
		{
			Vector2 v0 = new Vector2(0, 3).RotatedBy(Timer + NPC.whoAmI);
			for (int i = 0; i < 4; i++)
			{
				Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Top, v0.RotatedBy(i / 4f * MathHelper.TwoPi), ModContent.ProjectileType<GreenFlameProj>(), 20, 0f, Main.maxPlayers);
				GreenFlameProj gFP = p0.ModProjectile as GreenFlameProj;
				if (gFP is not null)
				{
					gFP.OwnerNPC = NPC;
				}
			}
		}
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc != NPC)
			{
				if (npc.type == ModContent.NPCType<NPCs.LanternGhostKing.LanternGhostKing>())
				{
					Vector2 v0 = NPC.Center - npc.Center;
					if (v0.Length() < 400)
					{
						NPC.velocity += Vector2.Normalize(v0) * 2.5f;
						break;
					}
				}
			}
		}
		NPC.velocity *= 0.95f;
		if (Timer > MoveTime)
		{
			MoveTime = Main.rand.Next(900, 1500);
			Timer = 0;
		}
		Lighting.AddLight(NPC.Center + new Vector2(0, 24), new Vector3(1f, 0.2f, 0f));
		Lighting.AddLight(NPC.Center + new Vector2(0, -30), new Vector3(0f, 0.7f, 1f));
	}

	public override void HitEffect(NPC.HitInfo hit) => base.HitEffect(hit);

	public override void OnKill()
	{
		KillEffect(NPC.Center);
	}

	public void KillEffect(Vector2 pos)
	{
		for (int g = 0; g < 12; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 8f, 0).RotatedByRandom(MathHelper.TwoPi);
			string texturePath = ModAsset.GreenFlameLantern_Gore_0_Mod;
			if (texturePath is not null)
			{
				texturePath = texturePath.Remove(texturePath.Length - 1, 1);
				texturePath += g;
			}
			var gore = new NormalGore
			{
				Velocity = vel,
				Position = pos + vel * 6,
				Texture = ModContent.Request<Texture2D>(texturePath).Value,
				RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Main.rand.NextFloat(0.8f, 1.2f),
				MaxTime = Main.rand.Next(300, 340),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(gore);
		}

		for (int g = 0; g < 20; g++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 15).RotatedByRandom(MathHelper.TwoPi);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2.4f;
			offsetPos *= 6;
			var sparkFlame = new GreenLanternFragment
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + offsetPos,
				RotateSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Rotate2Speed = Main.rand.NextFloat(-0.5f, 0.5f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 0.2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Rotation2 = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(105, 150),
				Scale = Main.rand.NextFloat(0.6f, 1f),
				Frame = Main.rand.Next(0, 4),
				Gravity = true,
			};
			Ins.VFXManager.Add(sparkFlame);
		}
		for (int x = 0; x < 20; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 25).RotatedByRandom(MathHelper.TwoPi);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new GreenLanternRedStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(80, 160),
				Scale = Main.rand.NextFloat(0.5f, 1f),
				Gravity = true,
			};
			Ins.VFXManager.Add(spark);
		}
		for (int x = 0; x < 15; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 12).RotatedByRandom(MathHelper.TwoPi);
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new GreenLanternCyanStar
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(0, -30) + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(50, 100),
				Scale = Main.rand.NextFloat(0.5f, 1f),
			};
			Ins.VFXManager.Add(spark);
		}
		for (int u = 0; u < 15; u++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 16f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new LanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(Main.rand.NextFloat(30), 0).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 45),
				Scale = Main.rand.NextFloat(50f, 120f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int u = 0; u < 8; u++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 8f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new GreenLanternFlame
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos + new Vector2(0, -30) + new Vector2(Main.rand.NextFloat(30), 0).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 45),
				Scale = Main.rand.NextFloat(50f, 70f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int i = 0; i < 8; i++)
		{
			Vector2 vel = new Vector2(0, -5).RotatedBy(i / 8f * MathHelper.TwoPi);
			if (i % 2 == 1)
			{
				vel *= 0.65f;
			}
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), pos, vel, ModContent.ProjectileType<GreenFlameSharpCrystal>(), 20, 1, Main.maxPlayers);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var glow = ModAsset.GreenFlameLantern_glow.Value;
		SpriteEffects effects = SpriteEffects.None;
		NPC.spriteDirection = NPC.direction;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, effects, 0);
		spriteBatch.Draw(glow, NPC.Center - Main.screenPosition, NPC.frame, new Color(1f, 1f, 1f, 0) * (0.75f + 0.25f * MathF.Sin(Timer * 0.1f)), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, effects, 0);

		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float star_scale = 0f;
		if (Timer % 150 > 120)
		{
			star_scale = (Timer % 150 - 120) / 30f;
		}
		float timeValue = MathF.Sin((float)Main.time * 0.07f + NPC.whoAmI) * 0.5f + 0.5f;
		Color c0 = new Color(0f, 0.75f * timeValue, 0.75f, 0);
		var drawPos = NPC.Center - Main.screenPosition + new Vector2(0, -30);
		Main.spriteBatch.Draw(star, drawPos, null, c0, MathHelper.PiOver2, star.Size() / 2f, new Vector2(star_scale / 1.5f, 1f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, drawPos, null, c0, 0, star.Size() / 2f, new Vector2(star_scale / 1.5f, 0.5f), SpriteEffects.None, 0);

		Main.spriteBatch.Draw(star, drawPos, null, c0, MathHelper.PiOver4, star.Size() / 2f, star_scale / 5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, drawPos, null, c0, -MathHelper.PiOver4, star.Size() / 2f, star_scale / 5f, SpriteEffects.None, 0);

		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(spot, drawPos, null, new Color(1f, 1f, 0.7f, 0), 0, spot.Size() / 2f, star_scale, SpriteEffects.None, 0);
		return false;
	}
}
