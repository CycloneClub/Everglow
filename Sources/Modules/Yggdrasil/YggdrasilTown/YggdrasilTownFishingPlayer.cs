using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing.Trash;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown;

public class YggdrasilTownFishingPlayer : ModPlayer
{
	public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
	{
		if (!YggdrasilWorld.InYggdrasil || attempt.inLava || attempt.inHoney || attempt.crate || npcSpawn > 0
			|| itemDrop <= ItemID.None || itemDrop >= ItemID.Count || itemDrop == attempt.questFish
			|| attempt.veryrare || attempt.legendary)
		{
			return;
		}

		bool inLampWood = Player.InModBiome<LampWoodForest>();
		bool inTwilight = Player.InModBiome<TwilightForsetAndRelic>();
		if (!inLampWood && !inTwilight && !Player.InModBiome<YggdrasilTownBiome>())
		{
			return;
		}

		// 森林可能与城镇背景群系重叠，优先采用森林渔获。
		if (itemDrop is ItemID.OldShoe or ItemID.TinCan or ItemID.Seaweed)
		{
			if (inLampWood)
			{
				itemDrop = Main.rand.NextBool() ? ModContent.ItemType<WormEatenLampfruitHusk>() : ModContent.ItemType<TangledOldRope>();
			}
			else if (inTwilight)
			{
				itemDrop = Main.rand.NextBool() ? ModContent.ItemType<FadedRitualCloth>() : ModContent.ItemType<LeakyCrystalHusk>();
			}
			else
			{
				itemDrop = Main.rand.NextBool() ? ModContent.ItemType<CloggedPressureGauge>() : ModContent.ItemType<SoggyMenu>();
			}
		}
		else if (attempt.rare)
		{
			if (inLampWood)
			{
				itemDrop = Main.rand.NextBool() ? ModContent.ItemType<GuidyFish>() : ModContent.ItemType<DimmyFish>();
			}
			else if (inTwilight)
			{
				itemDrop = ModContent.ItemType<Duskfin>();
			}
			else
			{
				return;
			}
		}
		else if (attempt.uncommon)
		{
			itemDrop = ModContent.ItemType<Filefish>();
		}
		else if (attempt.common)
		{
			itemDrop = ModContent.ItemType<GrassCarp>();
		}
		else
		{
			return;
		}

		// 让原版声呐根据最终物品 ID 显示名称。
		sonar.Text = null;
	}
}
