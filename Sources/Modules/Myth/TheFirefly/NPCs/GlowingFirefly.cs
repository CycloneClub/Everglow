using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Items.Materials;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Myth.TheFirefly.NPCs;

public class GlowingFirefly : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
	}

	public override void SetDefaults()
	{
		NPC.damage = 0;
		NPC.width = 20;
		NPC.height = 20;
		NPC.defense = 0;
		NPC.lifeMax = 5;
		NPC.knockBackResist = 1f;
		NPC.value = Item.buyPrice(0, 0, 0, 0);
		NPC.color = new Color(0, 0, 0, 0);
		NPC.alpha = 0;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.catchItem = ModContent.ItemType<Items.GlowingFirefly>();
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
		if (!fireflyBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		return 0.3f;
	}

	public override void AI()
	{
		Player player = Main.player[NPC.FindClosestPlayer()];
		if (NPC.ai[1] > 0)
		{
			if (NPC.ai[0] < 4)
			{
				NPC.ai[0] += 0.18f;
			}
			else
			{
				NPC.ai[0] += 0.5f;
			}

			if (NPC.ai[0] >= 8f)
			{
				NPC.ai[0] = 4f;
			}

			NPC.velocity.Y = 0f;
			UpdateMove();
		}
		else
		{
			if ((player.Center - NPC.Center).Length() < 80f || NPC.life != NPC.lifeMax)
			{
				NPC.ai[1] = 1f;
			}

			foreach (NPC same in Main.npc)
			{
				if (same.type == NPC.type)
				{
					if ((same.Center - NPC.Center).Length() < 60)
					{
						if (same.ai[1] > 0)
						{
							NPC.ai[1] = 1;
							NPC.ai[2] = Main.rand.Next(100);
							aimPos = new Vector2(0, Main.rand.NextFloat(12f, 220f)).RotatedByRandom(6.283) + NPC.Center;
							break;
						}
					}
				}
			}
		}
	}

	private Vector2 aimPos = Vector2.Zero;

	private void UpdateMove()
	{
		NPC.ai[2] += 1;
		if (NPC.ai[2] % 40 == 0)
		{
			Vector2 vNext = new Vector2(0, Main.rand.NextFloat(12f, 220f)).RotatedByRandom(6.283) + NPC.Center + Vector2.Normalize(NPC.Center - Main.player[Player.FindClosest(NPC.Center, 0, 0)].Center) * 6 + new Vector2(0, -6);
			while (!Collision.CanHit(NPC.Center, 0, 0, vNext, 0, 0) || Main.tile[(int)(vNext.X / 16), (int)(vNext.Y / 16)].LiquidAmount != 0)
			{
				vNext = new Vector2(0, Main.rand.NextFloat(12f, 220f)).RotatedByRandom(6.283) + NPC.Center + Vector2.Normalize(NPC.Center - Main.player[Player.FindClosest(NPC.Center, 0, 0)].Center) * 6 + new Vector2(0, -6);
			}
			aimPos = vNext;
		}
		if ((NPC.Center - aimPos).Length() >= 20)
		{
			NPC.velocity = Vector2.Normalize(aimPos - NPC.Center) * 1f;
		}
		else
		{
			NPC.velocity *= 0;
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteEffects effects = SpriteEffects.None;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}

		Texture2D tx = ModAsset.NPCs_GlowingFirefly.Value;
		Texture2D tg = ModAsset.GlowingFireflyGlow.Value;
		var vector = new Vector2(tx.Width / 2f, tx.Height / (float)Main.npcFrameCount[NPC.type] / 2f);

		Color color0 = Lighting.GetColor((int)(NPC.Center.X / 16d), (int)(NPC.Center.Y / 16d));
		var color1 = new Color(255, 255, 255, 0);
		Main.spriteBatch.Draw(tx, NPC.Center - Main.screenPosition, new Rectangle(0, 50 * (int)NPC.ai[0], 46, 50), color0, NPC.rotation, vector, NPC.scale, effects, 0f);
		Main.spriteBatch.Draw(tg, NPC.Center - Main.screenPosition, new Rectangle(0, 50 * (int)NPC.ai[0], 46, 50), color1, NPC.rotation, vector, NPC.scale, effects, 0f);

		return false;
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

	public override bool? CanBeCaughtBy(Item item, Player player)
	{
		return true;
	}

	public override void OnCaughtBy(Player player, Item item, bool failed)
	{
		Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, 0, 0, ModContent.ItemType<Items.GlowingFirefly>(), 1);
		NPC.active = false;
	}
}
