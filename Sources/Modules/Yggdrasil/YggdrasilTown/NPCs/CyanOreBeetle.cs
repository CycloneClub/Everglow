using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class CyanOreBeetle : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 5;
		NPCSpawnManager.RegisterNPC(Type);
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 54;
		NPC.height = 42;
		NPC.lifeMax = 50;
		NPC.damage = 16;
		NPC.defense = 6;
		NPC.friendly = false;
		NPC.aiStyle = 3;
		NPC.knockBackResist = 0.75f;
		NPC.value = 30;
		NPC.HitSound = SoundID.NPCHit4;
		NPC.DeathSound = SoundID.NPCDeath4;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
		if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}

		return 3f;
	}

	public void Stop()
	{
		NPC.frame = new Rectangle(0, NPC.height * 4, NPC.width, NPC.height);
		NPC.velocity.X *= 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		int scale = Main.rand.Next(1, 100);
		if (scale <= 50)
		{
			NPC.scale = 1;
		}
		else if (scale > 50 && scale <= 85)
		{
			NPC.scale = 1.25f;
		}
		else
		{
			NPC.scale = 1.5f;
		}
		NPC.lifeMax = (int)(NPC.lifeMax * NPC.scale);
		NPC.life = NPC.lifeMax;
	}

	public override void AI()
	{
		NPC.TargetClosest();
		Player player = Main.player[NPC.target];
		if (NPC.Center.X > Main.maxTilesX * 16 - 320 || NPC.Center.X < 320)
		{
			NPC.active = false;
		}
		if (NPC.Center.Y > Main.maxTilesY * 16 - 320 || NPC.Center.Y < 320)
		{
			NPC.active = false;
		}
		if (Main.rand.NextBool(840) || (NPC.Center - player.Center).Length() > 400)
		{
			NPC.spriteDirection = Math.Sign((player.Center - NPC.Center).X);
		}

		NPC.localAI[0] += 1;
		bool HasNearTile = false;
		Vector2 Total = Vector2.Zero;
		for (int x = 0; x < 10; x++)
		{
			Vector2 v0 = NPC.Center;
			Vector2 v1 = new Vector2(0, 16).RotatedBy(x / 5d * Math.PI);
			if (Collision.SolidCollision(v0 + v1, 1, 1))
			{
				HasNearTile = true;
				Total += v1 / 12f;
			}
			else
			{
				Total -= v1 / 12f;
			}
		}
		for (int x = 0; x < 20; x++)
		{
			Vector2 v0 = NPC.Center;
			Vector2 v1 = new Vector2(0, 32).RotatedBy(x / 10d * Math.PI);
			if (Collision.SolidCollision(v0 + v1, 1, 1))
			{
				HasNearTile = true;
				Total += v1 / 48f;
			}
			else
			{
				Total -= v1 / 48f;
			}
		}
		if (HasNearTile)
		{
			if (NPC.localAI[0] % Math.Clamp((int)(12 - NPC.velocity.Length()), 1, 12) == 0)
			{
				if (NPC.frame.Y < 84)
				{
					NPC.frame.Y += 42;
				}
				else
				{
					NPC.frame.Y = 0;
				}
			}
			if (Math.Abs(NPC.velocity.X) < 4 * NPC.scale)
			{
				NPC.velocity.X += NPC.spriteDirection * NPC.scale * 0.2f;
			}

			NPC.rotation = (float)(Math.Atan2(Total.Y, Total.X) + Math.PI / 2d * 3);
			if (NPC.velocity == NPC.oldVelocity)
			{
				NPC.velocity += new Vector2(0, -3).RotatedBy(NPC.rotation);
			}
		}
		else
		{
			NPC.rotation = 0;
			NPC.frame.Y = 126;
		}
	}

	public override void OnKill()
	{
		Vector2 GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanOreBeetleHead").Type, NPC.scale);
		for (int f = 0; f < 3; f++)
		{
			GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanOreBeetleFoot0").Type, NPC.scale);
			GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanOreBeetleFoot1").Type, NPC.scale);
		}
		GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanVineOre2").Type, NPC.scale * Main.rand.NextFloat(0.85f, 1.15f));
		GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanVineOre3").Type, NPC.scale * Main.rand.NextFloat(0.85f, 1.15f));
		GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanVineOre4").Type, NPC.scale * Main.rand.NextFloat(0.85f, 1.15f));
		GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanVineOre5").Type, NPC.scale * Main.rand.NextFloat(0.85f, 1.15f));
		GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanVineOre6").Type, NPC.scale * Main.rand.NextFloat(0.85f, 1.15f));
		GoreVelocity = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
		Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), GoreVelocity, ModContent.Find<ModGore>("Everglow/CyanVineOre7").Type, NPC.scale * Main.rand.NextFloat(0.85f, 1.15f));
		for (int f = 0; f < 13; f++)
		{
			Vector2 DustVelocity = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(6.28d);
			Dust.NewDust(NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), 0, 0, ModContent.DustType<Dusts.CyanVine>(), DustVelocity.X, DustVelocity.Y);
			DustVelocity = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(6.28d);
			Dust.NewDust(NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), 0, 0, DustID.WoodFurniture, DustVelocity.X, DustVelocity.Y);
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CyanVineOre>(), 1, 1, 2));
	}
}
