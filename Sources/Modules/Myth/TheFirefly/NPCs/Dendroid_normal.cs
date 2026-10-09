using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Myth.TheFirefly.NPCs;

public class Dendroid_normal : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 20;
	}

	public override void SetDefaults()
	{
		NPC.damage = 24;
		NPC.width = 40;
		NPC.height = 56;
		NPC.defense = 18;
		NPC.lifeMax = 140;
		NPC.knockBackResist = 0.4f;
		NPC.value = Item.buyPrice(0, 0, 12, 0);
		NPC.aiStyle = NPCAIStyleID.Fighter;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
		if (!fireflyBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		return 0.24f;
	}

	public override void AI()
	{
		Player player = Main.player[NPC.FindClosestPlayer()];
		if (NPC.velocity.X > 0)
			NPC.spriteDirection = 1;
		else
		{
			NPC.spriteDirection = -1;
		}
		if (NPC.collideY || NPC.collideX)
		{
			NPC.velocity.X += NPC.spriteDirection * NPC.scale * 0.3f;

			Vector2 ToPlayer = player.Center - NPC.Center;
			if (ToPlayer.X * NPC.velocity.X < 0)
			{
				if (ToPlayer.Length() > 1000 * NPC.scale || Main.rand.NextBool(240))
				{
					NPC.velocity.X *= -1;
					NPC.spriteDirection *= -1;
				}
			}
		}
	}
	public override void FindFrame(int frameHeight)
	{
		frameHeight = NPC.height;
		NPC.frameCounter++;
		float frameChangeFrequency = Math.Max(7 - Math.Abs(NPC.velocity.X) * 5, 1);
		if (NPC.collideY || NPC.collideX)
		{
			if (NPC.frame.Y < 6 * frameHeight)
			{
				NPC.frame.Y = 6 * frameHeight;
			}

			if (NPC.frameCounter > frameChangeFrequency)
			{
				NPC.frameCounter = 0;
				if (NPC.frame.Y < 19 * frameHeight)
					NPC.frame.Y += frameHeight;
				else
				{
					NPC.frame.Y = 6 * frameHeight;
				}
			}
		}
		else
		{
			NPC.frame.Y = 5 * frameHeight;
		}
	}
	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		spriteBatch.Draw(ModAsset.Dendroid_normal_glow.Value, NPC.Center - Main.screenPosition, NPC.frame, new Color(255, 255, 255, 0), NPC.rotation, new Vector2(NPC.width, NPC.height) / 2f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
	}

	public override void OnKill()
	{
		for (int i = 0; i < 18; i++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Clentaminator_Blue, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 0, default, 0.6f);
		}
		for (int i = 0; i < 6; i++)
		{
			int index = Dust.NewDust(NPC.position - new Vector2(8), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283);
			Main.dust[index].noGravity = true;
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MothScaleDust>(), 1, 1, 1));
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.scale = Main.rand.NextFloat(0.83f, 1.17f);
	}
}
