using Everglow.Commons.Mechanics.Miscs;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

[NoGameModeScale]
public class Guard_Slimy : ModNPC
{
	public int MyYggdrasilTownGuardWhoAmI = -1;
	public Vector2 OldPosition;
	public bool Jumping = false;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 4;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 22;
		NPC.height = 22;
		NPC.lifeMax = 250;
		NPC.damage = 30;
		NPC.defense = 6;
		NPC.friendly = true;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.9f;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.hide = false;
		NPC.noGravity = false;
		NPC.scale = 1f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (MyYggdrasilTownGuardWhoAmI == -1)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc.active && npc.type == ModContent.NPCType<Guard_of_YggdrasilTown>())
				{
					if ((npc.Center - NPC.Center).Length() < 2000)
					{
						Guard_of_YggdrasilTown goYT = npc.ModNPC as Guard_of_YggdrasilTown;
						if (goYT != null)
						{
							if (goYT.MySlimyWhoAmI == NPC.whoAmI)
							{
								MyYggdrasilTownGuardWhoAmI = npc.whoAmI;
								NPC.frame = new Rectangle(0, 0, 22, 32);
								return;
							}
						}
					}
				}
			}
		}
		else
		{
			NPC.active = false;
		}
	}

	public override void AI()
	{
		if (MyYggdrasilTownGuardWhoAmI == -1)
		{
			NPC.active = false;
			return;
		}
		NPC owner = Main.npc[MyYggdrasilTownGuardWhoAmI];
		Guard_of_YggdrasilTown goYT = owner.ModNPC as Guard_of_YggdrasilTown;
		if (!owner.active || goYT == null || goYT.MySlimyWhoAmI != NPC.whoAmI)
		{
			NPC.active = false;
			return;
		}
		Vector2 aimPos = owner.Center + new Vector2(-60 * owner.spriteDirection, 0);
		float distance = (aimPos - NPC.Center).Length();
		if (distance > 400)
		{
			NPC.Center = aimPos;
		}
		if (distance > 40 && !Jumping && NPC.ai[0] < 1)
		{
			Jumping = true;
			NPC.ai[0] = 0;
			NPC.velocity += new Vector2(Math.Sign((aimPos - NPC.Center).X) * Main.rand.NextFloat(1, 2.47f), -Main.rand.NextFloat(5, 7f));
			NPC.position += NPC.velocity;
		}
		if (Main.rand.NextBool(150) && !Jumping && NPC.ai[0] < 1)
		{
			Jumping = true;
			NPC.ai[0] = 0;
			NPC.velocity += new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), -Main.rand.NextFloat(2, 5f));
			NPC.position += NPC.velocity;
		}
		if (NPC.collideY && Jumping && Math.Abs(OldPosition.Y - NPC.position.Y) <= 0.01f)
		{
			Jumping = false;
			NPC.ai[0] = 5;
			NPC.velocity *= 0;
		}
		if (NPC.velocity.X > 0)
		{
			NPC.spriteDirection = -1;
		}
		if (NPC.velocity.X < 0)
		{
			NPC.spriteDirection = 1;
		}
		if (Jumping)
		{
			NPC.ai[0]++;
			if (NPC.ai[0] > 10)
			{
				NPC.frame = new Rectangle(0, 64, 22, 32);
				NPC.ai[0] = 11;
			}
			else
			{
				NPC.frame = new Rectangle(0, 32, 22, 32);
			}
		}
		else
		{
			NPC.ai[0]--;
			if (NPC.ai[0] > 0)
			{
				NPC.frame = new Rectangle(0, 96, 22, 32);
			}
			else
			{
				NPC.ai[0] = 0;
				NPC.frame = new Rectangle(0, 0, 22, 32);
			}

			// Face to my owner.
			if (Main.rand.NextBool(60) && NPC.ai[0] < 1)
			{
				if ((aimPos - NPC.Center).X > 0)
				{
					NPC.spriteDirection = 1;
				}
				if ((aimPos - NPC.Center).X < 0)
				{
					NPC.spriteDirection = -1;
				}
			}
		}
		OldPosition = NPC.position;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texture = ModAsset.Guard_Slimy.Value;
		SpriteEffects spriteEffect = SpriteEffects.None;
		if (NPC.spriteDirection == -1)
		{
			spriteEffect = SpriteEffects.FlipHorizontally;
		}
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, spriteEffect, 0);
		return false;
	}

	public override void OnKill()
	{
	}
}
