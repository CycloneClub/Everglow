using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
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
	}

	public override void FindFrame(int frameHeight)
	{
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
		}
		GlowValue = 1f;
		MoveTimer++;
		MoveScale = 1f;
		NPC.FaceTarget();
		UpdateMoveAndLight();
	}

	public void IdleAI()
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
		UpdateMoveAndLight();
	}

	public void UpdateMoveAndLight()
	{
		if (MoveTimer > 60)
		{
			MoveTimer = Main.rand.Next(15);
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
		Angry = true;
	}

	public override void OnKill()
	{
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
