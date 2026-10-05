using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class PurpleBombCaterpillar : ModNPC
{
	public float MoveTimer = 0;

	public float MoveScale = 1f;

	public float GlowValue = 1f;

	public bool Angry = false;

	public bool Sleep = false;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 10;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		LampWoodForest lampBiome = ModContent.GetInstance<LampWoodForest>();
		return !lampBiome.IsBiomeActive(Main.LocalPlayer) ? 0f : 3f;
	}

	public override void SetDefaults()
	{
		NPC.width = 88;
		NPC.height = 36;
		NPC.lifeMax = 80;
		NPC.damage = 25;
		NPC.defense = 10;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		MoveTimer = 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.scale = Main.rand.NextFloat(0.85f, 1.15f);
		NPC.direction = Main.rand.NextBool() ? 1 : -1;
	}

	public override void FindFrame(int frameHeight)
	{
		if (Sleep)
		{
			if (NPC.frame.Y < frameHeight * 9)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;

					NPC.frame.Y += frameHeight;
				}
			}
		}
		else
		{
			if (NPC.frame.Y > 0)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter >= 10)
				{
					NPC.frameCounter = 0;

					NPC.frame.Y -= frameHeight;
				}
			}
		}
	}

	public override void AI()
	{
		if (!Angry)
		{
			IdleAI();
			return;
		}
		int oldDir = NPC.direction;
		NPC.TargetClosest();
		if (NPC.target < 0)
		{
			IdleAI();
			return;
		}
		Player player = Main.player[NPC.target];
		if (!Collision.CanHit(NPC, player))
		{
			NPC.direction = oldDir;
			NPC.target = -1;
			IdleAI();
			return;
		}
		if (MoveTimer > 60)
		{
			MoveTimer = 0;
			if (!Angry && Main.rand.NextBool(12))
			{
				Sleep = true;
			}
		}
		GlowValue = 1f;
		MoveTimer++;
		MoveScale = 1f;
		NPC.FaceTarget();
		UpdateMoveAndLight();
	}

	public void IdleAI()
	{
		if (Sleep)
		{
			NPC.velocity.X *= 0.5f;
			GlowValue = 0.3f;
			MoveScale = 0f;
			MoveTimer += 0.01f;
			if (MoveTimer > 6)
			{
				if (Main.rand.NextBool(600))
				{
					Sleep = false;
				}
			}
			if (MoveTimer > 12)
			{
				Sleep = false;
			}
		}
		else
		{
			GlowValue = 0.5f;
			MoveScale = 0.3f;
			if (MoveTimer < 15f)
			{
				MoveTimer += 0.25f;
			}
			else
			{
				MoveTimer++;
			}
		}
		UpdateMoveAndLight();
	}

	public void UpdateMoveAndLight()
	{
		if (Sleep)
		{
			Lighting.AddLight(NPC.Center, new Vector3(1f, 0.05f, 0.6f) * GlowValue);
			return;
		}
		if (MoveTimer > 60)
		{
			MoveTimer = Main.rand.Next(15);
			if (!Angry && Main.rand.NextBool(12))
			{
				Sleep = true;
				MoveTimer = 0;
			}
		}
		if (MoveTimer % 60 == 15)
		{
			if (NPC.target < 0)
			{
				if (Main.rand.NextBool(15))
				{
					NPC.direction *= -1;
				}
			}
			if (NPC.collideY)
			{
				NPC.spriteDirection = NPC.direction;
			}
			else
			{
				MoveTimer = 0;
			}
		}
		if (MoveTimer % 60 == 30)
		{
			NPC.velocity.X += NPC.spriteDirection * 6 * MoveScale * NPC.scale;
		}
		else
		{
			if (NPC.collideY)
			{
				NPC.velocity *= 0.95f;
			}
			else
			{
				NPC.velocity *= 0.999f;
				NPC.velocity.Y += 0.25f;
			}
			if (MoveTimer % 60 == 31 && NPC.collideX)
			{
				if (NPC.target >= 0)
				{
					NPC.velocity.Y -= MathF.Abs(NPC.oldVelocity.X) * 0.5f;
					NPC.position.Y -= 16;
					NPC.velocity.X = NPC.oldVelocity.X * 0.5f;
				}
				else
				{
					NPC.velocity.X = -NPC.oldVelocity.X * 0.5f;
					NPC.direction *= -1;
					NPC.spriteDirection = NPC.direction;
				}
			}
		}
		Lighting.AddLight(NPC.Center, new Vector3(1f, 0.05f, 0.6f) * GlowValue);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		Sleep = false;
		for (int g = 0; g < 11; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(24, 72)).RotatedByRandom(MathHelper.TwoPi);
			float dropScale = Main.rand.NextFloat(6f, 12f);
			var blood = new PurpleBombCaterpillarBloodDrop
			{
				Velocity = vel / dropScale,
				Active = true,
				Visible = true,
				Position = NPC.position + new Vector2(Main.rand.NextFloat(0, NPC.width), Main.rand.NextFloat(0, NPC.height)) + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(42, 84),
				Scale = dropScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		Angry = true;
	}

	public override void OnKill()
	{
		KillEffect();
	}

	public void KillEffect()
	{
		Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<PurpleBombCaterpillarDeath>(), NPC.damage, 3.5f);
		p0.Bottom = NPC.Bottom;
		p0.scale = NPC.scale;
		p0.direction = NPC.direction;
		p0.spriteDirection = NPC.spriteDirection;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D eyeStalks = ModAsset.PurpleBombCaterpillar.Value;
		Texture2D eyeStalks_glow = ModAsset.PurpleBombCaterpillar_glow.Value;
		Texture2D body = ModAsset.PurpleBombCaterpillar_Body.Value;
		Texture2D body_glow = ModAsset.PurpleBombCaterpillar_Body_glow.Value;
		SpriteEffects flip = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		float squzzeValue = 1f;
		float originX = 0.5f;
		if (MoveTimer % 60 is >= 15 and < 30)
		{
			squzzeValue = (MoveTimer % 60 - 15f) / 15f * 0.3f * MoveScale;
			originX -= squzzeValue * 0.5f * NPC.spriteDirection;
			squzzeValue = 1 - squzzeValue;
		}
		if (MoveTimer % 60 is >= 30 and <= 45)
		{
			float value = MoveTimer % 60 - 30f;
			squzzeValue = MathF.Sin(value * 0.5f - MathHelper.PiOver2) / (value + 3f) * 1f * MoveScale;
			originX -= squzzeValue * 0.5f * NPC.spriteDirection;
			squzzeValue = 1 - squzzeValue;
		}
		spriteBatch.Draw(eyeStalks, NPC.Bottom - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * new Vector2(0.5f, 0.95f), NPC.scale, flip, 0);
		spriteBatch.Draw(eyeStalks_glow, NPC.Bottom - Main.screenPosition, NPC.frame, new Color(1f, 0.65f, 0.25f, 0) * GlowValue, NPC.rotation, NPC.frame.Size() * new Vector2(0.5f, 0.95f), NPC.scale, flip, 0);

		spriteBatch.Draw(body, NPC.Bottom - Main.screenPosition, null, drawColor, NPC.rotation, body.Size() * new Vector2(originX, 0.95f), new Vector2(squzzeValue, 2 - squzzeValue) * NPC.scale, flip, 0);
		spriteBatch.Draw(body_glow, NPC.Bottom - Main.screenPosition, null, new Color(1f, 0.65f, 0.25f, 0) * GlowValue, NPC.rotation, body.Size() * new Vector2(originX, 0.95f), new Vector2(squzzeValue, 2 - squzzeValue) * NPC.scale, flip, 0);
		return false;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
	}
}
